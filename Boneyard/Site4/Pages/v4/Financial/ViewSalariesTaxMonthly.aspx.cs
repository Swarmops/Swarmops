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

public partial class Pages_v4_ViewSalariesTaxMonthly : PageV4Base
{

    private static readonly int PPOrgId = Organization.PPSEid;
    private bool userHasChangeAuthority;

    protected void Page_Load (object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PopulateGrid();
        }

        if (Assembly.GetAssembly(typeof(ScriptManager)).FullName.IndexOf("3.5") != -1)
        {
            this.GridSalaryTaxData.MasterTableView.FilterExpression = @"it[""ParentIdentity""] = 0";
        }
        else
        {
            this.GridSalaryTaxData.MasterTableView.FilterExpression = "ParentIdentity=0";
        }

        userHasChangeAuthority = _authority.HasPermission(Permission.CanDoBudget, PPOrgId, -1,
                                                          Authorization.Flag.ExactOrganization);
    }


    // Lots of stuff here copied from http://demos.telerik.com/aspnet-ajax/grid/examples/hierarchy/selfreferencing/defaultcs.aspx


    public void Page_PreRenderComplete(object sender, EventArgs e)
    {
        HideExpandColumnRecursive(this.GridSalaryTaxData.MasterTableView);
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

    protected void GridSalaryTaxData_ItemDataBound(object sender, GridItemEventArgs e)
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


    void button_Click(object sender, EventArgs e)
    {
        ((Button)sender).CssClass = (((Button)sender).CssClass == "rgExpand") ? "rgCollapse" : "rgExpand";
    }


    protected void GridSalaryTaxData_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
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
        Salaries salaries = Salaries.ForOrganization(Organization.PPSE, true);
        List<GridRow> rows = new List<GridRow>();
        Dictionary<int, GridRow> sumRows = new Dictionary<int, GridRow>();

        foreach (Salary salary in salaries)
        {
            int monthKey = salary.PayoutDate.Year*100 + salary.PayoutDate.Month;

            rows.Add(new GridRow(salary.Identity, monthKey, salary.PayrollItem.PersonCanonical, salary.GrossSalaryCents / 100.0,
                                 salary.SubtractiveTaxCents / 100.0, salary.AdditiveTaxCents / 100.0));

            if (sumRows.ContainsKey(monthKey))
            {
                sumRows[monthKey].Add(salary.GrossSalaryCents / 100.0, salary.SubtractiveTaxCents / 100.0, salary.AdditiveTaxCents / 100.0);
            }
            else
            {
                sumRows[monthKey] = new GridRow(monthKey, 0,
                                                salary.PayoutDate.ToString("yyyy MMMM", CultureInfo.InvariantCulture),
                                                salary.GrossSalaryCents / 100.0, salary.SubtractiveTaxCents / 100.0, salary.AdditiveTaxCents / 100.0);
            }
        }

        foreach (int key in sumRows.Keys)
        {
            rows.Add(sumRows[key]);
        }


        this.GridSalaryTaxData.DataSource = rows;
    }



    protected void GridSalaryTaxData_ItemCreated (object sender, GridItemEventArgs e)
    {
        // CreateExpandCollapseButton(e.Item, "Name");

        if (e.Item is GridHeaderItem && e.Item.OwnerTableView != this.GridSalaryTaxData.MasterTableView)
        {
            e.Item.Style["display"] = "none";
        }

        if (e.Item is GridNestedViewItem)
        {
            e.Item.Cells[0].Visible = false;
        }


        if (e.Item is GridDataItem)
        {
            GridRow row = (GridRow) e.Item.DataItem;

            if (row == null)
            {
                return;
            }

            int year = DateTime.Today.Year;

            HyperLink editLink = (HyperLink)e.Item.FindControl("ManageLink");

            if (row.Identity > 200000)
            {
                editLink.Attributes["href"] = "ViewSalariesTaxMonthlyForm.aspx?Month=" + row.Identity.ToString();
            }
            else
            {
                editLink.Visible = false;
            }

            // Label labelLabel = (Label)e.Item.FindControl("LabelRowLabel");
            Label labelSalaryGross = (Label)e.Item.FindControl("LabelSalaryGross");
            Label labelSalaryGrossMain = (Label)e.Item.FindControl("LabelSalaryGrossMain");
            Label labelSalaryGrossMainTax = (Label)e.Item.FindControl("LabelSalaryGrossMainTax");
            Label labelSalaryDeducted = (Label)e.Item.FindControl("LabelSalaryDeducted");
            Label labelTaxmanTotal = (Label)e.Item.FindControl("LabelTaxmanTotal");

            // labelLabel.Text = row.Label;
            labelSalaryGross.Text = row.SalaryGross.ToString("N0");
            labelSalaryGrossMain.Text = row.SalaryGrossMain.ToString("N0");
            labelSalaryGrossMainTax.Text = row.SalaryGrossMainTax.ToString("N0");
            labelSalaryDeducted.Text = row.SubtractiveTax.ToString("N0");
            labelTaxmanTotal.Text = row.TaxTotal.ToString("N0");
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
            this.GridSalaryTaxData.MasterTableView.SortExpressions.Clear();
            this.GridSalaryTaxData.MasterTableView.GroupByExpressions.Clear();
            PopulateGrid();
            this.GridSalaryTaxData.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            this.GridSalaryTaxData.MasterTableView.SortExpressions.Clear();
            this.GridSalaryTaxData.MasterTableView.GroupByExpressions.Clear();
            this.GridSalaryTaxData.MasterTableView.CurrentPageIndex =
                this.GridSalaryTaxData.MasterTableView.PageCount - 1;
            PopulateGrid();
            this.GridSalaryTaxData.Rebind();
        }
    }


    protected class GridRow
    {

        public GridRow (int identity, int parentIdentity, string label, double grossSalary, double subtractive, double additive)
        {
            this.identity = identity;
            this.parentIdentity = parentIdentity;
            this.label = label;
            this.grossSalary = grossSalary;
            this.subtractive = subtractive;
            this.additive = additive;
        }

        public void Add (double grossSalary, double subtractive, double additive)
        {
            this.grossSalary += grossSalary;
            this.subtractive += subtractive;
            this.additive += additive;
        }

        private string label;
        private int identity;
        private int parentIdentity;
        private double grossSalary;
        private double subtractive;
        private double additive;

        public string Label { get { return this.label; }}
        public double SalaryGross { get { return this.grossSalary; } }
        public double SalaryGrossMain { get { return this.SalaryGross; } }
        public double SalaryGrossMainTax { get { return this.additive; } }
        public double SubtractiveTax { get { return this.subtractive; } }
        public double TaxTotal { get { return this.additive + this.subtractive; } }
        public int Identity { get { return this.identity; } }
        public int ParentIdentity { get{ return this.parentIdentity; } }
        public string Name { get { return this.Label; } }

        public double SalaryGrossOld { get { return 0.0; } }
        public double SalaryGrossYoung { get { return 0.0; } }
        public double SalaryGrossOldTax { get { return 0.0; } }
        public double SalaryGrossYoungTax { get { return 0.0; } }
    }
}