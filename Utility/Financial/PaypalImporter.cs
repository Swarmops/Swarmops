using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.Financial
{
    public class PaypalImporter
    {
        public static void Run (string data, Organization organization, Person runningPerson)
        {
            if (!data.StartsWith ("Date\t Time\t Time Zone\t Name\t Type"))
            {
                runningPerson.SendPhoneMessage (
                    "The file you uploaded does not appear to be a PayPal tab-delimited file of all activity. No processing done. The data has been discarded.");
                throw new ArgumentException ("This does not appear to be a PayPal file");
            }

            ImportResult result = ImportPaypal (data);

            ImportStats stats = ProcessImportedData (result, organization, runningPerson);

            try
            {
                runningPerson.SendPhoneMessage ("The PayPal file was processed. See mail for more details.");
            }
            catch (Exception)
            {
                // Ignore error on SMS transmit
            }

            string mailBody = string.Empty;

            mailBody += String.Format ("Rows processed: {0,9:N0}\r\n", stats.ProcessedTransactionCount);
            mailBody += String.Format ("Tx imported:    {0,9:N0}\r\n", stats.ImportedTransactionCount);
            mailBody += String.Format ("Tx modified:    {0,9:N0}\r\n",
                stats.ModifiedTransactionCount - stats.ImportedTransactionCount);

            runningPerson.SendNotice ("PayPal file imported", mailBody, organization.Identity);
        }


        protected static ImportResult ImportPaypal (string contents)
        {
            string[] lines = contents.Split ('\n');
            ImportResult result = new ImportResult();
            List<ImportedRow> rows = new List<ImportedRow>();

            foreach (string line in lines)
            {
                string[] parts = line.Split ('\t');

                if (parts.Length < 30)
                {
                    continue;
                }

                if (StripQuotes (parts[6]) != "SEK")
                {
                    continue; // HACK: Need to fix currency support at some time
                }

                // Get current balance from the first line in the file

                if (result.CurrentBalance == 0.0)
                {
                    result.CurrentBalance = Double.Parse (StripQuotes (parts[34]), CultureInfo.InvariantCulture);
                }

                ImportedRow row = new ImportedRow();

                // DEBUG -- REMOVE WHEN DEPLOYING

                if (Debugger.IsAttached)
                {
                    Console.WriteLine ("New Row -----");

                    Console.WriteLine ("- SuppliedTxId: {0}", parts[12]);
                    Console.WriteLine ("- Comment:      {0}", parts[4]);
                    Console.WriteLine ("- DateTime:     {0} {1}", parts[0], parts[1]);
                    Console.WriteLine ("- AmountGross:  {0}", parts[7]);
                    Console.WriteLine ("- Fee:          {0}", parts[8]);
                    Console.WriteLine ("- AmountNet:    {0}", parts[9]);
                }

                row.SuppliedTransactionId = StripQuotes (parts[12]);
                row.Comment = StripQuotes (parts[4]);
                row.DateTime = DateTime.Parse (StripQuotes (parts[0]) + " " + StripQuotes (parts[1]),
                    CultureInfo.InvariantCulture);
                row.AmountCentsGross = Int64.Parse (StripQuotes (parts[7]).Replace (".", "").Replace (",", ""));
                row.FeeCents = Int64.Parse (StripQuotes (parts[8]).Replace (".", "").Replace (",", ""));
                row.AmountCentsNet = Int64.Parse (StripQuotes (parts[9]).Replace (".", "").Replace (",", ""));

                rows.Add (row);
            }

            result.Rows = rows;
            return result;
        }


        protected static string StripQuotes (string input)
        {
            if (input.StartsWith ("\""))
            {
                input = input.Substring (1);
            }

            if (input.EndsWith ("\""))
            {
                input = input.Substring (0, input.Length - 1);
            }

            return input;
        }


        protected static ImportStats ProcessImportedData (ImportResult import, Organization organization,
            Person importingPerson)
        {
            FinancialAccount payPalAccount = organization.FinancialAccounts.AssetsPaypal;
            FinancialAccount bankFees = organization.FinancialAccounts.CostsBankFees;
            FinancialAccount donations = organization.FinancialAccounts.IncomeDonations;

            int autoDepositLimit = 1000; // TODO: Get from organization parameters
            int autoWithdrawalLimit = 0;

            ImportStats result = new ImportStats();

            foreach (ImportedRow row in import.Rows)
            {
                // Each row is at least a stub, probably more.

                // If too old, ignore.

                if (row.DateTime < new DateTime (2008, 12, 4))
                {
                    continue;
                }

                string importKey = row.SuppliedTransactionId;

                // If importKey is empty, construct a hash from the data fields.

                if (string.IsNullOrEmpty (importKey))
                {
                    string hashKey = row.HashBase + row.Comment +
                                     (row.AmountCentsNet/100.0).ToString (CultureInfo.InvariantCulture) +
                                     row.CurrentBalance.ToString (CultureInfo.InvariantCulture) +
                                     row.DateTime.ToString ("yyyy-MM-dd-hh-mm-ss");

                    importKey = SHA1.Hash (hashKey).Replace (" ", "");
                }

                if (importKey.Length > 30)
                {
                    importKey = importKey.Substring (0, 30);
                }

                Int64 amountCents = row.AmountCentsNet;

                if (amountCents == 0)
                {
                    amountCents = row.AmountCentsGross;
                }

                Dictionary<int, long> nominalTransaction = new Dictionary<int, long>();

                FinancialTransaction transaction = null;

                try
                {
                    transaction = FinancialTransaction.FromImportKey (organization, importKey);
                }
                catch (Exception)
                {
                    // if we get here, that means the transaction did not yet exist

                    transaction = FinancialTransaction.ImportWithStub (organization.Identity, row.DateTime,
                        payPalAccount.Identity, amountCents,
                        row.Comment, importKey,
                        importingPerson.Identity);
                    result.ImportedTransactionCount++;

                    if (transaction == null)
                    {
                        // No transaction was created. This is an error condition as it should have been created if it didn't
                        // exist, and the "exist" case is handled in the FromImportKey attempt above. Abort with error.
                        // Throw new exception?

                        continue;
                    }
                }

                result.ProcessedTransactionCount++;

                nominalTransaction[payPalAccount.Identity] = amountCents;

                // The transaction was created. Examine if the autobook criteria are true.

                if (amountCents < 0)
                {
                    if ((-amountCents) < autoWithdrawalLimit*100)
                    {
                        // Book against autoWithdrawal account.

                        nominalTransaction[bankFees.Identity] = -amountCents;
                    }
                }
                else if (amountCents > 0)
                {
                    if (row.FeeCents < 0)
                    {
                        // This is always an autodeposit, if there is a fee (which is never > 0.0)

                        nominalTransaction[bankFees.Identity] = -row.FeeCents;
                        nominalTransaction[donations.Identity] = -row.AmountCentsGross;
                    }
                    else if (amountCents < autoDepositLimit*100)
                    {
                        // Book against autoDeposit account.

                        nominalTransaction[donations.Identity] = -amountCents;
                    }
                }

                if (transaction.Rows.AmountCentsTotal != 0) // If transaction is unbalanced, balance it
                {
                    if (transaction.RecalculateTransaction (nominalTransaction, importingPerson))
                    {
                        result.ModifiedTransactionCount++;
                    }
                }
            }

            return result;
        }

        protected class ImportResult
        {
            public double CurrentBalance;
            public List<ImportedRow> Rows;
        }

        protected class ImportStats
        {
            public int ImportedTransactionCount;
            public int ModifiedTransactionCount;
            public int ProcessedTransactionCount;
        }

        protected class ImportedRow
        {
            public Int64 AmountCentsGross;
            public Int64 AmountCentsNet;
            public string Comment;
            public double CurrentBalance;
            public DateTime DateTime;
            public Int64 FeeCents;
            public string HashBase;
            public string SuppliedTransactionId;
        }
    }
}