using System;
using System.Security;
using System.Web;
using System.Web.UI;
using Swarmops.Common.Enums;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend
{
    /// <summary>
    ///     Base class to use for all data generators (JSON, etc). It supplies identification and localization.
    /// </summary>
    public class PageV5Base : Page
    {
        public int DbVersionRequired = 0; // v5 mechanism
        public Access PageAccessRequired = null; // v5 mechanism
        public bool IsDashboard = false; // if true, master doesn't box in

        protected new MasterV5Base Master
        {
            get { return (MasterV5Base) base.Master; }
        }

        protected string PageTitle
        {
            get { return Master.CurrentPageTitle; }
            set { Master.CurrentPageTitle = value; }
        }

        protected string PageIcon
        {
            get { return Master.CurrentPageIcon; }
            set { Master.CurrentPageIcon = value; }
        }

        protected string InfoBoxLiteral
        {
            get { return Master.CurrentPageInfoBoxLiteral; }
            set { Master.CurrentPageInfoBoxLiteral = value; }
        }

        // ReSharper disable once InconsistentNaming
        private EasyUIControl EasyUIControlsUsed
        {
            get { return Master.EasyUIControlsUsed; }
            set { Master.EasyUIControlsUsed = value; }
        }

        private IncludedControl IncludedControlsUsed
        {
            get { return Master.IncludedControlsUsed; }
            set { Master.IncludedControlsUsed = value; }
        }

        protected Person CurrentUser
        {
            get { return Master.CurrentUser; }
        }

        protected Organization CurrentOrganization
        {
            get { return Master.CurrentOrganization; }
        }

        protected Authority CurrentAuthority
        {
            get { return Master.CurrentAuthority; }
        }

        /// <summary>
        ///     This is used to identify special cases for pilot installations. It is also used as an installation-unique
        ///     encryption key salt in addition to record-specific salting.
        /// </summary>
        protected string InstallationId
        {
            get { return SystemSettings.InstallationId; }
        }

        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInitComplete (EventArgs e)
        {
            base.OnInitComplete (e);
        }

        protected override void OnPreInit (EventArgs e)
        {
            CommonV5.CulturePreInit (Request);

            base.OnPreInit (e);
        }

        protected override void OnPreRender (EventArgs e)
        {
            // Check that the page has security defined

            if (this.PageAccessRequired == null && !this.GetType().FullName.ToLowerInvariant().Contains ("accessdenied"))
            {
                throw new SecurityException ("Page security is undefined at " + this.GetType().FullName +
                                             ". This is not permitted: minimum security must be defined for every page.");
            }

            // Check security of page against users's credentials

            if (!this.CurrentAuthority.HasAccess (this.PageAccessRequired))
            {
                Response.Redirect ("/Pages/v5/Security/AccessDenied.aspx");
            }

            // Check necessary database revision

            if (this.DbVersionRequired > SupportFunctions.DatabaseSchemaVersion)
            {
                Response.Redirect ("/Pages/v5/Security/DatabaseUpgradeRequired.aspx");
            }

            base.OnPreRender (e);
        }

        protected string LocalizeCount (string resourceString, int count)
        {
            return LocalizeCount (resourceString, count, false);
        }

        protected string LocalizeCount (string resourceString, int count, bool capitalize)
        {
            string result;
            string[] parts = resourceString.Split ('|');

            switch (count)
            {
                case 0:
                    result = parts[0];
                    break;
                case 1:
                    result = parts[1];
                    break;
                default:
                    result = String.Format (parts[2], count);
                    break;
            }

            if (capitalize)
            {
                result = Char.ToUpperInvariant (result[0]) + result.Substring (1);
            }

            return result;
        }


        protected static AuthenticationData GetAuthenticationDataAndCulture()
        {
            // This function is called from static page methods in AJAX calls to get
            // the current set of authentication data. Static page methods cannot access
            // the instance data of PageV5Base.

            return CommonV5.GetAuthenticationDataAndCulture (HttpContext.Current);
        }

        public void RegisterControl (EasyUIControl control) // public for use by child controls
        {
            this.EasyUIControlsUsed |= control;
        }

        public void RegisterControl (IncludedControl control) // public for use by child controls
        {
            this.IncludedControlsUsed |= control;
        }

        public string JavascriptEscape (string input)
        {
            return CommonV5.JavascriptEscape (input);
        }


        // Common localizations

        // ReSharper disable InconsistentNaming
        public string Localized_SwitchLabelOn_Upper
        {
            get { return JavascriptEscape(Resources.Global.Global_On.ToUpperInvariant()); }
        }

        public string Localized_SwitchLabelOff_Upper
        {
            get { return JavascriptEscape(Resources.Global.Global_Off.ToUpperInvariant()); }
        }

        public string Localized_ConfirmDialog_Ok
        {
            get { return JavascriptEscape(Resources.Global.Global_Ok); }
        }

        public string Localized_ConfirmDialog_Proceed
        {
            get { return JavascriptEscape(Resources.Global.Global_Proceed); }
        }

        public string Localized_ConfirmDialog_Cancel
        {
            get { return JavascriptEscape(Resources.Global.Global_Cancel); }
        }

    }


}