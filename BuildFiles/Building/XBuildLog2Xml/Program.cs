using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace XBuildLog2Xml
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFileName = "msbuild.log";

            if (Debugger.IsAttached)
            {
                logFileName = "E:\\Temp\\" + logFileName;
            }

            Dictionary<string,bool> errorDupeCheck = new Dictionary<string, bool>();
            List<ProjectResult> projectResults = new List<ProjectResult>();
            int sumWarningsCode = 0;
            int sumWarningsEnvironment = 0;
            int sumErrors = 0;

            ProjectResult currentProject = new ProjectResult
            {
                ProjectName = "Solution"
            };

            const string projectStartRegex = "Building target \"Compile\" in project \"[a-zA-Z/]+/(?<projectName>[A-Za-z0-9\\-]+)\\.csproj\"";
            const string messageRegex = "(?<fileName>[A-Za-z\\-_0-9\\.]+).cs\\((?<lineNumber>[0-9]+),[0-9]+\\): (?<messageType>[a-z]+) CS(?<messageCode>[0-9]{4}): (?<messageText>.+)";

            Regex regexProject = new Regex (projectStartRegex, RegexOptions.Compiled);
            Regex regexMessage = new Regex (messageRegex, RegexOptions.Compiled);


            // first, read original log into parsed data


            using (TextReader reader = new StreamReader (logFileName, Encoding.UTF8, true))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    // 'Building target "Compile" in ".../.../[A-Za-z]+.csproj"'
                    // 'Database-ExchangeRates.cs(59,34): warning CS0219: The variable `testDate' is assigned but its value is never used'

                    line = line.Trim();

                    Match matchProject = regexProject.Match (line);
                    if (matchProject.Success)
                    {
                        // Store old project

                        projectResults.Add (currentProject);

                        currentProject = new ProjectResult
                        {
                            ProjectName = "Project " + matchProject.Groups["projectName"].Value
                        };
                    }

                    Match matchMessage = regexMessage.Match (line);

                    if (matchMessage.Success)
                    {
                        BuildMessage newMessage = new BuildMessage
                        {
                            Code = matchMessage.Groups["messageCode"].Value,
                            File = matchMessage.Groups["fileName"].Value,
                            Line = Int32.Parse (matchMessage.Groups["lineNumber"].Value),
                            Description = matchMessage.Groups["messageText"].Value
                        };

                        if (matchMessage.Groups["messageType"].Value.ToLowerInvariant() == "warning")
                        {
                            sumWarningsCode++;
                            newMessage.Type = MessageType.CodeWarning;
                            currentProject.CodeWarnings.Add (newMessage);
                        }
                        else
                        {
                            string errorKey = newMessage.Description + newMessage.File + newMessage.Line.ToString();

                            if (!errorDupeCheck.ContainsKey (errorKey))
                            {
                                // Errors are duplicated in the MSBuild summary, so need a dupecheck

                                sumErrors++;
                                newMessage.Type = MessageType.Error;
                                currentProject.Errors.Add(newMessage);
                                errorDupeCheck[errorKey] = true;
                            }
                        }
                    }
                    else if (line.Contains ("error"))
                    {
                        currentProject.Errors.Add (new BuildMessage
                        {
                            Type = MessageType.Error,
                            Description = line
                        });
                        sumErrors++;
                    }
                    else if (line.Contains ("warning"))
                    {
                        currentProject.EnvironmentWarnings.Add(new BuildMessage
                        {
                            Type = MessageType.EnvironmentWarning,
                            Description = line
                        });
                        sumWarningsEnvironment++;
                    }

                    line = reader.ReadLine();
                }

                projectResults.Add (currentProject);
            }

            // and once we have it in data format, write it as an xml document

            using (XmlWriter xmlWriter = new XmlTextWriter ("xbuild.xml", Encoding.UTF8))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement ("xbuild");
                xmlWriter.WriteAttributeString ("environment_warning_count", sumWarningsEnvironment.ToString());
                xmlWriter.WriteAttributeString ("code_warning_count", sumWarningsCode.ToString());
                xmlWriter.WriteAttributeString ("error_count", sumErrors.ToString());

                foreach (ProjectResult result in projectResults)
                {
                    if (result.WarningsTotal + result.ErrorsTotal > 0)
                    {
                        if (result.ErrorsTotal > 0 && sumErrors > 0) // write only errors if there are any errors
                        {
                            xmlWriter.WriteStartElement ("project");
                            xmlWriter.WriteAttributeString ("name", result.ProjectName);

                            WriteXmlBuildMessages (xmlWriter, result.Errors);

                            xmlWriter.WriteEndElement();
                        }
                        else if (result.WarningsTotal > 0 && sumErrors == 0)  // if there are no errors in ANY project, write the warnings for all projects
                        {
                            xmlWriter.WriteStartElement("project");
                            xmlWriter.WriteAttributeString("name", result.ProjectName);

                            WriteXmlBuildMessages(xmlWriter, result.CodeWarnings);
                            WriteXmlBuildMessages(xmlWriter, result.EnvironmentWarnings);

                            xmlWriter.WriteEndElement();
                        }
                    }
                }

                xmlWriter.WriteEndDocument();
            }



        }

        private static void WriteXmlBuildMessages (XmlWriter xmlWriter, List<BuildMessage> messages)
        {
            foreach (BuildMessage message in messages)
            {
                WriteXmlBuildMessage (xmlWriter, message);
            }
        }


        private static void WriteXmlBuildMessage (XmlWriter xmlWriter, BuildMessage message)
        {
            if (message.Type == MessageType.Error)
            {
                xmlWriter.WriteStartElement ("error");
            }
            else
            {
                xmlWriter.WriteStartElement("warning");
            }

            xmlWriter.WriteAttributeString ("message", message.Description);
            xmlWriter.WriteAttributeString ("code", message.Code); // may be null or empty

            if (message.Line > 0 || !string.IsNullOrEmpty (message.File))
            {
                xmlWriter.WriteAttributeString ("name", message.File);
                xmlWriter.WriteAttributeString ("line", message.Line.ToString());
            }

            xmlWriter.WriteEndElement();
        }

        public enum MessageType
        {
            Unknown = 0,
            Error,
            CodeWarning,
            EnvironmentWarning
        };

        public class ProjectResult
        {
            public ProjectResult()
            {
                Errors = new List<BuildMessage>();
                CodeWarnings = new List<BuildMessage>();
                EnvironmentWarnings = new List<BuildMessage>();
            }

            public string ProjectName { get; set; }

            public int ErrorsTotal
            {
                get { return Errors.Count; }
            }

            public int WarningsTotal
            {
                get { return CodeWarningsTotal + EnvironmentWarningsTotal; }
            }

            public int CodeWarningsTotal
            {
                get { return CodeWarnings.Count; }
            }

            public int EnvironmentWarningsTotal
            {
                get { return EnvironmentWarnings.Count; }
            }

            public List<BuildMessage> Errors;
            public List<BuildMessage> CodeWarnings;
            public List<BuildMessage> EnvironmentWarnings;
        }

        public class BuildMessage
        {
            public MessageType Type { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public string Folder { get; set; }
            public string File { get; set; }
            public int Line { get; set; }
        }
    }
}
