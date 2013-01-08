using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using Swarmops.Logic.Security;
using Swarmops.Logic.Swarm;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Structure;

/// <summary>
/// Summary description for PageV5Base
/// Base class to use for all pages that uses the Activizr-v5 master page
/// </summary>

public class PageV5Base : System.Web.UI.Page
{
    public PermissionSet pagePermissionDefault = new PermissionSet(Permission.CanSeeSelf); //Use from menu;
    public Access PageAccessRequired = null; // v5 mechanism

    /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
    protected override void OnInitComplete(System.EventArgs e)
    {
        base.OnInitComplete(e);

        int currentUserId = 0;
        int currentOrganizationId = 0;

        string identity = HttpContext.Current.User.Identity.Name;
        string[] identityTokens = identity.Split(',');

        string userIdentityString = identityTokens[0];
        string organizationIdentityString = identityTokens[1];

        currentUserId = Convert.ToInt32(userIdentityString);
        currentOrganizationId = Convert.ToInt32(organizationIdentityString);
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
        // Unlock Telerik

        this.Application["Telerik.Web.UI.Key"] = "Activizr";

        // Localization

        // Set default culture (English, United States, but that doesn't work so fake it to GB)

        string preferredCulture = "en-GB";

        // -----------  SET CULTURE ------------

        // Does the user have a culture preference?

        if (Request.Cookies["PreferredCulture"] != null)
        {
            // Yes, set it
            preferredCulture = Request.Cookies["PreferredCulture"].Value;
        }
        else
        {
            // No, determine from browser
            string browserPreference = "en-GB";
            if (Request.UserLanguages != null && Request.UserLanguages.Length > 0)
            {
                browserPreference = Request.UserLanguages[0];
                preferredCulture = browserPreference;
            }


            /*
            string[] languages = (string[])Application["Cultures"];
            for (int index = 0; index < languages.Length; index++)
            {
                if (languages[index].StartsWith(browserPreference))
                {
                    preferredCulture = languages[index];
                }
            }*/
        }

        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(preferredCulture);
        }
        catch (Exception exception)
        {
            throw new Exception("Could not set culture \"" + preferredCulture + "\"", exception);
            // Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

 	    base.OnPreInit(e);
    }

    protected override void  OnPreRender(EventArgs e)
    {
        // Check security of page against users's credentials

        if (!CurrentUser.HasAccess (this.PageAccessRequired))
        {
            Response.Redirect("/Pages/v5/Security/AccessDenied.aspx");
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
}


