using System;
using System.Web.UI;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Common
{

    /// <summary>
    ///     Summary description for MasterV4Base
    /// </summary>
    public class MasterV5Base : MasterPage
    {
        public bool CurrentPageAllowed = false;
        public string CurrentPageIcon = string.Empty;

        public string CurrentPageInfoBoxLiteral = string.Empty;
        public bool CurrentPageProhibited = false;

        public string CurrentPageTitle = string.Empty;

        // ReSharper disable InconsistentNaming
        protected Authority _authority = null; // This is set in Master-v5.master.cs

        public EasyUIControl EasyUIControlsUsed { get; set; }
        // these are set by each page, and called by Master to render in ExternalScripts control
        public IncludedControl IncludedControlsUsed { get; set; } // as above with IncludedScripts control
        // ReSharper restore InconsistentNaming


        public MasterV5Base()
        {
            IncludedControlsUsed = IncludedControl.JsonParameters; // Include this by default to reduce errors
        }


        public DateTime PermissionCacheTimestamp
        {
            get
            {
                if (Session["MainMenu-v5_Enabling_TimeStamp"] != null)
                {
                    return new DateTime ((long) Session["MainMenu-v5_Enabling_TimeStamp"]);
                }
                return DateTime.MinValue;
            }
            set { Session["MainMenu-v5_Enabling_TimeStamp"] = value.Ticks; }
        }

        public Person CurrentUser
        {
            get { return this._authority.Person; }
        }

        public Organization CurrentOrganization
        {
            get { return this._authority.Organization; }
        }

        public Authority CurrentAuthority
        {
            get { return this._authority; }
        }
    }

}