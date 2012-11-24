using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Financial;

public partial class Default : PageV5Base
{
    public Default()
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = Resources.Global.Dashboard_PageTitle;
        this.PageIcon = "iconshock-steering-wheel";

        this.LabelActionListMotions.Text = Resources.Pages.Governance.ListMotions_PageTitle;
        this.LabelActionVote.Text = Resources.Pages.Governance.Vote_PageTitle;

        this.LabelSidebarInfo.Text = Resources.Global.Sidebar_Information;
        this.LabelSidebarActions.Text = Resources.Global.Sidebar_Actions;
        this.LabelSidebarTodo.Text = Resources.Global.Sidebar_Todo;

        this.LabelActionItemsHere.Text = Resources.Global.Sidebar_Todo_Placeholder;
        this.LabelGoThere.Text = Resources.Global.Global_GoThere;
        this.LabelDashboardTemporaryContent.Text = Resources.Global.Dashboard_Main_Temporary;
        this.LabelDashboardInfo.Text = Resources.Global.Dashboard_Info_Temporary;
        this.LabelGoThere2.Text = Resources.Global.Global_GoThere;

        // THIS IS A ONE-OFF, DELETE:

        OrganizationFinancialAccounts.PrimePiratpartietSE();
    }
}
