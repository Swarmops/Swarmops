using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default : PageV5Base
{
    public Default()
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = Resources.Pages.Global.Dashboard_PageTitle;
        this.PageIcon = "iconshock-steering-wheel";

        this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;
        this.LabelActionVote.Text = Resources.Pages.Governance.Vote_PageTitle;

        this.LabelSidebarInfo.Text = Resources.Pages.Global.Sidebar_Information;
        this.LabelSidebarActions.Text = Resources.Pages.Global.Sidebar_Actions;
        this.LabelSidebarTodo.Text = Resources.Pages.Global.Sidebar_Todo;

        this.LabelActionItemsHere.Text = Resources.Pages.Global.Sidebar_Todo_Placeholder;
        this.LabelGoThere.Text = Resources.Pages.Global.Global_GoThere;
        this.LabelDashboardTemporaryContent.Text = Resources.Pages.Global.Dashboard_Main_Temporary;
        this.LabelDashboardInfo.Text = Resources.Pages.Global.Dashboard_Info_Temporary;
        this.LabelGoThere2.Text = Resources.Pages.Global.Global_GoThere;
    }
}
