using System;
using Resources;
using Swarmops.Logic.Financial;

public partial class Default : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        PageTitle = Global.Dashboard_PageTitle;
        PageIcon = "iconshock-steering-wheel";

        /*
        this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;
        this.LabelActionVote.Text = Resources.Pages.Governance.Vote_PageTitle;*/

        InfoBoxLiteral =
            "This is a Dashboard placeholder. It will contain a snapshot of the state of things as soon as the basic functions are re-implemented in the new interface.";

        ExchangeRateSnapshot.Create();
    }
}