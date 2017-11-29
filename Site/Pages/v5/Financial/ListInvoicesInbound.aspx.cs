using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using Resources;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class ListInvoicesInbound : PageV5Base
    {
        private Dictionary<int, bool> _attestationRights;
        private List<RepeatedDocument> _documentList;

        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            PageIcon = "iconshock-stamped-paper";

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Participant, AccessType.Read);   // No specific access aspect for owning a budget (yet?)

            if (!Page.IsPostBack)
            {
                Localize();
            }

            // this.TextDenyReason.Style [HtmlTextWriterStyle.FontSize] = "60% !important";

            this._attestationRights = GetAttestationRights();
            this._documentList = new List<RepeatedDocument>();
            this.LiteralCanOverdraftBudgets.Text =
                CurrentAuthority.HasAccess (new Access (CurrentOrganization, AccessAspect.Administration)).ToString(CultureInfo.InvariantCulture).ToLowerInvariant();

            PopulateInboundInvoices();
            PopulateExpenses();
            // PopulateSalaries();

            RegisterControl(EasyUIControl.DataGrid);

            this.RepeaterLightboxItems.DataSource = this._documentList;
            this.RepeaterLightboxItems.DataBind();
        }


        static private Dictionary<int, bool> GetAttestationRights()
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            // Right now, this function is quite primitive. At some point in the future, it needs to take into
            // account that a budget may have several attesters. Right now, it just loops over all accounts and
            // checks the owner.

            Dictionary<int, bool> result = new Dictionary<int, bool>();
            FinancialAccounts accounts = FinancialAccounts.ForOrganization(authData.CurrentOrganization);

            foreach (FinancialAccount account in accounts)
            {
                if (account.OwnerPersonId == authData.CurrentUser.Identity ||
                    (account.OwnerPersonId == 0 && authData.Authority.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.Administration))))
                {
                    if (account.AccountType == FinancialAccountType.Cost)
                    {
                        result[account.Identity] = true;
                    }
                }
            }

            return result;
        }

        [WebMethod]
        public static int[] GetUninitializedBudgets()
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            // This function finds the exceptional accounts that aren't initialized, to let a sysadmin approve expenses to them.

            List<int> result = new List<int>();
            FinancialAccounts accounts = FinancialAccounts.ForOrganization(authData.CurrentOrganization);

            foreach (FinancialAccount account in accounts)
            {
                if (account.AccountType == FinancialAccountType.Cost)
                {
                    if (account.OwnerPersonId == 0 &&
                        authData.Authority.HasAccess (new Access (authData.CurrentOrganization,
                            AccessAspect.Administration)))
                    {
                        result.Add (account.Identity);
                    }
                }
            }

            return result.ToArray();
        }

        private void Localize()
        {
            PageTitle = Resources.Pages.Financial.ListInboundInvoices_Title;
            InfoBoxLiteral = Resources.Pages.Financial.ListInboundInvoices_Info;
            this.LabelListInboundInvoicesHeader.Text = Resources.Pages.Financial.ListInboundInvoices_Header;
            this.LabelGridHeaderItem.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Item;
            this.LabelGridHeaderDueDate.Text = Resources.Global.Financial_DueDate;
            this.LabelGridHeaderBeneficiary.Text = Resources.Pages.Financial.ListInboundInvoices_Header_Sender;
            this.LabelGridHeaderBudget.Text = Resources.Global.Financial_Budget;
            this.LabelGridHeaderRequested.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Requested;
            this.LabelGridHeaderProgress.Text = Resources.Global.Global_Progress;
            this.LabelGridHeaderDocs.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Docs;
            this.LabelGridHeaderActions.Text = Resources.Global.Global_Action;

            /* this.LiteralErrorInsufficientBudget.Text = JavascriptEscape (Resources.Pages.Financial.AttestCosts_OutOfBudget);
             this.LiteralWarnUnintializedBudget.Text =
                 JavascriptEscape (Resources.Pages.Financial.AttestCosts_Warn_UninitializedBudget);*/
            /*
                        this.LabelDescribeDeny.Text = Resources.Pages.Financial.AttestCosts_Modal_DescribeOptionDeny;
                        this.LabelDescribeCorrect.Text = String.Format (Resources.Pages.Financial.AttestCosts_Modal_DescribeOptionAmount, CurrentOrganization.Currency.DisplayCode);
                        this.LabelDescribeRebudget.Text = Resources.Pages.Financial.AttestCosts_Modal_DescribeOptionRebudget;

                        this.LabelRadioCorrect.Text = Resources.Pages.Financial.AttestCosts_Modal_RadioOptionAmount;
                        this.LabelRadioDeny.Text = Resources.Pages.Financial.AttestCosts_Modal_RadioOptionDeny;
                        this.LabelRadioRebudget.Text = Resources.Pages.Financial.AttestCosts_Modal_RadioOptionRebudget;*/

            this.LiteralButtonCorrect.Text = JavascriptEscape(Resources.Pages.Financial.AttestCosts_Modal_ButtonAmount); 
            this.LiteralButtonDeny.Text = JavascriptEscape(Resources.Pages.Financial.AttestCosts_Modal_ButtonDeny);
            this.LiteralButtonRebudget.Text = JavascriptEscape(Resources.Pages.Financial.AttestCosts_Modal_ButtonRebudget);

   /*         this.LabelModalDenyHeader.Text = Resources.Pages.Financial.AttestCosts_Modal_Header;
            this.LabelWhatProblem.Text = Resources.Pages.Financial.AttestCosts_Modal_WhatIsProblem;

            this.LiteralPleaseSelectBudget.Text = JavascriptEscape (Resources.Pages.Financial.AttestCosts_Error_PleaseSelectBudget);
            this.LiteralCannotRebudgetSalary.Text =
                JavascriptEscape (Resources.Pages.Financial.AttestCosts_Error_CantRebudgetSalary);

            this.LiteralConfirmOverdraftNo.Text = JavascriptEscape(Resources.Pages.Financial.AttestCosts_OverdraftConfirmNo);
            this.LiteralConfirmOverdraftYes.Text = JavascriptEscape(Resources.Pages.Financial.AttestCosts_OverdraftConfirmYes);
            this.LiteralConfirmOverdraft.Text = JavascriptEscape(Resources.Pages.Financial.AttestCosts_OverdraftConfirm);*/
        }

        static protected IPayable PayableFromRecordId (string recordId)
        {
            char recordType = recordId[0];
            int itemId = Int32.Parse(recordId.Substring(1));
            
            switch (recordType)
            {
                case 'E': // Expense claim
                    return ExpenseClaim.FromIdentity (itemId);
                case 'A': // Cash advance
                    return CashAdvance.FromIdentity(itemId);
                case 'I': // Inbound invoice
                    return InboundInvoice.FromIdentity (itemId);
                default:
                    throw new NotImplementedException("Unknown record type");
            }
        }


        [WebMethod]
        public static AjaxCallResult DenyItem (string recordId, string reason)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            IPayable payable = PayableFromRecordId (recordId);
            FinancialAccount budget = payable.Budget;

            if (budget.OrganizationId != authData.CurrentOrganization.Identity ||
                (budget.OwnerPersonId != authData.CurrentUser.Identity &&
                budget.OwnerPersonId != Person.NobodyId))
            {
                throw new UnauthorizedAccessException();
            }

            if (String.IsNullOrEmpty (reason.Trim()))
            {
                reason = Resources.Global.Global_NoReasonGiven;
            }

            payable.DenyAttestation (authData.CurrentUser, reason);

            return new AjaxCallResult { Success = true };
        }


        [WebMethod]
        public static void RebudgetItem (string recordId, int newAccountId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            IPayable payable = PayableFromRecordId (recordId);
            FinancialAccount newAccount = FinancialAccount.FromIdentity(newAccountId);

            if (payable.Budget.OrganizationId != authData.CurrentOrganization.Identity ||
                payable.Budget.OwnerPersonId != authData.CurrentUser.Identity ||
                newAccount.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException();
            }

            payable.SetBudget (newAccount, authData.CurrentUser);
        }

        [WebMethod]
        public static AjaxCallResult AttestCorrectedItem(string recordId, string amountString)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            double amount = 0.0;

            try
            {
                amount = Double.Parse (amountString, NumberStyles.Currency, Thread.CurrentThread.CurrentCulture);
            }
            catch (Exception)
            {
                return new AjaxCallResult
                {
                    Success = false,
                    DisplayMessage = String.Format(Resources.Global.Error_CurrencyParsing, 1000.00)
                };
            }

            Int64 amountCents = (Int64) (amount*100);

            if (amountCents < 0)
            {
                return new AjaxCallResult
                {
                    Success = false,
                    DisplayMessage = Resources.Pages.Financial.AttestCosts_CannotAttestNegative
                };
            }

            if (amountCents == 0)
            {
                return new AjaxCallResult
                {
                    Success = false,
                    DisplayMessage = Resources.Pages.Financial.AttestCosts_CannotAttestZero
                };
            }

            IPayable payable = PayableFromRecordId (recordId);
            FinancialAccount budget = payable.Budget;

            if (budget.OrganizationId != authData.CurrentOrganization.Identity ||
                budget.OwnerPersonId != authData.CurrentUser.Identity)
            {
                throw new UnauthorizedAccessException();
            }

            Int64 centsRemaining = budget.GetBudgetCentsRemaining();

            if (centsRemaining/100.0 < amount)
            {
                // TODO: Handle the special case where the IPayable is not on current year, so against another (last) year's budget

                string notEnoughFunds;

                if (centsRemaining > 0)
                {
                    notEnoughFunds = String.Format (Resources.Pages.Financial.AttestCosts_OutOfBudgetPrecise,
                        authData.CurrentOrganization.Currency.DisplayCode, centsRemaining/100.0, DateTime.UtcNow.Year);
                }
                else
                {
                    notEnoughFunds = String.Format (Resources.Pages.Financial.AttestCosts_BudgetIsEmpty,
                        DateTime.UtcNow.Year);
                }

                return new AjaxCallResult
                {
                    Success = false,
                    DisplayMessage = notEnoughFunds
                };
            }

            payable.SetAmountCents (amountCents, authData.CurrentUser);
            payable.Attest (authData.CurrentUser);

            return new AjaxCallResult
            {
                Success = true
            };
        }



        [WebMethod]
        public static BudgetRemainder[] GetRemainingBudgets()
        {
            Dictionary<int, bool> accountLookup = GetAttestationRights();

            if (accountLookup.Count == 0)
            {
                return new BudgetRemainder[0];
            }

            Organization organization = FinancialAccount.FromIdentity (accountLookup.Keys.First()).Organization;

            List<BudgetRemainder> result = new List<BudgetRemainder>();
            int currentYear = DateTime.UtcNow.Year;

            foreach (int accountId in accountLookup.Keys)
            {
                FinancialAccount account = FinancialAccount.FromIdentity (accountId);
                Int64 remaining = account.GetBudgetCentsRemaining (currentYear);

                result.Add (new BudgetRemainder { AccountId = accountId, Remaining = remaining/100.0 });
            }

            return result.ToArray();
        }


        public class BudgetRemainder
        {
            public int AccountId { get; set; }
            public double Remaining { get; set; }
        }

       
        
        [WebMethod]
        public static AjaxCallResult Attest (string identifier)
        {
            identifier = HttpUtility.UrlDecode (identifier);

            return HandleAttestationDeattestation (identifier, AttestationMode.Attestation);
        }

        [WebMethod]
        public static AjaxCallResult Deattest (string identifier)
        {
            identifier = HttpUtility.UrlDecode (identifier);

            return HandleAttestationDeattestation (identifier, AttestationMode.Deattestation);
        }


        private static AjaxCallResult HandleAttestationDeattestation (string identifier, AttestationMode mode)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            IAttestable attestableItem;
            string attestedTemplate;
            string deattestedTemplate;

            char costType = identifier[0];
            int itemId = Int32.Parse (identifier.Substring (1));
            Int64 amountCents;
            string beneficiary;
            string result;

            // Find the item we are attesting or deattesting

            switch (costType)
            {
                case 'A': // Case advance
                    CashAdvance advance = CashAdvance.FromIdentity (itemId);
                    if (advance.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException ("Called to attest out-of-org line item");
                    }
                    if (advance.Budget.OwnerPersonId != authData.CurrentUser.Identity &&
                        advance.Budget.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityException ("Called without attestation privileges");
                    }

                    attestableItem = advance;
                    attestedTemplate = Resources.Pages.Financial.AttestCosts_AdvanceAttested;
                    deattestedTemplate = Resources.Pages.Financial.AttestCosts_AdvanceDeattested;
                    beneficiary = advance.Person.Name;
                    amountCents = advance.AmountCents;

                    break;
                case 'E': // Expense claim
                    ExpenseClaim expense = ExpenseClaim.FromIdentity (itemId);
                    if (expense.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException ("Called to attest out-of-org line item");
                    }
                    if (expense.Budget.OwnerPersonId != authData.CurrentUser.Identity &&
                        expense.Budget.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityException ("Called without attestation privileges");
                    }

                    attestableItem = expense;
                    attestedTemplate = Resources.Pages.Financial.AttestCosts_ExpenseAttested;
                    deattestedTemplate = Resources.Pages.Financial.AttestCosts_ExpenseDeattested;
                    beneficiary = expense.Claimer.Name;
                    amountCents = expense.AmountCents;

                    break;
                case 'I': // Inbound invoice
                    InboundInvoice invoice = InboundInvoice.FromIdentity (itemId);
                    if (invoice.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException ("Called to attest out-of-org line item");
                    }
                    if (invoice.Budget.OwnerPersonId != authData.CurrentUser.Identity &&
                        invoice.Budget.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityException ("Called without attestation privileges");
                    }

                    attestableItem = invoice;
                    attestedTemplate = Resources.Pages.Financial.AttestCosts_InvoiceAttested;
                    deattestedTemplate = Resources.Pages.Financial.AttestCosts_InvoiceDeattested;
                    beneficiary = invoice.Supplier;
                    amountCents = invoice.AmountCents;

                    break;
                case 'S': // Salary payout
                    Salary salary = Salary.FromIdentity (itemId);
                    if (salary.PayrollItem.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException ("Called to attest out-of-org line item");
                    }
                    if (salary.PayrollItem.Budget.OwnerPersonId != authData.CurrentUser.Identity &&
                        salary.PayrollItem.Budget.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityException ("Called without attestation privileges");
                    }

                    attestableItem = salary;
                    attestedTemplate = Resources.Pages.Financial.AttestCosts_SalaryAttested;
                    deattestedTemplate = Resources.Pages.Financial.AttestCosts_SalaryDeattested;
                    beneficiary = salary.PayrollItem.PersonCanonical;
                    amountCents = salary.GrossSalaryCents + salary.AdditiveTaxCents;

                    break;
                case 'P': // Parley, aka Conference
                    Parley parley = Parley.FromIdentity (itemId);
                    if (parley.OrganizationId != authData.CurrentOrganization.Identity)
                    {
                        throw new InvalidOperationException ("Called to attest out-of-org line item");
                    }
                    if (parley.Budget.OwnerPersonId != authData.CurrentUser.Identity &&
                        parley.Budget.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityException ("Called without attestation privileges");
                    }

                    attestableItem = parley;
                    attestedTemplate = Resources.Pages.Financial.AttestCosts_ParleyAttested;
                    deattestedTemplate = Resources.Pages.Financial.AttestCosts_ParleyDeattested;
                    beneficiary = parley.Person.Name;
                    amountCents = parley.BudgetCents;

                    break;
                default:
                    throw new InvalidOperationException ("Unknown Cost Type in HandleAttestationDeattestation: \"" +
                                                         identifier + "\"");
            }

            // Finally, attest or deattest

            if (mode == AttestationMode.Attestation)
            {
                Int64 budgetRemaining = attestableItem.Budget.GetBudgetCentsRemaining();

                result = string.Empty;

                if (amountCents > -budgetRemaining)
                {
                    if (
                        authData.Authority.HasAccess (new Access (authData.CurrentOrganization,
                            AccessAspect.Administration)))
                    {
                        // Admin rights, so allow (forced) overdraft

                        // Unless budget was nonzero and allocated, set protest message

                        if (attestableItem.Budget.Owner != null || attestableItem.Budget.GetBudgetCents() != 0)

                        result = Resources.Pages.Financial.AttestCosts_Overdrafted + " ";
                    }
                    else
                    {
                        // Do not allow overdraft

                        return new AjaxCallResult
                        {
                            DisplayMessage = Resources.Pages.Financial.AttestCosts_OutOfBudget,
                            Success = false
                        };
                    }
                }

                attestableItem.Attest (authData.CurrentUser);
                result += string.Format (attestedTemplate, itemId, beneficiary,
                    authData.CurrentOrganization.Currency.Code,
                    amountCents/100.0);
            }
            else if (mode == AttestationMode.Deattestation)
            {
                attestableItem.Deattest (authData.CurrentUser);
                result = string.Format (deattestedTemplate, itemId, beneficiary,
                    authData.CurrentOrganization.Currency.Code,
                    amountCents/100.0);
            }
            else
            {
                throw new InvalidOperationException ("Unknown Attestation Mode: " + mode);
            }

            FinancialAccount.ClearAttestationAdjustmentsCache (authData.CurrentOrganization);

            return new AjaxCallResult {DisplayMessage = result, Success = true};
        }


        private void PopulateExpenses()
        {
            ExpenseClaims expenses = ExpenseClaims.ForOrganization (CurrentOrganization).WhereUnattested;

            foreach (ExpenseClaim expenseClaim in expenses)
            {
                if (this._attestationRights.ContainsKey (expenseClaim.BudgetId) ||
                    expenseClaim.Budget.OwnerPersonId == Person.NobodyId)
                {
                    AddDocuments (expenseClaim.Documents,
                        "E" + expenseClaim.Identity.ToString (CultureInfo.InvariantCulture),
                        String.Format (Global.Financial_ExpenseClaimSpecificationWithClaimer + " - ",
                            expenseClaim.Identity, expenseClaim.ClaimerCanonical) +
                        Global.Financial_ReceiptSpecification);
                }
            }
        }


        private void PopulateInboundInvoices()
        {
            InboundInvoices invoices = InboundInvoices.ForOrganization (CurrentOrganization, true);

            foreach (InboundInvoice invoice in invoices)
            {
                Documents dox = invoice.Documents;
                bool hasDox = (dox.Count > 0 ? true : false);

                AddDocuments (invoice.Documents, "I" + invoice.Identity.ToString (CultureInfo.InvariantCulture),
                    String.Format (Global.Financial_InboundInvoiceSpecificationWithSender + " - ",
                        invoice.OrganizationSequenceId, invoice.Supplier) + Global.Global_ImageSpecification);
            }
        }


        private void AddDocuments (Documents docs, string baseId, string titleBase)
        {
            int countTotal = docs.Count;
            int count = 0;

            foreach (Document document in docs)
            {
                RepeatedDocument newDoc = new RepeatedDocument
                {
                    DocId = document.Identity,
                    BaseId = baseId,
                    Title = String.Format (titleBase, ++count, countTotal)
                };

                this._documentList.Add (newDoc);
            }
        }

        private enum AttestationMode
        {
            Unknown = 0,
            Attestation,
            Deattestation
        };

        public class RepeatedDocument
        {
            public int DocId { get; set; }
            public string BaseId { get; set; }
            public string Title { get; set; }
        }


    }
}