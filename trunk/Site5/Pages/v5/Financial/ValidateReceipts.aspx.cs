using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class ValidateReceipts : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            if (!this.CurrentUser.HasAccess (new Access(CurrentOrganization, AccessAspect.Financials, AccessType.Write)))
            {
                throw new UnauthorizedAccessException();
            }

            this.PageIcon = "iconshock-invoice-greentick";

            if (!Page.IsPostBack)
            {
                Localize();
            }

            _documentList = new List<RepeatedDocument>();

            PopulateExpenses();

            this.RepeaterLightboxItems.DataSource = _documentList;
            this.RepeaterLightboxItems.DataBind();

        }


        private Dictionary<int, bool> _attestationRights;
        private List<RepeatedDocument> _documentList; 



        private Dictionary<int, bool> GetAttestationRights()
        {
            // Right now, this function is quite primitive. At some point in the future, it needs to take into
            // account that a budget may have several attesters. Right now, it just loops over all accounts and
            // checks the owner.

            Dictionary<int, bool> result = new Dictionary<int, bool>();
            FinancialAccounts accounts = FinancialAccounts.ForOrganization(this.CurrentOrganization);

            foreach (FinancialAccount account in accounts)
            {
                if (account.OwnerPersonId == this.CurrentUser.Identity)
                {
                    result[account.Identity] = true;
                }
            }

            return result;
        }

        private void Localize()
        {
            this.PageTitle = Resources.Pages.Financial.ValidateReceipts_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Financial.ValidateReceipts_Info;
            this.LabelAttestCostsHeader.Text = Resources.Pages.Financial.ValidateReceipts_Header_ReceiptsAwaitingValidation;
            this.LabelGridHeaderAction.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Action;  // Reuse some strings from Attest Costs
            this.LabelGridHeaderBudget.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Budget;
            this.LabelGridHeaderDescription.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Description;
            this.LabelGridHeaderDocs.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Docs;
            this.LabelGridHeaderRequested.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Requested;

            FinancialTransactionTagSets tagSets = FinancialTransactionTagSets.ForOrganization(CurrentOrganization);

            int descriptionWidth = 140 + 160;

            if (tagSets.Count > 0)
            {
                descriptionWidth = 140;
                int tagWidth = 360/(tagSets.Count + 1);

                //this.LiteralBudgetNameWidth.Text = tagWidth.ToString(CultureInfo.InvariantCulture);

                StringBuilder tagSetHtml = new StringBuilder();

                foreach (FinancialTransactionTagSet tagSet in tagSets)
                {
                    tagSetHtml.AppendFormat(
                        "<th data-options=\"field:'tagSet{0}',width:{1},sortable:true,order:'asc'\">{2}</th>\r\n",
                        tagSet.Identity, tagWidth, FinancialTransactionTagSetType.GetLocalizedName(tagSet.FinancialTransactionTagSetTypeId));
                }

                this.LiteralExtraTags.Text = tagSetHtml.ToString();
            }

            this.LiteralDescriptionThStart.Text=String.Format("<th data-options=\"field:'description',width:{0}\">", descriptionWidth);
        }

        private enum AttestationMode
        {
            Unknown = 0,
            Attestation,
            Deattestation
        };

        [WebMethod]
        public static string Validate (string identifier)
        {
            identifier = HttpUtility.UrlDecode(identifier);

            string result = HandleValidationDevalidation(identifier, AttestationMode.Attestation);

            return HttpUtility.UrlEncode(result).Replace("+", "%20");
        }

        [WebMethod]
        public static string Devalidate(string identifier)
        {
            identifier = HttpUtility.UrlDecode(identifier);

            string result = HandleValidationDevalidation(identifier, AttestationMode.Deattestation);

            return HttpUtility.UrlEncode(result).Replace("+", "%20");
        }


        private static string HandleValidationDevalidation (string identifier, AttestationMode mode)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            IValidatable validatableItem = null;
            string validatedTemplate = string.Empty;
            string devalidatedTemplate = string.Empty;

            char costType = identifier[0];
            int itemId = Int32.Parse(identifier.Substring(1));
            Int64 amountCents;
            string beneficiary = string.Empty;
            string result = string.Empty;

            // A lot of this code was copied from attest/deattest, even though validation only concerns expense receipts

            switch(costType)
            {
                case 'E': // Expense claim
                    ExpenseClaim expense = ExpenseClaim.FromIdentity(itemId);
                    if (expense.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException("Called to attest out-of-org line item");
                    }
                    if (!authData.CurrentUser.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.Financials, AccessType.Write)))
                    {
                        throw new UnauthorizedAccessException();
                    }

                    validatableItem = expense;
                    validatedTemplate = Resources.Pages.Financial.ValidateReceipts_ReceiptsValidated;
                    devalidatedTemplate = Resources.Pages.Financial.ValidateReceipts_ReceiptsDevalidated;
                    amountCents = expense.AmountCents;

                    break;
                default:
                    throw new InvalidOperationException("Unknown Cost Type in HandleValidationDevalidation: \"" + identifier + "\"");
            }

            // Finally, attest or deattest

            if (mode == AttestationMode.Attestation)
            {
                validatableItem.Validate(authData.CurrentUser);
                result = string.Format(validatedTemplate, itemId, authData.CurrentOrganization.Currency.Code,
                                       amountCents/100.0);
            }
            else if (mode == AttestationMode.Deattestation)
            {
                validatableItem.Devalidate(authData.CurrentUser);
                result = string.Format(devalidatedTemplate, itemId, authData.CurrentOrganization.Currency.Code,
                                       amountCents / 100.0);
            }
            else
            {
                throw new InvalidOperationException("Unknown Attestation Mode: " + mode.ToString());
            }

            return result;
        }



        private void PopulateExpenses()
        {
            ExpenseClaims expenses = ExpenseClaims.ForOrganization(this.CurrentOrganization).WhereUnvalidated;

            foreach (var expenseClaim in expenses)
            {
                AddDocuments(expenseClaim.Documents, "E" + expenseClaim.Identity.ToString(CultureInfo.InvariantCulture), String.Format(Resources.Global.Financial_ExpenseClaimSpecification + " - ", expenseClaim.Identity) + Resources.Global.Financial_ReceiptSpecification);
            }
        }


        private void AddDocuments (Documents docs, string baseId, string titleBase)
        {
            int countTotal = docs.Count;
            int count = 0;

            foreach (Document document in docs)
            {
                RepeatedDocument newDoc = new RepeatedDocument {DocId = document.Identity, BaseId = baseId, Title=String.Format(titleBase, ++count, countTotal)};

                _documentList.Add(newDoc);
            }
        }

        public class RepeatedDocument
        {
            public int DocId { get; set; }
            public string BaseId { get; set; }
            public string Title { get; set; }
        }
    }
}
