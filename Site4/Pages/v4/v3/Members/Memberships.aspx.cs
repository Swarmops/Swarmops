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
using Activizr.Logic.Support;

using Membership = Activizr.Logic.Pirates.Membership;

public partial class Pages_Members_Memberships : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        // Get requested person

        Person displayedPerson = Person.FromIdentity(Convert.ToInt32("" + Request["id"]));

        // Authorize

        this.pagePermissionDefault = new PermissionSet(Permission.CanViewMemberships);

        bool allowed = _authority.CanSeePerson(displayedPerson) ||
            (_authority.HasRoleAtOrganization(Organization.PPSE, RoleType.OrganizationMemberService, Authorization.Flag.AnyGeographyExactOrganization));

        // If they got this far the have been able to see the person.
        // Should maybe check directly on Memberservice PersonRole instead?

        if (!allowed)
        {
            ((MasterV4Base)Master).CurrentPageProhibited = true;
        }

        // If not in viewstate, initialize page

        if (!this.IsPostBack)
        {
            // Localize

            this.labelMembershipsHeader.Text =
                LocalizationManager.GetLocalString("Interface.Pages.Member.Memberships.Header",
                                                   "Viewing/Editing Member - Memberships");
            this.labelCurrentMember.Text = displayedPerson.Name + " (#" + displayedPerson.Identity.ToString() + ")";

            // Set list of memberships

            ReadMembershipList();

            // Populate list of organizations (initial population)

            PopulateOrganizations();
        }
        if (!_authority.HasAnyPermission(Permission.CanEditMemberships))
            this.ButtonAddMembership.Enabled = false;

        this.PersonDetailsPagesMenu.CurrentPage = 1;
    }

    private void PopulateOrganizations ()
    {
        dropOrganizations.Items.Clear();

        Person person = Person.FromIdentity(Convert.ToInt32("" + Request["id"]));

        Memberships memberships = person.GetMemberships();

        // Generate combined list of orgs: available at this geography vs. available through this authority


        //Organizations organizations = Organizations.GetAllOrganizationsAvailableAtGeography(person.GeographyId);
        Organizations organizations = Organizations.GetAll();
        Organizations authorizedOrgs = _authority.GetOrganizations(RoleTypes.AllRoleTypes).ExpandAll();

        organizations = organizations.LogicalAnd(authorizedOrgs);


        // Which orgs is the person already a member of?

        foreach (BasicOrganization organization in organizations)
        {
            if (organization.AcceptsMembers)
            {
                bool isMemberAlready = false;

                foreach (Membership membership in memberships)
                {
                    if (membership.OrganizationId == organization.OrganizationId)
                    {
                        isMemberAlready = true;
                    }
                }

                if (!isMemberAlready)
                {
                    this.dropOrganizations.Items.Add(new ListItem(organization.Name,
                                                                  organization.OrganizationId.ToString()));
                }
            }
        }
    }

    protected void GridMemberships_RowCommand (object sender, GridViewCommandEventArgs e)
    {
        // TODO: Re-authorize?

        int index = Convert.ToInt32(e.CommandArgument);
        int viewingPersonId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
        int membershipId = Convert.ToInt32(this.gridMemberships.DataKeys[index].Value);

        Membership membership = Membership.FromIdentity(membershipId);
        BasicPerson person = Person.FromIdentity(membership.PersonId);

        switch (e.CommandName)
        {
            case "Extend":
                membership.Expires = membership.Expires.AddYears(1);
                PWLog.Write(Person.FromIdentity(viewingPersonId), PWLogItem.Person, person.Identity,
                                PWLogAction.MembershipRenewed,
                                "Membership in " + Organization.FromIdentity(membership.OrganizationId).NameShort +
                                " extended manually.",
                                "Membership was extended by " + Person.FromIdentity(viewingPersonId).Name  + " (#" +
                                viewingPersonId.ToString() + ") to last until " +
                                membership.Expires.AddYears(1).ToString("yyyy-MMM-dd") + ".");
                Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ExtendedMembership, viewingPersonId,
                                                   membership.OrganizationId, person.GeographyId, membership.PersonId,
                                                   membershipId, string.Empty);
                ReadMembershipList();
                break;
            case "Terminate":
                Membership.FromIdentity(membershipId).Terminate(EventSource.PirateWeb, Person.FromIdentity(viewingPersonId), "Membership in " + Organization.FromIdentity(membership.OrganizationId).NameShort + " removed manually.");
                ReadMembershipList();
                PopulateOrganizations();
                CheckStillVisible();
                break;
            default:
                throw new InvalidOperationException("Not supported: Command \"" + e.CommandName + "\"");
        }

        gridMemberships.DataBind();
    }


    private void CheckStillVisible ()
    {
        // This function duplicates the check done in Page_Load, after a membership has been removed.
        // If the displayed person is no longer visible, then the viewer is redirected to Member Search.

        Person displayedPerson = Person.FromIdentity(Convert.ToInt32("" + Request["id"]));

        Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
        Authority authority = viewingPerson.GetAuthority();

        if (!authority.CanSeePerson(displayedPerson))
        {
            Page.ClientScript.RegisterStartupScript(typeof(Page), "RedirectMessage",
                                                    "alert ('After your removal of this membership, this person no longer has any visible memberships. This person is no longer visible. Therefore, you are being redirected to the Member Search page.'); document.location.href='Search.aspx';",
                                                    true);
        }
    }


    private void ReadMembershipList ()
    {
        Person displayedPerson = Person.FromIdentity(Convert.ToInt32("" + Request["id"]));
        Memberships memberships = displayedPerson.GetMemberships();


        memberships = memberships.RemoveToMatchAuthority(displayedPerson.Geography, _authority);

        HttpContext.Current.Session["MembershipList"] = memberships;
    }


    protected void ButtonAddMembership_Click (object sender, EventArgs e)
    {
        // TODO: Re-authorize?
        int organizationId = Convert.ToInt32(dropOrganizations.SelectedValue);
        Person person = Person.FromIdentity(Convert.ToInt32("" + Request["id"]));
        if (_authority.HasPermission(Permission.CanEditMemberships, organizationId, -1, Authorization.Flag.AnyGeography))
        {
            int viewingPersonId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
            DateTime paidUntil = DateTime.Now.AddYears(1);

            PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MembershipAdd,
                            "Membership in " + Organization.FromIdentity(organizationId).NameShort + " added manually.",
                            "Membership was added by " + Person.FromIdentity(viewingPersonId) + " (#" +
                            viewingPersonId.ToString() + ") to last until " + paidUntil.ToString("yyyy-MMM-dd") + ".");

            Membership.Create(person.PersonId, organizationId, paidUntil);
            Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.AddedMembership, viewingPersonId,
                                               organizationId, person.GeographyId, person.PersonId, 0, string.Empty);

            ReadMembershipList();
            gridMemberships.DataBind();
            PopulateOrganizations();
        }
    }


    protected void MembershipsDataSource_Selecting (object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["memberships"] = (Memberships)HttpContext.Current.Session["MembershipList"];
    }
}