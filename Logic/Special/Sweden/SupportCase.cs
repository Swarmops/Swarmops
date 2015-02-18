using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Logic.Special.Sweden
{
    public class SupportCase : IHasIdentity
    {
        public readonly DateTime DateTimeClosed;
        public readonly string Email;
        public readonly bool Open;
        public readonly int SupportCaseId;
        public readonly string Title;

        public SupportCase (int identity, string title)
        {
            this.SupportCaseId = identity;
            this.Title = title;
            this.Open = false;
            this.DateTimeClosed = DateTime.MinValue;
            this.Email = string.Empty;
        }

        public SupportCase (int identity, string title, string email)
        {
            this.SupportCaseId = identity;
            this.Title = title;
            this.Email = email;

            int indexOfBracket = email.IndexOf ('<');

            if (indexOfBracket >= 0)
            {
                this.Email = email.Substring (indexOfBracket + 1, email.IndexOf ('>') - indexOfBracket - 1);
            }
        }

        public int Identity
        {
            get { return this.SupportCaseId; }
        }

        public void CloseWithComment (string comment)
        {
            SupportDatabase.CloseWithComment (Identity, comment);
        }
    }
}