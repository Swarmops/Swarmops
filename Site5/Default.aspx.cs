using System;
using Resources;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Swarm;

public partial class Default : PageV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        PageTitle = Global.Dashboard_PageTitle;
        PageIcon = "iconshock-steering-wheel";
        this.PageAccessRequired = new Access(AccessAspect.Null, AccessType.Read); // dummy security until there's something to show on Dashboard

        /*
        this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;
        this.LabelActionVote.Text = Resources.Pages.Governance.Vote_PageTitle;*/

        InfoBoxLiteral =
            "This is a Dashboard placeholder. It will contain a snapshot of the state of things as soon as the basic functions are re-implemented in the new interface.";

        // If Open Ledgers, redirect to Balance Sheet: Don't show Dashboard

        if (CurrentUser.Identity == Person.OpenLedgersIdentity)
        {
            Response.Redirect ("/Ledgers/BalanceSheet");
        }
    }
}