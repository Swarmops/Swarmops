using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
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

public partial class Pages_Members_List : CompressedViewStatePageBase
{
    protected class OrgMembershipCacheEntry
    {
        public int orgId;
        public Dictionary<int, Activizr.Logic.Pirates.Membership> memberships = null;
        public DateTime membershipCacheTime = DateTime.MinValue;
    }

    static protected Dictionary<int, OrgMembershipCacheEntry> membershipCache = null;
    static protected object membershipCacheLock = new object();

    static List<int> handledOrganizations = new List<int>(new int[] { Organization.UPSEid, Organization.PPSEid });

    protected void Page_Load (object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {

            PersonList.PersonList = new People();

            PersonList.JsEditFunction = "LoadMember"; // Load subframe via javascript

            // Populate list of organizations (initial population)
            DropOrganizations.LoadTree(Organization.Root);


            Organizations organizationList = _authority.GetOrganizations(RoleTypes.AllRoleTypes);
            organizationList = organizationList.RemoveRedundant();
            DropOrganizations.Content.pruneToSubtree(organizationList.Identities);
            DropOrganizations.Content.expandLevels(true, 2);
            if (organizationList.Count > 0)
                DropOrganizations.SelectedValue = "" + organizationList[0].Identity;



            // Populate geographies

            PopulateGeographies();
        }

        PersonList.ViewEditCommand += PersonList_onEditCommand;
    }

    protected void PersonList_onEditCommand (object sender, GridViewCommandEventArgs e)
    {

        switch (e.CommandName)
        {
            case "ViewEdit":
                int index = Convert.ToInt32(e.CommandArgument);
                GridView gv = (GridView)sender;
                int personId = Convert.ToInt32(gv.DataKeys[index].Value);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "LoadMember", "parent.LoadMember('" + personId + "');", true);
                break;

        }
    }

    private People GetResults (bool getNonMembers,int gracePeriod)
    {
        LabelListHeader.Visible = false;
        LabelDupeHeader.Visible = false;
        LabelExpiryHeader.Visible = false;
        PersonList.ShowExpiry = false;

        People results = null;
        if (getNonMembers)
            results = People.FromGeography(Convert.ToInt32(DropGeographies.SelectedValue));
        else
            results = People.FromOrganizationAndGeography(Convert.ToInt32(DropOrganizations.SelectedValue),
                                                                 Convert.ToInt32(DropGeographies.SelectedValue));

        results = results.GetVisiblePeopleByAuthority(_authority, gracePeriod).RemoveUnlisted();
        return results;
    }

    protected void ButtonList_Click (object sender, EventArgs e)
    {

        People results = GetResults(false,0);
        LabelListHeader.Visible = false;
        Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        PersonList.ShowExpiry = true;
        PersonList.ListedOrg = selectedOrg.Identity;
        if (_authority.HasPermission(Permission.CanEditMemberships, selectedOrg.Identity, Convert.ToInt32(DropOrganizations.SelectedValue), Authorization.Flag.Default))
        {
            PersonList.ShowStatus = true;
        }
        PersonList.PersonList = results;

    }

    protected void ButtonDupeList_Click (object sender, EventArgs e)
    {

        People results = GetResults(false,0);
        results = results.RemoveUnique();

        LabelDupeHeader.Visible = true;
        Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        PersonList.ShowExpiry = true;
        PersonList.ListedOrg = selectedOrg.Identity;
        if (_authority.HasPermission(Permission.CanEditMemberships, selectedOrg.Identity, Convert.ToInt32(DropOrganizations.SelectedValue), Authorization.Flag.Default))
        {
            PersonList.ShowStatus = true;
        }
        PersonList.PersonList = results;
    }

    private static void RebuildMembershipCacheForOrg (int handledOrgId)
    {
        Organizations orgs = Organization.FromIdentity(handledOrgId).GetTree();
        Memberships ms = Memberships.ForOrganizations(orgs, true);
        OrgMembershipCacheEntry msc = null;
        foreach (Activizr.Logic.Pirates.Membership m in ms)
        {
            if (!membershipCache.ContainsKey(m.OrganizationId))
            {
                msc = new OrgMembershipCacheEntry();
                msc.memberships = new Dictionary<int, Activizr.Logic.Pirates.Membership>();
                membershipCache[m.OrganizationId] = msc;
            }
            else
            {
                msc = membershipCache[m.OrganizationId];
            }

            msc.membershipCacheTime = DateTime.Now;
            msc.memberships[m.PersonId] = m;
            msc.orgId = m.OrganizationId;
        }
    }

    protected void ButtonExpiringList_Click (object sender, EventArgs e)
    {
        Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        DateTime lowerlimit = DateTime.Today;
        DateTime upperlimit = DateTime.Today;

        int days = 0;
        if (!int.TryParse(this.ExpiryDays.Text, out days))
        {
            this.ExpiryDays.Text = "Error";
            return;
        }

        upperlimit = DateTime.Today.AddDays(Math.Abs(days) + 1);

        days = 0;
        if (!int.TryParse(this.SinceDays.Text, out days))
        {
            this.SinceDays.Text = "Error";
        }

        days = Math.Abs(days);
        if (days > Activizr.Logic.Pirates.Membership.GracePeriod)
        {
            this.SinceDays.Text = "" + Activizr.Logic.Pirates.Membership.GracePeriod;
            days = Activizr.Logic.Pirates.Membership.GracePeriod;
        }
        lowerlimit = DateTime.Today.AddDays(-Math.Abs(days));

        lock (membershipCacheLock)
        {
            if (membershipCache == null)
            {
                membershipCache = new Dictionary<int, OrgMembershipCacheEntry>();
            }

            //Tidy up cache to release memory
            if (membershipCache.Count > 0)
            {
                int[] keys = new int[membershipCache.Count];
                membershipCache.Keys.CopyTo(keys, 0);
                foreach (int orgid in keys)
                {
                    if (DateTime.Now > membershipCache[orgid].membershipCacheTime.AddMinutes(5))
                        membershipCache.Remove(orgid);
                }
            }

            //load cache
            if (!membershipCache.ContainsKey(selectedOrg.Identity)
                || DateTime.Now > membershipCache[selectedOrg.Identity].membershipCacheTime.AddMinutes(5))
            {
                RebuildMembershipCacheForOrg(selectedOrg.Identity);
            }

        }




        People results = GetResults(true,Activizr.Logic.Pirates.Membership.GracePeriod);

        lock (membershipCacheLock)
        {
            results = results.Filter(delegate(Person p)
            {
                if (membershipCache.ContainsKey(selectedOrg.Identity))
                {
                    OrgMembershipCacheEntry msc = membershipCache[selectedOrg.Identity];
                    if (msc.memberships.ContainsKey(p.Identity))
                    {
                        Activizr.Logic.Pirates.Membership m = msc.memberships[p.Identity];
                        //expired within search Period AND was not prematurely terminated
                        if (m.Expires >= lowerlimit && m.Expires < upperlimit
                        && (m.DateTerminated < m.MemberSince || m.DateTerminated.AddDays(1) > m.Expires))
                            return true;
                    }
                }
                return false;

            });
        }

        PersonList.ShowExpiry = true;
        PersonList.ListedOrg = selectedOrg.Identity;
        if (_authority.HasPermission(Permission.CanEditMemberships, selectedOrg.Identity, Convert.ToInt32(DropGeographies.SelectedValue), Authorization.Flag.Default))
            PersonList.ShowStatus = true;

        LabelExpiryHeader.Visible = true;
        PersonList.PersonList = results;
    }

    protected void PopulateGeographies ()
    {
        DropGeographies.LoadTree(Geography.Root);
        DropGeographies.Content.expandSubnode("" + _currentUser.Country.GeographyId, true, 1);

        Organization org = Organization.FromIdentity(Convert.ToInt32(DropOrganizations.SelectedValue));
        Geographies geoList = _authority.GetGeographiesForOrganization(org);

        geoList = geoList.RemoveRedundant();
        //geoList = geoList.FilterAbove(Geography.FromIdentity(org.AnchorGeographyId));

        List<int> permitted = new List<int>();

        foreach (Geography nodeRoot in geoList)
        {
            Geographies nodeTree = nodeRoot.GetTree();

            foreach (Geography node in nodeTree)
            {

                if (_authority.HasPermission(Permission.CanSeePeople, org.Identity, node.Identity, Authorization.Flag.Default))
                    permitted.Add(node.GeographyId);
            }
        }

        this.DropGeographies.Content.pruneToValues(permitted.ToArray());
        if (permitted.Count > 0)
        {
            this.DropGeographies.Content.expandSubnode("" + permitted[0], true, 2);
            this.DropGeographies.SelectedValue = "" + permitted[0];
        }

        RecalculatePersonCount();
    }


    protected void DropOrganizations_SelectedIndexChanged (object sender, EventArgs e)
    {
        PopulateGeographies();
    }


    protected void RecalculatePersonCount ()
    {
        this.ButtonList.Text = "List ";
        ButtonDupeList.Enabled = false;
        try
        {
            Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
            Organizations orgTree = selectedOrg.GetTree();

            Geography selectedGeo = Geography.FromIdentity(Convert.ToInt32(this.DropGeographies.SelectedValue));
            Geographies geoTree = selectedGeo.GetTree();

            int memberCount = 0;

            memberCount = orgTree.GetMemberCountForGeographies(geoTree);

            this.ButtonList.Text = "List " + memberCount.ToString("#,##0") + " people";
            this.ButtonList.Enabled = (memberCount > 0 ? true : false);

            ButtonDupeList.Enabled = false;
            ButtonExpiringList.Enabled = false;
            ButtonDupeList.Enabled = false;
            ButtonExpiringList.Enabled = false;
            PanelForwards.Visible = false;
            PanelBackwards.Visible = false;

            Literal1.Text = Literal1.Text.Replace("2500", "3500");

            if (memberCount <= 3500)
            {
                ButtonDupeList.Enabled = true;
                foreach (int handled in handledOrganizations)
                {
                    if (selectedOrg.Inherits(handled) || selectedOrg.Identity == handled)
                    {
                        if (_authority.HasPermission(Permission.CanEditMemberships, selectedOrg.Identity, selectedGeo.Identity , Authorization.Flag.Default))
                            PanelForwards.Visible = true;

                        ButtonExpiringList.Enabled = true;
                        if (_authority.HasPermission(Permission.CanSeeExpiredDuringGracePeriod, selectedOrg.Identity, selectedGeo.Identity, Authorization.Flag.Default))
                            PanelBackwards.Visible = true;
                    }
                }
            }

        }
        catch { }
    }

    protected void DropGeographies_SelectedIndexChanged (object sender, EventArgs e)
    {
        RecalculatePersonCount();
    }
}