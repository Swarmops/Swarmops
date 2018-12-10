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

                    FinancialAccountDocument lastBankStatement = assetAccount.GetMostRecentDocument(FinancialAccountDocumentType.BankStatement);
                    int lastStatementMonth = (this.CurrentOrganization.FirstFiscalYear-1) *100 + 12;  // December of the year before

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
                                           assetAccount.Identity.ToString(CultureInfo.InvariantCulture) + monthIterator;
                        bankStatement.Name = string.Format(Resources.Pages.Ledgers.EndOfMonth_UploadBankStatementFor,
                            assetAccount.Name, "PDF", new DateTime(monthIterator / 100, monthIterator % 100, 15).ToString("MMMM yyyy"));
                        bankStatement.Completed = false; // TODO
                        bankStatement.Skippable = skippable;

                        group1.Items.Add(bankStatement);
                    }

                }
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
                                itemGroupName: '<span class=""itemGroupHeader""" + previousGroupIdData + @">" + group.Header.Replace(" ", "&nbsp;").Replace("'", "''") + @"</span>',
                                action: ""<img src='/Images/Icons/iconshock-green-tick-128x96px.png' data-group='" + group.Id + "'" + previousGroupIdData + @" class='group-status-icon status-completed' style='display:" + (groupReady? "inline" : "none") + @"' />"",
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
                                if (item.DependsOn.Length > 0)
                                {
                                    // add as disabled
                                    itemDisabledClass = " action-list-item-disabled";
                                }

                                if (item.Skippable)
                                {
                                    itemName += " (<span class='action-skip'><a href='javascript:skipItem(\\'" + item.Id + "\\'); return false;'>" +
                                                Server.HtmlEncode(Resources.Global.Global_SkipThis) + "</a></span>)";
                                }

                                builder.Append(@"            
                                $('#TableEomItems').datagrid('appendRow', {
                                    itemName: ""<span class='action-list-item" + itemDisabledClass + @"' data-item='" + item.Id + @"' data-dependson='" + item.DependsOn + @"' data-group='" + group.Id + @"'>" + itemName + @"</span>"",
                                    action: ""<img src='/Images/Icons/transparency-16px.png' data-item='" + item.Id + @"' data-group='" + group.Id + @"' class='action action-icon eomitem-" + item.Icon + @"' data-callback='" + item.Callback + @"' data-dependson='" + item.DependsOn + @"' style='display:inline' /><img src='/Images/Abstract/ajaxloader-48x36px.gif' data-group='" + group.Id + @"' class='status-icon status-icon-pleasewait' data-item='" + item.Id + @"' style='display:none' /><img src='/Images/Icons/iconshock-green-tick-128x96px.png' data-group='" + group.Id + @"' class='status-icon status-icon-completed' data-item='" + item.Id + @"' style='display:none' />""
                                    });
                                ");
                            }
                            else
                            {
                                builder.Append(@"            
                                $('#TableEomItems').datagrid('appendRow', {
                                    itemName: ""<span class='action-list-item action-list-item-completed' data-item='" + item.Id + @"' data-group='" + group.Id + @"'>" + item.Name + @"</span>"",
                                    action: ""<img src='/Images/Icons/iconshock-green-tick-128x96px.png' data-group='" + group.Id + @"' class='status-icon status-icon-completed' data-item='" + item.Id + @"' style='display:inline' />""
                                    });
                                ");
                            }
                        }

                        previousGroupId = group.Id;  // only set if group has items
                    }
                }

                return builder.ToString();
                
            }
        }

        [WebMethod]
        public static AjaxCallResult CreateVatReport()
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (!authData.Authority.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.BookkeepingDetails)))
            {
                throw new UnauthorizedAccessException();
            }

            VatReport.CreateNext(authData.CurrentOrganization);

            return new AjaxCallResult {Success = true};
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
    }
}