using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Swarm;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Pages.Security
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if this is the first run ever. If so, redirect to Init.

            if (!SwarmDb.Configuration.IsConfigured())
            {
                Response.Redirect("/Pages/v5/Init/", true);
                return;
            }

            // THE DAMN BITID NEEDS TO GO INTO ANDROID WALLET SO WE CAN STREAMLINE AND REWRITE THIS POS


            this.LabelLoginFailed.Visible = false;
            this.TextLogin.Focus();

            // Check for SSL

            /* DISABLE temporarily
            if ((!Request.IsSecureConnection) && Request.Url.ToString().StartsWith("http://"))  // Both are necessary, as some reverse proxies will https-ify an original nonsecure connection
            {
                if (!Request.Url.ToString().StartsWith("http://dev.swarmops.com/") && !Request.Url.ToString().StartsWith("http://localhost:") && !Request.Url.ToString().StartsWith("http://swarmops-"))
                {
                    Response.Redirect(Request.Url.ToString().Replace("http:", "https:"));
                }
            } */
        }

        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            string loginToken = this.TextLogin.Text;
            string password = this.TextPassword.Text;

            if (!string.IsNullOrEmpty(loginToken.Trim()) && !string.IsNullOrEmpty(password.Trim()))
            {
                try
                {
                    BasicPerson authenticatedPerson = Swarmops.Logic.Security.Authentication.Authenticate(loginToken, password);
                    Person p = Person.FromIdentity(authenticatedPerson.PersonId);

                    if (p.PreferredCulture != Thread.CurrentThread.CurrentCulture.Name)
                        p.PreferredCulture = Thread.CurrentThread.CurrentCulture.Name;

                    // TODO: Determine logged-on organization; possibly ask for clarification

                    FormsAuthentication.RedirectFromLoginPage(authenticatedPerson.PersonId.ToString() + ",1", true);
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.ToString());
                    this.LabelLoginFailed.Text = exception.ToString();
                    this.LabelLoginFailed.Visible = true;
                    this.TextLogin.Focus();

                }
            }
        }
    }
}
