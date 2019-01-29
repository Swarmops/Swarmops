using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Types.Common;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.Ledgers
{
    public partial class EndOfMonth : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.BookkeepingDetails);

            this.PageTitle =
                this.Title =
                    this.LabelHeader.Text =
                        String.Format(Resources.Pages.Ledgers.EndOfMonth_Title, DateTime.UtcNow.AddMonths(-1));

            this.InfoBoxLiteral = Resources.Pages.Ledgers.EndOfMonth_Info;

            // Check which reports are required

            ItemGroups = new List<EomItemGroup>();

            // Group I: External data & accounts

            EomItemGroup group1 = new EomItemGroup();
            group1.Header = Resources.Pages.Ledgers.EndOfMonth_Header_ExternalData;
            group1.Id = "ExternalData";

            string lastUploadItemId = string.Empty;

            // Iterate over all Balance accounts and check for automation;
            // if so, add it to an upload sequence

            FinancialAccounts assetAccounts = FinancialAccounts.ForOrganization(this.CurrentOrganization,
                FinancialAccountType.Asset);

            DateTime lastMonthEndDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddDays(-1);
            int lastMonth = lastMonthEndDate.Year*100 + lastMonthEndDate.Month;
            bool skippable = true;

            foreach (FinancialAccount assetAccount in assetAccounts)
            {
                if (assetAccount.AutomationProfileId != 0)
                {
                    // This account has automation
                    // If automation has Bank Account Statement enabled (assume true for now):

                    FinancialAccountDocument lastBankStatement =
                        assetAccount.GetMostRecentDocument(FinancialAccountDocumentType.BankStatement);
                    int lastStatementMonth = (this.CurrentOrganization.FirstFiscalYear - 1)*100 + 12;
                        // December of the year before

                    if (lastBankStatement != null)
                    {
                        lastStatementMonth = lastBankStatement.ConcernsPeriodStart.Year*100 +
                                             lastBankStatement.ConcernsPeriodStart.Month;
                        skippable = false;
                    }

                    string lastId = string.Empty;
                    int monthIterator = lastStatementMonth;

                    while (monthIterator < lastMonth)
                    {
                        monthIterator++;
                        if (monthIterator%100 == 13)
                        {
                            monthIterator += 88;
                        }

                        EomItem bankStatement = new EomItem();
                        bankStatement.DependsOn = lastId; // empty for first record
                        bankStatement.Id = lastId = "BankStatement-" +
                                                    assetAccount.Identity.ToString(CultureInfo.InvariantCulture) + '-' + 
                                                    monthIterator;
                        bankStatement.Name = string.Format(Resources.Pages.Ledgers.EndOfMonth_UploadBankStatementFor,
                            assetAccount.Name, "PDF",
                            new DateTime(monthIterator/100, monthIterator%100, 15).ToString("MMMM yyyy"));
                        bankStatement.Completed = false; // TODO
                        bankStatement.Icon = "upload";
                        bankStatement.Skippable = skippable;

                        group1.Items.Add(bankStatement);
                    }

                    // Add data upload item

                    // TODO: Check if the last data upload was into the current month

                    EomItem dataUploadItem = new EomItem();
                    dataUploadItem.Id = "BankTransactionData-" + assetAccount.Identity.ToString(CultureInfo.InvariantCulture);
                    dataUploadItem.Icon = "upload";
                    dataUploadItem.Completed = false; // todo
                    dataUploadItem.Name = String.Format(Resources.Pages.Ledgers.EndOfMonth_UploadTransactionDataFor,
                        assetAccount.Name, "CSV");
                    dataUploadItem.Skippable = false;

                    group1.Items.Add(dataUploadItem);
                }
            }

            // If there are any items in Group 1, OR if an account matching is necessary, then
            // add that as an action item

            if (group1.Items.Count() > 0 || FinancialTransactions.GetUnbalanced(this.CurrentOrganization).Count() > 0)
            {
                EomItem matchAccounts = new EomItem();
                matchAccounts.DependsOn = lastUploadItemId; // may be empty if there's nothing to upload and that's ok
                matchAccounts.Id = "MatchAccounts";
                matchAccounts.Completed = false; // we already know there's at least one unbalanced
                matchAccounts.Icon = "wrench";
                matchAccounts.Skippable = false;
                matchAccounts.Name = Resources.Pages.Ledgers.EndOfMonth_MatchAccounts;

                group1.Items.Add(matchAccounts);
            }

            if (group1.Items.Count > 0)
            {
                ItemGroups.Add(group1);
            }

            EomItemGroup group2 = new EomItemGroup();
            group2.Header = Resources.Pages.Ledgers.EndOfMonth_Header_PayrollTaxes;
            group2.Id = "PayrollTaxes";

            ReportRequirement vatRequired = VatReport.IsRequired(this.CurrentOrganization);

            if (vatRequired == ReportRequirement.Required || vatRequired == ReportRequirement.Completed)
            {
                EomItem vatReport = new EomItem();
                vatReport.Id = "VatReport";
                vatReport.Callback = "CreateVatReport";
                vatReport.Completed = (vatRequired == ReportRequirement.Completed ? true : false);
                vatReport.Name = String.Format(Resources.Pages.Ledgers.EndOfMonth_CreateVatReport,
                    (vatReport.Completed
                        ? VatReport.LastReportDescription(this.CurrentOrganization)
                        : VatReport.NextReportDescription(this.CurrentOrganization)));
                vatReport.Icon = "document";

                group2.Items.Add(vatReport);
            }

            Payroll payroll = Payroll.ForOrganization(this.CurrentOrganization);
            if (payroll.Any())
            {
                // There is active payroll

                // TODO: Taxes for last month and processing for this month
            }
            else
            {
                EomItem payrollInactive = new EomItem();
                payrollInactive.Id = "PayrollInactive";
                payrollInactive.Completed = true;
                payrollInactive.Name = Resources.Pages.Ledgers.EndOfMonth_PayrollInactive;
                payrollInactive.Icon = "document";
                group2.Items.Add(payrollInactive);
            }


            if (group2.Items.Count > 0)
            {
                ItemGroups.Add(group2);
            }
        }


        public string JavascriptDocReady
        {
            get
            {
                StringBuilder builder = new StringBuilder(16384);
                string previousGroupId = string.Empty;

                foreach (EomItemGroup group in ItemGroups)
                {
                    if (group.Items.Count > 0)
                    {
                        string previousGroupIdData = string.Empty;
                        if (!string.IsNullOrEmpty(previousGroupId))
                        {
                            previousGroupIdData = " data-previous-group='" + previousGroupId + "'";
                        }

                        bool groupReady = true;
                        foreach (EomItem item in group.Items)
                        {
                            if (!item.Completed)
                            {
                                groupReady = false;
                            }
                        }

                        // Group header

                        builder.Append(@"

                            $('#TableEomItems').datagrid('appendRow', {
                                itemGroupName: ""<span class='itemGroupHeader'" + previousGroupIdData + @">" +
                                       group.Header.Replace(" ", "&nbsp;") + @"</span>"",
                                action: ""<img src='/Images/Icons/iconshock-green-tick-128x96px.png' data-group='" +
                                       group.Id + "'" + previousGroupIdData +
                                       @" class='group-status-icon status-completed' style='display:" +
                                       (groupReady ? "inline" : "none") + @"' />"",
                                itemId: '" + group.Id + @"'
                            });

                            rowCount = $('#TableEomItems').datagrid('getRows').length;

                            $('#TableEomItems').datagrid('mergeCells', {
                                index: rowCount - 1,
                                colspan: 2,
                                type: 'body',
                                field: 'itemGroupName'
                            });

                        ");

                        foreach (EomItem item in group.Items)
                        {
                            if (!item.Completed)
                            {
                                string itemName = Server.HtmlEncode(item.Name);
                                string itemDisabledClass = string.Empty;
                                string iconDisabledClass = string.Empty;
                                string iconIsUploadClass = string.Empty;

                                if (!string.IsNullOrEmpty(item.DependsOn))
                                {
                                    // add as disabled
                                    itemDisabledClass = " action-list-item-disabled";
                                    iconDisabledClass = " action-icon-disabled";
                                }

                                if (item.Skippable)
                                {
                                    itemName += "<span class='action-skip'> (<a>" +
                                                Server.HtmlEncode(Resources.Global.Global_SkipThis) + "</a>)</span>";
                                }

                                if (item.Icon == "upload")
                                {
                                    iconIsUploadClass = " is-upload";
                                }

                                builder.Append(@"            
                                $('#TableEomItems').datagrid('appendRow', {
                                    itemName: ""<span class='action-list-item" + itemDisabledClass + @"' data-item='" +
                                               item.Id + @"' data-dependson='" + item.DependsOn + @"' data-group='" +
                                               group.Id + @"'>" + itemName + @"</span>"",
                                    action: ""<img src='/Images/Icons/transparency-16px.png' data-item='" + item.Id +
                                               @"' data-group='" + group.Id + @"' class='action action-icon eomitem-" +
                                               item.Icon + iconIsUploadClass + iconDisabledClass + @"' data-callback='" +
                                               item.Callback + @"' data-dependson='" + item.DependsOn +
                                               @"' /><img src='/Images/Abstract/ajaxloader-48x36px.gif' data-group='" +
                                               group.Id + @"' class='status-icon status-icon-pleasewait' data-item='" +
                                               item.Id +
                                               @"' style='display:none' /><img src='/Images/Icons/iconshock-green-tick-128x96px.png' data-group='" +
                                               group.Id + @"' class='status-icon status-icon-completed' data-item='" +
                                               item.Id + @"' style='display:none' />""
                                    });
                                ");
                            }
                            else
                            {
                                builder.Append(@"            
                                $('#TableEomItems').datagrid('appendRow', {
                                    itemName: ""<span class='action-list-item action-list-item-completed' data-item='" +
                                               item.Id + @"' data-group='" + group.Id + @"'>" + item.Name + @"</span>"",
                                    action: ""<img src='/Images/Icons/iconshock-green-tick-128x96px.png' data-group='" +
                                               group.Id + @"' class='status-icon status-icon-completed' data-item='" +
                                               item.Id + @"' style='display:inline' />""
                                    });
                                ");
                            }
                        }

                        previousGroupId = group.Id; // only set if group has items
                    }
                }

                return builder.ToString();

            }
        }

        [WebMethod]
        public static AjaxCallResult CreateVatReport(string itemId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            if (!authData.Authority.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.BookkeepingDetails)))
            {
                throw new UnauthorizedAccessException();
            }

            VatReport.CreateNext(authData.CurrentOrganization);

            return new AjaxCallResult {Success = true};
        }


        [WebMethod]
        public static AjaxCallResult UploadBankStatement(string guid, string itemId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            if (!authData.Authority.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.BookkeepingDetails)))
            {
                throw new UnauthorizedAccessException();
            }

            string[] parts = itemId.Split('-');
            FinancialAccount account = FinancialAccount.FromIdentity(Int32.Parse(parts[1]));
            int yearMonth = Int32.Parse(parts[2]);
            DateTime statementStart = new DateTime(yearMonth/100, yearMonth%100, 1);

            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException();
            }

            Documents documents = Documents.RecentFromDescription(guid);

            FinancialAccountDocument accountDoc = FinancialAccountDocument.Create(account, FinancialAccountDocumentType.BankStatement, authData.CurrentUser,
                statementStart, statementStart.AddMonths(1), string.Empty);

            documents.SetForeignObjectForAll(accountDoc);

            return new AjaxCallResult {Success = true};
        }

        [WebMethod]
        public static AjaxCallResult UploadTransactionData(string guid, string itemId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            if (!authData.Authority.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.BookkeepingDetails)))
            {
                throw new UnauthorizedAccessException();
            }

            string[] parts = itemId.Split('-');
            FinancialAccount account = FinancialAccount.FromIdentity(Int32.Parse(parts[1]));

            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException();
            }

            Documents documents = Documents.RecentFromDescription(guid);

            // Continue here

            throw new NotImplementedException();
        }




        private class EomItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Icon { get; set; }
            public string Callback { get; set; }
            public bool Completed { get; set; }
            public string DependsOn { get; set; }
            public bool Skippable { get; set; }
        }

        private class EomItemGroup
        {
            public EomItemGroup()
            {
                Items = new List<EomItem>();
            }

            public string Id { get; set; }
            public string Header { get; set; }
            public List<EomItem> Items { get; }
        }

        private List<EomItemGroup> ItemGroups { get; set; }


        public string Localized_SkipPrompt_BankStatement
        {
            get { return CommonV5.JavascriptEscape(Resources.Pages.Ledgers.EndOfMonth_SkipBankStatementDialog); }
        }

        public string Localized_SkipPrompt_Generic
        {
            get { return CommonV5.JavascriptEscape(Resources.Pages.Ledgers.EndOfMonth_SkipDocumentDialogGeneric); }
        }

        public string Localized_SkipYes
        {
            get { return CommonV5.JavascriptEscape(Resources.Global.Global_SkipYes); }
        }

        public string Localized_SkipNo
        {
            get { return CommonV5.JavascriptEscape(Resources.Global.Global_SkipNo); }
        }
    }
}