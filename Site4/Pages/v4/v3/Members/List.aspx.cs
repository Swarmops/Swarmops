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

public partial class Pages_Members_List : PageV4Base
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

            // Populate list of organizations (initial population)

            Organizations organizationList = _authority.GetOrganizations(RoleTypes.AllRoleTypes);
            organizationList = organizationList.RemoveRedundant();

            foreach (Organization organization in organizationList)
            {
                Organizations organizationTree = organization.GetTree();

                foreach (Organization organizationOption in organizationTree)
                {
                    string orgLabel = organizationOption.NameShort;

                    /*
					for (int loop = 0; loop < organizationOption.Generation; loop++)
					{
						orgLabel = "|-- " + orgLabel;
					}*/

                    DropOrganizations.Items.Add(new ListItem(orgLabel, organizationOption.OrganizationId.ToString()));
                }
            }

            // Populate geographies

            PopulateGeographies();
        }
    }

    private People GetResults ()
    {
        LabelListHeader.Visible = false;
        LabelDupeHeader.Visible = false;
        LabelExpiryHeader.Visible = false;
        PersonList.ShowExpiry = false;

        People results = People.FromOrganizationAndGeography(Convert.ToInt32(DropOrganizations.SelectedValue),
                                                             Convert.ToInt32(DropGeographies.SelectedValue));
        results = results.GetVisiblePeopleByAuthority(_authority).RemoveUnlisted();
        return results;
    }

    protected void ButtonList_Click (object sender, EventArgs e)
    {

        People results = GetResults();
        PersonList.PersonList = results;
        LabelListHeader.Visible = false;
    }

    protected void ButtonDupeList_Click (object sender, EventArgs e)
    {

        People results = GetResults();
        results = results.RemoveUnique();

        PersonList.PersonList = results;
        LabelDupeHeader.Visible = true;
    }

    private static void RebuildMembershipCacheForOrg (int handledOrgId)
    {
        Organizations orgs = Organization.FromIdentity(handledOrgId).GetTree();
        Memberships ms = Memberships.ForOrganizations(orgs, false);
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
        int days = 0;
        if (!int.TryParse(this.ExpiryDays.Text, out days))
        {
            this.ExpiryDays.Text = "Error";
            return;
        }

        DateTime lowerlimit = DateTime.Today;
        DateTime upperlimit = DateTime.Today;

        if (days < 0)
            lowerlimit = DateTime.Today.AddDays(days);
        else
            upperlimit = DateTime.Today.AddDays(days + 1);

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




        People results = GetResults();

        results = results.Filter(delegate(Person p)
        {
            if (membershipCache.ContainsKey(selectedOrg.Identity))
            {
                OrgMembershipCacheEntry msc = membershipCache[selectedOrg.Identity];
                if (msc.memberships.ContainsKey(p.Identity))
                {
                    Activizr.Logic.Pirates.Membership m = msc.memberships[p.Identity];
                    if (m.Expires >= lowerlimit && m.Expires < upperlimit)
                        return true;
                }
            }
            return false;

        });

        PersonList.ShowExpiry = true;
        PersonList.ListedOrg = selectedOrg.Identity;

        PersonList.PersonList = results;
        LabelExpiryHeader.Visible = true;
    }

    protected void PopulateGeographies ()
    {

        Organization org = Organization.FromIdentity(Convert.ToInt32(DropOrganizations.SelectedValue));
        Geographies geoList = _authority.GetGeographiesForOrganization(org);

        geoList = geoList.RemoveRedundant();
        geoList = geoList.FilterAbove(Geography.FromIdentity(org.AnchorGeographyId));

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
                if (_authority.HasPermission(Permission.CanSeePeople, org.Identity, node.Identity, Authorization.Flag.Default))
                    DropGeographies.Items.Add(new ListItem(nodeLabel, node.GeographyId.ToString()));
            }
        }
        if (_authority.HasPermission(Permission.CanSeePeople, org.Identity, -1, Authorization.Flag.Default))
            DropGeographies.Items.Add(new ListItem("Any", Geography.RootIdentity.ToString()));

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

            // Optimize a little: if we are at the org's top anchor, do not parse the geography, just select all
            // Removed, doesnt work if people live in other geo.

            if (false && selectedGeo.Identity == selectedOrg.AnchorGeographyId)
            {
                memberCount = orgTree.GetMemberCount();
            }
            else
            {
                memberCount = orgTree.GetMemberCountForGeographies(geoTree);
            }

            this.ButtonList.Text = "List " + memberCount.ToString("#,##0") + " people";
            this.ButtonList.Enabled = (memberCount > 0 ? true : false);

            ButtonDupeList.Enabled = false;
            ButtonExpiringList.Enabled = false;
            
            Literal1.Text=Literal1.Text.Replace("2500","3500");
            if (memberCount <= 3500)
            {
                ButtonDupeList.Enabled = true;
                foreach (int handled in handledOrganizations)
                {
                    if (selectedOrg.Inherits(handled) || selectedOrg.Identity==handled )
                    {
                        ButtonExpiringList.Enabled = true;
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