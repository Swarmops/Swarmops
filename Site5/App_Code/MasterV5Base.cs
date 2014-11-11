using System;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

/// <summary>
/// Summary description for MasterV4Base
/// </summary>

public class MasterV5Base : System.Web.UI.MasterPage
{

    public bool CurrentPageAllowed = false;
    public bool CurrentPageProhibited = false;

    public string CurrentPageTitle = string.Empty;
    public string CurrentPageIcon = string.Empty;

    public string CurrentPageInfoBoxLiteral = string.Empty;

    // ReSharper disable InconsistentNaming
    protected Person _currentUser = null;   // These are set in Master-v5.master.cs
    protected Authority _authority = null;
    protected Organization _currentOrganization = null;
    
    public EasyUIControl EasyUIControlsUsed { get; set; }  // these are set by each page, and called by Master to render in ExternalScripts control
    // ReSharper restore InconsistentNaming

    public DateTime PermissionCacheTimestamp
    {
        get
        {
            if (Session["MainMenu-v5_Enabling_TimeStamp"] != null)
            {
                return new DateTime((long)Session["MainMenu-v5_Enabling_TimeStamp"]);
            }
            else
                return DateTime.MinValue;

        }
        set
        {
            Session["MainMenu-v5_Enabling_TimeStamp"] = value.Ticks;
        }
    }

    public Person CurrentUser
    {
        get { return this._currentUser; }
    }

    public Organization CurrentOrganization
    {
        get { return this._currentOrganization; }
    }

    public Authority CurrentAuthority
    {
        get { return this._authority; }
    }

}


