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
using Activizr.Basic.Interfaces;
using Activizr.Basic.Types;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Support;

using Telerik.Web.UI;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;

public partial class Pages_v4_PopupAuditTransaction : PageV4Base
{
    FinancialTransaction _transaction = null;

    protected void Page_Load (object sender, EventArgs e)
    {
        _transaction = GetTransaction();

        if (!_authority.HasPermission(Permission.CanSeeEconomyTransactions, _transaction.OrganizationId, -1, Authorization.Flag.ExactOrganization))
            throw new UnauthorizedAccessException("Access Denied");


        if (!Page.IsPostBack)
        {
            // Populate all data

            this.DatePicker.DateInput.Culture = new CultureInfo("sv-SE");
            this.DatePicker.SelectedDate = _transaction.DateTime;
            this.TextDescription.Text = _transaction.Description;
            this.TextDescription.Style.Add(HtmlTextWriterStyle.Width, "100%");

            PopulateGrid();
            PopulateEvents();

            IHasIdentity dependency = _transaction.Dependency;
        }

        Documents documents = _transaction.Documents;

        Page.Title = "Auditing Transaction #" + _transaction.Identity.ToString();
    }

    private void PopulateGrid ()
    {

        this.GridTransactionRows.DataSource = _transaction.Rows;
    }

    private List<TransactionEvent> transactionEvents;

    private void PopulateEvents()
    {
        int curIdentity = 1;

        transactionEvents = new List<TransactionEvent>();

        Documents documents = _transaction.Documents;

        foreach (Document document in documents)
        {
            AddDocumentEvent(0, document);
        }

        IHasIdentity dependency = _transaction.Dependency;
        
        if (dependency is Payout)
        {
            AddPayoutEvent(0, dependency as Payout);
        }

        if (dependency is ExpenseClaim)
        {
            AddExpenseClaimEvent(0, dependency as ExpenseClaim);
        }

        if (dependency is InboundInvoice)
        {
            AddInboundInvoiceEvent(0, dependency as InboundInvoice);
        }

        if (dependency is Salary)
        {
            AddSalaryEvent(0, dependency as Salary, false);
        }

        this.TreeEvents.DataSource = transactionEvents;
        this.TreeEvents.DataBind();
    }

    private void AddDocumentEvent (int parentId, Document document)
    {
        int newId = transactionEvents.Count + 1;

        transactionEvents.Add(new TransactionEvent(newId, parentId, "Document: <a target=\"_blank\" href=\"/Pages/v4/Support/DownloadDocument.aspx?DocumentId=" + document.Identity.ToString() + "\">" + HttpUtility.HtmlEncode(document.ClientFileName) + "</a>", true));
        transactionEvents.Add(
            new TransactionEvent(newId + 1, newId,
                                 "Uploaded by " + Person.FromIdentity(document.UploadedByPersonId).Canonical + " at " +
            document.UploadedDateTime.ToString("yyyy-MM-dd HH:mm")));
    }

    private void AddPayoutEvent (int parentId, Payout payout)
    {
        int newId = transactionEvents.Count + 1;

        transactionEvents.Add(new TransactionEvent(newId, parentId, "This is Payout #" + payout.Identity.ToString() + "."));
        transactionEvents.Add(
            new TransactionEvent(newId + 1, newId,
                                 "Performed by " + Person.FromIdentity(payout.CreatedByPersonId).Canonical + " at " +
                                 payout.CreatedDateTime.ToString("yyyy-MM-dd HH:mm")));

        foreach (ExpenseClaim claim in payout.DependentExpenseClaims)
        {
            AddExpenseClaimEvent(newId, claim);
        }
        foreach (InboundInvoice invoice in payout.DependentInvoices)
        {
            AddInboundInvoiceEvent(newId, invoice);
        }
        foreach (Salary salary in payout.DependentSalariesNet)
        {
            AddSalaryEvent(newId, salary, false);
        }
        foreach (Salary salary in payout.DependentSalariesTax)
        {
            AddSalaryEvent(newId, salary, true);
        }
    }

    private void AddExpenseClaimEvent(int parentId, ExpenseClaim claim)
    {
        int newId = transactionEvents.Count + 1;

        transactionEvents.Add(new TransactionEvent(newId, parentId, "Expense Claim #" + claim.Identity.ToString()));
        transactionEvents.Add(
            new TransactionEvent(newId + 1, newId,
                                 "Description: " + claim.Description));
        transactionEvents.Add(
            new TransactionEvent(newId + 2, newId,
                                 "Claimed by " + claim.Claimer.Canonical + " at " +
                                 claim.CreatedDateTime.ToString("yyyy-MM-dd HH:mm")));

        AddValidations(newId, claim);

        foreach (Document document in claim.Documents)
        {
            AddDocumentEvent(newId, document);
        }
    }

    private void AddInboundInvoiceEvent(int parentId, InboundInvoice invoice)
    {
        int newId = transactionEvents.Count + 1;

        transactionEvents.Add(new TransactionEvent(newId, parentId, "Invoice #" + invoice.Identity.ToString()));
        transactionEvents.Add(
            new TransactionEvent(newId + 1, newId,
                                 "Supplier: " + invoice.Supplier));
        transactionEvents.Add(
            new TransactionEvent(newId + 2, newId,
                                 "Received: " +
                                 invoice.CreatedDateTime.ToString("yyyy-MM-dd HH:mm")));

        transactionEvents.Add(
            new TransactionEvent(newId + 3, newId,
                                 "Budget: " +
                                 invoice.Budget.Name));

        AddValidations(newId, invoice);

        foreach (Document document in invoice.Documents)
        {
            AddDocumentEvent(newId, document);
        }
    }

    private void AddSalaryEvent(int parentId, Salary salary, bool tax)
    {
        int newId = transactionEvents.Count + 1;

        transactionEvents.Add(new TransactionEvent(newId, parentId, (tax? "Tax for ": string.Empty) + "Salary #" + salary.Identity.ToString()));
        transactionEvents.Add(
            new TransactionEvent(newId + 1, newId,
                                 "To: " + salary.PayrollItem.Person.Canonical));
        transactionEvents.Add(
            new TransactionEvent(newId + 2, newId,
                                 "Payday: " + salary.PayoutDate.ToString("yyyy-MM-dd")));

        AddValidations(newId, salary);

        transactionEvents.Add(
            new TransactionEvent(transactionEvents.Count + 1, newId,
                         "Base Salary: " + salary.BaseSalaryDecimal.ToString("N2")));

        PayrollAdjustments adjustments = salary.Adjustments;

        foreach (PayrollAdjustment adjustment in adjustments)
        {
            if (adjustment.Type == PayrollAdjustmentType.GrossAdjustment)
            {
                transactionEvents.Add(
                    new TransactionEvent(transactionEvents.Count + 1, newId,
                                 adjustment.Description + ": " + adjustment.AmountDecimal.ToString("N2")));
            }
        }

        transactionEvents.Add(
            new TransactionEvent(transactionEvents.Count + 1, newId,
                         "Primary Income Tax: " + (-salary.SubtractiveTaxDecimal).ToString("N2")));

        foreach (PayrollAdjustment adjustment in adjustments)
        {
            if (adjustment.Type == PayrollAdjustmentType.NetAdjustment)
            {
                transactionEvents.Add(
                    new TransactionEvent(transactionEvents.Count + 1, newId,
                                 adjustment.Description + ": " + adjustment.AmountDecimal.ToString("N2")));
            }
        }

        transactionEvents.Add(
            new TransactionEvent(transactionEvents.Count + 1, newId,
                         "NET PAYOUT: " + salary.NetSalaryDecimal.ToString("N2")));

        transactionEvents.Add(
            new TransactionEvent(transactionEvents.Count + 1, newId,
                         "Additional Tax: " + salary.AdditiveTaxDecimal.ToString("N2")));

    }


    private void AddValidations (int parentId, IHasIdentity validatableObject)
    {
        FinancialValidations validations = FinancialValidations.ForObject(validatableObject);

        foreach (FinancialValidation validation in validations)
        {
            transactionEvents.Add(
                new TransactionEvent(transactionEvents.Count + 1, parentId,
                                     validation.ValidationType + " by " + validation.Person.Canonical + " at " +
                                     validation.DateTime.ToString("yyyy-MM-dd HH:mm")));

        }
    }


    private class TransactionEvent : IHasIdentity
    {
        public TransactionEvent(int identity, int parentIdentity, string text)
        {
            this.identity = identity;
            this.parentIdentity = parentIdentity;
            this.text = HttpUtility.HtmlEncode(text);
        }

        public TransactionEvent(int identity, int parentIdentity, string text, bool literal)
        {
            this.identity = identity;
            this.parentIdentity = parentIdentity;
            this.text = text;
        }

        private int identity;
        private int parentIdentity;
        private string text;
    
        public int ParentIdentity
        {
            get { return this.parentIdentity; }
        }

        public string Text
        {
            get { return this.text; }
        }

        #region IHasIdentity Members

        public int  Identity
        {
	        get { return this.identity; }
        }

        #endregion
    }

    private FinancialTransaction GetTransaction ()
    {
        int transactionId = Int32.Parse(Request.QueryString["TransactionId"]);
        FinancialTransaction transaction = FinancialTransaction.FromIdentity(transactionId);

        return transaction;
    }

    protected void GridTransactionRows_ItemCreated (object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            FinancialTransactionRow row = (FinancialTransactionRow)e.Item.DataItem;

            if (row == null)
            {
                return;
            }

            string field = "LabelDebit";

            if (row.AmountCents < 0)
            {
                field = "LabelCredit";
            }

            Label labelDelta = (Label)e.Item.FindControl(field);
            labelDelta.Text = row.Amount.ToString("+#,##0.00;-#,##0.00", new CultureInfo("sv-SE"));
        }
    }



}