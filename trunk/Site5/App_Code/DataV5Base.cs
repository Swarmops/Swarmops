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
/// Base class to use for all pages that uses the Swarmops-v5 master page
/// </summary>

public class DataV5Base : System.Web.UI.Page
{
    public PermissionSet pagePermissionDefault = new PermissionSet(Permission.CanSeeSelf); //Use from menu;
    public Access PageAccessRequired = null; // v5 mechanism

    /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
    protected override void OnInitComplete(System.EventArgs e)
    {
        base.OnInitComplete(e);

        string identity = HttpContext.Current.User.Identity.Name;
        string[] identityTokens = identity.Split(',');

        string userIdentityString = identityTokens[0];
        string organizationIdentityString = identityTokens[1];

        this.CurrentUser = Person.FromIdentity(Int32.Parse(userIdentityString));
        this.CurrentOrganization = Organization.FromIdentity(Int32.Parse(organizationIdentityString));
    }

    protected Person CurrentUser { get; private set; }
    protected Organization CurrentOrganization { get; private set; }


    protected override void OnPreInit(EventArgs e)
    {
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


    protected string TryLocalize(string input)
    {
        if (!input.StartsWith("[Loc]"))
        {
            return input;
        }

        string[] inputParts = input.Split('|');

        string resourceKey = inputParts[0].Substring(5);
        object translatedResource = GetGlobalResourceObject("Global", resourceKey);

        if (translatedResource == null)
        {
            throw new NotImplementedException("Unimplemented localization resource key: \"" + resourceKey + "\"");
        }

        if (inputParts.Length == 1)
        {
            return translatedResource.ToString();
        }
        else
        {
            object argument = null;

            if (inputParts[1].StartsWith("[Date]"))
            {
                argument = DateTime.Parse(inputParts[1].Substring(6), CultureInfo.InvariantCulture);
            }
            else
            {
                argument = inputParts[1];
            }

            return String.Format(translatedResource.ToString(), argument);
        }
    }


    protected static AuthenticationData GetAuthenticationDataAndCulture()
    {
        return GetAuthenticationDataAndCulture(HttpContext.Current);
    }

    protected static AuthenticationData GetAuthenticationDataAndCulture(HttpContext suppliedContext)
    {
        // This function is called from static page methods in AJAX calls to get
        // the current set of authentication data. Static page methods cannot access
        // the instance data of PageV5Base.

        AuthenticationData result = new AuthenticationData();

        // Find various credentials

        string identity = suppliedContext.User.Identity.Name;
        string[] identityTokens = identity.Split(',');

        string userIdentityString = identityTokens[0];
        string organizationIdentityString = identityTokens[1];

        int currentUserId = Convert.ToInt32(userIdentityString);
        int currentOrganizationId = Convert.ToInt32(organizationIdentityString);

        result.CurrentUser = Person.FromIdentity(currentUserId);
        result.CurrentOrganization = Organization.FromIdentity(currentOrganizationId);

        string userCultureString = result.CurrentUser.PreferredCulture;

        if (!string.IsNullOrEmpty(userCultureString))
        {
            CultureInfo userCulture = new CultureInfo(userCultureString); // may throw on invalid database data
            Thread.CurrentThread.CurrentCulture = userCulture;
            Thread.CurrentThread.CurrentUICulture = userCulture;
        }

        return result;
    }


    protected static string JsonSanitize (string input)
    {
        return input.Replace("\"", "\\\"");
    }

}

