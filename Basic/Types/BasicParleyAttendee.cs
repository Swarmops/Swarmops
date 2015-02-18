using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicParleyAttendee : IHasIdentity
    {
        public BasicParleyAttendee (int parleyAttendeeId, int parleyId, int personId, DateTime signupDateTime,
            bool active, DateTime cancelDateTime, bool invoiced, int outboundInvoiceId, bool isGuest)
        {
            ParleyAttendeeId = parleyAttendeeId;
            ParleyId = parleyId;
            PersonId = personId;
            SignupDateTime = signupDateTime;
            Active = active;
            CancelDateTime = cancelDateTime;
            Invoiced = invoiced;
            OutboundInvoiceId = outboundInvoiceId;
        }

        public BasicParleyAttendee (BasicParleyAttendee original)
            : this (
                original.ParleyAttendeeId, original.ParleyId, original.PersonId, original.SignupDateTime,
                original.Active, original.CancelDateTime, original.Invoiced, original.OutboundInvoiceId,
                original.IsGuest)
        {
            // empty copy ctor
        }

        public int ParleyAttendeeId { get; private set; }
        public int ParleyId { get; private set; }
        public int PersonId { get; private set; }
        public DateTime SignupDateTime { get; private set; }
        public bool Active { get; protected set; }
        public DateTime CancelDateTime { get; private set; }
        public bool Invoiced { get; protected set; }
        public int OutboundInvoiceId { get; protected set; }
        public bool IsGuest { get; private set; }

        public int Identity
        {
            get { return ParleyAttendeeId; }
        }
    }
}