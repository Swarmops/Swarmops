using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Activizr.Logic.Pirates;
using Activizr.Logic.Support;

public partial class Pages_v4_Admin_Impersonate : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        Label1.Text = "You need to be SystemAdmin to Impersonate";
        if (_authority.HasRoleType(Activizr.Basic.Enums.RoleType.SystemAdmin))
        {
            if (Session["ImpersonationInProgress"] == null)
            {
                Panel1.Visible = true;
            }
        }
        else
        {
            Panel1.Visible = false;
        }

        if (Session["ImpersonationInProgress"] != null)
        {
            int persId = Int32.Parse("" + Session["ImpersonationInProgress"]);
            Person p = Person.FromIdentity(persId);

            Label1.Text = "Logged in as #" + p.Identity + " " + p.Name;
            Panel2.Visible = true;
        }
        else
        {
            Panel2.Visible = false;
        }
    }
    
    protected void Button1_Click (object sender, EventArgs e)
    {
        if (_authority.HasRoleType(Activizr.Basic.Enums.RoleType.SystemAdmin))
        {
            int persId = Int32.Parse("" + TextBoxPersId.Text);
            int currentUserId = _currentUser.Identity;
            Person p = Person.FromIdentity(persId);
            FormsAuthentication.SetAuthCookie("" + persId, false);
            PWLog.Write(currentUserId, PWLogItem.Person, persId, PWLogAction.StartImpersonation, "Impersonating user #" + p.Identity + " " + p.Name, "");
            Session["ImpersonationInProgress"] = currentUserId;
            Label1.Text = "Logged in as #" + p.Identity + " " + p.Name;
            Panel1.Visible = false;
        }

    }
    
    protected void Button2_Click (object sender, EventArgs e)
    {

            int persId = Int32.Parse("" + "" + Session["ImpersonationInProgress"]);
            Person p = Person.FromIdentity(persId);
            FormsAuthentication.SetAuthCookie("" + persId, false);           
            Session["ImpersonationInProgress"] = null;
            Session.Contents.Remove("ImpersonationInProgress");

        Panel2.Visible = false;
    }
}
