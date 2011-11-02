using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using jQuery.ServerControls;
using jQuery.ServerControls.TreeDropdown;
using Activizr.Logic.Structure;

public partial class Tests_JL_TestGeoTreeJQ : System.Web.UI.Page
{
    int childnr = 0;
    protected void Page_Load (object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            this.GeographyDropdown1.SetValue("0", "Please Select");

            GeographyDropdown1.LoadTree(Organization.Root);
            
            //node0.pruneToValues(new string[] { "34","83", "84", "85" });
            
            //GeographyDropdown1.Content.expandSubnode(""+Geography.SwedenId,true,2);
            GeographyDropdown1.DropDown = true;
            GeographyDropdown1.MultiSelect = true;
            GeographyDropdown1.CheckBoxes = true;
            GeographyDropdown1.SelectedValue="83,84";
        }

    }

    private void buildgeoTree (TreeViewNode node0, Geography g)
    {
        foreach (Geography cg in g.Children)
        {
            childnr++;
            TreeViewNode node = new TreeViewNode(cg.Name, "" + cg.Identity);
            node0.AddChild(node);
            buildgeoTree(node, cg);
        }
    }
    
    protected void Button1_Click (object sender, EventArgs e)
    {
        TextBox1.Text = GeographyDropdown1.SelectedValue + " " + GeographyDropdown1.SelectedText;

    }
}
