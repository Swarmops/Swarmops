using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Security;

public partial class Pages_v5_Ledgers_SetRootBudgets : PageV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageAccessRequired = new Access(_currentOrganization, AccessAspect.Bookkeeping, AccessType.Write);

        this.PageIcon = "iconshock-moneybag";
        this.PageTitle = Resources.Pages.Ledgers.SetRootBudgets_PageTitle;

        if (!Page.IsPostBack)
        {
            FinancialAccounts accounts = GetRootLevelAccounts();

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
        }
        else
        {
            _year = Int32.Parse(DropYears.SelectedItem.Text);
        }

        this.DropYears.Style[HtmlTextWriterStyle.FontWeight] = "bold";
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

        textBudget.Text = String.Format("{0:N0}", account.GetBudgetCents(_year)/100);
        textBudget.Style[HtmlTextWriterStyle.TextAlign] = "right";
        textBudget.Style[HtmlTextWriterStyle.Width] = "150px";

    }

    protected void DropYears_SelectedIndexChanged(object sender, EventArgs e)
    {
        FinancialAccounts accounts = GetRootLevelAccounts();

        this.RepeaterAccountBudgets.DataSource = accounts;
        this.RepeaterAccountBudgets.DataBind();
    }

    private FinancialAccounts GetRootLevelAccounts()
    {
        FinancialAccounts allAccounts = FinancialAccounts.ForOrganization(_currentOrganization, FinancialAccountType.Result);

        // Select root accounts

        FinancialAccounts accounts = new FinancialAccounts();

        foreach (FinancialAccount account in allAccounts)
        {
            if (account.ParentFinancialAccountId == 0)
            {
                accounts.Add(account);
            }
        }

        return accounts;
    }
}