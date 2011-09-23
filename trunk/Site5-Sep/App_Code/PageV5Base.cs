using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using Activizr.Logic.Security;
using Activizr.Logic.Pirates;
using Activizr.Basic.Enums;
using Activizr.Logic.Structure;

/// <summary>
/// Summary description for PageV5Base
/// Base class to use for all pages that uses the Activizr-v5 master page
/// </summary>

public class PageV5Base : System.Web.UI.Page
{
    public PermissionSet pagePermissionDefault = new PermissionSet(Permission.CanSeeSelf); //Use from menu;

    public Person _currentUser = null;
    public Authority _authority = null;
    public Organization _currentOrganization = null;

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
        _currentUser = Person.FromIdentity(currentUserId);
        _currentOrganization = Organization.FromIdentity(currentOrganizationId);
        _authority = _currentUser.GetAuthority();
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

    protected override void OnPreInit(EventArgs e)
    {
        // Localization

        // Set default culture (English, United States)

        string preferredCulture = "en-US";

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
            string browserPreference = "en-US";
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
}


