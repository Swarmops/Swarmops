using System;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Pages.v5.Ledgers.TaxForms
{
    public partial class PayrollForms : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            PageIcon = "iconshock-treasure";
            PageTitle = Resources.Pages.Financial.PayrollTaxForms_PageTitle;
            InfoBoxLiteral = Resources.Pages.Financial.PayrollTaxForms_Info;

            if (!Page.IsPostBack)
            {
                // TODO: DETERMINE COUNTRIES ON PAYROLL

                this.DropCountries.Items.Add (new ListItem(Resources.GeographyNames.Country_SE, "SE"));

                Localize();
            }

            RegisterControl (EasyUIControl.DataGrid | EasyUIControl.Tree);
        }


        private void Localize()
        {
            this.LabelContentHeader.Text = Resources.Pages.Ledgers.TaxForms_Payroll_TaxFormsMonthlyHeader;
            this.LiteralAdditiveTax.Text = Resources.Pages.Ledgers.TaxForms_Payroll_AdditiveTax;
            this.LiteralCostTotal.Text = Resources.Pages.Ledgers.TaxForms_Payroll_TotalCost;
            this.LiteralDeductedTax.Text = Resources.Pages.Ledgers.TaxForms_Payroll_DeductedTax;
            this.LiteralFormsIcons.Text = Resources.Global.Global_View;
            this.LiteralGrossPay.Text = Resources.Pages.Ledgers.TaxForms_Payroll_GrossPay;
            this.LiteralTaxTotal.Text = Resources.Pages.Ledgers.TaxForms_Payroll_TotalTax;
            this.LiteralTimePeriod.Text = Resources.Global.Global_YearMonth;
        }
    }
}