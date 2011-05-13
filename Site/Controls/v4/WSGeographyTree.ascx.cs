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
using System.Web.Services;
using System.Web.Script.Services;
using Activizr.Logic.Cache;
using Activizr.Basic.Types;


public partial class Controls_v4_WSGeographyTree : System.Web.UI.UserControl
{
    public RadTreeView tree
    {
        get { return Tree; }
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

    public int RootId
    {
        get
        {
            return Root.Identity;
        }

        set
        {
            if (root == null || root.Identity != value)
            {
                Root = Geography.FromIdentity(value);
            }
        }
    }

    protected void Page_Load (object sender, EventArgs e)
    {

    }

    private RadTreeViewEventHandler selectedNodeChanged;

    public RadTreeViewEventHandler SelectedNodeChanged
    {
        get
        {
            return selectedNodeChanged;
        }
        set
        {
            if (value != selectedNodeChanged)
            {
                if (selectedNodeChanged != null)
                {
                    Tree.NodeClick -= selectedNodeChanged;
                    selectedNodeChanged = null;
                }
                if (value != null)
                {
                    Tree.NodeClick += value;
                    selectedNodeChanged = value;
                }
            }

        }
    }

    public RadTreeNode EnsureGeographyLoaded (int geoToLoad, int expandTree)
    {
        try
        {
            RadTreeNode nod = Tree.FindNodeByValue(geoToLoad.ToString());
            if (nod != null)
            {
                //Already loaded, just return it.
                if (expandTree == 1)
                    nod.ExpandParentNodes();
                return nod;
            }

            //get the line to make sure all nodes down to the selected are loaded;
            BasicGeography[] line = GeographyCache.GetGeographyLine(geoToLoad);

            if (Tree.Nodes.Count == 0)
            {
                // Tree is empty, insert the root.
                Tree.Nodes.Add(new RadTreeNode(this.Root.Name, this.Root.Identity.ToString()));
            }
            BasicGeography parentbg = null;
            RadTreeNodeCollection parentCollection = null;
            nod = Tree.Nodes[0];
            foreach (BasicGeography bg in line)
            {
                nod = Tree.FindNodeByValue(bg.Identity.ToString());
                if (parentbg == null)
                {
                    if (nod != null)
                    {
                        parentbg = bg;
                        parentCollection = nod.Nodes;
                    }
                }
                else
                {
                    // if node found that parent is already loaded
                    if (nod == null)
                    {
                        AddChildren(parentCollection, parentbg.Identity);
                        nod = Tree.FindNodeByValue(bg.Identity.ToString());
                        nod.ParentNode.ExpandMode = TreeNodeExpandMode.ClientSide;
                    }
                    nod.ParentNode.Expanded = (expandTree == 0) ? nod.ParentNode.Expanded : (expandTree < 0) ? false : true;
                    parentbg = bg;
                    parentCollection = nod.Nodes;
                }
            }
            Tree.Nodes[0].Expanded = (expandTree == 0) ? Tree.Nodes[0].Expanded : (expandTree < 0) ? false : true; ;
            return nod;
        }
        catch
        {
            return null;
        }
    }
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
            RadTreeNode nod = EnsureGeographyLoaded(value.Identity, 1 );
            if (nod != null)
            {
                nod.Selected = true;
            }
            else
            {
                Root = Geography.Root;
                nod = EnsureGeographyLoaded(value.Identity,1 );
                if (nod != null)
                {
                    nod.Selected = true;
                }
            }
        }
    }

    public int SelectedGeographyId
    {
        get
        {
            if (SelectedGeography != null)
                return SelectedGeography.Identity;
            else
                return 0;
        }
        set
        {
            SelectedGeography = Geography.FromIdentity(value);
        }
    }

    public bool CheckBoxes
    {
        get
        {
            return Tree.CheckBoxes = true;
        }
        set
        {
            Tree.CheckBoxes = value;
        }
    }


    public int[] CheckedValues
    {
        get
        {
            List<int> res = new List<int>();
            foreach (RadTreeNode node in Tree.CheckedNodes)
            {
                res.Add(int.Parse(node.Value));
            }
            return res.ToArray();
        }
        set
        {
            Tree.ClearCheckedNodes();
            foreach (int v in value)
            {
                RadTreeNode found = EnsureGeographyLoaded(v, 1);
                if (found != null && found.Checkable)
                {
                    found.Checked = true;
                }
            }
        }
    }

    public void SetNodeStyles (int[] nodeValues, Style s)
    {
        foreach (int v in nodeValues)
        {
            RadTreeNode found = EnsureGeographyLoaded(v, 0);
            if (found != null)
            {
                found.ApplyStyle(s);
                found.Style["padding-top"] = "0px";
                found.Style["padding-left"] = "2px";
                found.Style["padding-bottom"] = "0px";
                found.Style["padding-right"] = "2px";
                found.Style["margin-top"] = "3px";
                found.Style["margin-left"] = "0px";
                found.Style["margin-bottom"] = "3px";
                found.Style["margin-right"] = "0px";
            }
        }
    }



    private void Populate (int levels)
    {
        if (root == null)
        {
            root = Geography.Root;
        }

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

        Tree.Nodes.Add(RecursiveAdd(lookup, geos[0].ParentIdentity, levels - 1)[0]);
        Tree.Nodes[0].Expanded = true;
        Tree.Nodes[0].Selected = true;
    }

    private void AddChildren (RadTreeNodeCollection coll, int parentIdentity)
    {
        BasicGeography[] geos = GeographyCache.GetGeographyChildren(parentIdentity);
        foreach (BasicGeography geo in geos)
        {
            RadTreeNode node = new RadTreeNode(geo.Name, geo.Identity.ToString());
            node.ExpandMode = TreeNodeExpandMode.ClientSide;
            coll.Add(node);
            if ((GeographyCache.CountGeographyChildren(geo.Identity)) > 0)
            {
                node.ExpandMode = TreeNodeExpandMode.WebService;
            }
            node.Expanded = node.ParentNode.Expanded;
        }
    }


    private RadTreeNodeCollection RecursiveAdd (Dictionary<int, Geographies> lookup, int parentIdentity, int levels)
    {

        RadTreeNodeCollection result = new RadTreeNodeCollection(this.Tree);

        foreach (Geography geo in lookup[parentIdentity])
        {
            RadTreeNode node = new RadTreeNode(geo.Name, geo.Identity.ToString());

            if (lookup.ContainsKey(geo.Identity) && (levels > 0))
            {
                RadTreeNodeCollection collection = RecursiveAdd(lookup, geo.Identity, levels - 1);

                foreach (RadTreeNode subnode in collection)
                {
                    node.Nodes.Add(subnode);
                }
            }

            if (levels > 0)
                node.Expanded = true;
            if (levels < 1)
                node.ExpandMode = TreeNodeExpandMode.WebService;
            result.Add(node);
        }

        return result;
    }



    public Geography Root
    {
        get
        {
            if (this.root == null)
            {
                return Geography.Root;
            }
            else
                return root;
        }

        set
        {
            if (this.root == null || this.root.Identity != value.Identity)
            {
                this.root = value;

                if (Tree.Nodes.Count > 0)
                {
                    Tree.Nodes.Clear();
                    Populate(2);
                }
            }
        }
    }

    public string OnClientNodeClicking
    {
        set { Tree.OnClientNodeClicking = value; }
        get { return Tree.OnClientNodeClicking; }
    }


    public string ParentClientID
    {
        set { this.parentClientID = value; }
        get { return this.parentClientID; }
    }


    private string parentClientID;

    private Geography root;

    protected void Tree_PreRender (object sender, EventArgs e)
    {
        if (Tree.Nodes.Count == 0)
        {
            this.root = Geography.Root;
            Populate(3);
        }
    }

    public static void EmitScripts (Control page)
    {
        ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "GeoTree_NodeClicking",
        @"
; function GeoTree_NodeClicking(sender, args) {
            // ugly hack to get this to work in FF
            if (typeof (DropOrganizations_OnClientNodeClicking) == 'function')
                DropGeographies_OnClientNodeClicking(sender, args);
        };
", true);
    }

}