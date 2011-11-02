using System;
using System.Collections;
using System.Collections.Generic;
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


public partial class Controls_v4_GeographyTree : System.Web.UI.UserControl
{
    protected void Page_Load (object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Tree.Nodes.Count == 0)
            {
                Populate();
            }

            if (!String.IsNullOrEmpty(this.onClientNodeClicking))
            {
                Tree.OnClientNodeClicking = this.onClientNodeClicking;
            }
        }
    }

    public void Tree_NodeClick (object sender, RadTreeNodeEventArgs e)
    {
        if (SelectedNodeChanged != null)
        {
            SelectedNodeChanged(this, new EventArgs());
        }
    }

    public event EventHandler SelectedNodeChanged;

    public Geography SelectedGeography
    {
        get
        {
            string selectedIdentity = Tree.SelectedValue;

            if (String.IsNullOrEmpty(selectedIdentity))
            {
                return null;
            }

            return Geography.FromIdentity(Int32.Parse(selectedIdentity));
        }
        set
        {
            if (value != null)
            {
                RadTreeNode node = Tree.FindNodeByValue(value.Identity.ToString());
                if (node != null)
                {
                    node.Selected = true;
                }
            }
            else if (Tree.SelectedNode != null)
            {
                while (Tree.SelectedNode != null)
                {
                    Tree.SelectedNode.Selected = false;
                }
            }

        }
    }

    private void Populate ()
    {
        Geography wasSelectedGeo = this.SelectedGeography;

        Tree.Nodes.Clear();

        if (roots == null)
        {
            roots = Geographies.FromSingle(Geography.Root);
        }

        RadTreeNode topNode = new RadTreeNode("", "");


        foreach (Geography root in roots)
        {
            Geographies geos = root.GetTree();

            // We need a real fucking tree structure.

            Dictionary<int, Geographies> lookup = new Dictionary<int, Geographies>();

            foreach (Geography geo in geos)
            {
                if (!lookup.ContainsKey(geo.ParentIdentity))
                {
                    lookup[geo.ParentIdentity] = new Geographies();
                }

                lookup[geo.ParentIdentity].Add(geo);
            }
            topNode.Nodes.Add(RecursiveAdd(lookup, geos[0].ParentIdentity)[0]);
        }

        //Re-added selection of first node to avoid crash in alert activists /JL 2010-12-21
        if (roots.Count > 1)
        {
            //need the dummy root
            Tree.Nodes.Add(topNode);
            topNode.Enabled = false;
            topNode.Expanded = true;
            topNode.Nodes[0].Selected = true;

        }
        else
        {
            Tree.Nodes.Add(topNode.Nodes[0]);
            Tree.Nodes[0].Selected = true;
        }

        if (wasSelectedGeo != null)
            SelectedGeography = wasSelectedGeo;

        Tree.Nodes[0].Expanded = true;

    }


    private RadTreeNodeCollection RecursiveAdd (Dictionary<int, Geographies> lookup, int parentIdentity)
    {
        RadTreeNodeCollection result = new RadTreeNodeCollection(this.Tree);

        foreach (Geography geo in lookup[parentIdentity])
        {
            RadTreeNode node = new RadTreeNode(geo.Name, geo.Identity.ToString());

            if (lookup.ContainsKey(geo.Identity))
            {
                RadTreeNodeCollection collection = RecursiveAdd(lookup, geo.Identity);

                foreach (RadTreeNode subnode in collection)
                {
                    node.Nodes.Add(subnode);
                }
            }

            result.Add(node);
        }

        return result;
    }



    public Geographies Roots
    {
        set
        {
            this.roots = value;
            Populate();
        }
    }

    public Geography Root
    {
        set
        {
            Roots = Geographies.FromSingle(value);
        }
    }

    public string OnClientNodeClicking
    {
        set { this.onClientNodeClicking = value; }
        get { return this.onClientNodeClicking; }
    }


    public string ParentClientID
    {
        set { this.parentClientID = value; }
        get { return this.parentClientID; }
    }


    private string parentClientID;


    private string onClientNodeClicking;


    private Geographies roots;

}