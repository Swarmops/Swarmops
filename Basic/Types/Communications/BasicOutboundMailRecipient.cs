using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Communications
{
    public class BasicOutboundMailRecipient : IHasIdentity
    {
        #region Creation and Construction

        /// <summary>
        ///     Normal constructor.
        /// </summary>
        public BasicOutboundMailRecipient (int outboundMailRecipientId, int outboundMailId, int personId, bool asOfficer,
            int personType)
        {
            this.outboundMailId = outboundMailId;
            this.outboundMailRecipientId = outboundMailRecipientId;
            this.personId = personId;
            this.asOfficer = asOfficer;
            this.personType = personType;
        }

        /// <summary>
        ///     Copy constructor.
        /// </summary>
        public BasicOutboundMailRecipient (BasicOutboundMailRecipient original)
            : this (
                original.outboundMailRecipientId, original.outboundMailId, original.personId, original.asOfficer,
                original.personType)
        {
        }

        #endregion

        #region Properties and Accessors

        public int OutboundMailRecipientId
        {
            get { return this.outboundMailRecipientId; }
        }

        public int OutboundMailId
        {
            get { return this.outboundMailId; }
        }

        public int PersonId
        {
            get { return this.personId; }
        }


        public bool AsOfficer
        {
            get { return this.asOfficer; }
        }

        public int PersonType
        {
            get { return this.personType; }
        }

        public int Identity
        {
            get { return this.OutboundMailRecipientId; }
        }

        #endregion

        #region Private fields

        private readonly bool asOfficer;
        private readonly int outboundMailId;
        private readonly int outboundMailRecipientId;
        private readonly int personId;
        private readonly int personType; //0 = Person, 1 = Reporter

        #endregion
    }
}