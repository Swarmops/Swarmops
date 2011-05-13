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

public partial class m_Pages_Members_List : System.Web.UI.Page
{
    private class RememberListParams
    {
        public RememberListParams ()
        {
        }
        public RememberListParams (string init)
            : this()
        {
            string[] par = (init + "||||").Split(new char[] { '|' });
            Org = par[0];
            Geo = par[1];

        }

        public string Org;
        public string Geo;

        public override string ToString ()
        {
            return Org + "|" + Geo;
        }
    }
    protected void Page_Load (object sender, EventArgs e)
    {
        int viewerPersonId = Int32.Parse(HttpContext.Current.User.Identity.Name);
        Person viewerPerson = Person.FromIdentity(viewerPersonId);
        Authority authority = viewerPerson.GetAuthority();


        if (!Page.IsPostBack)
        {
            PersonList.PersonList = new People();

            // Populate list of organizations (initial population)

            Organizations organizationList = authority.GetOrganizations(RoleTypes.AllLocalRoleTypes);
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


            if (Request.Cookies["mListParams"] != null)
            {
                CheckBoxRemember.Checked = true;

                RememberListParams r = new RememberListParams(Request.Cookies["mListParams"].Value);
                try { DropOrganizations.SelectedValue = r.Org; }
                catch (Exception) { }

                PopulateGeographies();

                try { DropGeographies.SelectedValue = r.Geo; }
                catch (Exception) { }

                RecalculatePersonCount();
            }
            else
            {
                PopulateGeographies();
            }

        }
    }

    protected void ButtonList_Click (object sender, EventArgs e)
    {
        try
        {
            Response.Cookies.Clear();
            if (CheckBoxRemember.Checked)
            {
                RememberListParams r = new RememberListParams();
                r.Org = DropOrganizations.SelectedValue;
                r.Geo = DropGeographies.SelectedValue;
                Response.Cookies["mListParams"].Value = r.ToString();
                Response.Cookies["mListParams"].Expires = DateTime.Now.AddMonths(1);
                Response.Cookies["mListParams"].Path = "/m";
            }

            ExecuteSearch();
        }
        catch (Exception)
        {
        }
    }

    private void ExecuteSearch ()
    {
        int viewerPersonId = Int32.Parse(HttpContext.Current.User.Identity.Name);
        Person viewer = Person.FromIdentity(viewerPersonId);
        Authority authority = viewer.GetAuthority();

        People results = People.FromOrganizationAndGeography(Convert.ToInt32(DropOrganizations.SelectedValue),
                                                             Convert.ToInt32(DropGeographies.SelectedValue));
        results = results.GetVisiblePeopleByAuthority(authority).RemoveUnlisted();

        PersonList.PersonList = results;
    }


    protected void PopulateGeographies ()
    {
        Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
        Authority authority = viewingPerson.GetAuthority();

        try
        {

            Organization org = Organization.FromIdentity(Convert.ToInt32(DropOrganizations.SelectedValue));
            Geographies geoList = authority.GetGeographiesForOrganization(org);

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
                    if (authority.HasPermission(Permission.CanSeePeople, org.Identity, node.Identity, Authorization.Flag.Default))
                        DropGeographies.Items.Add(new ListItem(nodeLabel, node.GeographyId.ToString()));
                }
            }
            if (authority.HasPermission(Permission.CanSeePeople, org.Identity, -1, Authorization.Flag.Default))
                DropGeographies.Items.Add(new ListItem("Any", Geography.RootIdentity.ToString()));

        }
        catch (Exception)
        {
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
        int memberCount = 0;
        try
        {
            Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
            Organizations orgTree = selectedOrg.GetTree();

            Geography selectedGeo = Geography.FromIdentity(Convert.ToInt32(this.DropGeographies.SelectedValue));
            Geographies geoTree = selectedGeo.GetTree();


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
        }
        catch (Exception)
        {
        }

        this.ButtonList.Text = "List " + memberCount.ToString("#,##0") + " people";
        this.ButtonList.Enabled = (memberCount > 0 ? true : false);
    }

    protected void DropGeographies_SelectedIndexChanged (object sender, EventArgs e)
    {
        RecalculatePersonCount();
    }
}