using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class ViewVatReports : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            PageTitle = Resources.Pages.Ledgers.ViewVatReports_PageTitle;
            InfoBoxLiteral = Resources.Pages.Ledgers.ViewVatReports_Info;

            // Security: If the org has open ledgers, then anyone may read. Otherwise, Financials.Read.

            if (!String.IsNullOrEmpty(CurrentOrganization.OpenLedgersDomain))
            {
                PageAccessRequired = new Access(AccessAspect.Null, AccessType.Read);
            }
            else
            {
                PageAccessRequired = new Access(CurrentOrganization, AccessAspect.Financials, AccessType.Read);
            }


            if (!Page.IsPostBack)
            {
                // Populate VAT report dropdown

                // TODO: Disable if viewing specific report and not open ledgers

                VatReports reports = VatReports.ForOrganization(CurrentOrganization, true);
                reports.Sort(VatReports.VatReportSorterByDate);

                foreach (VatReport report in reports)
                {
                    this.DropReports.Items.Add(new ListItem(report.Description,
                        report.Identity.ToString(CultureInfo.InvariantCulture)));
                }

                Localize();
            }

            RegisterControl(EasyUIControl.DataGrid | EasyUIControl.Tree);
        }

        private void Localize()
        {
            this.LabelContentHeader.Text = Resources.Pages.Ledgers.ViewVatReports_Title_View;

            this.LiteralHeaderTransactionId.Text = Resources.Global.Financial_TransactionIdShort;
            this.LiteralHeaderDateTime.Text = Resources.Pages.Ledgers.ViewVatReports_Header_DateTime;
            this.LiteralHeaderDescription.Text = Resources.Pages.Ledgers.ViewVatReports_Header_Description;
            this.LiteralHeaderTurnover.Text = Resources.Pages.Ledgers.ViewVatReports_Header_Turnover;
            this.LiteralHeaderVatInbound.Text = Resources.Pages.Ledgers.ViewVatReports_Header_Inbound;
            this.LiteralHeaderVatOutbound.Text = Resources.Pages.Ledgers.ViewVatReports_Header_Outbound;
            // Localize all controls - todo
        }
    }
}