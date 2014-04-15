using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Swarmops.Logic.Security;

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

        public string ImportHash
        {
            get
            {
                string importKey = string.Empty;

                if (!string.IsNullOrEmpty(UniqueId))
                {
                    importKey = UniqueId;
                }
                else if (!string.IsNullOrEmpty(NotUniqueId))
                {
                    string commentKey = Description.ToLowerInvariant();

                    string hashKey = NotUniqueId + commentKey + (TransactionNetCents / 100.0).ToString(CultureInfo.InvariantCulture) + (AccountBalanceCents / 100.0).ToString(CultureInfo.InvariantCulture) +
                                     DateTime.ToString("yyyy-MM-dd-00-00-00"); 
                    
                    // the 00-00-00 used to be hh-mm-ss but is replaced this way for backward hash compatibility
                    // maybe replace cleanly at a certain year to include seconds in hash or so

                    importKey = SHA1.Hash(hashKey).Replace(" ", "");
                }

                if (importKey.Length > 30)
                {
                    importKey = importKey.Substring(0, 30);
                }

                return importKey;
            }
        }
    }
}
