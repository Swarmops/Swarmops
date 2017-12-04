using System;
using Swarmops.Frontend;

public partial class Pages_v5_Finance_BlockchainCodeUpgrading : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        PageTitle = "Upgrade in progress";
        PageIcon = "iconshock-coins-cross";
        InfoBoxLiteral = "An upgrade of the blockchain is in progress. Until the upgrade has verifiably completed, all financial operations are suspended.";
    }
}