using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Financial;

public partial class Pages_v5_Finance_RequestCashAdvance : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = Resources.Pages.Finance.RequestCashAdvance_PageTitle;
        this.PageIcon = "iconshock-walletmoney";
        this.InfoBoxLiteral = Resources.Pages.Finance.RequestCashAdvance_Info;

        if (!Page.IsPostBack)
        {
            // Prime bank details

            this.TextBank.Text = this.CurrentUser.BankName;
            this.TextClearing.Text = this.CurrentUser.BankClearing;
            this.TextAccount.Text = this.CurrentUser.BankAccount;
            this.TextAmount.Text = 0.ToString("N2");
            this.TextAmount.Focus();
        }
    }
}