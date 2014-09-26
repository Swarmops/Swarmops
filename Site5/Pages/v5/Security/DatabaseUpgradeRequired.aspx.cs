using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Security_DatabaseUpgradeRequired : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = Resources.Pages.Security.DatabaseUpgradeRequired_PageTitle;
        this.PageIcon = "iconshock-disconnect";

        this.LabelDbUpgradeRequiredHeader.Text = Resources.Pages.Security.DatabaseUpgradeRequired_Header;
        this.InfoBoxLiteral = Resources.Pages.Security.DatabaseUpgradeRequired_Info;
        this.LiteralDbUpgradeRequiredRant.Text = Resources.Pages.Security.DatabaseUpgradeRequired_Rant;
    }
}