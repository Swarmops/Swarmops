using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Telerik.Web.UI;

namespace Activizr
{
    public partial class MasterV5 : MasterV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Event subscriptions

            this.LanguageSelector.LanguageChanged += new EventHandler(LanguageSelector_LanguageChanged);

            // Titles etc

            this.IconPage.ImageUrl = "/Images/PageIcons/" + this.CurrentPageIcon + "-40px.png";
            this.LabelPageTitle.Text = this.CurrentPageTitle;
            this.Page.Title = "Activizr - " + this.CurrentPageTitle;

            // Check for SSL and force it

            if (!Request.IsSecureConnection)
            {
                if (!Request.Url.ToString().StartsWith("http://dev.activizr.com/") && !Request.Url.ToString().StartsWith("http://localhost:"))
                {
                    Response.Redirect(Request.Url.ToString().Replace("http:", "https:"));
                }
            }

            Localize();

            // Current authentication

            string identity = HttpContext.Current.User.Identity.Name;
            string[] identityTokens = identity.Split(',');

            string userIdentityString = identityTokens[0];
            string organizationIdentityString = identityTokens[1];

            int currentUserId = Convert.ToInt32(userIdentityString);
            int currentOrganizationId = Convert.ToInt32(organizationIdentityString);

            this.LabelCurrentUserName.Text = Person.FromIdentity(currentUserId).Name;
            this.LabelCurrentOrganizationName.Text = Organization.FromIdentity(currentOrganizationId).Name;

            // Security stuff

            _viewingPerson = Person.FromIdentity(currentUserId);
            _authority = _viewingPerson.GetAuthority();

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
        }

        private Person _viewingPerson;
        private Authority _authority;
        private static string _buildIdentity;


        private void Localize()
        {
            string cultureString = Thread.CurrentThread.CurrentCulture.ToString();
            string cultureStringLower = cultureString.ToLowerInvariant();

            string flagName = "uk";

            if (cultureStringLower != "en-us" && cultureString.Length > 3)
            {
                flagName = cultureStringLower.Substring(3);
            }

            this.ImageCultureIndicator.ImageUrl = "~/Images/Flags/" + flagName + ".png";

            this.LinkLogout.Text = Resources.Pages.Global.CurrentUserInfo_Logout;
            this.LabelPreferences.Text = Resources.Pages.Global.CurrentUserInfo_Preferences;

            if (cultureStringLower != "en-us" && cultureStringLower != "sv-se" && cultureString.Trim().Length > 0)
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

                item.Visible = itemUserLevel <= 4;

                if (item.IsSeparator)
                {
                    continue;
                }

                if (dynamic == "true")
                {
                    item.Text = GetBuildIdentity(); // only dynamically constructed atm -- if more, switch on "template" field
                }
                else
                {
                    item.Text = GetGlobalResourceObject("Menu5", resourceKey).ToString();
                }

                bool enabled = topLevel;

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


        protected void Page_Init(object sender, EventArgs e)
        {
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

            _viewingPerson.PreferredCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
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
    }
}