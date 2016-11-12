using System;
using System.Collections;
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
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

using Telerik.Web.UI;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;

public partial class Pages_v4_PopupEditTransaction : PageV4Base
{
    FinancialTransaction _transaction = null;

    protected void Page_Load (object sender, EventArgs e)
    {
        _transaction = GetTransaction();

        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, _transaction.OrganizationId, -1, Authorization.Flag.ExactOrganization))
            throw new UnauthorizedAccessException("Access Denied");


        if (!Page.IsPostBack)
        {
            // Populate all data

            this.DatePicker.DateInput.Culture = new CultureInfo("sv-SE");
            this.DatePicker.SelectedDate = _transaction.DateTime;
            this.TextDescription.Text = _transaction.Description;
            this.TextDescription.Style.Add(HtmlTextWriterStyle.Width, "100%");
            this.TextAmount.Style.Add(HtmlTextWriterStyle.Width, "80px");
            this.DropAccounts.Style.Add(HtmlTextWriterStyle.Width, "200px");

            this.TextAmount.Text = (-_transaction.Rows.AmountTotal).ToString("N2", new CultureInfo("sv-SE"));

            PopulateAccounts(_transaction.OrganizationId);
            PopulateGrid();

            IHasIdentity dependency = _transaction.Dependency;

            if (dependency != null)
            {
                this.PanelModifiableTransaction.Visible = false;
                this.PanelUnmodifiableTransaction.Visible = true;

                if (dependency is ExpenseClaim)
                {
                    this.LabelDependency.Text = "Expense Claim #" + dependency.Identity.ToString();
                    this.PanelDependencyDocuments.Visible = true;
                    this.DocumentsDependency.Documents = Documents.ForObject(dependency);
                }
                else if (dependency is Payout)
                {
                    this.PanelDependencyDocuments.Visible = false;
                    this.PanelPayoutDetails.Visible = true;
                    this.LabelDependency.Text = "Payout #" + dependency.Identity.ToString();
                    this.LabelPayoutIdentity.Text = dependency.Identity.ToString();
                    this.LabelPayoutReference.Text = ((Payout) dependency).Reference;
                }
            }
        }

        this.DocumentList.Documents = _transaction.Documents;

        Page.Title = "Editing Transaction #" + _transaction.Identity.ToString();
    }

    private void PopulateAccounts (int organizationId)
    {
        FinancialAccounts accounts = FinancialAccounts.ForOrganization(Organization.FromIdentity(organizationId));

        this.DropAccounts.Items.Add(new ListItem("-- Select account --", "0"));
        foreach (FinancialAccount account in accounts)
        {
            this.DropAccounts.Items.Add(new ListItem(account.Name, account.Identity.ToString()));
        }
    }

    private void PopulateGrid ()
    {

        this.GridTransactionRows.DataSource = _transaction.Rows;
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

    protected void ButtonClose_Click (object sender, EventArgs e)
    {

        if (_authority.HasPermission(Permission.CanDoEconomyTransactions, _transaction.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            GetTransaction().Description = this.TextDescription.Text;
        }

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
    }

    protected void ButtonAdd_Click (object sender, EventArgs e)
    {

        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, _transaction.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('You do not have access to change financial records.');", true);
            return;
        }

        int accountId = Int32.Parse(this.DropAccounts.SelectedValue);

        if (accountId == 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('Please select an account.');", true);
            return;
        }

        FinancialAccount account = FinancialAccount.FromIdentity(accountId);

        string amountString = this.TextAmount.Text;
        Double amount = Double.Parse(amountString, new CultureInfo("sv-SE"));

        // Register new row in transaction

        GetTransaction().AddRow(account, amount, _currentUser);

        PopulateGrid();
        this.GridTransactionRows.Rebind();

        this.TextAmount.Text = (-GetTransaction().Rows.AmountTotal).ToString("N2", new CultureInfo("sv-SE"));
    }

    protected void ButtonUpload_Click (object sender, EventArgs e)
    {
        string serverPath = @"C:\Data\Uploads\PirateWeb";  // TODO: Read from web.config

        FinancialTransaction currentTransaction = GetTransaction();
        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, _transaction.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('You do not have access to change financial records.');", true);

            this.Upload.UploadedFiles.Clear(); // I wonder if this works for canceling uploads?

            return;
        }

        if (this.Upload.UploadedFiles.Count > 0)
        {

        }

        foreach (UploadedFile file in this.Upload.UploadedFiles)
        {
            string clientFileName = file.GetName();
            string extension = file.GetExtension();

            Document newDocument =
                Document.Create(Guid.NewGuid().ToString() + ".tmp", clientFileName,
                                file.ContentLength, string.Empty, currentTransaction, _currentUser);

            string serverFileName = String.Format("document_{0:D5}_transaction_{1:D6}{2}", newDocument.Identity, currentTransaction.Identity, file.GetExtension().ToLower());
            file.SaveAs(serverPath + Path.DirectorySeparatorChar + serverFileName);

            newDocument.ServerFileName = serverFileName;

            File.Delete(serverPath + Path.DirectorySeparatorChar + file.GetName());
        }
    }


}