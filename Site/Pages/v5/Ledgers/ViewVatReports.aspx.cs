using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

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

            VatReportDocuments = new List<RepeatedDocument>();

            // Security: If the org has open ledgers, or key was given, then anyone may read. Otherwise, Financials.Read.

            int specificReportId = 0;
            VatReport initialReport = null;
            VatReport specificReport = null;
            string reportKey = Request.QueryString["ReportKey"];
            if (!string.IsNullOrEmpty(reportKey))
            {
                specificReport = VatReport.FromGuid(reportKey);
                specificReportId = specificReport.Identity;
                this.VatReportKey = reportKey;
            }

            if (CurrentOrganization.HasOpenLedgers || specificReportId > 0)
            {
                PageAccessRequired = new Access(AccessAspect.Null, AccessType.Read);
            }
            else
            {
                PageAccessRequired = new Access(CurrentOrganization, AccessAspect.Financials, AccessType.Read);
            }


            if (!Page.IsPostBack)
            {
                Localize();

                // Populate VAT report dropdown

                if (specificReportId > 0 && !CurrentOrganization.HasOpenLedgers)
                {
                    // Show one single report

                    this.LabelContentHeader.Text = specificReport.Description;
                    this.DropReports.Visible = false;
                    this.InitialReportId = specificReport.Identity;

                    AddDocuments(specificReport);
                }
                else
                {
                    // Populate dropdown and documents list

                    VatReports reports = VatReports.ForOrganization(CurrentOrganization, true);
                    reports.Sort(VatReports.VatReportSorterByDate);

                    foreach (VatReport report in reports)
                    {
                        this.DropReports.Items.Add(new ListItem(report.Description,
                            report.Identity.ToString(CultureInfo.InvariantCulture)));

                        AddDocuments(report);
                    }

                    initialReport = reports.Last();
                    this.InitialReportId = initialReport.Identity;
                    this.DropReports.SelectedValue = this.InitialReportId.ToString(CultureInfo.InvariantCulture);
                }
            }

            RegisterControl(EasyUIControl.DataGrid | EasyUIControl.Tree);

            this.RepeaterLightboxItems.DataSource = this.VatReportDocuments;
            this.RepeaterLightboxItems.DataBind();
        }

        private void AddDocuments(VatReport report)
        {
            VatReportItems items = report.Items;

            foreach (VatReportItem item in items)
            {
                FinancialTransaction tx = item.Transaction;

                Documents documents = Documents.ForObject(tx.Dependency);
                foreach (Document doc in documents)
                {
                    VatReportDocuments.Add(new RepeatedDocument
                    {
                        BaseId = item.FinancialTransactionId.ToString(CultureInfo.InvariantCulture),
                        DocId = doc.Identity,
                        Title = tx.Description + " - " + doc.Description
                    });
                }
            }
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
            this.LiteralHeaderDox.Text = Resources.Global.Global_Dox;
            // Localize all controls - todo
        }

        public int InitialReportId { get; private set; }

        public string VatReportKey { get; private set; }

        public List<RepeatedDocument> VatReportDocuments { get; private set; } 

        public class RepeatedDocument
        {
            public int DocId { get; set; }
            public string BaseId { get; set; }
            public string Title { get; set; }
        }
    }
}