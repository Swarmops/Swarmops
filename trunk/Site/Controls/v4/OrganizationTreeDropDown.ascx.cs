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

using Activizr.Logic.Structure;


public partial class Controls_v4_OrganizationTreeDropDown : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Organization organization = SelectedOrganization;

        if (organization != null)
        {
            this.DropOrganizations.Items[0].Text = organization.Name;
        }
    }


    public void Tree_SelectedNodeChanged(object sender, EventArgs args)
    {
        if (SelectedNodeChanged != null)
        {
            SelectedNodeChanged(this, args);
        }

        this.DropOrganizations.Items[0].Text = SelectedOrganization.Name;
    }


    public Organization SelectedOrganization
    {
        get
        {
            Controls_v4_OrganizationTree tree = (Controls_v4_OrganizationTree) this.DropOrganizations.Items[0].FindControl("OrganizationTree");

            return tree.SelectedOrganization;
        }
    }


    public event EventHandler SelectedNodeChanged;
}
