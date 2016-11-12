using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Services;
using CellAddressable;
using Activizr.Basic.Enums;
using Activizr.Logic.Security;
using Activizr.Logic.Pirates;
using Activizr.Basic.Types;

public partial class Admin_EditPermsGrid : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        ((MasterV4Base)this.Master).CurrentPageAllowed = true;

        AdminPermsMainGridTable t = new AdminPermsMainGridTable();

        t.LoadTable(MainTab);

        string innerContent = t[0, t.firstcol].Cell.InnerHtml;



        foreach (RoleType role in Enum.GetValues(typeof(RoleType)))
        {
                t.AddRole(role);
        }

        t[0, t.firstcol].Cell.InnerHtml = innerContent;

        foreach (Permission perm in Enum.GetValues(typeof(Permission)))
        {
            if (perm != Permission.Undefined)
                t.AddPermission(perm);
        }
        Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
        Authority authority = viewingPerson.GetAuthority();
        PermissionSet EditPerms = new PermissionSet(Permission.CanEditPermissions);

        bool hasPermission = authority.HasPermission(EditPerms,Authorization.Flag.Default );
        hasPermission |= authority.HasRoleType(RoleType.SystemAdmin);

        HttpContext.Current.Session["AllowedToEditPermissions"] = hasPermission;

        BasicPermission[] loadedPermissions = Activizr.Database.PirateDb.GetDatabase().GetPermissionsTable();
        foreach (RoleType role in Enum.GetValues(typeof(RoleType)))
        {
                foreach (Permission perm in Enum.GetValues(typeof(Permission)))
                {
                    if (perm != Permission.Undefined)
                        t.AddResult(role, perm, false, hasPermission);
                }
        }

        foreach (BasicPermission bp in loadedPermissions)
        {

            t.AddResult(bp.RoleType, bp.PermissionType, true, hasPermission);
        }

        for (int c = t.firstcol + 1; c < t.Columns.Count; ++c)
        {
            if (t.Columns[c - 1][1].Cell.InnerText.Trim() == t.Columns[c][1].Cell.InnerText.Trim())
                t.Columns[c - 1][1].JoinCell(CellJoinDirection.RIGHT);

            t.Columns[c - 1][0].JoinCell(CellJoinDirection.RIGHT);
        }


        t.GetHTMLTable(ref MainTab, true);
    }

    [WebMethod(EnableSession = true)]
    public static string SaveAccessClick (int pRole, int pPerm, bool pAccess)
    {
        try
        {

            bool hasPermission = HttpContext.Current.Session["AllowedToEditPermissions"] != null ? (bool)HttpContext.Current.Session["AllowedToEditPermissions"] : false;
            if (hasPermission)
            {
                Activizr.Database.PirateDb.GetDatabase().StoreOnePermission(pRole, pPerm, pAccess);
                Authorization.flagReload = true;
                Authorization.InitializeStaticData();
            }
            else
                return "Error:No Permission";
        }
        catch (Exception e)
        {
            return "Error:" + e.Message;
        }
        return "OK";
    }

   
}
