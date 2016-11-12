using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Basic.Interfaces;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Pages_v4_Financial_AttestCosts : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        attestationRights = GetAttestationRights();
        budgetsRemainingLookup = new Dictionary<int, decimal>();

        if (!Page.IsPostBack)
        {
            PopulateAttestablesGrid();
        }
    }

    private Dictionary<int, bool> attestationRights;
    private Dictionary<int, decimal> budgetsRemainingLookup;

    private void PopulateAttestablesGrid()
    {
        AttestableItems attestableItems = new AttestableItems();

        PopulateClaims(attestableItems);
        PopulateInvoices(attestableItems);
        PopulateSalaries(attestableItems);
        PopulateParleys(attestableItems);

        this.GridAttestables.DataSource = attestableItems;
    }

    private void PopulateClaims(AttestableItems items)
    {
        ExpenseClaims allClaims = ExpenseClaims.FromOrganization(Organization.PPSE);
        ExpenseClaims unattestedClaims = new ExpenseClaims();

        // LINQ would be nice here. "Where Attested=0".

        foreach (ExpenseClaim claim in allClaims)
        {
            if (!claim.Attested && claim.Open)
            {
                if (attestationRights.ContainsKey(claim.BudgetId))
                {
                    unattestedClaims.Add(claim);
                }
            }
        }

        unattestedClaims.Sort(SortGridClaims);

        foreach (ExpenseClaim claim in unattestedClaims)
        {
            AddAttestableItem(items, claim);
        }
    }


    private void AddAttestableItem(AttestableItems items, ExpenseClaim claim)
    {
        items.Add(new AttestableItem("E" + claim.Identity.ToString(), claim.ClaimerCanonical, (decimal)claim.Amount, claim.Budget, claim.Description, "Expense Claim #" + claim.Identity, claim));
    }

    private void AddAttestableItem(AttestableItems items, InboundInvoice invoice)
    {
        items.Add(new AttestableItem("I" + invoice.Identity.ToString(), invoice.Supplier, (decimal)invoice.Amount, invoice.Budget, "See scans:", "Inbound Invoice #" + invoice.Identity, invoice));
    }

    private void AddAttestableItem(AttestableItems items, Salary salary)
    {
        items.Add(new AttestableItem("S" + salary.Identity.ToString(), salary.PayrollItem.PersonCanonical, (decimal)(salary.CostTotalCents / 100.0m), salary.PayrollItem.Budget, String.Format("Base pay of {0:N0} plus mods & taxes", salary.BaseSalaryDecimal), "Salary #" + salary.Identity, salary));
    }

    private void AddAttestableItem(AttestableItems items, Parley parley)
    {
        items.Add(new AttestableItem("P" + parley.Identity.ToString(), parley.Person.Canonical, parley.GuaranteeDecimal, parley.Budget, String.Format("{2}: Budget of {0:N0} with guarantee of {1:N0}", parley.BudgetDecimal, parley.GuaranteeDecimal, parley.Name), "Conference #" + parley.Identity, parley));
    }


    private void PopulateInvoices(AttestableItems items)
    {
        InboundInvoices openInvoices = InboundInvoices.ForOrganization(Organization.PPSE);
        InboundInvoices unattestedInvoices = new InboundInvoices();

        foreach (InboundInvoice invoice in openInvoices)
        {
            if (!invoice.Attested && invoice.Open)
            {
                if (attestationRights.ContainsKey(invoice.BudgetId))
                {
                    unattestedInvoices.Add(invoice);
                }
            }
        }

        unattestedInvoices.Sort(SortGridInvoices);

        foreach (InboundInvoice invoice in unattestedInvoices)
        {
            AddAttestableItem(items, invoice);
        }
    }


    private void PopulateSalaries(AttestableItems items)
    {
        Salaries salaries = Salaries.ForOrganization(Organization.PPSE);
        Salaries unattestedSalaries = new Salaries();

        // LINQ would be nice here. "Where Attested=0".

        foreach (Salary salary in salaries)
        {
            if (!salary.Attested)
            {
                if (attestationRights.ContainsKey(salary.PayrollItem.BudgetId) && salary.PayrollItem.PersonId != _currentUser.Identity)
                {
                    unattestedSalaries.Add(salary);
                }
                else if (salary.PayrollItem.ReportsToPersonId == _currentUser.Identity)
                {
                    unattestedSalaries.Add(salary);
                }
            }
        }

        foreach (Salary salary in unattestedSalaries)
        {
            AddAttestableItem(items, salary);
        }
    }


    private void PopulateParleys(AttestableItems items)
    {
        Parleys parleys = Parleys.ForOrganization(Organization.PPSE);
        Parleys unattestedParleys = new Parleys();

        // LINQ would be nice here. "Where Attested=0".

        foreach (Parley parley in parleys)
        {
            if (!parley.Attested)
            {
                if (attestationRights.ContainsKey(parley.BudgetId))
                {
                    unattestedParleys.Add(parley);
                }
            }
        }

        foreach (Parley parley in unattestedParleys)
        {
            AddAttestableItem(items, parley);
        }
    }


    private Dictionary<int, bool> GetAttestationRights()
    {
        // Right now, this function is quite primitive. At some point in the future, it needs to take into
        // account that a budget may have several attesters. Right now, it just loops over all accounts and
        // checks the owner.

        Dictionary<int, bool> result = new Dictionary<int, bool>();
        FinancialAccounts accounts = FinancialAccounts.ForOrganization(Organization.PPSE);  // TODO: Hardcoded

        foreach (FinancialAccount account in accounts)
        {
            if (account.OwnerPersonId == _currentUser.Identity)
            {
                result [account.Identity] = true;
            }
        }

        return result;
    }


    private static int SortGridClaims (ExpenseClaim claim1, ExpenseClaim claim2)
    {
        return DateTime.Compare(claim2.CreatedDateTime, claim1.CreatedDateTime);
    }


    private static int SortGridInvoices (InboundInvoice invoice1, InboundInvoice invoice2)
    {
        return DateTime.Compare(invoice1.CreatedDateTime, invoice2.CreatedDateTime);
    }


    protected void GridAttestables_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            AttestableItem item = (AttestableItem) e.Item.DataItem;

            if (item == null)
            {
                return;
            }

            // Set Description field

            Label description = (Label)e.Item.FindControl("LabelDescription");

            description.Text = item.Description;

            if (item.Item is InboundInvoice)
            {
                InboundInvoice invoice = item.Item as InboundInvoice;

                Controls_v4_DocumentList docList = (Controls_v4_DocumentList)e.Item.FindControl("DocList");
                docList.Documents = invoice.Documents;
                docList.Visible = true;
                description.Visible = false;

                HyperLink editLink = (HyperLink)e.Item.FindControl("LinkEdit");
                editLink.Attributes["href"] = "#";
                editLink.Attributes["onclick"] = String.Format("return ShowInboundInvoiceForm('{0}','{1}');",
                                                               invoice.Identity, e.Item.ItemIndex);
            }
            else if (item.Item is ExpenseClaim)
            {
                ExpenseClaim claim = item.Item as ExpenseClaim;

                HyperLink editLink = (HyperLink)e.Item.FindControl("LinkEdit");
                editLink.Attributes["href"] = "#";
                editLink.Attributes["onclick"] = String.Format("return ShowExpenseClaimForm('{0}','{1}');",
                                                               claim.Identity, e.Item.ItemIndex);
            }

            // Calculate or retrieve remaining budget

            if (!budgetsRemainingLookup.ContainsKey(item.Budget.Identity))
            {
                int year = DateTime.Today.Year; // HACK: Should be item's date

                Int64 budgetCents = (Int64) item.Budget.GetBudget(year)*100;

                Int64 centsUsed = item.Budget.GetDeltaCents(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1));

                decimal budgetRemaining = (-budgetCents - centsUsed) / 100.0m;

                budgetsRemainingLookup[item.Budget.Identity] = budgetRemaining;
            }

            Label labelBudgetRemaining = (Label)e.Item.FindControl("LabelBudgetRemaining");

            labelBudgetRemaining.Text = String.Format("{0:N2}", budgetsRemainingLookup[item.Budget.Identity]);

        }
        
        
        // Set the images for the status indicators.

        return; // fix later

        if (e.Item is GridDataItem)
        {
            ExpenseClaim claim = (ExpenseClaim)e.Item.DataItem;

            if (claim == null)
            {
                return;
            }

            Label labelBudgetYear = (Label)e.Item.FindControl("LabelBudgetYear");

            if (claim.BudgetId == 0)
            {
                labelBudgetYear.Text = "UNBUDGETED!";
            }
            else
            {
                labelBudgetYear.Text = claim.Budget.Name + ", " + claim.BudgetYear.ToString();
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkEdit");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowExpenseClaimForm('{0}','{1}');",
                                                           claim.Identity, e.Item.ItemIndex);

        }
    }

    protected void GridAttestables_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        // This is FUCKING REDUNDANT. We already HAVE this data. We shouldn't need to get it AGAIN.

        PopulateAttestablesGrid();
    }



    protected void GridInvoices_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            InboundInvoice invoice = (InboundInvoice)e.Item.DataItem;

            if (invoice == null)
            {
                return;
            }

            Label labelBudgetYear = (Label)e.Item.FindControl("LabelBudgetYear");

            if (invoice.BudgetId == 0)
            {
                labelBudgetYear.Text = "UNBUDGETED!";
            }
            else
            {
                labelBudgetYear.Text = invoice.Budget.Name + ", " + invoice.CreatedDateTime.Year.ToString();
            }


            Controls_v4_DocumentList docList = (Controls_v4_DocumentList) e.Item.FindControl("DocListInvoice");

            docList.Documents = invoice.Documents;


            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkEdit");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowInboundInvoiceForm('{0}','{1}');",
                                                           invoice.Identity, e.Item.ItemIndex);

        }
    }




    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        if (e.Argument == "Rebind")
        {
            //this.GridTransactions.MasterTableView.SortExpressions.Clear();
            //this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            PopulateAttestablesGrid();
            this.GridAttestables.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            /* This should not happen. */
        }
    }

    /*
    protected void ButtonAttestSalaries_Click(object sender, EventArgs e)
    {
        List<string> identityStrings = new List<string>();

        foreach (string indexString in this.GridSalaries.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int salaryId = (int)this.GridSalaries.MasterTableView.DataKeyValues[index]["Identity"];
            Salary salary = Salary.FromIdentity(salaryId);

            // Mark as attested

            bool mayAttest = false;

            if (attestationRights.ContainsKey(salary.PayrollItem.BudgetId) && salary.PayrollItem.PersonId != _currentUser.Identity)
            {
                mayAttest = true;
            }

            if (salary.PayrollItem.ReportsToPersonId == _currentUser.Identity)
            {
                mayAttest = true;
            }

            if (mayAttest)
            {
                salary.Attest(_currentUser);
                PirateWeb.Logic.Support.PWEvents.CreateEvent(
                    EventSource.PirateWeb, EventType.SalaryAttested, _currentUser.Identity,
                    salary.PayrollItem.OrganizationId, 0, 0, salaryId, string.Empty);
            }
        }

        this.GridSalaries.Rebind();

    }

    protected void ButtonAttestInvoices_Click(object sender, EventArgs e)
    {
        List<string> identityStrings = new List<string>();

        foreach (string indexString in this.GridInvoices.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int inboundInvoiceId = (int)this.GridInvoices.MasterTableView.DataKeyValues[index]["Identity"];
            InboundInvoice invoice = InboundInvoice.FromIdentity(inboundInvoiceId);

            // Mark as attested


        }

        this.GridInvoices.Rebind();

    }

    protected void ButtonAttestClaims_Click(object sender, EventArgs e)
    {
        List<string> identityStrings = new List<string>();

        foreach (string indexString in this.GridExpenseClaims.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            int claimId = (int)this.GridExpenseClaims.MasterTableView.DataKeyValues[index]["Identity"];
            ExpenseClaim claim = ExpenseClaim.FromIdentity(claimId);

            // Mark as attested

            if (attestationRights.ContainsKey(claim.BudgetId))
            {
                claim.Attest(_currentUser);
                PirateWeb.Logic.Support.PWEvents.CreateEvent(
                    EventSource.PirateWeb, EventType.ExpenseAttested, _currentUser.Identity,
                    claim.OrganizationId, 0, claim.ClaimingPersonId, claimId, string.Empty);
            }
        }

        this.GridExpenseClaims.Rebind();

    }
    */

    protected class AttestableItem
    {
        public AttestableItem (string identity, string beneficiary, decimal amountDecimal, FinancialAccount account, string description, string identityDisplay, IHasIdentity item)
        {
            this.identityDisplay = identityDisplay;
            this.identity = identity;
            this.beneficiary = beneficiary;
            this.amountDecimal = amountDecimal;
            this.budget = account;
            this.description = description;
            this.item = item;
        }

        public FinancialAccount Budget { get { return this.budget; }}
        public string BudgetName { get { return this.budget.Name; } }
        public string Identity { get { return this.identity; }}
        public string Beneficiary { get { return this.beneficiary; } }
        public decimal AmountRequestedDecimal { get { return this.amountDecimal; }}
        public string Description { get { return this.description; }}
        public string IdentityDisplay { get { return this.identityDisplay; } }
        public IHasIdentity Item {get { return this.item; }}

        private FinancialAccount budget;
        private string identity;
        private string beneficiary;
        private decimal amountDecimal;
        private string description;
        private string identityDisplay;
        private IHasIdentity item;
    }

    protected class AttestableItems:List<AttestableItem>
    {
        // empty class, just want the typedef        
    }

    protected void ButtonAttest_Click(object sender, EventArgs e)
    {
        List<string> identityStrings = new List<string>();

        foreach (string indexString in this.GridAttestables.SelectedIndexes)
        {
            int index = Int32.Parse(indexString);
            string itemIdentityString = (string) this.GridAttestables.MasterTableView.DataKeyValues[index]["Identity"];
            int itemIdentity = Int32.Parse(itemIdentityString.Substring(1));


            // Mark items as attested

            switch (itemIdentityString[0])
            {
                case 'E':
                    ExpenseClaim claim = ExpenseClaim.FromIdentity(itemIdentity);

                    if (attestationRights.ContainsKey(claim.BudgetId))
                    {
                        claim.Attest(_currentUser);
                        Activizr.Logic.Support.PWEvents.CreateEvent(
                            EventSource.PirateWeb, EventType.ExpenseAttested, _currentUser.Identity,
                            claim.OrganizationId, 0, claim.ClaimingPersonId, claim.Identity, string.Empty);
                    }
                    break;
                case 'I':
                    InboundInvoice invoice = InboundInvoice.FromIdentity(itemIdentity);

                    if (attestationRights.ContainsKey(invoice.BudgetId))
                    {
                        invoice.Attest(_currentUser);
                        Activizr.Logic.Support.PWEvents.CreateEvent(
                            EventSource.PirateWeb, EventType.InboundInvoiceAttested, _currentUser.Identity,
                            invoice.OrganizationId, 0, 0, invoice.Identity, string.Empty);
                    }
                    break;
                case 'S':
                    Salary salary = Salary.FromIdentity(itemIdentity);

                    // Mark as attested

                    bool mayAttest = false;

                    if (attestationRights.ContainsKey(salary.PayrollItem.BudgetId) && salary.PayrollItem.PersonId != _currentUser.Identity)
                    {
                        mayAttest = true;
                    }

                    if (salary.PayrollItem.ReportsToPersonId == _currentUser.Identity)
                    {
                        mayAttest = true;
                    }

                    if (mayAttest)
                    {
                        salary.Attest(_currentUser);
                        Activizr.Logic.Support.PWEvents.CreateEvent(
                            EventSource.PirateWeb, EventType.SalaryAttested, _currentUser.Identity,
                            salary.PayrollItem.OrganizationId, 0, 0, salary.Identity, string.Empty);
                    } break;
                case 'P':
                    Parley parley = Parley.FromIdentity(itemIdentity);

                    if (attestationRights.ContainsKey(parley.BudgetId))
                    {
                        parley.Attest(_currentUser);
                    }
                    break;

            }

        }

        this.GridAttestables.Rebind();
    }
}


