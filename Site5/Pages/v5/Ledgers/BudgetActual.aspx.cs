using System;
using System.Globalization;
using Swarmops.Logic.Security;

public partial class Pages_v5_Ledgers_BudgetActual : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        if (!CurrentOrganization.IsEconomyEnabled)
        {
            Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
            return;
        }

        PageIcon = "iconshock-abacus";
        PageTitle = Resources.Pages.Ledgers.BudgetActual_PageTitle;
        InfoBoxLiteral = Resources.Pages.Ledgers.BudgetActual_Info;

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
        int year = DateTime.UtcNow.Year;

        this.LabelContentHeader.Text = string.Format (Resources.Pages.Ledgers.BudgetActual_ContentHeader,
            CurrentOrganization.Name);
        this.LiteralHeaderLastYearActual.Text = Global.Financial_Actuals + @" " + (year - 1).ToString(CultureInfo.InvariantCulture);
        this.LiteralHeaderBudget.Text = Global.Financial_Budget + @" " + year.ToString(CultureInfo.InvariantCulture); ;
        this.LiteralHeaderActual.Text = Global.Financial_Actuals + @" " + Global.Financial_YTD;
        this.LiteralHeaderExpected.Text = Resources.Pages.Ledgers.BudgetActual_ExpectedYtd;
        this.LiteralActual.Text = Global.Financial_Actuals;
        this.LiteralBudget.Text = Global.Financial_Budget;
        this.LiteralHeaderAccountName.Text = Resources.Pages.Ledgers.ProfitLossStatement_AccountName;
        this.LiteralHeaderFlags.Text = Global.Global_Flags;
    }
}