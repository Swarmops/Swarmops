using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_v5_Ledgers_ProfitLossStatement : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.CurrentOrganization.IsEconomyEnabled)
        {
            Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
            return;
        }

        this.PageIcon = "iconshock-abacus";
        this.PageTitle = Resources.Pages.Ledgers.ProfitLossStatement_PageTitle;
        this.InfoBoxLiteral = Resources.Pages.Ledgers.ProfitLossStatement_Info;

        if (!Page.IsPostBack)
        {
            int year = DateTime.Today.Year;
            int firstFiscalYear = CurrentOrganization.FirstFiscalYear;

            while (year >= firstFiscalYear)
            {
                this.DropYears.Items.Add(year.ToString(CultureInfo.InvariantCulture));
                year--;
            }

            Localize();
        }
    }

    private void Localize()
    {
        this.LabelContentHeader.Text = string.Format(Resources.Pages.Ledgers.ProfitLossStatement_ContentHeader, CurrentOrganization.Name);
        this.LiteralHeaderLastYear.Text = Resources.Pages.Ledgers.ProfitLossStatement_LastYear;
        this.LiteralHeaderQ1.Text = Resources.Pages.Ledgers.ProfitLossStatement_Q1;
        this.LiteralHeaderQ2.Text = Resources.Pages.Ledgers.ProfitLossStatement_Q2;
        this.LiteralHeaderQ3.Text = Resources.Pages.Ledgers.ProfitLossStatement_Q3;
        this.LiteralHeaderQ4.Text = Resources.Pages.Ledgers.ProfitLossStatement_Q4;
        this.LiteralHeaderYtd.Text = Resources.Pages.Ledgers.ProfitLossStatement_Ytd;
        this.LiteralHeaderAccountName.Text = Resources.Pages.Ledgers.ProfitLossStatement_AccountName;

    }
}