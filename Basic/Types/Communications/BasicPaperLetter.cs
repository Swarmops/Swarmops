using System;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Communications
{
    public class BasicPaperLetter : IHasIdentity
    {
        public BasicPaperLetter (int paperLetterId, int organizationId, string fromName, string replyAddress,
            DateTime receivedDate, int toPersonId, RoleType toPersonInRole, bool personal, int uploadedByPersonId,
            DateTime uploadedDateTime)
        {
            this.PaperLetterId = paperLetterId;
            this.OrganizationId = organizationId;
            this.FromName = fromName;
            this.ReplyAddress = replyAddress;
            this.ReceivedDate = receivedDate;
            this.ToPersonId = toPersonId;
            this.ToPersonInRole = toPersonInRole;
            this.Personal = personal;
            this.UploadedByPersonId = uploadedByPersonId;
            this.UploadedDateTime = uploadedDateTime;
        }

        public BasicPaperLetter (BasicPaperLetter original)
            : this (original.PaperLetterId, original.OrganizationId, original.FromName, original.ReplyAddress,
                original.ReceivedDate, original.ToPersonId, original.ToPersonInRole, original.Personal,
                original.UploadedByPersonId,
                original.UploadedDateTime)
        {
            // empty copy ctor
        }

        public int PaperLetterId { get; private set; }
        public int OrganizationId { get; protected set; }
        public string FromName { get; protected set; }
        public string ReplyAddress { get; protected set; }
        public int ToPersonId { get; protected set; }
        public RoleType ToPersonInRole { get; protected set; }
        public int GeographyId { get; protected set; }
        public bool Personal { get; protected set; }
        public DateTime ReceivedDate { get; protected set; }
        public int UploadedByPersonId { get; private set; }
        public DateTime UploadedDateTime { get; private set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.PaperLetterId; }
        }

        #endregion
    }
}