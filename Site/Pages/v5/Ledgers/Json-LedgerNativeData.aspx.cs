using System;
using System.Globalization;
using System.Text;
using Swarmops.Common;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class Json_LedgerNativeData : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            string yearString = Request.QueryString["Year"];
            string monthString = Request.QueryString["Month"];

            string emptyResponse = "[{\"id\":\"-\",\"description\":\"" +
                                   JsonSanitize(Resources.Pages.Ledgers.InspectLedgers_PleaseSelectAccount) + "\"}]";

            if (string.IsNullOrEmpty(yearString) || string.IsNullOrEmpty(monthString))
            {
                Response.Output.WriteLine(emptyResponse);
                Response.End();
            }

            int year = Int32.Parse(yearString);
            int month = Int32.Parse(monthString);

            FinancialAccount account = CurrentOrganization.FinancialAccounts.AssetsBitcoinHot;
            if (account == null || account.OrganizationId != CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException("All the nopes in the world");
            }

            if (!CurrentAuthority.HasAccess(new Access(CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read)) && CurrentOrganization.HasOpenLedgers)
            {
                throw new UnauthorizedAccessException("Access denied because security tokens say so");
            }

            DateTime periodStart, periodEnd;
            DateTime balanceStart = Constants.DateTimeLow;
            bool zeroStart = false;
            bool zeroEnd = false;
            bool displayDescription = CurrentAuthority.HasAccess(new Access(CurrentOrganization, AccessAspect.BookkeepingDetails, AccessType.Read));

            // HACK HACK HACK
            if (CurrentOrganization.Identity == 8 && CurrentOrganization.Name.StartsWith("Rick Falkvinge"))
            {
                // TODO
                displayDescription = true; // FIX THIS WITH A DAMN SETTING, YOU MORON, DON'T HARDCODE IT
            }

            bool canSeeAudit = CurrentAuthority.HasAccess(new Access(CurrentOrganization, AccessAspect.Auditing, AccessType.Read));

            if (month > 0 && month <= 12)
            {
                periodStart = new DateTime(year, month, 1);
                periodEnd = periodStart.AddMonths(1);
                if (account.AccountType == FinancialAccountType.Income ||
                    account.AccountType == FinancialAccountType.Cost)
                {
                    balanceStart = new DateTime(year, 1, 1);

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
                periodStart = new DateTime(year, (month - 21) * 3 + 1, 1);
                periodEnd = periodStart.AddMonths(3);
                if (account.AccountType == FinancialAccountType.Income ||
                    account.AccountType == FinancialAccountType.Cost)
                {
                    balanceStart = new DateTime(year, 1, 1);

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
                periodStart = new DateTime(year, 1, 1);
                periodEnd = new DateTime(year + 1, 1, 1);
                if (account.AccountType == FinancialAccountType.Income ||
                    account.AccountType == FinancialAccountType.Cost)
                {
                    zeroStart = true;
                    zeroEnd = true;
                }
            }
            else
            {
                throw new ArgumentException("Invalid month supplied: " + month.ToString(CultureInfo.InvariantCulture));
            }

            FinancialAccountRows rows = account.GetRows(periodStart, periodEnd);

            StringBuilder result = new StringBuilder(16384);

            Int64 runningBalance = 0L;
            Int64 runningBitcoinBalance = 0;
            string startString = Resources.Pages.Ledgers.InspectLedgers_InboundBalanceZero;
            string endString = Resources.Pages.Ledgers.InspectLedgers_OutboundBalanceZero;

            if (!zeroStart)
            {
                runningBitcoinBalance = account.GetForeignCurrencyBalanceDeltaCents(balanceStart, periodStart).Cents;
                runningBalance = account.GetDeltaCents(balanceStart, periodStart);
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

            result.Append("{" +
                           String.Format("\"description\":\"{0}\",\"balanceBitcoin\":\"{1:N2}\"", JsonSanitize(startString),
                               runningBitcoinBalance / 100.0) + "},");

            foreach (FinancialAccountRow row in rows)
            {
                string description = row.Description;

                if (!displayDescription)
                {
                    description = Resources.Pages.Ledgers.InspectLedgers_TxDetail_DescriptionWithheld;
                }

                Int64 deltaSatoshis = row.TransactionRow.AmountForeignCents.Cents;
                runningBitcoinBalance += deltaSatoshis;

                string hasDoxString =
                    "<img src='/Images/Icons/iconshock-search-256px.png' onmouseover=\"this.src='/Images/Icons/iconshock-search-hot-256px.png';\" onmouseout=\"this.src='/Images/Icons/iconshock-search-256px.png';\" data-txid='{0}' class='LocalIconInspect' style='cursor:pointer' height='20' width='20' />";

                string actionHtml = String.Format(hasDoxString, row.FinancialTransactionId.ToString(CultureInfo.InvariantCulture));

                if (canSeeAudit)
                {
                    actionHtml +=
                        String.Format(
                            "&nbsp;<img src=\"/Images/Icons/iconshock-flag-white-16px.png\" class=\"LocalIconFlag\" txId=\"{0}\" />",
                            row.FinancialTransactionId.ToString(CultureInfo.InvariantCulture));
                }

                result.Append("{" + String.Format(
                    "\"id\":\"{0:N0}\",\"datetime\":\"{1:MMM-dd HH:mm}\",\"description\":\"{2}\"," +
                    "\"deltaPresentation\":\"{3}\",\"deltaBitcoin\":\"{4}\",\"balanceBitcoin\":\"{5:N2}\",\"action\":\"{6}\"",
                    row.Transaction.OrganizationSequenceId,
                    row.TransactionDateTime,
                    JsonSanitize(description),
                    row.AmountCents != 0? (row.AmountCents / 100.0).ToString("N2") : string.Empty,
                    deltaSatoshis != 0? (deltaSatoshis / 100.0).ToString("N2"): string.Empty,
                    runningBitcoinBalance / 100.0,
                    JsonSanitize(actionHtml)) + "},");
            }

            if (rows.Count == 0)
            {
                // If there are no transactions in this time period, say so

                result.Append("{\"description\":\"" +
                               JsonSanitize(Resources.Pages.Ledgers.InspectLedgers_NoTransactions) + "\"},");
            }

            result.Append("{" +
                           String.Format("\"description\":\"{0}\",\"balance\":\"{1:N2}\"", JsonSanitize(endString),
                               runningBalance / 100.0) + "},");

            Response.Output.WriteLine("[" + result.ToString().TrimEnd(',') + "]");
            Response.End();
        }
    }
}