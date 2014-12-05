using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicPaperLetter : IHasIdentity
    {
        public BasicPaperLetter (int paperLetterId, int organizationId, string fromName, string replyAddress,
            DateTime receivedDate, int toPersonId, RoleType toPersonInRole, bool personal, int uploadedByPersonId,
            DateTime uploadedDateTime)
        {
            PaperLetterId = paperLetterId;
            OrganizationId = organizationId;
            FromName = fromName;
            ReplyAddress = replyAddress;
            ReceivedDate = receivedDate;
            ToPersonId = toPersonId;
            ToPersonInRole = toPersonInRole;
            Personal = personal;
            UploadedByPersonId = uploadedByPersonId;
            UploadedDateTime = uploadedDateTime;
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
            get { return PaperLetterId; }
        }

        #endregion
    }
}