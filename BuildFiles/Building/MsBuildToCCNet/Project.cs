// Copyright © 2006 by Christian Rodemeyer (mailto:christian@atombrenner.de)

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;

namespace Rodemeyer.MsBuildToCCNet
{
    internal class Project
    {
        internal Project(string file)
        {
            File = file;
            
            errors   = new List<Error>();
            warnings = new List<Warning>();
            messages = new List<Message>();
        }

        public readonly string File;

        public IEnumerable<Error> Errors
        {
            get { return errors; }
        }
        private List<Error> errors;

        public IEnumerable<Warning> Warnings
        {
            get { return warnings; }
        }
        private List<Warning> warnings;

        public IEnumerable<Message> Messages
        {
            get { return messages; }
        }
        private List<Message> messages;

        public int ErrorCount
        {
            get { return errors.Count; }
        }

        public int WarningCount
        {
            get { return warnings.Count; }
        }

        public int MessageCount
        {
            get { return messages.Count; }
        }

        public void Add(Error e)
        {
            errors.Add(e);
        }

        public void Add(Warning w)
        {
            warnings.Add(w);
        }

        public void Add(Message m)
        {
            messages.Add(m);
        }

    }

    internal class ErrorOrWarningBase
    {
        protected ErrorOrWarningBase(string code, string text, string file, int line, int column)
        {
            Code = code;
            Text = text;
            File = file == "" ? null : file;
            Line = line;
            Column = column;
        }

        public readonly string Code;
        public readonly string Text;
        public readonly string File;
        public readonly int Line;
        public readonly int Column;
    }

    internal class Warning: ErrorOrWarningBase
    {
        public Warning(BuildWarningEventArgs e) 
            : base (e.Code, e.Message, e.File, e.LineNumber, e.ColumnNumber)
        {}
    }

    internal class Error : ErrorOrWarningBase 
    {
        public Error(BuildErrorEventArgs e)
            : base(e.Code, e.Message, e.File, e.LineNumber, e.ColumnNumber)
        {}
    }

    internal class Message
    {
        public Message(BuildMessageEventArgs e)
        {
            Importance = e.Importance;
            Text = e.Message;
        }

        public readonly string Text;
        public readonly MessageImportance Importance;
    }
}
