using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;

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

        this.LabelDashboardTemporaryContent.Text = Resources.Global.Dashboard_Main_Temporary;
        this.InfoBoxLiteral = Resources.Global.Dashboard_Info_Temporary;
        this.LabelGoThere2.Text = Resources.Global.Global_GoThere;
    }
}
