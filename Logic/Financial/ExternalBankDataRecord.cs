using System;
using System.Collections.Generic;
using System.Globalization;
using Swarmops.Logic.Security;

namespace Swarmops.Logic.Financial
{
    public class ExternalBankDataRecord : IComparer<ExternalBankDataRecord>
    {
        public long AccountBalanceCents;
        public DateTime DateTime; // UTC!
        public string Description;
        public string CounterpartyName;
        public string CounterpartyBankAccount;
        public long FeeCents;

        public string NotUniqueId;
        // As it says, but something that still assists in creating a hash together w/ other fields

        public long TransactionGrossCents;
        public long TransactionNetCents;
        public string UniqueId;

        #region Implementation of IComparer<ExternalBankDataRecord>

        public int Compare (ExternalBankDataRecord x, ExternalBankDataRecord y)
        {
            // Use only DateTime to compare

            return x.DateTime.CompareTo (y.DateTime);
        }

        #endregion

        public string ImportHash
        {
            get
            {
                string importKey = string.Empty;

                if (!string.IsNullOrEmpty (this.UniqueId))
                {
                    importKey = this.UniqueId;
                }
                else if (!string.IsNullOrEmpty (this.NotUniqueId)) // no unique ID, but maybe something else
                {
                    string commentKey = this.Description.ToLowerInvariant();

                    // " / 100.0" below serves no purpose but must stay now that the hash scheme is in effect

                    string hashKey = this.NotUniqueId + commentKey +
                                     (this.TransactionNetCents/100.0).ToString (CultureInfo.InvariantCulture) +
                                     (this.AccountBalanceCents/100.0).ToString (CultureInfo.InvariantCulture) +
                                     this.DateTime.ToString ("yyyy-MM-dd-HH-mm-ss");

                    importKey = SHA1.Hash (hashKey).Replace (" ", "");
                }
                else // neither unique nor nonunique ID, like DE Postbank
                {
                    string commentKey = this.Description.ToLowerInvariant();

                    string hashKey = commentKey +
                                     this.TransactionNetCents.ToString(CultureInfo.InvariantCulture) +
                                     this.AccountBalanceCents.ToString(CultureInfo.InvariantCulture) +
                                     this.DateTime.ToString("yyyy-MM-dd-HH-mm-ss");

                    importKey = SHA1.Hash(hashKey).Replace(" ", "");

                }

                if (importKey.Length > 30)
                {
                    importKey = importKey.Substring (0, 30);
                }

                return importKey;
            }
        }
    }
}