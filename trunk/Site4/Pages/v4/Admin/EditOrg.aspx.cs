using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using Telerik.Web.UI;
using Activizr.Logic.Structure;
using Activizr.Logic.DataObjects;
using System.Drawing;
using Activizr.Basic.Types;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using System.Data;

public partial class Pages_v4_admin_EditOrg : PageV4Base
{
    protected class AddressList
    {
        private string _maType;
        private string _name;
        private string _email;
        public string maType
        {
            get
            {
                return _maType;
            }
            set
            {
                _maType = value;
            }
        }
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public string email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
            }
        }
    }

    FormViewMode enteringMode;
    Dictionary<int, List<BasicUptakeGeography>> otherUptakesDict = null;

    protected void Page_Load (object sender, EventArgs e)
    {

        this.pagePermissionDefault = new PermissionSet(Permission.CanEditOrg);

        enteringMode = FormView1.CurrentMode;
        Controls_v4_WSGeographyTreeDropDown.EmitScripts(this);
        Controls_v4_WSOrgTreeDropDown.EmitScripts(this);
        WSOrgTreeDropDownSelectForEdit.tree.SetAuthority(_authority, Permission.CanEditOrg);
        
        programmedStyleTag.InnerHtml =
            programmedStyleTag.InnerHtml
                .Replace("RadMultiPage1.ClientID", RadMultiPage1.ClientID)
                .Replace("RadTabStrip1.ClientID", RadTabStrip1.ClientID);
                
        WSOrgTreeDropDownSelectForEdit.tree.topSortCountryId = _currentUser.CountryId; //current users country at the top of the dropdown

        if (!IsPostBack)
            FormView1.ChangeMode(FormViewMode.ReadOnly);
        //if (Request.Browser.IsBrowser("Firefox"))
        //{
        //    RadAjaxManager1.EnableAJAX = false;
        //    ScriptManager.GetCurrent(this).EnablePartialRendering = false;
        //}

    }

    protected void DropSelectForEdit_SelectedNodeChanged (object sender, EventArgs e)
    {
        FormView1.ChangeMode(FormViewMode.ReadOnly);
        int org = int.Parse(WSOrgTreeDropDownSelectForEdit.SelectedOrganizationId.ToString());

        OrganizationRoles1.SelectedOrganization = 0;
        OrganizationRoles2.SelectedOrganization = 0;

        RadTabStrip1.SelectedIndex = 0;
        RadMultiPage1.SelectedIndex = 0;

        if (_authority.HasPermission(Permission.CanEditOrg, org, -1, Authorization.Flag.Default))
        {

            HiddenSelectedOrgID.Value = org.ToString();
            FormView1.DataBind();
        }
        else
        {
            RegisterJSAlert("FailAccess", GetLocalResourceObject("NoAccessOrg").ToString());
            HiddenSelectedOrgID.Value = "";
            FormView1.DataBind();
            FormView1.ChangeMode(FormViewMode.ReadOnly);

        }
    }

    protected void HiddenSelectedOrgID_ValueChanged (object sender, EventArgs e)
    {

    }

    protected void UpdateTopDrop_Click (object sender, EventArgs e)
    {
        int orgid = 0;
        int.TryParse(HiddenSelectedOrgID.Value, out orgid);

        this.WSOrgTreeDropDownSelectForEdit.tree.tree.Nodes.Clear();
        this.WSOrgTreeDropDownSelectForEdit.SelectedOrganizationId = orgid;
        FormView1.ChangeMode(FormViewMode.ReadOnly);

    }

    protected void FormView1_ItemCommand (object sender, FormViewCommandEventArgs e)
    {
        LabelModeCopying.Visible = false;
        LabelModeEditing.Visible = false;
        LabelModeInserting.Visible = false;
        if (e.CommandName == "Copy")
        {
            LabelModeCopying.Visible = true;
            FormView1.DataSourceID = "DuplicatingDataSource";
            FormView1.ChangeMode(FormViewMode.Edit);
        }
        else if (e.CommandName == "Cancel")
        {
            if (FormView1.DataSourceID != "OrganizationsDataSource")
                FormView1.DataSourceID = "OrganizationsDataSource";
            FormView1.ChangeMode(FormViewMode.ReadOnly);
            FormView1.DataBind();
        }
        else if (e.CommandName == "New")
        {
            if (FormView1.DataSourceID != "OrganizationsDataSource")
                FormView1.DataSourceID = "OrganizationsDataSource";
            LabelModeInserting.Visible = true;
            FormView1.ChangeMode(FormViewMode.Insert);
        }
        else if (e.CommandName == "Edit")
        {
            if (FormView1.DataSourceID != "OrganizationsDataSource")
                FormView1.DataSourceID = "OrganizationsDataSource";
            LabelModeEditing.Visible = true;
            FormView1.ChangeMode(FormViewMode.Edit);
        }
        else if (e.CommandName == "DeleteOrg")
        {
            try
            {
                int org = int.Parse(WSOrgTreeDropDownSelectForEdit.SelectedOrganization.Identity.ToString());

                if (!_authority.HasRoleType(RoleType.SystemAdmin))
                    throw new Exception("Only SystemAdmin can delete organizations.");

                Organization.FromIdentity(org).Delete();
                WSOrgTreeDropDownSelectForEdit.tree.tree.Nodes.Clear();
                WSOrgTreeDropDownSelectForEdit.Root = null;

                FormView1.ChangeMode(FormViewMode.ReadOnly);
                FormView1.DataBind();
            }
            catch (Exception ex)
            {
                RegisterJSAlert("NoDelete", ex.Message);
            }
        }
    }

    protected void ShowNames_DataBinding (object sender, EventArgs e)
    {
        DropDownList showNames = sender as DropDownList;
        Organization org = FormView1.DataItem as Organization;
        showNames.SelectedValue = "" + org.ShowNamesInNotifications;
    }

    protected void UsePaymentStatus_DataBinding (object sender, EventArgs e)
    {
        DropDownList usePaymentStatus = sender as DropDownList;
        Organization org = FormView1.DataItem as Organization;
        usePaymentStatus.SelectedValue = "" + org.UsePaymentStatus;
    }

    protected void FormView1_DataBinding (object sender, EventArgs e)
    {

    }

    protected void FormView1_DataBound (object sender, EventArgs e)
    {
        PrepareView();
    }

    private void PrepareView ()
    {
        int orgid = 0;
        int.TryParse(HiddenSelectedOrgID.Value, out orgid);
        if (enteringMode != FormView1.CurrentMode)
        {
            if (FormView1.CurrentMode == FormViewMode.Edit)
                PrepareEditView(orgid);

            if (FormView1.CurrentMode == FormViewMode.Insert)
                PrepareInsertView(orgid);
        }

        if (FormView1.CurrentMode == FormViewMode.ReadOnly)
        {
            PrepareItemView(orgid);
        }
    }
    private void PrepareItemView (int orgId)
    {
        Button deleteButton = FindControlRecursive(FormView1, "ButtonDelete") as Button;
        if (deleteButton != null)
        {
            if (_authority.HasRoleType(RoleType.SystemAdmin))
            {
                deleteButton.Enabled = true;
            }
            else
            {
                deleteButton.Enabled = false;
                deleteButton.ToolTip = GetLocalResourceObject("OnlySysAdm").ToString();
            }
        }
        GridView gv = FindControlRecursive(FormView1, "GridViewDisplayUptakes") as GridView;
        if (gv != null)
            gv.DataBind();
        gv = FindControlRecursive(FormView1, "GridViewMailDisplay") as GridView;
        if (gv != null)
        {
            List<AddressList> addresses = new List<AddressList>();
            if (orgId > 0)
            {
                Organization org = Organization.FromIdentity(orgId);
                foreach (MailAuthorType ma in Enum.GetValues(typeof(MailAuthorType)))
                {
                    if (ma != MailAuthorType.Unknown)
                    {
                        AddressList a = new AddressList();
                        a.maType = ma.ToString();
                        FunctionalMail.AddressItem aitem = org.GetFunctionalMailAddressInh(ma);
                        if (aitem != null)
                        {
                            a.name = aitem.Name;
                            a.email = aitem.Email;

                            addresses.Add(a);
                        }
                    }
                }

            }
            gv.DataSource = addresses;
            gv.DataBind();
        }
    }

    private void PrepareInsertView (int orgid)
    {
        Organization org = Organization.FromIdentity(orgid); //Current selected(parent)

        FillDefaultCountry(org.DefaultCountry.Code);

        HiddenField ParentIdentityHidden = FindControlRecursive(FormView1, "ParentIdentityHidden") as HiddenField;

        if (ParentIdentityHidden != null)
        {
            TextBox ParentIdentityTextBox = FindControlRecursive(FormView1, "ParentIdentityTextBox") as TextBox;
            ParentIdentityHidden.Value = orgid.ToString();
            ParentIdentityTextBox.Text = Organization.FromIdentity(orgid).Name;
        }

        Controls_v4_WSGeographyTreeDropDown ancorDrop = FindControlRecursive(FormView1, "AnchorGeographyDropdown") as Controls_v4_WSGeographyTreeDropDown;
        if (ancorDrop != null)
        {
            ancorDrop.RootId = Geography.RootIdentity;
            ancorDrop.SelectedGeographyId = org.AnchorGeographyId;
        }


        Controls_v4_WSGeographyTree tree = FindControlRecursive(FormView1, "UptakeGeoTree") as Controls_v4_WSGeographyTree;

        tree.tree.CheckBoxes = true;
        tree.tree.ClearSelectedNodes();
        tree.tree.ClearCheckedNodes();

        //mark wich ones are already taken and by whom
        UptakeGeography[] othersUptakes = OrganizationsDataObject.SelectOrgOthersUptake(orgid);

        tree.EnsureGeographyLoaded(org.AnchorGeographyId, 1);

        foreach (UptakeGeography up in othersUptakes)
        {
            RadTreeNode node = tree.EnsureGeographyLoaded(up.GeoId, 0);
            node.BackColor = Color.Silver;
            node.ToolTip += up.Organization.Name + "\r\n";
        }
    }

    private void PrepareEditView (int orgid)
    {
        Organization org = Organization.FromIdentity(orgid);

        FillDefaultCountry(org.DefaultCountry.Code);


        Controls_v4_WSOrgTreeDropDown WSOrgTreeDropDownParentOrg = FindControlRecursive(FormView1, "WSOrgTreeDropDownParentOrg") as Controls_v4_WSOrgTreeDropDown;
        if (WSOrgTreeDropDownParentOrg != null)
        {
            WSOrgTreeDropDownParentOrg.tree.SetAuthority(_authority, Permission.CanEditOrg);
            WSOrgTreeDropDownParentOrg.SelectedOrganizationId = org.ParentOrganizationId;
        }

        Controls_v4_WSGeographyTreeDropDown ancorDrop = FindControlRecursive(FormView1, "AnchorGeographyDropdown") as Controls_v4_WSGeographyTreeDropDown;
        if (ancorDrop != null)
        {
            ancorDrop.RootId = Geography.RootIdentity;
            ancorDrop.SelectedGeographyId = org.AnchorGeographyId;
        }


        Controls_v4_WSGeographyTree tree = FindControlRecursive(FormView1, "UptakeGeoTree") as Controls_v4_WSGeographyTree;
        if (tree != null)
        {
            BasicUptakeGeography[] uptakes = OrganizationsDataObject.SelectOrgMineUptake(orgid);
            List<int> uptakeIds = new List<int>();
            foreach (BasicUptakeGeography up in uptakes)
            {
                uptakeIds.Add(up.GeoId);
            }

            tree.tree.CheckBoxes = true;

            Style s = new Style();
            s.BorderColor = Color.Green;
            s.BorderStyle = BorderStyle.Solid;
            s.BorderWidth = Unit.Pixel(2);
            s.BackColor = Color.LightGreen;

            tree.SetNodeStyles(uptakeIds.ToArray(), s);

            tree.tree.ClearSelectedNodes();
            tree.CheckedValues = uptakeIds.ToArray();
            UptakeGeography[] othersUptakes = OrganizationsDataObject.SelectOrgOthersUptake(orgid);
            List<int> othersUptakeIds = new List<int>();
            foreach (UptakeGeography up in othersUptakes)
            {
                RadTreeNode node = tree.EnsureGeographyLoaded(up.GeoId, 0);
                node.BackColor = Color.Silver;
                node.ToolTip += up.Organization.Name + "\r\n";
                node.Style["padding-top"] = "1px";
                node.Style["padding-left"] = "2px";
                node.Style["padding-bottom"] = "1px";
                node.Style["padding-right"] = "2px";
                node.Style["margin-top"] = "2px";
                node.Style["margin-left"] = "0px";
                node.Style["margin-bottom"] = "2px";
                node.Style["margin-right"] = "0px";

            }
        }

        GridView gv = FindControlRecursive(FormView1, "GridViewMailEdit") as GridView;
        if (gv != null)
        {
            List<AddressList> addresses = new List<AddressList>();
            if (orgid > 0)
            {
                foreach (MailAuthorType ma in Enum.GetValues(typeof(MailAuthorType)))
                {
                    if (ma != MailAuthorType.Unknown)
                    {
                        AddressList a = new AddressList();
                        a.maType = ma.ToString();
                        FunctionalMail.AddressItem aitem = org.GetFunctionalMailAddress(ma);
                        FunctionalMail.AddressItem bitem = org.GetFunctionalMailAddressInh(ma);
                        if (aitem != null)
                        {
                            a.name = aitem.Name;
                            a.email = aitem.Email;

                        }
                        else
                        {
                            a.name = "";
                            a.email = "";
                        }
                        if (bitem != null)
                            addresses.Add(a);
                    }
                }

            }
            gv.DataSource = addresses;
            gv.DataBind();
        }

    }

    private void FillDefaultCountry (string defaultCountryCode)
    {
        try
        {
            DropDownList DropCountries = FindControlRecursive(FormView1, "DropDefaultCountry") as DropDownList;
            if (DropCountries.Items.Count < 1)
            {
                Countries countries = Countries.GetAll();

                foreach (Country country in countries)
                {
                    DropCountries.Items.Add(new ListItem(country.Code + " " + country.Name, country.Code));
                }

                DropCountries.Items.FindByValue(defaultCountryCode).Selected = true;
            }
        }
        catch { }
    }

    protected void Page_PreRender (object sender, EventArgs e)
    { }

    protected void FormView1_ItemCreated (object sender, EventArgs e)
    {
        PrepareView();
    }

    protected void FormView1_ItemInserting (object sender, FormViewInsertEventArgs e)
    {
        int org = int.Parse(WSOrgTreeDropDownSelectForEdit.SelectedOrganization.Identity.ToString());
        PrepareInsertView(org);
    }

    protected void FormView1_ItemInserted (object sender, FormViewInsertedEventArgs e)
    {
        SaveUptakes();
    }

    protected void FormView1_ItemUpdating (object sender, FormViewUpdateEventArgs e)
    {

    }

    protected void FormView1_ItemUpdated (object sender, FormViewUpdatedEventArgs e)
    {
        SaveUptakes();
        SaveOptionalFields();
    }


    protected void OrganizationsDataSource_Updated (object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            throw e.Exception;
        }
    }

    protected void OrganizationsDataSource_Deleted (object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            throw e.Exception;
        }
    }

    protected void UptakesDataSource_Deleted (object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            throw e.Exception;
        }
    }

    protected void UptakesDataSource_Inserted (object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            throw e.Exception;
        }
    }

    protected void OrganizationsDataSource_Inserted (object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            throw e.Exception;
        }
        else
        {
            HandleRecordWasInserted((int)e.ReturnValue);
        }
    }

    protected void DuplicatingDataSource_Updated (object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            throw e.Exception;
        }
        else
        {
            if (FormView1.DataSourceID != "OrganizationsDataSource")
                FormView1.DataSourceID = "OrganizationsDataSource";
            HandleRecordWasInserted((int)e.ReturnValue);
        }
    }

    protected void RadTabStrip1_TabClick (object sender, RadTabStripEventArgs e)
    {
        int org = 0;
        int.TryParse(WSOrgTreeDropDownSelectForEdit.SelectedOrganization.Identity.ToString(), out org);

        if (e.Tab.Index == 1)
        {
            if (org != OrganizationRoles1.SelectedOrganization)
                OrganizationRoles1.SelectedOrganization = org;
            OrganizationRoles2.SelectedOrganization = 0;
        }
        else if (e.Tab.Index == 2)
        {
            if (org != OrganizationRoles2.SelectedOrganization)
                OrganizationRoles2.SelectedOrganization = org;
            OrganizationRoles1.SelectedOrganization = 0;
        }
        else
        {
            OrganizationRoles2.SelectedOrganization = 0;
            OrganizationRoles1.SelectedOrganization = 0;
        }

    }


    private void SaveOptionalFields ()
    {
        int orgid = 0;
        int.TryParse(HiddenSelectedOrgID.Value, out orgid);
        if (orgid != 0)
        {
            Organization org = Organization.FromIdentity(orgid);
            DropDownList ddl = null;

            //ShowNamesInNotifications
            ddl = FindControlRecursive(FormView1, "DropDownShowNamesInNotifications") as DropDownList;

            if (ddl.SelectedValue.ToLower() == "true")
                org.ShowNamesInNotifications = true;
            else if (ddl.SelectedValue.ToLower() == "false")
                org.ShowNamesInNotifications = false;
            else
                org.ShowNamesInNotifications = null;

            //UsePaymentStatus
            ddl = FindControlRecursive(FormView1, "DropDownUsePaymentStatus") as DropDownList;

            if (ddl.SelectedValue.ToLower() == "true")
                org.UsePaymentStatus = true;
            else if (ddl.SelectedValue.ToLower() == "false")
                org.UsePaymentStatus = false;
            else
                org.UsePaymentStatus = null;

            //Functional Mail addresses
            GridView gv = FindControlRecursive(FormView1, "GridViewMailEdit") as GridView;
            if (gv != null)
            {
                foreach (GridViewRow gRow in gv.Rows)
                {
                    Label LabelType = FindControlRecursive(gRow, "LabelType") as Label;
                    TextBox TextBoxName = FindControlRecursive(gRow, "TextBoxName") as TextBox;
                    TextBox TextBoxEmail = FindControlRecursive(gRow, "TextBoxEmail") as TextBox;
                    MailAuthorType maType = (MailAuthorType)Enum.Parse(typeof(MailAuthorType), LabelType.Text);
                    FunctionalMail.AddressItem directItem = org.GetFunctionalMailAddress(maType);
                    if (directItem == null
                        || TextBoxName.Text != directItem.Name
                        || TextBoxEmail.Text != directItem.Email)
                    {
                        FunctionalMail.AddressItem inhItem = org.GetFunctionalMailAddressInh(maType);
                        if (inhItem != null
                            && TextBoxName.Text == inhItem.Name
                            && TextBoxEmail.Text == inhItem.Email)
                        {
                            TextBoxName.Text = "";
                            TextBoxEmail.Text = "";
                        }
                        org.SetFunctionalMailAddress(maType, TextBoxName.Text, TextBoxEmail.Text);

                    }
                }
            }
        }
    }


    private void HandleRecordWasInserted (int newId)
    {
        this.WSOrgTreeDropDownSelectForEdit.tree.tree.Nodes.Clear();
        this.WSOrgTreeDropDownSelectForEdit.SelectedOrganizationId = newId;
        HiddenSelectedOrgID.Value = newId.ToString();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "HiddenSelectedOrgID", "setTimeout('redrawTopDropdown()',100);", true);
    }

    private void RegisterJSAlert (string key, string message)
    {
        string escapedMessage = Server.UrlEncode(message).Replace("+", "%20");
        ScriptManager.RegisterStartupScript(this, this.GetType(), key, "alert(unescape('" + escapedMessage + "'));", true);
    }


    protected void GridViewDisplayUptakes_DataBinding (object sender, EventArgs e)
    {
        int orgid = 0;
        int.TryParse(HiddenSelectedOrgID.Value, out orgid);

        BasicUptakeGeography[] othersUptakes = OrganizationsDataObject.SelectOrgOthersUptake(orgid);
        otherUptakesDict = new Dictionary<int, List<BasicUptakeGeography>>();

        if (othersUptakes != null)
        {
            foreach (BasicUptakeGeography up in othersUptakes)
            {
                if (!otherUptakesDict.ContainsKey(up.GeoId))
                    otherUptakesDict[up.GeoId] = new List<BasicUptakeGeography>();
                otherUptakesDict[up.GeoId].Add(up);
            }
        }
    }

    protected void GridViewDisplayUptakes_RowDataBound (object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView gv = FindControlRecursive(e.Row, "GridViewOtherUptakes") as GridView;
            gv.DataBind();
        }
    }

    protected void GridViewOtherUptakes_RowDataBound (object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            UptakeGeography bu = ((UptakeGeography)e.Row.DataItem);
            Organization org = Organization.FromIdentity(bu.OrgId);
            if (org.AnchorGeographyId == bu.GeoId)
                e.Row.Cells[4].Text = "Anchor";
            else if (!_authority.HasPermission(Permission.CanEditOrg, bu.OrgId, -1, Authorization.Flag.Default))
                e.Row.Cells[4].Text = "No Access";
            else
            {
                LinkButton deletebtn = e.Row.FindControl("lnkDelete") as LinkButton;
                string jscript = GetLocalResourceObject("ConfimDeleteUptakeJS").ToString();
                //return confirm('Do you really want to remove {0} as uptake area for {1}?');
                jscript = string.Format(jscript, bu.Geography.Name, bu.Organization.Name);
                deletebtn.OnClientClick = jscript;
            }
        }
    }

    protected void GridViewDisplayUptakes_RowDeleting (object sender, GridViewDeleteEventArgs e)
    {

    }

    protected void GridViewOtherUptakes_RowDeleting (object sender, GridViewDeleteEventArgs e)
    {
        GridView grid = sender as GridView;

    }

    private void SaveUptakes ()
    {
        Controls_v4_WSGeographyTree tree = FindControlRecursive(FormView1, "UptakeGeoTree") as Controls_v4_WSGeographyTree;
        int orgid = 0;
        int.TryParse(HiddenSelectedOrgID.Value, out orgid);
        OrganizationsDataObject.UpdateOrgUptake(orgid, tree.CheckedValues);
    }


    protected void RadAjaxManager1_AjaxRequest (object sender, AjaxRequestEventArgs e)
    {
    }

    protected void OnRowDataBound_GridViewMailDisplay (object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

        }
    }

    protected Control FindControlRecursive (Control c, string id)
    {
        if (c.ID == id) return c;
        foreach (Control subC in c.Controls)
        {
            Control found = FindControlRecursive(subC, id);
            if (found != null && found.ID == id)
                return found;
        }
        return null;
    }

    protected void OrganizationsDataSource_DataBinding (object sender, EventArgs e)
    {

    }

    protected void OrganizationsDataSource_Updating (object sender, ObjectDataSourceMethodEventArgs e)
    {
        if (((OrganizationsDataObject.Org)e.InputParameters["org"]).Identity == Organization.RootIdentity)
        {
            ((OrganizationsDataObject.Org)e.InputParameters["org"]).ParentOrganizationId = 0;
        }
    }
}
