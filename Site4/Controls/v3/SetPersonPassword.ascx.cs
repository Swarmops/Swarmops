using System;
using System.Data;
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
using Activizr.Logic.Support;
using Activizr.Logic.Structure;

public partial class Controls_SetPersonPassword : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		// Default control to apply to the currently viewing user

		if (this.Person == null)
		{
			this.Person = Person.FromIdentity(Convert.ToInt32(HttpContext.Current.User.Identity.Name));
			this.DisplayRandomize = false;
		}
    }

	// These two fields could be converted to properties if desired

	public Person Person;
	public bool DisplayRandomize
	{
		get
		{
			return this.ButtonRandomizePassword.Visible;
		}
		set
		{
			this.ButtonRandomizePassword.Visible = value;
		}
	}

	protected void ButtonSetPassword_Click(object sender, EventArgs e)
    {
        string password1 = this.TextPassword1.Text;
        string password2 = this.TextPassword2.Text;
        string oldpassword = this.TextOldPassword.Text;

        if (!Person.ValidatePassword(oldpassword))
            RequiredFieldValidator3.IsValid = false;
        else
        {
            RequiredFieldValidator3.IsValid = true;
            if (password1 == password2 && password1.Length > 0)
            {
                Person.SetPassword(this.TextPassword1.Text);
                Page.ClientScript.RegisterStartupScript(typeof(Page), "SuccessMessage", string.Format(string.Format("alert ('{0}');", GetLocalResourceObject("JsAlertSuccess").ToString()), Person.Name), true);
            }
        }
    }

	protected void ButtonRandomizePassword_Click(object sender, EventArgs e)
	{
		string newPassword = Formatting.GeneratePassword(8);

		string mailBody = string.Format(GetLocalResourceObject("NewPasswordMailMsg").ToString(), newPassword);

        Person.SendNotice(GetLocalResourceObject("NewPasswordMailSubject").ToString(), mailBody, Organization.PPSEid);
		Person.SetPassword(newPassword);

        Page.ClientScript.RegisterStartupScript(typeof(Page), "SuccessMessage", string.Format(string.Format("alert ('{0}');", GetLocalResourceObject("JsAlertSuccessMail").ToString()), Person.Name), true);
	}
}
