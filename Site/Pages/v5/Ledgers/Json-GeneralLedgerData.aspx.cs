using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class Json_GeneralLedgerData : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            string yearString = Request.QueryString["Year"];
            string monthString = Request.QueryString["Month"];

            int year = Int32.Parse (yearString);
            int month = Int32.Parse (monthString);

            DateTime dawnOfMankind = new DateTime (1901, 1, 1);
            // no org will ever import bookkeeping from before this date

            FinancialAccount account = CurrentOrganization.FinancialAccounts.AssetsBankAccountMain.Children.First();
            if (account.OrganizationId != CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException ("All the nopes in the world");
            }

            if (!CurrentAuthority.HasAccess (new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read)))
            {
                throw new UnauthorizedAccessException ("Access denied because security tokens say so");
            }

            DateTime periodStart, periodEnd;
            bool zeroStart = false;
            bool zeroEnd = false;
            bool displayDescription = CurrentAuthority.HasAccess (new Access (CurrentOrganization, AccessAspect.BookkeepingDetails, AccessType.Read));

            // HACK HACK HACK
            if (CurrentOrganization.Identity == 8 && CurrentOrganization.Name.StartsWith("Rick Falkvinge"))
            {
                // TODO
                displayDescription = true; // FIX THIS WITH A DAMN SETTING, YOU MORON, DON'T HARDCODE IT
            }

            bool canSeeAudit = CurrentAuthority.HasAccess(new Access(CurrentOrganization, AccessAspect.Auditing, AccessType.Read));

            if (month > 0 && month <= 12)
            {
                periodStart = new DateTime (year, month, 1);
                periodEnd = periodStart.AddMonths (1);
            }
            else if (month > 20 && month < 25) // quarters 1..4 are coded as months 21..24
            {
                periodStart = new DateTime (year, (month - 21)*3 + 1, 1);
                periodEnd = periodStart.AddMonths (3);
            }
            else if (month == 31) // "whole year" is coded as month 31
            {
                periodStart = new DateTime (year, 1, 1);
                periodEnd = new DateTime (year + 1, 1, 1);
            }
            else
            {
                throw new ArgumentException ("Invalid month supplied: " + month.ToString (CultureInfo.InvariantCulture));
            }

            DateTime profitLossStart = new DateTime(periodStart.Year, 1, 1);
            DateTime balanceStart = dawnOfMankind;

            FinancialAccounts accounts = FinancialAccounts.ForOrganization(CurrentOrganization);
            FinancialAccountRows rows = accounts.GetRows (periodStart, periodEnd);

            StringBuilder result = new StringBuilder (16384);

            Dictionary<int, Int64> runningBalanceLookup = new Dictionary<int, long>();
            Dictionary<int, FinancialAccount> accountLookup = new Dictionary<int, FinancialAccount>();

            foreach (FinancialAccount accountLoop in accounts)
            {
                accountLookup[accountLoop.Identity] = accountLoop;
            }

            int currentTransactionId = 0;

            foreach (FinancialAccountRow row in rows)
            {
                string creditString = string.Empty;
                string debitString = string.Empty;

                account = accountLookup[row.FinancialAccountId];
                if (!runningBalanceLookup.ContainsKey(row.FinancialAccountId))
                {
                    if (account.AccountType == FinancialAccountType.Asset || account.AccountType == FinancialAccountType.Debt)
                    {
                        runningBalanceLookup[account.Identity] = account.GetDeltaCents(balanceStart, periodStart);
                    }
                    else
                    {
                        runningBalanceLookup[account.Identity] = account.GetDeltaCents(profitLossStart, periodStart);
                    }
                }

                string hasDoxString =
                    "<img src='/Images/Icons/iconshock-search-256px.png' onmouseover=\"this.src='/Images/Icons/iconshock-search-hot-256px.png';\" onmouseout=\"this.src='/Images/Icons/iconshock-search-256px.png';\" txId='{0}' class='LocalIconInspect' style='cursor:pointer' height='20' width='20' />";

                string actionHtml = String.Format(hasDoxString, row.FinancialTransactionId.ToString(CultureInfo.InvariantCulture));


                if (row.FinancialTransactionId != currentTransactionId)
                {
                    // We're starting a new transaction here

                    // If it's not the first one, close the previous one first
                    if (currentTransactionId != 0)
                    {
                        result.Append("]},");
                    }

                    FinancialTransaction transaction = row.Transaction;
                    string description = transaction.Description;

                    result.Append("{" + String.Format(
                        "\"id\":\"{0:N0}\",\"datetime\":\"{1:MMM-dd HH:mm}\",\"description\":\"{2}\"," +
                        "\"action\":\"{6}\",\"state\":\"open\",\"children\":[",
                        row.Transaction.OrganizationSequenceId,
                        row.TransactionDateTime,
                        JsonSanitize(description),
                        debitString,
                        creditString,
                        runningBalanceLookup[row.FinancialAccountId]/100.0,
                        JsonSanitize(actionHtml)));

                }
                else
                {
                    // still same transaction
                    result.Append(",");
                }

                if (row.AmountCents < 0)
                {
                    creditString = String.Format ("{0:N2}", row.AmountCents/100.0);
                }
                else if (row.AmountCents > 0)
                {
                    debitString = String.Format ("{0:N2}", row.AmountCents/100.0);
                }

                runningBalanceLookup[row.FinancialAccountId] += row.AmountCents;

                /*
                if (canSeeAudit)
                {
                    actionHtml +=
                        String.Format (
                            "&nbsp;<img src=\"/Images/Icons/iconshock-flag-white-16px.png\" class=\"LocalIconFlag\" txId=\"{0}\" />",
                            row.FinancialTransactionId.ToString (CultureInfo.InvariantCulture));
                }*/

                result.Append ("{" + String.Format (
                    "\"id\":\"{0:N0}\",\"datetime\":\"{1:MMM-dd HH:mm}\",\"description\":\"{2}\"," +
                    "\"deltaPos\":\"{3}\",\"deltaNeg\":\"{4}\",\"balance\":\"{5:N2}\",\"action\":\"{6}\"",
                    row.Transaction.OrganizationSequenceId,
                    row.TransactionDateTime,
                    JsonSanitize ("description"),
                    debitString,
                    creditString,
                    runningBalanceLookup[row.FinancialAccountId]/100.0,
                    JsonSanitize (actionHtml)) + "},");
            }

            if (rows.Count == 0)
            {
                // If there are no transactions in this time period, say so

                result.Append ("{\"description\":\"" +
                               JsonSanitize (Resources.Pages.Ledgers.InspectLedgers_NoTransactions) + "\"},");
            }

            Response.Output.WriteLine ("[" + result.ToString().TrimEnd (',') + "]}]");
            Response.End();
        }
    }
}