using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

public partial class Pages_v5_Ledgers_BalanceSheet : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.CurrentOrganization.IsEconomyEnabled)
        {
            Response.Redirect("/Pages/v5/Finance/EconomyNotEnabled.aspx", true);
            return;
        }

        this.PageIcon = "iconshock-treasure";
        this.PageTitle = Resources.Pages.Ledgers.BalanceSheet_PageTitle;
        this.InfoBoxLiteral = Resources.Pages.Ledgers.BalanceSheet_Info;

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
        this.LabelContentHeader.Text = string.Format(Resources.Pages.Ledgers.BalanceSheet_ContentHeader, CurrentOrganization.Name);
        this.LiteralHeaderQ1.Text = Resources.Pages.Ledgers.BalanceSheet_Q1;
        this.LiteralHeaderQ2.Text = Resources.Pages.Ledgers.BalanceSheet_Q2;
        this.LiteralHeaderQ3.Text = Resources.Pages.Ledgers.BalanceSheet_Q3;
        this.LiteralHeaderQ4.Text = Resources.Pages.Ledgers.BalanceSheet_Q4;
        this.LiteralHeaderYtd.Text = Resources.Pages.Ledgers.BalanceSheet_Current;
        this.LiteralHeaderAccountName.Text = Resources.Pages.Ledgers.BalanceSheet_AccountName;
    }

}