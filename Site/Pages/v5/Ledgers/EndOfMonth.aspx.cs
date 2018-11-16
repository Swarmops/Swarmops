using System;
using System.Collections.Generic;
using System.Linq;
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
            DateTime utcMo

            // Check which reports are required

            // Group I: External data & accounts

            EomItemGroup group1 = new EomItemGroup();
            group1.Header = Resources.Pages.Ledgers.EndOfMonth_Header_ExternalData;

            // TODO: Iterate over all Balance account and check for automation;
            // if so, add it to an upload sequence

            ReportRequirement vatRequired = VatReport.IsRequired(this.CurrentOrganization);

            if (vatRequired != ReportRequirement.NotRequired) // it's either Required or Completed
            {
                EomItem vatReport = new EomItem();
                vatReport.Id = "VatReport";
                vatReport.Name = String.Format(Resources.Pages.Ledgers.EndOfMonth_CreateVatReport, VatReport.NextReportDescription (this.CurrentOrganization));
                vatReport.Completed = (vatRequired == ReportRequirement.Completed ? true : false);

                group1.Items.Add(vatReport);
            }

        }


        public string JavascriptDocReady
        {
            get
            {
                return string.Empty;
                
            }
        }

        private class EomItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public bool Completed { get; set; }
        }

        private class EomItemGroup
        {
            public EomItemGroup()
            {
                Items = new List<EomItem>();
            }

            public string Header { get; set; }
            public List<EomItem> Items { get; }
        }
    }
}