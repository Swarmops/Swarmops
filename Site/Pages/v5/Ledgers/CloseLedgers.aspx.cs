﻿using System;
using System.Web.UI;
using Swarmops.Common.Enums;
using Swarmops.Frontend;
using Swarmops.Localization;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

public partial class Pages_v5_Ledgers_CloseLedgers : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write);

        PageTitle = "Close Ledgers";
        PageIcon = "iconshock-calculator-lock";

        // Check if on a closable year

        if (CurrentOrganization.Parameters.EconomyEnabled == false ||
            CurrentOrganization.Parameters.FiscalBooksClosedUntilYear == DateTime.Today.Year - 1)
        {
            this.PanelCannotClose.Visible = true;
            this.PanelSuccess.Visible = false;
            this.LabelCannotCloseLedgersReason.Text = "Ledgers are already closed as far as possible. [LOC]";
            return; // a return out of Page_Load is kind of unusual, see it as a "break" or "abort"
        }

        // Check if all transactions are balanced, so we can close

        FinancialTransactions unbalancedTransactions = FinancialTransactions.GetUnbalanced (CurrentOrganization);
        // TODO: this fn should move to Organization

        int closingYear = CurrentOrganization.Parameters.FiscalBooksClosedUntilYear + 1;
        if (closingYear < 1000)
        {
            closingYear = CurrentOrganization.FirstFiscalYear;
        }


        bool hasOpenTxForClosingYear = false;

        foreach (FinancialTransaction unbalancedTransaction in unbalancedTransactions)
        {
            if (unbalancedTransaction.DateTime.Year <= closingYear)
            {
                hasOpenTxForClosingYear = true;
            }
        }

        if (hasOpenTxForClosingYear)
        {
            this.PanelCannotClose.Visible = true;
            this.PanelSuccess.Visible = false;
            return; // a return out of Page_Load is kind of unusual, see it as a "break" or "abort"
        }

        // Start actually closing the ledgers


        // First, roll over virtual balances.

        //if (false) // if this.CurrentOrganization.Parameters.VirtualBankingEnabled
        //{
        //    FinancialAccount rootAccount = FinancialAccount.FromIdentity(29);  // HACK: Hardcoded account; should be _organization.FinancialAccount.CostsVirtualBankingRoot
        //    FinancialAccount tempAccount = FinancialAccount.FromIdentity(98);  // HACK: Hardcoded account; should be _organization.FinancialAccount.AssetsVirtualRollover

        //    FinancialAccounts localAccounts = rootAccount.GetTree();

        //    foreach (FinancialAccount account in localAccounts)
        //    {
        //        Int64 currentBalanceCents = account.GetDeltaCents(new DateTime(closingYear, 1, 1), new DateTime(closingYear+1, 1, 1));
        //        Int64 budgetCents = -account.GetBudgetCents(closingYear);
        //        Int64 carryOverCents = budgetCents - currentBalanceCents;

        //        if (carryOverCents != 0)
        //        {
        //            FinancialTransaction transactionOldYear = FinancialTransaction.Create(1,
        //                                                                                  new DateTime(closingYear, 12,
        //                                                                                               31, 23, 50,
        //                                                                                               00),
        //                                                                                  "Budgetrest " + account.Name);
        //                // HACK: Localize rollover label

        //            transactionOldYear.AddRow(account, carryOverCents, null);
        //            transactionOldYear.AddRow(tempAccount, -carryOverCents, null);

        //            FinancialTransaction transactionNewYear = FinancialTransaction.Create(1,
        //                                                                                  new DateTime(closingYear + 1,
        //                                                                                               1, 1, 0, 10, 0),
        //                                                                                  "Budgetrest " +
        //                                                                                  closingYear.ToString() + " " +
        //                                                                                  account.Name);

        //            transactionNewYear.AddRow(account, -carryOverCents, null);
        //            transactionNewYear.AddRow(tempAccount, carryOverCents, null);
        //        }
        //    }
        //}

        // Then, actually close the ledgers.

        FinancialAccounts accounts = FinancialAccounts.ForOrganization (CurrentOrganization);
        Int64 balanceDeltaCents = 0;
        Int64 resultsDeltaCents = 0;

        foreach (FinancialAccount account in accounts)
        {
            Int64 accountBalanceCents;

            if (account.AccountType == FinancialAccountType.Asset || account.AccountType == FinancialAccountType.Debt)
            {
                accountBalanceCents = account.GetDeltaCents (new DateTime (2006, 1, 1),
                    new DateTime (closingYear + 1, 1, 1));
                balanceDeltaCents += accountBalanceCents;
            }
            else
            {
                accountBalanceCents = account.GetDeltaCents (new DateTime (closingYear, 1, 1),
                    new DateTime (closingYear + 1, 1, 1));
                resultsDeltaCents += accountBalanceCents;
            }
        }

        if (balanceDeltaCents == -resultsDeltaCents && closingYear < DateTime.Today.Year)
        {
            string transactionLabel = LocalizedStrings.Get(LocDomain.PagesLedgers, "CloseLedgers_AnnualProfit");

            if (balanceDeltaCents < 0)
            {
                transactionLabel = LocalizedStrings.Get(LocDomain.PagesLedgers, "CloseLedgers_AnnualLoss");
            }

            FinancialTransaction resultTransaction = FinancialTransaction.Create (CurrentOrganization.Identity,
                new DateTime (closingYear, 12, 31, 23, 59, 00), transactionLabel +  " " + closingYear);

            Int64 privateWithdrawals = 0;
            Int64 privateDeposits = 0;

            if (CurrentOrganization.FinancialAccounts.DebtsPrivateDeposits != null &&
                CurrentOrganization.FinancialAccounts.AssetsPrivateWithdrawals != null)
            {
                // TODO: Reset trees if there are subaccounts

                privateWithdrawals =
                    CurrentOrganization.FinancialAccounts.AssetsPrivateWithdrawals
                        .BalanceTotalCents;
                privateDeposits =
                    CurrentOrganization.FinancialAccounts.DebtsPrivateDeposits
                        .BalanceTotalCents;

            }

            resultTransaction.AddRow (CurrentOrganization.FinancialAccounts.CostsYearlyResult, -resultsDeltaCents, CurrentUser);
            resultTransaction.AddRow (CurrentOrganization.FinancialAccounts.DebtsEquity, -balanceDeltaCents + privateWithdrawals + privateDeposits, CurrentUser);

            if (privateWithdrawals != 0)
            {
                resultTransaction.AddRow(CurrentOrganization.FinancialAccounts.AssetsPrivateWithdrawals,
                    -privateWithdrawals, CurrentUser);
            }
            if (privateDeposits != 0)
            {
                resultTransaction.AddRow(CurrentOrganization.FinancialAccounts.DebtsPrivateDeposits, -privateDeposits, CurrentUser)
            }

            // Ledgers are now at zero-sum for the year's result accounts and from the start up until end-of-closing-year for the balance accounts.

            CurrentOrganization.Parameters.FiscalBooksClosedUntilYear = closingYear;
        }
        else
        {
            Console.WriteLine ("NOT creating transaction.");
        }
    }
}