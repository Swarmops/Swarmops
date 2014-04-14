using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swarmops.Logic.Financial
{
    public class ExternalBankDataRecord: IComparer<ExternalBankDataRecord>
    {
        public long AccountBalanceCents;
        public long TransactionGrossCents;
        public long TransactionNetCents;
        public long FeeCents;
        public string Description;
        public DateTime DateTime;  // UTC!
        public string UniqueId;
        public string NotUniqueId; // As it says, but something that still assists in creating a hash together w/ other fields

        #region Implementation of IComparer<ExternalBankDataRecord>

        public int Compare(ExternalBankDataRecord x, ExternalBankDataRecord y)
        {
            // Use only DateTime to compare

            return x.DateTime.CompareTo(y.DateTime);
        }

        #endregion
    }
}
