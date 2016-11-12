using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Logic.Communications;
using Activizr.Interface.Localization;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

using Membership = Activizr.Logic.Pirates.Membership;
using Activizr.Interface.Objects;
using Activizr.Logic.Security;

public partial class Pages_Members_SubscriptionSettings : PageV4Base
{
    Person _displayedPerson;

    protected void Page_Load (object sender, EventArgs e)
    {
        this.pagePermissionDefault = new PermissionSet(Permission.CanEditMemberSubscriptions);

        // Get requested person

        _displayedPerson = Person.FromIdentity(Convert.ToInt32("" + Request["id"]));
        Subscriptions1.DisplayedPerson = _displayedPerson;

        // Authorize
        bool allowed = _authority.CanSeePerson(_displayedPerson) ||
            (_authority.HasRoleAtOrganization(Organization.PPSE,RoleType.OrganizationMemberService,Authorization.Flag.AnyGeographyExactOrganization));

        // If they got this far the have been able to see the person.

        if (!allowed)
        {
            ((MasterV4Base)Master).CurrentPageProhibited = true;
        }

    }




}
