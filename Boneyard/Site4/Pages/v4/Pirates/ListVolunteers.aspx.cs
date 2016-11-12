using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

using Telerik.Web.UI;

using Roles = Activizr.Logic.Pirates.Roles;

public partial class Pages_v4_ListVolunteers : PageV4Base
{

    protected void Page_Load (object sender, EventArgs e)
    {
        int currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

        _currentUser = Person.FromIdentity(currentUserId);
        _authority = _currentUser.GetAuthority();

        if (!Page.IsPostBack)
        {
            Person districtLead = GetDistrictLead(_currentUser.Geography, Geography.SwedenId);
            if (districtLead != null)
                this.DropNewOwner.Items.Add(new ListItem(districtLead.Name + GetRoles(districtLead.Identity) + " (om feldistribuerad.)", districtLead.Identity.ToString()));

            People leads = this.GetDirectReports();

            foreach (Person lead in leads)
            {
                this.DropNewOwner.Items.Add(new ListItem(lead.Name + GetRoles(lead.Identity), lead.Identity.ToString()));
            }

            if (leads.Count > 0)
            {
                ButtonMarkAsDone.Visible = true;
            }
            else
            {
                //Lowest level cant remove volunteers in bulk.
                ButtonMarkAsDone.Visible = false;
            }
            People vicesAndAdmins = this.GetVicesAndAdmins();

            foreach (Person viceOrAdmin in vicesAndAdmins)
            {
                this.DropNewOwner.Items.Add(new ListItem(viceOrAdmin.Name + GetRoles(viceOrAdmin.Identity), viceOrAdmin.Identity.ToString()));
            }
            PopulateGrids(true);
            this.GridOther.Rebind();

        }

        if (currentUserId == 1) // Only show button to Rick
        {
            this.ButtonAutoAssign.Visible = true;
            this.ButtonAssignToDistrictLeads.Visible = true;
        }
    }

    /* Fix for Ticket #64 - showing role and geography of assignable leaders */
    private string GetRoles (int personId)
    {
        string roles = String.Empty;
        Authority authority = Authorization.GetPersonAuthority(personId);
        Dictionary<RoleType, List<string>> rolesDict = new Dictionary<RoleType, List<string>>();
        rolesDict.Add(RoleType.LocalLead, new List<string>());
        rolesDict.Add(RoleType.LocalDeputy, new List<string>());
        rolesDict.Add(RoleType.LocalAdmin, new List<string>());


        foreach (Activizr.Basic.Types.BasicPersonRole basicRole in authority.LocalPersonRoles)
        {
            PersonRole personRole = PersonRole.FromBasic(basicRole);
            if (personRole.Type == RoleType.LocalLead
                || personRole.Type == RoleType.LocalDeputy
                || personRole.Type == RoleType.LocalAdmin)
            {
                rolesDict[personRole.Type].Add(personRole.Geography.Name);
            }
        }

        foreach (RoleType r in rolesDict.Keys)
        {
            if (rolesDict[r].Count > 0)
            {
                string innerroles = r.ToString().Replace("Local", "") + ":";
                foreach (string town in rolesDict[r])
                {
                    innerroles += town.Replace("Distriktet", "distr.").Replace("valkrets", "valkr.").Replace("kommun", "k.") + ", ";
                }
                roles += innerroles.Substring(0, innerroles.Length - 2) + " ";
            }
        }

        if (!string.IsNullOrEmpty(roles))
        {
            roles = " (" + roles + ")";
        }

        return roles;
    }


    protected void PopulateGrids (bool rebuildOthersGrid)
    {
        Volunteers allVolunteers = Volunteers.GetOpen();
        int currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

        Person viewingPerson = Person.FromIdentity(currentUserId);
        Authority authority = viewingPerson.GetAuthority();

        Volunteers geoVolunteers = new Volunteers();
        Volunteers directReportVolunteers = new Volunteers();
        Volunteers vicesAndAdminsVolunteers = new Volunteers();
        Volunteers ownerVolunteers = new Volunteers();

        Geographies authGeos = this.GetAuthorityArea();
        People directReports = GetDirectReports();
        People vicesAndAdmins = this.GetVicesAndAdmins();

        // Build lookup tables

        Dictionary<int, bool> geoLookup = new Dictionary<int, bool>();
        Dictionary<int, bool> directReportLookup = new Dictionary<int, bool>();
        Dictionary<int, bool> vicesAndAdminsLookup = new Dictionary<int, bool>();
        Dictionary<int, bool> leadsVicesAndAdminsLookup = new Dictionary<int, bool>();

        foreach (Geography geo in authGeos)
        {
            geoLookup[geo.Identity] = true;
        }

        foreach (Person person in directReports)
        {
            directReportLookup[person.Identity] = true;
        }

        foreach (Person person in vicesAndAdmins)
        {
            vicesAndAdminsLookup[person.Identity] = true;
        }

        // Build volunteer lists
        Dictionary<int, Organization> viewerOrgDict = new Dictionary<int, Organization>();

        Organizations viewerOrgsList = authority.GetOrganizations(RoleTypes.AllLocalRoleTypes );
        foreach (Organization o in viewerOrgsList)
            viewerOrgDict[o.Identity]= o;

        //Volunteers filteredVolunteers = new Volunteers();

        //foreach (Volunteer volunteer in allVolunteers)
        //{
        //    try
        //    {
        //        Person p = volunteer.Person;
        //        filteredVolunteers.Add(volunteer);
        //    }
        //    catch
        //    {
        //    }
        //}

        //allVolunteers = filteredVolunteers;


        foreach (Volunteer volunteer in allVolunteers)
        {
            try
            {
                bool hasAuthority = false;
                // Only show geoVolunteers that volounteered for an org where viewer have a role.
                // (This is currently always PP)
                // Unless it is directly assigned.

                foreach (VolunteerRole rle in volunteer.Roles)
                {
                    if (viewerOrgDict.ContainsKey(rle.OrganizationId))
                    {
                        hasAuthority = true;
                        break;
                    }
                }

                if (hasAuthority && geoLookup.ContainsKey(volunteer.Geography.Identity))
                {

                    geoVolunteers.Add(volunteer);

                    /* Fix for the viewing users volunteers showing up in more than one list */
                    // BROKEN FIX -- this caused only one volunteer to be displayed, EVER, and delayed
                    // volunteer processing by several days -RF
                    //break;
                }

                // If directly assigned to me show regardless of org.
                if (volunteer.OwnerPersonId == currentUserId)
                {
                    ownerVolunteers.Add(volunteer);
                }

                if (hasAuthority && directReportLookup.ContainsKey(volunteer.OwnerPersonId))
                {
                    directReportVolunteers.Add(volunteer);
                }

                if (hasAuthority && vicesAndAdminsLookup.ContainsKey(volunteer.OwnerPersonId))
                {
                    vicesAndAdminsVolunteers.Add(volunteer);
                }
            }
            finally
            {
                // DEBUG USE ONLY - INTENTIONALLY NO EFFECT
            }
        }

        if (rebuildOthersGrid)
        {
            geoVolunteers.Remove(ownerVolunteers);
            geoVolunteers.Remove(directReportVolunteers);
            geoVolunteers.Remove(vicesAndAdminsVolunteers);
            this.GridOther.DataSource = geoVolunteers;
        }
        this.GridLeadGeography.DataSource = vicesAndAdminsVolunteers;
        this.GridReports.DataSource = directReportVolunteers;
        this.GridOwner.DataSource = ownerVolunteers;
    }


    protected Geographies GetAuthorityArea ()
    {
        Geographies result = new Geographies();

        int currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
        Person currentUser = Person.FromIdentity(currentUserId);
        Authority authority = currentUser.GetAuthority();

        foreach (BasicPersonRole role in authority.LocalPersonRoles)
        {
            Geographies roleGeographies = Geography.FromIdentity(role.GeographyId).GetTree();

            result = Geographies.LogicalOr(result, roleGeographies);
        }

        return result;
    }

    protected People GetDirectReports ()
    {
        People result = new People();

        foreach (BasicPersonRole role in _authority.LocalPersonRoles)
        {
            if (role.Type == RoleType.LocalLead || role.Type == RoleType.LocalDeputy || role.Type == RoleType.LocalAdmin)
            {
                Geographies geographies = Geography.FromIdentity(role.GeographyId).Children;

                // HACK: Compensate for current bad tree structure

                if (role.GeographyId == 34)
                { //Lägg till Skåne till Södra
                    geographies = geographies.LogicalOr(Geography.FromIdentity(36).Children);
                }

                if (role.GeographyId == 32)
                { //Lägg till Västra Götaland till Västra
                    geographies = geographies.LogicalOr(Geography.FromIdentity(355).Children);
                }

                foreach (Geography geography in geographies)
                {
                    People localLead = new People();
                    RoleLookup allRoles = RoleLookup.FromGeographyAndOrganization(geography.Identity,
                                                                                  role.OrganizationId);

                    Roles leadRoles = allRoles[RoleType.LocalLead];

                    foreach (PersonRole leadRole in leadRoles)
                    {
                        localLead.Add(leadRole.Person);
                    }

                    result = result.LogicalOr(localLead);
                }
            }
        }

        return result;
    }

    protected People GetVicesAndAdmins ()
    {
        People result = new People();


        foreach (BasicPersonRole role in _authority.LocalPersonRoles)
        {
            if (role.Type == RoleType.LocalLead)
            {
                People localPeople = new People();

                RoleLookup allRoles = RoleLookup.FromGeographyAndOrganization(role.GeographyId, role.OrganizationId);

                Roles viceRoles = allRoles[RoleType.LocalDeputy];
                Roles adminRoles = allRoles[RoleType.LocalAdmin];

                foreach (PersonRole localRole in viceRoles)
                {
                    localPeople.Add(localRole.Person);
                }

                foreach (PersonRole localRole in adminRoles)
                {
                    localPeople.Add(localRole.Person);
                }

                result = result.LogicalOr(localPeople);
            }
        }

        return result;
    }

    /* Fix for Ticket #64 - Vice på distrikt- och valkretsnivå bör kunna assigna användare 
     * som är tilldelade Lead, för att avlasta varandra och undvika flaskhalsar. */
    protected People GetLeadsVicesAndAdmins ()
    {
        People result = new People();

        foreach (BasicPersonRole role in _authority.LocalPersonRoles)
        {
            if (role.Type == RoleType.LocalLead || role.Type == RoleType.LocalDeputy)
            {
                People localPeople = new People();

                RoleLookup allRoles = RoleLookup.FromGeographyAndOrganization(role.GeographyId, role.OrganizationId);

                Roles leadRoles = allRoles[RoleType.LocalLead];
                Roles viceRoles = allRoles[RoleType.LocalDeputy];
                Roles adminRoles = allRoles[RoleType.LocalAdmin];

                foreach (PersonRole localRole in leadRoles)
                {
                    localPeople.Add(localRole.Person);
                }

                foreach (PersonRole localRole in viceRoles)
                {
                    localPeople.Add(localRole.Person);
                }

                foreach (PersonRole localRole in adminRoles)
                {
                    localPeople.Add(localRole.Person);
                }

                result = result.LogicalOr(localPeople);
            }
        }

        return result;
    }


    protected void ButtonAssign_Click (object sender, EventArgs e)
    {
        string test = string.Empty;
        Person person = Person.FromIdentity(Int32.Parse(this.DropNewOwner.SelectedValue));

        foreach (string indexString in this.GridOwner.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int volunteerId = (int)this.GridOwner.MasterTableView.DataKeyValues[index]["Identity"];
            Volunteer volunteer = Volunteer.FromIdentity(volunteerId);
            volunteer.Owner = person;

            NotifyAboutAssignedVolunteer(person, volunteer);

        }

        PopulateGrids(false);
        this.GridOwner.Rebind();
        this.GridOther.Rebind();
        this.GridLeadGeography.Rebind();
        this.GridReports.Rebind();
    }

    protected void ButtonRevertDirects_Click (object sender, EventArgs e)
    {
        foreach (string indexString in this.GridReports.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int volunteerId = (int)this.GridReports.MasterTableView.DataKeyValues[index]["Identity"];
            Volunteer volunteer = Volunteer.FromIdentity(volunteerId);
            Person oldOwner = volunteer.Owner;
            volunteer.Owner = _currentUser;

            NotifyAboutLostVolunteer(oldOwner, volunteer, _currentUser);

        }

        PopulateGrids(false);
        this.GridOwner.Rebind();
        this.GridReports.Rebind();

    }




    protected void ButtonRevertViceAndAdmin_Click (object sender, EventArgs e)
    {

        foreach (string indexString in this.GridLeadGeography.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int volunteerId = (int)this.GridLeadGeography.MasterTableView.DataKeyValues[index]["Identity"];
            Volunteer volunteer = Volunteer.FromIdentity(volunteerId);
            Person oldOwner = volunteer.Owner;
            volunteer.Owner = _currentUser;

            NotifyAboutLostVolunteer(oldOwner, volunteer, _currentUser);

        }

        PopulateGrids(false);

        this.GridOwner.Rebind();
        this.GridLeadGeography.Rebind();

    }



    protected void ButtonRevertOthers_Click (object sender, EventArgs e)
    {
        foreach (string indexString in this.GridOther.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int volunteerId = (int)this.GridOther.MasterTableView.DataKeyValues[index]["Identity"];
            Volunteer volunteer = Volunteer.FromIdentity(volunteerId);
            Person oldOwner = volunteer.Owner;
            volunteer.Owner = _currentUser;

            NotifyAboutLostVolunteer(oldOwner, volunteer, _currentUser);

        }

        PopulateGrids(true);

        this.GridOwner.Rebind();
        this.GridOther.Rebind();
    }

    protected void ButtonMarkAsDone_Click (object sender, EventArgs e)
    {
        //Validate that this user is allowed to remove.

        People leads = this.GetDirectReports();

        if (leads.Count > 0)
        {
            bool checkedTooNew = false;
            foreach (string indexString in this.GridOwner.SelectedIndexes)
            {
                int index = Int32.Parse(indexString);
                int volunteerId = (int)this.GridOwner.MasterTableView.DataKeyValues[index]["Identity"];
                Volunteer volunteer = Volunteer.FromIdentity(volunteerId);
                if (volunteer.OpenedDateTime.AddDays(7) < DateTime.Now)
                {
                    volunteer.Close("Bulk-removed by " + _currentUser.Name);
                }
                else
                {
                    checkedTooNew = true;
                }
            }

            if (checkedTooNew)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "BulkMsg",
                    "alert('Note! Some of the volounteers were to new to remove in bulk.\r\n(Needs to be at least a week.)');", true);
            }

            PopulateGrids(false);
            this.GridOwner.Rebind();
            this.GridOther.Rebind();
            this.GridLeadGeography.Rebind();
            this.GridReports.Rebind();
        }
        else
        {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "BulkMsg",
                    "alert('Not allowed!');", true);
        }

    }

    protected void ButtonAutoAssign_Click (object sender, EventArgs e)
    {
        // BUG HERE: The new owner is not notified, but the perrson in
        // the dropdown box

        string test = string.Empty;
        Person person = Person.FromIdentity(Int32.Parse(this.DropNewOwner.SelectedValue));

        foreach (string indexString in this.GridOwner.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int volunteerId = (int)this.GridOwner.MasterTableView.DataKeyValues[index]["Identity"];
            Volunteer volunteer = Volunteer.FromIdentity(volunteerId);

            // Try to assign automatically
            volunteer.AutoAssign(volunteer.Geography, Organization.PPSEid, volunteer.Owner, Geography.SwedenId); //TODO:Ugly sweden hardcoded(30)

            NotifyAboutAssignedVolunteer(person, volunteer);
        }

        PopulateGrids(false);
        this.GridOwner.Rebind();
        this.GridOther.Rebind();
        this.GridLeadGeography.Rebind();
        this.GridReports.Rebind();
    }

    private void NotifyAboutAssignedVolunteer (Person person, Volunteer volunteer)
    {
        //"A volunteer for officer duty, {0}, was just assigned to you. " +
        //"Please check this at https://pirateweb.net/Pages/v4/ListVolunteers.aspx and delegate or " +
        //"process as soon as possible. The person volunteers for the following roles:\r\n\r\n "+
        //"{1}" +
        //"\r\nAs a general rule, leads should be assigned by the lead immediately above " +
        //"them in the org, and vices and admins should be assigned by the lead in the same area. " +
        //"Delegate the assessment and assignment to the correct person.\r\n";

        string mailbody = this.GetLocalResourceObject("AssignmentMailMessage").ToString();
        string mailbodyInclusion = this.GetLocalResourceObject("AssignmentMailInclusion").ToString();
        string mailSubject = this.GetLocalResourceObject("AssignmentMailSubject").ToString();

        VolunteerRoles roles = volunteer.Roles;
        string rolesString = "";
        foreach (VolunteerRole role in roles)
        {
            rolesString += string.Format("\r\n" + mailbodyInclusion, role.RoleType.ToString(), role.Geography.Name);
        }

        mailbody = string.Format(mailbody, volunteer.Name, rolesString);

        person.SendOfficerNotice(mailSubject, mailbody, Organization.PPSEid);
    }
    private void NotifyAboutLostVolunteer (Person oldOwner, Volunteer volunteer, Person viewingPerson)
    {
        //"A volunteer for officer duty, {0}, was previously assigned to you, but was now taken over by {1} again." +

        string mailbody = this.GetLocalResourceObject("RevokedMailMessage").ToString();
        string mailSubject = this.GetLocalResourceObject("RevokedMailSubject").ToString();


        mailbody = string.Format(mailbody, volunteer.Name, viewingPerson.Name);

        oldOwner.SendOfficerNotice(mailSubject, mailbody, Organization.PPSEid);
    }


    protected void ButtonAssignToDistrictLeads_Click (object sender, EventArgs e)
    {
        string test = string.Empty;

        // the districts are the level below this
        int currentStopGeography = Geography.SwedenId; //TODO: Ugly hard coded Sweden

        foreach (string indexString in this.GridOwner.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int volunteerId = (int)this.GridOwner.MasterTableView.DataKeyValues[index]["Identity"];
            Volunteer volunteer = Volunteer.FromIdentity(volunteerId);
            Person assignee = GetDistrictLead(volunteer.Geography, currentStopGeography);
            volunteer.Owner = assignee;

            NotifyAboutAssignedVolunteer(assignee, volunteer);
        }

        PopulateGrids(false);
        this.GridOwner.Rebind();
        this.GridLeadGeography.Rebind();
        this.GridReports.Rebind();
    }


    protected Person GetDistrictLead (Geography geography, int stopGeography)
    {
        // First, climb to district level or top level

        Geography district = geography;

        // Changed below since district.AtLevel(GeographyLevel.District) is never true 
        // because AtLevel presumes that district is present in GeographyOfficialDesignations wich it isn't
        // So the only reliable method of finding the districts is to detect that they ar children of country

        while (district.ParentGeographyId != 0 && district.ParentGeographyId != stopGeography)
        {
            district = district.Parent;
        }


        // We are now at District or world

        RoleLookup roleLookup = RoleLookup.FromGeographyAndOrganization(district.Identity, Organization.PPSEid);

        if (roleLookup[RoleType.LocalLead].Count == 0)
        {
            return Person.FromIdentity(1);
        }

        return roleLookup[RoleType.LocalLead][0].Person;
    }


    protected void GridOwner_ItemCommand (object source, GridCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "ManageVolunteer":
                // This turned out to be not the right way to do it
                break;
            default:
                break;
        }
    }

    protected void Grid_ItemCreated (object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            HyperLink editLink = (HyperLink)e.Item.FindControl("ManageLink");
            if (editLink != null)
            {
                editLink.Attributes["href"] = "#";
                editLink.Attributes["onclick"] = String.Format("return ShowManageForm('{0}','{1}');",
                                                               e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex][
                                                                   "Identity"], e.Item.ItemIndex);
            }

            /* Fix for Ticket #64 - showing parent geography of volunteers */
            Label labelGeographyParent = (Label)e.Item.FindControl("LabelGeographyParent");
            Volunteer currentVolunteer = e.Item.DataItem as Volunteer;
            if (currentVolunteer != null)
            {
                string geographyName = currentVolunteer.Geography.Name;

                if (currentVolunteer.Geography.ParentGeographyId != 0)
                {
                    geographyName += ", " + currentVolunteer.Geography.Parent.Name;
                }

                labelGeographyParent.Text = geographyName;


                // Display different color if volunteer has been waiting too long (two weeks)
                Label labelOpenedDateTime = (Label)e.Item.FindControl("LabelOpenedDateTime");
                labelOpenedDateTime.Text = currentVolunteer.OpenedDateTime.ToString("yyyy-MMM-dd HH:mm:ss");

                // RF bugfix: comparison was turned wrong way
                if (currentVolunteer.OpenedDateTime.AddDays(7).CompareTo(DateTime.Now) < 0)
                {
                    labelOpenedDateTime.ForeColor = System.Drawing.Color.FromArgb(232, 0, 0);
                }
            }
        }
    }

    protected void ButtonSendReminder_Click (object sender, EventArgs e)
    {
        // Not finished
    }


    protected void RadAjaxManager1_AjaxRequest (object sender, AjaxRequestEventArgs e)
    {
        if (e.Argument == "Rebind")
        {
            this.GridOwner.MasterTableView.SortExpressions.Clear();
            this.GridOwner.MasterTableView.GroupByExpressions.Clear();
            PopulateGrids(false);
            this.GridOwner.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            this.GridOwner.MasterTableView.SortExpressions.Clear();
            this.GridOwner.MasterTableView.GroupByExpressions.Clear();
            this.GridOwner.MasterTableView.CurrentPageIndex = this.GridOwner.MasterTableView.PageCount - 1;
            PopulateGrids(false);
            this.GridOwner.Rebind();
        }
    }

}