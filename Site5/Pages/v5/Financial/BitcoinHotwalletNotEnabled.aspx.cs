using System;

public partial class Pages_v5_Finance_BitcoinHotwallet : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        PageTitle = "Hotwallet Not Enabled";
        PageIcon = "iconshock-coins-cross";
        InfoBoxLiteral = "Please contact an organization administrator for assistance.";
    }
}