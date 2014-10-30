using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.ExtensionMethods;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using Swarmops.Basic.Enums;
using Swarmops.Database;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;
using Swarmops.Site.Automation;
using NBitcoin;

namespace Swarmops.Pages.Security
{

    public partial class Login : DataV5Base // "Data" because we don't have a master page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Person.FromIdentity(1).BitIdAddress = "14fRQCbR62EGzjPQks9XRAVRiqWhftn3dA";

            // Check if this is the first run ever. If so, redirect to Init.

            if (!SwarmDb.Configuration.IsConfigured())
            {
                Response.Redirect("/Pages/v5/Init/", true);
                return;
            }


            // DEBUG: Log the entire raw request

            Persistence.Key["BitIdTest_Raw"] = Request.ToRaw();
            Persistence.Key["BitIdTest_AddressNight"] = Request.Params["address"];


            // Check for POST data - for BitId via Webform

            Persistence.Key["BitIdTest_HttpMethod"] = Request.HttpMethod;

            if (Request.HttpMethod == "POST")
            {
                // We should ONLY get here if we're getting a BitId by Webform submission.

                if (Request.Params["address"] != null)
                {
                    // looks like it

                    BitIdCredentials credentials = new BitIdCredentials
                    {
                        address = Request.Params["address"],
                        uri = Request.Params["uri"],
                        signature = Request.Params["signature"]
                    };

                    ProcessRespondBitId(credentials, Response);
                    Persistence.Key["BitIdTest_SuccessForm"] = DateTime.Now.ToString();
                    Response.End();
                    return;
                }
                else if (Request.ContentType == "application/json")
                {
                    BitIdCredentials credentials =
                        new JavaScriptSerializer().Deserialize<BitIdCredentials>(
                            new StreamReader(Request.InputStream).ReadToEnd());
                        // TODO: untested but seems to work. Throws?

                    ProcessRespondBitId(credentials, Response);
                    return;
                }
            }



            // If this is the Dev Sandbox, autologin

            if (Request.Url.Host == "dev.swarmops.com" &&
                PilotInstallationIds.IsPilot(PilotInstallationIds.DevelopmentSandbox) &&
                Request.QueryString["SuppressAutologin"] != "true")
            {
                Response.AppendCookie(new HttpCookie("DashboardMessage",
                    HttpUtility.UrlEncode(
                        "<p>You have been logged on as <strong>Sandbox Administrator</strong> to the Swarmops Development Sandbox.</p><br/><p>This machine runs the latest development build, so you may run into diagnostic code and half-finished features. All data here is bogus test data and is reset every night.</p><br/><p><strong>In other words, welcome, and play away!</strong></p><br/><br/>")));
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

            if (Request.Url.ToString().StartsWith("http://") && !cloudFlareSsl)
                // only check client-side as many server sites de-SSL the connection before reaching the web server
            {
                if (!Request.Url.ToString().StartsWith("http://dev.swarmops.com/") &&
                    !Request.Url.ToString().StartsWith("http://localhost:"))
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

            string nonce = guidString + DateTime.UtcNow.Ticks.ToString("x8");

            string hostName = Request.Url.Host;

            string bitIdUri = "bitid://" + hostName + "/Security/Login.aspx?x=" + nonce;

            if (Request.Url.ToString().StartsWith("http://") && !cloudFlareSsl)
            {
                bitIdUri += "&u=1";
            }

            this.LiteralUri.Text = HttpUtility.UrlEncode(bitIdUri);
            this.LiteralNonce.Text = nonce;

            GuidCache.Set(bitIdUri + "-Logon", "Unauth");

            // TODO: need to NOT FUCKING USE GOOGLE CHARTS for this but bring home a QR package

            this.ImageBitIdQr.ImageUrl =
                "http://chart.apis.google.com/chart?cht=qr&chs=400x400&chl=" + HttpUtility.UrlEncode(bitIdUri);
        }


        protected void ProcessRespondBitId(BitIdCredentials credentials, HttpResponse response)
        {
            Persistence.Key["BitId_Processing"] = DateTime.Now.ToString();

            BitcoinAddress testAddress = new BitcoinAddress(credentials.address);
            if (testAddress.VerifyMessage(credentials.uri, credentials.signature))
            {
                // woooooo

                Persistence.Key["BitId_KeyVerified"] = DateTime.Now.ToString();

                try
                {
                    if (this.CurrentUser != null)
                    {
                        if (GuidCache.Get(credentials.uri + "-Intent") as string == "Register")
                        {
                            // set currentUser bitid
                            // Then go do something else, I guess
                        }
                    }

                    if (GuidCache.Get(credentials.uri + "-Logon") as string == "Unauth")
                    {
                        Person person = Person.FromBitIdAddress(credentials.address);

                        // TODO: Determine last logged-on organization. Right now, log on to Sandbox.

                        GuidCache.Set(credentials.uri + "-LoggedOn",
                            person.Identity.ToString(CultureInfo.InvariantCulture) + ",1,,BitId 2FA");
                    }

                    response.StatusCode = 200;
                    response.ContentType = "application/json";
                    response.Write(
                        new JavaScriptSerializer().Serialize(new BitIdResponseSuccess()
                        {
                            address = credentials.address,
                            signature = credentials.signature
                        }));
                    response.End();
                }
                catch (Exception e)
                {
                    Persistence.Key["BitIdLogin_Debug_Exception"] = e.ToString();
                }
            }
            else
            {
                // TODO: return fuckoff code (technical term)
            }
        }

        [WebMethod]
        public static bool TestLogin(string uriEncoded, string nonce)
        {
            string uri = HttpUtility.UrlDecode(uriEncoded);

            // a little sloppy nonce and uri checking rather than full parsing
            // TODO: Full parse
            if (!uri.Contains(nonce) || !uri.Contains(HttpContext.Current.Request.Url.Host))
            {
                throw new ArgumentException();
            }

            string result = GuidCache.Get(uri + "-LoggedOn") as string;
            if (string.IsNullOrEmpty(result))
            {
                return false;
            }

            // We have a successful login when we get here

            GuidCache.Delete(uri + "-Logon");
            GuidCache.Delete(uri + "-LoggedOn");

            FormsAuthentication.RedirectFromLoginPage(result, true); // set auth cookie
            return true;
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
        }

        private void Localize()
        {
            this.LabelCurrentOrganizationName.Text = Resources.Global.Global_Organization;
            this.LabelCurrentUserName.Text = Resources.Global.Title_Person_Generic;
            this.LabelPageTitle.Text = Resources.Pages_Security.Login_PageTitle;
            this.LabelPreferences.Text = Resources.Global.Global_NA;
            this.LabelSidebarInfoHeader.Text = Resources.Global.Sidebar_Information;
            this.LabelSidebarHelpHeader.Text = Resources.Global.Sidebar_Help;
            this.LabelSidebarInfoContent.Text = Resources.Pages_Security.Login_Info;
            this.LabelSidebarManualLoginHeader.Text = Resources.Pages_Security.Login_ManualLogin;
            this.LabelHeader.Text = Resources.Pages_Security.Login_Header;
            this.LabelSidebarResetPassword.Text = Resources.Pages_Security.Login_ResetPassword;

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


        // ReSharper disable InconsistentNaming
        [Serializable]
        public class BitIdCredentials
        {
            // Lowercase for Json serializability to BitId spec.
            public string address { get; set; }
            public string uri { get; set; }
            public string signature { get; set; }
        }

        [Serializable]
        public class BitIdResponseSuccess
        {
            // Lowercase for Json serializability to BitId spec.
            public string address { get; set; }
            public string signature { get; set; }
        }
        // ReSharper restore InconsistentNaming


        protected void ButtonCheat_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}


