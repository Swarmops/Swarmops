using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Logic.Pirates;

namespace Activizr.Pages.Security
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if this is the first run ever. If so, redirect to Init.

            if (!PirateDb.Configuration.IsConfigured())
            {
                Response.Redirect("/Pages/v5/Init/", true);
                return;
            }

            using (StreamWriter temp = new StreamWriter("/tmp/actdebug2.txt", true))
            {
                temp.WriteLine(DateTime.Now.ToShortTimeString() + "In Page_Load of Login.aspx.cs - username is " + this.TextLogin.Text + ", pwd is " +
                               this.TextPassword.Text);
            }


            // Unlock Telerik

            this.Application["Telerik.Web.UI.Key"] = "Activizr";

            this.LabelLoginFailed.Visible = false;
            this.TextLogin.Focus();

            // Check for SSL

            if (!Request.IsSecureConnection)
            {
                if (!Request.Url.ToString().StartsWith("http://dev.activizr.com/") && !Request.Url.ToString().StartsWith("http://localhost:") && Request.Url.ToString().StartsWith("http://activizr-") && Request.Url.ToString().StartsWith("http://live.activizr.com"))
                {
                    using (StreamWriter temp = new StreamWriter("/tmp/actdebug2a.txt", true))
                    {
                        temp.WriteLine(DateTime.Now.ToShortTimeString() + "Login Page_Load - redirecting to SSL");
                    }

                    Response.Redirect(Request.Url.ToString().Replace("http:", "https:"));
                }
            }

            using (StreamWriter temp = new StreamWriter("/tmp/actdebug2a.txt", true))
            {
                temp.WriteLine(DateTime.Now.ToShortTimeString() + "Exiting page_load");
            }


        }

        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            string loginToken = this.TextLogin.Text;
            string password = this.TextPassword.Text;

            using (StreamWriter temp = new StreamWriter("/tmp/actdebug2a.txt", true))
            {
                temp.WriteLine(DateTime.Now.ToShortTimeString() + "In button_login_click - login is " + this.TextLogin.Text);
            }



            if (!string.IsNullOrEmpty(loginToken.Trim()) && !string.IsNullOrEmpty(password.Trim()))
            {
                try
                {
                    BasicPerson authenticatedPerson = Activizr.Logic.Security.Authentication.Authenticate(loginToken, password);
                    Person p = Person.FromIdentity(authenticatedPerson.PersonId);

                    if (p.PreferredCulture != Thread.CurrentThread.CurrentCulture.Name)
                        p.PreferredCulture = Thread.CurrentThread.CurrentCulture.Name;

                    // TODO: Determine logged-on organization; possibly ask for clarification

                    FormsAuthentication.RedirectFromLoginPage(authenticatedPerson.PersonId.ToString() + ",1", true);

                    using (StreamWriter temp = new StreamWriter("/tmp/actdebug2a.txt", true))
                    {
                        temp.WriteLine(DateTime.Now.ToShortTimeString() + "Button_login_click - RedirectFromLoginPage succeeded");
                    }
                }
                catch (Exception exception)
                {
                    StreamWriter temp = new StreamWriter("/tmp/actdebug2.txt");
                    temp.WriteLine(exception.ToString());
                    temp.Close();

                    System.Diagnostics.Debug.WriteLine(exception.ToString());
                    this.LabelLoginFailed.Text = exception.ToString();
                    this.LabelLoginFailed.Visible = true;
                    this.TextLogin.Focus();

                }
            }
        }
    }
}
