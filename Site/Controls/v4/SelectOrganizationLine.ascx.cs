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

public partial class Controls_v4_SelectOrganizationLine : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public void DropOrganizations_SelectedNodeChanged(object sender, EventArgs e)
    {
        this.LabelOrganizationComment.Text = DropOrganizations.SelectedOrganization.Name + " was selected. Add interesting fact here.";

        if (this.SelectedNodeChanged != null)
        {
            SelectedNodeChanged(this, e);
        }
    }

    public Organization SelectedOrganization
    {
        get
        {
            return this.DropOrganizations.SelectedOrganization;
        }
    }

    public event EventHandler SelectedNodeChanged;
}
