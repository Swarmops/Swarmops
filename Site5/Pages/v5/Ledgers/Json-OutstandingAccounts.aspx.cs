using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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

            OutstandingAccountType accountType = OutstandingAccountType.Expenses;
            DateTime targetDateTime = DateTime.Today.AddDays(1);
            bool renderPresentTime = true;
            FinancialAccount balanceAccount = null;
            Int64 ledgerExpectedCents;

            if (Request.QueryString ["Year"] != null)
            {
                targetDateTime = _authenticationData.CurrentOrganization.GetEndOfFiscalYear (Int32.Parse((string) Request.QueryString["Year"], CultureInfo.InvariantCulture));
                renderPresentTime = false;
            }

            // Assume claims
            balanceAccount = _authenticationData.CurrentOrganization.FinancialAccounts.DebtsExpenseClaims;

            OutstandingAccounts outstandingAccounts = new OutstandingAccounts();

            if (renderPresentTime)
            {
                ExpenseClaims claims = ExpenseClaims.ForOrganization(_authenticationData.CurrentOrganization);
                Payouts payouts = Payouts.ForOrganization(_authenticationData.CurrentOrganization);
                ledgerExpectedCents = balanceAccount.GetDeltaCents(DateTime.MinValue, DateTime.MaxValue); // get ALL transactions

                foreach (ExpenseClaim claim in claims)
                {
                    outstandingAccounts.Add(OutstandingAccount.FromExpenseClaim(claim, DateTime.MinValue));
                }
                foreach (Payout payout in payouts)
                {
                    foreach (ExpenseClaim claim in payout.DependentExpenseClaims)
                    {
                        outstandingAccounts.Add(OutstandingAccount.FromExpenseClaim(claim, payout.ExpectedTransactionDate));
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

                ExpenseClaims allClaims = ExpenseClaims.ForOrganization(_authenticationData.CurrentOrganization, true); // includes closed

                // For each claim, determine whether it was open or not at targetDateTime

                foreach (ExpenseClaim claim in allClaims)
                {
                    // if it wasn't opened until after target datetime, discard

                    if (claim.CreatedDateTime > targetDateTime)
                    {
                        continue;
                    }

                    // At this point, we have the full set of expense claims opened before targetDateTime. We want the
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
                            continue; // some legacy from when Swarmops was primitive - earliest claims don't have payouts
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
                                throw new InvalidOperationException("This should not happen (transaction not found on closed payout)");
                            }

                            if (transaction.DateTime > targetDateTime)
                            {
                                // yes, the claim was opened before targetDateTime and closed after it. This should be included.
                                includeThisClaim = true;
                                dateTimeClosed = transaction.DateTime;
                            }
                        }
                    }

                    if (includeThisClaim)
                    {
                        outstandingAccounts.Add(OutstandingAccount.FromExpenseClaim(claim, dateTimeClosed));
                    }
                }



                // Finally, get the ledger balance on the targeted DateTime.

                ledgerExpectedCents = balanceAccount.GetDeltaCents(DateTime.MinValue, targetDateTime);
            }




            Response.ContentType = "application/json";
            Response.Output.WriteLine (FormatJson(outstandingAccounts, ledgerExpectedCents));
            Response.End();
        }

        private string FormatJson (OutstandingAccounts outstandingAccounts, Int64 balanceExpectedCents)
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
                    account.Recipient,
                    account.Description,
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
                                Resources.Pages.Ledgers.ViewOutstandingAccounts_FooterLedgerBalance, -balanceExpectedCents / 100.0);  // Expenses is a debt account, so reverse sign

            result.Append("},{");

            result.AppendFormat("\"description\":\"{0}\",\"amount\":\"{1:N2}\"",
                                Resources.Pages.Ledgers.ViewOutstandingAccounts_FooterDifference, (centsTotal + balanceExpectedCents) / 100.0);


            result.Append("}]}"); // on separate line to suppress warning

            return result.ToString();
        }

        private AuthenticationData _authenticationData;

        private enum OutstandingAccountType
        {
            Unknown = 0,
            Expenses,
            InboundInvoices,
            OutboundInvoices,
            CashAdvances
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
        }

        protected class OutstandingAccounts: List<OutstandingAccount>
        {
            // empty class declaration - just want the name
        }
    }
}