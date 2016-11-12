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
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Pages_v4_Financial_AllocateFunds : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        attestationRights = GetAttestationRights();
        this.ComboClaimPerson.Authority = _currentUser.GetAuthority();
        this.ComboBudgetPerson.Authority = _currentUser.GetAuthority();

        if (!Page.IsPostBack)
        {
            this.DropMethod.Style [HtmlTextWriterStyle.Width] = "300px";

            // Populate

            foreach (int budgetId in attestationRights.Keys)
            {
                this.DropBudgets.Items.Add(new ListItem(FinancialAccount.FromIdentity(budgetId).Name, budgetId.ToString()));
            }

            this.LabelForTheItem.Text = "for the expense claim, purchase order, budget, or transfer order";
        }
    }

    private Dictionary<int, bool> attestationRights;


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
                result[account.Identity] = true;
            }
        }

        return result;
    }



    protected void Validator_TextAmount_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // Validate that the TextAmount box holds a parsable double.

        double dummy;

        args.IsValid = Double.TryParse(this.TextAmount.Text, NumberStyles.Float, new CultureInfo("sv-SE"), out dummy);
    }


    protected void Validate_BudgetPerson_Required(object source, ServerValidateEventArgs args)
    {
        // Validate that a person is selected in ComboBudgetPerson, but only if the control is visible.

        if (!this.ComboBudgetPerson.Visible)
        {
            args.IsValid = true;
            return;
        }

        args.IsValid = (this.ComboBudgetPerson.SelectedPerson != null ? true : false);
    }


    protected void Validate_CompanyName_Required(object source, ServerValidateEventArgs args)
    {
        // If we're in Create Purchase Order mode, verify that a company name has been entered.

        if (!this.TextPurchaseOrderCompany.Visible)
        {
            args.IsValid = true;
            return;
        }

        args.IsValid = (this.TextPurchaseOrderCompany.Text.Length > 0);
    }


    protected void Validate_ClaimPerson_Required(object source, ServerValidateEventArgs args)
    {
        // If we're in pre-attest-a-claim mode, verify that the person has been specified.

        if (!this.ComboClaimPerson.Visible)
        {
            args.IsValid = true;
            return;
        }

        args.IsValid = (this.ComboClaimPerson.SelectedPerson != null ? true : false);
    }


    protected void Validate_SubBudget_Required(object source, ServerValidateEventArgs args)
    {
        // If we're in transfer mode, verify that a budget has been specified.

        if (!this.DropSubBudget.Visible)
        {
            args.IsValid = true;
            return;
        }

        args.IsValid = (this.DropSubBudget.SelectedFinancialAccount != null ? true : false);
    }


    protected void DropMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        string currentValue = this.DropMethod.SelectedValue;
        bool showClaimPerson = false;
        bool showBudgetPerson = false;
        bool showCompanyName = false;
        bool showBudgetName = false;
        string descriptionText = "for the expense claim, purchase order, new budget, or transfer order";

        switch (currentValue)
        {
            case "ExpenseClaim":
                showClaimPerson = true;
                descriptionText = "for the expense claim";
                break;
            case "CreateSubBudget":
                showBudgetPerson = true;
                descriptionText = "for the new budget";
                break;
            case "TransferSubBudget":
                showBudgetName = true;
                descriptionText = "for the transfer order. THIS GOES IN THE BOOKKEEPING so be specific.";
                break;
            case "PurchaseOrder":
                showCompanyName = true;
                descriptionText = "- or rather, name of the company we're buying from";
                break;
        }

        this.ComboClaimPerson.Visible = showClaimPerson;
        this.ComboBudgetPerson.Visible = showBudgetPerson;
        this.DropSubBudget.Visible = showBudgetName;
        this.TextPurchaseOrderCompany.Visible = showCompanyName;

        this.LabelForTheItem.Text = descriptionText;
    }


    protected void DropBudgets_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.DropBudgets.SelectedValue != "0")
        {
            FinancialAccount selectedBudget = FinancialAccount.FromIdentity(Int32.Parse(this.DropBudgets.SelectedValue));

            if (selectedBudget.GetTree().Count > 1)
            {
                this.DropSubBudget.Populate(selectedBudget);
                this.DropMethod.Items[4].Enabled = true;
            }
            else
            {
                this.DropMethod.Items[4].Enabled = false;
            }
        }
    }


    protected void Submit_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            return; // do not process if validators fail
        }

        string method = this.DropMethod.SelectedValue;
        string result = string.Empty;

        switch (method)
        {
            case "ExpenseClaim":
                result = ProcessExpenseClaim();
                break;
            case "PurchaseOrder":
                // result = ProcessPurchaseOrder();
                break;
            case "CreateSubBudget":
                result = ProcessCreateSubBudget();
                break;
            case "TransferSubBudget":
                result = ProcessTransferSubBudget();
                break;
            default:
                throw new NotImplementedException(); // this is a good practice
        }

        // Reset values

        this.DropBudgets.SelectedIndex = 0;
        this.DropMethod.SelectedIndex = 0;
        this.TextAmount.Text = "0,00"; // TODO: LOCALIZE
        this.TextDescription.Text = string.Empty;

        // Notify user of result

        if (result.Length > 1)
        {
            Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('" + Server.HtmlEncode(result).Replace("'", "") + "');", true);

        }
    }

    private string ProcessTransferSubBudget()
    {
        int year = DateTime.Today.Year;
        FinancialAccount fromAccount = FinancialAccount.FromIdentity(Int32.Parse(this.DropBudgets.SelectedValue));
        FinancialAccount toAccount = this.DropSubBudget.SelectedFinancialAccount;
        double amount = Double.Parse(this.TextAmount.Text, NumberStyles.Float, new CultureInfo("sv-SE"));
        string description = this.TextDescription.Text;

        // Transfer the budget

        fromAccount.SetBudget(year, fromAccount.GetBudget(year) + amount);  // budgets are negative -- this decreases the budget
        toAccount.SetBudget(year, toAccount.GetBudget(year) - amount);  // budgets are negative -- this increases the budget

        // Set result

        return amount.ToString("N2") + " was transferred from " + fromAccount.Name + " to " + toAccount.Name + ".";
    }


    private string ProcessCreateSubBudget()
    {
        int year = DateTime.Today.Year;

        FinancialAccount parentAccount = FinancialAccount.FromIdentity(Int32.Parse(this.DropBudgets.SelectedValue));
        string budgetName = this.TextDescription.Text;
        Person owner = this.ComboBudgetPerson.SelectedPerson;
        double currentBudget = parentAccount.GetBudget(year);
        double amount = Double.Parse(this.TextAmount.Text, NumberStyles.Float, new CultureInfo("sv-SE"));

        FinancialAccount account = FinancialAccount.Create(Organization.PPSEid, budgetName, FinancialAccountType.Cost,
                                                           parentAccount.Identity);
        account.Owner = owner;
        account.SetBudget(year, -amount);  // cost accounts have a negative budget
        parentAccount.SetBudget(year, currentBudget + amount); // cost accounts have a negative budget -- this will LOWER the budget

        return "The budget " + budgetName + " was created, owned by " + owner.Canonical +
            " and with an initial budget for " + year.ToString() + " of " + 
            amount.ToString("N2") + ". Do not forget to instruct " + (owner.IsFemale? "her" : "him") + 
            " on the duties associated, such as attesting expenses, etc.";
    }


    private string ProcessExpenseClaim()
    {
        int year = DateTime.Today.Year;
        FinancialAccount account = FinancialAccount.FromIdentity(Int32.Parse(this.DropBudgets.SelectedValue));
        string expenseName = this.TextDescription.Text;
        Person claimer = this.ComboClaimPerson.SelectedPerson;
        Int64 amountCents = (Int64) (Double.Parse(this.TextAmount.Text, NumberStyles.Float, new CultureInfo("sv-SE")) * 100);

        // Create the expense claim record

        ExpenseClaim newClaim = ExpenseClaim.Create(claimer, Organization.PPSE, account, DateTime.Today, expenseName, amountCents);
        newClaim.Claimed = false;
        newClaim.Attest(_currentUser);

        return "The claim was created and pre-attested. " + claimer.Canonical +
               " has it in the list of approved expenses.";
    }
}


