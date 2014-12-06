using System;
using System.IO;
using System.Web;
using System.Web.UI;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

/// <summary>
///     Summary description for ControlV5Base
/// </summary>
public class ControlV5Base : UserControl
{
    private Authority _authority;
    private Organization _currentOrganization;
    private Person _currentUser;

    protected Organization CurrentOrganization
    {
        get { return this._currentOrganization; }
    }

    protected Person CurrentUser
    {
        get { return this._currentUser; }
    }

    protected Authority CurrentAuthority
    {
        get { return this._authority; }
    }

    protected override void OnLoad (EventArgs e)
    {
        int currentUserId = 0;
        int currentOrganizationId = 0;

        string identity = HttpContext.Current.User.Identity.Name;
        string[] identityTokens = identity.Split (',');

        string userIdentityString = identityTokens[0];
        string organizationIdentityString = identityTokens[1];

        currentUserId = Convert.ToInt32 (userIdentityString);
        currentOrganizationId = Convert.ToInt32 (organizationIdentityString);
        this._currentUser = Person.FromIdentity (currentUserId);
        this._authority = this._currentUser.GetAuthority();
        try
        {
            this._currentOrganization = Organization.FromIdentity (currentOrganizationId);
        }
        catch (ArgumentException)
        {
            if (PilotInstallationIds.IsPilot (PilotInstallationIds.DevelopmentSandbox))
            {
                // It's possible this organization was deleted. Log on to Sandbox instead.
                this._currentOrganization = Organization.Sandbox;
            }
        }

        base.OnLoad (e);
    }

}