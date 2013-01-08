using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    [Serializable]
    public class BasicCashAdvance: IHasIdentity
    {
        public BasicCashAdvance(int cashAdvanceId, int organizationId, int personId, DateTime createdDateTime, int createdByPersonId,
            int financialAccountId, Int64 amountCents, string description, bool open, bool attested, 
            bool paidOut, int attestedByPersonId, DateTime attestedDateTime)
        {
            this.CashAdvanceId = cashAdvanceId;
            this.OrganizationId = organizationId;
            this.PersonId = personId;
            this.CreatedDateTime = createdDateTime;
            this.CreatedByPersonId = createdByPersonId;
            this.FinancialAccountId = financialAccountId;
            this.AmountCents = amountCents;
            this.Description = description;
            this.Open = open;
            this.Attested = attested;
            this.PaidOut = paidOut;
            this.AttestedByPersonId = attestedByPersonId;
            this.AttestedDateTime = attestedDateTime;
        }

        public BasicCashAdvance (BasicCashAdvance original):
            this(original.CashAdvanceId, original.OrganizationId, original.PersonId, original.CreatedDateTime, original.CreatedByPersonId,
            original.FinancialAccountId, original.AmountCents,original.Description, original.Open, original.Attested,
            original.PaidOut, original.AttestedByPersonId, original.AttestedDateTime)
        {
            // copy ctor - no action other than copying original object
        }

        public BasicCashAdvance()
        {
            // Never call this ctor directly. Produces an intentional compile-time error. Exists for serialization only.
        }


        public int CashAdvanceId { get; private set; }
        public int OrganizationId { get; private set; }
        public int PersonId { get; protected set; }
        public DateTime CreatedDateTime { get; protected set; }
        public int CreatedByPersonId { get; protected set; }
        public int FinancialAccountId { get; protected set; }
        public Int64 AmountCents { get; protected set; }
        public string Description { get; protected set; }
        public bool Open { get; protected set; }
        public bool Attested { get; protected set; }
        public bool PaidOut { get; protected set; }
        public int AttestedByPersonId { get; protected set; }
        public DateTime AttestedDateTime { get; protected set; }

        // IHasIdentity interface

        public int Identity { get { return this.CashAdvanceId; } }
    }
}
