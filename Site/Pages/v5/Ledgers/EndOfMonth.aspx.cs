using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Types.Common;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Pages.Ledgers
{
    public partial class EndOfMonth : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.BookkeepingDetails);

            this.PageTitle =
                this.Title =
                    String.Format(Resources.Pages.Ledgers.EndOfMonth_Title, DateTime.UtcNow.AddMonths(-1));

            this.InfoBoxLiteral = Resources.Pages.Ledgers.EndOfMonth_Info;

            // Check which reports are required

            ItemGroups = new List<EomItemGroup>();

            // Group I: External data & accounts

            EomItemGroup group1 = new EomItemGroup();
            group1.Header = Resources.Pages.Ledgers.EndOfMonth_Header_ExternalData;

            // TODO: Iterate over all Balance account and check for automation;
            // if so, add it to an upload sequence


            EomItemGroup group2 = new EomItemGroup();
            group2.Header = Resources.Pages.Ledgers.EndOfMonth_Header_PayrollTaxes;
            group2.Id = "PayrollTaxes";

            ReportRequirement vatRequired = VatReport.IsRequired(this.CurrentOrganization);

            if (vatRequired == ReportRequirement.Required || vatRequired == ReportRequirement.Completed)
            {
                EomItem vatReport = new EomItem();
                vatReport.Id = "VatReport";
                vatReport.Callback = "CreateVatReport";
                vatReport.Name = String.Format(Resources.Pages.Ledgers.EndOfMonth_CreateVatReport, VatReport.NextReportDescription (this.CurrentOrganization));
                vatReport.Completed = (vatRequired == ReportRequirement.Completed ? true : false);
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

                        // Group header

                        builder.Append(@"

                            $('#TableEomItems').datagrid('appendRow', {
                                itemGroupName: '<span class=""itemGroupHeader""" + previousGroupIdData + @">" + group.Header.Replace(" ", "&nbsp;").Replace("'", "''") + @"</span>',
                                action: ""<img src='/Images/Icons/iconshock-green-tick-128x96px.png' data-group='" + group.Id + "'" + previousGroupIdData + @" class='group-status-icon status-completed' style='display:none' />"",
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
                            builder.Append(@"            
                                $('#TableEomItems').datagrid('appendRow', " + '{' + @"
                                    itemName: ""<span class='action-list-item' data-item='" + item.Id + @"' data-group='" + group.Id + @"'>" + item.Name + @"</span>"",
                                    action: ""<img src='/Images/Icons/transparency-16px.png' data-item='" + item.Id + @"' data-group='" + group.Id + @"' class='action action-icon eomitem-" + item.Icon + @"' data-callback='" + item.Callback + @"' style='display:inline' /><img src='/Images/Abstract/ajaxloader-48x36px.gif' data-group='" + group.Id + @"' class='status-icon status-icon-pleasewait' data-item='" + item.Id + @"' style='display:none' /><img src='/Images/Icons/iconshock-green-tick-128x96px.png' data-group='" + group.Id + @"' class='status-icon status-icon-completed' data-item='" + item.Id + @"' style='display:none' />""
                                });
                            ");
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