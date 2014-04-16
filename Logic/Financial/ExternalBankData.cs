using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Financial
{
    public class ExternalBankData
    {
        public void LoadData(Stream dataStream, Organization organization)
        {
            using (TextReader reader = new StreamReader(dataStream))  // TODO: Is encoding necessary?
            {
                LoadData(reader, organization);
            }
        }

        public void LoadData (TextReader reader, Organization organization)
        {
            string data = reader.ReadToEnd();
            LoadData(data, organization);
        }

        public void LoadData(string data, Organization organization)
        {
            List<ExternalBankDataRecord> recordList = new List<ExternalBankDataRecord>();

            if (Profile == null)
            {
                throw new InvalidOperationException("Cannot call LoadData before a profile has been set");
            }

            string organizationCurrencyCode = organization.Currency.Code;

            int crlfIndex = data.IndexOfAny(new char[] {'\n', '\r'});

            string fieldKeyLine = data.Substring(0, crlfIndex);

            string[] dataKeyFields = fieldKeyLine.Split('\t');

            Dictionary<ExternalBankDataFieldName, int> fieldNameLookup =
                new Dictionary<ExternalBankDataFieldName, int>();

            // below is an N^2 loop but doesn't matter in such a small context

            foreach (ExternalBankDataFieldName fieldName in Profile.FieldNames.Keys)
            {
                for (int index = 0; index < dataKeyFields.Length; index++)
                {
                    if (dataKeyFields[index].Trim() == Profile.FieldNames[fieldName])
                    {
                        fieldNameLookup[fieldName] = index;
                        break;
                    }
                }

                if (!fieldNameLookup.ContainsKey(fieldName))   // wasn't found
                {
                    throw new ArgumentException("Field key \"" + fieldName + "\" was not supplied or found in data file");
                }
            }
            

            data = data.Substring(crlfIndex).Trim();

            string[] lines = data.Split(new char[] {'\r', '\n'});

            foreach (string lineData in lines)
            {
                string line = lineData.Trim();

                if (line.Length < 1)
                {
                    continue; // empty lines may exist due to split on either CR or LF
                }

                string[] lineFields = line.Split('\t');

                // If wrong currency, ignore

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.Currency))
                {
                    string currency = lineFields[fieldNameLookup[ExternalBankDataFieldName.Currency]];

                    if (currency != organizationCurrencyCode)
                    {
                        continue; // ignore this record
                    }
                }


                ExternalBankDataRecord newRecord = new ExternalBankDataRecord();

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.Description))
                {
                    newRecord.Description = lineFields[fieldNameLookup[ExternalBankDataFieldName.Description]];
                }

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.AccountBalance))
                {
                    // Dividing up to step-by-step statements instead of one long statement assists debugging
                    // of culture and other error sources

                    string balanceString = lineFields[fieldNameLookup[ExternalBankDataFieldName.AccountBalance]];
                    double parsedBalance = Double.Parse(balanceString, NumberStyles.Currency, new CultureInfo(Profile.Culture));
                    newRecord.AccountBalanceCents = (long) Math.Floor(parsedBalance*100.0);
                }

                if (!fieldNameLookup.ContainsKey(ExternalBankDataFieldName.Date) && !fieldNameLookup.ContainsKey(ExternalBankDataFieldName.DateTime))
                {
                    throw new InvalidOperationException("Cannot parse transactions file without at least a date field");
                }

                DateTime dateTime = DateTime.MinValue;

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.Date))
                {
                    string dateString = lineFields[fieldNameLookup[ExternalBankDataFieldName.Date]];
                    dateTime = DateTime.Parse(dateString, new CultureInfo(Profile.Culture));

                    if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.Time))
                    {
                        string timeString = lineFields[fieldNameLookup[ExternalBankDataFieldName.Time]];
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
                    dateTime = DateTime.Parse(lineFields[fieldNameLookup[ExternalBankDataFieldName.DateTime]], new CultureInfo(Profile.Culture));
                }

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.TimeZone))
                {
                    // Valid time zone formats are "XXX+hh:mm". The XXX are ignored.

                    // Throws exception if this doesn't parse, which is what we want

                    TimeSpan timeZone =
                        TimeSpan.Parse(lineFields[fieldNameLookup[ExternalBankDataFieldName.TimeZone]].Substring(4));

                    dateTime -= timeZone;  // minus, to bring the time to UTC. If time 13:00 is in tz +01:00, the UTC time is 12:00
                }

                newRecord.DateTime = dateTime;

                if (!fieldNameLookup.ContainsKey(ExternalBankDataFieldName.TransactionNet))
                {
                    throw new ArgumentException("There must be a transaction amount field in the bank data profile");
                }

                string amountNetString = lineFields[fieldNameLookup[ExternalBankDataFieldName.TransactionNet]];
                double amountNetDouble = Double.Parse(amountNetString, NumberStyles.Currency,
                                                      new CultureInfo(Profile.Culture));

                newRecord.TransactionNetCents = (long) (amountNetDouble*100.0);

                // TODO: Net, fee

                if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.UniqueId))
                {
                    newRecord.UniqueId = lineFields[fieldNameLookup[ExternalBankDataFieldName.UniqueId]];
                }
                else if (fieldNameLookup.ContainsKey(ExternalBankDataFieldName.NotUniqueId))
                {
                    newRecord.NotUniqueId = lineFields[fieldNameLookup[ExternalBankDataFieldName.NotUniqueId]];
                }

                recordList.Add(newRecord);
            }

            if (Profile.LatestTransactionLocation == LatestTransactionLocation.Top)
            {
                LatestAccountBalanceCents = recordList[0].AccountBalanceCents;
            }
            else if (Profile.LatestTransactionLocation == LatestTransactionLocation.Bottom)
            {
                LatestAccountBalanceCents = recordList [recordList.Count- 1].AccountBalanceCents;
            }
            else
            {
                throw new ArgumentException("LatestTransactionLocation is undefined");
            }

            recordList.Sort(new ExternalBankDataRecord());

            this.Records = recordList.ToArray();
        }

        public ExternalBankDataProfile Profile { get; set; }
        public ExternalBankDataRecord[] Records { get; private set; }
        public long LatestAccountBalanceCents;
    }


    // Helper datakeeping classes for frontend below


    public class ExternalBankMismatchingDateTime
    {
        public DateTime DateTime;
        public int MasterTransactionCount;
        public long MasterDeltaCents;
        public int SwarmopsTransactionCount;
        public long SwarmopsDeltaCents;
    }
}
