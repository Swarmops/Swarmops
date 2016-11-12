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
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Pages_v4_Accounting_ClaimExpense : PageV4Base
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

            PopulateGrid();
            PopulatePreattested();
        }
    }

    protected void DropBudgets_SelectedNodeChanged (object sender, EventArgs e)
    {
        FinancialAccount account = DropBudgets.SelectedFinancialAccount;

        this.LabelBudgetOwner.Text = account.Owner.Name + " owns this budget.";
    }

    protected void ButtonUpload_Click(object sender, EventArgs e)
    {
        ProcessUpload();

        int temporaryId = Int32.Parse(this.TemporaryDocumentIdentity.Text);

        this.DocumentList.Documents = Documents.ForObject(new TemporaryIdentity(temporaryId));
    }


    private void ProcessUpload()
    {
        TemporaryIdentity tempId = new TemporaryIdentity(Int32.Parse(this.TemporaryDocumentIdentity.Text));

        if (tempId.Identity == 0)
        {
            // Ok, storing the temporary id in the page state is REALLY ugly.
            // How are you supposed to solve this class of problems, where you
            // need to persist data on a page for later processing?

            tempId = TemporaryIdentity.GetNew();
            this.TemporaryDocumentIdentity.Text = tempId.Identity.ToString();
        }

        string serverPath = @"C:\Data\Uploads\PirateWeb";  // TODO: Read from web.config

        foreach (UploadedFile file in this.Upload.UploadedFiles)
        {
            string clientFileName = file.GetName();
            string extension = file.GetExtension();

            Document newDocument =
                Document.Create (Guid.NewGuid().ToString() + ".tmp", clientFileName,
                                file.ContentLength, string.Empty, tempId, _currentUser);

            string serverFileName = String.Format("document_{0:D5}_expensebyperson_{1:D6}{2}", newDocument.Identity, _currentUser.Identity, file.GetExtension().ToLower());
            file.SaveAs(serverPath + Path.DirectorySeparatorChar + serverFileName);

            newDocument.ServerFileName = serverFileName;

            File.Delete(serverPath + Path.DirectorySeparatorChar + file.GetName());
        }
    }

    protected void ButtonSubmitClaim_Click(object sender, EventArgs e)
    {
        // First, if there's an upload that the user hasn't processed, process it first.

        if (this.Upload.UploadedFiles.Count > 0)
        {
            ProcessUpload();
        }

        // If args were invalid, abort

        if (!Page.IsValid)
        {
            return;
        }

        // Set bank details

        _currentUser.BankName = this.TextBank.Text.Trim();
        _currentUser.BankClearing = Formatting.CleanNumber(this.TextBankClearing.Text.Trim());
        _currentUser.BankAccount = Formatting.CleanNumber(this.TextAccount.Text.Trim());

        // Read the form data

        int temporaryId = Int32.Parse(this.TemporaryDocumentIdentity.Text);

        int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
        Organization organization = Organization.FromIdentity(organizationId);

        Int64 amountCents = (Int64) (Double.Parse(this.TextAmount.Text, new CultureInfo("sv-SE")) * 100);
        FinancialAccount account = this.DropBudgets.SelectedFinancialAccount;
        DateTime created = DateTime.Now;
        DateTime expenseDate = (DateTime) this.DatePicker.SelectedDate;
        string description = this.TextDescription.Text;
        int claimId = 0;

        if (this.RadioNewClaim.Checked)
        {
            // Create the expense claim record

            ExpenseClaim newClaim = ExpenseClaim.Create(_currentUser,
                                                        organization, account, expenseDate,
                                                        description, amountCents);
            claimId = newClaim.Identity;

            // Move documents to the new expense claim

            Documents.ForObject(new TemporaryIdentity(temporaryId)).SetForeignObjectForAll(newClaim);

        }
        else
        {
            // This was a pre-approved claim: modify the existing claim and transaction

            ExpenseClaim claim = ExpenseClaim.FromIdentity(Int32.Parse(this.DropPreattested.SelectedValue));
            FinancialTransaction transaction = claim.FinancialTransaction;
            claimId = claim.Identity;

            claim.Claimed = true;

            // If claimed amount exceeds attested amount, unattest

            if (amountCents > claim.AmountCents)
            {
                claim.Deattest(_currentUser);
            }

            // Change amount and date

            claim.SetAmountCents (amountCents, _currentUser);
            claim.ExpenseDate = expenseDate;

            // Move documents to the expense claim

            Documents.ForObject(new TemporaryIdentity(temporaryId)).SetForeignObjectForAll(claim);
        }

        // Create the event for PirateBot-Mono to send off mails

        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ExpenseCreated,
                                                     _currentUser.Identity, organizationId, Geography.RootIdentity, _currentUser.Identity,
                                                     claimId, string.Empty);

        Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('Your expense claim has been submitted.');", true);

        // Clear the text fields

        this.TextDescription.Text = string.Empty;
        this.TextAmount.Text = "0,00"; // TODO: LOCALIZE BY CULTURE
        this.TemporaryDocumentIdentity.Text = "0";

        PopulateGrid();
        PopulatePreattested();
    }

    private void PopulatePreattested()
    {
        ExpenseClaims allClaims = ExpenseClaims.FromClaimingPersonAndOrganization(_currentUser, Organization.PPSE);

        ExpenseClaims preAttested = new ExpenseClaims();

        foreach (ExpenseClaim claim in allClaims)
        {
            if (claim.Open && claim.Attested && !claim.Claimed)
            {
                preAttested.Add(claim);
            }
        }

        if (preAttested.Count > 0)
        {
            this.RadioPreattested.Checked = true;
            this.RadioNewClaim.Checked = false;
            this.DropPreattested.Items.Clear();
            this.DropPreattested.Items.Add(new ListItem("-- Select One --", "0"));

            foreach (ExpenseClaim claim in preAttested)
            {
                this.DropPreattested.Items.Add(new ListItem(string.Format("Up to {0:N2}: {1} ({2})", claim.Amount, claim.Description, claim.Budget.Name), claim.Identity.ToString()));
            }

            this.DropPreattested.SelectedIndex = 0;
        }
        else
        {
            this.RadioNewClaim.Checked = true;
            this.RadioPreattested.Checked = false;
            this.RadioPreattested.Enabled = false;
            this.DropPreattested.Items.Clear();
            this.DropPreattested.Items.Add(new ListItem("-- None --", "0"));
            this.DropPreattested.Enabled = false;
        }


    }


    protected void Validator_DocumentList_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // There need to be documents, either already uploaded or in the upload pipe now.

        if (this.TemporaryDocumentIdentity.Text != "0" || this.Upload.UploadedFiles.Count > 0)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }


    protected void Validate_DropPreattested_SelectionRequired(object source, ServerValidateEventArgs args)
    {
        // If "preattested" is selected, a preattested claim must be selected.

        if (this.DropPreattested.SelectedIndex != 0 || this.RadioNewClaim.Checked)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
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

        if (this.DropBudgets.SelectedFinancialAccount == null && this.DropBudgets.Visible)
        {
            args.IsValid = false;
            return;
        }

        args.IsValid = true;
    }


    protected void Validator_DatePicker_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (this.DatePicker.SelectedDate == null)
        {
            args.IsValid = false;
            return;
        }

        DateTime selectedDate = (DateTime) this.DatePicker.SelectedDate;

        if (selectedDate > DateTime.Today)
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

        args.IsValid = Double.TryParse(this.TextAmount.Text, NumberStyles.Float, new CultureInfo("sv-SE"), out dummy);
    }

    private void PopulateGrid()
    {
        ExpenseClaims allClaims = ExpenseClaims.FromClaimingPerson(_currentUser);

        ExpenseClaims gridData = new ExpenseClaims();
        DateTime oneYearAgo = DateTime.Today.AddYears(-1);

        foreach (ExpenseClaim claim in allClaims)
        {
            // Add those that are less than a year old AND/OR not repaid yet.

            if (claim.ExpenseDate > oneYearAgo || claim.Open)
            {
                gridData.Add(claim);
            }
        }

        gridData.Sort(SortGridItems);

        this.GridExpenseClaims.DataSource = gridData;
        this.GridExpenseClaims.Rebind();
    }


    private static int SortGridItems (ExpenseClaim claim1, ExpenseClaim claim2)
    {
        return DateTime.Compare(claim2.ExpenseDate, claim1.ExpenseDate);
    }


    protected void GridExpenseClaims_ItemDataBound(object sender, GridItemEventArgs e)
    {
        // Don't care -- use ItemCreated
    }


    protected void GridExpenseClaims_ItemCreated(object sender, GridItemEventArgs e)
    {
        // Set the images for the status indicators.

        string imageUrlTodo = "~/Images/Public/Fugue/icons-shadowless/minus-small.png";
        string imageUrlTick = "~/Images/Public/Fugue/icons-shadowless/tick.png";
        string imageUrlFail = "~/Images/Public/Fugue/icons-shadowless/cross-circle-frame.png";
        string imageUrlWarn = "~/Images/Public/Fugue/icons-shadowless/exclamation-frame.png";

        if (e.Item is GridDataItem)
        {
            ExpenseClaim claim = (ExpenseClaim) e.Item.DataItem;

            if (claim == null)
            {
                return;
            }

            Image imageClaimed = (Image)e.Item.FindControl("ImageClaimed");
            Image imageAttested = (Image)e.Item.FindControl("ImageAttested");
            Image imageValidated = (Image)e.Item.FindControl("ImageValidated");
            Image imageRepaid = (Image)e.Item.FindControl("ImageRepaid");

            imageClaimed.ImageUrl = claim.Claimed ? imageUrlTick : imageUrlTodo;
            imageAttested.ImageUrl = claim.Attested ? imageUrlTick : imageUrlTodo;
            imageValidated.ImageUrl = claim.Validated ? imageUrlTick : imageUrlTodo;

            if (claim.Open == false && claim.Repaid == false)
            {
                imageRepaid.ImageUrl = imageUrlFail;
            }
            else
            {
                imageRepaid.ImageUrl = claim.Repaid ? imageUrlTick : imageUrlTodo;
            }
        }

    }

    protected void RadioNewClaim_CheckedChanged(object sender, EventArgs e)
    {
        ProcessPreattestChange();
    }

    protected void RadioPreattested_CheckedChanged(object sender, EventArgs e)
    {
        ProcessPreattestChange();
    }

    protected void DropPreattested_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool selectedNotNull = (this.DropPreattested.SelectedIndex != 0);
        this.RadioNewClaim.Checked = !selectedNotNull;
        this.RadioPreattested.Checked = selectedNotNull;

        ProcessPreattestChange();
        CheckExceedage();
    }

    private void ProcessPreattestChange()
    {
        bool selectedPreattested = this.RadioPreattested.Checked;

        if (this.DropPreattested.SelectedIndex == 0)
        {
            selectedPreattested = false;
        }
        else
        {
            ExpenseClaim claim = ExpenseClaim.FromIdentity(Int32.Parse(this.DropPreattested.SelectedValue));

            this.LabelPreattestedBudget.Text = claim.Budget.Name;
            this.LabelDescription.Text = claim.Description;
            this.TextDescription.Text = claim.Description;
        }

        this.DropBudgets.Visible = !selectedPreattested;
        this.LabelBudgetOwner.Visible = selectedPreattested;
        this.TextDescription.Visible = !selectedPreattested;
        this.LabelDescription.Visible = selectedPreattested;
        this.LabelPreattestedBudget.Visible = selectedPreattested;
        
    }


    protected void TextAmount_TextChanged(object sender, EventArgs e)
    {
        CheckExceedage();
    }

    private void CheckExceedage()
    {
        this.IconWarningExceedage.Visible = false;
        this.LabelWarningExceedage.Visible = false;
        
        if (this.RadioPreattested.Checked && this.DropPreattested.SelectedIndex != 0)
        {
            // Compare the preattested amount with the claimed amount. If claimed > attested, show warning that re-attestation is needed.

            try
            {
                decimal claimedAmount = Decimal.Parse(this.TextAmount.Text, NumberStyles.Float, new CultureInfo("sv-SE"));
                decimal attestedAmount =
                    ExpenseClaim.FromIdentity(Int32.Parse(this.DropPreattested.SelectedValue)).Amount;

                bool warningVisible = (claimedAmount > attestedAmount);
                this.LabelWarningExceedage.Visible = warningVisible;
                this.IconWarningExceedage.Visible = warningVisible;
            }
            catch (Exception)
            {
                // Ignore exceptions, which will come from a non-selection of preattested claim or an invalid number
            }
        }
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
