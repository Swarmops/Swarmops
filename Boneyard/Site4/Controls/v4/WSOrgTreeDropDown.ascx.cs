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
using Telerik.Web.UI;


public partial class Controls_v4_WSOrgTreeDropDown : System.Web.UI.UserControl
{

    public Organization root
    {
        set { tree.Root = value; }
    }

    public string Skin
    {
        get
        {
            return tree.Skin;
        }
        set
        {
            tree.Skin = value;
        }
    }
    public Controls_v4_WSOrgTree tree
    {
        get { return (Controls_v4_WSOrgTree)this.DropOrganizations.Items[0].FindControl("OrgTree"); }
    }
    protected void Page_Load (object sender, EventArgs e)
    {
    }

    private RadTreeViewEventHandler selectedNodeChanged;

    public event RadTreeViewEventHandler SelectedNodeChanged
    {
        add
        {
            tree.SelectedNodeChanged += Tree_SelectedNodeChanged;
            selectedNodeChanged = value;

        }
        remove
        {
            tree.SelectedNodeChanged -= Tree_SelectedNodeChanged;
            selectedNodeChanged = null;
        }
    }


    public void Tree_SelectedNodeChanged (object sender, RadTreeNodeEventArgs args)
    {

        this.DropOrganizations.Text = SelectedOrganization.Name;
        this.DropOrganizations.Items[0].Text = SelectedOrganization.Name;
        this.DropOrganizations.Items[0].Value = SelectedOrganization.Identity.ToString();
        this.DropOrganizations.Items[0].Selected = true;
        if (selectedNodeChanged != null)
        {
            selectedNodeChanged(sender, args);
        }
    }

    public int SelectedOrganizationId
    {
        get
        {
            try
            {
                return tree.SelectedOrganization.Identity;
            }
            catch
            {
                return 0;
            }
        }
        set
        {
            try
            {
                tree.SelectedOrganizationId = value;
                this.DropOrganizations.Text = SelectedOrganization.Name;
            }
            catch
            {
                tree.RootId = Organization.RootIdentity;
                try
                {
                    tree.SelectedOrganizationId = value;
                    this.DropOrganizations.Text = SelectedOrganization.Name;
                }
                catch
                {
                    tree.SelectedOrganizationId = Organization.RootIdentity;
                    this.DropOrganizations.Text = SelectedOrganization.Name;
                }

            }
            this.DropOrganizations.Items[0].Text = SelectedOrganization.Name;
            this.DropOrganizations.Items[0].Value = SelectedOrganization.Identity.ToString();
            this.DropOrganizations.Items[0].Selected = true;
        }
    }

    public Organization SelectedOrganization
    {
        get
        {
            return tree.SelectedOrganization;
        }
    }

    public int RootId
    {
        get
        {
            return tree.RootId;
        }

        set
        {
            tree.RootId = value;
        }
    }


    public Organization Root
    {
        get
        {
            return tree.Root;
        }
        set
        {
            tree.Root = value;
        }
    }

    public static void EmitScripts (Control page)
    {
        Controls_v4_WSOrgTree.EmitScripts(page);
        ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "DropOrganizations_OnClientNodeClick",
        @"
;function DropOrganizations_OnClientNodeClicking(sender, args) {
            var node = args.get_node()
            var tree = node.get_treeView();
            var elem = tree.get_element();
            while (elem != null && elem.id.substring(elem.id.length  - ('_DropDown').length, elem.id.length) != '_DropDown') {
                elem = elem.parentNode;
            }
            var id = elem.id
            id = id.substr(0, id.length - ('_DropDown').length);
            var comboBox = $find(id);
            comboBox.set_text(node.get_text());
            comboBox.set_value(node.get_value());
            comboBox.hideDropDown();
        };
", true);
    }
}
