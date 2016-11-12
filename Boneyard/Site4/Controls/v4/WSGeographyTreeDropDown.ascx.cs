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


public partial class Controls_v4_WSGeographyTreeDropDown : System.Web.UI.UserControl
{
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


    public Geography root
    {
        set { tree.Root = value; }
    }

    public Controls_v4_WSGeographyTree tree
    {
        get { return (Controls_v4_WSGeographyTree)this.DropGeographies.Items[0].FindControl("GeographyTree"); }
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

        this.DropGeographies.Text = SelectedGeography.Name;
        this.DropGeographies.Items[0].Text = SelectedGeography.Name;
        this.DropGeographies.Items[0].Value = SelectedGeography.Identity.ToString();
        this.DropGeographies.Items[0].Selected = true;
        if (selectedNodeChanged != null)
        {
            selectedNodeChanged(sender, args);
        }
    }


    public int SelectedGeographyId
    {
        get
        {
            try
            {
                return tree.SelectedGeography.Identity;
            }
            catch
            {
                return 0;
            }
        }
        set
        {
            if (value > 0)
            {
                tree.SelectedGeographyId = value;
                this.DropGeographies.Text = SelectedGeography.Name;
                this.DropGeographies.Items[0].Text = SelectedGeography.Name;
                this.DropGeographies.Items[0].Value = SelectedGeography.Identity.ToString();
                this.DropGeographies.Items[0].Selected = true;
            }
            else
            {
                this.SelectedGeographyId = Geography.RootIdentity;
            }
        }
    }

    public Geography SelectedGeography
    {
        get
        {
            return tree.SelectedGeography;
        }
        set
        {
            try
            {
                SelectedGeographyId = value.Identity;
            }
            catch
            { }
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


    public Geography Root
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
        Controls_v4_WSGeographyTree.EmitScripts(page);
        ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "DropGeographies_OnClientNodeClicking",
        @"
;function DropGeographies_OnClientNodeClicking(sender, args) {
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
