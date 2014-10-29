using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using Swarmops.Basic.Enums;
using Swarmops.Database;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;
using Swarmops.Site.Automation;

namespace Swarmops.Pages.Security
{

    public partial class Login : DataV5Base   // "Data" because we don't have a master page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if this is the first run ever. If so, redirect to Init.

            if (!SwarmDb.Configuration.IsConfigured())
            {
                Response.Redirect("/Pages/v5/Init/", true);
                return;
            }

            // If this is the Dev Sandbox, autologin

            if (Request.Url.Host == "dev.swarmops.com" &&
                PilotInstallationIds.IsPilot(PilotInstallationIds.DevelopmentSandbox) &&
                Request.QueryString["SuppressAutologin"] != "true")
            {
                Response.AppendCookie(new HttpCookie("DashboardMessage", HttpUtility.UrlEncode("<p>You have been logged on as <strong>Sandbox Administrator</strong> to the Swarmops Development Sandbox.</p><br/><p>This machine runs the latest development build, so you may run into diagnostic code and half-finished features. All data here is bogus test data and is reset every night.</p><br/><p><strong>In other words, welcome, and play away!</strong></p><br/><br/>")));
                FormsAuthentication.RedirectFromLoginPage("1,1", true);
            }

            // Check for SSL and force it

            // Special case for CloudFlare deployments - there is a case where somebody will get their connections de-SSLed at the server

            string cloudFlareVisitorScheme = Request.Headers["CF-Visitor"];
            bool cloudFlareSsl = false;

            if (!string.IsNullOrEmpty(cloudFlareVisitorScheme))
            {
                if (cloudFlareVisitorScheme.Contains("\"scheme\":\"https\""))
                {
                    cloudFlareSsl = true;
                }
            }

            // TODO: Same thing for Pound deployments

            // Rewrite if applicable

            if (Request.Url.ToString().StartsWith("http://") && !cloudFlareSsl) // only check client-side as many server sites de-SSL the connection before reaching the web server
            {
                if (!Request.Url.ToString().StartsWith("http://dev.swarmops.com/") && !Request.Url.ToString().StartsWith("http://localhost:"))
                {
                    Response.Redirect(Request.Url.ToString().Replace("http:", "https:"));
                }
            } 



            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginTop] = "-3px";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginRight] = "3px";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.Cursor] = "pointer";

            Localize();

            // Generate BitID tokens

            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString().Replace("-", "");

            // for now, fake it

            this.ImageBitIdQr.ImageUrl =
                "http://chart.apis.google.com/chart?cht=qr&chs=400x400&chl=bitid%3A//dev.swarmops.com/Security/Login.aspx/BitIdLogin%3Fx%3D" + guidString + "%26u%3D1";
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
        }

        private void Localize()
        {
            this.LabelCurrentOrganizationName.Text = Resources.Global.Global_Organization;
            this.LabelCurrentUserName.Text = Resources.Global.Title_Person_Generic;
            this.LabelPageTitle.Text = Resources.Pages.Security.Login_PageTitle;
            this.LabelPreferences.Text = Resources.Global.Global_NA;
            this.LabelSidebarInfoHeader.Text = Resources.Global.Sidebar_Information;
            this.LabelSidebarHelpHeader.Text = Resources.Global.Sidebar_Help;
            this.LabelSidebarInfoContent.Text = Resources.Pages.Security.Login_Info;
            this.LabelSidebarManualLoginHeader.Text = Resources.Pages.Security.Login_ManualLogin;
            this.LabelHeader.Text = Resources.Pages.Security.Login_Header;
            this.LabelSidebarResetPassword.Text = Resources.Pages.Security.Login_ResetPassword;

        }


        private static string _buildIdentity;


        private string GetBuildIdentity()
        {
            // Read build number if not loaded, or set to "Private" if none

            if (_buildIdentity == null)
            {
                try
                {
                    using (
                        StreamReader reader = File.OpenText(HttpContext.Current.Request.MapPath("~/BuildIdentity.txt")))
                    {
                        _buildIdentity = "Build " + reader.ReadLine();
                    }
                }
                catch (Exception)
                {
                    _buildIdentity = "Private Build";
                }
            }

            return _buildIdentity;
        }



        protected void ButtonLogin_Click(object sender, EventArgs args)
        {
            // Check the host names and addresses again as a security measure - after all, we can be called from outside our intended script
            /*
            if (!(VerifyHostName(this.TextServerName.Text) && VerifyHostAddress(this.TextServerAddress.Text)))
            {
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    return; // Probable hack attempt - fail silently
                }
            }

            Swarmops.Logic.Swarm.Person expectedPersonOne = Swarmops.Logic.Security.Authentication.Authenticate("1",
                this.TextFirstUserPassword1.Text);

            if (expectedPersonOne != null)
            {
                FormsAuthentication.RedirectFromLoginPage("1,1", true);
                Response.Redirect("/", true);
            }*/
        }
    }
}