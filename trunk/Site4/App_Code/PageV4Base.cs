using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Activizr.Logic.Security;
using Activizr.Logic.Pirates;
using Activizr.Basic.Enums;

/// <summary>
/// Summary description for PageV4Base
/// Base class to use for all pages that uses the PirateWeb-v4 master page
/// </summary>

public class PageV4Base : System.Web.UI.Page
{
    public PermissionSet pagePermissionDefault = new PermissionSet(Permission.CanSeeSelf); //Use from menu;

    public Person _currentUser = null;
    public Authority _authority = null;

    /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
    protected override void OnInitComplete (System.EventArgs e)
    {
        base.OnInitComplete(e);

        int currentUserId = 0;
        currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
        _currentUser = Person.FromIdentity(currentUserId);
        _authority = _currentUser.GetAuthority();
    }

    public virtual void AsyncPostBackError(ScriptManager scriptManager, object sender, AsyncPostBackErrorEventArgs e)
    {
        scriptManager.AsyncPostBackErrorMessage = "There was an error while doing a partial page update:\r\n" + e.Exception.ToString();
    }

    protected new MasterV4Base Master
    { get { return (MasterV4Base)base.Master; } }
}


