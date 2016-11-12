using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;

using Telerik.Web.UI;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

public partial class Pages_v4_ViewResultSheet : PageV4Base
{

    private static readonly int PPOrgId = Organization.PPSEid;

    protected void Page_Load (object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PopulateGrid();
        }

        if (Assembly.GetAssembly(typeof(ScriptManager)).FullName.IndexOf("3.5") != -1)
        {
            this.GridBudgetAccounts.MasterTableView.FilterExpression = @"it[""ParentFinancialAccountId""] = 0";
        }
        else
        {
            this.GridBudgetAccounts.MasterTableView.FilterExpression = "ParentFinancialAccountId=0";
        }

        FinancialAccounts testAccounts = FinancialAccounts.FromBankTransactionTag("BG 472-5107");

        if (testAccounts.Count != 1)
        {
            this.LabelTest.Text = testAccounts.Count.ToString() + " accounts returned";
        }

        this.LabelTest.Text = testAccounts[0].Name + " (" + testAccounts[0].AssignedGeography.Name + ")";
    }


    // Lots of stuff here copied from http://demos.telerik.com/aspnet-ajax/grid/examples/hierarchy/selfreferencing/defaultcs.aspx


    public void Page_PreRenderComplete(object sender, EventArgs e)
    {
        HideExpandColumnRecursive(this.GridBudgetAccounts.MasterTableView);
    }


    public void HideExpandColumnRecursive(GridTableView tableView)
    {
        GridItem[] nestedViewItems = tableView.GetItems(GridItemType.NestedView);
        foreach (GridNestedViewItem nestedViewItem in nestedViewItems)
        {
            foreach (GridTableView nestedView in nestedViewItem.NestedTableViews)
            {
                nestedView.Style["border"] = "0";

                Button MyExpandCollapseButton = (Button)nestedView.ParentItem.FindControl("MyExpandCollapseButton");
                if (nestedView.Items.Count == 0)
                {
                    if (MyExpandCollapseButton != null)
                    {
                        MyExpandCollapseButton.Style["visibility"] = "hidden";
                    }
                    nestedViewItem.Visible = false;
                }
                else
                {
                    if (MyExpandCollapseButton != null)
                    {
                        MyExpandCollapseButton.Style.Remove("visibility");
                    }
                }

                if (nestedView.HasDetailTables)
                {
                    HideExpandColumnRecursive(nestedView);
                }
            }
        }
    }

    protected void GridBudgetAccounts_ItemCommand(object source, GridCommandEventArgs e)
    {
        if (e.CommandName == RadGrid.ExpandCollapseCommandName)
        {
            if (e.Item is GridDataItem)
            {
                PopulateLine(e.Item);
            }
        }
    }

    protected void GridBudgetAccounts_ItemDataBound(object sender, GridItemEventArgs e)
    {
        CreateExpandCollapseButton(e.Item, "Name");
    }

    public void CreateExpandCollapseButton(GridItem item, string columnUniqueName)
    {
        if (item is GridDataItem)
        {
            FinancialAccount account = (FinancialAccount) item.DataItem;


            if (item.FindControl("MyExpandCollapseButton") == null)
            {
                Button button = new Button();
                button.Click += new EventHandler(button_Click);
                button.CommandName = "ExpandCollapse";
                button.CssClass = (item.Expanded) ? "rgCollapse" : "rgExpand";
                button.ID = "MyExpandCollapseButton";

                if (item.OwnerTableView.HierarchyLoadMode == GridChildLoadMode.Client)
                {
                    string script = String.Format(@"$find(""{0}"")._toggleExpand(this, event); ToggleLineMode('{1}'); return false;", item.Parent.Parent.ClientID, account.Identity);

                    button.OnClientClick = script;
                }

                int level = item.ItemIndexHierarchical.Split(':').Length;
                if (level > 1)
                {
                    button.Style["margin-left"] = level * 10 + "px";
                }

                TableCell cell = ((GridDataItem)item)[columnUniqueName];
                cell.Controls.Add(button);
                cell.Controls.Add(new LiteralControl("&nbsp;"));
                cell.Controls.Add(new LiteralControl(((GridDataItem)item).GetDataKeyValue(columnUniqueName).ToString()));
            }
        }
    }


    void button_Click(object sender, EventArgs e)
    {
        ((Button)sender).CssClass = (((Button)sender).CssClass == "rgExpand") ? "rgCollapse" : "rgExpand";
    }


    protected void GridBudgetAccounts_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
    {
        if (e.Column is GridExpandColumn)
        {
            e.Column.Visible = false;
        }
        else if (e.Column is GridBoundColumn)
        {
            e.Column.HeaderStyle.Width = Unit.Pixel(300);
        }
        else if (e.Column is GridTemplateColumn)
        {
            e.Column.HeaderStyle.Width = Unit.Pixel(50);
        }
    }


    protected void PopulateGrid ()
    {
        FinancialAccounts allAccounts = FinancialAccounts.ForOrganization(Organization.PPSE);
        Dictionary<int, bool> accountIsParent = new Dictionary<int, bool>();

        FinancialAccounts resultAccounts = new FinancialAccounts();
        foreach (FinancialAccount account in allAccounts)
        {
            if (account.AccountType == FinancialAccountType.Income ||
                account.AccountType == FinancialAccountType.Cost)
            {
                resultAccounts.Add(account);

                if (account.ParentIdentity != 0)
                {
                    
                }
            }
        }

        // Add expansion headers


        this.GridBudgetAccounts.DataSource = resultAccounts;

        PopulateLookups(resultAccounts);
    }


    // This function is a bit of black magic.

    // It makes sure that all the accounts are populated with their respective values. In order to
    // summarize the values for every subtree, it 1) sorts the accounts in an order so that a
    // parents always comes before a child, 2) calculates the values for all accounts,
    // 3) iterates in reverse order and adds every account's values to the parent account, if there is one.


    private void PopulateLookups(FinancialAccounts accounts)
    {
        singleLookups = new Dictionary<int, decimal>[6];
        treeLookups = new Dictionary<int, decimal>[6];

        for (int index = 0; index < 6; index++ )
        {
            treeLookups[index] = new Dictionary<int, decimal>();
            singleLookups[index] = new Dictionary<int, decimal>();
        }

        int year = 2010;

        DateTime[] quarterBoundaries = {
                                           new DateTime(year, 1, 1), new DateTime(year, 3, 1), new DateTime(year, 6, 1),
                                           new DateTime(year, 9, 1), new DateTime(year + 1, 1, 1)
                                       };

        // 1) Actually, the accounts are already sorted. Or are supposed to be, anyway,
        // since FinancialAccounts.ForOrganization gets the _tree_ rather than the flat list.

        // 2) Add all values to the accounts.

        foreach (FinancialAccount account in accounts)
        {
            // Find this year's inbound

            singleLookups [0][account.Identity] = account.GetDelta(new DateTime(year-1, 1, 1), new DateTime(year, 1, 1));

            // Find quarter diffs

            for (int quarter = 0; quarter < 4; quarter++)
            {
                singleLookups[quarter+1][account.Identity] = account.GetDelta(quarterBoundaries[quarter], quarterBoundaries[quarter+1]);
            }

            // Find outbound

            singleLookups [5][account.Identity] = account.GetDelta(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1));

            // copy to treeLookups

            for (int index=0; index < 6; index++)
            {
                treeLookups[index][account.Identity] = singleLookups[index][account.Identity];
            }
        }

        // 3) Add all children's values to parents

        for (int index = 0; index < 6; index++)
        {
            AddChildrenValuesToParents(treeLookups[index], accounts);
        }

        // Done.
    }


    private void AddChildrenValuesToParents (Dictionary<int,decimal> lookup, FinancialAccounts accounts)
    {
        // Iterate backwards and add any value to its parent's value, as they are sorted in tree order.

        for (int index = accounts.Count - 1; index >= 0; index--)
        {
            int parentFinancialAccountId = accounts[index].ParentFinancialAccountId;
            int accountId = accounts[index].Identity;

            if (parentFinancialAccountId != 0)
            {
                lookup[parentFinancialAccountId] += lookup[accountId];
            }
        }
    }


    private Dictionary<int, decimal>[] singleLookups;
    private Dictionary<int, decimal>[] treeLookups;
    

    protected void GridBudgetAccounts_ItemCreated (object sender, GridItemEventArgs e)
    {
        // CreateExpandCollapseButton(e.Item, "Name");

        if (e.Item is GridHeaderItem && e.Item.OwnerTableView != this.GridBudgetAccounts.MasterTableView)
        {
            e.Item.Style["display"] = "none";
        }

        if (e.Item is GridNestedViewItem)
        {
            e.Item.Cells[0].Visible = false;
        }

        if (e.Item is GridDataItem)
        {
            int year = DateTime.Today.Year;

            FinancialAccount account = (FinancialAccount)e.Item.DataItem;

            if (account == null)
            {
                return;
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("ManageLink");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowAccountForm('{0}','{1}');",
                                                           account.FinancialAccountId, e.Item.ItemIndex);

            PopulateLine(e.Item);
        }
    }

    protected void PopulateLine (GridItem item)
    {
        FinancialAccount account = (FinancialAccount)item.DataItem;

        if (account == null)
        {
            return;
        }

        Label labelAccountName = (Label)item.FindControl("LabelAccountName");
        Literal literalLastYear = (Literal)item.FindControl("LiteralLastYear");
        Literal literalQ1 = (Literal)item.FindControl("LiteralQuarter1");
        Literal literalQ2 = (Literal)item.FindControl("LiteralQuarter2");
        Literal literalQ3 = (Literal)item.FindControl("LiteralQuarter3");
        Literal literalQ4 = (Literal)item.FindControl("LiteralQuarter4");
        Literal literalYtd = (Literal)item.FindControl("LiteralThisYear");

        PopulateDualFigure(literalLastYear, "LastYear", 0, account.Identity);
        PopulateDualFigure(literalQ1, "Q1", 1, account.Identity);
        PopulateDualFigure(literalQ2, "Q2", 2, account.Identity);
        PopulateDualFigure(literalQ3, "Q3", 3, account.Identity);
        PopulateDualFigure(literalQ4, "Q4", 4, account.Identity);
        PopulateDualFigure(literalYtd, "Ytd", 5, account.Identity);
        labelAccountName.Text = account.Name;
    }

    private void PopulateDualFigure(Literal literal, string spanPrefix, int category, int accountId)
    {
        decimal treeValue = treeLookups[category][accountId];
        decimal singleValue = singleLookups[category][accountId];

        string span =
            string.Format(
                "<span ID=\"GridSpanExpanded{1}{0}\" style=\"display:none\">{2:N0}</span><span ID=\"GridSpanCollapsed{1}{0}\">{4}{3:N0}</span>",
                accountId, spanPrefix, singleValue, treeValue, singleValue != treeValue? "<b>&Sigma;</b>&nbsp;": string.Empty);
        literal.Text = span;
    }

    private void PopulateFigureCollapsed (Label label, int category, int accountId)
    {
        string prefix = string.Empty;

        decimal treeValue = treeLookups[category][accountId];
        decimal singleValue = singleLookups[category][accountId];

        if (treeValue != singleValue)
        {
            prefix = "Σ  ";
        }

        label.Text = prefix + treeValue.ToString("N0", new CultureInfo("sv-SE"));
    }

    /*
    protected void ButtonCreateTransaction_Click (object sender, EventArgs e)
    {

        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, PPOrgId))
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('You do not have access to financial records.');", true);
            return;
        }

        int accountId = Int32.Parse(this.DropAccountsCreate.SelectedValue);

        if (accountId == 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('Please select an account.');", true);
            return;
        }

        double amount = Double.Parse(this.TextAmountCreate.Text, new CultureInfo("sv-SE"));
        string description = this.TextDescriptionCreate.Text;
        DateTime transactionDateTime = (DateTime)this.DateCreate.SelectedDate;

        FinancialTransaction transaction = FinancialTransaction.Create(PPOrgId, transactionDateTime, description);
        transaction.AddRow(accountId, amount, _currentUser.Identity);

        // As the RadWindowManager and RadAjaxUpdate are part of the UpdatePanel we're rewriting, we
        // need to make the client call the function only when the ajax call has completed. We set
        // 200ms for this, but pretty much any amount of time should be ok, as long as it's delayed
        // past the actual ajax rewrite.

        ScriptManager.RegisterStartupScript(this, Page.GetType(), "finishthejob",
                                            "ShowTransactionFormDelayed (" + transaction.Identity + ");", true);
    }*/


    protected void RadAjaxManager1_AjaxRequest (object sender, AjaxRequestEventArgs e)
    {
        // TODO: There is a problem with re-getting the query parameters here -- the user 
        // may have changed the data in the web form, which will repopulate the grid 
        // with different data when the popup closes. This would be extremely confusing 
        // to the user. Is there  a good way to invisibly cache the query base 
        // (account, start date, end date)?


        if (e.Argument == "Rebind")
        {
            this.GridBudgetAccounts.MasterTableView.SortExpressions.Clear();
            this.GridBudgetAccounts.MasterTableView.GroupByExpressions.Clear();
            PopulateGrid();
            this.GridBudgetAccounts.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            this.GridBudgetAccounts.MasterTableView.SortExpressions.Clear();
            this.GridBudgetAccounts.MasterTableView.GroupByExpressions.Clear();
            this.GridBudgetAccounts.MasterTableView.CurrentPageIndex =
                this.GridBudgetAccounts.MasterTableView.PageCount - 1;
            PopulateGrid();
            this.GridBudgetAccounts.Rebind();
        }
    }
}