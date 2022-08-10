using System;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Swarmops.Localization;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend
{
    /// <summary>
    ///     Summary description for ControlV5Base
    /// </summary>
    public class ControlV5Base : UserControl
    {
        private Authority _authority;

        protected Organization CurrentOrganization
        {
            get { return _authority.Organization; }
        }

        protected Person CurrentUser
        {
            get { return _authority.Person; }
        }

        protected Authority CurrentAuthority
        {
            get { return this._authority; }
        }

        protected override void OnInit (EventArgs e)
        {
            this._authority = CommonV5.InitAuthority();
            base.OnInit (e);
        }

        protected string JavascriptEscape (string input)
        {
            return CommonV5.JavascriptEscape (input);
        }



        // ReSharper disable InconsistentNaming
        public string Localized_AjaxCallException
        {
            get { return JavascriptEscape (LocalizedStrings.Get(LocDomain.Global, "Error_AjaxCallException")); }
        }

        public string Localized_AjaxGeneralErrorSettingValue
        {
            get { return JavascriptEscape (LocalizedStrings.Get(LocDomain.Global, "Resources.Global.Error_UnspecifiedAjaxSetValue")); }
        }

        public string Localized_ConfirmDialog_Ok
        {
            get { return JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Resources.Global.Global_Ok")); }
        }

        public string Localized_ConfirmDialog_Proceed
        {
            get { return JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Resources.Global.Global_Proceed")); }
        }

        public string Localized_ConfirmDialog_Cancel
        {
            get { return JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Resources.Global.Global_Cancel")); }
        }
    }
}