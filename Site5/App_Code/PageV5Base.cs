using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using Swarmops.Database;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Structure;

/// <summary>
/// Base class to use for all data generators (JSON, etc). It supplies identification and localization.
/// </summary>

public class PageV5Base : System.Web.UI.Page
{
    public PermissionSet pagePermissionDefault = new PermissionSet(Permission.CanSeeSelf); //Use from menu;
    public Access PageAccessRequired = null; // v5 mechanism
    public int DbVersionRequired = 0; // v5 mechanism

    /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
    protected override void OnInitComplete(System.EventArgs e)
    {
        base.OnInitComplete(e);
    }

    protected new MasterV5Base Master
    { get { return (MasterV5Base)base.Master; } }

    protected string PageTitle
    {
        get { return this.Master.CurrentPageTitle; }
        set { this.Master.CurrentPageTitle = value; }
    }

    protected string PageIcon
    {
        get { return this.Master.CurrentPageIcon; }
        set { this.Master.CurrentPageIcon = value; }
    }

    protected string InfoBoxLiteral
    {
        get { return this.Master.CurrentPageInfoBoxLiteral; }
        set { this.Master.CurrentPageInfoBoxLiteral = value; }
    }

    protected Person CurrentUser
    {
        get { return this.Master.CurrentUser; }
    }

    protected Organization CurrentOrganization
    {
        get { return this.Master.CurrentOrganization; }
    }

    protected Authority CurrentAuthority
    {
        get { return this.Master.CurrentAuthority; }
    }

    protected override void OnPreInit(EventArgs e)
    {
        CommonV5Base.CulturePreInit(Request);

 	    base.OnPreInit(e);
    }

    protected override void  OnPreRender(EventArgs e)
    {
        // Check security of page against users's credentials

        if (!CurrentUser.HasAccess (this.PageAccessRequired))
        {
            Response.Redirect("/Pages/v5/Security/AccessDenied.aspx");
        }

        // Check necessary database revision

        if (this.DbVersionRequired > SwarmDb.DbVersion)
        {
            Response.Redirect("/Pages/v5/Security/DatabaseUpgradeRequired.aspx");
        }

 	    base.OnPreRender(e);
    }

    protected string LocalizeCount (string resourceString, int count)
    {
        return LocalizeCount(resourceString, count, false);
    }

    protected string LocalizeCount (string resourceString, int count, bool capitalize)
    {
        string result;
        string[] parts = resourceString.Split('|');

        switch (count)
        {
            case 0:
                result = parts[0];
                break;
            case 1:
                result = parts[1];
                break;
            default:
                result = String.Format(parts[2], count);
                break;
        }

        if (capitalize)
        {
            result = Char.ToUpperInvariant(result[0]) + result.Substring(1);
        }

        return result;
    }


    protected static AuthenticationData GetAuthenticationDataAndCulture()
    {
        // This function is called from static page methods in AJAX calls to get
        // the current set of authentication data. Static page methods cannot access
        // the instance data of PageV5Base.

        return CommonV5Base.GetAuthenticationDataAndCulture(HttpContext.Current);
    }

    /// <summary>
    /// This is used to identify special cases for pilot installations. Because of the privacy implications, it
    /// should not be used at all from general availability onwards, except for those pilot installations.
    /// </summary>
    protected string InstallationId
    {
        get { return Persistence.Key["SwarmopsInstallationId"]; }
    }
}



public class AuthenticationData
{
    public Person CurrentUser;
    public Organization CurrentOrganization;
}