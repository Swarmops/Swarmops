using System;
using System.Collections.Generic;
using System.Web;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

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
}
