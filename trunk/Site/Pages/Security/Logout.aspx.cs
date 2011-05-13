using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Security_Logout : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		FormsAuthentication.SignOut();
		Response.Redirect("~/default.aspx");
	}
}
