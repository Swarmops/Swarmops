using System;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class FinancialActionBase : LogEntryBase<FinancialActionBase>
    {
        public int FinancialAccountId { get; set; }
        public string FinancialAccountName { get; set; }
        public int ActingPersonId { get; set; }
        public int BeneficiaryPersonId { get; set; }
        public int OrganizationId { get; set; }

        [Obsolete("Find and eliminate all uses of double-precision in financial")]
        public double Amount { get; set; }
        public Int64 AmountCents { get; set; }

        [Obsolete("Find and eliminate all uses of double-precision in financial")]
        public double Vat { get; set; }
        public Int64 VatCents { get; set; }

        public string CurrencyCode { get; set; }
        public string Description { get; set; }
        public int OwnerPersonId { get; set; }
        public string OwnerPersonName { get; set; }
        /// <summary>
        /// Always in UTC
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}