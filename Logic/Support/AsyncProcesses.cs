using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public class AsyncProcesses
    {
        public static ImportExternalTransactionDataResults ImportExternalTransactionData(ExternalBankData import, ImportExternalTransactionDataArgs args)
        {
            FinancialAccount assetAccount = args.Account;
            FinancialAccount autoDepositAccount = args.Organization.FinancialAccounts.IncomeDonations;
            int autoDepositLimit = 0; // Disabled; TODO: this.CurrentOrganization.Parameters.AutoDonationLimit;

            bool autosetInitialBalance = false;
            ImportExternalTransactionDataResults result = new ImportExternalTransactionDataResults();
            int count = 0;
            int progressUpdateInterval = import.Records.Length/40;
            Int64 importedCentsTotal = 0;

            if (progressUpdateInterval > 100)
            {
                progressUpdateInterval = 100;
            }

            ProgressBarBackend progressDisplay = new ProgressBarBackend(args.Guid);

            Currency organizationCurrency = assetAccount.Organization.Currency;
            Currency accountCurrency = assetAccount.ForeignCurrency;

            if (accountCurrency == null)
            {
                accountCurrency = organizationCurrency;
            }

            FinancialAccountRows existingRows = assetAccount.GetRows(Constants.DateTimeLow, Constants.DateTimeHigh);
                // gets all
            if (existingRows.Count == 0)
            {
                autosetInitialBalance = true;
            }


            foreach (ExternalBankDataRecord row in import.Records)
            {
                // Update progress.

                count++;
                if (progressUpdateInterval < 2 || count%progressUpdateInterval == 0)
                {
                    int percent = (count*99)/import.Records.Length;

                    progressDisplay.Set(percent);
                }

                // Update high- and low-water marks.

                if (row.DateTime < result.EarliestTransaction)
                {
                    result.EarliestTransaction = row.DateTime;
                }

                if (row.DateTime > result.LatestTransaction)
                {
                    result.LatestTransaction = row.DateTime;
                }


                string importKey = row.ImportHash;

                Int64 amountCents = row.TransactionNetCents;

                if (amountCents == 0)
                    // defensive programming - these _should_ be duplicated in the interpreter if no "fee" field
                {
                    amountCents = row.TransactionGrossCents;
                }

                Int64 foreignCents = amountCents;
                importedCentsTotal += amountCents;

                if (accountCurrency.Identity != organizationCurrency.Identity)
                {
                    amountCents =
                        new Money(amountCents, accountCurrency, row.DateTime).ToCurrency(organizationCurrency).Cents;
                }

                FinancialTransaction transaction = FinancialTransaction.ImportWithStub(args.Organization.Identity,
                    row.DateTime,
                    assetAccount.Identity, amountCents,
                    row.Description, importKey, Sha256.Compute(row.RawData),
                    args.CurrentUser.Identity);

                if (transaction != null)
                {
                    // The transaction was created.

                    result.TransactionsImported++;

                    // If non-presentation currency, log the account currency amount as well.

                    if (accountCurrency.Identity != organizationCurrency.Identity)
                    {
                        transaction.Rows[0].AmountForeignCents = new Money(foreignCents, accountCurrency);
                    }

                    if (row.Description.ToLowerInvariant().StartsWith(args.Organization.IncomingPaymentTag))
                    {
                        // Check for previously imported payment group

                        // TODO: MAKE FLEXIBLE - CALL PAYMENTREADERINTERFACE!
                        // HACK HACK HACK HACK

                        PaymentGroup group = PaymentGroup.FromTag(args.Organization,
                            "SEBGM" + DateTime.Today.Year + // TODO: Get tags from org
                            row.Description.Substring(args.Organization.IncomingPaymentTag.Length).Trim());

                        if (group != null && group.Open)
                        {
                            // There was a previously imported and not yet closed payment group matching this transaction
                            // Close the payment group and match the transaction against accounts receivable

                            transaction.Dependency = group;
                            group.Open = false;
                            transaction.AddRow(args.Organization.FinancialAccounts.AssetsOutboundInvoices, -amountCents,
                                args.CurrentUser);
                        }
                    }
                    else if (amountCents < 0)
                    {
                        // Autowithdrawal mechanisms removed, condition kept because of downstream else-if conditions
                    }
                    else if (amountCents > 0)
                    {
                        if (row.FeeCents < 0)
                        {
                            // This is always an autodeposit, if there is a fee (which is never > 0.0)

                            transaction.AddRow(args.Organization.FinancialAccounts.CostsBankFees, -row.FeeCents,
                                args.CurrentUser);
                            transaction.AddRow(autoDepositAccount, -row.TransactionGrossCents, args.CurrentUser);
                        }
                        else if (amountCents < autoDepositLimit*100)
                        {
                            // Book against autoDeposit account.

                            transaction.AddRow(autoDepositAccount, -amountCents, args.CurrentUser);
                        }
                    }
                }
                else
                {
                    // Transaction was not imported; assume duplicate

                    result.DuplicateTransactions++;
                }
            }

            // Import complete. Return true if the bookkeeping account matches the bank data.

            Int64 databaseAccountBalanceCents;

            if (accountCurrency.Identity == organizationCurrency.Identity)
            {
                databaseAccountBalanceCents = assetAccount.BalanceTotalCents;
            }
            else
            {
                // foreign-currency account
                databaseAccountBalanceCents = assetAccount.ForeignCurrencyBalance.Cents;
            }


            // Subtract any transactions made after the most recent imported transaction.
            // This is necessary in case of Paypal and others which continuously feed the
            // bookkeeping account with new transactions; it will already have fed transactions
            // beyond the end-of-file.

            Int64 beyondEofCents = assetAccount.GetDeltaCents(result.LatestTransaction.AddSeconds(1),
                DateTime.Now.AddDays(2));
            // Caution: the "AddSeconds(1)" is not foolproof, there may be other new txs on the same second.

            if (databaseAccountBalanceCents - beyondEofCents == import.LatestAccountBalanceCents)
            {
                Payouts.AutomatchAgainstUnbalancedTransactions(args.Organization);
                OutboundInvoices.AutomatchAgainstUnbalancedTransactions(args.Organization, args.CurrentUser);
                result.AccountBalanceMatchesBank = true;
                result.BalanceMismatchCents = 0;
            }
            else
            {
                result.AccountBalanceMatchesBank = false;
                result.BalanceMismatchCents = (databaseAccountBalanceCents - beyondEofCents) -
                                              import.LatestAccountBalanceCents;

                if (autosetInitialBalance)
                {
                    Int64 newInitialBalanceCents = -result.BalanceMismatchCents;
                    Money initialBalance = new Money(newInitialBalanceCents, accountCurrency);

                    assetAccount.InitialBalance = initialBalance;
                    result.InitialBalanceCents = newInitialBalanceCents;
                    result.InitialBalanceCurrencyCode = accountCurrency.Code;

                    // make an approximation of conversion rate set for initial balance in presentation to tell user
                    initialBalance.ValuationDateTime = new DateTime(assetAccount.Organization.FirstFiscalYear, 1, 1);
                    result.BalanceMismatchCents = initialBalance.ToCurrency(assetAccount.Organization.Currency).Cents;
                }
            }

            result.CurrencyCode = args.Organization.Currency.Code;
            GuidCache.Set(args.Guid + "-Results", result);
            return result;
        }

        public class ImportExternalTransactionDataArgs
        {
            public string Guid { get; set; }
            public Organization Organization { get; set; }
            public FinancialAccount Account { get; set; }
            public Person CurrentUser { get; set; }
        }
    }

    [Serializable]
    public class ImportExternalTransactionDataResults
    {
        public bool AccountBalanceMatchesBank;
        public long BalanceMismatchCents;
        public string CurrencyCode;
        public int DuplicateTransactions;
        public DateTime EarliestTransaction;
        public DateTime LatestTransaction;
        public int TransactionsImported;
        public Int64 InitialBalanceCents;
        public string InitialBalanceCurrencyCode;

        public ImportExternalTransactionDataResults()
        {
            this.EarliestTransaction = Constants.DateTimeHigh;
            this.LatestTransaction = Constants.DateTimeLow;
        }
    }
}
