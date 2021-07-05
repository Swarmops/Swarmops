using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Localization;
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

            PageTitle = LocalizedStrings.Get(LocDomain.PagesLedgers, "ViewVatReports_PageTitle");
            InfoBoxLiteral = LocalizedStrings.Get(LocDomain.PagesLedgers, "ViewVatReports_Info");

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

                    if (reports.Count > 0)
                    {

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
                    else
                    {
                        // There are no VAT reports for this organization (yet?) so display an error instead

                        this.PanelShowVatReports.Visible = false;
                        this.PanelShowNoVatReports.Visible = true;
                    }

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

                Documents documents = Documents.ForObject(tx.Dependency ?? tx); // tx.Dependency if not null, else tx

                int pageCounter = 0;
                int pagesTotal = documents.Count;

                foreach (Document doc in documents)
                {
                    pageCounter++;

                    VatReportDocuments.Add(new RepeatedDocument
                    {
                        BaseId = item.FinancialTransactionId.ToString(CultureInfo.InvariantCulture),
                        DocId = doc.Identity,
                        Title = tx.Description + " " + Document.GetLocalizedPageCounter(pageCounter, pagesTotal)
                    });
                }
            }
        }

        private void Localize()
        {
            this.LabelContentHeader.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ViewVatReports_Title_View");
            this.LabelSidebarDownload.Text = LocalizedStrings.Get(LocDomain.Global, "Global_DownloadThis");

            this.LiteralHeaderTransactionId.Text = LocalizedStrings.Get(LocDomain.Global, "Financial_TransactionIdShort");
            this.LiteralHeaderDateTime.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Date");
            this.LiteralHeaderDescription.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ViewVatReports_Header_Description");
            this.LiteralHeaderTurnover.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ViewVatReports_Header_Turnover");
            this.LiteralHeaderVatInbound.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ViewVatReports_Header_Inbound");
            this.LiteralHeaderVatOutbound.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ViewVatReports_Header_Outbound");
            this.LiteralHeaderDox.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Dox");

            this.LabelHeaderNoVatReportsToDisplay.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ViewVatReports_Header_NoReports");
            this.LabelNoVatReportsToDisplay.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ViewVatReports_NoReports");
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