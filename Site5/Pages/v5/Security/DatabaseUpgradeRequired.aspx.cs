using System;

public partial class Security_DatabaseUpgradeRequired : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle = Resources.Pages.Security.DatabaseUpgradeRequired_PageTitle;
        PageIcon = "iconshock-disconnect";

        this.LabelDbUpgradeRequiredHeader.Text = Resources.Pages.Security.DatabaseUpgradeRequired_Header;
        InfoBoxLiteral = Resources.Pages.Security.DatabaseUpgradeRequired_Info;
        this.LiteralDbUpgradeRequiredRant.Text = Resources.Pages.Security.DatabaseUpgradeRequired_Rant;
    }
}