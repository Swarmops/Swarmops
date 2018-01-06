using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Swarmops.Interface.Support;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class FileExpenseClaim : PageV5Base
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

                FinancialTransactionTagSet tagSet = FinancialTransactionTagSet.FromIdentity (tagSetId);

                if (tagSet.VisibilityLevel <= 1)
                {
                    dataSourceVisibleTags.Add (item);

                    if (!tagSet.AllowUntagged)
                    {
                        dataSourceForcedTags.Add (item);
                    }
                }
            }

            // Bind data

            // Unused for now

            /*
            this.RepeaterTagLabels.DataSource = dataSourceVisibleTags;
            this.RepeaterTagDrop.DataSource = dataSourceVisibleTags;
            this.RepeaterTagDropScript.DataSource = dataSourceVisibleTags;
            this.RepeaterErrorCheckTags.DataSource = dataSourceForcedTags;

            this.RepeaterTagLabels.DataBind();
            this.RepeaterTagDrop.DataBind();
            this.RepeaterTagDropScript.DataBind();
            this.RepeaterErrorCheckTags.DataBind();*/

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

            this.BoxTitle.Text = PageTitle = Resources.Pages.Financial.FileExpenseClaim_PageTitle;
            PageIcon = "iconshock-moneyback";
            InfoBoxLiteral = Resources.Pages.Financial.FileExpenseClaim_Info;

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Participant, AccessType.Write);

            if (!Page.IsPostBack)
            {
                // Prime bank details

                this.TextBank.Text = CurrentUser.BankName;
                this.TextClearing.Text = CurrentUser.BankClearing;
                this.TextAccount.Text = CurrentUser.BankAccount;
                this.CurrencyAmount.Cents = 0;
                this.CurrencyVat.Cents = 0;
                this.CurrencyAmount.Focus();

                Localize();
            }
        }


        private void Localize()
        {
            /* Main tab */

            this.LabelAmount.Text = string.Format (Resources.Pages.Financial.FileExpenseClaim_Amount,
                CurrentOrganization.Currency.DisplayCode);
            this.LabelPurpose.Text = Resources.Pages.Financial.FileExpenseClaim_Description;
            this.LabelBudget.Text = Resources.Pages.Financial.FileExpenseClaim_Budget;
            this.LabelHeaderBankDetails.Text = Resources.Pages.Financial.FileExpenseClaim_HeaderBankDetails;
            this.LabelBankName.Text = Resources.Pages.Financial.FileExpenseClaim_BankName;
            this.LabelBankClearing.Text = Resources.Pages.Financial.FileExpenseClaim_BankClearing;
            this.LabelBankAccount.Text = Resources.Pages.Financial.FileExpenseClaim_BankAccount;
            this.LabelHeaderImageFiles.Text = Resources.Pages.Financial.FileExpenseClaim_HeaderReceiptImages;
            this.LabelImageFiles.Text = Resources.Pages.Financial.FileExpenseClaim_UploadRecieptImages;
            this.LabelVat.Text = string.Format(Resources.Pages.Financial.FileExpenseClaim_Vat,
                CurrentOrganization.Currency.DisplayCode);

            this.ButtonRequest.Text = Resources.Pages.Financial.FileExpenseClaim_ButtonRequest;

            /* Expensify tab */

            this.LabelExpensifyUploadHeader.Text = Resources.Pages.Financial.FileExpenseClaim_Title_Expensify;
            this.LabelExpensifyCsv.Text = Resources.Pages.Financial.FileExpenseClaim_Expensify_CsvUploadDescription;
            this.LabelExpensifyInstructions1.Text =
                Resources.Pages.Financial.FileExpenseClaim_Expensify_InstructionsBasic;
            this.ButtonExpensifyUpload.Text = Resources.Global.Global_UploadFile;

            if (CurrentOrganization.VatEnabled)
            {
                this.LabelExpensifyInstructions2.Text =
                    String.Format(Resources.Pages.Financial.FileExpenseClaim_Expensify_InstructionsNeedVat, CurrentOrganization.Name);
            }
            else
            {
                this.LabelExpensifyInstructions2.Text =
                    Resources.Pages.Financial.FileExpenseClaim_Expensify_InstructionsNothingAdvanced;
            }
        }


        protected void ButtonRequest_Click (object sender, EventArgs e)
        {
            // The data has been validated client-side already. We'll throw unfriendly exceptions if invalid data is passed here.
            // People who choose to disable JavaScript and then submit bad input almost deserve to be hurt.

            Int64 amountCents = this.CurrencyAmount.Cents;
            Int64 vatCents = this.CurrencyVat.Cents;

            string description = this.TextPurpose.Text;

            FinancialAccount budget = FinancialAccount.FromIdentity (Int32.Parse (Request.Form["DropBudgets"]));

            // sanity check

            if (budget.Organization.Identity != CurrentOrganization.Identity)
            {
                throw new InvalidOperationException ("Budget-organization mismatch; won't file expense claim");
            }

            // Store bank details for current user

            CurrentUser.BankName = this.TextBank.Text;
            CurrentUser.BankClearing = this.TextClearing.Text;
            CurrentUser.BankAccount = this.TextAccount.Text;

            // Get documents; check that documents have been uploaded

            Documents documents = Documents.RecentFromDescription (this.FileUpload.GuidString);

            if (documents.Count == 0)
            {
                throw new InvalidOperationException ("No documents uploaded");
            }

            ExpenseClaim claim = ExpenseClaim.Create (CurrentUser, CurrentOrganization, budget, DateTime.UtcNow,
                description, amountCents, vatCents);

            foreach (int tagSetId in this._tagSetIds)
            {
                string selectedTagString =
                    Request.Form["DropTags" + tagSetId.ToString (CultureInfo.InvariantCulture)];

                if (!String.IsNullOrEmpty (selectedTagString))
                {
                    int selectedTagType = Int32.Parse (selectedTagString);
                    if (selectedTagType != 0)
                    {
                        claim.FinancialTransaction.CreateTag (
                            FinancialTransactionTagType.FromIdentity (selectedTagType),
                            CurrentUser);
                    }
                }
            }

            documents.SetForeignObjectForAll (claim);

            string successMessage = string.Format (Resources.Pages.Financial.FileExpenseClaim_SuccessMessagePartOne,
                CurrentOrganization.Currency.Code,
                amountCents/100.0,
                budget.Name);

            if (budget.OwnerPersonId != CurrentUser.Identity)
            {
                successMessage += "<br/><br/>" + Resources.Pages.Financial.FileExpenseClaim_SuccessMessagePartTwo +
                                  "<br/>";
            }
            else
            {
                successMessage += "<br/><br/>" +
                                  Resources.Pages.Financial.FileExpenseClaim_SuccessMessagePartTwoOwnBudget +
                                  "<br/>";
                claim.Attest (CurrentUser);
            }

            DashboardMessage.Set (successMessage);

            // Redirect to dashboard

            Response.Redirect ("/", true);
        }


        protected class TagSetDataSourceItem
        {
            public int TagSetId { get; set; }
            public string TagSetLocalizedName { get; set; }
        }


        // ASPX localizations


        // ReSharper disable InconsistentNaming
        public string Localized_ValidationError_MissingTag
        {
            get { return JavascriptEscape(Resources.Pages.Financial.FileExpenseClaim_ValidationError_MissingTag); }
        }

        public string Localized_ValidationError_BankAccount
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankAccount); }
        }

        public string Localized_ValidationError_BankClearing
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankClearing); }
        }

        public string Localized_ValidationError_BankName
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankName); }
        }

        public string Localized_ValidationError_Purpose
        {
            get { return JavascriptEscape(Resources.Pages.Financial.FileExpenseClaim_ValidationError_Purpose); }
        }

        public string Localized_ValidationError_Budget
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_Budget); }
        }

        public string Localized_ValidationError_Amount
        {
            get { return JavascriptEscape(String.Format(Resources.Pages.Financial.FileExpenseClaim_ValidationError_Amount, CurrentOrganization.Currency.DisplayCode)); }
        }

        public string Localized_ValidationError_Documents
        {
            get { return JavascriptEscape(Resources.Pages.Financial.FileExpenseClaim_ValidationError_Documents); }
        }

    }
}