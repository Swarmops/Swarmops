using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Localization;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class InspectLedgers : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read);
            DbVersionRequired = 0; // base schema is fine
            PageTitle = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_PageTitle");
            InfoBoxLiteral = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_Info");
            PageIcon = "iconshock-ledger-inspect";

            if (!Page.IsPostBack)
            {
                DateTime today = DateTime.Today;
                int year = today.Year;
                int firstYear = CurrentOrganization.FirstFiscalYear;

                while (year >= firstYear)
                {
                    this.DropYears.Items.Add(year.ToString(CultureInfo.InvariantCulture));
                    this.DropGeneralYears.Items.Add(year.ToString(CultureInfo.InvariantCulture));
                    this.DropHotwalletYears.Items.Add(year.ToString(CultureInfo.InvariantCulture));
                    year--;
                }

                for (int monthNumber = 1; monthNumber <= 12; monthNumber++)
                {
                    this.DropMonths.Items.Add(new ListItem(new DateTime(2014, monthNumber, 1).ToString("MMM"),
                        monthNumber.ToString(CultureInfo.InvariantCulture))); // will autolocalize
                    this.DropGeneralMonths.Items.Add(new ListItem(new DateTime(2014, monthNumber, 1).ToString("MMM"),
                        monthNumber.ToString(CultureInfo.InvariantCulture))); // will autolocalize
                    this.DropHotwalletMonths.Items.Add(new ListItem(new DateTime(2014, monthNumber, 1).ToString("MMM"),
                        monthNumber.ToString(CultureInfo.InvariantCulture))); // will autolocalize
                }

                this.DropMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q1"), "21"));
                // quarters and all-year are coded as fake month numbers
                this.DropMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q2"), "22"));
                this.DropMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q3"), "23"));
                this.DropMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q4"), "24"));
                this.DropMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_AllYear"), "31"));

                this.DropGeneralMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q1"), "21"));
                // quarters and all-year are coded as fake month numbers
                this.DropGeneralMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q2"), "22"));
                this.DropGeneralMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q3"), "23"));
                this.DropGeneralMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q4"), "24"));
                this.DropGeneralMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_AllYear"), "31"));

                this.DropHotwalletMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q1"), "21"));
                // quarters and all-year are coded as fake month numbers
                this.DropHotwalletMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q2"), "22"));
                this.DropHotwalletMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q3"), "23"));
                this.DropHotwalletMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_Q4"), "24"));
                this.DropHotwalletMonths.Items.Add(new ListItem(LocalizedStrings.Get(LocDomain.Global, "Global_AllYear"), "31"));

                DateTime lastMonth = today.AddMonths(-1);

                this.DropYears.SelectedValue = 
                this.DropGeneralYears.SelectedValue =
                this.DropHotwalletYears.SelectedValue =
                    lastMonth.Year.ToString(CultureInfo.InvariantCulture);

                this.DropMonths.SelectedValue =
                this.DropGeneralMonths.SelectedValue =
                this.DropHotwalletMonths.SelectedValue =
                    lastMonth.Month.ToString(CultureInfo.InvariantCulture);
            }

            RegisterControl (EasyUIControl.DataGrid | EasyUIControl.Tree);

            Localize();
        }

        private void Localize()
        {
            this.LabelHeaderInspect.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "");
            this.LabelHeaderInspect.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_Header_Inspect");
            this.LabelHeaderInspectFor.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_Header_For");
            this.LabelHeaderGeneral.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_GeneralLedger");
            this.LabelHeaderHotwallet.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_Header_Hotwallet");

            this.LabelGridHeaderAction.Text = this.LabelTreeHeaderAction.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Action");
            this.LabelGridHeaderBalance.Text = this.LabelTreeHeaderBalance.Text = LocalizedStrings.Get(LocDomain.Global, "Ledgers_Balance");
            this.LabelGridHeaderDateTime.Text = this.LabelTreeHeaderDateTime.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Timestamp");
            this.LabelGridHeaderDeltaNegative.Text = this.LabelTreeHeaderDeltaNegative.Text = LocalizedStrings.Get(LocDomain.Global, "Ledgers_Credit");
            this.LabelGridHeaderDeltaPositive.Text = this.LabelTreeHeaderDeltaPositive.Text = LocalizedStrings.Get(LocDomain.Global, "Ledgers_Debit");
            this.LabelGridHeaderDescription.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Description");
            this.LabelTreeHeaderDescriptionAccount.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_Detail");
            this.LabelGridHeaderId.Text = this.LabelTreeHeaderId.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_TransactionId");
            
            this.LabelGridHeaderAccountName.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_AccountName");
            this.LabelGridHeaderDateTimeEntered.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_LoggedDate");
            this.LabelGridHeaderDeltaNegative2.Text = LocalizedStrings.Get(LocDomain.Global, "Ledgers_Credit");
            this.LabelGridHeaderDeltaPositive2.Text = LocalizedStrings.Get(LocDomain.Global, "Ledgers_Debit");
            this.LabelGridHeaderInitials.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_LoggedByInitials");

            this.LabelHotwalletHeaderDateTime.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Timestamp");
            this.LabelHotwalletHeaderDescription.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Description");
            this.LabelHotwalletHeaderId.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_TransactionId");
            this.LabelHotwalletHeaderPresentationCurrency.Text = CurrentOrganization.Currency.DisplayCode;
            this.LabelHotwalletHeaderMicrocoin.Text = Currency.BitcoinCash.DisplayCode;
            this.LabelHotwalletHeaderBalance.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "BitcoinHotwallet_BalanceMicrocoins");
            this.LabelHotwalletActions.Text = LocalizedStrings.Get(LocDomain.Global, "Global_Action");

            this.LabelFlagNotAvailable.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_FlaggingNotAvailable");

            
            this.LabelAddTransactionRowsHeader.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_HeaderAddTransactionRow");
            this.LabelTrackedTransactionHeader.Text =
                LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_HeaderAutoTransactionTracking");

            // if write access

            if (
                CurrentAuthority.HasAccess (new Access (CurrentOrganization, AccessAspect.BookkeepingDetails,
                    AccessType.Write)))
            {
                LiteralEditHeader.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_EditingTransactionX");

                this.LabelAddRowAccount.Text = LocalizedStrings.Get(LocDomain.Global, "Financial_Account");
                this.LabelAddRowAmount.Text = LocalizedStrings.Get(LocDomain.Global, "Financial_Amount");
                this.LiteralErrorAddRowSelectAccount.Text =
                    LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_TxDetail_ErrorAddRowNoAccount");
                this.LiteralAddRowButton.Text = JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "Global_Add"));
            }
            else // read access, at a minimum of AccessAspect.Bookkeeping
            {
                LiteralEditHeader.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_InspectingTransactionX");
            }

            this.LabelCreateTxDialogHeader.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_CreateTransactionDialogHeader");
            this.LabelAddTxDateTime.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_CreateTransactionDateTime");
            this.LabelAddTxDescription.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_CreateTransactionDescription");
            this.LabelAddTxFirstRowAccount.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_CreateTransactionFirstAccount");
            this.LabelAddTxFirstRowAmount.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_CreateTransactionFirstAmount");

            this.LabelAddTransaction.Text = LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_CreateTransactionSidebar");

            // Access helpers to JavaScript (these don't actually determine access, but help in UI prettiness)

            this.LiteralAuditAccess.Text =
                (CurrentAuthority.HasAccess (new Access (CurrentOrganization, AccessAspect.Auditing, AccessType.Write)))
                    ? "true"
                    : "false";
            this.LiteralWriteAccess.Text =
                (CurrentAuthority.HasAccess(new Access(CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write)))
                    ? "true"
                    : "false";
            this.LiteralDetailAccess.Text =
                (CurrentAuthority.HasAccess(new Access(CurrentOrganization, AccessAspect.BookkeepingDetails, AccessType.Read)))
                    ? "true"
                    : "false";

            this.LiteralLedgersClosedUntil.Text = CurrentOrganization.Parameters.FiscalBooksClosedUntilYear.ToString();
        }

        [WebMethod]
        public static bool AddTransactionRow (int txId, int accountId, string amountString)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.Authority.HasAccess (new Access (authData.CurrentOrganization,
                    AccessAspect.Bookkeeping, AccessType.Write)))
            {
                return false; // fail
            }

            FinancialTransaction transaction = FinancialTransaction.FromIdentity (txId);

            if (authData.CurrentOrganization.Parameters.FiscalBooksClosedUntilYear >= transaction.DateTime.Year)
            {
                return false; // can't edit closed books
            }

            FinancialAccount account = FinancialAccount.FromIdentity (accountId);

            Int64 amountCents = Formatting.ParseDoubleStringAsCents(amountString);

            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new InvalidOperationException ("Account/Organization mismatch");
            }

            if (amountCents == 0)
            {
                return false;
            }

            transaction.AddRow (account, amountCents, authData.CurrentUser);

            return true;
        }

        [WebMethod]
        public static string GetUnbalancedAmount (int txId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.Authority.HasAccess (new Access (authData.CurrentOrganization,
                    AccessAspect.Bookkeeping, AccessType.Read)))
            {
                return string.Empty; // leave no clue to an attacker why the call failed
            }

            FinancialTransaction transaction = FinancialTransaction.FromIdentity (txId);

            return (-transaction.Rows.AmountCentsTotal/100.0).ToString ("N2");
        }

        [WebMethod]
        public static string GetTransactionTracking (int txId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.Authority.HasAccess (new Access (authData.CurrentOrganization,
                    AccessAspect.BookkeepingDetails, AccessType.Read)))
            {
                return string.Empty; // leave no clue to an attacker why the call failed
            }

            FinancialTransaction transaction = FinancialTransaction.FromIdentity (txId);

            IHasIdentity dependency = transaction.Dependency;

            if (dependency == null)
            {
                return string.Empty;
            }

            return GetTrackingDetails (dependency);
        }

        private static string GetTrackingDetails (object someObject)
        {
            string objectType = someObject.GetType().Name;

            switch (objectType)
            {
                case "Payout":
                    Payout payout = (Payout) someObject;
                    string result =
                        String.Format (LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_TxDetail_ThisIsPayoutX"), payout.Identity) +
                        " ";

                    List<string> subValidations = new List<string>();
                    List<string> subResults = new List<string>();

                    if (payout.DependentExpenseClaims.Count > 0)
                    {
                        if (payout.DependentExpenseClaims.Count == 1)
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_ExpenseClaimSpecificationWithClaimer"),
                                                payout.DependentExpenseClaims[0].OrganizationSequenceId,
                                                HttpUtility.HtmlEncode (payout.DependentExpenseClaims[0].Claimer.Name)) +
                                            ".</strong>");
                        }
                        else
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_ExpenseClaimsSpecificationWithClaimer"),
                                                Formatting.GenerateRangeString (payout.DependentExpenseClaims.Identities),
                                                HttpUtility.HtmlEncode (payout.DependentExpenseClaims[0].Claimer.Name)) +
                                            ".</strong>");
                        }

                        foreach (ExpenseClaim claim in payout.DependentExpenseClaims)
                        {
                            subValidations.Add (GetObjectDetails (claim));
                        }
                    }

                    if (payout.DependentCashAdvancesPayback.Count > 0)
                    {
                        if (payout.DependentCashAdvancesPayback.Count == 1)
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_CashAdvancePaybackSpecification"),
                                                payout.DependentCashAdvancesPayback[0].Identity) + ".</strong>");
                        }
                        else
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_CashAdvancePaybacksSpecification"),
                                                Formatting.GenerateRangeString (payout.DependentExpenseClaims.Identities)) +
                                            ".</strong>");
                        }
                    }

                    if (payout.DependentCashAdvancesPayout.Count > 0)
                    {
                        CashAdvance advance0 = payout.DependentCashAdvancesPayout[0];

                        if (payout.DependentCashAdvancesPayout.Count == 1)
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_CashAdvanceLongSpecificationWithRecipient"),
                                                advance0.OrganizationSequenceId, advance0.Person.Name) + ".</strong>");
                        }
                        else
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_CashAdvancesLongSpecificationWithRecipient"),
                                                Formatting.GenerateRangeString (payout.DependentExpenseClaims.Identities),
                                                advance0.Person.Name) + ".</strong>");
                        }

                        foreach (CashAdvance advance in payout.DependentCashAdvancesPayout)
                        {
                            subValidations.Add (GetObjectDetails (advance));
                        }
                    }

                    if (payout.DependentInvoices.Count > 0)
                    {
                        // Assume _one_ invoice

                        InboundInvoice invoice = payout.DependentInvoices[0];

                        subResults.Add ("<strong>" + String.Format (
                            LocalizedStrings.Get(LocDomain.Global, "Financial_InboundInvoiceSpecificationWithSender"), invoice.OrganizationSequenceId,
                            invoice.Supplier) + "</strong>");

                        subValidations.Add (GetObjectDetails (invoice));
                    }

                    if (payout.DependentSalariesNet.Count > 0)
                    {
                        // Assume one salary

                        Salary salary = payout.DependentSalariesNet[0];

                        subResults.Add ("<strong>" +
                                        String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_SalaryDualSpecificationWithRecipient"),
                                            salary.Identity, salary.PayoutDate,
                                            HttpUtility.HtmlEncode (salary.PayrollItem.PersonCanonical)) +
                                        "</strong>");

                        subValidations.Add (GetObjectDetails (salary));
                    }

                    if (payout.DependentSalariesTax.Count > 0)
                    {
                        Salary salary0 = payout.DependentSalariesTax[0];

                        if (payout.DependentSalariesTax.Count == 1)
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_SalaryTaxDualSpecification"), salary0.Identity,
                                                salary0.PayoutDate) + "</strong>");
                        }
                        else
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_SalariesTaxSpecification"), salary0.PayoutDate) +
                                            "</strong>");
                        }
                    }


                    result +=
                        String.Join (" " + LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_TxDetail_CombinedWith") + " ",
                            subResults) + ". " +
                        String.Format (LocalizedStrings.Get(LocDomain.Global, "InspectLedgers_TxDetail_PaidOutBy"),
                            payout.CreatedByPerson.Canonical);

                    return "<p>" + result + "</p><p>" + String.Join ("</p><p>", subValidations) + "</p>";

                case "ExpenseClaim":
                case "CashAdvance":
                case "InboundInvoice":
                case "Salary":
                    return "<p>" + GetObjectDetails ((IHasIdentity) someObject) + "</p>";

                default:
                    return "Unimplemented dependency type: " + objectType;
            }
        }

        private static string GetValidationDetails (FinancialValidations validations)
        {
            string result = string.Empty;

            foreach (FinancialValidation validation in validations)
            {
                if (validation.ValidationType == FinancialValidationType.Approval)
                {
                    result += String.Format (LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_TxDetail_AttestedByX") + ". ",
                        validation.Person.Canonical,
                        validation.DateTime);
                }
                if (validation.ValidationType == FinancialValidationType.Validation)
                {
                    result += String.Format (LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_TxDetail_ValidatedByX") + ". ",
                        validation.Person.Canonical,
                        validation.DateTime);
                }
            }

            return result;
        }

        private static string GetDocumentDetails (Documents documents, IHasIdentity identifiableObject)
        {
            string docLink = string.Empty;
            string objectIdString = identifiableObject.GetType().Name + identifiableObject.Identity;

            foreach (Document document in documents)
            {
                docLink += "<a href='/Pages/v5/Support/StreamUpload.aspx?DocId=" + document.Identity +
                           "' class='FancyBox_Gallery' rel='" + objectIdString + "'>&nbsp;</a>";
            }

            return "Documents were uploaded by " + documents[0].UploadedByPerson.Canonical + " at " +
                   documents[0].UploadedDateTime.ToString ("yyyy-MMM-dd HH:mm") +
                   ". <a href='#' class='linkViewDox' objectId='" + objectIdString +
                   "'>View documents.</a><span class='hiddenDocLinks'>" + docLink + "</span>";
        }

        private static string GetObjectDetails (IHasIdentity identifiableObject)
        {
            switch (identifiableObject.GetType().Name)
            {
                case "ExpenseClaim":
                    ExpenseClaim claim = (ExpenseClaim) identifiableObject;

                    return "<strong>" +
                           String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_ExpenseClaimLongSpecification"), claim.Identity) +
                           ":</strong> " + claim.Organization.Currency.Code + " " +
                           (claim.AmountCents/100.0).ToString ("N2") + ". " +
                           HttpUtility.HtmlEncode (GetValidationDetails (claim.Validations)) + " " +
                           GetDocumentDetails (claim.Documents, claim);

                case "CashAdvance":
                    CashAdvance advance = (CashAdvance) identifiableObject;

                    return "<strong>" +
                           String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_CashAdvanceSpecification"), advance.Identity) +
                           ":</strong> " + advance.Organization.Currency.Code + " " +
                           (advance.AmountCents/100.0).ToString ("N2") + ". " +
                           HttpUtility.HtmlEncode (GetValidationDetails (advance.Validations));

                case "InboundInvoice":
                    InboundInvoice invoice = (InboundInvoice) identifiableObject;

                    return "<strong>" +
                           String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_InboundInvoiceSpecification"), invoice.Identity) +
                           ":</strong> " + invoice.Organization.Currency.Code + " " +
                           (invoice.AmountCents/100.0).ToString ("N2") + ". " +
                           GetValidationDetails (invoice.Validations) + " " +
                           GetDocumentDetails (invoice.Documents, invoice);

                case "Salary":
                    Salary salary = (Salary) identifiableObject;

                    return "<strong>" +
                           String.Format (LocalizedStrings.Get(LocDomain.Global, "Financial_SalaryIdentity"), salary.Identity) +
                           ":</strong> " +
                           String.Format (LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_TxDetail_SalaryDetail"),
                               salary.PayrollItem.Organization.Currency.Code,
                               salary.BaseSalaryCents/100.0, // base salary
                               (salary.GrossSalaryCents - salary.BaseSalaryCents)/100.0, // before-tax adjustments
                               salary.GrossSalaryCents/100.0, // before-tax adjusted salary
                               salary.SubtractiveTaxCents/100.0, // tax deduction
                               (salary.NetSalaryCents + salary.SubtractiveTaxCents -
                                salary.GrossSalaryCents)/100.0, // after-tax adjustments
                               salary.NetSalaryCents/100.0) + // actual payout amount
                           " " + GetValidationDetails (salary.Validations);

                default:
                    throw new NotImplementedException ("Unhandled object type in GetObjectDetails: " +
                                                       identifiableObject.GetType().Name);
            }
        }

        [WebMethod]
        public static AjaxInputCallResult CreateTransaction(string dateTimeString, string amountString, string description,
            int budgetId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            FinancialAccount budget = FinancialAccount.FromIdentity(budgetId);

            if (budget.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException();
            }
            if (!authData.Authority.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.BookkeepingDetails)))
            {
                throw new UnauthorizedAccessException();
            }

            Int64 amountCents = Formatting.ParseDoubleStringAsCents(amountString);

            DateTime txTime = DateTime.Parse(dateTimeString);

            // TODO: Return better error codes/messages if one of these fail

            FinancialTransaction transaction = FinancialTransaction.Create(authData.CurrentOrganization, txTime,
                description);
            transaction.AddRow(budget, amountCents, authData.CurrentUser);

            AjaxInputCallResult result = new AjaxInputCallResult
            {
                Success = true,
                ObjectIdentity = transaction.Identity,
                NewValue = (amountCents/100.0).ToString("N2")
            };

            return result;
        }




        public string Localized_CreateTx
        {
            get { return LocalizedStrings.Get(LocDomain.PagesLedgers, "InspectLedgers_CreateTransactionButton"); }
        }
    }
}