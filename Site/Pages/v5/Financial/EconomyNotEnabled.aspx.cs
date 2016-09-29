using System;
using Swarmops.Frontend;

public partial class Pages_v5_Finance_EconomyNotEnabled : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        PageTitle = "Economy Not Enabled";
        PageIcon = "iconshock-coins-cross";
        InfoBoxLiteral = "Please contact an organization administrator for assistance.";
    }
}