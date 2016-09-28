using System;
using Swarmops.Logic.Security;

public partial class Pages_v5_Finance_BitcoinHotwalletNotEnabled : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        PageTitle = "Hotwallet Not Enabled";
        PageIcon = "iconshock-coins-cross";
        InfoBoxLiteral = "Please contact an organization administrator for assistance.";

        this.PageAccessRequired = new Access (AccessAspect.Null);
    }
}