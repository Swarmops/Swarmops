using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Swarmops.Common.Interfaces;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Financial
{
    public class ExternalBankDataProfile: IHasIdentity
    {
        public FeeSignage FeeSignage;
        public LatestTransactionLocation LatestTransactionLocation;
        public ExternalBankDateTimePrecision Precision;

        private ExternalBankDataProfile()
        {
            FieldNames = new Dictionary<ExternalBankDataFieldName, string>();
            FieldDelimiter = ',';
        }

        public string DateTimeFormatString
        {
            get
            {
                switch (this.Precision)
                {
                    case ExternalBankDateTimePrecision.Day:
                        return "yyyy-MM-dd";
                    case ExternalBankDateTimePrecision.Minute:
                        return "yyyy-MM-dd HH:mm";
                    case ExternalBankDateTimePrecision.Second:
                        return "yyyy-MM-dd HH:mm:ss";
                    case ExternalBankDateTimePrecision.Microsecond:
                        return "yyyy-MM-dd HH:mm:ss.ffffff";
                    default:
                        throw new NotImplementedException ("Unknown ExternalDateTimePrecision: " + this.Precision);
                }
            }
        }

        public static int SESebId
        {
            get { return 1; }
        } // Ugly pre-db hacks

        public static int PaypalId
        {
            get { return 2; }
        }

        public static int DEPostbankId
        {
            get { return 3; }
        }

        public static int CZFioId
        {
            get { return 4; }
        }

        public string Name { get; set; }

        [XmlIgnore]
        public Country Country
        {
            get
            {
                if (CountryCode == string.Empty) return null;
                return Country.FromCode(CountryCode);
            }
            set
            {
                if (value == null)
                {
                    CountryCode = string.Empty;
                }
                else
                {
                    CountryCode = value.Code;
                }
            }
        }
        public string CountryCode { get; set; }
        public string Culture { get; set; }
        public string Encoding { get; set; }
        public string Currency { get; set; }
        public string BankDataAccountReader { get; set; }
        public string BankDataPaymentsReader { get; set; }
        public int IgnoreInitialLines { get; set; }
        public string InitialReplacements { get; set; }
        public int ExternalBankDataProfileId { get; set; }
        public char FieldDelimiter { get; set; }
        public string DateTimeCustomFormatString { get; set; }
        public int Identity { get { return ExternalBankDataProfileId; } }

        public Dictionary<ExternalBankDataFieldName, string> FieldNames { get; private set; }

        public static ExternalBankDataProfile FromIdentity (int externalBankDataProfileId)
        {
            ExternalBankDataProfile result = new ExternalBankDataProfile();
            result.ExternalBankDataProfileId = externalBankDataProfileId;

            // Ugly hack for now

            if (externalBankDataProfileId == SESebId)
            {
                result.Name = "SEB";
                result.Country = Country.FromCode ("SE");
                result.Culture = "sv-SE";

                result.FieldNames[ExternalBankDataFieldName.Date] = "Bokföringsdatum";
                result.FieldNames[ExternalBankDataFieldName.DescriptionPrimary] = "Text/mottagare";
                result.FieldNames[ExternalBankDataFieldName.TransactionNet] = "Belopp";
                result.FieldNames[ExternalBankDataFieldName.AccountBalance] = "Saldo";
                result.FieldNames[ExternalBankDataFieldName.NotUniqueId] = "Verifikationsnummer";

                result.LatestTransactionLocation = LatestTransactionLocation.Top;
                result.FeeSignage = FeeSignage.Unknown; // no inline fees
                result.Precision = ExternalBankDateTimePrecision.Day;

                result.BankDataAccountReader = StockBankDataReaders.CommaSeparatedValuesAccountReader;
                result.BankDataPaymentsReader = StockBankDataReaders.SEPaymentsBankgiroReader;

                return result;
            }

            if (externalBankDataProfileId == PaypalId)
            {
                result.Name = "Paypal";
                result.Country = null;
                result.Culture = "sv-SE"; // what the actual fuck? Paypal's giving me sv-SE culture in export file

                result.FieldNames[ExternalBankDataFieldName.Date] = "Date";
                result.FieldNames[ExternalBankDataFieldName.Time] = "Time";
                result.FieldNames[ExternalBankDataFieldName.TimeZone] = "Time Zone";
                result.FieldNames[ExternalBankDataFieldName.DescriptionPrimary] = "Name";
                result.FieldNames[ExternalBankDataFieldName.Currency] = "Currency";
                result.FieldNames[ExternalBankDataFieldName.TransactionGross] = "Gross";
                result.FieldNames[ExternalBankDataFieldName.TransactionFee] = "Fee";
                result.FieldNames[ExternalBankDataFieldName.TransactionNet] = "Net";
                result.FieldNames[ExternalBankDataFieldName.AccountBalance] = "Balance";
                result.FieldNames[ExternalBankDataFieldName.UniqueId] = "Transaction ID";

                result.LatestTransactionLocation = LatestTransactionLocation.Top;
                result.FeeSignage = FeeSignage.Negative;
                result.Precision = ExternalBankDateTimePrecision.Second;

                result.BankDataAccountReader = StockBankDataReaders.CommaSeparatedValuesAccountReader;
                result.BankDataPaymentsReader = null; // No aggregated payments with Paypal

                return result;
            }

            if (externalBankDataProfileId == DEPostbankId)
            {
                result.Name = "DE Postbank";
                result.Country = Country.FromCode("DE");
                result.Culture = "de-DE";
                result.IgnoreInitialLines = 7;
                result.InitialReplacements = ";|\t| €||Referenz NOTPROVIDED Verwendungszweck|";

                result.FieldNames[ExternalBankDataFieldName.Date] = "Buchungstag";
                result.FieldNames[ExternalBankDataFieldName.DescriptionPrimary] = "Buchungsdetails";
                result.FieldNames[ExternalBankDataFieldName.DescriptionSecondary] = "Umsatzart";
                result.FieldNames[ExternalBankDataFieldName.TransactionNet] = "Betrag (€)";
                result.FieldNames[ExternalBankDataFieldName.AccountBalance] = "Saldo (€)";

                result.LatestTransactionLocation = LatestTransactionLocation.Top;
                result.FeeSignage = FeeSignage.Unknown; // no inline fees
                result.Precision = ExternalBankDateTimePrecision.Day;

                result.BankDataAccountReader = StockBankDataReaders.CommaSeparatedValuesAccountReader;
                result.BankDataPaymentsReader = null; // No aggregated payments with Postbank

                result.FieldDelimiter = ';'; // DE Postbank uses semicolon as delimiter for no reason in particular

                return result;
            }

            if (externalBankDataProfileId == CZFioId)
            {
                // Czech Fio Bank

                result.Name = "CZ Fio";
                result.Country = Country.FromCode("CZ");
                result.Culture = "cz-CZ";
                result.Encoding = "UTF-8";

                result.InitialReplacements = ";|\t";  // before CSV helper is implemented, replace field separators with spaces

                result.FieldNames[ExternalBankDataFieldName.Date] = "Datum";  // in dd.mm.yyyy format
                result.FieldNames[ExternalBankDataFieldName.DescriptionPrimary] = "Zpráva pro příjemce";
                result.FieldNames[ExternalBankDataFieldName.CounterpartyName] = "Název protiúčtu";
                result.FieldNames[ExternalBankDataFieldName.CounterpartyAccount] = "Protiúčet";
                result.FieldNames[ExternalBankDataFieldName.CounterpartyBank] = "Kód banky";
                result.FieldNames[ExternalBankDataFieldName.DescriptionSecondary] = "Typ";
                result.FieldNames[ExternalBankDataFieldName.TransactionNet] = "Objem";   // has comma as radix
                result.FieldNames[ExternalBankDataFieldName.Currency] = "Měna";          // must be czk

                result.LatestTransactionLocation = LatestTransactionLocation.Bottom;
                result.FeeSignage = FeeSignage.Unknown; // no inline fees
                result.Precision = ExternalBankDateTimePrecision.Day;
                result.DateTimeFormatString = "dd.MM.yyyy";

                result.BankDataAccountReader = StockBankDataReaders.CommaSeparatedValuesAccountReader;
                result.BankDataPaymentsReader = null; // No aggregated payments with Postbank

                result.FieldDelimiter = ';'; // CZ Fio uses semicolon as delimiter for no reason in particular

                return result;
            }

            throw new ArgumentException ("Unrecognized profile Id");
        }
    }

    public enum ExternalBankDataFieldName
    {
        Unknown = 0,
        AccountBalance,
        TransactionGross,
        TransactionFee,
        TransactionNet,
        DescriptionPrimary,
        DescriptionSecondary,
        CounterpartyAccount,
        CounterpartyBank,
        CounterpartyName,
        UniqueId,
        NotUniqueId,
        Date,
        Time,
        DateTime,
        TimeZone,
        Currency
    }

    public enum LatestTransactionLocation
    {
        Unknown = 0,

        /// <summary>
        ///     Latest transaction is at the top of the file (the first record, meaning reverse chrono order).
        /// </summary>
        Top,

        /// <summary>
        ///     Latest transaction is at the bottom of the file (last record, chrono order).
        /// </summary>
        Bottom
    }

    public enum ExternalBankDateTimePrecision
    {
        Unknown = 0,

        /// <summary>
        ///     The external bank has day precision only. Typical for oldbanks.
        /// </summary>
        Day,

        /// <summary>
        ///     The external bank has minute (hour:minute) precision on top of day.
        /// </summary>
        Minute,

        /// <summary>
        ///     The external bank tags transactions with hour, minute, and second.
        /// </summary>
        Second,

        /// <summary>
        ///     The external bank has millisecond-level timestamps for transactions. Not implemented.
        /// </summary>
        Millisecond,

        /// <summary>
        ///     The external bank has microsecond-resolution timestamp for transactions
        /// </summary>
        Microsecond,

        /// <summary>
        ///     The external bank has nanosecond-resolution timestamp for transactions
        /// </summary>
        Nanosecond
    }

    public enum FeeSignage
    {
        Unknown = 0,

        /// <summary>
        ///     The transaction fee is positively listed (as a positive number), so that gross - fee = net.
        /// </summary>
        Positive,

        /// <summary>
        ///     The transaction fee is listed as negative, so that gross + fee = net. Example: Paypal.
        /// </summary>
        Negative
    }
}