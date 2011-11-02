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

public partial class Pages_v4_ViewBalanceSheet : PageV4Base
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

        PopulateDebugInfo();
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

    protected void GridBudgetAccounts_ItemDataBound(object sender, GridItemEventArgs e)
    {
        CreateExpandCollapseButton(e.Item, "Name");
    }

    public void CreateExpandCollapseButton(GridItem item, string columnUniqueName)
    {
        if (item is GridDataItem)
        {
            if (item.FindControl("MyExpandCollapseButton") == null)
            {
                Button button = new Button();
                button.Click += new EventHandler(button_Click);
                button.CommandName = "ExpandCollapse";
                button.CssClass = (item.Expanded) ? "rgCollapse" : "rgExpand";
                button.ID = "MyExpandCollapseButton";

                if (item.OwnerTableView.HierarchyLoadMode == GridChildLoadMode.Client)
                {
                    string script = String.Format(@"$find(""{0}"")._toggleExpand(this, event); return false;", item.Parent.Parent.ClientID);

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


    private Dictionary<int,DebugInfoLine> debugLookup;


    private void PopulateDebugInfo()
    {
        debugLookup = new Dictionary<int, DebugInfoLine>();

        ExpenseClaims expenseClaims = ExpenseClaims.FromOrganization(Organization.PPSE);
        InboundInvoices inboundInvoices = InboundInvoices.ForOrganization(Organization.PPSE);
        Salaries salaries = Salaries.ForOrganization(Organization.PPSE);
        Payouts payouts = Payouts.ForOrganization(Organization.PPSE);

        debugLookup[Organization.PPSE.FinancialAccounts.DebtsExpenseClaims.Identity] = new DebugInfoLine();
        debugLookup[Organization.PPSE.FinancialAccounts.DebtsInboundInvoices.Identity] = new DebugInfoLine();
        debugLookup[Organization.PPSE.FinancialAccounts.CostsAllocatedFunds.Identity] = new DebugInfoLine();
        debugLookup[Organization.PPSE.FinancialAccounts.DebtsSalary.Identity] = new DebugInfoLine();
        debugLookup[Organization.PPSE.FinancialAccounts.DebtsTax.Identity] = new DebugInfoLine();

        foreach (Payout payout in payouts)
        {
            foreach (ExpenseClaim claim in payout.DependentExpenseClaims)
            {
                debugLookup[Organization.PPSE.FinancialAccounts.DebtsExpenseClaims.Identity].Payouts -= claim.Amount;
            }

            foreach (InboundInvoice invoice in payout.DependentInvoices)
            {
                debugLookup[Organization.PPSE.FinancialAccounts.DebtsInboundInvoices.Identity].Payouts -= (decimal) invoice.Amount;
            }

            foreach (Salary salary in payout.DependentSalariesNet)
            {
                debugLookup[Organization.PPSE.FinancialAccounts.DebtsSalary.Identity].Payouts -= salary.NetSalaryDecimal;
            }

            foreach (Salary salary in payout.DependentSalariesTax)
            {
                debugLookup[Organization.PPSE.FinancialAccounts.DebtsTax.Identity].Payouts -= salary.TaxTotalDecimal;
            }
        }


        foreach (ExpenseClaim claim in expenseClaims)
        {
            if (claim.Open)
            {
                AddExpenseToDebug(claim);
            }
        }

        foreach (InboundInvoice invoice in inboundInvoices)
        {
            AddInboundInvoiceToDebug(invoice);
        }

        foreach (Salary salary in salaries)
        {
            AddSalaryNetToDebug(salary);
            AddSalaryTaxToDebug(salary);
        }

        List<DebugInfoLine> debugInfoLines = new List<DebugInfoLine>();

        foreach (int accountId in debugLookup.Keys)
        {
            debugLookup[accountId].AccountId = accountId;
            debugLookup[accountId].Actual = FinancialAccount.FromIdentity(accountId).GetDelta(new DateTime(2006, 1, 1), DateTime.Today.AddDays(2)); // two days to account for all possible time zones
        }

        foreach (DebugInfoLine line in debugLookup.Values)
        {
            debugInfoLines.Add(line);
        }

        this.GridDebug.DataSource = debugInfoLines;
    }

    private void AddInboundInvoiceToDebug(InboundInvoice invoice)
    {
        debugLookup[Organization.PPSE.FinancialAccounts.DebtsInboundInvoices.Identity].Invoices -= invoice.Amount;
    }

    private void AddSalaryNetToDebug(Salary salary)
    {
        debugLookup[Organization.PPSE.FinancialAccounts.DebtsSalary.Identity].Salaries -= salary.NetSalaryDecimal;
    }

    private void AddSalaryTaxToDebug(Salary salary)
    {
        debugLookup[Organization.PPSE.FinancialAccounts.DebtsTax.Identity].Salaries -= salary.TaxTotalDecimal;
    }

    private void AddExpenseToDebug(ExpenseClaim claim)
    {
        int accountId = Organization.PPSE.FinancialAccounts.DebtsExpenseClaims.Identity;

        if (!claim.Claimed)
        {
            accountId = Organization.PPSE.FinancialAccounts.CostsAllocatedFunds.Identity;
        }

        debugLookup[accountId].Expenses -= claim.Amount;
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

        FinancialAccounts balanceAccounts = new FinancialAccounts();
        foreach (FinancialAccount account in allAccounts)
        {
            if (account.AccountType == FinancialAccountType.Asset ||
                account.AccountType == FinancialAccountType.Debt)
            {
                balanceAccounts.Add(account);
            }
        }

        // Add main headers

        this.GridBudgetAccounts.DataSource = balanceAccounts;

        PopulateLookups(balanceAccounts);
    }


    // This function is a bit of black magic.

    // It makes sure that all the accounts are populated with their respective values. In order to
    // summarize the values for every subtree, it 1) sorts the accounts in an order so that a
    // parents always comes before a child, 2) calculates the values for all accounts,
    // 3) iterates in reverse order and adds every account's values to the parent account, if there is one.


    private void PopulateLookups(FinancialAccounts accounts)
    {
        balanceInboundLookup = new Dictionary<int, decimal>();
        diffQuarterLookups = new Dictionary<int, decimal> [4];

        for (int quarterIndex = 0; quarterIndex < 4; quarterIndex++ )
        {
            diffQuarterLookups[quarterIndex] = new Dictionary<int, decimal>();
        }

        balanceOutboundLookup = new Dictionary<int, decimal>();

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

            balanceInboundLookup [account.Identity] = account.GetDelta(new DateTime(2006, 1, 1), new DateTime(year, 1, 1));

            // Find quarter diffs

            for (int quarter = 0; quarter < 4; quarter++)
            {
                diffQuarterLookups[quarter][account.Identity] = account.GetDelta(quarterBoundaries[quarter], quarterBoundaries[quarter+1]);
            }

            // Find outbound

            balanceOutboundLookup [account.Identity] = account.GetDelta(new DateTime(2006, 1, 1), new DateTime(year + 1, 1, 1));
        }

        // 3) Add all children's values to parents

        AddChildrenValuesToParents(balanceInboundLookup, accounts);
        AddChildrenValuesToParents(diffQuarterLookups[0], accounts);
        AddChildrenValuesToParents(diffQuarterLookups[1], accounts);
        AddChildrenValuesToParents(diffQuarterLookups[2], accounts);
        AddChildrenValuesToParents(diffQuarterLookups[3], accounts);
        AddChildrenValuesToParents(balanceOutboundLookup, accounts);

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


    private Dictionary<int, decimal> balanceInboundLookup;
    private Dictionary<int, decimal> balanceOutboundLookup;
    private Dictionary<int, decimal> [] diffQuarterLookups;
    

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
            FinancialAccount account = (FinancialAccount)e.Item.DataItem;

            if (account == null)
            {
                return;
            }

            int year = DateTime.Today.Year;

            HyperLink editLink = (HyperLink)e.Item.FindControl("ManageLink");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("alert ('There are no properties for a balance account.');",
                                                           account.FinancialAccountId, e.Item.ItemIndex);

            Label labelAccountName = (Label)e.Item.FindControl("LabelAccountName");
            Label labelInbound = (Label)e.Item.FindControl("LabelInbound");
            Label labelQ1 = (Label)e.Item.FindControl("LabelDiffQ1");
            Label labelQ2 = (Label)e.Item.FindControl("LabelDiffQ2");
            Label labelQ3 = (Label)e.Item.FindControl("LabelDiffQ3");
            Label labelQ4 = (Label)e.Item.FindControl("LabelDiffQ4");
            Label labelOutbound = (Label)e.Item.FindControl("LabelOutbound");

            labelInbound.Text = balanceInboundLookup [account.Identity].ToString("N0", new CultureInfo("sv-SE"));
            labelOutbound.Text = balanceOutboundLookup [account.Identity].ToString("N0", new CultureInfo("sv-SE"));
            labelQ1.Text = diffQuarterLookups[0][account.Identity].ToString("N0", new CultureInfo("sv-SE"));
            labelQ2.Text = diffQuarterLookups[1][account.Identity].ToString("N0", new CultureInfo("sv-SE"));
            labelQ3.Text = diffQuarterLookups[2][account.Identity].ToString("N0", new CultureInfo("sv-SE"));
            labelQ4.Text = diffQuarterLookups[3][account.Identity].ToString("N0", new CultureInfo("sv-SE"));
            labelAccountName.Text = account.Name;
        }
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



    public class DebugInfoLine
    {
        public int AccountId { get { return this.accountId; } set { this.accountId = value; } }
        public string AccountName { get { return FinancialAccount.FromIdentity(AccountId).Name; } }
        public decimal Expenses { get { return this.expenses; } set { this.expenses = value; } }
        public decimal Invoices { get { return this.invoices; } set { this.invoices = value; } }
        public decimal Salaries { get { return this.salaries; } set { this.salaries = value; } }
        public decimal Payouts { get { return this.payouts; } set { this.payouts = value; } }
        public decimal ExpectedTotal { get { return (decimal) (Expenses + (decimal) Invoices + (decimal) Salaries + Payouts); } }
        public decimal Actual { get { return this.actual; } set { this.actual = value; } }
        public decimal Diff { get { return Actual - ExpectedTotal; } }

        private int accountId;
        private decimal expenses;
        private decimal salaries;
        private decimal payouts;
        private decimal invoices;
        private decimal actual;
    }
}