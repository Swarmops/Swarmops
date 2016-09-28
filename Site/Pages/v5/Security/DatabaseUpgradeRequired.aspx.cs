using System;
using Resources.Pages;

public partial class Security_DatabaseUpgradeRequired : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        PageTitle = Security.DatabaseUpgradeRequired_PageTitle;
        PageIcon = "iconshock-disconnect";

        this.LabelDbUpgradeRequiredHeader.Text = Security.DatabaseUpgradeRequired_Header;
        InfoBoxLiteral = Security.DatabaseUpgradeRequired_Info;
        this.LiteralDbUpgradeRequiredRant.Text = Security.DatabaseUpgradeRequired_Rant;
    }
}