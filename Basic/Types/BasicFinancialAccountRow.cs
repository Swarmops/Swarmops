using System;
using System.Collections.Generic;
using System.Text;

namespace Activizr.Basic.Types
{
    public class BasicFinancialAccountRow
    {
        public BasicFinancialAccountRow (int financialAccountId, int financialTransactionId, DateTime dateTime,
                                         string comment, Int64 amountCents, DateTime rowDateTime, int rowCreatedByPersonId)
        {
            this.FinancialAccountId = financialAccountId;
            this.FinancialTransactionId = financialTransactionId;
            this.TransactionDateTime = dateTime;
            this.Description = comment;
            this.AmountCents = amountCents;
            this.RowDateTime = rowDateTime;
            this.RowCreatedByPersonId = rowCreatedByPersonId;
        }

        public BasicFinancialAccountRow (BasicFinancialAccountRow original)
            : this(
                original.FinancialAccountId, original.FinancialTransactionId, original.TransactionDateTime, original.Description,
                original.AmountCents, original.RowDateTime, original.RowCreatedByPersonId)
        {
        }

        public int FinancialAccountId { get; private set; }
        public int FinancialTransactionId { get; private set; }
        public DateTime TransactionDateTime { get; private set; }
        public string Description { get; private set; }
        public Int64 AmountCents { get; private set; }
        public DateTime RowDateTime { get; private set; }
        public int RowCreatedByPersonId { get; private set; }

    }
}