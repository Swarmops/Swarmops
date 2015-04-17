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

    protected Organization CurrentOrganization
    {
        get { return _authority.Organization; }
    }

    protected Person CurrentUser
    {
        get { return _authority.Person; }
    }

    protected Authority CurrentAuthority
    {
        get { return this._authority; }
    }

    protected override void OnInit (EventArgs e)
    {
        this._authority = CommonV5.GetAuthenticationDataAndCulture (HttpContext.Current).Authority;

        base.OnInit (e);
    }

    protected string JavascriptEscape (string input)
    {
        return CommonV5.JavascriptEscape (input);
    }

}