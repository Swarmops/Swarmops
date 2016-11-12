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

using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

using Membership = Activizr.Logic.Pirates.Membership;

public partial class Pages_v4_PopupManageVolunteer : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        string volunteerIdString = Request.QueryString["VolunteerId"];

        if (String.IsNullOrEmpty(volunteerIdString))
        {
            volunteerIdString = "1"; // for debug
        }

        int volunteerId = Int32.Parse(volunteerIdString);
        Volunteer volunteer = Volunteer.FromIdentity(volunteerId);

        int currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

        if (volunteer.OwnerPersonId != currentUserId)
        {
            throw new UnauthorizedAccessException();
        }

        Page.Title = "Managing " + volunteer.Name;
        this.LabelVolunteerPhone.Text = volunteer.Phone;
        this.LabelVolunteerName.Text = volunteer.Name;

        if (volunteer.Person.GetMemberships().Count > 1)
        {
            this.LabelVolunteerName.Text += " (#" + volunteer.PersonId + ")";
            this.LiteralMemberFound.Text = this.LiteralFound.Text ;
        }
        else
        {
            this.LiteralMemberFound.Text = this.LiteralNotFound.Text;
        }

        this.LabelVolunteerGeography.Text = volunteer.Geography.Name + ", " + volunteer.Geography.Parent.Name;

        VolunteerRoles roles = volunteer.Roles;

        foreach (VolunteerRole role in roles)
        {
            string roleDescription = string.Empty;

            switch (role.RoleType)
            {
                case RoleType.LocalLead:
                    roleDescription = "Primary lead of ";
                    break;
                case RoleType.LocalDeputy:
                    roleDescription = "Deputy of ";
                    break;
                case RoleType.LocalAdmin:
                    roleDescription = "Administrator of ";
                    break;
                default:
                    roleDescription = "UNKNOWN ROLE [" + role.RoleType.ToString() + "] of ";
                    break;
            }

            this.ChecksVolunteerRoles.Items.Add(
                new ListItem(roleDescription + role.Geography.Name + " (check if you assign the role)",
                             role.Identity.ToString()));
        }
    }


    protected void ButtonClose_Click (object sender, EventArgs e)
    {
        string volunteerIdString = Request.QueryString["VolunteerId"];
        int volunteerId = Int32.Parse(volunteerIdString);
        Volunteer volunteer = Volunteer.FromIdentity(volunteerId);

        int currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
        Person currentUser = Person.FromIdentity(currentUserId);

        int organizationId = 0;

        Memberships memberships = volunteer.Person.GetMemberships();

        foreach (Membership membership in memberships)
        {
            if (volunteer.Roles.Count > 0 && membership.OrganizationId == volunteer.Roles[0].OrganizationId)
            {
                organizationId = volunteer.Roles[0].OrganizationId; // Assume same org across all roles
            }
        }

        foreach (VolunteerRole role in volunteer.Roles)
        {
            bool assigned = this.ChecksVolunteerRoles.Items.FindByValue(role.Identity.ToString()).Selected;

            role.Close(assigned);

            if (organizationId == role.OrganizationId && assigned)
            {
                // If identified as member, autoassign & log

                volunteer.Person.AddRole(role.RoleType, role.OrganizationId, role.GeographyId);
                PWLog.Write(currentUser, PWLogItem.Person, volunteer.PersonId, PWLogAction.RoleAssigned,
                                "Assigned Role [" + role.RoleType.ToString() + "] for organization [" +
                                Organization.FromIdentity(role.OrganizationId).Name + "] and geography [" +
                                role.Geography.Name + "]", string.Empty);
                Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.AddedRole, currentUserId, Organization.PPSEid,
                                                   role.GeographyId, volunteer.PersonId, 0, string.Empty);
            }
        }

        volunteer.Close(this.TextAssessment.Text);

        // Close window and force refresh

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
    }
}