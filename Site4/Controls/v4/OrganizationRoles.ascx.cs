using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Activizr.Interface.DataObjects;
using Activizr.Basic.Enums;

public partial class Controls_v4_OrganizationRoles : System.Web.UI.UserControl
{
    string lastOrg = "";
    string lastGeo = "";
    protected void Page_Load (object sender, EventArgs e)
    {

    }

    public enum ListScope
    {
        MineAndSub, Mine, SubOrgs
    }

    public ListScope Scope
    {
        set
        {
            switch (value)
            {
                case ListScope.MineAndSub:
                    OrganizationRoleDataSource.SelectMethod = "SelectAllByOrganization";
                    break;
                case ListScope.Mine:
                    OrganizationRoleDataSource.SelectMethod = "SelectThisByOrganization";
                    break;
                case ListScope.SubOrgs:
                    OrganizationRoleDataSource.SelectMethod = "SelectSubByOrganization";
                    break;
            }
            OrganizationRoleDataSource.DataBind();

        }
    }
    public int SelectedOrganization
    {
        get
        {
            int ret = 0;
            int.TryParse(HiddenField1.Value, out ret);
            return ret;
        }
        set
        {
            HiddenField1.Value = value.ToString();
            RadGrid1.DataBind();
        }
    }
    protected void RadGrid1_ItemDataBound (object sender, Telerik.Web.UI.GridItemEventArgs e)
    {

        if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
        {
            TableCell geoCell = (e.Item as GridDataItem)["GeographyName"];
            TableCell personCell = (e.Item as GridDataItem)["PersonName"];
            OrganizationRolesDataObject.Role roleObject = (OrganizationRolesDataObject.Role)e.Item.DataItem;

            foreach (TableCell c in e.Item.Cells)
            {
                int cellPadding = roleObject.Level * 10;
                if (roleObject.GeographyName != lastGeo)
                    c.Style["border-top"] = "1px dashed black";
                if (roleObject.OrganisationName != lastOrg)
                    c.Style["border-top"] = "1px solid black";
                if (c.UniqueID == geoCell.UniqueID)
                    cellPadding += (roleObject.GeoLevel * 10);
                c.Style[HtmlTextWriterStyle.PaddingLeft] = cellPadding.ToString() + "px";

                if (c.UniqueID == personCell.UniqueID)
                {
                    c.Text = "<a href='/Pages/v4/v3/MembersFramed/PersonFrame.aspx?id=" + roleObject.PersonId + "' target='_blank'>" + roleObject.PersonName + "</a>";
                }
            }
            lastOrg = roleObject.OrganisationName;
            lastGeo = roleObject.GeographyName;

                        

        }
    }
}
