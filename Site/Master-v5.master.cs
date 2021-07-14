using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Swarmops.Interface.Support;
using Swarmops.Localization;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend
{
    public partial class MasterV5 : MasterV5Base
    {
        protected void Page_Init (object sender, EventArgs e)
        {
            try
            {
                this._authority = CommonV5.GetAuthenticationDataAndCulture(HttpContext.Current).Authority;
                this._authority.Organization.GetMemberCount(); // will throw if Organization can't be looked up
            }
            catch (Exception)
            {
                // if this fails FOR WHATEVER REASON then we're not authenticated

                this._authority = null;
                FormsAuthentication.SignOut();
                Response.Redirect("/", true);
            }

            // BEGIN TEST CODE

            foreach (FinancialAccount account in FinancialAccounts.ForOrganization(CurrentOrganization))
            {
                if (account.Name == "[LOC]Asset_PrivateWithdrawals")
                {
                    CurrentOrganization.FinancialAccounts.AssetsPrivateWithdrawals = account;
                }
                else if (account.Name == "[LOC]Debt_PrivateDeposits")
                {
                    CurrentOrganization.FinancialAccounts.DebtsPrivateDeposits = account;
                }
            }

            VatReports.CreateNewReports();

            // END TEST CODE

        }

        protected void Page_Load (object sender, EventArgs e)
        {
            // Event subscriptions

            // Titles and other page elements

            Page.Title = @"Swarmops - " + CurrentOrganization.NameShort + @" - " + CurrentPageTitle;

            this.ExternalScriptEasyUI.Controls = EasyUIControlsUsed.ToString();
            this.IncludedScripts.Controls = IncludedControlsUsed.ToString();

            this.LiteralSidebarInfo.Text = CurrentPageInfoBoxLiteral;

            // Set logo image. If there is no logo image, use text.

            if (CurrentOrganization.Identity == Organization.SandboxIdentity)
            {
                this.ImageLogo.ImageUrl = "~/Images/Other/swarmops-sandbox-logo--istockphoto.png";
                this.ImageLogo.Visible = true;
                this.LabelOrganizationName.Visible = false;
            }
            else
            {
                Document logoLandscapeDoc = CurrentOrganization.LogoLandscape;

                if (logoLandscapeDoc == null)
                {
                    this.ImageLogo.ImageUrl = "~/Images/Other/blank-logo-640x360.png";
                }
                else
                {
                    this.ImageLogo.ImageUrl = "~/Support/StreamUpload.aspx?DocId=" +
                                              logoLandscapeDoc.Identity.ToString(CultureInfo.InvariantCulture);
                }
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

            // TODO: Same thing for Pound deployments

            // Rewrite if applicable

            if (Request.Url.ToString().StartsWith ("http://") && !cloudFlareSsl && CurrentUser.Identity > 0)
                // only check client-side as many server sites de-SSL the connection before reaching the web server
            {
                if (!Request.Url.ToString().StartsWith ("http://dev.swarmops.com/") &&
                    !Request.Url.ToString().StartsWith("http://sandbox.swarmops.com/") &&
                    !Request.Url.ToString().StartsWith("http://localhost:"))
                {
                    if (SystemSettings.RequireSsl)
                    {
                        Response.Redirect (Request.Url.ToString().Replace ("http:", "https:"));
                    }
                }
            }

            Localize();

            _cacheVersionMark = Logic.Support.Formatting.SwarmopsVersion;
            if (_cacheVersionMark.StartsWith ("Debug"))
            {
                _cacheVersionMark = DateTime.UtcNow.ToString ("yyyy-MM-dd HH:mm:ss.ffff");
            }
            _cacheVersionMark = SHA1.Hash (_cacheVersionMark).Replace(" ", "").Substring (0, 8);

            this.LabelCurrentUserName.Text = CurrentAuthority.Person.Name;
            this.LabelCurrentOrganizationName.Text = CurrentAuthority.Organization.Name;

            this.LabelActionPlaceholder1.Text = "Action shortcut 1 (TODO)";
            this.LabelActionPlaceholder2.Text = "Action shortcut 2 (TODO)";
            this.LabelNoTodoItems.Text = LocalizedStrings.Get(LocDomain.Global, "Global_NoActionItems");

            // Set up todo items

            DashboardTodos todos = DashboardTodos.ForAuthority (CurrentAuthority);

            this.RepeaterTodoItems.DataSource = todos;
            this.RepeaterTodoItems.DataBind();

            if (todos.Count == 0)
            {
                // If no todos, hide the entire Todos box
                this.LiteralDocumentReadyHook.Text += @"$('div#divDashboardTodo').hide();";
            }

            // Set up main menu 

            SetupMenuItems();

            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginTop] = "-3px";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginRight] = "3px";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.Cursor] = "pointer";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.Height] = "25px";

            SetupDropboxes();

            // Check for message to display

            HttpCookie dashMessage = Request.Cookies["DashboardMessage"];

            if (dashMessage != null && dashMessage.Value.Length > 0)
            {
                this.LiteralDocumentReadyHook.Text +=
                    string.Format ("alertify.alert(SwarmopsJS.unescape('{0}'));", dashMessage.Value);
                DashboardMessage.Reset();
            }
            else
            {
                this.LiteralDocumentReadyHook.Text = string.Empty;
            }

            // Enable support for RTL languages

            if (Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
            {
                this.LiteralBodyAttributes.Text = @"dir='rtl' class='right-to-left'";
            }

            // If we're running as an open-something identity, remove the Preferences div

            if (CurrentUser.Identity < 0)
            {
                this.LiteralDocumentReadyHook.Text += @" $('#divUserPreferences').hide();";
            }

        }

        private void SetupDropboxes()
        {
        }

        private void Localize()
        {
            this.LabelSidebarInfo.Text = LocalizedStrings.Get(LocDomain.Global, "Sidebar_Information");
            this.LabelSidebarActions.Text = LocalizedStrings.Get(LocDomain.Global, "Sidebar_Actions");
            this.LabelSidebarTodo.Text = LocalizedStrings.Get(LocDomain.Global, "Sidebar_Todo");

            string cultureString = Thread.CurrentThread.CurrentCulture.ToString();
            string cultureStringLower = cultureString.ToLowerInvariant();

            if (cultureStringLower == "en-us")
            {
                cultureString = "en-US";
                cultureStringLower = "en-us";
                Thread.CurrentThread.CurrentCulture = new CultureInfo (cultureString);

                HttpCookie cookieCulture = new HttpCookie ("PreferredCulture");
                cookieCulture.Value = cultureString;
                cookieCulture.Expires = DateTime.Now.AddDays (365);
                Response.Cookies.Add (cookieCulture);
            }


            if (cultureStringLower == "af-za") // "South African Afrikaans", a special placeholder for localization code
            {
                InitTranslation();
            }
            else
            {
                this.LiteralCrowdinScript.Text = string.Empty;
            }

            this.ImageCultureIndicator.ImageUrl = SupportFunctions.FlagFileFromCultureId(cultureString);

            this.LinkLogout.Text = LocalizedStrings.Get(LocDomain.Global, "CurrentUserInfo_Logout");
            this.LabelPreferences.Text = LocalizedStrings.Get(LocDomain.Global, "CurrentUserInfo_Preferences");
            // this.LiteralCurrentlyLoggedIntoSwitch.Text = string.Format(LocalizedStrings.Get(LocDomain.Global, "Master_SwitchOrganizationDialog, _currentOrganization.Name);

            this.MasterLabelEditPersonHeaderAccount.Text = LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonHeaderAccount");
            this.MasterLabelEditPersonMail.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Mail");
            this.MasterLabelEditPersonName.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Name");
            this.MasterLabelEditPersonPhone.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Phone");
            this.MasterLabelEditPersonTwitter.Text = LocalizedStrings.Get(LocDomain.Global, "Global_TwitterId");

            this.MasterLabelEditPersonHeaderSecurity.Text = LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonHeaderSecurity");
            this.MasterLabelEditPerson2FA.Text = LocalizedStrings.Get(LocDomain.Global, "Master_EditPerson2FAEnable");
            this.MasterLabelEditPersonCurrentPassword.Text = LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonCurrentPassword");
            this.MasterLabelEditPersonNewPassword1.Text = LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonNewPassword1");
            this.MasterLabelEditPersonNewPassword2.Text = LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonNewPassword2");
            this.MasterEditPerson2FA.Label = LocalizedStrings.Get(LocDomain.Global, "Master_EditPerson2FAEnableShort");

            this.MasterLabelEditPersonHeaderSecurityProvisioning.Text =
                LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonHeaderSecurityProvisioning");
            this.MasterEditPerson2FAProvisioning.Label = LocalizedStrings.Get(LocDomain.Global, "Master_EditPerson2FAEnableShort");
            this.MasterLabelEditPersonResetPassword.Text = LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonResetPasswordLabel");
            this.MasterLabelEditPerson2FAProvisioning.Text = LocalizedStrings.Get(LocDomain.Global, "Master_EditPerson2FAEnable");

            this.MasterLabelEditPersonHeaderPaymentHistory.Text = LocalizedStrings.Get(LocDomain.Global, "Financial_PaymentHistory");
            this.MasterPersonEditLiteralHeaderAmountOwed.Text = LocalizedStrings.Get(LocDomain.Global, "Financial_Owed");
            this.MasterPersonEditLiteralHeaderAmountPaid.Text = LocalizedStrings.Get(LocDomain.Global, "Financial_Paid");
            this.MasterPersonEditLiteralHeaderItemOpenedDate.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Opened");
            this.MasterPersonEditLiteralHeaderItemClosedDate.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Closed");
            this.MasterPersonEditLiteralHeaderItemNotes.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Notes");
            this.MasterPersonEditLiteralHeaderItemName.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Item");
            this.MasterPersonEditLiteralHeaderItemDescription.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Description");


            this.MasterLabelBitIdRegisterHeader.Text = LocalizedStrings.Get(LocDomain.Global, "Master_BitIdRegister_Header");

            this.MasterLabelDashboardProfitLoss.Text = String.Format(LocalizedStrings.Get(LocDomain.Global, "Financial_ProfitToDate"),
                CurrentOrganization.Currency.DisplayCode);
        }

        protected string _cacheVersionMark;  // this is just a cache buster for style sheets on new versions

        private void SetupMenuItems()
        {
            // RadMenuItemCollection menuItems = this.MainMenu.Items;
            // SetupMenuItemsRecurse(menuItems, true);
        }

        /*
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
        }*/


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


        /*
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
        }*/


        protected void LinkLogout_Click (object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        private void InitTranslation()
        {
            string crowdinCode =
                "<!-- Crowdin JIPT Begin -->\r\n" +
                "<script type=\"text/javascript\">\r\n" +
                "  var _jipt = [];\r\n" +
                "  _jipt.push(['project', 'activizr']);\r\n" +
                "</script>\r\n" +
                "<script type=\"text/javascript\" src=\"//cdn.crowdin.com/jipt/jipt.js\"></script>\r\n" +
                "<!-- Crowdin JIPT End -->\r\n";

            this.LiteralCrowdinScript.Text = crowdinCode;
            // Page.ClientScript.RegisterStartupScript(this.GetType(), "crowdin", crowdinCode, false);
        }


        // Localized strings for accessing from aspx pages

        // ReSharper disable InconsistentNaming    <-- because the Localized_ prefix isn't appreciated
        public string Localized_LoadingPlaceholderShort
        {
            get { return CommonV5.JavascriptEscape (LocalizedStrings.Get(LocDomain.Global, "Global_LoadingPlaceholderShort")); }
        }

        public string Localized_MasterPersonEditResetPasswordButton
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonResetPasswordButton")); }
        }

        public string Localized_MasterPersonEditResetPasswordConfirm_Header
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonResetPasswordConfirm_Header").ToUpperInvariant()); }
        }

        public string Localized_MasterPersonEditResetPasswordConfirm_Text
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonResetPasswordConfirm_Text")); }
        }

        public string Localized_MasterPersonEditResetPasswordProhibited_Header
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonResetPasswordProhibited_Header").ToUpperInvariant()); }
        }

        public string Localized_MasterPersonEditResetPasswordProhibited_Text
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Master_EditPersonResetPasswordProhibited_Text")); }
        }

        public string Localized_Cancel
        {
            get { return CommonV5.JavascriptEscape (LocalizedStrings.Get(LocDomain.Global, "Global_Cancel")); }
        }

        public string Localized_Confirm
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Global_Confirm")); }
        }

        public string Localized_MasterPersonEdit_CannotProvision2FA
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Master_EditPerson2FAEnable_CannotProvision")); }
        }

        public string Localized_MasterPersonEdit_ConfirmRemove2FAProvision
        {
            get
            {
                return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Master_EditPerson2FAEnable_ConfirmRemoveProvision"));
            }
        }

        public string Localized_MasterPersonEdit_ConfirmRemove2FAOwn
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Master_EditPerson2FAEnable_ConfirmRemoveOwn")); }
        }

        public string Localized_MasterPersonEdit_Cancelled2FARemoval
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Master_EditPerson2FAEnable_NotRemoved")); }
        }

        public string Localized_BitIdRegister_Fail
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Master_BitIdRegister_Fail")); }
        }

        public string Localized_BitIdRegister_Sidebar
        {
            get { return CommonV5.JavascriptEscape(String.Format(LocalizedStrings.Get(LocDomain.Global, "Master_BitIdRegister_Sidebar"), "Mycelium", "Ledger")); } 
            
            // the parameters are examples of mobile wallets supporting BitId authentication; this was moved out of the resource file because it changes
        }


        public string Localized_GenericAjaxError
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Error_AjaxCallException")); }
        }


        public string Localized_ClientError_SocketFail
        {
            get
            {
                return
                    CommonV5.JavascriptEscape(
                        ErrorMessages.Localized("Client_SocketConnectionFault"));
            }
        }

        public string Localized_ClientError_HeartbeatFail
        {
            get { return CommonV5.JavascriptEscape(ErrorMessages.Localized("Client_ServerHeartbeatLost")); }
        }

        public string Localized_AllErrorsClear
        {
            get { return CommonV5.JavascriptEscape(ErrorMessages.Localized("AllClear")); }
        }

        public string Localized_AllTodosClear
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Sidebar_Todo_None")); }
        }

        public string GenerateBitIdToken
        {
            get
            {
                string cloudFlareVisitorScheme = this.Request.Headers["CF-Visitor"];
                bool cloudFlareSsl = false;

                if (!string.IsNullOrEmpty(cloudFlareVisitorScheme))
                {
                    if (cloudFlareVisitorScheme.Contains("\"scheme\":\"https\""))
                    {
                        cloudFlareSsl = true;
                    }
                }

                Guid guid = Guid.NewGuid();
                string guidString = guid.ToString().Replace("-", "");

                this.bitIdNonce = guidString + DateTime.UtcNow.Ticks.ToString("x8");

                string hostName = this.Request.Url.Host;

                string bitIdUri = "bitid://" + hostName + "/Security/Login.aspx/BitIdLogin?x=" + this.bitIdNonce;

                /* if (this.Request.Url.ToString().StartsWith("http://") && !cloudFlareSsl) // comment out -- never use insecure BitId
                {
                    bitIdUri += "&u=1";
                }*/

                GuidCache.Set(bitIdUri + "-Intent", "Register-" + CurrentUser.Identity.ToString(CultureInfo.InvariantCulture));  // Intent: register BitID

                return bitIdUri;
            }
        }

        private string bitIdNonce;

        public string GenerateBitIdNonce
        {
            get { return this.bitIdNonce; }
        }
    }
}