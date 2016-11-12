using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Media;
using Activizr.Logic.Pirates;

using Telerik.Web.UI;

public partial class Pages_v4_ListReporters : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Reporters reporters = Reporters.GetAll();
            reporters.Sort();

            this.GridReporters.DataSource = reporters;

            MediaCategories categories = MediaCategories.GetAll();

            foreach (MediaCategory category in categories)
            {
                this.CheckListCategories.Items.Add(new ListItem(category.Name, category.Identity.ToString()));
            }
        }
        this.GridReporters.Columns.FindByUniqueNameSafe("DeleteCommand").Visible 
            = _authority.HasAnyPermission(Permission.CanHandlePress);

       this.ButtonAdd.Enabled = _authority.HasAnyPermission(Permission.CanHandlePress);
    }

    /*
    protected void PopulateGrid (int accountId, DateTime start, DateTime end)
    {
        if (accountId == 0)
        {
            return;
        }

        FinancialAccount account = FinancialAccount.FromIdentity(accountId);

        DateTime balanceStart = new DateTime(DateTime.Today.Year, 1, 1);

        if (account.AccountType == FinancialAccountType.Asset || account.AccountType == FinancialAccountType.Debt)
        {
            balanceStart = new DateTime(2006, 1, 1); // The dawn of mankind, for our purposes
        }

        currentAccountBalance = account.GetDelta(balanceStart, start.AddSeconds(-1));

        // We use "AddDays (1)" onto the end date, as the underlying select works like this:
        // "get rows where datetime >= start or datetime < end"
        // This assures us that the dates given in the interface make an inclusive range.

        this.GridTransactions.DataSource = FinancialAccount.FromIdentity(accountId).GetRows(start, end.AddDays(1));
    }


    protected void PopulateAccounts()
    {
        int organizationId = Organization.PPSEid; // TODO: Get from a future dropdown

        FinancialAccounts accounts = FinancialAccounts.ForOrganization(organizationId);

        this.DropAccounts.Items.Add(new ListItem("-- Select account --", "0"));
        this.DropAccountsCreate.Items.Add(new ListItem("-- Select account --", "0"));
        foreach (FinancialAccount account in accounts)
        {
            this.DropAccounts.Items.Add(new ListItem(account.Name, account.Identity.ToString()));
            this.DropAccountsCreate.Items.Add(new ListItem(account.Name, account.Identity.ToString()));
        }
    }*/


    protected void GridReporters_ItemCreated (object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            Reporter reporter = e.Item.DataItem as Reporter;

            if (reporter == null)
            {
                return;
            }

            /*
            HyperLink editLink = (HyperLink) e.Item.FindControl("ManageLink");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowTransactionForm('{0}','{1}');",
                                                           row.FinancialTransactionId, e.Item.ItemIndex);*/

            Label labelCategories = (Label) e.Item.FindControl("LabelCategories");
            List<string> categoryNames = new List<string>();

            foreach (MediaCategory mediaCategory in reporter.MediaCategories)
            {
                categoryNames.Add(mediaCategory.Name);
            }

            labelCategories.Text = String.Join(", ", categoryNames.ToArray());

        }
    }

    /*
    protected void ButtonCreateTransaction_Click (object sender, EventArgs e)
    {
        Person currentUser = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));

        if (!currentUser.HasFinancialAccess)
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
        DateTime transactionDateTime = (DateTime) this.DateCreate.SelectedDate;

        FinancialTransaction transaction = FinancialTransaction.Create(1, transactionDateTime, description);
        transaction.AddRow(accountId, amount, currentUser.Identity);

        // As the RadWindowManager and RadAjaxUpdate are part of the UpdatePanel we're rewriting, we
        // need to make the client call the function only when the ajax call has completed. We set
        // 200ms for this, but pretty much any amount of time should be ok, as long as it's delayed
        // past the actual ajax rewrite.

        ScriptManager.RegisterStartupScript(this, Page.GetType(), "finishthejob",
                                            "ShowTransactionFormDelayed (" + transaction.Identity + ");", true);
    }


    protected void ButtonViewTransactions_Click (object sender, EventArgs e)
    {
        Person currentUser = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));

        if (!currentUser.HasFinancialAccess)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('You do not have access to financial records.');", true);
            return;
        }

        int accountId = Int32.Parse(this.DropAccounts.SelectedValue);

        if (accountId == 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('Please select an account.');", true);
            return;
        }

        if (this.DateStart.SelectedDate > this.DateEnd.SelectedDate)
        {
            DateTime largerDate = (DateTime) this.DateStart.SelectedDate;
            this.DateStart.SelectedDate = this.DateEnd.SelectedDate;
            this.DateEnd.SelectedDate = largerDate;
        }

        FinancialAccount account = FinancialAccount.FromIdentity(accountId);

        this.LabelTransactionsTitle.Text = "Transactions in account '" + account.Name + "'";

        PopulateGrid(accountId, (DateTime) this.DateStart.SelectedDate, (DateTime) this.DateEnd.SelectedDate);
        this.GridTransactions.Rebind();

        // Hide the accounts dropdown in "Create Transaction" and replace it with a static account name

        this.DropAccountsCreate.Visible = false;
        this.LabelAccountCreate.Visible = true;
        this.DropAccountsCreate.SelectedValue = accountId.ToString();
        this.LabelAccountCreate.Text = account.Name;
    }


    private double currentAccountBalance = 100.0; // any number will do; this is used to iterate as the grid populates

    */
    protected void RadAjaxManager1_AjaxRequest (object sender, AjaxRequestEventArgs e)
    {
        // TODO: There is a problem with re-getting the query parameters here -- the user 
        // may have changed the data in the web form, which will repopulate the grid 
        // with different data when the popup closes. This would be extremely confusing 
        // to the user. Is there  a good way to invisibly cache the query base 
        // (account, start date, end date)?

        /*
        if (e.Argument == "Rebind")
        {
            this.GridTransactions.MasterTableView.SortExpressions.Clear();
            this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            PopulateGrid(accountId, (DateTime) this.DateStart.SelectedDate, (DateTime) this.DateEnd.SelectedDate);
            this.GridTransactions.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            this.GridTransactions.MasterTableView.SortExpressions.Clear();
            this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            this.GridTransactions.MasterTableView.CurrentPageIndex = this.GridTransactions.MasterTableView.PageCount - 1;
            PopulateGrid(accountId, (DateTime) this.DateStart.SelectedDate, (DateTime) this.DateEnd.SelectedDate);
            this.GridTransactions.Rebind();
        }*/
    }


    protected void GridReporters_ItemCommand(object source, GridCommandEventArgs args)
    {
        if (args.CommandName == "Delete")
        {
            Reporter reporter =
                Reporter.FromIdentity(
                    (int) this.GridReporters.MasterTableView.DataKeyValues[args.Item.ItemIndex]["ReporterId"]);

            reporter.Delete();
            RepopulateReporterGrid();
        }
    }


    protected void ButtonAdd_Click(object sender, EventArgs e)
    {
        string reporterName = this.TextReporterName.Text;
        string mediaName = this.TextMediaName.Text;
        string email = this.TextEmail.Text;

        this.TextReporterName.Text = string.Empty;
        this.TextMediaName.Text = string.Empty;
        this.TextEmail.Text = string.Empty;

        string name = reporterName + " (" + mediaName + ")";

        if (reporterName.Length < 2)
        {
            name = mediaName;
        }

        List<string> categories = new List<string>();

        foreach (ListItem item in this.CheckListCategories.Items)
        {
            if (item.Selected)
            {
                item.Selected = false;
                categories.Add(item.Text);
            }
        }

        Reporter.Create(name, email, categories.ToArray());

        RepopulateReporterGrid();
    }

    private void RepopulateReporterGrid()
    {
        Reporters reporters = Reporters.GetAll();
        reporters.Sort();

        this.GridReporters.DataSource = reporters;
        this.GridReporters.DataBind();
    }
}