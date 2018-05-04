using System;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicFinancialAccountRow
    {
        public BasicFinancialAccountRow (int financialAccountId, int financialTransactionId, int financialTransactionRowId, DateTime dateTime,
            string comment, Int64 amountCents, DateTime rowDateTime, int rowCreatedByPersonId)
        {
            FinancialAccountId = financialAccountId;
            FinancialTransactionId = financialTransactionId;
            TransactionDateTime = dateTime;
            Description = comment;
            AmountCents = amountCents;
            RowDateTime = rowDateTime;
            RowCreatedByPersonId = rowCreatedByPersonId;
        }

        public BasicFinancialAccountRow (BasicFinancialAccountRow original)
            : this (
                original.FinancialAccountId, original.FinancialTransactionId, original.FinancialTransactionRowId,
                original.TransactionDateTime, original.Description,
                original.AmountCents, original.RowDateTime, original.RowCreatedByPersonId)
        {
        }

        public int FinancialAccountId { get; private set; }
        public int FinancialTransactionId { get; private set; }
        public int FinancialTransactionRowId { get; private set; }
        public DateTime TransactionDateTime { get; private set; }
        public string Description { get; private set; }
        public Int64 AmountCents { get; private set; }
        public DateTime RowDateTime { get; private set; }
        public int RowCreatedByPersonId { get; private set; }

    }
}