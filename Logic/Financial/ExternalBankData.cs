using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Web.ExtensionMethods;
using Swarmops.Common;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class ExternalBankData
    {
        public long LatestAccountBalanceCents;
        public ExternalBankDataProfile Profile { get; set; }
        public ExternalBankDataRecord[] Records { get; private set; }

        public void LoadData(TextReader reader, Organization organization, Currency expectedCurrency)
        {
            string data = reader.ReadToEnd();
            LoadData(data, organization, expectedCurrency);
        }

        public void LoadData(string data, Organization organization, Currency accountCurrency)
        {
            List<ExternalBankDataRecord> recordList = new List<ExternalBankDataRecord>();

            if (Profile == null)
            {
                throw new InvalidOperationException("Cannot call LoadData before a profile has been set");
            }

            if (Profile.BankDataAccountReader != StockBankDataReaders.CommaSeparatedValuesAccountReader)
            {
                throw new NotImplementedException("Only tab separated values are supported at this point");
            }

            // TODO: Implement bank data reader factory here

            string expectedCurrencyCode = organization.Currency.Code;
            if (accountCurrency != null)
            {
                expectedCurrencyCode = accountCurrency.Code;
            }

            // TODO: This function must be made aware of the expected currency code

            // Replace any strings initially to force the bank data into readability (per profile)

            if (!String.IsNullOrEmpty(Profile.InitialReplacements))
            {
                string[] replacements = Profile.InitialReplacements.Split('|');

                for (int loop = 0; loop < replacements.Length / 2; loop++)
                {
                    data = data.Replace(replacements[loop * 2], replacements[loop * 2 + 1]);
                }
            }

            int crlfIndex;

            for (int loop = 0; loop < Profile.IgnoreInitialLines && !String.IsNullOrEmpty(data); loop++)
            {
                crlfIndex = data.IndexOfAny(new[] { '\n', '\r' });
                if (crlfIndex > 0)
                {
                    data = data.Substring(crlfIndex).Trim();
                }
                else
                {
                    data = string.Empty;
                }
            }

            crlfIndex = data.IndexOfAny(new[] { '\n', '\r' });

            string fieldKeyLine = data.Substring(0, crlfIndex);

            string[] dataKeyFields = fieldKeyLine.Split('\t');

            Dictionary<ExternalBankDataFieldName, int> fieldNameLookup =
                new Dictionary<ExternalBankDataFieldName, int>();

            // below is an N^2 loop but doesn't matter in such a small context

            foreach (ExternalBankDataFieldName fieldName in Profile.FieldNames.Keys)
            {
                if (fieldName == ExternalBankDataFieldName.IgnoreLines)
                {
                    continue; // ignore
                }

                for (int index = 0; index < dataKeyFields.Length; index++)
                {
                    if (StripQuotes(dataKeyFields[index]).Trim().ToLowerInvariant() == Profile.FieldNames[fieldName].ToLowerInvariant())
                    {
                        fieldNameLookup[fieldName] = index;
                        break;
                    }
                }

                if (!fieldNameLookup.ContainsKey(fieldName)) // wasn't found
                {
                    throw new InvalidOperationException("Field key \"" + fieldName +
                                                         "\" was not supplied or found in data file");
                }
            }


            data = data.Substring(crlfIndex).Trim();

            string[] lines = data.Split(new[] { '\r', '\n' });

            foreach (string lineData in lines)
            {
                string line = lineData.Trim();

                if (line.Length < 1)
                {
                    continue; // empty lines may exist due to split on either CR or LF
                }

                string[] lineFields = line.Split('\t');

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.IgnoreLines))
                {
                    if (lineFields[0].ToLowerInvariant() ==
                        Profile.FieldNames[ExternalBankDataFieldName.IgnoreLines].ToLowerInvariant())
                    {
                        continue;
                    }
                }

                // If wrong currency, ignore

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.Currency))
                {
                    string currency = StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.Currency]]);

                    if (currency != expectedCurrencyCode)
                    {
                        continue; // ignore this record
                    }
                }

                ExternalBankDataRecord newRecord = new ExternalBankDataRecord();
                newRecord.RawData = lineData;

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.DescriptionPrimary))
                {
                    newRecord.Description =
                        StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.DescriptionPrimary]]);
                }

                // If primary description is empty, try to apply a secondary description

                if (String.IsNullOrEmpty(newRecord.Description))
                {
                    if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.DescriptionSecondary))
                    {
                        newRecord.Description =
                        StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.DescriptionSecondary]]);
                    }
                }

                // Check if there's counterparty information

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.CounterpartyName))
                {
                    string counterpartyName =
                        StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.CounterpartyName]]);

                    if (newRecord.Description != counterpartyName)
                    {
                        newRecord.CounterpartyName = counterpartyName;
                    }
                }

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.CounterpartyBank))
                {
                    string counterpartyBank =
                        StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.CounterpartyBank]]);

                    if (!string.IsNullOrEmpty(counterpartyBank))
                    {
                        newRecord.CounterpartyBankAccount = counterpartyBank;
                    }
                }

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.CounterpartyAccount))
                {
                    string counterpartyAccount =
                        StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.CounterpartyAccount]]);

                    if (!string.IsNullOrEmpty(counterpartyAccount))
                    {
                        if (!string.IsNullOrEmpty(newRecord.CounterpartyBankAccount))
                        {
                            newRecord.CounterpartyBankAccount += "|";
                        }

                        newRecord.CounterpartyBankAccount += counterpartyAccount;
                    }
                }



                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.AccountBalance))
                {
                    // Dividing up to step-by-step statements instead of one long statement assists debugging
                    // of culture and other error sources

                    string balanceString =
                        StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.AccountBalance]]);
                    try
                    {
                        newRecord.AccountBalanceCents = ParseAmountString(balanceString);
                    }
                    catch (Exception innerException)
                    {
                        throw new FormatException("Couldn't parse account balance string - \"" + balanceString + "\"",
                            innerException);
                    }
                }

                if (!fieldNameLookup.ContainsKey(ExternalBankDataFieldName.Date) &&
                    !fieldNameLookup.ContainsKey(ExternalBankDataFieldName.DateTime))
                {
                    throw new InvalidOperationException("Cannot parse transactions file without at least a date field");
                }

                DateTime dateTime = Constants.DateTimeLow;

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.Date))
                {
                    string dateString = StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.Date]]);

                    if (!String.IsNullOrEmpty(Profile.DateTimeCustomFormatString))
                    {
                        dateTime = DateTime.ParseExact(dateString, Profile.DateTimeCustomFormatString, new CultureInfo(Profile.Culture));
                    }
                    else
                    {
                        dateTime = DateTime.Parse(dateString, new CultureInfo(Profile.Culture));
                    }

                    if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.Time))
                    {
                        string timeString = StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.Time]]);
                        TimeSpan timeOfDay = TimeSpan.Parse(timeString);

                        dateTime += timeOfDay;
                    }
                    else
                    {
                        // move transaction to like mid-day of the organization's time zone. For now, all orgs are in Europe, so add 12 hours
                        // this is a HACK HACK HACK

                        dateTime = dateTime.AddHours(12);
                    }
                }
                else // no Date field, so by earlier logic, must have a DateTime field
                {
                    dateTime =
                        DateTime.Parse(StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.DateTime]]),
                            new CultureInfo(Profile.Culture));
                }

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.TimeZone))
                {
                    // Valid time zone formats are "XXX+hh:mm". The XXX are ignored.

                    // Throws exception if this doesn't parse, which is what we want

                    string timeZoneString = StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.TimeZone]]);
                    timeZoneString = timeZoneString.Substring(timeZoneString.Length - 6);
                    TimeSpan timeZone = TimeSpan.Parse(timeZoneString);

                    dateTime -= timeZone;
                    // minus, to bring the time to UTC. If time 13:00 is in tz +01:00, the UTC time is 12:00

                    // Then move from UTC to local time, to match various bookkeeping laws. TODO: local time per organization, perhaps?

                    dateTime = dateTime.ToLocalTime();
                }

                // PILOT SPECIAL CASE: if Paypal and PPSE Pilot program, ignore everything before 2014

                if (dateTime.Year < 2014 && organization.Identity == 1 &&
                    fieldNameLookup.ContainsKey(ExternalBankDataFieldName.UniqueId) &&
                    PilotInstallationIds.IsPilot(PilotInstallationIds.PiratePartySE))
                {
                    continue; // Do not import PayPal records from before 2013
                }

                newRecord.DateTime = dateTime;

                if (!fieldNameLookup.ContainsKey(ExternalBankDataFieldName.TransactionNet))
                {
                    throw new ArgumentException("There must be a transaction amount field in the bank data profile");
                }

                string amountNetString =
                    StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.TransactionNet]]);

                newRecord.TransactionNetCents = ParseAmountString(amountNetString);

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.TransactionGross))
                {
                    string amountGrossString =
                        StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.TransactionGross]]);
                    newRecord.TransactionGrossCents = ParseAmountString(amountGrossString);

                    if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.TransactionFee))
                    {
                        string amountFeeString =
                            StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.TransactionFee]]);
                        newRecord.FeeCents = ParseAmountString(amountFeeString);

                        if (Profile.FeeSignage == FeeSignage.Positive)
                        {
                            newRecord.FeeCents = -newRecord.FeeCents;
                        }
                    }
                }
                else
                {
                    newRecord.TransactionGrossCents = newRecord.TransactionNetCents;
                    // if no "Gross" field, copy from net
                }

                // Check for consistency of gross/net/fee:

                if (newRecord.TransactionNetCents != newRecord.TransactionGrossCents + newRecord.FeeCents)
                {
                    throw new InvalidDataException(
                        "For a record, the net transaction amount does not match the gross less the fee.");
                }

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.UniqueId))
                {
                    newRecord.UniqueId = StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.UniqueId]]);
                }
                else if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.NotUniqueId))
                {
                    newRecord.NotUniqueId =
                        StripQuotes(lineFields[fieldNameLookup[ExternalBankDataFieldName.NotUniqueId]]);
                }

                recordList.Add(newRecord);
            }

            if (Profile.LatestTransactionLocation == LatestTransactionLocation.Top)
            {
                this.LatestAccountBalanceCents = recordList[0].AccountBalanceCents;
            }
            else if (Profile.LatestTransactionLocation == LatestTransactionLocation.Bottom)
            {
                this.LatestAccountBalanceCents = recordList[recordList.Count - 1].AccountBalanceCents;
            }
            else
            {
                throw new ArgumentException("LatestTransactionLocation is undefined");
            }

            recordList.Sort(new ExternalBankDataRecord());

            Records = recordList.ToArray();
        }


        private Int64 ParseAmountString(string input)
        {
            // This parser deals with a number of different cases and cultures.
            // It returns cents.

            // "1"         => 100
            // "1.00"      => 100
            // "1,00"      => 100
            // "100"       => 10,000
            // "1,000"     => 100,000
            // "1,000.00"  => 100,000
            // "1 000,00"  => 100,000

            // Normally, this could be parsed as a double, but your typical Pentium
            // errors in double handling creates rare mismatches of single cents.
            // Therefore, must be parsed as an int.

            char testDecimalSeparator = '1'; // dummy non-separator

            if (input.Length > 3)
            {
                testDecimalSeparator = input[input.Length - 3];
            }

            if (testDecimalSeparator != '.' && testDecimalSeparator != ',')
            {
                // No known decimal separator where one is expected for monetary amounts, 
                // so multiply amount by 100 by adding two zeroes to string, as we're supposed
                // to be returning cents

                bool weirdCaseTriggered = false;

                if (input.Length > 2)
                {
                    testDecimalSeparator = input[input.Length - 2];

                    if (testDecimalSeparator == '.' || testDecimalSeparator == ',')
                    {
                        // Fucking SEB doesn't even have the manners to only have whole units or cents,
                        // but can present a half-crown as ",5". Damn them...

                        input += "0";
                        weirdCaseTriggered = true;
                    }
                }

                if (!weirdCaseTriggered)
                {
                    input += "00";
                }
            }


            // remove all known noise from the digit sequence: all whitespace, thousands accents, commas, periods

            input = input.RemoveAllWhitespace();
            input = input.Replace("'", "").Replace(",", "").Replace(".", "");

            // parse what's remaining as cents in Int64

            return Int64.Parse(input);
        }

        private string StripQuotes(string input)
        {
            input = input.Trim();

            if (input.Length > 1)
            {
                if (input[0] == '\"')
                {
                    if (input.EndsWith("\""))
                    {
                        return input.Trim('\"', ' ');
                    }
                }
            }

            return input;
        }
    }


    // Below: Helper classes for the frontend that locates mismatches with the bank master records.


    public class ExternalBankMismatchingDateTime
    {
        public DateTime DateTime;
        public long MasterDeltaCents;
        public int MasterTransactionCount;
        public ExternalBankMismatchingRecordDescription[] MismatchingRecords;
        public long SwarmopsDeltaCents;
        public int SwarmopsTransactionCount;
    }

    public class ExternalBankMismatchingRecordDescription
    {
        public string Description;
        public long[] MasterCents; // may have multiple txs with this description
        public ExternalBankMismatchResyncAction[] ResyncActions;
        public long[] SwarmopsCents; // may have multiple txs with this description
        // public object[] TransactionDependencies;    // matching the Swarmops array
        public FinancialTransaction[] Transactions; // matching the Swarmops array

        public ExternalBankMismatchingRecordDescription()
        {
            this.Description = string.Empty;
            this.MasterCents = new long[0];
            this.SwarmopsCents = new long[0];
            // this.TransactionDependencies = new object[0];
            this.ResyncActions = new ExternalBankMismatchResyncAction[0];
        }
    }

    public class ExternalBankMismatchConstruction
    {
        public Dictionary<string, ExternalBankMismatchingRecordConstruction> Master;
        public Dictionary<string, ExternalBankMismatchingRecordConstruction> Swarmops;

        public ExternalBankMismatchConstruction()
        {
            this.Master = new Dictionary<string, ExternalBankMismatchingRecordConstruction>();
            this.Swarmops = new Dictionary<string, ExternalBankMismatchingRecordConstruction>();
        }
    }

    public class ExternalBankMismatchingRecordConstruction
    {
        public List<long> Cents;
        public List<FinancialTransaction> Transactions;

        public ExternalBankMismatchingRecordConstruction()
        {
            this.Cents = new List<long>();
            this.Transactions = new List<FinancialTransaction>();
        }
    }

    public enum ExternalBankMismatchResyncAction
    {
        Unknown = 0,

        /// <summary>
        ///     No action necessary
        /// </summary>
        NoAction,

        /// <summary>
        ///     Zero out and rewrite the Swarmops database transaction
        /// </summary>
        RewriteSwarmops,

        /// <summary>
        ///     Zero out the Swarmops database transaction (it's not in the master)
        /// </summary>
        DeleteSwarmops,

        /// <summary>
        ///     Create a Swarmops transaction (master exists, not in Swarmops)
        /// </summary>
        CreateSwarmops,

        /// <summary>
        ///     No resync possible - manual action required
        /// </summary>
        ManualAction
    }
}