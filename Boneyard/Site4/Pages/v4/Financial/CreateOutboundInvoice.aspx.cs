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

public partial class Pages_v4_Financial_CreateOutboundInvoice : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.DropBudgets.Populate(Organization.PPSE, FinancialAccountType.Result);
            this.GridOutboundInvoices.Organization = Organization.PPSE;
            this.DatePicker.SelectedDate = DateTime.Today.AddDays(30);
            this.TextNewItemAmount.Text = "0,00"; // TODO: Localize
            ViewState["Items"] = string.Empty;
        }

        this.TextNewItemDescription.Style[HtmlTextWriterStyle.Width] = "300px";
        this.TextNewItemAmount.Style[HtmlTextWriterStyle.Width] = "100px";
    }


    protected void DropBudgets_SelectedNodeChanged (object sender, EventArgs e)
    {
        FinancialAccount account = DropBudgets.SelectedFinancialAccount;
    }

    protected void ButtonSubmitInvoice_Click(object sender, EventArgs e)
    {
        // If args were invalid, abort

        if (!Page.IsValid)
        {
            return;
        }

        if (this.TextNewItemDescription.Text.Length > 0)
        {
            AddItemsInEdit(false);
        }

        // Read the form data

        string customer = this.TextCustomer.Text;
        string paperAddress = this.TextPaperAddress.Text;
        string mailAddress = this.TextMailAddress.Text;
        int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
        bool domestic = this.CheckDomestic.Checked;
        string theirReference = this.TextTheirReference.Text;
        FinancialAccount budget = this.DropBudgets.SelectedFinancialAccount;
        DateTime created = DateTime.Now;
        DateTime dueDate = (DateTime) this.DatePicker.SelectedDate;
        string items = (string) ViewState["Items"];

        // If no line items, abort

        if (items.Length < 2)
        {
            Page.ClientScript.RegisterStartupScript(typeof(Page), "FailMessage", @"alert ('Add line items before sending the invoice.');", true);
            return;
        }

        // Create the invoice record

        OutboundInvoice newInvoice = OutboundInvoice.Create(Organization.FromIdentity(organizationId), _currentUser,
                                                            dueDate, budget, customer, mailAddress, paperAddress,
                                                            Currency.FromCode("SEK"), domestic, theirReference);

        // Add the invoice items

        string[] itemArray = items.Split('|');

        for (int index = 0; index < itemArray.Length; index += 2)
        {
            double amount = Double.Parse(itemArray[index + 1], CultureInfo.InvariantCulture);

            newInvoice.AddItem(itemArray[index], amount);
        }


        // Create the financial transaction with rows

        FinancialTransaction transaction = 
            FinancialTransaction.Create(organizationId, created, 
            "Outbound Invoice #" + newInvoice.Identity + " to " + customer);

        transaction.AddRow(Organization.FromIdentity(organizationId).FinancialAccounts.AssetsOutboundInvoices, newInvoice.AmountCents, _currentUser);
        transaction.AddRow(budget, -newInvoice.AmountCents, _currentUser);

        // Make the transaction dependent on the outbound invoice

        transaction.Dependency = newInvoice;

        // Create the event for PirateBot-Mono to send off mails

        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.OutboundInvoiceCreated,
                                                   _currentUser.Identity, organizationId, 1, _currentUser.Identity,
                                                   newInvoice.Identity, string.Empty);
        
  		Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('The outbound invoice was registered and will be sent shortly.');", true);

        // Clear the text fields

        this.TextCustomer.Text = string.Empty;
        this.TextMailAddress.Text = string.Empty;
        this.TextPaperAddress.Text = string.Empty;
        this.TextTheirReference.Text = string.Empty;
        ViewState["Items"] = string.Empty;
        this.TextNewItemDescription.Text = string.Empty;
        this.TextNewItemAmount.Text = "0,00";

        this.LiteralLeftItemSpacer.Text = "&nbsp;";
        this.LiteralItems.Text = string.Empty;

        this.GridOutboundInvoices.Reload();

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


    private static int SortGridItems (ExpenseClaim claim1, ExpenseClaim claim2)
    {
        return DateTime.Compare(claim2.ExpenseDate, claim1.ExpenseDate);
    }

    protected void ButtonAddItem_Click(object sender, EventArgs e)
    {
        AddItemsInEdit(true);

        this.TextNewItemDescription.Text = string.Empty;
        this.TextNewItemAmount.Text = "0,00";
        this.TextNewItemDescription.Focus();
    }

    private void AddItemsInEdit(bool displayErrorMessages)
    {
        if (this.TextNewItemDescription.Text.Length < 2)
        {
            if (displayErrorMessages)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "StartupMessage", "alert('Please write a description.');", true);
            }
            return;
        }

        double amount = 0.0;
        if (!Double.TryParse(this.TextNewItemAmount.Text, NumberStyles.Float, new CultureInfo("sv-SE"), out amount))
        {
            if (displayErrorMessages)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "StartupMessage", "alert('Please write a numeric amount.');", true);
            }
            return;
        }
        string amountString = amount.ToString(CultureInfo.InvariantCulture);

        string currentItems = (string)ViewState["Items"];

        if (currentItems.Length > 0)
        {
            currentItems += "|";
        }

        currentItems += this.TextNewItemDescription.Text.Replace("|", ":") + "|" + amountString;
        ViewState["Items"] = currentItems;

        // Construct the table

        string table = "<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"400px\">";
        string[] itemsArray = currentItems.Split('|');

        for (int index = 0; index < itemsArray.Length; index += 2)
        {
            double localAmount = Double.Parse(itemsArray[index + 1], CultureInfo.InvariantCulture);

            table += "<tr><td>" + Server.HtmlEncode(itemsArray[index]) + "</td><td align=\"right\">" +
                     localAmount.ToString("N2", new CultureInfo("sv-SE")) + "</td></tr>";
        }
        table += "</table>";

        this.LiteralItems.Text = table;
        this.LiteralLeftItemSpacer.Text += "<br/>&nbsp;";
        
    }
}
