using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Basic.Types;
using Activizr.Basic.Enums;
using Activizr.Interface.Localization;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;


public partial class Pages_Members_BasicDetails : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        this.pagePermissionDefault = new PermissionSet(Permission.CanSeePeople);

        // Get requested person

        Person displayedPerson = Person.FromIdentity(Convert.ToInt32(""+Request["id"]));

        bool allowed = _authority.CanSeePerson(displayedPerson) ||
            (_authority.HasRoleAtOrganization(Organization.PPSE, RoleType.OrganizationMemberService,
                             Authorization.Flag.AnyGeographyExactOrganization));

        // If they got this far the have been able to see the person.

        if (!allowed)
        {
            ((MasterV4Base)Master).CurrentPageProhibited = true;
        }

        this.PersonDetails.Person = displayedPerson;
        this.PersonDetails.DataChanged += new EventHandler(PersonDetails_DataChanged);


        this.GeographyLine.Geography = displayedPerson.Geography;


         }

    private void Pages_Members_BasicDetails_LanguageChanged (object sender, EventArgs e)
    {
    }

    private void PersonDetails_DataChanged (object sender, EventArgs e)
    {
        // Reload person to force new geography

        Person person = Person.FromIdentity(this.PersonDetails.Person.Identity);
        HttpContext.Current.Session["DisplayedPerson"] = person;

        this.GeographyLine.Geography = person.Geography;
    }
}