using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Frontend;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

public partial class Pages_v5_Finance_Json_AttestableCosts : DataV5Base
{
    private Dictionary<int, Int64> _attestationRights;
    private AttestableItems _items;
    private AttestableItems _attestedItems;

    protected void Page_Load (object sender, EventArgs e)
    {
        // Get all attestable items

        this._attestationRights = GetAttestationRights();
        this._items = new AttestableItems();
        this._attestedItems = new AttestableItems();

        PopulateCashAdvances();
        PopulateExpenses();
        PopulateInboundInvoices();
        PopulateSalaries();
        PopulateParleys();

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
            "<img src='/Images/Icons/iconshock-search-256px.png' onmouseover=\\\"this.src='/Images/Icons/iconshock-search-hot-256px.png';\\\" onmouseout=\\\"this.src='/Images/Icons/iconshock-search-256px.png';\\\" baseid='{6}' class='LocalViewDox' style='cursor:pointer' height='20' width='20' />";

        result.Append ("{\"rows\":[");

        foreach (AttestableItem item in this._items)
        {
            result.Append("{");
            result.AppendFormat(
                "\"item\":\"{0}\",\"beneficiary\":\"{1}\",\"description\":\"{2}\",\"budgetName\":\"{3}\",\"amountRequested\":\"{4:N2}\",\"itemId\":\"{5}\"," +
                "\"dox\":\"" + (item.HasDox ? hasDoxString : "&nbsp;") + "\"," +
                "\"actions\":\"<span style=\\\"position:relative;left:-2px\\\">" +
                "<img id=\\\"IconApproval{5}\\\" class=\\\"LocalIconApproval LocalNew LocalFundsInsufficient\\\" accountid=\\\"{6}\\\" amount=\\\"{4}\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconApproved{5}\\\" class=\\\"LocalIconApproved LocalNew\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconDenial{5}\\\" class=\\\"LocalIconDenial LocalNew\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconDenied{5}\\\" class=\\\"LocalIconDenied LocalNew\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconUndo{5}\\\" class=\\\"LocalIconUndo LocalNew\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" /></span>\"",
                JsonSanitize(GetGlobalResourceObject("Global", item.IdentityDisplay).ToString()),
                JsonSanitize(item.Beneficiary), JsonSanitize(TryLocalize(item.Description)),
                JsonSanitize(item.BudgetName),
                item.AmountRequestedCents / 100.0, item.Identity, item.Budget.Identity);
            result.Append("},");
        }

        foreach (AttestableItem item in this._attestedItems)
        {
            result.Append("{");
            result.AppendFormat(
                "\"item\":\"{0}\",\"beneficiary\":\"{1}\",\"description\":\"{2}\",\"budgetName\":\"{3}\",\"previous\":\"yes\",\"amountRequested\":\"{4:N2}\",\"itemId\":\"{5}\"," +
                "\"dox\":\"" + (item.HasDox ? hasDoxString : "&nbsp;") + "\"," +
                "\"actions\":\"<span style=\\\"position:relative;left:-2px\\\">" +
                "<img id=\\\"IconApproval{5}\\\" class=\\\"LocalIconApproval LocalFundsInsufficient LocalPreviouslyAttested\\\" accountid=\\\"{6}\\\" amount=\\\"{4}\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconApproved{5}\\\" class=\\\"LocalIconApproved LocalPreviouslyAttested\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconDenial{5}\\\" class=\\\"LocalIconDenial LocalPreviouslyAttested\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconDenied{5}\\\" class=\\\"LocalIconDenied LocalPreviouslyAttested\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconUndo{5}\\\" class=\\\"LocalIconUndo LocalPreviouslyAttested\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" /></span>\"",
                JsonSanitize(GetGlobalResourceObject("Global", item.IdentityDisplay).ToString()),
                JsonSanitize(item.Beneficiary), JsonSanitize(TryLocalize(item.Description)),
                JsonSanitize(item.BudgetName),
                item.AmountRequestedCents / 100.0, item.Identity, item.Budget.Identity);
            result.Append("},");
        }

        result.Remove(result.Length - 1, 1); // remove last comma

        result.Append ("]}");

        return result.ToString();
    }


    private Dictionary<int, Int64> GetAttestationRights()
    {
        // Right now, this function is quite primitive. At some point in the future, it needs to take into
        // account that a budget may have several attesters. Right now, it just loops over all accounts and
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
            if (this._attestationRights.ContainsKey (advance.BudgetId) ||
                advance.Budget.OwnerPersonId == Person.NobodyId)
            {
                AttestableItem item = new AttestableItem (
                    "A" + advance.Identity.ToString (CultureInfo.InvariantCulture),
                    advance.Person.Name, advance.AmountCents, advance.Budget,
                    advance.Description, "Financial_CashAdvance", false, advance);

                if (!advance.Attested) // if not attested
                {
                    this._items.Add (item);
                }
                else if (!advance.PaidOut) // if attested, but still reversible
                {
                    this._attestedItems.Add (item);
                }
            }
        }
    }


    private void PopulateExpenses()
    {
        ExpenseClaims expenses = ExpenseClaims.ForOrganization (CurrentOrganization);

        foreach (ExpenseClaim expenseClaim in expenses)
        {
            if (this._attestationRights.ContainsKey (expenseClaim.BudgetId) ||
                expenseClaim.Budget.OwnerPersonId == Person.NobodyId)
            {
                Documents dox = expenseClaim.Documents;
                bool hasDox = (dox.Count > 0 ? true : false);

                AttestableItem item = new AttestableItem (
                    "E" + expenseClaim.Identity.ToString (CultureInfo.InvariantCulture),
                    expenseClaim.ClaimerCanonical, expenseClaim.AmountCents, expenseClaim.Budget,
                    expenseClaim.Description, "Financial_ExpenseClaim", hasDox, expenseClaim);

                if (expenseClaim.Attested)
                {
                    this._attestedItems.Add (item);
                }
                else
                {
                    this._items.Add (item);
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

            if (this._attestationRights.ContainsKey (invoice.BudgetId) ||
                invoice.Budget.OwnerPersonId == Person.NobodyId)
            {
                AttestableItem item = new AttestableItem ("I" + invoice.Identity.ToString (CultureInfo.InvariantCulture),
                    invoice.Supplier, invoice.BudgetAmountCents, invoice.Budget, invoice.Description,
                    "Financial_InvoiceInbound", hasDox, invoice);

                if (invoice.Attested)
                {
                    this._attestedItems.Add (item);
                }
                else
                {
                    this._items.Add (item);
                }
            }
        }
    }


    private void PopulateSalaries()
    {
        Salaries salaries = Salaries.ForOrganization (CurrentOrganization).WhereUnattested;

        // No unattestability for previously attested salaries because complex

        foreach (Salary salary in salaries)
        {
            if (this._attestationRights.ContainsKey (salary.PayrollItem.BudgetId) ||
                salary.PayrollItem.Budget.OwnerPersonId == Person.NobodyId)
            {
                this._items.Add (new AttestableItem ("S" + salary.Identity.ToString (CultureInfo.InvariantCulture),
                    salary.PayrollItem.PersonCanonical, salary.CostTotalCents, salary.PayrollItem.Budget,
                    "[Loc]Financial_SalarySpecification|[Date]" +
                    salary.PayoutDate.ToString (CultureInfo.InvariantCulture), "Financial_Salary", false, salary));
            }
        }
    }


    private void PopulateParleys()
    {
        Parleys parleys = Parleys.ForOrganization (CurrentOrganization).WhereUnattested;

        foreach (Parley parley in parleys)
        {
            if (this._attestationRights.ContainsKey (parley.BudgetId) || parley.Budget.OwnerPersonId == Person.NobodyId)
            {
                this._items.Add (new AttestableItem ("P" + parley.Identity.ToString (CultureInfo.InvariantCulture),
                    parley.Person.Canonical, parley.BudgetCents, parley.ParentBudget, parley.Name, "Financial_Parley",
                    false, parley));
            }
        }
    }


    protected class AttestableItem
    {
        public AttestableItem (string identity, string beneficiary, Int64 amountCents, FinancialAccount account,
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

    protected class AttestableItems : List<AttestableItem>
    {
        // empty class, we just want the name definition        
    }
}