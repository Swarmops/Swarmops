using System;
using System.Globalization;
using Resources.Pages;
using Swarmops.Logic.Security;

public partial class Pages_v5_Ledgers_ProfitLossStatement : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        if (!CurrentOrganization.IsEconomyEnabled)
        {
            Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
            return;
        }

        PageIcon = "iconshock-abacus";
        PageTitle = Ledgers.ProfitLossStatement_PageTitle;
        InfoBoxLiteral = Ledgers.ProfitLossStatement_Info;

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
        this.LabelContentHeader.Text = string.Format (Ledgers.ProfitLossStatement_ContentHeader,
            CurrentOrganization.Name);
        this.LiteralHeaderLastYear.Text = Ledgers.ProfitLossStatement_LastYear;
        this.LiteralHeaderQ1.Text = Ledgers.ProfitLossStatement_Q1;
        this.LiteralHeaderQ2.Text = Ledgers.ProfitLossStatement_Q2;
        this.LiteralHeaderQ3.Text = Ledgers.ProfitLossStatement_Q3;
        this.LiteralHeaderQ4.Text = Ledgers.ProfitLossStatement_Q4;
        this.LiteralHeaderYtd.Text = Ledgers.ProfitLossStatement_Ytd;
        this.LiteralHeaderAccountName.Text = Ledgers.ProfitLossStatement_AccountName;
    }
}