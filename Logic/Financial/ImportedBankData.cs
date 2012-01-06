using System;
using System.Collections.Generic;

namespace Activizr.Logic.Financial
{
    public class ImportedBankData
    {
        public double CurrentBalance;
        public Int64 CurrentBalanceCents;
        public List<ImportedBankRow> Rows;
    }

    public class ImportedBankRow
    {
        public DateTime DateTime;
        public string HashBase;
        public string SuppliedTransactionId;
        public Int64 CurrentBalanceCents;
        public Int64 AmountCentsNet;
        public Int64 AmountCentsGross;
        public Int64 FeeCents;
        public string Comment;
    };
}