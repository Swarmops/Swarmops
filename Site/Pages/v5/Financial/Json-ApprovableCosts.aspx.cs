using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Frontend;
using Swarmops.Localization;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

public partial class Pages_v5_Finance_Json_ApprovableCosts : DataV5Base
{
    private Dictionary<int, Int64> _approvalRights;
    private ApprovableCosts _approvableCosts;
    private ApprovableCosts _approvedCosts;

    protected void Page_Load (object sender, EventArgs e)
    {
        // Get all attestable items

        this._approvalRights = GetApprovalRights();
        this._approvableCosts = new ApprovableCosts();
        this._approvedCosts = new ApprovableCosts();

        PopulateCashAdvances();
        PopulateExpenses();
        PopulateInboundInvoices();
        //PopulateSalaries();
        //PopulateParleys();

        // Format as JSON and return

        Response.ContentType = "application/json";
        string json = FormatAsJson();
        Response.Output.WriteLine (json);
        Response.End();
    }

    private string FormatAsJson()
    {
        StringBuilder result = new StringBuilder (16384);

        string hasDoxString =
            "<img baseid='{5}' class='LocalIconDox action-icon' />";

        result.Append ("{\"rows\":[");

        foreach (ApprovableCost cost in this._approvableCosts)
        {
            result.Append("{");
            result.AppendFormat(
                "\"item\":\"{0}\",\"beneficiary\":\"{1}\",\"description\":\"{2}\",\"budgetName\":\"{3}\",\"amountRequested\":\"{4:N2}\",\"itemId\":\"{5}\"," +
                "\"dox\":\"" + (cost.HasDox ? hasDoxString : "&nbsp;") + "\"," +
                "\"actions\":\"<span style='position:relative;left:-2px'>" +
                "<img id='IconApproval{5}' class='LocalIconApproval LocalNew LocalFundsInsufficient action-icon' accountid='{6}' amount='{4}' baseid='{5}' />" +
                "<img id='IconApproved{5}' class='LocalIconApproved LocalNew status-icon' baseid='{5}' />" +
                "<img id='IconUndo{5}' class='LocalIconUndo LocalNew action-icon' baseid='{5}' />" +
                "<img id='IconWait{5}' class='LocalIconWait LocalNew status-icon' baseid='{5}' />" +
                "<img id='IconDenial{5}' class='LocalIconDenial LocalNew action-icon' baseid='{5}' />" +
                "<img id='IconDenied{5}' class='LocalIconDenied LocalNew status-icon' baseid='{5}' />" +
                "</span>\"",
                JsonSanitize(cost.IdentityDisplay.ToString()),
                JsonSanitize(cost.Beneficiary), JsonSanitize(TryLocalize(cost.Description)),
                JsonSanitize(cost.BudgetName),
                cost.AmountRequestedCents / 100.0, cost.Identity, cost.Budget.Identity);

            result.Append("},");
        }

        foreach (ApprovableCost cost in this._approvedCosts)
        {
            result.Append("{");
            result.AppendFormat(
                "\"item\":\"{0}\",\"beneficiary\":\"{1}\",\"description\":\"{2}\",\"budgetName\":\"{3}\",\"approved\":\"yes\",\"amountRequested\":\"{4:N2}\",\"itemId\":\"{5}\"," +
                "\"dox\":\"" + (cost.HasDox ? hasDoxString : "&nbsp;") + "\"," +
                "\"actions\":\"<span style='position:relative;left:-2px'>" +
                "<img id='IconApproval{5}' class='LocalIconApproval LocalFundsInsufficient LocalApproved action-icon' accountid='{6}' amount='{4}' baseid='{5}' />" +
                "<img id='IconApproved{5}' class='LocalIconApproved LocalApproved status-icon' baseid='{5}' />" +
                "<img id='IconUndo{5}' class='LocalIconUndo LocalApproved action-icon' baseid='{5}' />" +
                "<img id='IconWait{5}' class='LocalIconWait LocalApproved status-icon' baseid='{5}' />" +
                "<img id='IconDenial{5}' class='LocalIconDenial LocalApproved action-icon' baseid='{5}' />" +
                "<img id='IconDenied{5}' class='LocalIconDenied LocalApproved status-icon' baseid='{5}' />" +
                "</span>\"",
                JsonSanitize(cost.IdentityDisplay.ToString()),
                JsonSanitize(cost.Beneficiary), JsonSanitize(TryLocalize(cost.Description)),
                JsonSanitize(cost.BudgetName),
                cost.AmountRequestedCents / 100.0, cost.Identity, cost.Budget.Identity);
            result.Append("},");
        }
        if (result.ToString().EndsWith(","))
        {
            result.Remove(result.Length - 1, 1); // remove last comma
        }

        result.Append ("]}");

        return result.ToString();
    }


    private Dictionary<int, Int64> GetApprovalRights()
    {
        // Right now, this function is quite primitive. At some point in the future, it needs to take into
        // account that a budget may have several approvers. Right now, it just loops over all accounts and
        // checks the owner.

        Dictionary<int, Int64> result = new Dictionary<int, Int64>();
        FinancialAccounts accounts = FinancialAccounts.ForOrganization (CurrentOrganization);

        foreach (FinancialAccount account in accounts)
        {
            if (account.OwnerPersonId == CurrentUser.Identity)
            {
                if (account.AccountType == FinancialAccountType.Cost)
                {
                    result[account.Identity] = account.GetBudgetCentsRemaining();
                }
                else
                {
                    result[account.Identity] = 1; // any value
                }
            }
        }

        return result;
    }


    private void PopulateCashAdvances()
    {
        CashAdvances advances = CashAdvances.ForOrganization (CurrentOrganization);

        foreach (CashAdvance advance in advances)
        {
            if (this._approvalRights.ContainsKey (advance.BudgetId) ||
                advance.Budget.OwnerPersonId == Person.NobodyId)
            {
                ApprovableCost cost = new ApprovableCost (
                    "A" + advance.Identity.ToString (CultureInfo.InvariantCulture),
                    advance.Person.Name, advance.AmountCents, advance.Budget,
                    advance.Description, "Financial_CashAdvance", false, advance);

                if (!advance.Attested) // if not attested
                {
                    this._approvableCosts.Add (cost);
                }
                else if (!advance.PaidOut) // if attested, but still reversible
                {
                    this._approvedCosts.Add (cost);
                }
            }
        }
    }


    private void PopulateExpenses()
    {
        ExpenseClaims expenses = ExpenseClaims.ForOrganization (CurrentOrganization);
        bool vatEnabled = CurrentOrganization.VatEnabled;

        foreach (ExpenseClaim expenseClaim in expenses)
        {
            if (this._approvalRights.ContainsKey (expenseClaim.BudgetId) ||
                expenseClaim.Budget.OwnerPersonId == Person.NobodyId)
            {
                Documents dox = expenseClaim.Documents;
                bool hasDox = (dox.Count > 0 ? true : false);

                ApprovableCost cost = null;
                string expenseClaimLoc = LocalizedStrings.Get(LocDomain.Global, "Financial_ExpenseClaim");
                if (string.IsNullOrEmpty(expenseClaimLoc))
                {
                    expenseClaimLoc = "Expense";
                }

                if (vatEnabled)
                {
                    cost = new ApprovableCost(
                        "E" + expenseClaim.Identity.ToString(CultureInfo.InvariantCulture),
                        expenseClaim.ClaimerCanonical, expenseClaim.AmountCents - expenseClaim.VatCents, expenseClaim.Budget,
                        expenseClaim.Description, expenseClaimLoc, hasDox, expenseClaim);
                }
                else
                {
                    cost = new ApprovableCost(
                        "E" + expenseClaim.Identity.ToString(CultureInfo.InvariantCulture),
                        expenseClaim.ClaimerCanonical, expenseClaim.AmountCents, expenseClaim.Budget,
                        expenseClaim.Description, expenseClaimLoc, hasDox, expenseClaim);
                }

                if (expenseClaim.Attested)
                {
                    this._approvedCosts.Add (cost);
                }
                else
                {
                    this._approvableCosts.Add (cost);
                }
            }
        }
    }


    private void PopulateInboundInvoices()
    {
        InboundInvoices invoices = InboundInvoices.ForOrganization (CurrentOrganization);

        foreach (InboundInvoice invoice in invoices)
        {
            Documents dox = invoice.Documents;
            bool hasDox = (dox.Count > 0 ? true : false);

            if (this._approvalRights.ContainsKey (invoice.BudgetId) ||
                invoice.Budget.OwnerPersonId == Person.NobodyId)
            {
                ApprovableCost cost = new ApprovableCost ("I" + invoice.Identity.ToString (CultureInfo.InvariantCulture),
                    invoice.Supplier, invoice.BudgetAmountCents, invoice.Budget, invoice.Description,
                    "[Loc]Financial_InvoiceInbound", hasDox, invoice);

                if (invoice.Attested)
                {
                    this._approvedCosts.Add (cost);
                }
                else
                {
                    this._approvableCosts.Add (cost);
                }
            }
        }
    }


    private void PopulateSalaries()
    {
        Salaries salaries = Salaries.ForOrganization (CurrentOrganization);

        // No unattestability for previously attested salaries because complex

        foreach (Salary salary in salaries)
        {
            if (this._approvalRights.ContainsKey (salary.PayrollItem.BudgetId) ||
                salary.PayrollItem.Budget.OwnerPersonId == Person.NobodyId)
            {
                if (!salary.Attested)
                {
                    this._approvableCosts.Add(new ApprovableCost("S" + salary.Identity.ToString(CultureInfo.InvariantCulture),
                        salary.PayrollItem.PersonCanonical, salary.CostTotalCents, salary.PayrollItem.Budget,
                        "[Loc]Financial_SalarySpecification|[Date]" +
                        salary.PayoutDate.ToString(CultureInfo.InvariantCulture), "Financial_Salary", false, salary));
                }
                else
                {
                    this._approvedCosts.Add(new ApprovableCost("S" + salary.Identity.ToString(CultureInfo.InvariantCulture),
                        salary.PayrollItem.PersonCanonical, salary.CostTotalCents, salary.PayrollItem.Budget,
                        "[Loc]Financial_SalarySpecification|[Date]" +
                        salary.PayoutDate.ToString(CultureInfo.InvariantCulture), "Financial_Salary", false, salary));
                }
            }
        }
    }


    private void PopulateParleys()
    {
        Parleys parleys = Parleys.ForOrganization (CurrentOrganization).WhereNotApproved;

        foreach (Parley parley in parleys)
        {
            if (this._approvalRights.ContainsKey (parley.BudgetId) || parley.Budget.OwnerPersonId == Person.NobodyId)
            {
                this._approvableCosts.Add (new ApprovableCost ("P" + parley.Identity.ToString (CultureInfo.InvariantCulture),
                    parley.Person.Canonical, parley.BudgetCents, parley.ParentBudget, parley.Name, "Financial_Parley",
                    false, parley));
            }
        }
    }


    protected class ApprovableCost
    {
        public ApprovableCost (string identity, string beneficiary, Int64 amountCents, FinancialAccount account,
            string description, string identityDisplay, bool hasDox, IHasIdentity item)
        {
            IdentityDisplay = identityDisplay;
            Identity = identity;
            Beneficiary = beneficiary;
            AmountRequestedCents = amountCents;
            Budget = account;
            Description = description;
            HasDox = hasDox;
            Item = item;
        }

        public FinancialAccount Budget { get; private set; }

        public string BudgetName
        {
            get { return Budget.Name; }
        }

        public string Identity { get; private set; }
        public string Beneficiary { get; private set; }
        public Int64 AmountRequestedCents { get; private set; }
        public string Description { get; private set; }
        public string IdentityDisplay { get; private set; }
        public IHasIdentity Item { get; private set; }
        public bool HasDox { get; private set; }
    }

    protected class ApprovableCosts : List<ApprovableCost>
    {
        // empty class, we just want the name definition        
    }
}