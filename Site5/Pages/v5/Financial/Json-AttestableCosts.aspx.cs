using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Interfaces;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

public partial class Pages_v5_Finance_Json_AttestableCosts : DataV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get all attestable items

        _attestationRights = GetAttestationRights();
        _items = new AttestableItems();

        PopulateCashAdvances();
        PopulateExpenses();
        PopulateInboundInvoices();
        PopulateSalaries();
        PopulateParleys();

        // Format as JSON and return

        Response.ContentType = "application/json";
        string json = FormatAsJson();
        Response.Output.WriteLine(json);
        Response.End();
    }

    private string FormatAsJson()
    {
        StringBuilder result = new StringBuilder(16384);

        string hasDoxString =
            "<img src=\\\"/Images/Icons/iconshock-glass-16px.png\\\" onmouseover=\\\"this.src='/Images/Icons/iconshock-glass-16px-hot.png';\\\" onmouseout=\\\"this.src='/Images/Icons/iconshock-glass-16px.png';\\\" baseid=\\\"{5}\\\" class=\\\"LocalViewDox\\\" style=\\\"cursor:pointer\\\" />";

        result.Append("{\"rows\":[");

        foreach (AttestableItem item in _items)
        {
            result.Append("{");
            result.AppendFormat(
                "\"item\":\"{0}\",\"beneficiary\":\"{1}\",\"description\":\"{2}\",\"budgetName\":\"{3}\",\"amountRequested\":\"{4:N2}\",\"itemId\":\"{5}\"," +
                "\"dox\":\"" + (item.HasDox? hasDoxString: "&nbsp;") + "\"," +
                "\"actions\":\"<span style=\\\"position:relative;top:3px\\\">" +
                    "<img id=\\\"IconApproval{5}\\\" class=\\\"LocalIconApproval\\\" baseid=\\\"{5}\\\" height=\\\"16\\\" width=\\\"16\\\" />" +
                    "<img id=\\\"IconApproved{5}\\\" class=\\\"LocalIconApproved\\\" baseid=\\\"{5}\\\" height=\\\"16\\\" width=\\\"16\\\" />&nbsp;&nbsp;" +
                    "<img id=\\\"IconDenial{5}\\\" class=\\\"LocalIconDenial\\\" baseid=\\\"{5}\\\" height=\\\"16\\\" width=\\\"16\\\" />" +
                    "<img id=\\\"IconDenied{5}\\\" class=\\\"LocalIconDenied\\\" baseid=\\\"{5}\\\" height=\\\"16\\\" width=\\\"16\\\" /></span>\"",
                 JsonSanitize(GetGlobalResourceObject("Global", item.IdentityDisplay).ToString()), JsonSanitize(item.Beneficiary), JsonSanitize(TryLocalize(item.Description)), JsonSanitize(item.BudgetName),
                item.AmountRequestedCents/100.0, item.Identity);
            result.Append("},");
        }

        result.Remove(result.Length - 1, 1); // remove last comma

        result.Append("]}");

        return result.ToString();
    }

    private Dictionary<int, bool> _attestationRights;
    private AttestableItems _items;

    protected class AttestableItem
    {
        public AttestableItem(string identity, string beneficiary, Int64 amountCents, FinancialAccount account, string description, string identityDisplay, bool hasDox, IHasIdentity item)
        {
            this.IdentityDisplay = identityDisplay;
            this.Identity = identity;
            this.Beneficiary = beneficiary;
            this.AmountRequestedCents = amountCents;
            this.Budget = account;
            this.Description = description;
            this.HasDox = hasDox;
            this.Item = item;
        }

        public FinancialAccount Budget { get; private set; }
        public string BudgetName { get { return this.Budget.Name; } }
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



    private Dictionary<int, bool> GetAttestationRights()
    {
        // Right now, this function is quite primitive. At some point in the future, it needs to take into
        // account that a budget may have several attesters. Right now, it just loops over all accounts and
        // checks the owner.

        Dictionary<int, bool> result = new Dictionary<int, bool>();
        FinancialAccounts accounts = FinancialAccounts.ForOrganization(this.CurrentOrganization);

        foreach (FinancialAccount account in accounts)
        {
            if (account.OwnerPersonId == this.CurrentUser.Identity)
            {
                result[account.Identity] = true;
            }
        }

        return result;
    }


    private void PopulateCashAdvances()
    {
        CashAdvances advances = CashAdvances.ForOrganization(this.CurrentOrganization).WhereUnattested;

        foreach (CashAdvance advance in advances)
        {
            if (_attestationRights.ContainsKey(advance.BudgetId) || advance.Budget.OwnerPersonId == Person.NobodyId)
            {
                _items.Add(new AttestableItem("A" + advance.Identity.ToString(CultureInfo.InvariantCulture),
                                              advance.Person.Name, advance.AmountCents, advance.Budget,
                                              advance.Description, "Financial_CashAdvance", false, advance));
            }
        }
    }


    private void PopulateExpenses()
    {
        ExpenseClaims expenses = ExpenseClaims.ForOrganization(this.CurrentOrganization).WhereUnattested;

        foreach (var expenseClaim in expenses)
        {
            if (_attestationRights.ContainsKey(expenseClaim.BudgetId) || expenseClaim.Budget.OwnerPersonId == Person.NobodyId)
            {
                Documents dox = expenseClaim.Documents;
                bool hasDox = (dox.Count > 0 ? true : false);

                _items.Add(new AttestableItem("E" + expenseClaim.Identity.ToString(CultureInfo.InvariantCulture), expenseClaim.ClaimerCanonical, expenseClaim.AmountCents, expenseClaim.Budget, expenseClaim.Description, "Financial_ExpenseClaim", hasDox, expenseClaim));
            }
        }
    }


    private void PopulateInboundInvoices()
    {
        InboundInvoices invoices = InboundInvoices.ForOrganization(this.CurrentOrganization).WhereUnattested;

        foreach (InboundInvoice invoice in invoices)
        {
            Documents dox = invoice.Documents;
            bool hasDox = (dox.Count > 0 ? true : false);

            if (_attestationRights.ContainsKey(invoice.BudgetId) || invoice.Budget.OwnerPersonId == Person.NobodyId)
            {
                _items.Add(new AttestableItem("I" + invoice.Identity.ToString(CultureInfo.InvariantCulture), invoice.Supplier, invoice.AmountCents, invoice.Budget, invoice.InvoiceReference, "Financial_InvoiceInbound", hasDox, invoice));
            }
        }
    }


    private void PopulateSalaries()
    {
        Salaries salaries = Salaries.ForOrganization(this.CurrentOrganization).WhereUnattested;

        foreach (Salary salary in salaries)
        {
            if (_attestationRights.ContainsKey(salary.PayrollItem.BudgetId) || salary.PayrollItem.Budget.OwnerPersonId == Person.NobodyId)
            {
                _items.Add(new AttestableItem("S" + salary.Identity.ToString(CultureInfo.InvariantCulture), salary.PayrollItem.PersonCanonical, salary.CostTotalCents, salary.PayrollItem.Budget, "[Loc]Financial_SalarySpecification|[Date]" + salary.PayoutDate.ToString(CultureInfo.InvariantCulture), "Financial_Salary", false, salary));
            }
        }
    }


    private void PopulateParleys()
    {
        Parleys parleys = Parleys.ForOrganization(this.CurrentOrganization).WhereUnattested;

        foreach (Parley parley in parleys)
        {
            if (_attestationRights.ContainsKey(parley.BudgetId) || parley.Budget.OwnerPersonId == Person.NobodyId)
            {
                _items.Add(new AttestableItem("P" + parley.Identity.ToString(CultureInfo.InvariantCulture), parley.Person.Canonical, parley.BudgetCents, parley.ParentBudget, parley.Name, "Financial_Parley", false, parley));
            }
        }
    }

}