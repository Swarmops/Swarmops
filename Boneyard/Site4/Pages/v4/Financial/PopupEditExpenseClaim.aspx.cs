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

public partial class Pages_v4_Financial_PopupEditExpenseClaim : PageV4Base
{
    private ExpenseClaim _expenseClaim = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        _expenseClaim = ExpenseClaim.FromIdentity(Int32.Parse(Request.QueryString["ExpenseClaimId"]));

        // TODO: Verify authority (economy assistant or claimer)

        if (!Page.IsPostBack)
        {
            this.LabelClaimer.Text = _expenseClaim.ClaimerCanonical;
            this.LabelClaimDate.Text = _expenseClaim.CreatedDateTime.ToString("yyyy-MM-dd HH:mm");
            this.TextDescription.Text = _expenseClaim.Description;
            this.LabelAttested.Text = _expenseClaim.Attested? "Yes." : "No.";
            this.LabelValidated.Text = _expenseClaim.Validated? "Yes." : "No.";

            if (_expenseClaim.AmountCents < 0)
            {
                // If the expense amount is negative, include income accounts (donations, etc)

                this.DropAccounts.Populate(_expenseClaim.Organization, FinancialAccountType.Result);
            }
            else
            {
                this.DropAccounts.Populate(_expenseClaim.Organization, FinancialAccountType.Cost);
            }

            if (_expenseClaim.BudgetId != 0)
            {
                this.DropAccounts.SelectedFinancialAccount = _expenseClaim.Budget;
            }

            this.DateExpense.SelectedDate = _expenseClaim.ExpenseDate;
            this.TextAmount.Text = _expenseClaim.Amount.ToString("N2", new CultureInfo("sv-SE"));
            this.TextDescription.Style[HtmlTextWriterStyle.Width] = "250px";
        }

        // The DocumentList control does not hold state - yet - so must be initialized always

        this.DocumentList.Documents = Documents.ForObject(_expenseClaim);
    }


    protected void TextAmount_TextChanged(object sender, EventArgs e)
    {
        ShowReattestationWarning();
        ShowRevalidationWarning();
    }

    protected void DropAccounts_SelectedNodeChanged(object sender, EventArgs e)
    {
        ShowReattestationWarning();
    }

    protected void DateExpense_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        ShowRevalidationWarning();
    }

    private void ShowReattestationWarning()
    {
        if (_expenseClaim.Attested && _expenseClaim.Open)
        {
            this.PanelWarningReattestation.Visible = true;
        }
    }

    private void ShowRevalidationWarning()
    {
        if (_expenseClaim.Validated && _expenseClaim.Open)
        {
            this.PanelWarningRevalidation.Visible = true;
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

        double newAmount = Double.Parse(this.TextAmount.Text, new CultureInfo("sv-SE"));
        FinancialAccount newAccount = this.DropAccounts.SelectedFinancialAccount;
        string newDescription = this.TextDescription.Text;
        DateTime newDate = (DateTime) this.DateExpense.SelectedDate;

        if (newDescription != _expenseClaim.Description)
        {
            _expenseClaim.Description = newDescription;
            // Nothing invalidated because description updated

            // Should it?
        }

        if (newAccount.Identity != _expenseClaim.BudgetId)
        {
            if (_expenseClaim.Attested)
            {
                _expenseClaim.Attested = false;
            }

            if (_expenseClaim.BudgetYear == 0)
            {
                _expenseClaim.BudgetYear = _expenseClaim.CreatedDateTime.Year;
            }

            _expenseClaim.SetBudget (newAccount, _currentUser);
        }

        if (newAmount != (double) _expenseClaim.Amount)
        {
            if (_expenseClaim.Attested)
            {
                if (newAmount > _expenseClaim.PreApprovedAmount && newAmount > (double) _expenseClaim.Amount)
                {
                    _expenseClaim.Attested = false;
                }
            }

            _expenseClaim.SetAmountCents ((Int64) newAmount * 100, _currentUser);
        }

        if (newDate != _expenseClaim.ExpenseDate)
        {
            if (_expenseClaim.Validated)
            {
                _expenseClaim.Validated = false;
            }

            _expenseClaim.ExpenseDate = newDate;
        }

        // The financial transaction is now updated in ExpenseClaim.

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


    protected void Validator_DateExpense_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (this.DateExpense.SelectedDate == null)
        {
            args.IsValid = false;
            return;
        }

        DateTime selectedDate = (DateTime)this.DateExpense.SelectedDate;

        if (selectedDate > DateTime.Today)
        {
            args.IsValid = false;
            return;
        }

        args.IsValid = true;
    }


    protected void ButtonConfirmedKill_Click(object sender, EventArgs e)
    {
        // Store validation action

        if (_expenseClaim.Validated)
        {
            FinancialValidations.Create(FinancialValidationType.Devalidation, _expenseClaim, _currentUser);
        }

        if (_expenseClaim.Attested)
        {
            FinancialValidations.Create(FinancialValidationType.Deattestation, _expenseClaim, _currentUser);
        }

        FinancialValidations.Create(FinancialValidationType.Kill, _expenseClaim, _currentUser);

        _expenseClaim.Kill(_currentUser);

        // Finally, close form and rebind

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
    }
}
