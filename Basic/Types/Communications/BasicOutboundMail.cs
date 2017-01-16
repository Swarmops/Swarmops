using System;
using Swarmops.Common;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Communications
{
    public class BasicOutboundMail : IHasIdentity
    {
        #region Construction and Creation

        // =============================
        //   CONSTRUCTION AND CREATION
        // =============================


        /// <summary>
        ///     Basic constructor. This is for database use and NOT for public construction.
        /// </summary>
        public BasicOutboundMail (int outboundMailId, MailAuthorType authorType, int authorPersonId, string title,
            string body, int mailPriority, int mailTypeId, int organizationId,
            int geographyId,
            DateTime createdDateTime, DateTime releaseDateTime, bool readyForPickup, bool resolved,
            bool processed,
            DateTime resolvedDateTime, DateTime startProcessDateTime, DateTime endProcessDateTime,
            int recipientCount, int recipientsSuccess, int recipientsFail)
        {
            this.outboundMailId = outboundMailId;
            this.authorType = authorType;
            this.authorPersonId = authorPersonId;
            this.title = title;
            this.body = body;
            this.mailPriority = mailPriority;
            this.mailType = mailTypeId;
            this.geographyId = geographyId;
            this.organizationId = organizationId;
            this.createdDateTime = createdDateTime;
            this.releaseDateTime = releaseDateTime;
            this.readyForPickup = readyForPickup;
            this.resolved = resolved;
            this.processed = processed;
            this.resolvedDateTime = resolvedDateTime;
            this.startProcessDateTime = startProcessDateTime;
            this.endProcessDateTime = endProcessDateTime;
            this.recipientCount = recipientCount;
            this.recipientsSuccess = recipientsSuccess;
            this.recipientsFail = recipientsFail;

            // phew.
        }

        /// <summary>
        ///     Constructor used by OutboundMail after creating record in db. NOT for other use
        /// </summary>
        public BasicOutboundMail (int outboundMailId, MailAuthorType authorType, int authorPersonId, string title,
            string body, int mailPriority, int mailType, int organizationId,
            int geographyId, DateTime createdDateTime, DateTime releaseDateTime)
            : this (outboundMailId, authorType, authorPersonId, title, body,
                mailPriority, mailType, organizationId, geographyId,
                createdDateTime, releaseDateTime,
                false, false, false, Constants.DateTimeLow, Constants.DateTimeLow, Constants.DateTimeLow, 0, 0, 0)
        {
            // nothing more to do
        }

        /// <summary>
        ///     Copy constructor. Used when PirateWeb.Logic constructs an OutboundMail object.
        /// </summary>
        /// <param name="original">The original to copy.</param>
        public BasicOutboundMail (BasicOutboundMail original)
            : this (
                original.outboundMailId, original.authorType, original.authorPersonId, original.title,
                original.body, original.mailPriority, original.mailType,
                original.organizationId, original.geographyId, original.createdDateTime, original.releaseDateTime,
                original.readyForPickup, original.resolved, original.processed,
                original.resolvedDateTime, original.startProcessDateTime, original.endProcessDateTime,
                original.recipientCount, original.recipientsSuccess, original.recipientsFail)
        {
            // nothing more to do after copying fields
        }

        #endregion

        #region Properties and Accessors

        // ============================
        //   PROPERTIES AND ACCESSORS
        // ============================


        public int OutboundMailId
        {
            get { return this.outboundMailId; }
        }

        public MailAuthorType AuthorType
        {
            get { return this.authorType; }
        }

        public int AuthorPersonId
        {
            get { return this.authorPersonId; }
        }

        public string Title
        {
            get { return this.title; }
            protected set { this.title = value; }
        }

        public string Body
        {
            get { return this.body; }
        }

        public int OrganizationId
        {
            get { return this.organizationId; }
        }

        public int GeographyId
        {
            get { return this.geographyId; }
        }

        public int MailPriority
        {
            get { return this.mailPriority; }
        }

        public int MailType
        {
            get { return this.mailType; }
        }

        public DateTime ReleaseDateTime
        {
            get { return this.releaseDateTime; }
        }

        public DateTime StartProcessDateTime
        {
            get { return this.startProcessDateTime; }
            set { this.startProcessDateTime = value; }
        }

        public bool ReadyForPickup
        {
            get { return this.readyForPickup; }
            set { this.readyForPickup = value; }
        }

        public bool Resolved
        {
            get { return this.resolved; }
            set { this.resolved = value; }
        }

        public bool Processed
        {
            get { return this.processed; }
            set { this.processed = value; }
        }

        public int RecipientCount
        {
            get { return this.recipientCount; }
            set { this.recipientCount = value; }
        }

        public int RecipientsSuccess
        {
            get { return this.recipientsSuccess; }
            set { this.recipientsSuccess = value; }
        }

        public int RecipientsFail
        {
            get { return this.recipientsFail; }
            set { this.recipientsFail = value; }
        }

        public int Identity
        {
            get { return this.OutboundMailId; }
        }

        #endregion

        #region Private fields

        // ==================
        //   PRIVATE FIELDS
        // ==================


        private readonly int authorPersonId;
        private readonly MailAuthorType authorType;
        private readonly string body;
        private readonly DateTime createdDateTime;
        private readonly DateTime endProcessDateTime;
        private readonly int geographyId;
        private readonly int mailPriority;
        private readonly int mailType;
        private readonly int organizationId;
        private readonly int outboundMailId;
        private readonly DateTime releaseDateTime;
        private readonly DateTime resolvedDateTime;
        private bool processed;
        private bool readyForPickup;
        private int recipientCount;
        private int recipientsFail;
        private int recipientsSuccess;
        private bool resolved;
        private DateTime startProcessDateTime;
        private string title;

        #endregion
    }
}