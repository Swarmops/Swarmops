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
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Structure;
using Telerik.Web.UI;



public partial class Controls_v4_FinancialAccountTree : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void Tree_NodeClick(object sender, RadTreeNodeEventArgs e)
    {
        if (SelectedNodeChanged != null)
        {
            SelectedNodeChanged(this, new EventArgs());
        }
    }

    public event EventHandler SelectedNodeChanged;


    public FinancialAccount SelectedFinancialAccount
    {
        get
        {
            string selectedIdentity = Tree.SelectedValue;

            if (String.IsNullOrEmpty(selectedIdentity))
            {
                return null;
            }

            return FinancialAccount.FromIdentity(Int32.Parse(selectedIdentity));
        }
        set
        {
            if (Tree.SelectedNode != null)
            {
                Tree.SelectedNode.Selected = false;
            }
            Tree.FindNodeByValue(value.Identity.ToString()).Selected = true;
        }
    }


    public void Populate(Organization organization, FinancialAccountType accountType)
    {
        FinancialAccounts accounts = FinancialAccounts.ForOrganization(organization, accountType);

        Populate(accounts);
    }

    public void Populate(FinancialAccount root)
    {
        this.Tree.Nodes.Clear();
        FinancialAccounts accounts = root.GetTree();
        accounts.Remove(root);

        if (accounts.Count > 0)
        {
            Populate(accounts);
        }
    }

    private void Populate(FinancialAccounts accounts)
    {
        // We still need a real fucking tree structure.

        Dictionary<int, FinancialAccounts> lookup = new Dictionary<int, FinancialAccounts>();

        foreach (FinancialAccount account in accounts)
        {
            if (!lookup.ContainsKey(account.ParentIdentity))
            {
                lookup[account.ParentIdentity] = new FinancialAccounts();
            }

            lookup[account.ParentIdentity].Add(account);
        }

        RadTreeNodeCollection collection = RecursiveAdd(lookup, accounts[0].ParentIdentity);

        foreach (RadTreeNode subnode in collection)
        {
            Tree.Nodes.Add(subnode);
        }

        //Tree.Nodes[0].Expanded = true;
        //Tree.Nodes[0].Selected = true;
    }


    private RadTreeNodeCollection RecursiveAdd(Dictionary<int, FinancialAccounts> lookup, int parentIdentity)
    {
        RadTreeNodeCollection result = new RadTreeNodeCollection(this.Tree);

        foreach (FinancialAccount account in lookup[parentIdentity])
        {
            RadTreeNode node = new RadTreeNode(account.Name, account.Identity.ToString());

            if (lookup.ContainsKey(account.Identity))
            {
                RadTreeNodeCollection collection = RecursiveAdd(lookup, account.Identity);

                foreach (RadTreeNode subnode in collection)
                {
                    node.Nodes.Add(subnode);
                }
            }

            result.Add(node);
        }

        return result;
    }
}
