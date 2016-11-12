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


public partial class Controls_v4_GeographyTreeDropDown : System.Web.UI.UserControl
{
    protected void Page_Load (object sender, EventArgs e)
    {
        Geography selectedGeo = SelectedGeography;

        if (selectedGeo != null)
        {
            this.DropGeographies.Items[0].Text = selectedGeo.Name;
        }
        this.Tree.OnClientNodeClicking = this.ClientID + "_OnClientNodeClick";
    }


    public void Tree_SelectedNodeChanged (object sender, EventArgs args)
    {
        if (SelectedNodeChanged != null)
        {
            SelectedNodeChanged(this, args);
        }

        this.DropGeographies.Items[0].Text = SelectedGeography.Name;
    }

    public Controls_v4_GeographyTree Tree
    {
        get
        {
            return (Controls_v4_GeographyTree)this.DropGeographies.Items[0].FindControl("GeographyTree");
        }

    }

    public Geography SelectedGeography
    {
        get
        {
            Controls_v4_GeographyTree tree = (Controls_v4_GeographyTree)this.DropGeographies.Items[0].FindControl("GeographyTree");

            return tree.SelectedGeography;
        }
        set
        {
            Controls_v4_GeographyTree tree = (Controls_v4_GeographyTree)this.DropGeographies.Items[0].FindControl("GeographyTree");

            tree.SelectedGeography = value;
            if (value != null)
            {
                this.DropGeographies.Items[0].Text = value.Name;
            }
            else
            {
                this.DropGeographies.Items[0].Text = "";
            }
        }
    }


    public Geographies Roots
    {
        set
        {
            Controls_v4_GeographyTree tree = (Controls_v4_GeographyTree)this.DropGeographies.Items[0].FindControl("GeographyTree");

            tree.Roots = value;
            SelectedGeography = value[0];
            this.DropGeographies.Items[0].Text = SelectedGeography.Name;
        }
    }

    public Geography Root
    {
        set
        {
            Roots = Geographies.FromSingle(value);
        }
    }

    public bool AutoPostBack
    {
        get { return this.DropGeographies.AutoPostBack; }
        set { this.DropGeographies.AutoPostBack = value; }
    }

    public bool Enabled
    {
        get { return this.DropGeographies.Enabled; }
        set { this.DropGeographies.Enabled = value; }
    }

    public event EventHandler SelectedNodeChanged;

}
