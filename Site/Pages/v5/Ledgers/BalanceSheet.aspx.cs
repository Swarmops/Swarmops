using System;
using System.Globalization;
using Swarmops.Localization;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.Ledgers
{
    public partial class BalanceSheet : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Financial/EconomyNotEnabled", true);
                return;
            }

            PageIcon = "iconshock-treasure";
            PageTitle = Resources.Pages.Ledgers.BalanceSheet_PageTitle;
            InfoBoxLiteral = Resources.Pages.Ledgers.BalanceSheet_Info;

            // Security: If the org has open ledgers, then anyone may read. Otherwise, Financials.Read.

            if (CurrentOrganization.HasOpenLedgers)
            {
                PageAccessRequired = new Access (AccessAspect.Null, AccessType.Read);
            }
            else
            {
                PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Financials, AccessType.Read);
            }


            if (!Page.IsPostBack)
            {
                int year = DateTime.Today.Year;
                int firstFiscalYear = CurrentOrganization.FirstFiscalYear;

                while (year >= firstFiscalYear)
                {
                    this.DropYears.Items.Add (year.ToString (CultureInfo.InvariantCulture));
                    year--;
                }

                Localize();
            }

            RegisterControl (EasyUIControl.DataGrid | EasyUIControl.Tree);
        }


        private void Localize()
        {
            this.LabelContentHeader.Text = string.Format(LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_ContentHeader"),
                CurrentOrganization.Name);
            this.LabelSidebarDownload.Text = LocalizedStrings.Get(LocDomain.Global, "Global_DownloadThis");

            this.LiteralHeaderQ1.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Q1");
            this.LiteralHeaderQ2.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Q2");
            this.LiteralHeaderQ3.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Q3");
            this.LiteralHeaderQ4.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Q4");
            this.LiteralHeaderYtd.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ProfitLossStatement_Ytd");
            this.LiteralHeaderAccountName.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ProfitLossStatement_AccountName");

            this.LiteralHeaderYtd.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_Current");
            this.LiteralHeaderAccountName.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "Ledgers.BalanceSheet_AccountName");
        }

        // Localized strings for direct access from ASPX

        // ReSharper disable InconsistentNaming
        public string Localized_StartYear
        {
            get { return JavascriptEscape(LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_StartYear")); }
        }

        public string Localized_EndYear
        {
            get { return JavascriptEscape(LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_EndYear")); }
        }

        public string Localized_DownloadFileName
        {
            get { return JavascriptEscape(LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_DownloadFileName")); }
        }

    }
}