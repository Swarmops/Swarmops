using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Pirates;
using Activizr.Basic.Types;
using Activizr.Basic.Enums;
using Activizr.Logic.Structure;




public partial class Pages_Special_FI_HandlePaymentStatus : PageV4Base
{



    protected void Page_Load (object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            DropOrganizations.LoadTree(Organization.Root);


            Organizations organizationList = _authority.GetOrganizations(RoleTypes.AllRoleTypes);
            organizationList = organizationList.RemoveRedundant();
            DropOrganizations.Content.pruneToSubtree(organizationList.Identities);
            DropOrganizations.Content.expandLevels(true, 2);
            if (organizationList.Count > 0)
                DropOrganizations.SelectedValue = "" + organizationList[0].Identity;

            string[] statuses = Enum.GetNames(typeof(MembershipPaymentStatus));
            MembershipPaymentStatus[] statusesValues = Enum.GetValues(typeof(MembershipPaymentStatus)) as MembershipPaymentStatus[];
            DropDownListStatusSearch.Items.Clear();
            DropDownListStatusChange.Items.Clear();
            foreach (MembershipPaymentStatus st in statusesValues)
            {
                DropDownListStatusSearch.Items.Add(new ListItem(st.ToString(), "" + (int)st));
                if ((int)st > 0)
                    DropDownListStatusChange.Items.Add(new ListItem(st.ToString(), "" + (int)st));
            }

        }

    }


    protected void ObjectDataSource1_ObjectCreating (object sender, ObjectDataSourceEventArgs e)
    {
        int[] listedPersons = ViewState["persons"] as int[];
        ListPersonDataSource ds = new ListPersonDataSource(listedPersons);
        ds.selectedOrgId = int.Parse(DropOrganizations.SelectedValue);
        e.ObjectInstance = ds;
    }

    protected void ButtonSearch_Click (object sender, EventArgs e)
    {
        RebuildGrid();

    }

    private void RebuildGrid ()
    {
        int selectedOrg = int.Parse(DropOrganizations.SelectedValue);
        int selectedStatus = int.Parse(DropDownListStatusSearch.SelectedValue);
        BasicMembership[] ms = Activizr.Database.PirateDb.GetDatabase().GetValidMembershipsForOrganizationAndPaymentStatus(new int[] { selectedOrg }, selectedStatus);
        List<int> peopleIds = new List<int>();
        foreach (BasicMembership bm in ms)
        {
            peopleIds.Add(bm.PersonId);
        }

        ViewState["persons"] = peopleIds.ToArray();
        GridView1.DataBind();
    }

    protected void GridView1_RowDataBound (object sender, GridViewRowEventArgs e)
    {

    }

    protected void ButtonChange_Click (object sender, EventArgs e)
    {

        MembershipPaymentStatus newStatus = (MembershipPaymentStatus)Convert.ToInt32(DropDownListStatusChange.SelectedValue);
        DateTime nowvalue = DateTime.Now;
        string checkedIds = "" + Request.Form["CheckboxSelect"];
        string[] checkedIdArr = checkedIds.Split(',');
        List<int> ids = new List<int>();
        Memberships membershipsToLoad = new Memberships();

        foreach (string id in checkedIdArr)
        {
            Membership ms = Membership.FromIdentity(Convert.ToInt32(id));
            membershipsToLoad.Add(ms);
        }
        Membership.LoadPaymentStatuses(membershipsToLoad);
        foreach (Membership ms in membershipsToLoad)
        {
            ms.SetPaymentStatus(newStatus, nowvalue);
        }

        RebuildGrid();

    }
}
