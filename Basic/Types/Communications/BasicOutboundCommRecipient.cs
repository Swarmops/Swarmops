using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Communications
{
    public class BasicOutboundCommRecipient : IHasIdentity
    {
        public BasicOutboundCommRecipient (int outboundCommRecipientId, int outboundCommId, int personId, bool open,
            bool success, string failReason)
        {
            OutboundCommRecipientId = outboundCommRecipientId;
            OutboundCommId = outboundCommId;
            PersonId = personId;
            Open = open;
            Success = success;
            FailReason = failReason;
        }

        public BasicOutboundCommRecipient (BasicOutboundCommRecipient original) :
            this (
            original.OutboundCommRecipientId, original.OutboundCommId, original.PersonId, original.Open,
            original.Success, original.FailReason)
        {
            // copy ctor
        }

        public int OutboundCommRecipientId { get; private set; }
        public int OutboundCommId { get; private set; }
        public int PersonId { get; private set; }
        public bool Open { get; protected set; }
        public bool Success { get; protected set; }
        public string FailReason { get; protected set; }

        #region Implementation of IHasIdentity

        public int Identity
        {
            get { return OutboundCommRecipientId; }
        }

        #endregion
    }
}