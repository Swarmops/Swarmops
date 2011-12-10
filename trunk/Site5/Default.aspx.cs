using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Structure;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LabelOrgOne.Text = Organization.FromIdentity(1).Name;
        LabelTranslation2.Text = Resources.Pages.Global.CurrentUserInfo_SelectLanguage;
    }
}