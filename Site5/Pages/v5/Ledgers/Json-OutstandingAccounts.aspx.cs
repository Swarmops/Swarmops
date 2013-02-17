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
            this.PageAccessRequired = new Access(AccessAspect.Financials, AccessType.Read);
            _authenticationData = GetAuthenticationDataAndCulture();

            OutstandingAccountType accountType = OutstandingAccountType.Expenses;
            DateTime targetDateTime = DateTime.Today.AddDays(1);
            bool renderPresentTime = true;
            FinancialAccount balanceAccount = null;
            Int64 balanceAmountCents;

            if (Request.QueryString ["Year"] != null)
            {
                targetDateTime = _authenticationData.CurrentOrganization.GetEndOfFiscalYear (Int32.Parse((string) Request.QueryString["Year"], CultureInfo.InvariantCulture));
                renderPresentTime = false;
            }

            // Assume claims
            balanceAccount = _authenticationData.CurrentOrganization.FinancialAccounts.DebtsExpenseClaims;

            ExpenseClaims claims = null;
            Payouts payouts = null;
            if (renderPresentTime)
            {
                claims = ExpenseClaims.ForOrganization(_authenticationData.CurrentOrganization);
                payouts = Payouts.ForOrganization(_authenticationData.CurrentOrganization);
                balanceAmountCents = balanceAccount.GetDeltaCents(DateTime.MinValue, DateTime.MaxValue); // get ALL transactions
            }
            else
            {
                // TODO
                claims = new ExpenseClaims();
                payouts = new Payouts();
                balanceAmountCents = balanceAccount.GetDeltaCents(DateTime.MinValue, targetDateTime);
            }


            OutstandingAccounts outstandingAccounts = new OutstandingAccounts();
            foreach (ExpenseClaim claim in claims)
            {
                outstandingAccounts.Add((OutstandingAccount) claim);
            }
            foreach (Payout payout in payouts)
            {
                foreach (ExpenseClaim claim in payout.DependentExpenseClaims)
                {
                    outstandingAccounts.Add((OutstandingAccount)claim);
                }
            }


            Response.ContentType = "application/json";
            Response.Output.WriteLine (FormatJson(outstandingAccounts, balanceAmountCents));
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
                                "Total", balanceExpectedCents, centsTotal/100.0);

            result.Append("},{");

            result.AppendFormat("\"description\":\"{0}\",\"amount\":\"{1:N2}\"",
                                "Expected", balanceExpectedCents / 100.0);

            result.Append("},{");

            result.AppendFormat("\"description\":\"{0}\",\"amount\":\"{1:N2}\"",
                                "Difference", (centsTotal - balanceExpectedCents) / 100.0);


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

            public static explicit operator OutstandingAccount(ExpenseClaim claim)
            {
                OutstandingAccount result = new OutstandingAccount
                {
                    AmountCents = claim.AmountCents,
                    Description = claim.Description,
                    Identity = claim.Identity,
                    Recipient = claim.ClaimerCanonical,
                    CreatedDateTime = claim.CreatedDateTime,
                    ExpectedClosed = DateTime.MinValue
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