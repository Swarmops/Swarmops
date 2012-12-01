using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Security_AccessDenied : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = Resources.Pages.Security.AccessDenied_PageTitle;
        this.PageIcon = "iconshock-disconnect";

        this.LabelAccessDeniedHeader.Text = Resources.Pages.Security.AccessDenied_Header;
        this.LabelSidebarInfo.Text = Resources.Global.Sidebar_Information;
        this.LabelAccessDeniedInfo.Text = Resources.Pages.Security.AccessDenied_Info;
        this.LiteralAccessDeniedRant.Text = String.Format(Resources.Pages.Security.AccessDenied_Rant, this.CurrentOrganization.Name);
    }
}