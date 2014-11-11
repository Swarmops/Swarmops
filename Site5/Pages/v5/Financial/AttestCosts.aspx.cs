using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Services;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class AttestCosts : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.PageIcon = "iconshock-stamped-paper";

            if (!Page.IsPostBack)
            {
                Localize();
            }

            _attestationRights = GetAttestationRights();
            _documentList = new List<RepeatedDocument>();

            PopulateInboundInvoices();
            PopulateExpenses();
            // PopulateSalaries();

            this.EasyUIControlsUsed = EasyUIControl.DataGrid;

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
            this.PageTitle = Resources.Pages_Financial.AttestCosts_PageTitle;
            this.InfoBoxLiteral = Resources.Pages_Financial.AttestCosts_Info;
            this.LabelAttestCostsHeader.Text = Resources.Pages_Financial.AttestCosts_Header_CostsAwaitingAttestation;
            this.LabelGridHeaderAction.Text = Resources.Pages_Financial.AttestCosts_GridHeader_Action;
            this.LabelGridHeaderBeneficiary.Text = Resources.Pages_Financial.AttestCosts_GridHeader_Beneficiary;
            this.LabelGridHeaderBudget.Text = Resources.Pages_Financial.AttestCosts_GridHeader_Budget;
            this.LabelGridHeaderDescription.Text = Resources.Pages_Financial.AttestCosts_GridHeader_Description;
            this.LabelGridHeaderDocs.Text = Resources.Pages_Financial.AttestCosts_GridHeader_Docs;
            this.LabelGridHeaderItem.Text = Resources.Pages_Financial.AttestCosts_GridHeader_Item;
            this.LabelGridHeaderRequested.Text = Resources.Pages_Financial.AttestCosts_GridHeader_Requested;
        }

        private enum AttestationMode
        {
            Unknown = 0,
            Attestation,
            Deattestation
        };

        [WebMethod]
        public static string Attest (string identifier)
        {
            identifier = HttpUtility.UrlDecode(identifier);

            string result = HandleAttestationDeattestation(identifier, AttestationMode.Attestation);

            return HttpUtility.UrlEncode(result).Replace("+", "%20");
        }

        [WebMethod]
        public static string Deattest(string identifier)
        {
            identifier = HttpUtility.UrlDecode(identifier);

            string result = HandleAttestationDeattestation(identifier, AttestationMode.Deattestation);

            return HttpUtility.UrlEncode(result).Replace("+", "%20");
        }


        private static string HandleAttestationDeattestation (string identifier, AttestationMode mode)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            IAttestable attestableItem = null;
            string attestedTemplate = string.Empty;
            string deattestedTemplate = string.Empty;

            char costType = identifier[0];
            int itemId = Int32.Parse(identifier.Substring(1));
            Int64 amountCents;
            string beneficiary = string.Empty;
            string result = string.Empty;

            // Find the item we are attesting or deattesting

            switch(costType)
            {
                case 'A': // Case advance
                    CashAdvance advance = CashAdvance.FromIdentity(itemId);
                    if (advance.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException("Called to attest out-of-org line item");
                    }
                    if (advance.Budget.OwnerPersonId != authData.CurrentUser.Identity && advance.Budget.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityAccessDeniedException("Called without attestation privileges");
                    }

                    attestableItem = advance;
                    attestedTemplate = Resources.Pages_Financial.AttestCosts_AdvanceAttested;
                    deattestedTemplate = Resources.Pages_Financial.AttestCosts_AdvanceDeattested;
                    beneficiary = advance.Person.Name;
                    amountCents = advance.AmountCents;

                    break;
                case 'E': // Expense claim
                    ExpenseClaim expense = ExpenseClaim.FromIdentity(itemId);
                    if (expense.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException("Called to attest out-of-org line item");
                    }
                    if (expense.Budget.OwnerPersonId != authData.CurrentUser.Identity && expense.Budget.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityAccessDeniedException("Called without attestation privileges");
                    }

                    attestableItem = expense;
                    attestedTemplate = Resources.Pages_Financial.AttestCosts_ExpenseAttested;
                    deattestedTemplate = Resources.Pages_Financial.AttestCosts_ExpenseDeattested;
                    beneficiary = expense.Claimer.Name;
                    amountCents = expense.AmountCents;

                    break;
                case 'I': // Inbound invoice
                    InboundInvoice invoice = InboundInvoice.FromIdentity(itemId);
                    if (invoice.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException("Called to attest out-of-org line item");
                    }
                    if (invoice.Budget.OwnerPersonId != authData.CurrentUser.Identity && invoice.Budget.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityAccessDeniedException("Called without attestation privileges");
                    }

                    attestableItem = invoice;
                    attestedTemplate = Resources.Pages_Financial.AttestCosts_InvoiceAttested;
                    deattestedTemplate = Resources.Pages_Financial.AttestCosts_InvoiceDeattested;
                    beneficiary = invoice.Supplier;
                    amountCents = invoice.AmountCents;

                    break;
                case 'S': // Salary payout
                    Salary salary = Salary.FromIdentity(itemId);
                    if (salary.PayrollItem.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException("Called to attest out-of-org line item");
                    }
                    if (salary.PayrollItem.Budget.OwnerPersonId != authData.CurrentUser.Identity && salary.PayrollItem.Budget.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityAccessDeniedException("Called without attestation privileges");
                    }

                    attestableItem = salary;
                    attestedTemplate = Resources.Pages_Financial.AttestCosts_SalaryAttested;
                    deattestedTemplate = Resources.Pages_Financial.AttestCosts_SalaryDeattested;
                    beneficiary = salary.PayrollItem.PersonCanonical;
                    amountCents = salary.GrossSalaryCents;

                    break;
                case 'P': // Parley, aka Conference
                    Parley parley = Parley.FromIdentity(itemId);
                    if (parley.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException("Called to attest out-of-org line item");
                    }
                    if (parley.Budget.OwnerPersonId != authData.CurrentUser.Identity && parley.Budget.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityAccessDeniedException("Called without attestation privileges");
                    }

                    attestableItem = parley;
                    attestedTemplate = Resources.Pages_Financial.AttestCosts_ParleyAttested;
                    deattestedTemplate = Resources.Pages_Financial.AttestCosts_ParleyDeattested;
                    beneficiary = parley.Person.Name;
                    amountCents = parley.BudgetCents;

                    break;
                default:
                    throw new InvalidOperationException("Unknown Cost Type in HandleAttestationDeattestation: \"" + identifier + "\"");
            }

            // Finally, attest or deattest

            if (mode == AttestationMode.Attestation)
            {
                attestableItem.Attest(authData.CurrentUser);
                result = string.Format(attestedTemplate, itemId, beneficiary, authData.CurrentOrganization.Currency.Code,
                                       amountCents/100.0);
            }
            else if (mode == AttestationMode.Deattestation)
            {
                attestableItem.Deattest(authData.CurrentUser);
                result = string.Format(deattestedTemplate, itemId, beneficiary, authData.CurrentOrganization.Currency.Code,
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
            ExpenseClaims expenses = ExpenseClaims.ForOrganization(this.CurrentOrganization).WhereUnattested;

            foreach (var expenseClaim in expenses)
            {
                if (_attestationRights.ContainsKey(expenseClaim.BudgetId) || expenseClaim.Budget.OwnerPersonId == Person.NobodyId)
                {
                    AddDocuments(expenseClaim.Documents, "E" + expenseClaim.Identity.ToString(CultureInfo.InvariantCulture), String.Format(Resources.Global.Financial_ExpenseClaimSpecificationWithClaimer + " - ", expenseClaim.Identity, expenseClaim.ClaimerCanonical) + Resources.Global.Financial_ReceiptSpecification);
                }
            }
        }


        private void PopulateInboundInvoices()
        {
            InboundInvoices invoices = InboundInvoices.ForOrganization(this.CurrentOrganization).WhereUnattested;

            foreach (InboundInvoice invoice in invoices)
            {
                Documents dox = invoice.Documents;
                bool hasDox = (dox.Count > 0 ? true : false);

                if (_attestationRights.ContainsKey(invoice.BudgetId) || invoice.Budget.OwnerPersonId == Person.NobodyId)
                {
                    AddDocuments(invoice.Documents, "I" + invoice.Identity.ToString(CultureInfo.InvariantCulture), String.Format(Resources.Global.Financial_InboundInvoiceSpecificationWithSender + " - ", invoice.Identity, invoice.Supplier) + Resources.Global.Global_ImageSpecification);
                }
            }
        }


        private void AddDocuments(Documents docs, string baseId, string titleBase)
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