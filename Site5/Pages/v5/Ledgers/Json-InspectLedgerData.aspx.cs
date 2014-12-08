using System;
using System.Globalization;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class Json_InspectLedgerData : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            string accountIdString = Request.QueryString["AccountId"];
            string yearString = Request.QueryString["Year"];
            string monthString = Request.QueryString["Month"];

            string emptyResponse = "[{\"id\":\"-\",\"description\":\"" +
                                   JsonSanitize (Resources.Pages.Ledgers.InspectLedgers_PleaseSelectAccount) + "\"}]";

            if (string.IsNullOrEmpty (accountIdString) || string.IsNullOrEmpty (yearString) ||
                string.IsNullOrEmpty (monthString) || accountIdString == "undefined")
            {
                Response.Output.WriteLine (emptyResponse);
                Response.End();
            }

            int accountId = Int32.Parse (accountIdString);
            int year = Int32.Parse (yearString);
            int month = Int32.Parse (monthString);

            DateTime dawnOfMankind = new DateTime (1901, 1, 1);
            // no org will ever import bookkeeping from before this date

            if (accountId <= 0)
            {
                Response.Output.WriteLine (emptyResponse);
                Response.End();
            }

            FinancialAccount account = FinancialAccount.FromIdentity (accountId);
            if (account.OrganizationId != CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException ("All the nopes in the world");
            }

            if (!CurrentUser.HasAccess (new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read)))
            {
                throw new UnauthorizedAccessException ("Access denied because security tokens say so");
            }

            DateTime periodStart, periodEnd;
            DateTime balanceStart = dawnOfMankind;
            bool zeroStart = false;
            bool zeroEnd = false;

            if (month > 0 && month <= 12)
            {
                periodStart = new DateTime (year, month, 1);
                periodEnd = periodStart.AddMonths (1);
                if (account.AccountType == FinancialAccountType.Income ||
                    account.AccountType == FinancialAccountType.Cost)
                {
                    balanceStart = new DateTime (year, 1, 1);

                    if (month == 1)
                    {
                        zeroStart = true;
                    }
                    if (month == 12)
                    {
                        zeroEnd = true;
                    }
                }
            }
            else if (month > 20 && month < 25) // quarters 1..4 are coded as months 21..24
            {
                periodStart = new DateTime (year, (month - 21)*3 + 1, 1);
                periodEnd = periodStart.AddMonths (3);
                if (account.AccountType == FinancialAccountType.Income ||
                    account.AccountType == FinancialAccountType.Cost)
                {
                    balanceStart = new DateTime (year, 1, 1);

                    if (month == 21)
                    {
                        zeroStart = true;
                    }
                    if (month == 24)
                    {
                        zeroEnd = true;
                    }
                }
            }
            else if (month == 31) // "whole year" is coded as month 31
            {
                periodStart = new DateTime (year, 1, 1);
                periodEnd = new DateTime (year + 1, 1, 1);
                if (account.AccountType == FinancialAccountType.Income ||
                    account.AccountType == FinancialAccountType.Cost)
                {
                    zeroStart = true;
                    zeroEnd = true;
                }
            }
            else
            {
                throw new ArgumentException ("Invalid month supplied: " + month.ToString (CultureInfo.InvariantCulture));
            }


            FinancialAccountRows rows = account.GetRows (periodStart, periodEnd);

            StringBuilder result = new StringBuilder (16384);

            Int64 runningBalance = 0L;
            string startString = Resources.Pages.Ledgers.InspectLedgers_InboundBalanceZero;
            string endString = Resources.Pages.Ledgers.InspectLedgers_OutboundBalanceZero;

            if (!zeroStart)
            {
                runningBalance = account.GetDeltaCents (dawnOfMankind, periodStart);
                startString = Resources.Pages.Ledgers.InspectLedgers_InboundBalance;
            }
            if (!zeroEnd)
            {
                endString = Resources.Pages.Ledgers.InspectLedgers_OutboundBalance;
            }
            else if (periodEnd > DateTime.Now)
            {
                // account is zeroed at end of this period, but we're not yet at end of period, so add a "to date" disclaimer
                endString = Resources.Pages.Ledgers.InspectLedgers_OutboundBalanceZeroToDate;
            }

            result.Append ("{" +
                           String.Format ("\"description\":\"{0}\",\"balance\":\"{1:N0}\"", JsonSanitize (startString),
                               runningBalance/100.0) + "},");

            foreach (FinancialAccountRow row in rows)
            {
                string creditString = string.Empty;
                string debitString = string.Empty;

                if (row.AmountCents < 0)
                {
                    creditString = String.Format ("{0:N0}", row.AmountCents/100.0);
                }
                else if (row.AmountCents > 0)
                {
                    debitString = String.Format ("{0:N0}", row.AmountCents/100.0);
                }

                runningBalance += row.AmountCents;

                string actionHtml = String.Format (
                    "<img src=\"/Images/Icons/iconshock-magnifyingglass-16px.png\" class=\"LocalIconInspect\" baseid=\"\" />&nbsp;<img src=\"/Images/Icons/iconshock-flag-white-16px.png\" class=\"LocalIconFlag\" baseid=\"{0}\" />",
                    row.FinancialTransactionId.ToString (CultureInfo.InvariantCulture));

                result.Append ("{" + String.Format (
                    "\"id\":\"{0}\",\"datetime\":\"{1:MMM-dd HH:mm}\",\"description\":\"{2}\"," +
                    "\"deltaPos\":\"{3}\",\"deltaNeg\":\"{4}\",\"balance\":\"{5:N0}\",\"action\":\"{6}\"",
                    row.FinancialTransactionId,
                    row.TransactionDateTime,
                    JsonSanitize (row.Description),
                    debitString,
                    creditString,
                    runningBalance/100.0,
                    JsonSanitize (actionHtml)) + "},");
            }

            if (rows.Count == 0)
            {
                // If there are no transactions in this time period, say so

                result.Append ("{\"description\":\"" +
                               JsonSanitize (Resources.Pages.Ledgers.InspectLedgers_NoTransactions) + "\"},");
            }

            result.Append ("{" +
                           String.Format ("\"description\":\"{0}\",\"balance\":\"{1:N0}\"", JsonSanitize (endString),
                               runningBalance/100.0) + "},");

            Response.Output.WriteLine ("[" + result.ToString().TrimEnd (',') + "]");
            Response.End();
        }
    }
}