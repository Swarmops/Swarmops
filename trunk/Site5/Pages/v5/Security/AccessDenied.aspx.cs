using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Security_AccessDenied : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageTitle = "Access Denied [LOC]";
        this.PageIcon = "iconshock-disconnect";
    }
}