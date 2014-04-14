using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Financial
{
    public class ExternalBankDataProfile
    {
        private ExternalBankDataProfile()
        {
            this.FieldNames = new Dictionary<ExternalBankDataFieldName, string>();
        }

        static public ExternalBankDataProfile FromIdentity (int externalBankDataProfileId)
        {
            ExternalBankDataProfile result = new ExternalBankDataProfile();

            // Ugly hack for now

            if (externalBankDataProfileId == SESebId)
            {
                result.Name = "SEB";
                result.Country = Country.FromCode("SE");
                result.Culture = "sv-SE";

                result.FieldNames[ExternalBankDataFieldName.Date] = "Bokföringsdatum";
                result.FieldNames[ExternalBankDataFieldName.Description] = "Text/mottagare";
                result.FieldNames[ExternalBankDataFieldName.TransactionNet] = "Belopp";
                result.FieldNames[ExternalBankDataFieldName.AccountBalance] = "Saldo";
                result.FieldNames[ExternalBankDataFieldName.NotUniqueId] = "Verifikationsnummer";

                return result;
            }

            if (externalBankDataProfileId == PaypalId)
            {
                result.Name = "Paypal";
                result.Country = null;
                result.Culture = "sv-SE"; // what the actual fuck? Paypal's giving me sv-SE culture

                result.FieldNames[ExternalBankDataFieldName.Date] = "Date";
                result.FieldNames[ExternalBankDataFieldName.Time] = "Time";
                result.FieldNames[ExternalBankDataFieldName.TimeZone] = "Time Zone";
                result.FieldNames[ExternalBankDataFieldName.Description] = "Name";
                result.FieldNames[ExternalBankDataFieldName.Currency] = "Currency";
                result.FieldNames[ExternalBankDataFieldName.TransactionGross] = "Gross";
                result.FieldNames[ExternalBankDataFieldName.TransactionFee] = "Fee";
                result.FieldNames[ExternalBankDataFieldName.TransactionNet] = "Net";
                result.FieldNames[ExternalBankDataFieldName.AccountBalance] = "Balance";
                result.FieldNames[ExternalBankDataFieldName.UniqueId] = "Transaction ID";

                return result;
            }

            throw new ArgumentException("Unrecognized profile Id");
        }

        static public int SESebId { get { return 1; } }   // Ugly pre-db hacks
        static public int PaypalId { get { return 2; } }

        public string Name { get; set; }
        public Country Country { get; set; }
        public string Culture { get; set; }
        public string Currency { get; set; }

        public Dictionary<ExternalBankDataFieldName, string> FieldNames { get; private set; }
    }

    public enum ExternalBankDataFieldName
    {
        Unknown = 0,
        AccountBalance,
        TransactionGross,
        TransactionFee,
        TransactionNet,
        Description,
        UniqueId,
        NotUniqueId,
        Date,
        Time,
        DateTime,
        TimeZone,
        Currency
    }
}
