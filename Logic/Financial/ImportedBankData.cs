using System;
using System.Collections.Generic;

namespace Swarmops.Logic.Financial
{
    public class ImportedBankData
    {
        public double CurrentBalance;
        public Int64 CurrentBalanceCents;
        public List<ImportedBankRow> Rows;
    }

    public class ImportedBankRow
    {
        public Int64 AmountCentsGross;
        public Int64 AmountCentsNet;
        public string Comment;
        public Int64 CurrentBalanceCents;
        public DateTime DateTime;
        public Int64 FeeCents;
        public string HashBase;
        public string SuppliedTransactionId;
    };
}