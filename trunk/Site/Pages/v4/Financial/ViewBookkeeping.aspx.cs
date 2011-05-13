using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;

using Telerik.Web.UI;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

public partial class Pages_v4_ViewBookkeeping : PageV4Base
{

    int _organizationId = 0;

    protected void Page_Load (object sender, EventArgs e)
    {
        _organizationId = 0;
        Int32.TryParse(this.DropOrganizations.SelectedValue, out _organizationId);

        if (!Page.IsPostBack)
        {
            this.GridTransactions.DataSource = new FinancialTransactions();
            this.DropOrganizations.Items.Add(new ListItem("Piratpartiet SE", Organization.PPSEid.ToString()));
            if (_currentUser.Identity == 1)
            {
                // Ok, so this is an ugly-as-all-hell hack. But it keeps me sane.

                this.DropOrganizations.Items.Add(new ListItem("Rick's Sandbox", "55"));
            }
            PopulateAccounts();
            this.DateStart.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
            this.DateEnd.SelectedDate = new DateTime(DateTime.Now.Year, 12, 31);
            this.DateCreate.SelectedDate = DateTime.Today;

            this.DateStart.DateInput.Culture = new CultureInfo("sv-SE");
            this.DateEnd.DateInput.Culture = new CultureInfo("sv-SE");
            this.DateCreate.DateInput.Culture = new CultureInfo("sv-SE");
        }
    }


    protected void PopulateGrid (int accountId, DateTime start, DateTime end)
    {
        if (accountId == 0)
        {
            this.GridTransactions.DataSource = new FinancialAccountRows();
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


    protected void PopulateAccounts ()
    {
        this.DropAccounts.Items.Clear();
        this.DropAccountsCreate.Items.Clear();

        this.DropAccounts.Items.Add(new ListItem("-- Select account --", "0"));
        this.DropAccountsCreate.Items.Add(new ListItem("-- Select account --", "0"));

        if (_organizationId == 0)
        {
            return; // no valid org
        }

        FinancialAccounts accounts = FinancialAccounts.ForOrganization(Organization.FromIdentity(_organizationId));

        foreach (FinancialAccount account in accounts)
        {
            this.DropAccounts.Items.Add(new ListItem("[" + account.AccountType.ToString().Substring(0, 1) + "] " + account.Name, account.Identity.ToString()));
            this.DropAccountsCreate.Items.Add(new ListItem("[" + account.AccountType.ToString().Substring(0, 1) + "] " + account.Name, account.Identity.ToString()));
        }
    }


    protected void GridTransactions_ItemCreated (object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            FinancialAccountRow row = (FinancialAccountRow)e.Item.DataItem;

            if (row == null)
            {
                return;
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("ManageLink");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowTransactionForm('{0}','{1}');",
                                                           row.FinancialTransactionId, e.Item.ItemIndex);

            string field = "LabelDebit";

            if (row.Amount < 0.0m)
            {
                field = "LabelCredit";
            }

            Label labelDelta = (Label)e.Item.FindControl(field);
            labelDelta.Text = row.Amount.ToString("+#,##0.00;-#,##0.00", new CultureInfo("sv-SE"));

            currentAccountBalance += row.Amount;

            Label labelBalance = (Label)e.Item.FindControl("LabelBalance");
            labelBalance.Text = currentAccountBalance.ToString("#,##0.00", new CultureInfo("sv-SE"));
        }
    }


    protected void ButtonCreateTransaction_Click (object sender, EventArgs e)
    {

        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, _organizationId,-1,Authorization.Flag.ExactOrganization))
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

        FinancialTransaction transaction = FinancialTransaction.Create(Organization.PPSEid, transactionDateTime, description);
        transaction.AddRow(FinancialAccount.FromIdentity(accountId), amount, _currentUser);

        // As the RadWindowManager and RadAjaxUpdate are part of the UpdatePanel we're rewriting, we
        // need to make the client call the function only when the ajax call has completed. We set
        // 200ms for this, but pretty much any amount of time should be ok, as long as it's delayed
        // past the actual ajax rewrite.

        ScriptManager.RegisterStartupScript(this, Page.GetType(), "finishthejob",
                                            "ShowTransactionFormDelayed (" + transaction.Identity + ");", true);
    }


    protected void ButtonViewTransactions_Click (object sender, EventArgs e)
    {

        int accountId = Int32.Parse(this.DropAccounts.SelectedValue);

        if (accountId == 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('Please select an account.');", true);
            return;
        }
        RedrawForNewAccount();
    }

    private void RedrawForNewAccount ()
    {
        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, _organizationId, -1, Authorization.Flag.ExactOrganization))
        {
            CreateTransactionPanel.Visible = false;
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('You do not have access to financial records.');", true);
            return;
        }
        int accountId = Int32.Parse(this.DropAccounts.SelectedValue);

        if (this.DateStart.SelectedDate > this.DateEnd.SelectedDate)
        {
            DateTime largerDate = (DateTime)this.DateStart.SelectedDate;
            this.DateStart.SelectedDate = this.DateEnd.SelectedDate;
            this.DateEnd.SelectedDate = largerDate;
        }

        PopulateGrid(accountId, (DateTime)this.DateStart.SelectedDate, (DateTime)this.DateEnd.SelectedDate);
        this.GridTransactions.Rebind();

        if (accountId != 0)
        {
            FinancialAccount account = FinancialAccount.FromIdentity(accountId);

            this.LabelTransactionsTitle.Text = "Transactions in account '" + account.Name + "'";


            // Hide the accounts dropdown in "Create Transaction" and replace it with a static account name

            this.DropAccountsCreate.Visible = false;
            this.LabelAccountCreate.Visible = true;
            this.DropAccountsCreate.SelectedValue = accountId.ToString();
            this.LabelAccountCreate.Text = account.Name;
        }
        else
        {
            this.LabelTransactionsTitle.Text = "No account selected.";
            this.DropAccountsCreate.Visible = true;
            this.LabelAccountCreate.Visible = false;
            this.LabelAccountCreate.Text = "";
        }
    }


    private decimal currentAccountBalance = 0.0m; // any number will do; this is used to iterate as the grid populates


    protected void RadAjaxManager1_AjaxRequest (object sender, AjaxRequestEventArgs e)
    {
        // TODO: 
        // There is a problem with re-getting the query parameters here -- the user 
        // may have changed the data in the web form, which will repopulate the grid 
        // with different data when the popup closes. This would be extremely confusing 
        // to the user. Is there  a good way to invisibly cache the query base 
        // (account, start date, end date)?

        int accountId = Int32.Parse(this.DropAccounts.SelectedValue);

        if (e.Argument == "Rebind")
        {
            this.GridTransactions.MasterTableView.SortExpressions.Clear();
            this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            PopulateGrid(accountId, (DateTime)this.DateStart.SelectedDate, (DateTime)this.DateEnd.SelectedDate);
            this.GridTransactions.Rebind();
            this.PopulateAccounts();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            this.GridTransactions.MasterTableView.SortExpressions.Clear();
            this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            this.GridTransactions.MasterTableView.CurrentPageIndex = this.GridTransactions.MasterTableView.PageCount - 1;
            PopulateGrid(accountId, (DateTime)this.DateStart.SelectedDate, (DateTime)this.DateEnd.SelectedDate);
            this.GridTransactions.Rebind();
            this.PopulateAccounts();
        }
    }


    protected void DropOrganizations_SelectedIndexChanged (object sender, EventArgs e)
    {
        PopulateAccounts();
        CreateTransactionPanel.Visible = true;
        ButtonCreateAccount.Visible = true;

        RedrawForNewAccount();
    }
    protected void DropAccounts_SelectedIndexChanged (object sender, EventArgs e)
    {
        RedrawForNewAccount();
    }

    protected void ButtonCreateAccount_Click (object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, Page.GetType(), "finishthejob",
                                            "ShowAddAccountForm(" + _organizationId + ");", true);

    }
}