using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class Json_OutstandingAccounts : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.Financials, AccessType.Read);
            _authenticationData = GetAuthenticationDataAndCulture();

            OutstandingAccountType accountType = OutstandingAccountType.ExpenseClaims;
            DateTime targetDateTime = DateTime.Today.AddDays(1);
            bool renderPresentTime = true;
            FinancialAccount balanceAccount = null;
            Int64 ledgerExpectedCents;
            OutstandingAccounts outstandingAccounts = null;
            bool reverseLedgerSign = false;

            if (Request.QueryString ["Year"] != null)
            {
                targetDateTime = _authenticationData.CurrentOrganization.GetEndOfFiscalYear (Int32.Parse((string) Request.QueryString["Year"], CultureInfo.InvariantCulture));
                renderPresentTime = false;
            }

            if (Request.QueryString ["AccountType"] != null)
            {
                accountType = (OutstandingAccountType)Enum.Parse(typeof(OutstandingAccountType), Request.QueryString["AccountType"]); // Will throw on invalid input. Sucks to be hacker-wannabe
            }


            switch (accountType)
            {
                case OutstandingAccountType.ExpenseClaims:
                    balanceAccount = _authenticationData.CurrentOrganization.FinancialAccounts.DebtsExpenseClaims;
                    outstandingAccounts = GetOutstandingExpenseClaims(renderPresentTime, targetDateTime);
                    reverseLedgerSign = true; // Expenses is debt and negative in ledger
                    break;

                case OutstandingAccountType.CashAdvances:
                    outstandingAccounts = GetOutstandingCashAdvances(renderPresentTime, targetDateTime);
                    balanceAccount =
                        _authenticationData.CurrentOrganization.FinancialAccounts.AssetsOutstandingCashAdvances;
                    break;
                default:
                    throw new NotImplementedException("Unimplemented Outstanding Account Type");
            }

            if (renderPresentTime)
            {
                ledgerExpectedCents = balanceAccount.GetDeltaCents(DateTime.MinValue, DateTime.MaxValue); // get ALL transactions
            }
            else
            {
                ledgerExpectedCents = balanceAccount.GetDeltaCents(DateTime.MinValue, targetDateTime);
            }

            if (reverseLedgerSign)
            {
                ledgerExpectedCents = -ledgerExpectedCents;
            }


            Response.ContentType = "application/json";
            Response.Output.WriteLine (FormatJson(outstandingAccounts, ledgerExpectedCents));
            Response.End();
        }


        private OutstandingAccounts GetOutstandingExpenseClaims(bool renderPresentTime, DateTime targetDateTime)
        {
            OutstandingAccounts outstandingAccounts = new OutstandingAccounts();

            if (renderPresentTime)
            {
                ExpenseClaims claims = ExpenseClaims.ForOrganization(_authenticationData.CurrentOrganization);
                Payouts payouts = Payouts.ForOrganization(_authenticationData.CurrentOrganization);

                foreach (ExpenseClaim claim in claims)
                {
                    outstandingAccounts.Add(OutstandingAccount.FromExpenseClaim(claim, DateTime.MinValue));
                }
                foreach (Payout payout in payouts)
                {
                    foreach (ExpenseClaim claim in payout.DependentExpenseClaims)
                    {
                        outstandingAccounts.Add(OutstandingAccount.FromExpenseClaim(claim,
                                                                                    payout.ExpectedTransactionDate));
                    }
                }
            }
            else
            {
                // This is a very expensive op. We need to load all expense claims and process them in logic, looking at their
                // payouts and end financial transactions, as the traceability was built to be efficient backwards (tracing money
                // paid out to its documentation), rather than forwards.

                // A possible optimization could be to load the payouts into a hash table initially instead of looking them up
                // with two dbroundtrips per expense claim. It should be more efficient to have three dbroundtrips to load expenses,
                // payouts, and the relevant transactions, then stuff it all into hash tables keyed by identity and process it
                // in-memory.

                // A future optimization involves adding "ClosedDateTime" to ExpenseClaims and Payouts at the database level.

                // Load all (ALL) expense claims for org

                ExpenseClaims allClaims = ExpenseClaims.ForOrganization(_authenticationData.CurrentOrganization, true);
                // includes closed

                // For each claim, determine whether it was open or not at targetDateTime

                foreach (ExpenseClaim claim in allClaims)
                {
                    // if it wasn't opened until after target datetime, discard

                    if (claim.CreatedDateTime > targetDateTime)
                    {
                        continue;
                    }

                    // At this point, we are iterating over full set of expense claims opened before targetDateTime. We want the
                    // set of claims that were still open - as determined by the ledger account Expense Claims - on targetDateTime.
                    //
                    // For the expense claims that are still open, this is trivially true.
                    //
                    // However, for others, we need to look at the corresponding Payout and its FinancialTransaction to determine
                    // whetherthe transaction's date is on the other side of targetDateTime.

                    bool includeThisClaim = false;
                    DateTime dateTimeClosed = DateTime.MinValue;

                    if (claim.Open)
                    {
                        includeThisClaim = true;
                    }
                    else
                    {
                        // claim is closed. This is where we need to look first at payout then at financial transaction.

                        Payout payout = claim.Payout;

                        if (payout == null)
                        {
                            // TODO: Find outbound invoice item that depends on this expense claim
                            continue;
                            // some legacy from when Swarmops was primitive - earliest claims don't have payouts
                        }

                        if (payout.Open)
                        {
                            // Transaction is not yet closed. Include this claim, set closed to expected-closed.
                            includeThisClaim = true;
                            dateTimeClosed = payout.ExpectedTransactionDate;
                        }
                        else
                        {
                            FinancialTransaction transaction = payout.FinancialTransaction;

                            if (transaction == null)
                            {
                                throw new InvalidOperationException(
                                    "This should not happen (transaction not found on closed payout)");
                            }

                            if (transaction.DateTime > targetDateTime)
                            {
                                // yes, the claim was opened before targetDateTime and closed after it. This should be included.
                                includeThisClaim = true;
                                dateTimeClosed = transaction.DateTime;
                            }
                        }

                        // TODO: An expense claim can also be closed through an outbound invoice, in case there was a larger cash
                        // advance that wasn't fully covered, and where the two are added together. Check for this condition as well.
                        //
                        // OutboundInvoiceItem should have a dependency on the expense claim if this is the case.
                    }

                    if (includeThisClaim)
                    {
                        outstandingAccounts.Add(OutstandingAccount.FromExpenseClaim(claim, dateTimeClosed));
                    }
                }
            }

            return outstandingAccounts;
        }


        private OutstandingAccounts GetOutstandingCashAdvances(bool renderPresentTime, DateTime targetDateTime)
        {
            OutstandingAccounts outstandingAccounts = new OutstandingAccounts();

            // This is a very expensive op. We need to load ALL the cash advances, and determine the opening date from its associated
            // payout. Then, we need to determine when it was paid pack through another associated payout (or invoice payment) which 
            // I don't know how to find at the time of writing this comment, and if the target date is in between those two, then the
            // cash advance was outstanding on the target date (or is outstanding now)

            // A possible optimization could be to load the payouts into a hash table initially instead of looking them up
            // with two dbroundtrips per expense claim. It should be more efficient to have three dbroundtrips to load expenses,
            // payouts, and the relevant transactions, then stuff it all into hash tables keyed by identity and process it
            // in-memory.

            // A future optimization involves adding "ClosedDateTime" to some tables.

            // Load all (ALL) cash advances for org

            CashAdvances allCashAdvances = CashAdvances.ForOrganization(_authenticationData.CurrentOrganization, true);
            // includes closed

            // For each advance, determine whether it was open or not at targetDateTime

            foreach (CashAdvance cashAdvance in allCashAdvances)
            {
                // if it wasn't opened until after target datetime, discard (optimization)

                if (cashAdvance.CreatedDateTime > targetDateTime)
                {
                    continue;
                }

                // At this point, we are iterating over full set of cash advances opened before targetDateTime, but not necessarily
                // paid out before targetDateTime. We want the set of advances that had been paid out, and had not been paid back,
                // as determined by the ledger account Cash Advances - on targetDateTime.

                bool includeThisAdvance = false;
                DateTime dateTimePaidBack = DateTime.MinValue;
                DateTime dateTimePaidOut = DateTime.MaxValue;


                if (!cashAdvance.PaidOut)
                {
                    // This cash advance hasn't entered the ledger yet

                    continue;
                }


                Payout payoutOut = cashAdvance.PayoutOut;
                Payout payoutBack = cashAdvance.PayoutBack;

                if (payoutOut == null)
                {
                    continue;
                    // This cash advance has not been paid out yet
                }

                try
                {
                    dateTimePaidOut = payoutOut.FinancialTransaction.DateTime;
                }
                catch (ArgumentException)
                {
                    // It's possible the payout exists, but hasn't found its transaction yet. If so, this will throw
                    // an ArgumentException here. In either case, it's not in the ledger, so don't include it.

                    continue;
                }

                if (dateTimePaidOut > targetDateTime)
                {
                    // This cash advance falls outside the scope of our window, so ignore
                    continue;
                }

                if (payoutBack != null)
                {
                    try
                    {
                        dateTimePaidBack = payoutBack.FinancialTransaction.DateTime;
                    }
                    catch (ArgumentException)
                    {
                        // as above
                        continue;
                    }

                    if (dateTimePaidBack > targetDateTime)
                    {
                        includeThisAdvance = true;
                    }
                }
                else
                {
                    // As there is no payback, this advance is paid out and still open.

                    includeThisAdvance = true;

                    // TODO: If there is no payout where the cash advance is deducted, it may be
                    // invoiced to close it.

                    // TODO: Find OutboundInvoiceItem that depends on this CashAdvance. Look at the invoice date. That's our PaidBack datetime.
                }


                if (includeThisAdvance)
                {
                    outstandingAccounts.Add(OutstandingAccount.FromCashAdvance(cashAdvance, dateTimePaidOut));
                }
            }

            return outstandingAccounts;
        }


        private string FormatJson(OutstandingAccounts outstandingAccounts, Int64 balanceExpectedCents)
        {
            StringBuilder result = new StringBuilder(16384);

            result.Append("{\"rows\":[");

            Int64 centsTotal = 0;

            foreach (OutstandingAccount account in outstandingAccounts)
            {
                result.Append("{");
                result.AppendFormat(
                    "\"id\":\"{0}\"," +
                    "\"created\":\"{1}\"," +
                    "\"expected\":\"{2}\"," +
                    "\"recipient\":\"{3}\"," +
                    "\"description\":\"{4}\"," +
                    "\"amount\":\"{5:N2}\"",
                    account.Identity,
                    account.CreatedDateTime.ToShortDateString(),
                    (account.ExpectedClosed.Year <= 1950 ? Resources.Global.Global_NA : account.ExpectedClosed.ToShortDateString()),
                    JsonSanitize(account.Recipient),
                    JsonSanitize(account.Description),
                    account.AmountCents / 100.0);
                result.Append("},");
                centsTotal += account.AmountCents;
            }
            
            if (outstandingAccounts.Count > 0)
            {
                result.Remove(result.Length - 1, 1); // remove last comma
            }

            result.Append("],\"footer\":[");

            result.Append("{");

            result.AppendFormat("\"description\":\"{0}\",\"amount\":\"{2:N2}\"",
                                Resources.Pages.Ledgers.ViewOutstandingAccounts_FooterTotal, balanceExpectedCents, centsTotal/100.0);

            result.Append("},{");

            result.AppendFormat("\"description\":\"{0}\",\"amount\":\"{1:N2}\"",
                                Resources.Pages.Ledgers.ViewOutstandingAccounts_FooterLedgerBalance, balanceExpectedCents / 100.0);  // Expenses is a debt account, so reverse sign

            result.Append("},{");

            result.AppendFormat("\"description\":\"{0}\",\"amount\":\"{1:N2}\"",
                                Resources.Pages.Ledgers.ViewOutstandingAccounts_FooterDifference, (centsTotal - balanceExpectedCents) / 100.0);


            result.Append("}]}"); // on separate line to suppress warning

            return result.ToString();
        }

        private AuthenticationData _authenticationData;

        private enum OutstandingAccountType
        {
            Unknown = 0,
            ExpenseClaims,
            CashAdvances,
            InboundInvoices,
            OutboundInvoices
        }


        protected class OutstandingAccount
        {
            public Int64 AmountCents { get; set; }
            public string Description { get; set; }
            public int Identity { get; set; }
            public string Recipient { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public DateTime ExpectedClosed { get; set; }

            public static OutstandingAccount FromExpenseClaim (ExpenseClaim claim, DateTime dateTimeExpectedClosed)
            {
                OutstandingAccount result = new OutstandingAccount
                {
                    AmountCents = claim.AmountCents,
                    Description = claim.Description,
                    Identity = claim.Identity,
                    Recipient = claim.ClaimerCanonical,
                    CreatedDateTime = claim.CreatedDateTime,
                    ExpectedClosed = dateTimeExpectedClosed
                };

                return result;
            }

            public static OutstandingAccount FromCashAdvance (CashAdvance advance, DateTime dateTimePaidOut)
            {
                OutstandingAccount result = new OutstandingAccount
                {
                    AmountCents = advance.AmountCents,
                    Description = advance.Description,
                    Identity = advance.Identity,
                    Recipient = advance.Person.Canonical,
                    CreatedDateTime = dateTimePaidOut,
                    ExpectedClosed = dateTimePaidOut.AddDays(90)
                };

                return result;
            }
        }

        protected class OutstandingAccounts: List<OutstandingAccount>
        {
            // empty class declaration - just want the name
        }
    }
}