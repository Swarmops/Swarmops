using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Telerik.Web.UI;

public partial class Pages_v5_Ledgers_SetRootBudgets : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageAccessRequired = new Access(_currentOrganization, AccessAspect.Bookkeeping, AccessType.Write);

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
            } while (accounts[0].GetBudgetCents(currentYear) > 0);

            this.DropYears.SelectedIndex = 0;

            this.RepeaterAccountNames.DataSource = accounts;
            this.RepeaterAccountNames.DataBind();

            this.RepeaterAccountBudgets.DataSource = accounts;
            this.RepeaterAccountBudgets.DataBind();

            this.RepeaterAccountActuals.DataSource = accounts;
            this.RepeaterAccountActuals.DataBind();

            UpdateYearlyResult(accounts);
        }
        else
        {
            _year = Int32.Parse(DropYears.SelectedItem.Text);
        }

        this.DropYears.Style[HtmlTextWriterStyle.FontWeight] = "bold";
        this.ButtonSetBudgets.Style[HtmlTextWriterStyle.MarginTop] = "4px";
        this.ButtonSetBudgets.Style[HtmlTextWriterStyle.MarginBottom] = "5px";
    }

    protected void UpdateYearlyResult(FinancialAccounts rootAccounts)
    {
        Int64 budgetSum = 0;
        Int64 actualSum = 0;

        foreach (FinancialAccount account in rootAccounts)
        {
            budgetSum += account.GetTree().GetBudgetSumCents(_year)/100;
            actualSum -= account.GetTree().GetDeltaCents(new DateTime(_year,1,1), new DateTime(_year+1, 1, 1))/100;  // negative: results accounts are sign reversed
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

        textBudget.Text = String.Format("{0:N0}", account.GetTree().GetBudgetSumCents(_year)/100);
        textBudget.Style[HtmlTextWriterStyle.TextAlign] = "right";
        textBudget.Style[HtmlTextWriterStyle.Width] = "80px";

        Label labelOwnerName = (Label) e.Item.FindControl("LabelBudgetOwner");
        Person accountOwner = account.Owner;
        if (accountOwner == null)
        {
            labelOwnerName.Text = "None [LOC]";
        }
        else
        {
            labelOwnerName.Text = accountOwner.Formal;
        }

        RadToolTip toolTip = (RadToolTip)e.Item.FindControl("ToolTip");

        Controls_v5_PersonDetailPopup personDetail = (Controls_v5_PersonDetailPopup)toolTip.FindControl("PersonDetail");
        personDetail.PersonChanged += new EventHandler(PersonDetail_PersonChanged);

        if (!Page.IsPostBack && _authority != null)
        {
            personDetail.Person = accountOwner;
            personDetail.Account = account;
        }

        HiddenField hiddenAccountId = (HiddenField) e.Item.FindControl("HiddenAccountId");
        hiddenAccountId.Value = account.Identity.ToString();

        // this.TooltipManager.TargetControls.Add(labelBudgetOwner.ClientID, line.AccountIdentity.ToString(), true);
    }

    void PersonDetail_PersonChanged(object sender, EventArgs e)
    {
        // Here, an owner has changed. We need to re-bind the owners column.

        /*
        FinancialAccounts accounts = _account.Children;
        AccountBudgetLines childData = new AccountBudgetLines();

        foreach (FinancialAccount account in accounts)
        {
            childData.Add(new AccountBudgetLine(account, _year));
        }

        this.RepeaterBudgetOwners.DataSource = childData;
        this.RepeaterBudgetOwners.DataBind();

        RebindTooltips();*/
    }

    protected void DropYears_SelectedIndexChanged(object sender, EventArgs e)
    {
        FinancialAccounts accounts = GetRootLevelResultAccounts();

        UpdateYearlyResult(accounts);

        this.RepeaterAccountBudgets.DataSource = accounts;
        this.RepeaterAccountBudgets.DataBind();

        this.RepeaterAccountActuals.DataSource = accounts;
        this.RepeaterAccountActuals.DataBind();

        this.RebindTooltips();
    }

    private FinancialAccounts GetRootLevelResultAccounts()
    {
        FinancialAccount yearlyResult = _currentOrganization.FinancialAccounts.CostsYearlyResult;
        FinancialAccounts allAccounts = FinancialAccounts.ForOrganization(_currentOrganization, FinancialAccountType.Result);

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
            personDetail.Account = account;
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

        Int64 actuals = account.GetTree().GetDeltaCents(new DateTime(_year, 1, 1), new DateTime(_year + 1, 1, 1))/
                             100;

        labelActuals.Text = String.Format("{0:N0}", -actuals);  // negative as results accounts are sign reversed
    }

    protected void ButtonSetBudgets_Click(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}