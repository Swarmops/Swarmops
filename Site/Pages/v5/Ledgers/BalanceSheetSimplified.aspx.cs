using System;
using System.Globalization;
using Swarmops.Logic.Security;
using Swarmops.Localization;

namespace Swarmops.Frontend.Pages.Ledgers
{
    /// <summary>
    /// This is a simplified display for economists.
    /// </summary>
    public partial class BalanceSheetSimplified : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Financial/EconomyNotEnabled", true);
                return;
            }

            PageIcon = "iconshock-treasure";
            PageTitle = LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_PageTitle");
            InfoBoxLiteral = LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_Info_Simplified");

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
            this.LabelContentHeader.Text = string.Format (LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_ContentHeader"),
                CurrentOrganization.Name);

            this.LiteralAssetsDelta.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_AssetsDelta");
            this.LiteralLiabilitiesDelta.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_LiabilitiesDelta");
            this.LiteralAssets.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_Assets");
            this.LiteralLiabilities.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_Liabilities");

            this.LiteralHeaderAccountName.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_AccountName");
        }

        // Localized strings for direct access from ASPX

        // ReSharper disable InconsistentNaming
        public string Localized_Assets
        {
            get { return JavascriptEscape(LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_Assets")); }
        }

        public string Localized_Liabilities
        {
            get { return JavascriptEscape(LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_Liabilities")); }
        }

        public string Localized_DownloadFileName
        {
            get { return JavascriptEscape(LocalizedStrings.Get(LocDomain.PagesLedgers, "BalanceSheet_DownloadFileName")); }
        }

    }
}