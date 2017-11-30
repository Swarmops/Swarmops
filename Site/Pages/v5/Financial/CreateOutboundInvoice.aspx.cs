using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Resources;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class CreateOutboundInvoice : PageV5Base
    {
        private int[] _tagSetIds;

        protected void Page_Init (object sender, EventArgs e)
        {
            string tagSetIdsString = Request["ctl00$PlaceHolderMain$HiddenTagSetIdentifiers"];

            // Find our tag ids, either from previously hidden var or load from org

            if (String.IsNullOrEmpty (tagSetIdsString))
            {
                this._tagSetIds = FinancialTransactionTagSets.ForOrganization (CurrentOrganization).Identities;
            }
            else
            {
                string[] tagSetIdStrings = tagSetIdsString.Split (',');
                this._tagSetIds = new int[tagSetIdStrings.Length];

                for (int index = 0; index < tagSetIdStrings.Length; index++)
                {
                    this._tagSetIds[index] = Int32.Parse (tagSetIdStrings[index]);
                }
            }

            // Construct data source

            List<TagSetDataSourceItem> dataSourceVisibleTags = new List<TagSetDataSourceItem>();
            List<TagSetDataSourceItem> dataSourceForcedTags = new List<TagSetDataSourceItem>();

            foreach (int tagSetId in this._tagSetIds)
            {
                TagSetDataSourceItem item = new TagSetDataSourceItem
                {
                    TagSetId = tagSetId,
                    TagSetLocalizedName =
                        FinancialTransactionTagSetType.GetLocalizedName (
                            FinancialTransactionTagSet.FromIdentity (tagSetId).
                                FinancialTransactionTagSetTypeId)
                };
                dataSourceVisibleTags.Add (item);

                if (!FinancialTransactionTagSet.FromIdentity (tagSetId).AllowUntagged)
                {
                    dataSourceForcedTags.Add (item);
                }
            }

            // Bind data

            this.RepeaterTagLabels.DataSource = dataSourceVisibleTags;
            this.RepeaterTagDrop.DataSource = dataSourceVisibleTags;
            this.RepeaterTagDropScript.DataSource = dataSourceVisibleTags;
            this.RepeaterErrorCheckTags.DataSource = dataSourceForcedTags;

            this.RepeaterTagLabels.DataBind();
            this.RepeaterTagDrop.DataBind();
            this.RepeaterTagDropScript.DataBind();
            this.RepeaterErrorCheckTags.DataBind();

            // Write set list back to hidden variable

            List<string> tagSetIdStringList = new List<string>();

            foreach (int tagSetId in this._tagSetIds)
            {
                tagSetIdStringList.Add (tagSetId.ToString (CultureInfo.InvariantCulture));
            }

            this.HiddenTagSetIdentifiers.Value = String.Join (",", tagSetIdStringList.ToArray());
        }

        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.BoxTitle.Text = PageTitle = Resources.Pages.Financial.CreateOutboundInvoice_PageTitle;
            PageIcon = "iconshock-invoice";
            InfoBoxLiteral = Resources.Pages.Financial.CreateInboundInvoice_Info;

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Secretarial, AccessType.Write);

            if (!Page.IsPostBack)
            {
                // Prime bank details

                this.CurrencyAmount.Cents = 0;
                this.CurrencyVat.Cents = 0;
                this.TextClient.Focus();
                this.TextDueDate.Text = DateTime.Today.AddDays (30).ToShortDateString(); // Use current culture

                Localize();
            }
        }


        private void Localize()
        {
            this.LabelClient.Text = Global.Financial_Client;
            this.LabelDueDate.Text = Global.Financial_DueDate;
            this.LabelAmount.Text = string.Format (Resources.Pages.Financial.CreateInboundInvoice_Amount,
                CurrentOrganization.Currency.Code);
            this.LabelVat.Text = string.Format(Resources.Pages.Financial.CreateInboundInvoice_AmountVat,
                CurrentOrganization.Currency.Code);
            this.LabelPurpose.Text = Resources.Pages.Financial.CreateInboundInvoice_Description;
            this.LabelBudget.Text = Global.Financial_Budget;
            this.LabelHeaderBankDetails.Text = Resources.Pages.Financial.CreateInboundInvoice_HeaderPaymentDetails;
            this.LabelHeaderImageFiles.Text = Resources.Pages.Financial.CreateInboundInvoice_HeaderInvoiceImage;
            this.LabelImageFiles.Text = Resources.Pages.Financial.CreateInboundInvoice_UploadInvoiceImage;
            this.LabelReference.Text = Resources.Pages.Financial.CreateInboundInvoice_Reference;

            this.ButtonCreate.Text = Resources.Pages.Financial.CreateOutboundInvoice_ButtonCreate;
        }


        protected void ButtonCreate_Click (object sender, EventArgs e) // TODO
        {
            // The data has been validated client-side already. We'll throw unfriendly exceptions if invalid data is passed here.
            // People who choose to disable JavaScript and then submit bad input almost deserve to be hurt.

            Int64 amountCents = this.CurrencyAmount.Cents;
            Int64 amountVatCents = this.CurrencyVat.Cents;

            string description = this.TextPurpose.Text;

            DateTime dueDate = DateTime.Parse (this.TextDueDate.Text);


            FinancialAccount budget = FinancialAccount.FromIdentity (Int32.Parse (Request.Form["DropBudgets"]));

            // sanity check

            if (budget.Organization.Identity != CurrentOrganization.Identity)
            {
                throw new InvalidOperationException ("Budget-organization mismatch; won't file invoice");
            }


            // Get documents; check that documents have been uploaded

            Documents documents = Documents.RecentFromDescription (this.FileUpload.GuidString);

            if (documents.Count == 0)
            {
                throw new InvalidOperationException ("No documents uploaded");
            }

            OutboundInvoice newInvoice = OutboundInvoice.Create (CurrentOrganization, dueDate, budget, this.TextClient.Text, string.Empty, string.Empty, CurrentOrganization.Currency, false, this.TextReference.Text, CurrentUser);

            newInvoice.AddItem(this.TextPurpose.Text, amountCents);

            // TODO: VAT -- needs to be PER ITEM, and dbfields must update for this, quite a large work item, do not short circuit hack this

            documents.SetForeignObjectForAll(newInvoice);

            // Create financial transaction in the ledger (this logic should not be in the presentation layer at all, move it to a better OutboundInvoice.Create that takes OutboundInvoiceItems as parameter)

            FinancialTransaction txOut = FinancialTransaction.Create(CurrentOrganization, DateTime.UtcNow,
                "Outbound Invoice #" + newInvoice.OrganizationSequenceId.ToString("N0"));

            txOut.AddRow(CurrentOrganization.FinancialAccounts.AssetsOutboundInvoices, amountCents, CurrentUser);
            if (amountVatCents > 0)
            {
                txOut.AddRow(CurrentOrganization.FinancialAccounts.DebtsVatOutboundUnreported, -amountVatCents,
                    CurrentUser);
                txOut.AddRow(budget, -(amountCents - amountVatCents), CurrentUser); // Sales value
            }
            else
            {
                txOut.AddRow(budget, -amountCents, CurrentUser);
            }


            // Make the transaction dependent on the inbound invoice

            txOut.Dependency = newInvoice;

            // If invoice is denominated in a non-presentation currency, record the native values for proper payment

            if (this.CurrencyAmount.NonPresentationCurrencyUsed)
            {
                Money currencyEntered = this.CurrencyAmount.NonPresentationCurrencyAmount;
                newInvoice.NativeCurrencyAmount = currencyEntered;
            }

            // Display success message

            this._invoiceId = newInvoice.OrganizationSequenceId; // a property returns the localized string

            // Reset all fields for next invoice

            this.FileUpload.Reset();
            this.TextClient.Text = String.Empty;
            this.TextPurpose.Text = String.Empty;
            this.TextReference.Text = String.Empty;
            this.CurrencyAmount.Cents = 0;
            this.CurrencyVat.Cents = 0;
            this.TextDueDate.Text = DateTime.Today.AddDays (30).ToShortDateString(); // Use current culture

            // the easyUI combo fields should reset automatically on form submission unless we explicitly reconstruct

            this.TextClient.Focus();
        }


        private int _invoiceId = 0;


        protected class TagSetDataSourceItem
        {
            public int TagSetId { get; set; }
            public string TagSetLocalizedName { get; set; }
        }

        // ReSharper disable InconsistentNaming
        public string Localized_ForGreatJustice
        {
            get
            {
                if (this._invoiceId == 0)
                {
                    return string.Empty;
                }

                return
                    JavascriptEscape (String.Format (Resources.Pages.Financial.CreateOutboundInvoice_SuccessMessage,
                        this._invoiceId));
            }
        }

        public string Localized_ValidationError_MissingTag
        {
            get { return JavascriptEscape(Resources.Pages.Financial.CreateInboundInvoice_ValidationError_MissingTag); }
        }

        public string Localized_ValidationError_Account
        {
            get { return JavascriptEscape(Resources.Pages.Financial.CreateInboundInvoice_ValidationError_Account); }
        }

        public string Localized_ValidationError_Purpose
        {
            get { return JavascriptEscape(Resources.Pages.Financial.CreateInboundInvoice_ValidationError_Purpose); }
        }

        public string Localized_ValidationError_Budget
        {
            get { return JavascriptEscape(Resources.Pages.Financial.CreateInboundInvoice_ValidationError_Budget); }
        }

        public string Localized_ValidationError_Amount
        {
            get { return JavascriptEscape(Resources.Pages.Financial.CreateInboundInvoice_ValidationError_Amount); }
        }

        public string Localized_ValidationError_Documents
        {
            get { return JavascriptEscape(Resources.Pages.Financial.CreateInboundInvoice_ValidationError_Documents); }
        }

    }
}