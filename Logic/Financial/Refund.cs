using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class Refund : BasicRefund
    {
        private Refund (BasicRefund basic) : base (basic)
        {
            // empty pvt ctor
        }

        public Payment Payment
        {
            get { return Payment.FromIdentity (PaymentId); }
        }

        public decimal AmountDecimal
        {
            get
            {
                long cents = AmountCents;

                if (AmountCents == 0)
                {
                    cents = Payment.AmountCents;
                }

                return cents/100.0m;
            }
        }

        public static Refund FromBasic (BasicRefund basic)
        {
            return new Refund (basic);
        }

        public static Refund FromIdentity (int refundId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetRefund (refundId));
        }

        public static Refund Create (Payment payment, Person creatingPerson)
        {
            return Create (payment, creatingPerson, 0L);
        }

        public static Refund Create (Payment payment, Person creatingPerson, Int64 amountCents)
        {
            if (amountCents > payment.AmountCents)
            {
                throw new ArgumentException ("Refund amount cannot exceed payment amount");
            }

            Refund refund =
                FromIdentity (SwarmDb.GetDatabaseForWriting()
                    .CreateRefund (payment.Identity, creatingPerson.Identity, amountCents));

            PWEvents.CreateEvent (EventSource.PirateWeb, EventType.RefundCreated, 0,
                refund.Payment.OutboundInvoice.Organization.Identity,
                Geography.RootIdentity, 0, refund.Identity, string.Empty);

            return refund;
        }
    }
}