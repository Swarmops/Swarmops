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
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Structure;

public partial class Controls_v4_FinancialAccountTreeDropDown : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (SelectedFinancialAccount != null)
        {
            this.DropFinancialAccounts.Items[0].Text = SelectedFinancialAccount.Name;
        }
        else
        {
            this.DropFinancialAccounts.Items[0].Text = "Select account...";
        }
    }



    public void Tree_SelectedNodeChanged(object sender, EventArgs args)
    {
        if (SelectedNodeChanged != null)
        {
            SelectedNodeChanged(this, args);
        }

        this.DropFinancialAccounts.Items[0].Text = SelectedFinancialAccount.Name;
    }


    public FinancialAccount SelectedFinancialAccount
    {
        get
        {
            Controls_v4_FinancialAccountTree tree = (Controls_v4_FinancialAccountTree)this.DropFinancialAccounts.Items[0].FindControl("FinancialAccountTree");

            return tree.SelectedFinancialAccount;
        }
        set
        {
            Controls_v4_FinancialAccountTree tree = (Controls_v4_FinancialAccountTree)this.DropFinancialAccounts.Items[0].FindControl("FinancialAccountTree");
            tree.SelectedFinancialAccount = value;

            this.DropFinancialAccounts.Items[0].Text = value.Name;
        }
    }


    public void Populate(Organization organization, FinancialAccountType accountType)
    {
        Controls_v4_FinancialAccountTree tree = (Controls_v4_FinancialAccountTree)this.DropFinancialAccounts.Items[0].FindControl("FinancialAccountTree");

        tree.Populate(organization, accountType);
    }

    public void Populate(FinancialAccount root)
    {
        Controls_v4_FinancialAccountTree tree = (Controls_v4_FinancialAccountTree)this.DropFinancialAccounts.Items[0].FindControl("FinancialAccountTree");

        tree.Populate(root);
    }

    public bool Enabled
    {
        get { return this.DropFinancialAccounts.Enabled; }
        set { this.DropFinancialAccounts.Enabled = value; }
    }

    public event EventHandler SelectedNodeChanged;
}
