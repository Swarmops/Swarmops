using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_v5_Finance_RequestCashAdvance : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = Resources.Pages.Finance.RequestCashAdvance_PageTitle;
        this.PageIcon = "iconshock-walletmoney";
    }
}