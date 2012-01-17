using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Telerik.Web.UI;

public partial class Pages_v5_Ledgers_SetRootBudgets : PageV5Base
{
    protected void Page_Init (object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            // Create controls, for amounts to be set in Page_Load. This is necessary for events to fire.
            // We don't know the year setting here yet.

            FinancialAccounts accounts = GetRootLevelResultAccounts();

            this.RepeaterAccountBudgets.DataSource = accounts;
            this.RepeaterAccountBudgets.DataBind();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageAccessRequired = new Access(_currentOrganization, AccessAspect.Bookkeeping, AccessType.Write);

        this.HiddenInitOrganizationId.Value = _currentOrganization.Identity.ToString();

        this.PageIcon = "iconshock-moneybag";
        this.PageTitle = Resources.Pages.Ledgers.SetRootBudgets_PageTitle;

        if (!Page.IsPostBack)
        {
            FinancialAccounts accounts = GetRootLevelResultAccounts();

            int currentYear = DateTime.Today.Year;
            _year = currentYear;

            do
            {
                this.DropYears.Items.Add(new ListItem(currentYear.ToString()));
                currentYear--;
            } while (accounts[0].GetTree().GetBudgetSumCents(currentYear) > 0);

            this.DropYears.SelectedIndex = 0;

            this.RepeaterAccountNames.DataSource = accounts;
            this.RepeaterAccountNames.DataBind();

            this.RepeaterAccountBudgets.DataSource = accounts;
            this.RepeaterAccountBudgets.DataBind();

            this.RepeaterAccountActuals.DataSource = accounts;
            this.RepeaterAccountActuals.DataBind();

            UpdateYearlyResult(accounts);

            this.LabelRootBudgetHeader.Text = String.Format(Resources.Pages.Ledgers.SetRootBudgets_PageHeader, _currentOrganization.Name);
            this.LabelAccountHeader.Text = Resources.Global.Financial_BookkeepingAccountShort;
            this.LabelYearlyResultLabel.Text = Resources.Global.Financial_YearlyResult;
            this.ButtonSetBudgets.Text = Resources.Pages.Ledgers.SetRootBudgets_SetNewBudgets;
            this.LabelBudgetOwnerHeader.Text = Resources.Pages.Ledgers.SetRootBudgets_BudgetOwnerHeader;

            this.LabelSidebarActions.Text = Resources.Global.Sidebar_Actions;
            this.LabelSidebarInfo.Text = Resources.Global.Sidebar_Information;
            this.LabelSidebarTodo.Text = Resources.Global.Sidebar_Todo;
            this.LabelDashboardInfo.Text = Resources.Pages.Ledgers.SetRootBudgets_Info;

            LocalizeActualsHeader();
        }
        else
        {
            _year = Int32.Parse(DropYears.SelectedItem.Text);
        }

        this.DropYears.Style[HtmlTextWriterStyle.FontWeight] = "bold";
        this.ButtonSetBudgets.Style[HtmlTextWriterStyle.MarginTop] = "4px";
        this.ButtonSetBudgets.Style[HtmlTextWriterStyle.MarginBottom] = "5px";
        this.ButtonSetBudgets.Style[HtmlTextWriterStyle.Padding] = "1px";

        RebindTooltips();
    }

    private void LocalizeActualsHeader()
    {
        if (_currentOrganization.Parameters.FiscalBooksClosedUntilYear >= _year)
        {
            this.LabelActualsHeader.Text = Resources.Global.Financial_Actuals;
        }
        else if (_year == DateTime.Today.Year)
        {
            this.LabelActualsHeader.Text = Resources.Global.Financial_ActualsToDate;
        }
        else
        {
            this.LabelActualsHeader.Text = Resources.Global.Financial_ActualsPreliminary;
        }
    }

    protected void UpdateYearlyResult(FinancialAccounts rootAccounts)
    {
        Int64 budgetSum = 0;
        Int64 actualSum = 0;

        foreach (FinancialAccount account in rootAccounts)
        {
            budgetSum += account.GetTree().GetBudgetSumCents(_year)/100;
            actualSum -= (account.GetTree().GetDeltaCents(new DateTime(_year,1,1), new DateTime(_year+1, 1, 1))+50)/100;  // negative: results accounts are sign reversed
        }

        this.TextYearlyResult.Text = String.Format("{0:N0}", budgetSum);
        this.TextYearlyResult.Style[HtmlTextWriterStyle.Width] = "80px";
        this.TextYearlyResult.Style[HtmlTextWriterStyle.TextAlign] = "right";

        this.LabelYearlyResultActuals.Text = String.Format("{0:N0}", actualSum);
    }

    private int _year;

    protected void RepeaterAccountNames_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        RepeaterItem item = e.Item;

        if (item.DataItem == null)
        {
            return;
        }

        FinancialAccount account = (FinancialAccount) item.DataItem;

        Label labelAccountName = (Label) e.Item.FindControl("LabelAccountName");
        Label labelAccountType = (Label) e.Item.FindControl("LabelAccountType");

        labelAccountName.Text = account.Name;

        // TODO: Add Type Too

        labelAccountType.Text = account.AccountType.ToString();
    }

    protected void RepeaterAccountBudgets_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        RepeaterItem item = e.Item;

        if (item.DataItem == null)
        {
            return;
        }

        FinancialAccount account = (FinancialAccount)item.DataItem;

        TextBox textBudget = (TextBox) e.Item.FindControl("TextBudget");

        if (_year > 0) // called from after viewstate parsed
        {
            textBudget.Text = String.Format("{0:N0}", account.GetTree().GetBudgetSumCents(_year)/100);
            textBudget.Style[HtmlTextWriterStyle.TextAlign] = "right";
            textBudget.Style[HtmlTextWriterStyle.Width] = "80px";
        }

        Label labelOwnerName = (Label) e.Item.FindControl("LabelBudgetOwner");
        Person accountOwner = account.Owner;
        if (accountOwner == null)
        {
            labelOwnerName.Text = Resources.Global.Global_NoOwner;
        }
        else
        {
            labelOwnerName.Text = accountOwner.Formal;
        }

        RadToolTip toolTip = (RadToolTip)e.Item.FindControl("ToolTip");

        Controls_v5_PersonDetailPopup personDetail = (Controls_v5_PersonDetailPopup)toolTip.FindControl("PersonDetail");
        personDetail.PersonChanged += new PersonChangedEventHandler(PersonDetail_PersonChanged);

        if (!Page.IsPostBack && _authority != null)
        {
            personDetail.Person = accountOwner;
            personDetail.Cookie = account;
        }

        HiddenField hiddenAccountId = (HiddenField) e.Item.FindControl("HiddenAccountId");
        hiddenAccountId.Value = account.Identity.ToString();

        // this.TooltipManager.TargetControls.Add(labelBudgetOwner.ClientID, line.AccountIdentity.ToString(), true);
    }


    void PersonDetail_PersonChanged(object sender, PersonChangedEventArgs e)
    {
        // Here, an owner has changed. We need to re-bind the owners column.

        // TODO: This resets the budget amounts to the values in the database, which is not really what we want.

        FinancialAccount account = (FinancialAccount) e.Cookie;
        account.Owner = e.NewPerson;

        FinancialAccounts accounts = GetRootLevelResultAccounts();

        this.RepeaterAccountBudgets.DataSource = accounts;
        this.RepeaterAccountBudgets.DataBind();

        RebindTooltips();
    }

    protected void DropYears_SelectedIndexChanged(object sender, EventArgs e)
    {
        FinancialAccounts accounts = GetRootLevelResultAccounts();

        UpdateYearlyResult(accounts);

        this.RepeaterAccountBudgets.DataSource = accounts;
        this.RepeaterAccountBudgets.DataBind();

        this.RepeaterAccountActuals.DataSource = accounts;
        this.RepeaterAccountActuals.DataBind();

        LocalizeActualsHeader();

        this.RebindTooltips();
    }

    private FinancialAccounts GetRootLevelResultAccounts()
    {
        Organization org = _currentOrganization;

        if (org == null && Page.IsPostBack)
        {
            // If we're being called from Page_Init to create controls just to catch events, then _currentOrganization won't be set.
            // We need to create it temporarily from a hidden field:

            org = Organization.FromIdentity(Int32.Parse(Request["ctl00$PlaceHolderMain$HiddenInitOrganizationId"]));
        }

        FinancialAccount yearlyResult = org.FinancialAccounts.CostsYearlyResult;
        FinancialAccounts allAccounts = FinancialAccounts.ForOrganization(org, FinancialAccountType.Result);

        // Select root accounts

        FinancialAccounts accounts = new FinancialAccounts();

        foreach (FinancialAccount account in allAccounts)
        {
            if (account.ParentFinancialAccountId == 0 && account.Identity != yearlyResult.Identity)
            {
                accounts.Add(account);
            }
        }

        return accounts;
    }


    protected void RebindTooltips()
    {
        foreach (RepeaterItem repeaterItem in this.RepeaterAccountBudgets.Items)
        {
            HiddenField hiddenAccountId = (HiddenField)repeaterItem.FindControl("HiddenAccountId");
            FinancialAccount child = FinancialAccount.FromIdentity(Int32.Parse(hiddenAccountId.Value));
            RadToolTip toolTip = (RadToolTip)repeaterItem.FindControl("ToolTip");

            Controls_v5_PersonDetailPopup personDetail = (Controls_v5_PersonDetailPopup)toolTip.FindControl("PersonDetail");

            FinancialAccount account = FinancialAccount.FromIdentity(Int32.Parse(hiddenAccountId.Value));
            personDetail.Person = account.Owner;
            personDetail.Cookie = child;
        }
    }

    protected void RepeaterAccountActuals_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        RepeaterItem item = e.Item;

        if (item.DataItem == null)
        {
            return;
        }

        FinancialAccount account = (FinancialAccount)item.DataItem;

        Label labelActuals = (Label) e.Item.FindControl("LabelAccountActuals");

        Int64 actuals = (account.GetTree().GetDeltaCents(new DateTime(_year, 1, 1), new DateTime(_year + 1, 1, 1))+50)/
                             100;

        labelActuals.Text = String.Format("{0:N0}", -actuals);  // negative as results accounts are sign reversed
    }


    protected void ButtonSetBudgets_Click(object sender, EventArgs e)
    {
        foreach (RepeaterItem repeaterItem in this.RepeaterAccountBudgets.Items)
        {
            HiddenField hiddenAccountId = (HiddenField) repeaterItem.FindControl("HiddenAccountId");
            TextBox textBudget = (TextBox) repeaterItem.FindControl("TextBudget");
            FinancialAccount account = FinancialAccount.FromIdentity(Int32.Parse(hiddenAccountId.Value));

            // TODO: Possible race condition here, fix with HiddenField

            Int64 newBudgetAmount = Int64.Parse(textBudget.Text.Replace(",","").Replace(" ",""), Thread.CurrentThread.CurrentCulture);  // TODO: May throw -- catch and send error message

            Int64 currentBudgetAmount = account.GetTree().GetBudgetSumCents(_year)/100;

            // Set the new amount to the difference between the single budget of this account and the intended amount

            if (newBudgetAmount != currentBudgetAmount)
            {
                account.SetBudgetCents(_year,
                                       account.GetBudgetCents(_year) + (newBudgetAmount - currentBudgetAmount)*100);
            }
        }

        // After updating budgets, rebind repeater

        FinancialAccounts accounts = GetRootLevelResultAccounts();

        UpdateYearlyResult(accounts);

        this.RepeaterAccountBudgets.DataSource = accounts;
        this.RepeaterAccountBudgets.DataBind();

    }
}