using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Telerik.Web.UI;
using Membership = Swarmops.Logic.Swarm.Membership;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;

namespace Swarmops
{
    public partial class MasterV5 : MasterV5Base
    {
        protected void Page_Init (object sender, EventArgs e)
        {
            // Security stuff
            // Current authentication

            string identity = HttpContext.Current.User.Identity.Name;
            string[] identityTokens = identity.Split(',');

            string userIdentityString = identityTokens[0];
            string organizationIdentityString = identityTokens[1];

            int currentUserId = Convert.ToInt32(userIdentityString);
            int currentOrganizationId = Convert.ToInt32(organizationIdentityString);

            _currentUser = Person.FromIdentity(currentUserId);
            _authority = _currentUser.GetAuthority();
            _currentOrganization = Organization.FromIdentity(currentOrganizationId);

            OutboundComm.CreateNotification(null, NotificationResource.System_Startup);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Event subscriptions

            this.LanguageSelector.LanguageChanged += new EventHandler(LanguageSelector_LanguageChanged);

            // Titles and other page elements

            this.IconPage.ImageUrl = "/Images/PageIcons/" + this.CurrentPageIcon + "-40px.png";
            this.LabelPageTitle.Text = this.CurrentPageTitle;
            this.Page.Title = "Swarmops - " + this.CurrentPageTitle;

            this.LiteralSidebarInfo.Text = this.CurrentPageInfoBoxLiteral;

            // Set logo image. If custom image is installed, use it instead.

            this.ImageLogo.ImageUrl = "~/Images/Logo-Stock.png";
            if (File.Exists(Server.MapPath("~/Images/Logo-Custom.png")))
            {
                this.ImageLogo.ImageUrl = "~/Images/Logo-Custom.png";
            }

            // Check for SSL and force it

            if (!Request.IsSecureConnection)
            {
                if (!Request.Url.ToString().StartsWith("http://dev.swarmops.com/") && !Request.Url.ToString().StartsWith("http://live.activizr.com") && !Request.Url.ToString().StartsWith("http://localhost:") && !Request.Url.ToString().StartsWith("http://cryptdb"))
                {
                    // Response.Redirect(Request.Url.ToString().Replace("http:", "https:"));

                    // Only force this if set to force it in database
                    // TODO: Make admin init task
                }
            }

            Localize();

            this.LabelCurrentUserName.Text = _currentUser.Name;
            this.LabelCurrentOrganizationName.Text = _currentOrganization.Name;

            this.LabelActionPlaceholder1.Text = "Action shortcut 1 (TODO)";
            this.LabelActionPlaceholder2.Text = "Action shortcut 2 (TODO)";
            this.LabelActionItemsHere.Text = "Action items here (TODO)";

            RadMenu mainMenu = FindControl("MainMenu") as RadMenu;

            Dictionary<string, bool> enableCache = new Dictionary<string, bool>();

            if (Session["MainMenu-v4_Enabling"] != null
                && PermissionCacheTimestamp.AddSeconds(10) > DateTime.Now
                && Authorization.lastReload < PermissionCacheTimestamp)
            {
                enableCache = Session["MainMenu-v4_Enabling"] as Dictionary<string, bool>;
            }
            else
            {
                PermissionCacheTimestamp = DateTime.Now;
            }

            SetupMenuItems();

            Authorization.flagReload = false;
            // SetupMenuItemsEnabling(authority, enableCache, menuItems);
            Session["MainMenu-v4_Enabling"] = enableCache;

            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginTop] = "3px";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginRight] = "3px";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.Cursor] = "pointer";

            SetupDropboxes();

            this.ButtonSwitchOrganizations.Attributes.Add("onclick", "$('#" + this.TextSwitchToOrganizationId.ClientID + "').val($('#" + this.DropSwitchOrganizations.ClientID + "').val()); $('#" + this.ButtonActuatorSwitchOrganizations.ClientID + "').click();");

            // Check for message to display

            HttpCookie dashMessage = Request.Cookies["DashboardMessage"];

            if (dashMessage != null && dashMessage.Value.Length > 0)
            {
                this.LiteralDocumentReadyHook.Text = string.Format("alertify.alert(unescape('{0}'.replace(/\\+/g, '%20')));", dashMessage.Value);
                Response.SetCookie(new HttpCookie("DashboardMessage", string.Empty));
                Response.Cookies ["DashboardMessage"].Expires = DateTime.Now.AddYears(-10);
            }
            else
            {
                this.LiteralDocumentReadyHook.Text = string.Empty;
            }
        }

        private void SetupDropboxes()
        {
            this.DropSwitchOrganizations.Items.Clear();

            int sandboxId = Organization.SandboxIdentity;

            this.DropSwitchOrganizations.Items.Add(new ListItem(Resources.Global.Global_DropInits_SelectOrganization, "0"));
            this.DropSwitchOrganizations.Items.Add(new ListItem("Sandbox", sandboxId.ToString()));

            Memberships memberships = _currentUser.GetMemberships();

            foreach (Membership membership in memberships)
            {
                if (membership.OrganizationId != sandboxId) // sandbox is already added
                {
                    this.DropSwitchOrganizations.Items.Add(new ListItem(membership.Organization.Name,
                                                                        membership.OrganizationId.ToString(
                                                                            CultureInfo.InvariantCulture)));
                }
            }
        }

        private static string _buildIdentity;

        private void Localize()
        {
            this.LabelSidebarInfo.Text = Resources.Global.Sidebar_Information;
            this.LabelSidebarActions.Text = Resources.Global.Sidebar_Actions;
            this.LabelSidebarTodo.Text = Resources.Global.Sidebar_Todo;

            string cultureString = Thread.CurrentThread.CurrentCulture.ToString();
            string cultureStringLower = cultureString.ToLowerInvariant();

            if (cultureStringLower == "en-us")
            {
                // doesn't work with tooltip styling for some completely inexplicable reason, use en-gb

                cultureString = "en-GB";
                cultureStringLower = "en-gb";
                Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureString);

                HttpCookie cookieCulture = new HttpCookie("PreferredCulture");
                cookieCulture.Value = cultureString;
                cookieCulture.Expires = DateTime.Now.AddDays(365);
                Response.Cookies.Add(cookieCulture);
            }

            string flagName = "uk";

            if (cultureStringLower != "en-gb" && cultureString.Length > 3)
            {
                flagName = cultureStringLower.Substring(3);
            }

            this.ImageCultureIndicator.ImageUrl = "~/Images/Flags/" + flagName + ".png";

            this.LinkLogout.Text = Resources.Global.CurrentUserInfo_Logout;
            this.LabelPreferences.Text = Resources.Global.CurrentUserInfo_Preferences;
            this.LiteralCurrentlyLoggedIntoSwitch.Text = string.Format(Resources.Global.Master_SwitchOrganizationDialog, _currentOrganization.Name);

            if (cultureStringLower != "en-gb" && cultureStringLower != "sv-se" && cultureString.Trim().Length > 0)
            {
                this.LinkTranslate.Visible = true;
            }
            else
            {
                this.LinkTranslate.Visible = false;
            }
        }

        private void SetupMenuItems()
        {
            RadMenuItemCollection menuItems = this.MainMenu.Items;
            SetupMenuItemsRecurse(menuItems, true);
        }

        private bool SetupMenuItemsRecurse(RadMenuItemCollection menuItems, bool topLevel)
        {
            // string thisPageUrl = Request.Url.Segments[Request.Url.Segments.Length - 1].ToLower();
            bool anyItemEnabled = false;

            foreach (RadMenuItem item in menuItems)
            {
                int itemUserLevel = Convert.ToInt32(item.Attributes["UserLevel"]);
                string authorization = item.Attributes["Permission"];
                string resourceKey = item.Attributes["GlobalResourceKey"];
                string url = item.NavigateUrl;
                string dynamic = item.Attributes["Dynamic"];

                item.Visible = itemUserLevel <= 4;   // TODO: Replace with user's actual level
                bool enabled = topLevel;

                if (item.IsSeparator)
                {
                    continue;
                }

                if (dynamic == "true")
                {
                    switch (item.Attributes["Template"])
                    {
                        case "Build#":
                            item.Text = GetBuildIdentity(); // only dynamically constructed atm -- if more, switch on "template" field
                            break;
                        case "CloseLedgers":
                            int year = DateTime.Today.Year;
                            int booksClosedUntil = _currentOrganization.Parameters.FiscalBooksClosedUntilYear;

                            if (_currentOrganization.Parameters.EconomyEnabled && booksClosedUntil < year - 1)
                            {
                                item.Text = String.Format(Resources.Menu5.Menu5_Ledgers_CloseBooks, booksClosedUntil + 1);
                            }
                            else
                            {
                                enabled = false;  // maybe even invisible?
                                url = string.Empty;
                                item.Text = String.Format(Resources.Menu5.Menu5_Ledgers_CannotCloseBooks, booksClosedUntil + 1);
                            }
                            break;
                        default:
                            throw new InvalidOperationException("No case for dynamic menu item" + item.Attributes["Template"]);
                    }
                }
                else
                {
                    item.Text = GetGlobalResourceObject("Menu5", resourceKey).ToString();
                }

                if (item.Visible)
                {
                    enabled |= SetupMenuItemsRecurse(item.Items, false);
                    enabled |= !String.IsNullOrEmpty(url);
                }

                item.Enabled = enabled;
                if (enabled)
                {
                    anyItemEnabled = true;
                }
            }

            return anyItemEnabled;
        }

        private string GetBuildIdentity()
        {
            // Read build number if not loaded, or set to "Private" if none

            if (_buildIdentity == null)
            {
                try
                {
                    using (StreamReader reader = File.OpenText(HttpContext.Current.Request.MapPath("~/BuildIdentity.txt")))
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


                /*
                if (Convert.ToInt32(item.Attributes["UserLevel"]) < 4)   // TODO: user's menu level
                {
                    item.Visible = false;
                }*/

                /*
                item.Enabled = true;
                if (string.IsNullOrEmpty(item.Attributes["Permission"]) == false)
                {
                    string permString = item.Attributes["Permission"].ToString();
                    PermissionSet allowedFor = new PermissionSet(permString);
                }
                else
                {
                    RadMenuItem permissionParent = item;
                    while (permissionParent.Parent is RadMenuItem)
                    {
                        if (string.IsNullOrEmpty(permissionParent.Attributes["Permission"]) == false)
                        {
                            item.Enabled = permissionParent.Enabled;
                            item.Attributes["Permission"] = permissionParent.Attributes["Permission"].ToString();
                            break;
                        }
                        permissionParent = permissionParent.Parent as RadMenuItem;
                    }
                }
                if (string.IsNullOrEmpty(item.NavigateUrl) && item.Items.Count == 0)
                {
                    item.Enabled = false;
                }*/

                /*
                string[] currentItemUrlSplit = item.NavigateUrl.ToLower().Split(new char[] { '/', '?' }, StringSplitOptions.RemoveEmptyEntries);
                if (Array.Exists<string>(currentItemUrlSplit,
                                delegate(String s) { if (s == thisPageUrl) return true; else return false; })
                    )
                {
                    if (string.IsNullOrEmpty(item.Attributes["Permission"]) == false)
                    {
                        string permString = item.Attributes["Permission"].ToString();
                        ((PageV5Base)(this.Page)).pagePermissionDefault = new PermissionSet(permString);
                    }
                }
                if (item.Items.Count > 0)
                {
                    bool enabledSubItems = SetupMenuItemsEnabling(authority, enableCache, item.Items);
                    if (enabledSubItems)
                    {
                        item.Enabled = true;
                    }
                    else
                    {
                        item.Enabled = false;
                    }
                }
                else if (string.IsNullOrEmpty(item.NavigateUrl))
                {
                    item.Enabled = false;
                }*/
            
        


        string CollectItemID(IRadMenuItemContainer item)
        {
            if (item.Owner == null)
            {
                return "";
            }
            else
            {
                return CollectItemID(item.Owner) + "_" + ((RadMenuItem)item).ID;
            }
        }


        protected void LinkLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        public event EventHandler LanguageChanged;

        private void LanguageSelector_LanguageChanged(object sender, EventArgs e)
        {
            // Received event from control - refire

            _currentUser.PreferredCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            this.LiteralCrowdinScript.Text = string.Empty;

            if (LanguageChanged != null)
            {
                LanguageChanged(this, new EventArgs());
            }
        }

        protected void LinkTranslate_Click(object sender, EventArgs e)
        {
            string crowdinCode =
                "<!-- Crowdin JIPT Begin -->\r\n" +
                "<script type=\"text/javascript\" src=\"http://jipt.crowdin.net/jipt.js\" jipt=\"api\"></script>\r\n" +
                "<script type=\"text/javascript\">\r\n" +
                "    Crowdin.ready(function(jipt) {\r\n" +
                "        jipt.set_project_identifier('activizr');\r\n" +
                "        jipt.set_show_translations(true);\r\n" +
                "        jipt.set_marker('border');\r\n" +
                "        jipt.set_target_language('" + Thread.CurrentThread.CurrentCulture.ToString().Substring(0,2) + "');\r\n" +
                "        jipt.login('activizr_translator', 'anonymous', function() {\r\n" +
                "          jipt.start_translation();\r\n" +
                "        });\r\n" +
                "    });\r\n" +
                "</script>\r\n" +
                "<!-- Crowdin JIPT End -->\r\n";

            this.LiteralCrowdinScript.Text = crowdinCode;
            // Page.ClientScript.RegisterStartupScript(this.GetType(), "crowdin", crowdinCode, false);
        }

        protected void ButtonSwitchOrganizations_Click(object sender, EventArgs e)
        {
            int currentUserId = _currentUser.Identity;
            int desiredOrganizationId = Int32.Parse(this.TextSwitchToOrganizationId.Text);

            if (desiredOrganizationId != 0)
            {
                bool foundMembership = false;
                Memberships currentUserMemberships = _currentUser.GetMemberships();
                foreach (Membership membership in currentUserMemberships)
                {
                    if (membership.OrganizationId == desiredOrganizationId)
                    {
                        foundMembership = true;
                    }
                }

                if (foundMembership)
                {
                    FormsAuthentication.RedirectFromLoginPage(
                        string.Format(CultureInfo.InvariantCulture, "{0},{1}", currentUserId, desiredOrganizationId),
                        true);
                    Response.Redirect("/");
                }
            }
        }

    }
}