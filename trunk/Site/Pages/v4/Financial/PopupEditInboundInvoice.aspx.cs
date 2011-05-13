using System;
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
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

public partial class Pages_v4_Financial_PopupEditInboundInvoice : PageV4Base
{
    private InboundInvoice _invoice = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        _invoice = InboundInvoice.FromIdentity(Int32.Parse(Request.QueryString["InboundInvoiceId"]));

        // TODO: Verify authority (economy assistant or budget owner)

        if (!Page.IsPostBack)
        {
            this.LabelInvoiceFrom.Text = _invoice.Supplier;
            this.LabelInvoiceReceivedDate.Text = _invoice.CreatedDateTime.ToString("yyyy-MM-dd");
            this.LabelAttested.Text = _invoice.Attested ? "Yes." : "No.";

            this.DropAccounts.Populate(_invoice.Organization, FinancialAccountType.Cost);

            if (_invoice.BudgetId != 0)
            {
                this.DropAccounts.SelectedFinancialAccount = _invoice.Budget;
            }

            this.DateInvoiceDue.SelectedDate = _invoice.DueDate;
            this.TextAmount.Text = _invoice.Amount.ToString("N2", new CultureInfo("sv-SE"));
        }

        // The DocumentList control does not hold state - yet - so must be initialized always

        this.DocumentList.Documents = Documents.ForObject(_invoice);
    }


    protected void TextAmount_TextChanged(object sender, EventArgs e)
    {
        ShowReattestationWarning();
    }

    protected void DropAccounts_SelectedNodeChanged(object sender, EventArgs e)
    {
        ShowReattestationWarning();
    }

    protected void DateInvoiceDue_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        // nothing
    }

    private void ShowReattestationWarning()
    {
        if (_invoice.Attested && _invoice.Open)
        {
            this.PanelWarningReattestation.Visible = true;
        }
    }

    protected void ButtonSaveChanges_Click(object sender, EventArgs e)
    {
        // If the values in the controls are not valid, abort:

        if (!Page.IsValid)
        {
            return;
        }

        // Move on and parse values.

        Int64 newAmountCents = (Int64) (Double.Parse(this.TextAmount.Text, new CultureInfo("sv-SE")) * 100);
        
        FinancialAccount newAccount = this.DropAccounts.SelectedFinancialAccount;
        DateTime newDueDate = (DateTime) this.DateInvoiceDue.SelectedDate;

        if (newAccount.Identity != _invoice.BudgetId)
        {
            // TODO: Reset pre-attestation amount (which is not valid if budget changes)

            if (_invoice.Attested)
            {
                _invoice.Deattest(_currentUser);
            }

            _invoice.SetBudget (newAccount, _currentUser);
        }

        if (newAmountCents != _invoice.AmountCents)
        {
            if (_invoice.Attested)
            {
                if (newAmountCents > _invoice.AmountCents)
                {
                    _invoice.Deattest(_currentUser);
                }
            }

            _invoice.SetAmountCents (newAmountCents, _currentUser);
        }

        if (newDueDate != _invoice.DueDate)
        {
            _invoice.DueDate = newDueDate;
        }

        // Close and rebind

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
    }

    protected void Validator_DropAccounts_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // A budget must be selected.

        if (this.DropAccounts.SelectedFinancialAccount == null)
        {
            args.IsValid = false;
            return;
        }

        args.IsValid = true;
    }


    protected void Validator_TextAmount_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // Validate that the TextAmount box holds a parsable double.

        double dummy;

        args.IsValid = Double.TryParse(this.TextAmount.Text, NumberStyles.Any, new CultureInfo("sv-SE"), out dummy);
    }


    protected void Validator_DateInvoiceDue_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (this.DateInvoiceDue.SelectedDate == null)
        {
            args.IsValid = false;
            return;
        }

        args.IsValid = true;
    }


    protected void ButtonConfirmedKill_Click(object sender, EventArgs e)
    {
        if (_invoice.Attested)
        {
            _invoice.Deattest(_currentUser);
        }

        FinancialValidations.Create(FinancialValidationType.Kill, _invoice, _currentUser);

        // Set the state to Closed, Unvalidated

        _invoice.Open = false;

        // Undo all financial transaction changes

        FinancialTransaction transaction = _invoice.FinancialTransaction;

        transaction.RecalculateTransaction(new Dictionary<int, Int64>(), _currentUser);

        // Mark transaction as invalid in description

        transaction.Description = "Inbound Invoice #" + _invoice.Identity.ToString() + " (killed/zeroed)";

        // Finally, close form and rebind

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
    }
}
