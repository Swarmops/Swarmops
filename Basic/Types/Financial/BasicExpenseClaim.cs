using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicExpenseClaim : IHasIdentity
    {
        /// <summary>
        ///     Normal constructor
        /// </summary>
        public BasicExpenseClaim (int expenseClaimId, int claimingPersonId, DateTime createdDateTime,
            bool open, bool attested, bool documented, bool claimed, int organizationId,
            int organizationSequenceId, int geographyId, int budgetId, DateTime expenseDate,
            string description, double preApprovedAmount, Int64 amountCents, Int64 vatCents, bool repaid, bool keepSeparate)
        {
            this.ExpenseClaimId = expenseClaimId;
            this.ClaimingPersonId = claimingPersonId;
            this.CreatedDateTime = createdDateTime;
            this.Open = open;
            this.Attested = attested;
            this.Validated = documented;
            this.Claimed = claimed;
            this.OrganizationId = organizationId;
            this.GeographyId = geographyId;
            this.BudgetId = budgetId;
            this.ExpenseDate = expenseDate;
            this.Description = description;
            this.PreApprovedAmount = preApprovedAmount;
            this.AmountCents = amountCents;
            this.VatCents = vatCents;
            this.Repaid = repaid;
            this.KeepSeparate = keepSeparate;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        public BasicExpenseClaim (BasicExpenseClaim original)
            : this (original.Identity, original.ClaimingPersonId, original.CreatedDateTime,
                original.Open, original.Attested, original.Validated, original.Claimed,
                original.OrganizationId, original.OrganizationId, 
                original.GeographyId, original.BudgetId, original.ExpenseDate, original.Description,
                original.PreApprovedAmount, original.AmountCents, original.VatCents, original.Repaid, original.KeepSeparate)
        {
        }


        public int GeographyId { get; private set; }

        public int OrganizationId { get; private set; }
        public int OrganizationSequenceId { get; protected set; }
        public int ExpenseClaimId { get; private set; }
        public DateTime ExpenseDate { get; protected set; }
        public string Description { get; protected set; }
        public Int64 AmountCents { get; protected set; }
        public Int64 VatCents { get; protected set; }
        public double PreApprovedAmount { get; private set; }
        public int ClaimingPersonId { get; private set; }
        public int BudgetId { get; protected set; }
        public DateTime CreatedDateTime { get; private set; }
        public bool Validated { get; protected set; }
        public bool Open { get; protected set; }
        public bool Attested { get; protected set; }
        public bool Repaid { get; protected set; }
        public bool KeepSeparate { get; protected set; }
        public bool Claimed { get; protected set; }


        // The fields below are not just encapsulated yet

        public int Identity
        {
            get { return this.ExpenseClaimId; }
        }
    }
}