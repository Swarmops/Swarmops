// Copyright © 2006 by Christian Rodemeyer (mailto:christian@atombrenner.de)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Collections;

namespace Rodemeyer.MsBuildToCCNet
{
    /// <summary>
    /// Implements a custom MSBuild logger for integration into CruiseControl.NET
    /// The following MSBuild verbosity options (/v:) are supported
    /// - Quiet      => errors, warnings and number of projects build
    /// - Minimal    => errors, warnings, one entry for every project
    /// - Normal     => like Minimal plus messages with high importance 
    /// - Detailed   => like Normal plus messages with normal importance
    /// - Diagnostic => like Detailed plus messages with low importance 
    /// </summary>
    public class MsBuildToCCNetLogger : Logger
    {
        public MsBuildToCCNetLogger()
        { }

        private string logfile;
        private string commonPrefix;

        private string currentSolution = null;

        private int commonPrefixLength;
        MessageImportance loglevel;

        private Dictionary<string, Project> file_to_project = new Dictionary<string, Project>();
        private Stack<Project> project_stack = new Stack<Project>();
        private List<Project> projects = new List<Project>();
        private Project current_project;

        /// <summary>
        /// called by MSBuild just before starting the build
        /// </summary>
        /// <param name="eventSource"></param>
        public override void Initialize(IEventSource eventSource)
        {
            if (base.Parameters != null)
                this.logfile = base.Parameters.Split(';')[0]; // ignore all parameters but the first (which is the name of the file to log to)
            else
                this.logfile = "msbuild-output.xml"; // default, in case we are not startet from a ccnet task
           
            this.commonPrefix = Environment.CurrentDirectory;
            this.commonPrefixLength = commonPrefix.Length + 1;

            current_project = new Project("MSBuild");
            projects.Add(current_project);
            project_stack.Push(current_project);

            if (this.Verbosity > LoggerVerbosity.Minimal)
            {
                eventSource.MessageRaised += new BuildMessageEventHandler(OnMessageRaised);
                switch (Verbosity)
                {
                    case LoggerVerbosity.Normal: loglevel = MessageImportance.High; break;
                    case LoggerVerbosity.Detailed: loglevel = MessageImportance.Normal; break;
                    default: loglevel = MessageImportance.Low; break;
                }
            }
            eventSource.WarningRaised += new BuildWarningEventHandler(OnWarningRaised);
            eventSource.ErrorRaised += new BuildErrorEventHandler(OnErrorRaised);
            eventSource.ProjectStarted += new ProjectStartedEventHandler(OnProjectStarted);
            eventSource.ProjectFinished += new ProjectFinishedEventHandler(OnProjectFinished);
        }

        /// <summary>
        /// called by MSBuild after the build has finished
        /// </summary>
        public override void Shutdown()
        {
            XmlTextWriter w = new XmlTextWriter(logfile, System.Text.Encoding.UTF8);
            w.Formatting = this.Verbosity > LoggerVerbosity.Quiet ? Formatting.Indented : Formatting.None;
            WriteLog(w);
            w.Flush();
            w.Close();
        }

        /// <summary>
        /// Needed for sorting the project list by number of warnings
        /// </summary>
        class WarningComparer : IComparer<Project>
        {
            public int Compare(Project x, Project y)
            {
                return y.WarningCount - x.WarningCount;
            }
        }

        /// <summary>
        /// writes the in memory gathered information into the xml log file
        /// </summary>
        /// <param name="w"></param>
        private void WriteLog(XmlWriter w)
        {
            w.WriteStartDocument();
            w.WriteStartElement("msbuild");
            if (currentSolution != null)
            {
                w.WriteAttributeString("solution_name", Path.GetFileName(currentSolution));
                w.WriteAttributeString("solution_dir", Path.GetDirectoryName(currentSolution));
            }
            w.WriteAttributeString("project_count", XmlConvert.ToString(projects.Count));

            int errorCount = 0;
            int warningCount = 0;
            foreach (Project p in projects)
            {
                errorCount += p.ErrorCount;
                warningCount += p.WarningCount;
            }
            w.WriteAttributeString("warning_count", XmlConvert.ToString(warningCount));
            w.WriteAttributeString("error_count", XmlConvert.ToString(errorCount));
            buildHasErrors = errorCount > 0;
            if (!buildHasErrors) // Sort after WarningCount
            {
                projects.Sort(new WarningComparer());
            }
            foreach (Project p in projects)
            {
                if (Verbosity > LoggerVerbosity.Quiet || p.ErrorCount > 0 || p.WarningCount > 0)
                {
                    WriteProject(w, p);
                }
            }
            w.WriteEndElement();
            w.WriteEndDocument();
        }

        private bool buildHasErrors;

        /// <summary>
        /// removes the working folder from the filename
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string RemoveCommonPrefix(string file)
        {
            if (file.StartsWith(commonPrefix))
                return file.Substring(commonPrefixLength, file.Length - commonPrefixLength);
            else
                return file;
        }

        private void WriteProject(XmlWriter w, Project p)
        {
            w.WriteStartElement("project");
            string file = RemoveCommonPrefix(p.File);
            w.WriteAttributeString("dir", Path.GetDirectoryName(file));
            w.WriteAttributeString("name", Path.GetFileName(file));
            WriteErrorsOrWarnings(w, "error", p.Errors);
            if (!buildHasErrors) WriteErrorsOrWarnings(w, "warning", p.Warnings);
            WriteMessages(w, p.Messages);
            w.WriteEndElement();
        }

        private void WriteErrorsOrWarnings(XmlWriter w, string type, IEnumerable list)
        {
            foreach (ErrorOrWarningBase ew in list)
            {
                w.WriteStartElement(type);
                w.WriteAttributeString("code", ew.Code);
                w.WriteAttributeString("message", ew.Text);
                if (ew.File != null)
                {
                    w.WriteAttributeString("dir", Path.GetDirectoryName(ew.File));
                    w.WriteAttributeString("name", Path.GetFileName(ew.File));
                    w.WriteAttributeString("pos", "(" + XmlConvert.ToString(ew.Line) + ", " + XmlConvert.ToString(ew.Column) + ")");
                }
                w.WriteEndElement();
            }
        }

        private void WriteMessages(XmlWriter w, IEnumerable<Message> list)
        {
            foreach (Message m in list)
            {
                w.WriteStartElement("message");
                w.WriteAttributeString("importance", m.Importance.ToString());
                w.WriteString(m.Text);
                w.WriteEndElement();
            }
        }

        private void OnProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            if ( (currentSolution == null || currentSolution.EndsWith(".csproj")) && e.ProjectFile.EndsWith(".sln"))
            {
                currentSolution = RemoveCommonPrefix(e.ProjectFile);
            }

            if (currentSolution == null && e.ProjectFile.EndsWith(".csproj"))
            {
                currentSolution = RemoveCommonPrefix(e.ProjectFile);
            }

            if (!file_to_project.TryGetValue(e.ProjectFile, out this.current_project))
            {
                current_project = new Project(e.ProjectFile);
                file_to_project.Add(e.ProjectFile, current_project);
                projects.Add(current_project);
            }
            project_stack.Push(current_project);
        }

        void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            project_stack.Pop();
            current_project = (project_stack.Count == 0) ? null : project_stack.Peek();
        }

        private void OnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            current_project.Add(new Warning(e));
        }

        private void OnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            current_project.Add(new Error(e));
        }

        private void OnMessageRaised(object sender, BuildMessageEventArgs e)
        {
            if (e.Importance <= loglevel)
            {
                current_project.Add(new Message(e));
            }
        }

    }

}
