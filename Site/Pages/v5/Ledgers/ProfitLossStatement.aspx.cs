using System;
using System.Globalization;
using Resources;
using Swarmops.Frontend;
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
            PageTitle = Resources.Pages.Ledgers.ProfitLossStatement_PageTitle;
            InfoBoxLiteral = Resources.Pages.Ledgers.ProfitLossStatement_Info;

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
            this.LabelContentHeader.Text = string.Format (Resources.Pages.Ledgers.ProfitLossStatement_ContentHeader,
                CurrentOrganization.Name);
            this.LiteralHeaderLastYear.Text = Resources.Pages.Ledgers.ProfitLossStatement_LastYear;
            this.LiteralHeaderQ1.Text = Global.Global_Q1;
            this.LiteralHeaderQ2.Text = Global.Global_Q2;
            this.LiteralHeaderQ3.Text = Global.Global_Q3;
            this.LiteralHeaderQ4.Text = Global.Global_Q4;
            this.LiteralHeaderYtd.Text = Resources.Pages.Ledgers.ProfitLossStatement_Ytd;
            this.LiteralHeaderAccountName.Text = Resources.Pages.Ledgers.ProfitLossStatement_AccountName;
        }

        public string Localized_DownloadFileName
        {
            get { return JavascriptEscape (Resources.Pages.Ledgers.ProfitLossStatement_DownloadFileName); }
        }
    }

}