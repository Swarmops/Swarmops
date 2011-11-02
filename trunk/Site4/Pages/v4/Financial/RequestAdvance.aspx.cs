using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Pages_v4_Accounting_RequestAdvance : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.TextBank.Text = _currentUser.BankName;
            if (_currentUser.BankClearing.Length != 0)
            {
                this.TextAccount.Text = _currentUser.BankAccount;
                this.TextBankClearing.Text = _currentUser.BankClearing;
            }
            else
            {
                this.TextBankClearing.Text = string.Empty;
                this.TextAccount.Text = string.Empty;
            }

            this.TextAmount.Text = (0.0).ToString("N2", new CultureInfo("sv-SE"));

            this.DropBudgets.Populate(Organization.FromIdentity(1), FinancialAccountType.Cost);
            this.TextDescription.Style[HtmlTextWriterStyle.Width] = "300px";

            if (_authority.HasPermission(Permission.CanEditPeople, 1, 1, Authorization.Flag.AnyGeography))
            {
                this.UpdateImpersonation.Visible = true;
                this.PanelImpersonation.Visible = true;
            }
        }

        this.ComboPersonImpersonated.Authority = _authority;
    }

    protected void DropBudgets_SelectedNodeChanged (object sender, EventArgs e)
    {
        FinancialAccount account = DropBudgets.SelectedFinancialAccount;

        this.LabelBudgetOwner.Text = account.Owner.Name + " owns this budget.";
    }

    protected void ButtonSubmitClaim_Click(object sender, EventArgs e)
    {
        // First, if there's an upload that the user hasn't processed, process it first.

        // If args were invalid, abort

        if (!Page.IsValid)
        {
            return;
        }

        // Set bank details, if applicable

        if (!this.CheckImpersonate.Checked)
        {
            _currentUser.BankName = this.TextBank.Text.Trim();
            _currentUser.BankClearing = Formatting.CleanNumber(this.TextBankClearing.Text.Trim());
            _currentUser.BankAccount = Formatting.CleanNumber(this.TextAccount.Text.Trim());
        }

        // Read the form data

        Person advancePerson = Person.FromIdentity(_currentUser.Identity);

        if (this.CheckImpersonate.Checked && this.ComboPersonImpersonated.SelectedPerson != null)
        {
            advancePerson = Person.FromIdentity(this.ComboPersonImpersonated.SelectedPerson.Identity);
        }

        int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
        Organization organization = Organization.FromIdentity(organizationId);
        Int64 amountCents = (Int64) (Double.Parse(this.TextAmount.Text, new CultureInfo("sv-SE")) * 100);
        FinancialAccount account = this.DropBudgets.SelectedFinancialAccount;
        DateTime created = DateTime.Now;
        int budgetYear = created.Year;
        DateTime expenseDate = DateTime.Today;
        string description = this.TextDescription.Text;

        // Create the POSITIVE transaction and claim

        ExpenseClaim newClaim = ExpenseClaim.Create(advancePerson,
            organization, account, expenseDate, description, amountCents);
        newClaim.KeepSeparate = true; // assures separate payout
        newClaim.Validated = true; // no validation: no receipts (yet)

        // Create the event for PirateBot-Mono to send off mails for attestation etc.

        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ExpenseCreated,
                                                   _currentUser.Identity, organizationId, Geography.RootIdentity, advancePerson.Identity,
                                                   newClaim.Identity, string.Empty);

        // Create the NEGATIVE transaction and claim

        newClaim = ExpenseClaim.Create(advancePerson,
            organization, account, expenseDate, description, -amountCents);

        // This claim is automatically validated, attested

        newClaim.Attested = true;
        newClaim.Validated = true;

  		Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('Your cash request has been submitted. Note that this is a LOAN from the organization until the corresponding expenses have been documented.');", true);

        // Clear the text fields

        this.TextDescription.Text = string.Empty;
        this.TextAmount.Text = "0,00"; // TODO: LOCALIZE BY CULTURE
    }


    protected void Validator_DropBudgets_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // A budget must be selected.

        // First, an organization must be selected.

        if (this.DropOrganizations.SelectedValue == "0")
        {
            args.IsValid = false;
            return;
        }

        if (this.DropBudgets.SelectedFinancialAccount == null)
        {
            args.IsValid = false;
            return;
        }

        args.IsValid = true;
    }


    protected void Validator_TextAmount_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // Validate that the TextAmount box holds a parsable non-negative double.

        double amount;

        args.IsValid = Double.TryParse(this.TextAmount.Text, NumberStyles.Float, new CultureInfo("sv-SE"), out amount);

        if (args.IsValid && amount < 0.0)
        {
            args.IsValid = false;
        }
    }



    protected void CheckImpersonate_CheckedChanged(object sender, EventArgs e)
    {
        SetImpersonationParameters();
    }

    protected void ComboPersonImpersonated_SelectedPersonChanged (object sender, EventArgs e)
    {
        SetImpersonationParameters();
    }

    private void SetImpersonationParameters()
    {
        if (this.CheckImpersonate.Checked && this.ComboPersonImpersonated.SelectedPerson != null)
        {
            Person impersonatedPerson = this.ComboPersonImpersonated.SelectedPerson;
            this.TextBank.Text = impersonatedPerson.BankName;
            this.TextAccount.Text = impersonatedPerson.BankAccount;
            this.LabelBankDetails.Text = "the bank details of " + impersonatedPerson.Canonical;
            this.TextBank.Enabled = false;      // The bank details are disabled here for security reasons:
            this.TextAccount.Enabled = false;   // only the owner of the account is able to set their own bank account
            this.TextBankClearing.Enabled = false;
        }
        else
        {
            this.LabelBankDetails.Text = "your bank details";
            this.TextBank.Text = _currentUser.BankName.Trim();
            this.TextBankClearing.Text = _currentUser.BankClearing.Trim();
            this.TextAccount.Text = _currentUser.BankAccount.Trim();
            this.TextBank.Enabled = true;
            this.TextAccount.Enabled = true;
        }
    }

    protected void DropOrganizations_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void ValidatorClearingNumbersOnly_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = true;

        if (Formatting.CleanNumber(this.TextBankClearing.Text.Trim()).Length != 4)
        {
            args.IsValid = false;
        }
    }

    protected void ValidatorAccountNumbersOnly_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = true;

        if (Formatting.CleanNumber(this.TextAccount.Text.Trim()).Length != this.TextAccount.Text.Trim().Length)
        {
            args.IsValid = false;
        }
    }

    protected void ValidatorAccountNoClearing_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = true;

        if (Formatting.CleanNumber(this.TextAccount.Text.Trim()).StartsWith(Formatting.CleanNumber(this.TextBankClearing.Text.Trim())))
        {
            args.IsValid = false;
        }
    }

}
