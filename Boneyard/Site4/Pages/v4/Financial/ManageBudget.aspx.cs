using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Telerik.Web.UI;

public partial class Pages_v4_Financial_ManageBudget : PageV4Base
{
    protected void Page_Init (object sender, EventArgs e)
    {
        // In case we're in a postback, we need to recreate the suballocation controls in order for the viewstate to be recaptured.
        // But we don't have the viewstate in Init, yet. Still, we must have it here to recapture ViewState.
        //
        // The solution is to use hidden fields with the information we need to recreate the controls and then read them directly 
        // from the Request object.

        string accountIdString = Request["ctl00$BodyContent$HiddenInitBudgetId"];
        string yearString = Request["ctl00$BodyContent$HiddenInitYear"];
        
        if (!string.IsNullOrEmpty(accountIdString))
        {
            _account = FinancialAccount.FromIdentity(Int32.Parse(accountIdString));
            _year = Int32.Parse(yearString);

            InitSuballocation(false);
        }

        // If these are not present in the Request objects, the suballocation controls will be created in Page_Load without postback values.
    }

    private FinancialAccount _account;
    private int _year;
    private bool _initializingBudgets;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (ViewState[this.ClientID + "_BudgetId"] != null)
        {
            int accountId = Int32.Parse((string)ViewState[this.ClientID + "_BudgetId"]);
            int year = Int32.Parse((string)ViewState[this.ClientID + "_Year"]);


            if (_year != 0 && year != _year)
            {
                throw new InvalidOperationException("Mismatch between years in hidden fields and viewstate");
            }
            else
            {
                _year = year;
            }

            if (_account != null)
            {
                if (_account.Identity != accountId)
                {
                    throw new InvalidOperationException("Mismatch between account identities in hidden fields and viewstate");
                }
            }
            else
            {
                _account = FinancialAccount.FromIdentity(accountId);
                InitSuballocation(false);
            }

        }
        
        // Reset widths of textboxen

        for (int index = 1; index <= 12; index++)
        {
            TextBox predict = (TextBox) this.Master.FindControl("BodyContent").FindControl("TextPredictMonth" + index.ToString());
            predict.Style[HtmlTextWriterStyle.Width] = "55px";
            predict.Style[HtmlTextWriterStyle.TextAlign] = "right";

            TextBox actual = (TextBox) this.Master.FindControl("BodyContent").FindControl("TextActualsMonth" + index.ToString());
            actual.Style[HtmlTextWriterStyle.Width] = "55px";
            actual.Style[HtmlTextWriterStyle.TextAlign] = "right";
        }

        // Rebind tooltips' persons and authorities -- these are lost on postback

        RebindTooltips();
    }

    protected void RebindTooltips()
    {
        foreach (RepeaterItem repeaterItem in this.RepeaterBudgetOwners.Items)
        {
            HiddenField hiddenAccountId = (HiddenField) repeaterItem.FindControl("HiddenAccountId");
            FinancialAccount child = FinancialAccount.FromIdentity(Int32.Parse(hiddenAccountId.Value));
            RadToolTip toolTip = (RadToolTip) repeaterItem.FindControl("ToolTip");

            Controls_v4_PersonDetailPopup personDetail = (Controls_v4_PersonDetailPopup)toolTip.FindControl("PersonDetail");

            FinancialAccount account = FinancialAccount.FromIdentity(Int32.Parse(hiddenAccountId.Value));
            personDetail.Authority = _authority;
            personDetail.Person = account.Owner;
            personDetail.Account = account;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PopulateControls();
        }
    }


    private void PopulateControls()
    {
        FinancialAccounts accounts = FinancialAccounts.ForOwner(_currentUser);

        this.DropBudgets.Items.Clear();
        this.DropYears.Items.Clear();

        foreach (FinancialAccount account in accounts)
        {
            this.DropBudgets.Items.Add(new ListItem(account.Name + " / " + account.Organization.Name,
                                                    account.Identity.ToString()));
        }

        bool selectedBudget = false;
        string accountParameterString = Request.QueryString["AccountId"];

        if (!String.IsNullOrEmpty(accountParameterString))
        {
            int accountId = Int32.Parse(accountParameterString);

            if (FinancialAccount.FromIdentity(accountId).OwnerPersonId == _currentUser.Identity)
            {
                this.DropBudgets.SelectedValue = accountId.ToString();
            }
            else if (_currentUser.Identity == 1 && System.Diagnostics.Debugger.IsAttached) // DEBUG
            {
                FinancialAccount account = FinancialAccount.FromIdentity(accountId);
                this.DropBudgets.Items.Add(new ListItem(account.Name + " / " + account.Organization.Name,
                                                        account.Identity.ToString()));

                this.DropBudgets.SelectedValue = accountId.ToString();
            }
        }

        ViewState[this.ClientID + "_BudgetId"] = this.DropBudgets.SelectedValue;
        _account = FinancialAccount.FromIdentity(Int32.Parse(this.DropBudgets.SelectedValue));
        this.HiddenInitBudgetId.Value = _account.Identity.ToString();

        int yearStart = DateTime.Today.Year - 1;

        if (DateTime.Today.Month <= 6)
        {
            yearStart--;
        }

        for (int yearIndex = 0; yearIndex < 3; yearIndex++)
        {
            this.DropYears.Items.Add((yearStart+yearIndex).ToString());
        }

        this.DropYears.SelectedValue = DateTime.Today.Year.ToString();
        _year = DateTime.Today.Year;

        ViewState[this.ClientID + "_Year"] = this.DropYears.SelectedValue;
        this.HiddenInitYear.Value = this.DropYears.SelectedValue;

        PopulateBudgetData();
    }





    private void PopulateBudgetData()
    {
        Ledger.Accounts = FinancialAccounts.FromSingle(_account);
        int year = Int32.Parse(this.DropYears.SelectedValue);

        Ledger.DateStart = new DateTime(year, 1, 1);
        Ledger.DateEnd = new DateTime(year, 12, 31);
        Ledger.MaxAmount = 1.0e12m;

        double budget = _account.GetBudget(year);

        Ledger.Populate();

        // Populate predictions

        int monthIterator = 1;

        Int64[] monthlyPredicts = _account.GetBudgetMonthly(year);

        while (monthIterator <= 12)
        {
            TextBox box =
                (TextBox)
                this.Master.FindControl("BodyContent").FindControl("TextPredictMonth" + monthIterator.ToString());

            if (monthlyPredicts.Length == 12)
            {
                box.Text = (monthlyPredicts[monthIterator - 1] / 100).ToString("N0", new CultureInfo("sv-SE"));
            }
            else
            {
                box.Text = string.Empty;
            }

            monthIterator++;
        }

        // Populate actuals

        monthIterator = 1;

        while (monthIterator <= 12 && new DateTime (year,monthIterator,1).AddMonths(1) < DateTime.Today)
        {
            DateTime start = new DateTime(year, monthIterator, 1);
            Int64 deltaCents = _account.GetDeltaCents(start, start.AddMonths(1));

            decimal deltaDecimal = -deltaCents/100.0m;

            TextBox actual = (TextBox)this.Master.FindControl("BodyContent").FindControl("TextActualsMonth" + monthIterator.ToString());
            actual.Text = deltaDecimal.ToString("N0", new CultureInfo(_account.Organization.DefaultCountry.Culture));
            monthIterator++;
        }

        // Clear the non-set actuals fields

        while (monthIterator <= 12)
        {
            TextBox actual = (TextBox)this.Master.FindControl("BodyContent").FindControl("TextActualsMonth" + monthIterator.ToString());
            actual.Text = string.Empty;
            monthIterator++;
        }

        InitSuballocation(true);

        ScriptManager.RegisterStartupScript(this, Page.GetType(), "set_budget_iframe",
                            "var currWidth = document.getElementById('DivIframeContainer').offsetWidth; document.getElementById('DivIframeContainer').innerHTML='<iframe src=\"BudgetUsageBar.aspx?AccountId=" + _account.Identity.ToString() + "&Year=" + year.ToString() + "&Width=' + currWidth + '\" width=\"100%\" scrolling=\"no\" height=\"100\" frameBorder=\"0\" />';",
                            true);

    }

    private void InitSuballocation(bool setData)
    {
        this.TextThisAccountBudget.Text = (_account.GetBudgetCents(_year)/100).ToString("N0", new CultureInfo(_account.Organization.DefaultCountry.Culture));
        this.LabelThisAccountName.Text = _account.Name;
        this.TextThisAccountBudget.Style[HtmlTextWriterStyle.TextAlign] = "right";
        this.TextThisAccountBudget.Style[HtmlTextWriterStyle.Width] = "150px";

        FinancialAccounts accounts = _account.Children;

        AccountBudgetLines childData = new AccountBudgetLines();

        foreach (FinancialAccount account in accounts)
        {
            childData.Add(new AccountBudgetLine(account, _year));
        }

        _initializingBudgets = setData;

        this.RepeaterAccountNames.DataSource = childData;
        this.RepeaterAccountNames.DataBind();

        this.RepeaterBudgetTextBoxes.DataSource = childData;
        this.RepeaterBudgetTextBoxes.DataBind();

        this.RepeaterBudgetOwners.DataSource = childData;
        this.RepeaterBudgetOwners.DataBind();

        this.ButtonReallocate.Enabled = accounts.Count > 0 ? true : false;

        // this.TooltipManager.TargetControls.Add(this.LabelThisAccountOwner.ClientID, "1", true);

    }


    public class AccountBudgetLine
    {
        public AccountBudgetLine(FinancialAccount account, int year)
        {
            this.account = account;
            this.year = year;
        }

        private FinancialAccount account;
        private int year;

        public string AccountName { get { return account.Name;  } }
        public string AccountBudgetString { get { return account.GetTree().GetBudgetSum(year).ToString("N0", new CultureInfo (account.Organization.DefaultCountry.Culture)); } }
        public string AccountOwner { get { return account.Owner == null ? "Nobody" : account.Owner.Formal; } }
        public int AccountIdentity { get { return account.Identity; } }
        public FinancialAccount Account { get { return account; }}
    }

    public class AccountBudgetLines: List<AccountBudgetLine>
    {
        // just a typecast for readability
    }




    private Literal GetLiteral (string text)
    {
        Literal literal = new Literal();
        literal.Text = text;
        return literal;
    }


    protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
    {
        this.UpdateToolTip(args.Value, args.UpdatePanel);
    }

    private void UpdateToolTip(string identity, UpdatePanel panel)
    {
        Controls_v4_PersonDetailPopup details = new Controls_v4_PersonDetailPopup();
        // Control ctrl = Page.LoadControl("~/Controls/v4/PersonDetailPopup.ascx");

        panel.ContentTemplateContainer.Controls.Add(details);

        FinancialAccount account = FinancialAccount.FromIdentity(Int32.Parse(identity));
        
        // Controls_v4_PersonDetailPopup details = (Controls_v4_PersonDetailPopup)ctrl;
        details.Person = account.Owner;
        details.Authority = _authority;
    }


    protected void ButtonSelectBudget_Click(object sender, EventArgs e)
    {
        ViewState[this.ClientID + "_BudgetId"] = this.DropBudgets.SelectedValue;
        this.HiddenInitBudgetId.Value = this.DropBudgets.SelectedValue;
        ViewState[this.ClientID + "_Year"] = this.DropYears.SelectedValue;
        this.HiddenInitYear.Value = this.DropYears.SelectedValue;

        _account = FinancialAccount.FromIdentity(Int32.Parse(this.DropBudgets.SelectedValue));
        _year = Int32.Parse(this.DropYears.SelectedValue);

        PopulateBudgetData();

        bool enableChanges = Int32.Parse(this.DropYears.SelectedValue) >= DateTime.Today.Year;

        this.ButtonReallocate.Enabled = enableChanges;
        this.ButtonSavePredict.Enabled = enableChanges;

        RebindTooltips();
    }


    protected void ButtonReallocate_Click(object sender, EventArgs e)
    {
        FinancialAccounts children = _account.Children;

        double deltaTotal = 0.0;
        int budgetAttempt;
        int budgetTotal = 0;

        // In a first round, verify that all numbers are parsable

        foreach (RepeaterItem repeaterItem in this.RepeaterBudgetTextBoxes.Items)
        {
            HiddenField hiddenAccountId = (HiddenField)repeaterItem.FindControl("HiddenAccountId");
            FinancialAccount child = FinancialAccount.FromIdentity(Int32.Parse(hiddenAccountId.Value));

            TextBox box = (TextBox) repeaterItem.FindControl("TextChildBudget");
            string boxText = box.Text;
            if (!Int32.TryParse(boxText, NumberStyles.Number, new CultureInfo("sv-SE"), out budgetAttempt))
            {
                ScriptManager.RegisterStartupScript(this, Page.GetType(), "error",
                                    "alert('The budget for the account \"" + Server.HtmlEncode(child.Name).Replace("'", "''") + "\" does not appear to be a valid number. Please correct this and try again.');",
                                    true);
                return;
            }

            budgetTotal += budgetAttempt;
        }

        // Verify that the budget is not overrun by suballocations

        if (Math.Abs(budgetTotal) > Math.Abs(_account.GetTree().GetBudgetSum(_year)))
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "error",
                                "alert('You are trying to overallocate the budget. Please correct this and try again.');",
                                true);
            return;
        }

        // Then, move on to actually mod the budgets

        foreach (RepeaterItem repeaterItem in this.RepeaterBudgetTextBoxes.Items)
        {
            HiddenField hiddenAccountId = (HiddenField)repeaterItem.FindControl("HiddenAccountId");
            FinancialAccount child = FinancialAccount.FromIdentity(Int32.Parse(hiddenAccountId.Value));

            TextBox box = (TextBox)repeaterItem.FindControl("TextChildBudget");
            string boxText = box.Text;
            double newBudget = (double) Int32.Parse(boxText, NumberStyles.Number, new CultureInfo("sv-SE"));
            double curBudget = child.GetTree().GetBudgetSum(_year);

            // since we don't know here how much has been suballocated in turn, we need to produce a delta from the child's tree budget total and apply it to the child node

            double delta = newBudget - curBudget;
            child.SetBudget(_year, child.GetBudget(_year) + delta);

            deltaTotal += delta;
        }

        // Last, apply the total delta to the master account

        _account.SetBudget(_year, _account.GetBudget(_year) - deltaTotal);

        // Rewrite controls

        PopulateBudgetData();
    }


    protected void ButtonSavePredict_Click(object sender, EventArgs e)
    {
        int budgetTotal = 0;
        int budgetAttempt = 0;

        for (int month = 1; month <= 12; month++)
        {
            TextBox box = (TextBox)this.Master.FindControl("BodyContent").FindControl("TextPredictMonth" + month.ToString());
            string boxText = box.Text;
            if (!Int32.TryParse(boxText, NumberStyles.Number, new CultureInfo("sv-SE"), out budgetAttempt))
            {
                ScriptManager.RegisterStartupScript(this, Page.GetType(), "error",
                                    "alert('The prediction for \"" + Server.HtmlEncode(new DateTime (_year, month, 1).ToString("MMMM")) + "\" does not appear to be a valid number. Please correct this and try again.');",
                                    true);
                return;
            }

            budgetTotal += budgetAttempt;
        }

        // Verify that the budget is not overallocated

        double budgetCeiling = _account.GetBudget(_year);

        if (Math.Abs(budgetTotal) > Math.Abs(budgetCeiling))
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "error",
                                "alert('You are predicting an overallocation of the budget. Your budget is " + budgetCeiling.ToString("N0") + " and you are predicting a total of " + budgetTotal.ToString("N0") + ". Please correct this and try again.');",
                                true);
            return;
        }

        // Finally, actually set the values as good and repopulate them

        for (int month = 1; month <= 12; month++)
        {
            TextBox box = (TextBox)this.Master.FindControl("BodyContent").FindControl("TextPredictMonth" + month.ToString());
            string boxText = box.Text;
            Int64 newMonthly = Int64.Parse(boxText, NumberStyles.Number, new CultureInfo("sv-SE"));

            _account.SetBudgetMontly(_year, month, newMonthly*100);
        }

        PopulateBudgetData();
    }


    protected void RepeaterTextBoxes_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.DataItem == null)
        {
           return;
        }

        AccountBudgetLine line = (AccountBudgetLine) e.Item.DataItem;

        TextBox textBudget = (TextBox) e.Item.FindControl("TextChildBudget");
        textBudget.Style[HtmlTextWriterStyle.TextAlign] = "right";
        textBudget.Style[HtmlTextWriterStyle.Width] = "150px";

        HiddenField hiddenAccountId = (HiddenField)e.Item.FindControl("HiddenAccountId");
        hiddenAccountId.Value = line.AccountIdentity.ToString();

        // RIDDLE: What is textBudget.Text here?

        if (_initializingBudgets)
        {
            textBudget.Text = line.AccountBudgetString;
        }
    }

    protected void RepeaterBudgetOwners_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.DataItem == null)
        {
            return;
        }

        AccountBudgetLine line = (AccountBudgetLine)e.Item.DataItem;

        Label labelBudgetOwner = (Label)e.Item.FindControl("LabelChildBudgetOwner");
        labelBudgetOwner.Text = line.AccountOwner;

        HiddenField hiddenAccountId = (HiddenField)e.Item.FindControl("HiddenAccountId");
        hiddenAccountId.Value = line.AccountIdentity.ToString();

        RadToolTip toolTip = (RadToolTip) e.Item.FindControl("ToolTip");

        Controls_v4_PersonDetailPopup personDetail = (Controls_v4_PersonDetailPopup) toolTip.FindControl("PersonDetail");
        personDetail.PersonChanged += new EventHandler(PersonDetail_PersonChanged);

        if (!Page.IsPostBack && _authority != null)
        {
            personDetail.Authority = _authority;
            personDetail.Person = line.Account.Owner;
            personDetail.Account = line.Account;
        }

        // this.TooltipManager.TargetControls.Add(labelBudgetOwner.ClientID, line.AccountIdentity.ToString(), true);
    }

    void PersonDetail_PersonChanged(object sender, EventArgs e)
    {
        // Here, an owner has changed. We need to re-bind the owners column.

        FinancialAccounts accounts = _account.Children;
        AccountBudgetLines childData = new AccountBudgetLines();

        foreach (FinancialAccount account in accounts)
        {
            childData.Add(new AccountBudgetLine(account, _year));
        }

        this.RepeaterBudgetOwners.DataSource = childData;
        this.RepeaterBudgetOwners.DataBind();

        RebindTooltips();
    }


}


