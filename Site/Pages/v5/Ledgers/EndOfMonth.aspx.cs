using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Types.Common;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.Ledgers
{
    public partial class EndOfMonth : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.BookkeepingDetails);

            this.Title = Resources.Pages.Ledgers.EndOfMonth_Title;
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

                foreach (EomItemGroup group in ItemGroups)
                {
                    if (group.Items.Count > 0)
                    {
                        // Group header

                        builder.Append(@"

                            $('#TableEomItems').datagrid('appendRow', {
                                itemGroupName: '<span class=""itemGroupHeader"">" + group.Header.Replace(" ", "&nbsp;").Replace("'", "''") + @"</span>',
                                action: ""<img src='/Images/Icons/iconshock-green-tick-128x96px.png' data-group='" + group.Id + @"' class='group-status-icon style='display:none' />""
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
                                    itemName: ""<span class='action-list-item' data-group='" + group.Id + @"'>" + item.Name + @"</span>"",
                                    action: ""<img src='/Images/Icons/transparency-16px.png' data-group='" + group.Id + @"' class='action action-icon eomitem-" + item.Icon + @"' style='display:inline' />""
                                });
                            ");
                        }
                    }
                }

                return builder.ToString();
                
            }
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