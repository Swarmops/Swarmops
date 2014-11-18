using System;
using System.Globalization;
using System.Web;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

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

        if (!string.IsNullOrEmpty(identity))
        {
            string[] identityTokens = identity.Split(',');

            string userIdentityString = identityTokens[0];
            string organizationIdentityString = identityTokens[1];

            this.CurrentUser = Person.FromIdentity(Int32.Parse(userIdentityString));
            this.CurrentOrganization = Organization.FromIdentity(Int32.Parse(organizationIdentityString));
        }
        else
        {
            this.CurrentUser = null; // unauthenticated!
            this.CurrentOrganization = null; // unauthenticated!
        }
    }

    protected Person CurrentUser { get; private set; }
    protected Organization CurrentOrganization { get; private set; }


    protected override void OnPreInit(EventArgs e)
    {
        CommonV5.CulturePreInit(Request);

 	    base.OnPreInit(e);
    }

    protected override void  OnPreRender(EventArgs e)
    {
        // Check security of page against users's credentials

        if (this.PageAccessRequired != null)
        {
            if (!CurrentUser.HasAccess(this.PageAccessRequired))
            {
                Response.Redirect("/Pages/v5/Security/AccessDenied.aspx");
            }
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
        return CommonV5.GetAuthenticationDataAndCulture(HttpContext.Current);
    }

    protected static string JsonSanitize (string input)
    {
        return input.Replace("\"", "\\\"").Replace("  ", " ").Trim();
    }

}

