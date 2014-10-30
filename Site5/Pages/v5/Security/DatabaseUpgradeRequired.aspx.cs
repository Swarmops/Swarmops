using System;

public partial class Security_DatabaseUpgradeRequired : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = Resources.Pages_Security.DatabaseUpgradeRequired_PageTitle;
        this.PageIcon = "iconshock-disconnect";

        this.LabelDbUpgradeRequiredHeader.Text = Resources.Pages_Security.DatabaseUpgradeRequired_Header;
        this.InfoBoxLiteral = Resources.Pages_Security.DatabaseUpgradeRequired_Info;
        this.LiteralDbUpgradeRequiredRant.Text = Resources.Pages_Security.DatabaseUpgradeRequired_Rant;
    }
}