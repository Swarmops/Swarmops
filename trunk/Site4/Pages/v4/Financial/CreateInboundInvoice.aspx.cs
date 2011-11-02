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

public partial class Pages_v4_Financial_CreateInboundInvoice : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.TextAmount.Text = (0.0).ToString("N2", new CultureInfo("sv-SE"));
            this.DropBudgets.Populate(Organization.PPSE, FinancialAccountType.Cost);
            this.GridInboundInvoices.Organization = Organization.PPSE;
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

            string serverFileName = String.Format("document_{0:D5}_invoicebyperson_{1:D6}{2}", newDocument.Identity, _currentUser.Identity, file.GetExtension().ToLower());
            file.SaveAs(serverPath + Path.DirectorySeparatorChar + serverFileName);

            newDocument.ServerFileName = serverFileName;

            File.Delete(serverPath + Path.DirectorySeparatorChar + file.GetName());
        }

        this.GridInboundInvoices.Reload();
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

        

        // Read the form data

        string supplier = this.TextSupplier.Text;
        string bankAccount = this.TextAccount.Text;
        
        int temporaryId = Int32.Parse(this.TemporaryDocumentIdentity.Text);

        int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
        Organization org = Organization.FromIdentity(organizationId);
        Int64 amountCents = (Int64) (Double.Parse(this.TextAmount.Text, new CultureInfo("sv-SE"))*100);
        FinancialAccount budget = this.DropBudgets.SelectedFinancialAccount;
        DateTime dueDate = (DateTime) this.DatePicker.SelectedDate;

        string ocr = string.Empty;
        string invoiceNumber = string.Empty;

        if (this.DropReferenceType.SelectedValue == "OCR")
        {
            ocr = this.TextReference.Text;
        }
        else
        {
            invoiceNumber = this.TextReference.Text;
        }

        // Create the invoice record

        InboundInvoice newInvoice = InboundInvoice.Create(org, dueDate,
            amountCents, budget, supplier, bankAccount, ocr, invoiceNumber, _currentUser);

        // Move documents to the new invoice

        Documents.ForObject(new TemporaryIdentity(temporaryId)).SetForeignObjectForAll(newInvoice);

        // Create the event for PirateBot-Mono to send off mails

        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.InboundInvoiceReceived,
                                                   _currentUser.Identity, organizationId, 1, _currentUser.Identity,
                                                   newInvoice.Identity, string.Empty);
        
  		Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('The invoice was registered.');", true);

        // Clear the text fields

        this.TextSupplier.Text = string.Empty;
        this.TextAccount.Text = string.Empty;
        this.TextReference.Text = string.Empty;
        this.DropReferenceType.SelectedIndex = 0;
        this.TemporaryDocumentIdentity.Text = "0";
        this.TextAmount.Text = "0,00"; // TODO: LOCALIZE BY CULTURE

        // PopulateGrid();
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


    protected void Validator_DatePicker_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (this.DatePicker.SelectedDate == null)
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

    private static int SortGridItems (ExpenseClaim claim1, ExpenseClaim claim2)
    {
        return DateTime.Compare(claim2.ExpenseDate, claim1.ExpenseDate);
    }
}
