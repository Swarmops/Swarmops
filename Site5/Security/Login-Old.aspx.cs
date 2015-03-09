using System;
using System.Diagnostics;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;
using Swarmops.Interface.Support;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Pages.Security
{
    public partial class LoginOld : Page
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            // Check if this is the first run ever. If so, redirect to Init.

            if (!SwarmDb.Configuration.IsConfigured())
            {
                Response.Redirect ("/Pages/v5/Init/", true);
                return;
            }

            // If this is the Dev Sandbox, autologin

            if (Request.Url.Host == "dev.swarmops.com" &&
                PilotInstallationIds.IsPilot (PilotInstallationIds.DevelopmentSandbox))
            {
                DashboardMessage.Set ("<p>You have been logged on as <strong>Sandbox Administrator</strong> to the Swarmops Development Sandbox.</p><br/><p>This machine runs the latest development build, so you may run into diagnostic code and half-finished features. All data here is bogus test data and is reset every night.</p><br/><p><strong>In other words, welcome, and play away!</strong></p><br/><br/>");
                FormsAuthentication.RedirectFromLoginPage ("1,1", true);
            }


            // THE DAMN BITID NEEDS TO GO INTO ANDROID WALLET SO WE CAN STREAMLINE AND REWRITE THIS POS


            this.LabelLoginFailed.Visible = false;
            this.TextLogin.Focus();


            // Check for SSL and force it

            // Special case for CloudFlare deployments - there is a case where somebody will get their connections de-SSLed at the server

            string cloudFlareVisitorScheme = Request.Headers["CF-Visitor"];
            bool cloudFlareSsl = false;

            if (!string.IsNullOrEmpty (cloudFlareVisitorScheme))
            {
                if (cloudFlareVisitorScheme.Contains ("\"scheme\":\"https\""))
                {
                    cloudFlareSsl = true;
                }
            }

            // TODO: Same thing for Pound deployments

            // Rewrite if applicable

            if (Request.Url.ToString().StartsWith ("http://") && !cloudFlareSsl)
                // only check client-side as many server sites de-SSL the connection before reaching the web server
            {
                if (!Request.Url.ToString().StartsWith ("http://dev.swarmops.com/") &&
                    !Request.Url.ToString().StartsWith ("http://localhost:") &&
                    !Request.Url.ToString().StartsWith ("http://swarmops-"))
                {
                    Response.Redirect (Request.Url.ToString().Replace ("http:", "https:"));
                }
            }
        }

        protected void ButtonLogin_Click (object sender, EventArgs e)
        {
            string loginToken = this.TextLogin.Text;
            string password = this.TextPassword.Text;

            if (!string.IsNullOrEmpty (loginToken.Trim()) && !string.IsNullOrEmpty (password.Trim()))
            {
                try
                {
                    BasicPerson authenticatedPerson = Authentication.Authenticate (loginToken,
                        password);
                    Person p = Person.FromIdentity (authenticatedPerson.PersonId);

                    if (p.PreferredCulture != Thread.CurrentThread.CurrentCulture.Name)
                        p.PreferredCulture = Thread.CurrentThread.CurrentCulture.Name;

                    // TODO: Determine logged-on organization; possibly ask for clarification

                    FormsAuthentication.RedirectFromLoginPage (authenticatedPerson.PersonId + ",1", true);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine (exception.ToString());
                    this.LabelLoginFailed.Text = exception.ToString();
                    this.LabelLoginFailed.Visible = true;
                    this.TextLogin.Focus();
                }
            }
        }
    }
}