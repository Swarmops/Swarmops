using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Logic.Cache;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Telerik.Web.UI;


public partial class Controls_v4_WSOrgTree : System.Web.UI.UserControl
{
    public int topSortCountryId=0;
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
                Root = Organization.FromIdentity(value);
            }
        }
    }



    protected void Page_Load (object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Populate(4);
        }
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


    public Organization SelectedOrganization
    {
        get
        {

            string selectedIdentity = Tree.SelectedValue;

            if (String.IsNullOrEmpty(selectedIdentity))
            {
                return null;
            }

            return Organization.FromIdentity(Int32.Parse(selectedIdentity));
        }
        set
        {
            try
            {
                //get the line to make sure all nodes down to the selected are loaded;
                BasicOrganization[] line = OrganizationCache.GetOrganizationLine(value.Identity);

                if (Tree.Nodes.Count == 0)
                {
                    // Tree is empty, insert the root.
                    RadTreeNode rootNode = new RadTreeNode(this.Root.Name, this.Root.Identity.ToString());
                    SetAuthorityForNode(rootNode);
                    Tree.Nodes.Add(rootNode);
                }

                BasicOrganization parentbg = (BasicOrganization)this.Root;
                RadTreeNodeCollection parentCollection = Tree.Nodes[0].Nodes;

                foreach (BasicOrganization bg in line)
                {
                    RadTreeNode nod = Tree.FindNodeByValue(bg.Identity.ToString());
                    // if node found that parent is already loaded
                    if (nod == null)
                    {
                        AddChildren(parentCollection, parentbg.Identity);
                        nod = Tree.FindNodeByValue(bg.Identity.ToString());
                        nod.Expanded = true;
                        nod.Selected = true;
                    }
                    parentbg = bg;
                    parentCollection = nod.Nodes;
                }
                Tree.Nodes[0].Expanded = true;

            }
            catch
            {
                //nada
            }
        }
    }

    private Authority authority = null;
    private Permission requiredPermission = Permission.Undefined;

    public void SetAuthority (Authority pAuthority, Permission pRequiredPermission)
    {
        authority = pAuthority;
        requiredPermission = pRequiredPermission;
        foreach (RadTreeNode nod in Tree.GetAllNodes())
        {
            SetAuthorityForNode(nod);
        }

    }

    private void SetAuthorityForNode (RadTreeNode nod)
    {
        if (authority != null)
        {
            nod.Attributes["uid"] = authority.PersonId.ToString();
            nod.Attributes["perm"] = requiredPermission.ToString();
            int org = int.Parse(nod.Value);
            if (!authority.HasPermission(requiredPermission, org, -1, Authorization.Flag.AnyGeography))
                nod.CssClass = "nonAccessNode";
        }
    }

    public int SelectedOrganizationId
    {
        get
        {
            if (SelectedOrganization != null)
                return SelectedOrganization.Identity;
            else
                return 0;
        }
        set
        {
            SelectedOrganization = Organization.FromIdentity(value);
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
                RadTreeNode found = VerifyValueLoaded(v);
                if (found != null && found.Checkable)
                {
                    found.Checked = true;
                }
            }
        }
    }

    private RadTreeNode VerifyValueLoaded (int v)
    {
        RadTreeNode node = Tree.FindNodeByValue(v.ToString());
        SelectedOrganizationId = v;
        node = Tree.FindNodeByValue(v.ToString());
        return node;
    }




    private void Populate (int levels)
    {
        if (root == null)
        {
            root = Organization.Root;
        }

        Organizations orgs = root.GetTree();


        // We need a real fucking tree structure.

        Dictionary<int, Organizations> lookup = new Dictionary<int, Organizations>();

        foreach (Organization org in orgs)
        {
            if (!lookup.ContainsKey(org.ParentIdentity))
            {
                lookup[org.ParentIdentity] = new Organizations();
            }

            lookup[org.ParentIdentity].Add(org);
        }
        //Sort rootlevel
        if (lookup.ContainsKey(Organization.RootIdentity))
        {
            lookup[Organization.RootIdentity].Sort(
                delegate(Organization o1, Organization o2)
                {
                    if (o1.DefaultCountryId != topSortCountryId && o2.DefaultCountryId != topSortCountryId)
                        return 0;
                    else if (o1.DefaultCountryId != topSortCountryId && o2.DefaultCountryId == topSortCountryId)
                        return 1;
                    else if (o1.DefaultCountryId == topSortCountryId && o2.DefaultCountryId != topSortCountryId)
                        return -1;
                    else
                        return 0;
                });
        }


        Tree.Nodes.Add(RecursiveAdd(lookup, orgs[0].ParentIdentity, levels - 1)[0]);
        Tree.Nodes[0].Expanded = true;
        Tree.Nodes[0].Selected = true;
    }

    private void AddChildren (RadTreeNodeCollection coll, int parentIdentity)
    {
        BasicOrganization[] orgs = OrganizationCache.GetOrganizationChildren(parentIdentity);
        foreach (BasicOrganization org in orgs)
        {
            RadTreeNode node = new RadTreeNode(org.Name, org.Identity.ToString());
            SetAuthorityForNode(node);
            node.ExpandMode = TreeNodeExpandMode.WebService;
            coll.Add(node);
        }

    }


    private RadTreeNodeCollection RecursiveAdd (Dictionary<int, Organizations> lookup, int parentIdentity, int levels)
    {

        RadTreeNodeCollection result = new RadTreeNodeCollection(this.Tree);

        foreach (Organization org in lookup[parentIdentity])
        {
            RadTreeNode node = new RadTreeNode(org.Name, org.Identity.ToString());
            SetAuthorityForNode(node);

            if (lookup.ContainsKey(org.Identity) && (levels > 0))
            {
                RadTreeNodeCollection collection = RecursiveAdd(lookup, org.Identity, levels - 1);

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



    public Organization Root
    {
        get
        {
            if (this.root == null)
            {
                return Organization.Root;
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
                }
                Populate(3);
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

    private Organization root;


    public static void EmitScripts (Control page)
    {
        ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "OrgTree_NodeClicking",
        @"
;function OrgTree_NodeClicking(sender, args) {
            // ugly hack to get this to work in FF
            if (typeof (DropOrganizations_OnClientNodeClicking) == 'function')
                DropOrganizations_OnClientNodeClicking(sender, args);
        };
", true);
    }

}
