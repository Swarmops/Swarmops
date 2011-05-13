using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicFinancialTransactionRow : IHasIdentity
    {
        public BasicFinancialTransactionRow (int financialTransactionRowId, int financialAccountId,
                                             int financialTransactionId, Int64 amountCents, DateTime createdDateTime, int createdByPersonId)
        {
            this.FinancialTransactionRowId = financialTransactionRowId;
            this.FinancialAccountId = financialAccountId;
            this.FinancialTransactionId = financialTransactionId;
            this.AmountCents = amountCents;
            this.CreatedDateTime = createdDateTime;
            this.CreatedByPersonId = createdByPersonId;
        }

        public BasicFinancialTransactionRow (BasicFinancialTransactionRow original)
            : this(
                original.FinancialTransactionRowId, original.FinancialAccountId, original.FinancialTransactionId,
                original.AmountCents, original.CreatedDateTime, original.CreatedByPersonId)
        {
        }

        public int FinancialAccountId { get; private set; }
        public int FinancialTransactionId { get; private set; }
        public int FinancialTransactionRowId { get; private set;}
        public Int64 AmountCents { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public int CreatedByPersonId { get; private set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.FinancialTransactionRowId; }
        }

        #endregion
    }
}