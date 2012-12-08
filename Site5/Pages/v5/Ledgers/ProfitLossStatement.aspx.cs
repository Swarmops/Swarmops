using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_v5_Ledgers_ProfitLossStatement : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageIcon = "iconshock-abacus";
        this.PageTitle = Resources.Pages.Ledgers.ProfitLossStatement_PageTitle;
        this.InfoBoxLiteral = Resources.Pages.Ledgers.ProfitLossStatement_Info;
    }
}