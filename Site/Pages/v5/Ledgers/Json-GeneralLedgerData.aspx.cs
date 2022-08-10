﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Swarmops.Common;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Localization;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

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
            DateTime balanceStart = Constants.DateTimeLow;

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
            int rowCount = 0;

            foreach (FinancialAccountRow row in rows)
            {
                string creditString = string.Empty;
                string debitString = string.Empty;

                FinancialAccount account = accountLookup[row.FinancialAccountId];
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

                    string hasDoxString =
                        "<img src='/Images/Icons/iconshock-search-256px.png' onmouseover=\"this.src='/Images/Icons/iconshock-search-hot-256px.png';\" onmouseout=\"this.src='/Images/Icons/iconshock-search-256px.png';\" data-txid='{0}' class='LocalViewDox' style='cursor:pointer' height='20' width='20' />" +
                        "<img src='/Images/Icons/iconshock-download-240px.png' onmouseover=\"this.src='/Images/Icons/iconshock-download-hot-240px.png';\" onmouseout=\"this.src='/Images/Icons/iconshock-download-240px.png';\" data-docid='{1}' data-docname=\"{2}\" class='LocalDownloadDox' style='cursor:pointer' height='18' width='18' />";

                    string actionHtml = string.Empty;

                    Documents documents = Documents.ForObject(transaction.Dependency ?? transaction);

                    if (documents.Count > 0)
                    {
                        foreach (Document doc in documents)
                        {
                            actionHtml += String.Format("<a href='/Pages/v5/Support/StreamUpload.aspx?DocId={0}&hq=1' data-caption=\"{1}\" class='FancyBox_Gallery' data-fancybox='{2}'></a>",
                                doc.Identity, doc.ClientFileName.Replace("\"", "'"), transaction.Identity);
                        }

                        actionHtml = String.Format(hasDoxString, row.FinancialTransactionId.ToString(CultureInfo.InvariantCulture), documents[0].Identity, CurrentOrganization.Name + " - " + LocalizedStrings.Get(LocDomain.Global, "Financial_GeneralLedger") + " " + transaction.DateTime.ToShortDateString() + " - " + LocalizedStrings.Get(LocDomain.Global, "Financial_TransactionIdShort") + transaction.OrganizationSequenceId.ToString("N0"))) + "<span class='hiddenDocLinks'>" + actionHtml + "</span>";
                    }

                    result.Append("{" + String.Format(
                        "\"id\":\"{0:N0}\",\"idDisplay\":\"<span class='weight-more-emphasis'>{0:N0}</span>\",\"datetime\":\"<span class='weight-more-emphasis'>{1:MMM-dd HH:mm}</span>\",\"txDescription\":\"<span class='weight-more-emphasis tx-description'>{2}</span>\",\"action\":\"{3}\"," +
                        "\"state\":\"open\",\"children\":[",
                        row.Transaction.OrganizationSequenceId,
                        row.TransactionDateTime,
                        JsonSanitize(description),
                        JsonSanitize(actionHtml)));

                    if (transaction.Dependency != null)
                    {
                        IHasIdentity dependency = transaction.Dependency;
                        string info = string.Empty;
                        Documents docs = null;

                        if (dependency is VatReport)
                        {
                            VatReport report = (VatReport) dependency;
                            if (report.OpenTransactionId == transaction.Identity)
                            {
                                info = String.Format(LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_TxInfo_OpenVatReport"),
                                    report.DescriptionShort);
                            }
                            else if (report.CloseTransactionId == transaction.Identity)
                            {
                                info = String.Format(LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_TxInfo_CloseVatReport"),
                                    report.DescriptionShort);
                            }
                        }

                        // TODO: Continue here with adding Info row under transactions where it's helpful
                        // TODO: Remember that the Info row needs cell merging, colspan=5 or =6
                    }


                    /*
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
                        */

                    currentTransactionId = row.FinancialTransactionId;
                    rowCount = 1;

                }
                else
                {
                    // still same transaction
                    result.Append(",");
                    rowCount++;
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

                //result.Append("{\"description\":\"child\"}");

                string accountClass;
                switch (accountLookup[row.FinancialAccountId].AccountType)
                {
                    case FinancialAccountType.Asset:
                        accountClass = LocalizedStrings.Get(LocDomain.Global, "Financial_Asset");
                        break;
                    case FinancialAccountType.Debt:
                        accountClass = LocalizedStrings.Get(LocDomain.Global, "Financial_Debt");
                        break;
                    case FinancialAccountType.Income:
                        accountClass = LocalizedStrings.Get(LocDomain.Global, "Financial_Income");
                        break;
                    case FinancialAccountType.Cost:
                        accountClass = LocalizedStrings.Get(LocDomain.Global, "Financial_Cost");
                        break;
                    default:
                        throw new NotImplementedException();
                }

                string accountName = account.Name;

                if (account.ParentIdentity != 0)
                {
                    if (!accountLookup.ContainsKey(account.ParentIdentity))
                    {
                        accountLookup[account.ParentIdentity] = account.Parent;
                    }

                    accountName = accountLookup[account.ParentIdentity].Name + " » " + accountName;
                }

                result.Append ("{" + String.Format (
                    "\"id\":\"{0:N0}-{6:N0}\",\"idDisplay\":\"{0:N0}:{6:N0}\",\"datetime\":\"{1}\",\"txDescription\":\"{2}\"," +
                    "\"deltaPos\":\"{3}\",\"deltaNeg\":\"{4}\",\"balance\":\"{5:N2}\"",
                    row.Transaction.OrganizationSequenceId,
                    JsonSanitize (accountClass),
                    JsonSanitize (accountName),
                    debitString,
                    creditString,
                    runningBalanceLookup[row.FinancialAccountId]/100.0,
                    rowCount) + "}");
                
            }

            if (rows.Count == 0)
            {
                // If there are no transactions in this time period, say so

                result.Append ("{\"description\":\"" +
                               JsonSanitize (LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_NoTransactions")) + "\"},");
            }

            Response.Output.WriteLine ("[" + result.ToString().TrimEnd (',') + "]}]");
            Response.End();
        }
    }
}