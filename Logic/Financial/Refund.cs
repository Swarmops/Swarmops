using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
{
    public class Refund: BasicRefund
    {
        private Refund (BasicRefund basic): base (basic)
        {
            // empty pvt ctor
        }

        public static Refund FromBasic (BasicRefund basic)
        {
            return new Refund(basic);
        }

        public static Refund FromIdentity (int refundId)
        {
            return FromBasic(PirateDb.GetDatabase().GetRefund(refundId));
        }

        public static Refund Create (Payment payment, Person creatingPerson)
        {
            return Create(payment, creatingPerson, 0L);
        }

        public static Refund Create (Payment payment, Person creatingPerson, Int64 amountCents)
        {
            if (amountCents > payment.AmountCents)
            {
                throw new ArgumentException("Refund amount cannot exceed payment amount");
            }

            Refund refund = FromIdentity(PirateDb.GetDatabase().CreateRefund(payment.Identity, creatingPerson.Identity, amountCents));

            PWEvents.CreateEvent(EventSource.PirateWeb, EventType.RefundCreated, 0, refund.Payment.OutboundInvoice.Organization.Identity,
                     Geography.RootIdentity, 0, refund.Identity, string.Empty);

            return refund;
        }

        public Payment Payment
        {
            get
            {
                return Payment.FromIdentity(this.PaymentId);
            }
        }

        public decimal AmountDecimal
        {
            get
            {
                long cents = this.AmountCents;

                if (AmountCents == 0)
                {
                    cents = this.Payment.AmountCents;
                }

                return cents/100.0m;
            }
        }
    }
}
