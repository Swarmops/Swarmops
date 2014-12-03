using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class FileExpenseClaim : PageV5Base
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            string tagSetIdsString = Request["ctl00$PlaceHolderMain$HiddenTagSetIdentifiers"];

            // Find our tag ids, either from previously hidden var or load from org

            if (String.IsNullOrEmpty(tagSetIdsString))
            {
                _tagSetIds = FinancialTransactionTagSets.ForOrganization(this.CurrentOrganization).Identities;
            }
            else
            {
                string[] tagSetIdStrings = tagSetIdsString.Split(',');
                _tagSetIds = new int[tagSetIdStrings.Length];

                for (int index = 0; index < tagSetIdStrings.Length; index++)
                {
                    _tagSetIds[index] = Int32.Parse(tagSetIdStrings[index]);
                }
            }

            // Construct data source

            List<TagSetDataSourceItem> dataSourceVisibleTags = new List<TagSetDataSourceItem>();
            List<TagSetDataSourceItem> dataSourceForcedTags = new List<TagSetDataSourceItem>();

            foreach (int tagSetId in _tagSetIds)
            {
                TagSetDataSourceItem item = new TagSetDataSourceItem()
                {
                    TagSetId = tagSetId,
                    TagSetLocalizedName =
                        FinancialTransactionTagSetType.GetLocalizedName(
                            FinancialTransactionTagSet.FromIdentity(tagSetId).
                                FinancialTransactionTagSetTypeId)
                };

                FinancialTransactionTagSet tagSet = FinancialTransactionTagSet.FromIdentity(tagSetId);

                if (tagSet.VisibilityLevel <= 1)
                {
                    dataSourceVisibleTags.Add(item);

                    if (!tagSet.AllowUntagged)
                    {
                        dataSourceForcedTags.Add(item);
                    }
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

            foreach (int tagSetId in _tagSetIds)
            {
                tagSetIdStringList.Add(tagSetId.ToString(CultureInfo.InvariantCulture));
            }

            this.HiddenTagSetIdentifiers.Value = String.Join(",", tagSetIdStringList.ToArray());
        }

        private int[] _tagSetIds;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.BoxTitle.Text = this.PageTitle = Resources.Pages.Financial.FileExpenseClaim_PageTitle;
            this.PageIcon = "iconshock-moneyback";
            this.InfoBoxLiteral = Resources.Pages.Financial.FileExpenseClaim_Info;

            if (!Page.IsPostBack)
            {
                // Prime bank details

                this.TextBank.Text = this.CurrentUser.BankName;
                this.TextClearing.Text = this.CurrentUser.BankClearing;
                this.TextAccount.Text = this.CurrentUser.BankAccount;
                this.TextAmount.Text = 0.ToString("N2");
                this.TextAmount.Focus();

                Localize();
            }

            EasyUIControlsUsed = EasyUIControl.Tree;
            IncludedControlsUsed = IncludedControl.FileUpload;
        }


        private void Localize()
        {
            this.LabelAmount.Text = string.Format(Resources.Pages.Financial.FileExpenseClaim_Amount,
                                                  CurrentOrganization.Currency.DisplayCode);
            this.LabelPurpose.Text = Resources.Pages.Financial.FileExpenseClaim_Description;
            this.LabelBudget.Text = Resources.Pages.Financial.FileExpenseClaim_Budget;
            this.LabelHeaderBankDetails.Text = Resources.Pages.Financial.FileExpenseClaim_HeaderBankDetails;
            this.LabelBankName.Text = Resources.Pages.Financial.FileExpenseClaim_BankName;
            this.LabelBankClearing.Text = Resources.Pages.Financial.FileExpenseClaim_BankClearing;
            this.LabelBankAccount.Text = Resources.Pages.Financial.FileExpenseClaim_BankAccount;
            this.LabelHeaderImageFiles.Text = Resources.Pages.Financial.FileExpenseClaim_HeaderReceiptImages;
            this.LabelImageFiles.Text = Resources.Pages.Financial.FileExpenseClaim_UploadRecieptImages;

            this.LiteralErrorAmount.Text = Resources.Pages.Financial.FileExpenseClaim_ValidationError_Amount;
            this.LiteralErrorPurpose.Text = Resources.Pages.Financial.FileExpenseClaim_ValidationError_Purpose;
            this.LiteralErrorBudget.Text = Resources.Pages.Financial.RequestCashAdvance_ValidationError_Budget;
            this.LiteralErrorBankName.Text = Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankName;
            this.LiteralErrorBankClearing.Text = Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankClearing;
            this.LiteralErrorBankAccount.Text = Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankAccount;
            this.LiteralErrorDocuments.Text = Resources.Pages.Financial.FileExpenseClaim_ValidationError_Documents;

        }


        protected void ButtonRequest_Click(object sender, EventArgs e)
        {
            // The data has been validated client-side already. We'll throw unfriendly exceptions if invalid data is passed here.
            // People who choose to disable JavaScript and then submit bad input almost deserve to be hurt.

            double amount = Double.Parse(this.TextAmount.Text, NumberStyles.Number);
                // parses in current culture - intentional
            Int64 amountCents = (Int64) amount*100;

            string description = this.TextPurpose.Text;

            FinancialAccount budget = FinancialAccount.FromIdentity(Int32.Parse(this.Request.Form["DropBudgets"]));

            // sanity check

            if (budget.Organization.Identity != CurrentOrganization.Identity)
            {
                throw new InvalidOperationException("Budget-organization mismatch; won't file expense claim");
            }

            // Store bank details for current user

            CurrentUser.BankName = this.TextBank.Text;
            CurrentUser.BankClearing = this.TextClearing.Text;
            CurrentUser.BankAccount = this.TextAccount.Text;

            // Get documents; check that documents have been uploaded

            Documents documents = Documents.RecentFromDescription (this.FileUpload.GuidString);

            if (documents.Count == 0)
            {
                throw new InvalidOperationException("No documents uploaded");
            }

            ExpenseClaim claim = ExpenseClaim.Create (this.CurrentUser, this.CurrentOrganization, budget, DateTime.UtcNow, description, amountCents);

            foreach (int tagSetId in _tagSetIds)
            {
                string selectedTagString =
                    this.Request.Form["DropTags" + tagSetId.ToString(CultureInfo.InvariantCulture)];

                if (!String.IsNullOrEmpty(selectedTagString))
                {
                    int selectedTagType = Int32.Parse(selectedTagString);
                    if (selectedTagType != 0)
                    {
                        claim.FinancialTransaction.CreateTag(FinancialTransactionTagType.FromIdentity(selectedTagType),
                                                             CurrentUser);
                    }
                }
            }

            documents.SetForeignObjectForAll(claim);

            string successMessage = string.Format(Resources.Pages.Financial.FileExpenseClaim_SuccessMessagePartOne,
                                                  CurrentOrganization.Currency.Code,
                                                  (double) (amountCents/100.0),
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

            Response.AppendCookie(new HttpCookie("DashboardMessage", HttpUtility.UrlEncode(successMessage)));

            // Redirect to dashboard

            Response.Redirect("/", true);
        }


        protected class TagSetDataSourceItem
        {
            public int TagSetId { get; set; }
            public string TagSetLocalizedName { get; set; }
        }
    }
}