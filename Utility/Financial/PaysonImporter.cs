using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.Financial
{
    public class PaysonImporter
    {
        public static void Run(string data, Organization organization, Person runningPerson)
        {
            if (!data.StartsWith("\r\n<html>\r\n<head>\r\n    <style>\r\n        .tal"))
            {
                runningPerson.SendPhoneMessage(
                    "The file you uploaded does not appear to be a Payson export file. No processing done. The data has been discarded.");
                throw new ArgumentException("This does not appear to be a Payson file");
            }

            if (organization.Identity != 1)
            {
                runningPerson.SendPhoneMessage(
                    "Payson import is currently only supported for PPSE. No processing done. The data has been discarded.");
                throw new Exception("Payson is only supported for PPSE at the moment.");
            }

            ImportResult result = ImportPayson(data);

            ImportStats stats = ProcessImportedData(result, organization, runningPerson);

            try
            {
                runningPerson.SendPhoneMessage("The Payson file was processed. See mail for more details.");
            }
            catch (Exception)
            {
                // Ignore error on SMS transmit
            }

            string mailBody = string.Empty;

            mailBody += String.Format("Rows processed: {0,9:N0}\r\n", stats.ProcessedTransactionCount);
            mailBody += String.Format("Tx imported:    {0,9:N0}\r\n", stats.ImportedTransactionCount);
            mailBody += String.Format("Tx modified:    {0,9:N0}\r\n",
                stats.ModifiedTransactionCount - stats.ImportedTransactionCount);

            runningPerson.SendNotice("Payson file imported", mailBody, organization.Identity);
        }


        protected static ImportResult ImportPayson(string contents)
        {
            string regexPattern =
                @"<tr>\s+<td>\s*(?<datetime>[0-9]{4}-[0-9]{2}-[0-9]{2}\s[0-9]{2}:[0-9]{2}:[0-9]{2})\s*</td><td>(?<comment1>[^<]*)</td><td>[^>]*</td><td>(?<txid>[0-9]+)</td>\s*<td>(?<from>[^<]+)</td>\s*<td>(?<to>[^<]+)</td><td class=\""tal\"">(?<gross>[\-0-9,]+)</td><td class=\""tal\"">(?<fee>[\-0-9,]+)</td><td class=\""tal\"">(?<vat>[\-0-9,]+)</td><td class=\""tal\"">(?<net>[\-0-9,]+)</td><td class=\""tal\"">(?<balance>[\-0-9,]+)</td><td>(?<currency>[^<]+)</td><td>(?<reference>[^<]+)</td><td[^>]+?>(?<comment2>[^<]+)</td>";

            Regex regex = new Regex(regexPattern, RegexOptions.Singleline);
            Match match = regex.Match(contents);

            ImportResult result = new ImportResult();
            List<ImportedRow> rows = new List<ImportedRow>();

            while (match.Success)
            {
                if (match.Groups["currency"].Value != "SEK")
                {
                    continue; // HACK: Need to fix currency support at some time
                }

                // Get current balance from the first line in the file

                if (result.CurrentBalance == 0.0)
                {
                    result.CurrentBalance = Int64.Parse(match.Groups["balance"].Value.Replace(",", ""))/10000.0;
                }

                ImportedRow row = new ImportedRow();

                // DEBUG -- REMOVE WHEN DEPLOYING

                if (Debugger.IsAttached)
                {
                    Console.WriteLine("New Row -----");

                    Console.WriteLine("- SuppliedTxId: {0}", match.Groups["txid"].Value);
                    Console.WriteLine("- Comment:      {0}", HttpUtility.HtmlDecode(match.Groups["comment2"].Value));
                    Console.WriteLine("- DateTime:     {0}", match.Groups["datetime"].Value);
                    Console.WriteLine("- AmountGross:  {0}", match.Groups["gross"].Value);
                    Console.WriteLine("- Fee:          {0}", match.Groups["fee"].Value);
                    Console.WriteLine("- AmountNet:    {0}", match.Groups["net"].Value);
                }

                string comment = HttpUtility.HtmlDecode(match.Groups["comment2"].Value.Trim());
                if (String.IsNullOrEmpty(comment))
                {
                    comment = match.Groups["comment1"].Value.Trim();
                }

                row.SuppliedTransactionId = "Payson-" + match.Groups["txid"].Value;
                row.Comment = comment;
                row.DateTime = DateTime.Parse(match.Groups["datetime"].Value, CultureInfo.InvariantCulture);
                row.AmountCentsGross = Int64.Parse(match.Groups["gross"].Value.Replace(".", "").Replace(",", ""))/100;
                row.FeeCents = Int64.Parse(match.Groups["fee"].Value.Replace(".", "").Replace(",", ""))/100;
                row.AmountCentsNet = Int64.Parse(match.Groups["net"].Value.Replace(".", "").Replace(",", ""))/100;

                rows.Add(row);

                match = match.NextMatch();
            }

            result.Rows = rows;
            return result;
        }


        protected static string StripQuotes(string input)
        {
            if (input.StartsWith("\""))
            {
                input = input.Substring(1);
            }

            if (input.EndsWith("\""))
            {
                input = input.Substring(0, input.Length - 1);
            }

            return input;
        }


        protected static ImportStats ProcessImportedData(ImportResult import, Organization organization,
            Person importingPerson)
        {
            FinancialAccount paysonAccount = FinancialAccount.FromIdentity(99);
                // HACK -- need something more sophisticated long run that allows different accounts for different orgs
            FinancialAccount bankFees = organization.FinancialAccounts.CostsBankFees;
            FinancialAccount donations = organization.FinancialAccounts.IncomeDonations;

            int autoDepositLimit = 1000; // TODO: Get from organization parameters
            int autoWithdrawalLimit = 0;

            ImportStats result = new ImportStats();

            foreach (ImportedRow row in import.Rows)
            {
                // Each row is at least a stub, probably more.

                // If too old, ignore.

                if (row.DateTime < new DateTime(2010, 4, 1))
                {
                    continue;
                }

                string importKey = row.SuppliedTransactionId;

                // If importKey is empty, construct a hash from the data fields.

                if (string.IsNullOrEmpty(importKey))
                {
                    string hashKey = row.HashBase + row.Comment +
                                     (row.AmountCentsNet/100.0).ToString(CultureInfo.InvariantCulture) +
                                     row.CurrentBalance.ToString(CultureInfo.InvariantCulture) +
                                     row.DateTime.ToString("yyyy-MM-dd-hh-mm-ss");

                    importKey = SHA1.Hash(hashKey).Replace(" ", "");
                }

                if (importKey.Length > 30)
                {
                    importKey = importKey.Substring(0, 30);
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
                    transaction = FinancialTransaction.FromImportKey(organization, importKey);
                }
                catch (Exception)
                {
                    // if we get here, that means the transaction did not yet exist

                    transaction = FinancialTransaction.ImportWithStub(organization.Identity, row.DateTime,
                        paysonAccount.Identity, amountCents,
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

                nominalTransaction[paysonAccount.Identity] = amountCents;

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
                    if (transaction.RecalculateTransaction(nominalTransaction, importingPerson))
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