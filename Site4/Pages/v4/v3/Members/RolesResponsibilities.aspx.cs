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
using System.Collections.Generic;

public partial class Pages_Members_RolesResponsibilities : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        this.pagePermissionDefault = new PermissionSet(Permission.CanSeePeople);

        // Get requested person

        Person displayedPerson = Person.FromIdentity(Convert.ToInt32(""+Request["id"]));

        // Authorize



        bool allowed = _authority.CanSeePerson(displayedPerson) ||
            (_authority.HasRoleAtOrganization(Organization.PPSE, RoleType.OrganizationMemberService, Authorization.Flag.AnyGeographyExactOrganization));
        // If they got this far the have been able to see the person.

        if (!allowed)
        {
            ((MasterV4Base)Master).CurrentPageProhibited = true;
        }

        // Add very simple confirmation to the AddRole button
        string confirmText = LocalizationManager.GetLocalString("Interface.Pages.Member.RolesResponsibilities.ConfirmAddRole",
                                               "Are you sure you've chosen the correct role for this user?");
        this.ButtonAddRoleLocal.OnClientClick = "return confirm('" + confirmText.Replace("'", "\\'") + "');";
        this.ButtonAddOrgRole.OnClientClick = "return confirm('" + confirmText.Replace("'", "\\'") + "');";

        if (!_authority.HasAnyPermission(Permission.CanEditOrganisationalRoles))
            this.ButtonAddOrgRole.Enabled = false;

        if (!_authority.HasAnyPermission(Permission.CanEditLocalRoles))
            this.ButtonAddRoleLocal.Enabled = false;

        // If not in viewstate, initialize page

        if (!this.IsPostBack)
        {
            // Fix links

            // Localize

            this.labelCurrentMember.Text = displayedPerson.Name + " (#" + displayedPerson.Identity.ToString() + ")";

            // Populate list of organizations (initial population)

            Organizations organizationList = _authority.GetOrganizations(RoleTypes.AllRoleTypes);
            organizationList = organizationList.RemoveRedundant();
            organizationList.Add(Organization.Sandbox);

            Memberships displayedMemberOf = displayedPerson.GetMemberships();
            if (displayedMemberOf.Count < 1)
            {
                this.labelCurrentMember.Text += " NOTE! THIS PERSON IS NOT A MEMBER AND CAN NOT HAVE ROLES.";
            }
            else
            {
                foreach (Organization organization in organizationList)
                {
                    Organizations organizationTree = organization.GetTree();

                    foreach (Organization organizationOption in organizationTree)
                    {
                        bool canhave =  CheckIsMemberOf(displayedMemberOf, organizationOption);

                        if (canhave)
                        {
                            if (_authority.HasPermission(Permission.CanEditLocalRoles, organizationOption.OrganizationId, -1, Authorization.Flag.AnyGeography))
                            {
                                DropOrganizationsLocal.Items.Add(new ListItem(organizationOption.NameShort,
                                                                             organizationOption.OrganizationId.ToString()));
                            }
                            if (_authority.HasPermission(Permission.CanEditOrganisationalRoles, organizationOption.OrganizationId, -1, Authorization.Flag.Default))
                            {
                                DropOrganizationsOrg.Items.Add(new ListItem(organizationOption.NameShort,
                                                                             organizationOption.OrganizationId.ToString()));
                            }
                        }
                    }
                }
            }
            if (DropOrganizationsOrg.Items.Count == 0)
                AddOrgRolePanel.Visible = false;
            else
                AddOrgRolePanel.Visible = true;


            // Populate nodes

            PopulateGeographies();


            // Populate roles


            this.DropRolesLocal.Items.Add(new ListItem("Lead (primary PoC)", RoleType.LocalLead.ToString()));
            this.DropRolesLocal.Items.Add(new ListItem("Second-in-Command", "LocalDeputy"));
            this.DropRolesLocal.Items.Add(new ListItem("Active (visible)", "LocalActive"));
            this.DropRolesLocal.Items.Add(new ListItem("Admin (invisible)", "LocalAdmin"));

            foreach (RoleType r in RoleTypes.AllOrganizationalRoleTypes)
            {
                DropRolesOrg.Items.Add(new ListItem(r.ToString(), r.ToString()));
            }
        }

        PersonDetailsPagesMenu.CurrentPage = 2;
    }


    private static bool CheckIsMemberOf(Memberships displayedMemberOf, Organization organizationOption)
    {
        bool canhave = false;
        foreach (Activizr.Logic.Pirates.Membership m in displayedMemberOf)
        {
            if (organizationOption.Inherits(m.OrganizationId) || organizationOption.Identity == m.OrganizationId)
            {
                canhave = true;
                break;
            }
        }
        return canhave;
    }

    protected void PopulateGeographies ()
    {
        if (DropOrganizationsLocal.SelectedIndex >= 0)
        {

            Organization org = Organization.FromIdentity(Convert.ToInt32(DropOrganizationsLocal.SelectedValue));

            Geographies geoList = _authority.GetGeographiesForOrganization(org, RoleTypes.AllRoleTypes);

            geoList = geoList.RemoveRedundant();
            geoList = geoList.FilterAbove(Geography.FromIdentity(org.AnchorGeographyId));

            if (org.Identity == Organization.SandboxIdentity)
            {
                geoList = Geographies.FromSingle(Geography.Root);
            }

            this.DropGeographies.Items.Clear();

            foreach (Geography nodeRoot in geoList)
            {
                Geographies nodeTree = nodeRoot.GetTree();

                foreach (Geography node in nodeTree)
                {
                    string nodeLabel = node.Name;

                    for (int loop = 0; loop < node.Generation; loop++)
                    {
                        nodeLabel = "|-- " + nodeLabel;
                    }
                    if (_authority.HasPermission(Permission.CanEditLocalRoles,org.Identity,node.Identity,Authorization.Flag.Default))
                    DropGeographies.Items.Add(new ListItem(nodeLabel, node.GeographyId.ToString()));
                }
            }

            if (DropGeographies.Items.Count == 0)
                AddLocalRolePanel.Visible = false;
            else
                AddLocalRolePanel.Visible = true;

        }
    }


    protected void ButtonAddRole_Click (object sender, EventArgs e)
    {
        int personId = (Person.FromIdentity(Convert.ToInt32(""+Request["id"]))).Identity;
        int currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

        RoleType roleType = (RoleType)Enum.Parse(typeof(RoleType), DropRolesLocal.SelectedValue);
        int nodeId = Convert.ToInt32(DropGeographies.SelectedValue);
        int organizationId = Convert.ToInt32(DropOrganizationsLocal.SelectedValue);
        if (!_authority.HasPermission(Permission.CanEditLocalRoles, organizationId, nodeId,Authorization.Flag.Default ))
        {
            Page.ClientScript.RegisterStartupScript(typeof(Page), "ErrorMessage",
                            "alert ('You do not have permissions to add local roles for that combination of organisation and geography.');",
                            true);
            return;
        }

        /* Fix for Ticket #46 */

        // Check if role being added is lead, if so check if lead is already filled, if so return error, else continue
        if (roleType == RoleType.LocalLead)
        {
            List<Person> aggregate = new List<Person>();
            RoleLookup lead = RoleLookup.FromGeographyAndOrganization(nodeId, organizationId);
            foreach (PersonRole role in lead[RoleType.LocalLead])
            {
                aggregate.Add(role.Person);
            }

            // If we have one or more people in this role already, throw error (typically it should only be filled by one other person)
            if (aggregate.Count > 0)
            {
                string name = aggregate[0].Name;

                // Handle multiple users with this role
                if (aggregate.Count > 1)
                {
                    name = String.Empty;
                    int count = 0;
                    foreach (Person person in aggregate)
                    {
                        count++;
                        name += person.Name;
                        if (count < aggregate.Count) { name += ", "; }
                    }
                }

                Page.ClientScript.RegisterStartupScript(typeof(Page), "ErrorMessage",
                                            "alert ('This role is already filled by " + name + ". To assign a new user to this role please remove the old user first.');",
                                            true);

                return;
            }
        }

        int roleId = PersonRole.Create(personId, roleType, organizationId, nodeId).Identity;

        // If we made it this far, add the role
        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.AddedRole, currentUserId, organizationId,
                                           nodeId, personId, (int)roleType, string.Empty);

        this.GridLocalRoles.DataBind();
    }


    protected void DropOrganizationsNode_SelectedIndexChanged (object sender, EventArgs e)
    {
        PopulateGeographies();
    }


    protected void gridNodeRoles_RowCommand (object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "DeleteManual")
        {
            return; // We only handle Delete.
        }

        int roleId = Convert.ToInt32(e.CommandArgument);
        PersonRole personRole = PersonRole.FromIdentity(roleId);

        int viewingPersonId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

        // Re-authorize

        if (_authority.HasPermission(Permission.CanEditLocalRoles, personRole.OrganizationId, personRole.GeographyId, Authorization.Flag.Default))
        {
            personRole.Delete();
            Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.DeletedRole, viewingPersonId,
                                               personRole.OrganizationId, personRole.GeographyId, personRole.PersonId, (int)personRole.Type,
                                               string.Empty);
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(typeof(Page), "UnauthorizedMessage",
                                                    "alert ('There was an authorization error trying to delete the role. If you believe this is incorrect, contact an administrator.');",
                                                    true);
        }

        this.GridLocalRoles.DataBind();
    }
    protected void ButtonAddOrgRole_Click (object sender, EventArgs e)
    {
        int personId = (Person.FromIdentity(Convert.ToInt32(""+Request["id"]))).Identity;
        int currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

        RoleType roleType = (RoleType)Enum.Parse(typeof(RoleType), DropRolesOrg.SelectedValue);
        int nodeId = Geography.RootIdentity;
        int organizationId = Convert.ToInt32(DropOrganizationsOrg.SelectedValue);

        if (!_authority.HasPermission(Permission.CanEditOrganisationalRoles, organizationId, nodeId, Authorization.Flag.Default))
        {
            Page.ClientScript.RegisterStartupScript(typeof(Page), "ErrorMessage",
                            "alert ('You do not have permissions to add organisation roles for that organisation.');",
                            true);
            return;
        }

        int roleId = PersonRole.Create(personId, roleType, organizationId, nodeId).Identity;

        // If we made it this far, add the role
        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.AddedRole, currentUserId, organizationId,
                                           nodeId, personId, (int)roleType, string.Empty);

        this.GridOrgRoles.DataBind();
    }
    protected void GridOrgRoles_RowCommand (object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "DeleteManual")
        {
            return; // We only handle Delete.
        }

        int roleId = Convert.ToInt32(e.CommandArgument);
        PersonRole personRole = PersonRole.FromIdentity(roleId);

        int viewingPersonId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

        // Re-authorize

        if (_authority.HasPermission(Permission.CanEditOrganisationalRoles,personRole.OrganizationId,-1,Authorization.Flag.Default ))
        {
            personRole.Delete();
            Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.DeletedRole, viewingPersonId,
                                               personRole.OrganizationId, personRole.GeographyId, personRole.PersonId, (int)personRole.Type,
                                               string.Empty);
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(typeof(Page), "UnauthorizedMessage",
                                                    "alert ('There was an authorization error trying to delete the role. If you believe this is incorrect, contact an administrator.');",
                                                    true);
        }

        this.GridOrgRoles.DataBind();

    }
    protected void DropOrganizationsOrg_SelectedIndexChanged (object sender, EventArgs e)
    {

    }
}