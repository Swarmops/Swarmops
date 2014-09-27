using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

/// <summary>
/// Summary description for ControlV5Base
/// </summary>
public class ControlV5Base : System.Web.UI.UserControl
{
    public ControlV5Base()
    {
    }

    public Authority _authority = null;
    public Person _currentUser = null;
    public Organization _currentOrganization = null;

    protected override void OnLoad(EventArgs e)
    {
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

        base.OnLoad(e);
    }

    protected string GetBuildIdentity()
    {
        // Read build number if not loaded, or set to "Private" if none
        string buildIdentity = (string) GuidCache.Get("_buildIdentity");

        if (buildIdentity == null)
        {
            try
            {
                using (StreamReader reader = File.OpenText(HttpContext.Current.Request.MapPath("~/BuildIdentity.txt")))
                {
                    buildIdentity = "Build " + reader.ReadLine();
                }

                using (StreamReader reader = File.OpenText(HttpContext.Current.Request.MapPath("~/SprintName.txt")))
                {
                    buildIdentity += " (" + reader.ReadLine() + ")";
                }
            }
            catch (Exception)
            {
                buildIdentity = "Private Build";
            }

            GuidCache.Set("_buildIdentity", buildIdentity);
        }

        return buildIdentity;
    }
}
