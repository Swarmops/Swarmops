using System;
using System.Globalization;
using Swarmops.Frontend;
using Swarmops.Localization;
using Swarmops.Logic.Security;


namespace Swarmops.Frontend.Pages.Ledgers
{
    public partial class ProfitLossStatement : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Financial/EconomyNotEnabled", true);
                return;
            }

            PageIcon = "iconshock-abacus";
            PageTitle = LocalizedStrings.Get(LocDomain.PagesLedgers, "ProfitLossStatement_PageTitle");
            InfoBoxLiteral = LocalizedStrings.Get(LocDomain.PagesLedgers, "ProfitLossStatement_Info");

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
            this.LabelContentHeader.Text = string.Format (LocalizedStrings.Get(LocDomain.PagesLedgers, "ProfitLossStatement_ContentHeader"),
                CurrentOrganization.Name);
            this.LiteralHeaderLastYear.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ProfitLossStatement_LastYear");
            this.LiteralHeaderQ1.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Q1");
            this.LiteralHeaderQ2.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Q2");
            this.LiteralHeaderQ3.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Q3");
            this.LiteralHeaderQ4.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Q4");
            this.LiteralHeaderYtd.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ProfitLossStatement_Ytd");
            this.LiteralHeaderAccountName.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "ProfitLossStatement_AccountName");
        }

        public string Localized_DownloadFileName
        {
            get { return JavascriptEscape (LocalizedStrings.Get(LocDomain.PagesLedgers, "ProfitLossStatement_DownloadFileName")); }
        }
    }

}