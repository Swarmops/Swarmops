using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using PirateWeb.Interface.Localization;
using PirateWeb.Logic.Security;
using PirateWeb.Basic.Types;
using PirateWeb.Logic.Pirates;
using System.Threading;

public partial class Security_Login : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		this.LabelInvalidLogin.Text = String.Empty;
		this.TextLogin.Focus();

		this.LanguageSelector.LanguageChanged += new EventHandler(LanguageSelector_LanguageChanged);

		Localize();

        if (IsMobile (Request.UserAgent ))
        {

            wrapper.Style.Clear();
            wrapper.Attributes["class"] = "";

            Header.Visible = false;
            LanguageSelector.Visible = false;
            HtmlControl body = (HtmlControl)Page.FindControl("Body");
            body.Attributes["class"] = "";

        }
	}
    public static bool IsMobile (string userAgent)
    {
        userAgent = userAgent.ToLower();

        return userAgent.Contains("iphone") ||
             userAgent.Contains("ppc") ||
             userAgent.Contains("windows ce") ||
             userAgent.Contains("blackberry") ||
             userAgent.Contains("opera mini") ||
             userAgent.Contains("mobile") ||
             userAgent.Contains("palm") ||
             userAgent.Contains("symbian") ||
             userAgent.Contains("portable");
    }

	void LanguageSelector_LanguageChanged(object sender, EventArgs e)
	{
		Localize();
	}

	protected void Localize()
	{
		this.LabelWelcomeHeader.Text = LocalizationManager.GetLocalString("Interface.Security.Login.WelcomeHeader", "Welcome to PirateWeb!");
		this.LabelWelcomeParagraph.Text = LocalizationManager.GetLocalString("Interface.Security.Login.WelcomeParagraph", "PirateWeb is a member management system for pirate parties around the world. To login, please write your name, member number, or email address, along with your password.");

		this.LabelLogin.Text = LocalizationManager.GetLocalString("Interface.Security.Login.LoginPrompt", "Login:");
		this.LabelPassword.Text = LocalizationManager.GetLocalString("Interface.Security.Login.PasswordPrompt", "Password:");
		this.ButtonLogin.Text = LocalizationManager.GetLocalString("Interface.Security.Login.LoginButton", "Login");
        this.LinkLosenord.Text = LocalizationManager.GetLocalString("Interface.Security.Login.NewPasswordLink", this.LinkLosenord.Text);
    }

	protected void ButtonLogin_Click(object sender, EventArgs e)
	{
		string loginToken = this.TextLogin.Text;
		string password = this.TextPassword.Text;

        if (!string.IsNullOrEmpty(loginToken.Trim()) && !string.IsNullOrEmpty(password.Trim()))
        {
            try
            {
                BasicPerson authenticatedPerson = PirateWeb.Logic.Security.Authentication.Authenticate(loginToken, password);
                Person p = Person.FromIdentity(authenticatedPerson.PersonId);
                
                if (p.PreferredCulture != Thread.CurrentThread.CurrentCulture.Name)
                    p.PreferredCulture = Thread.CurrentThread.CurrentCulture.Name;

                FormsAuthentication.RedirectFromLoginPage(authenticatedPerson.PersonId.ToString(), false);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.ToString());
                this.LabelInvalidLogin.Text = LocalizationManager.GetLocalString("Interface.Security.Login.LoginFailed", "Login failed, try again.");
            }
        }
        else
        {
            this.LabelInvalidLogin.Text = LocalizationManager.GetLocalString("Interface.Security.Login.MissingUsernameOrPassword", "Both username and password must be filled in, try again.");
        }
	}
}
