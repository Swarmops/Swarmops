using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicFinancialTransactionRow : IHasIdentity
    {
        public BasicFinancialTransactionRow (int financialTransactionRowId, int financialAccountId,
            int financialTransactionId, Int64 amountCents, DateTime createdDateTime, int createdByPersonId)
        {
            FinancialTransactionRowId = financialTransactionRowId;
            FinancialAccountId = financialAccountId;
            FinancialTransactionId = financialTransactionId;
            AmountCents = amountCents;
            CreatedDateTime = createdDateTime;
            CreatedByPersonId = createdByPersonId;
        }

        public BasicFinancialTransactionRow (BasicFinancialTransactionRow original)
            : this (
                original.FinancialTransactionRowId, original.FinancialAccountId, original.FinancialTransactionId,
                original.AmountCents, original.CreatedDateTime, original.CreatedByPersonId)
        {
        }

        public int FinancialAccountId { get; private set; }
        public int FinancialTransactionId { get; private set; }
        public int FinancialTransactionRowId { get; private set; }
        public Int64 AmountCents { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public int CreatedByPersonId { get; private set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return FinancialTransactionRowId; }
        }

        #endregion
    }
}