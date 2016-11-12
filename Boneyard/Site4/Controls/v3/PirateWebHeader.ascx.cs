using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Interface.Localization;
using Activizr.Logic.Pirates;

public partial class Controls_PirateWebHeader : System.Web.UI.UserControl
{
    protected void Page_Load (object sender, EventArgs e)
    {
		this.labelLoggedInAs.Text = LocalizationManager.GetLocalString("Interface.Header.LoggedInAs", "You are logged in as");
		this.linkLogout.Text = LocalizationManager.GetLocalString("Interface.Header.Logout", "Logout");
		string userToken = HttpContext.Current.User.Identity.Name;

		int personId = Int32.Parse(userToken);
		Person person = Person.FromIdentity (personId);

		this.labelCurrentUserName.Text = person.Name + " (#" + person.PersonId.ToString() + ")";

    }

    // Testing some more
	// SqlMembershipProvider
}
