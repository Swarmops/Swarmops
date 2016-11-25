using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Versioning;
using System.Web;
using System.Web.ExtensionMethods;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using NBitcoin;
using Resources;
using Swarmops.Frontend;
using Swarmops.Interface.Support;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Pages.Security
{
    public partial class Login : DataV5Base // "Data" because we don't have a master page
    {
        private static string _buildIdentity;

        protected void Page_Load (object sender, EventArgs e)
        {
            // Check if this is the first run ever. If so, redirect to Init.

            if (!SupportFunctions.DatabaseConfigured)
            {
                // ReSharper disable once Html.PathError   <-- this line is required for Resharper to not flag the next line as invalid
                Response.Redirect ("/Initialize", true);
                return;
            }

            // Persistence.Key["Debug_RawData"] = Request.ToRaw();

            // Check for POST data - for BitId via Webform

            if (Request.HttpMethod == "POST")
            {
                // We should ONLY get here if we're getting a BitId by Webform submission.

                Persistence.Key["BitId_RawData"] = Request.ToRaw();

                if (Request.Params["address"] != null)
                {
                    // yes, indeed looks like it

                    BitIdCredentials credentials = new BitIdCredentials
                    {
                        address = Request.Params["address"],
                        uri = Request.Params["uri"],
                        signature = Request.Params["signature"]
                    };

                    ProcessRespondBitId (credentials, Response);
                    return;
                }
                if (Request.ContentType == "application/json")
                {
                    BitIdCredentials credentials =
                        new JavaScriptSerializer().Deserialize<BitIdCredentials> (
                            new StreamReader (Request.InputStream).ReadToEnd());
                    // TODO: untested but seems to work. Throws?

                    ProcessRespondBitId (credentials, Response);
                    return;
                }
            }

            string requestHost = Request.Url.Host;



            // If this is the Dev Sandbox, autologin

            if ((requestHost == "sandbox.swarmops.com" || requestHost == "dev.swarmops.com") &&
                PilotInstallationIds.IsPilot (PilotInstallationIds.DevelopmentSandbox) &&
                Request.QueryString["SuppressAutologin"] != "true")
            {
                DashboardMessage.Set ("<p>You have been logged on as <strong>Sandbox Administrator</strong> to the Swarmops Development Sandbox.</p><br/><p>This machine runs the latest development build, so you may run into diagnostic code and half-finished features. All data here is bogus test data and is reset every night.</p><br/><p><strong>In other words, welcome, and play away!</strong></p>");
                FormsAuthentication.SetAuthCookie (Authority.FromLogin (Person.FromIdentity (1), Organization.Sandbox).ToEncryptedXml(), true);
                Response.Redirect ("/");
            }

            // If we're on an Open Ledgers domain, autologin as Open Ledgers

            Organization organizationOpenLedgers = Organization.FromOpenLedgersDomain(requestHost); // returns null if doesn't exist

            if (organizationOpenLedgers != null)
            {
                DashboardMessage.Set (String.Format(Resources.Pages.Security.Login_AsOpenLedgers, organizationOpenLedgers.Name));
                FormsAuthentication.SetAuthCookie(Authority.FromLogin (Person.FromIdentity (Person.OpenLedgersIdentity), organizationOpenLedgers).ToEncryptedXml(), true);
                Response.Redirect (@"/Ledgers/BalanceSheet");
            }

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

            // TODO: Same thing for Pound/HAProxy deployments

            // Rewrite if applicable

            if (Request.Url.ToString().StartsWith ("http://") && !cloudFlareSsl)
                // only check client-side as many server sites de-SSL the connection before reaching the web server
            {
                if (!Request.Url.ToString().StartsWith ("http://dev.swarmops.com/") &&
                    !(Request.Url.ToString().StartsWith ("http://localhost:") && Debugger.IsAttached))  // Debugger.IsAttached is necessary, as link can be faked
                {
                    if (SystemSettings.RequireSsl)
                    {
                        Response.Redirect (Request.Url.ToString().Replace ("http:", "https:"));
                    }
                }
            }


            // If we're on a vanity domain, enable the self-signup link

            Organization vanityOrganization = Organization.FromVanityDomain (Request.Url.Host);

            if (vanityOrganization != null)
            {
                this.LiteralSelfSignupLink.Text = @"//" + vanityOrganization.VanityDomain + @"/Signup";
                this.LabelSelfSignup.Text = String.Format (Resources.Pages.Security.Login_SelfSignup, vanityOrganization.Name);
                this.LabelSelfSignupHeader.Text = Resources.Pages.Security.Login_SelfSignupHeader;
                this.PanelJoin.Visible = true;
            }


            // If we're debugging, enable the auto- / impersonation login. This MUST NEVER fire outside of development environment.

            if (Debugger.IsAttached && Path.DirectorySeparatorChar == '\\')
            // on Windows, with a debugger attached, so this is not a production environment
            {
                // but check that we're running against Localhost as well

                if (Request.Url.ToString().StartsWith("http://localhost:"))
                {
                    this.PanelCheat.Visible = true;
                }
            }
            
            
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginTop] = "-3px";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginRight] = "3px";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.Cursor] = "pointer";

            _cacheVersionMark = Logic.Support.Formatting.SwarmopsVersion;
            if (_cacheVersionMark.StartsWith("Debug"))
            {
                _cacheVersionMark = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            }
            _cacheVersionMark = SHA1.Hash(_cacheVersionMark).Replace(" ", "").Substring(0, 8);

            Localize();

            // Generate BitID tokens

            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString().Replace ("-", "");

            string nonce = guidString + DateTime.UtcNow.Ticks.ToString ("x8");

            string hostName = Request.Url.Host;

            string bitIdUri = "bitid://" + hostName + "/Security/Login.aspx/BitIdLogin?x=" + nonce;

            if (Request.Url.ToString().StartsWith ("http://") && !cloudFlareSsl)
            {
                bitIdUri += "&u=1";
            }

            this.LiteralUri.Text = HttpUtility.UrlEncode (bitIdUri);
            this.LiteralNonce.Text = nonce;

            GuidCache.Set (bitIdUri + "-Logon", "Unauth");

            // TODO: need to NOT USE GOOGLE CHARTS for this but bring home a free QR package

            this.ImageBitIdQr.ImageUrl =
                "https://chart.googleapis.com/chart?cht=qr&chs=400x400&chl=" + HttpUtility.UrlEncode (bitIdUri);
        }


        protected string _cacheVersionMark; // CSS cache buster

        protected static void ProcessRespondBitId (BitIdCredentials credentials, HttpResponse response)
        {

            BitcoinPubKeyAddress testAddress = new BitcoinPubKeyAddress (credentials.address);
            if (testAddress.VerifyMessage (credentials.uri, credentials.signature))
            {
                // woooooo

                try
                {
                    // Test for registration

                    string intent = GuidCache.Get(credentials.uri + "-Intent") as string;

                    if (!string.IsNullOrEmpty(intent))
                    {
                        if (intent.StartsWith("Register"))
                        {
                            int personId = Int32.Parse(intent.Substring("Register-".Length));
                            Person person = Person.FromIdentity(personId);
                            person.BitIdAddress = credentials.address;
                            GuidCache.Set(credentials.uri + "-Intent", "Complete");
                        }
                    }

                    // Otherwise, test for logon

                    if (GuidCache.Get (credentials.uri + "-Logon") as string == "Unauth")
                    {
                        Person person = Person.FromBitIdAddress (credentials.address);

                        // TODO: If above throws, show friendly "unknown wallet" message

                        // TODO: Determine last logged-on organization. Right now, log on to Sandbox.

                        GuidCache.Set (credentials.uri + "-LoggedOn",
                            person.Identity.ToString (CultureInfo.InvariantCulture) + ",1,,BitId 2FA");
                    }
                }
                catch (Exception e)
                {
                    Persistence.Key["BitIdLogin_Debug_Exception"] = e.ToString();
                }

                // TODO: Error codes

                response.StatusCode = 200;
                response.SetJson();
                response.Write ("{\"address\":\"" + credentials.address + "\",\"signature\":\"" + credentials.signature +
                                "\"}");
                response.End();
            }
            else
            {
                response.StatusCode = 401; // Be a bit more friendly in your return codes
            }
        }

        [WebMethod]
        public static bool TestLogin(string uriEncoded, string nonce)
        {
            try
            {
                string uri = HttpUtility.UrlDecode(uriEncoded);

                // a little sloppy nonce and uri checking rather than full parsing
                // TODO: Full URI parse, the above is not enough
                if (!uri.Contains(nonce) || !uri.Contains(HttpContext.Current.Request.Url.Host))
                {
                    throw new ArgumentException();
                }

                string result = (string)GuidCache.Get(uri + "-LoggedOn");
                if (string.IsNullOrEmpty(result))
                {
                    return false;
                }

                // We have a successful login when we get here

                GuidCache.Delete(uri + "-Logon");
                GuidCache.Delete(uri + "-LoggedOn");
                GuidCache.Set(nonce + "-Identity", result);

                return true;
            }
            catch (Exception e)
            {
                Persistence.Key["BitId_Test_Exception"] = e.ToString();

                throw;
            }
        }


        [WebMethod]
        // ReSharper disable once InconsistentNaming
        public static string TestCredentials (string credentialsLogin, string credentialsPass, string credentials2FA,
            string logonUriEncoded)
        {
            if (!string.IsNullOrEmpty (credentialsLogin.Trim()) && !string.IsNullOrEmpty (credentialsPass.Trim()))
            {
                string logonUri = HttpUtility.UrlDecode (logonUriEncoded);

                try
                {
                    Person authenticatedPerson = Authentication.Authenticate (credentialsLogin,
                        credentialsPass);

                    int lastOrgId = authenticatedPerson.LastLogonOrganizationId;

                    if (PilotInstallationIds.IsPilot (PilotInstallationIds.PiratePartySE) && (lastOrgId == 3 || lastOrgId == 0))
                    {
                        lastOrgId = 1; // legacy: log on to Piratpartiet SE if indeterminate; prevent sandbox for this pilot
                        authenticatedPerson.LastLogonOrganizationId = 1; // avoid future legacy problems
                    }
                    else if (lastOrgId == 0)
                    {
                        lastOrgId = Organization.SandboxIdentity;
                    }

                    Authority testAuthority = Authority.FromLogin (authenticatedPerson,
                        Organization.FromIdentity (lastOrgId));

                    if (!authenticatedPerson.MemberOfWithInherited (lastOrgId) && !testAuthority.HasSystemAccess (AccessType.Read))
                    {
                        // If the person doesn't have access to the last organization (anymore), log on to Sandbox
                        // unless first pilot, in which case throw (deny login)

                        if (PilotInstallationIds.IsPilot (PilotInstallationIds.PiratePartySE))
                        {
                            throw new UnauthorizedAccessException();
                        }

                        lastOrgId = Organization.SandboxIdentity;
                    }

                    GuidCache.Set (logonUri + "-LoggedOn",
                        Authority.FromLogin (authenticatedPerson, Organization.FromIdentity (lastOrgId)).ToEncryptedXml());

                    return "Success";  // Prepare here for "2FARequired" return code
                }
                catch (UnauthorizedAccessException)
                {
                    return "Fail";
                }
            }

            return "Fail";
        }


        [WebMethod]
        public static void BitIdLogin(string address, string signature, string uri)
        {
            BitIdCredentials credentials = new BitIdCredentials
            {
                address = address,
                signature = signature,
                uri = uri
            };

            ProcessRespondBitId(credentials, HttpContext.Current.Response);
        }

        protected override void OnPreInit (EventArgs e)
        {
            base.OnPreInit (e);
        }

        private void Localize()
        {
            this.LabelCurrentOrganizationName.Text = Global.Global_Organization;
            this.LabelCurrentUserName.Text = Participant.Localized (ParticipantTitle.Person, TitleVariant.Generic);
            this.LabelPreferences.Text = Global.Global_NA;
            this.LabelSidebarInfoHeader.Text = Global.Sidebar_Information;
            this.LabelSidebarHelpHeader.Text = Global.Sidebar_Help;
            this.LabelSidebarInfoContent.Text = Resources.Pages.Security.Login_Info;
            this.LabelSidebarManualLoginHeader.Text = Resources.Pages.Security.Login_ManualLogin;
            this.LabelHeader.Text = Resources.Pages.Security.Login_Header;
            this.LabelSidebarResetPassword.Text = Resources.Pages.Security.Login_ResetPassword;
            this.LiteralCredentialsUser.Text = Resources.Pages.Security.Login_Username;
            this.LiteralCredentialsPass.Text = Resources.Pages.Security.Login_Password;
            this.LiteralCredentials2FA.Text = Resources.Pages.Security.Login_GoogleAuthenticatorCode;
        }


        private string GetBuildIdentity()
        {
            // Read build number if not loaded, or set to "Private" if none

            if (_buildIdentity == null)
            {
                try
                {
                    using (
                        StreamReader reader = File.OpenText (HttpContext.Current.Request.MapPath ("~/BuildIdentity.txt"))
                        )
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


        protected void ButtonLogin_Click (object sender, EventArgs args)
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

        protected void ButtonCheat_Click (object sender, EventArgs e)
        {
            // If we're debugging, enable the auto- / impersonation login. This MUST NEVER fire outside of development environment.

            if (Debugger.IsAttached && Path.DirectorySeparatorChar == '\\')
            // on Windows, with a debugger attached, so this is not a production environment
            {
                // but check that we're running against Localhost on non-SSL on a nonstandard port as well

                if (Request.Url.ToString().StartsWith("http://localhost:"))
                {
                    Authority cheatLogon = Authority.FromLogin (Person.FromIdentity (1), Organization.Sandbox);
                    FormsAuthentication.RedirectFromLoginPage (cheatLogon.ToEncryptedXml(), true);
                }
            }
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
    }
}