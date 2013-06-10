using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Communications
{
    public class BasicOutboundCommRecipient: IHasIdentity
    {
        public BasicOutboundCommRecipient (int outboundCommRecipientId, int outboundCommId, int personId, bool open, bool success, string failReason)
        {
            this.OutboundCommRecipientId = outboundCommRecipientId;
            this.OutboundCommId = outboundCommId;
            this.PersonId = personId;
            this.Open = open;
            this.Success = success;
            this.FailReason = failReason;
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
            get { return this.OutboundCommRecipientId; }
        }

        #endregion
    }
}
