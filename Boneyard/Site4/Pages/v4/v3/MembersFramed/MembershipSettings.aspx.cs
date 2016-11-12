using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using Activizr.Logic.Structure;

public partial class Pages_Members_MembershipSettings : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        this.pagePermissionDefault = new PermissionSet(Permission.CanEditMemberships);

        // Get requested person

        Person displayedPerson = Person.FromIdentity(Convert.ToInt32(""+Request["id"]));

        // Authorize
        bool allowed = _authority.CanSeePerson(displayedPerson) ||
            (_authority.HasRoleAtOrganization(Organization.PPSE, RoleType.OrganizationMemberService,
                             Authorization.Flag.AnyGeographyExactOrganization));
        // If they got this far the have been able to see the person.

        if (!allowed)
        {
            ((MasterV4Base)Master).CurrentPageProhibited = true;
        }

        this.ControlSetPassword.Person = displayedPerson;
        this.ControlSetPassword.DisplayRandomize = true;

        // If not in viewstate, initialize page

        if (!this.IsPostBack)
        {
        }

    }
}