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


public partial class Controls_v4_OrganizationTree : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
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

            //Tree.NodeClick += new RadTreeViewEventHandler (Tree_NodeClick);

            Tree.Nodes.Add(RecursiveAdd(lookup, orgs[0].ParentIdentity)[0]);
            Tree.Nodes[0].Expanded = true;
       
            if (!String.IsNullOrEmpty(this.onClientNodeClicking))
            {
                Tree.OnClientNodeClicking = this.onClientNodeClicking;
            }
        }
    }


    public void Tree_NodeClick(object sender, RadTreeNodeEventArgs e)
    {
        // Fire SelectedNodeChanged. This is a bit of a cheat since we don't really know that
        // the node has changed when the user clicks on a node - but the same behavior needs
        // to be triggered anyway.

        if (this.SelectedNodeChanged != null)
        {
            SelectedNodeChanged (this, new EventArgs());
        }
    }


    private RadTreeNodeCollection RecursiveAdd (Dictionary<int,Organizations> lookup, int parentIdentity)
    {
        RadTreeNodeCollection result = new RadTreeNodeCollection(this.Tree);

        foreach (Organization org in lookup[parentIdentity])
        {
            RadTreeNode node = new RadTreeNode(org.NameShort, org.Identity.ToString());

            if (lookup.ContainsKey(org.Identity))
            {
                RadTreeNodeCollection collection = RecursiveAdd(lookup, org.Identity);

                foreach (RadTreeNode subnode in collection)
                {
                    node.Nodes.Add(subnode);
                }
            }

            result.Add(node);
        }

        return result;
    }


    public event EventHandler SelectedNodeChanged;


    public Organization Root
    {
        set
        {
            this.root = value;
        }
    }

    public Organization SelectedOrganization
    {
        get
        {
            if (Tree.SelectedNode != null)
            {
                return Organization.FromIdentity(Int32.Parse(Tree.SelectedNode.Value));
            }
            else
            {
                return null;
            }
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


    private Organization root;

}
