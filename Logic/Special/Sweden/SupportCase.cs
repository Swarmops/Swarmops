using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Logic.Special.Sweden
{
    public class SupportCase : IHasIdentity
    {
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

            int indexOfBracket = email.IndexOf('<');

            if (indexOfBracket >= 0)
            {
                this.Email = email.Substring(indexOfBracket + 1, email.IndexOf('>') - indexOfBracket - 1);
            }
        }

        public readonly int SupportCaseId;
        public readonly string Title;
        public readonly bool Open;
        public readonly string Email;
        public readonly DateTime DateTimeClosed;

        public int Identity
        {
            get { return SupportCaseId; }
        }

        public void CloseWithComment (string comment)
        {
            SupportDatabase.CloseWithComment(this.Identity, comment);
        }
    }
}