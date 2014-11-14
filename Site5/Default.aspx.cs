using System;

public partial class Default : PageV5Base
{
    public Default()
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = Resources.Global.Dashboard_PageTitle;
        this.PageIcon = "iconshock-steering-wheel";

        /*
        this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;
        this.LabelActionVote.Text = Resources.Pages.Governance.Vote_PageTitle;*/

        this.InfoBoxLiteral = "This is a Dashboard placeholder. It will contain a snapshot of the state of things as soon as the basic functions are re-implemented in the new interface.";
    }
}
