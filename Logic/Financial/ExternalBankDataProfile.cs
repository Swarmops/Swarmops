using System;
using System.Collections.Generic;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Financial
{
    public class ExternalBankDataProfile
    {
        public FeeSignage FeeSignage;
        public LatestTransactionLocation LatestTransactionLocation;
        public ExternalBankDateTimePrecision Precision;

        private ExternalBankDataProfile()
        {
            FieldNames = new Dictionary<ExternalBankDataFieldName, string>();
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

        public string Name { get; set; }
        public Country Country { get; set; }
        public string Culture { get; set; }
        public string Currency { get; set; }
        public string BankDataAccountReader { get; set; }
        public string BankDataPaymentsReader { get; set; }
        public int IgnoreInitialLines { get; set; }
        public string InitialReplacements { get; set; }

        public Dictionary<ExternalBankDataFieldName, string> FieldNames { get; private set; }

        public static ExternalBankDataProfile FromIdentity (int externalBankDataProfileId)
        {
            ExternalBankDataProfile result = new ExternalBankDataProfile();

            // Ugly hack for now

            if (externalBankDataProfileId == SESebId)
            {
                result.Name = "SEB";
                result.Country = Country.FromCode ("SE");
                result.Culture = "sv-SE";

                result.FieldNames[ExternalBankDataFieldName.Date] = "Bokföringsdatum";
                result.FieldNames[ExternalBankDataFieldName.Description] = "Text/mottagare";
                result.FieldNames[ExternalBankDataFieldName.TransactionNet] = "Belopp";
                result.FieldNames[ExternalBankDataFieldName.AccountBalance] = "Saldo";
                result.FieldNames[ExternalBankDataFieldName.NotUniqueId] = "Verifikationsnummer";

                result.LatestTransactionLocation = LatestTransactionLocation.Top;
                result.FeeSignage = FeeSignage.Unknown; // no inline fees
                result.Precision = ExternalBankDateTimePrecision.Day;

                result.BankDataAccountReader = StockBankDataReaders.TabSeparatedValuesAccountReader;
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
                result.FieldNames[ExternalBankDataFieldName.Description] = "Name";
                result.FieldNames[ExternalBankDataFieldName.Currency] = "Currency";
                result.FieldNames[ExternalBankDataFieldName.TransactionGross] = "Gross";
                result.FieldNames[ExternalBankDataFieldName.TransactionFee] = "Fee";
                result.FieldNames[ExternalBankDataFieldName.TransactionNet] = "Net";
                result.FieldNames[ExternalBankDataFieldName.AccountBalance] = "Balance";
                result.FieldNames[ExternalBankDataFieldName.UniqueId] = "Transaction ID";

                result.LatestTransactionLocation = LatestTransactionLocation.Top;
                result.FeeSignage = FeeSignage.Negative;
                result.Precision = ExternalBankDateTimePrecision.Second;

                result.BankDataAccountReader = StockBankDataReaders.TabSeparatedValuesAccountReader;
                result.BankDataPaymentsReader = null; // No aggregated payments with Paypal

                return result;
            }

            if (externalBankDataProfileId == DEPostbankId)
            {
                result.Name = "DE Postbank";
                result.Country = Country.FromCode("DE");
                result.Culture = "de-DE";
                result.IgnoreInitialLines = 7;
                result.InitialReplacements = ";|\t| €|";

                result.FieldNames[ExternalBankDataFieldName.Date] = "Buchungstag";
                result.FieldNames[ExternalBankDataFieldName.Description] = "Buchungsdetails";
                result.FieldNames[ExternalBankDataFieldName.TransactionNet] = "Betrag (€)";
                result.FieldNames[ExternalBankDataFieldName.AccountBalance] = "Saldo (€)";

                result.LatestTransactionLocation = LatestTransactionLocation.Top;
                result.FeeSignage = FeeSignage.Unknown; // no inline fees
                result.Precision = ExternalBankDateTimePrecision.Day;

                result.BankDataAccountReader = StockBankDataReaders.TabSeparatedValuesAccountReader;
                result.BankDataPaymentsReader = null; // No aggregated payments with Paypal

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
        Description,
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