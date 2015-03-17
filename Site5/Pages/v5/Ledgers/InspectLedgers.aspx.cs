using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using Resources;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
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
            PageTitle = Resources.Pages.Ledgers.InspectLedgers_PageTitle;
            InfoBoxLiteral = Resources.Pages.Ledgers.InspectLedgers_Info;
            PageIcon = "iconshock-ledger-inspect";

            if (!Page.IsPostBack)
            {
                DateTime today = DateTime.Today;
                int year = today.Year;
                int firstYear = CurrentOrganization.FirstFiscalYear;

                while (year >= firstYear)
                {
                    this.DropYears.Items.Add (year.ToString (CultureInfo.InvariantCulture));
                    year--;
                }

                for (int monthNumber = 1; monthNumber <= 12; monthNumber++)
                {
                    this.DropMonths.Items.Add (new ListItem (new DateTime (2014, monthNumber, 1).ToString ("MMM"),
                        monthNumber.ToString (CultureInfo.InvariantCulture))); // will autolocalize
                }

                this.DropMonths.Items.Add (new ListItem (Global.Global_Q1, "21"));
                // quarters and all-year are coded as fake month numbers
                this.DropMonths.Items.Add (new ListItem (Global.Global_Q2, "22"));
                this.DropMonths.Items.Add (new ListItem (Global.Global_Q3, "23"));
                this.DropMonths.Items.Add (new ListItem (Global.Global_Q4, "24"));
                this.DropMonths.Items.Add (new ListItem (Global.Global_AllYear, "31"));

                this.DropYears.SelectedIndex = 0;
                this.DropMonths.SelectedValue = today.Month.ToString (CultureInfo.InvariantCulture);
            }

            RegisterControl (EasyUIControl.DataGrid | EasyUIControl.Tree);

            Localize();
        }

        private void Localize()
        {
            this.LabelHeaderInspect.Text = Resources.Pages.Ledgers.InspectLedgers_Header_Inspect;
            this.LabelHeaderInspectFor.Text = Resources.Pages.Ledgers.InspectLedgers_Header_For;
            this.LabelGridHeaderAction.Text = Global.Global_Action;
            this.LabelGridHeaderBalance.Text = Global.Ledgers_Balance;
            this.LabelGridHeaderDateTime.Text = Global.Global_Timestamp;
            this.LabelGridHeaderDeltaNegative.Text = Global.Ledgers_Credit;
            this.LabelGridHeaderDeltaPositive.Text = Global.Ledgers_Debit;
            this.LabelGridHeaderDescription.Text = Global.Global_Description;
            this.LabelGridHeaderId.Text = Resources.Pages.Ledgers.InspectLedgers_TransactionId;

            
            this.LabelGridHeaderAccountName.Text = Resources.Pages.Ledgers.InspectLedgers_AccountName;
            this.LabelGridHeaderDateTimeEntered.Text = Resources.Pages.Ledgers.InspectLedgers_LoggedDate;
            this.LabelGridHeaderDeltaNegative2.Text = Global.Ledgers_Credit;
            this.LabelGridHeaderDeltaPositive2.Text = Global.Ledgers_Debit;
            this.LabelGridHeaderInitials.Text = Resources.Pages.Ledgers.InspectLedgers_LoggedByInitials;

            this.LabelFlagNotAvailable.Text = Resources.Pages.Ledgers.InspectLedgers_FlaggingNotAvailable;

            
            this.LabelAddTransactionRowsHeader.Text = Resources.Pages.Ledgers.InspectLedgers_HeaderAddTransactionRow;
            this.LabelTrackedTransactionHeader.Text =
                Resources.Pages.Ledgers.InspectLedgers_HeaderAutoTransactionTracking;

            // if write access

            if (
                CurrentUser.HasAccess (new Access (CurrentOrganization, AccessAspect.BookkeepingDetails,
                    AccessType.Write)))
            {
                LiteralEditHeader.Text = Resources.Pages.Ledgers.InspectLedgers_EditingTransactionX;

                this.LabelAddRowAccount.Text = Global.Financial_Account;
                this.LabelAddRowAmount.Text = Global.Financial_Amount;
                this.LiteralErrorAddRowSelectAccount.Text =
                    Resources.Pages.Ledgers.InspectLedgers_TxDetail_ErrorAddRowNoAccount;
                this.LiteralAddRowButton.Text = Global.Global_Add;
            }
            else // read access, at a minimum of AccessAspect.Bookkeeping
            {
                LiteralEditHeader.Text = Resources.Pages.Ledgers.InspectLedgers_InspectingTransactionX;
            }

            // Access helpers to JavaScript (these don't actually determine access, but help in UI prettiness)

            this.LiteralAuditAccess.Text =
                (CurrentUser.HasAccess (new Access (CurrentOrganization, AccessAspect.Auditing, AccessType.Write)))
                    ? "true"
                    : "false";
            this.LiteralWriteAccess.Text =
                (CurrentUser.HasAccess (new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write)))
                    ? "true"
                    : "false";
            this.LiteralDetailAccess.Text =
                (CurrentUser.HasAccess (new Access (CurrentOrganization, AccessAspect.BookkeepingDetails,
                    AccessType.Read)))
                    ? "true"
                    : "false";

            this.LiteralLedgersClosedUntil.Text = CurrentOrganization.Parameters.FiscalBooksClosedUntilYear.ToString();
        }

        [WebMethod]
        public static bool AddTransactionRow (int txId, int accountId, string amountString)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.CurrentUser.HasAccess (new Access (authData.CurrentOrganization,
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

            Double amountFloat = Double.Parse (amountString);
            Int64 amountCents = (Int64) (amountFloat*100.0);

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
                !authData.CurrentUser.HasAccess (new Access (authData.CurrentOrganization,
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
                !authData.CurrentUser.HasAccess (new Access (authData.CurrentOrganization,
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
                        String.Format (Resources.Pages.Ledgers.InspectLedgers_TxDetail_ThisIsPayoutX, payout.Identity) +
                        " ";

                    List<string> subValidations = new List<string>();
                    List<string> subResults = new List<string>();

                    if (payout.DependentExpenseClaims.Count > 0)
                    {
                        if (payout.DependentExpenseClaims.Count == 1)
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (Global.Financial_ExpenseClaimSpecificationWithClaimer,
                                                payout.DependentExpenseClaims[0].Identity,
                                                HttpUtility.HtmlEncode (payout.DependentExpenseClaims[0].Claimer.Name)) +
                                            ".</strong>");
                        }
                        else
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (Global.Financial_ExpenseClaimsSpecificationWithClaimer,
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
                                            String.Format (Global.Financial_CashAdvancePaybackSpecification,
                                                payout.DependentCashAdvancesPayback[0].Identity) + ".</strong>");
                        }
                        else
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (Global.Financial_CashAdvancePaybacksSpecification,
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
                                            String.Format (Global.Financial_CashAdvanceLongSpecificationWithRecipient,
                                                advance0.Identity, advance0.Person.Name) + ".</strong>");
                        }
                        else
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (Global.Financial_CashAdvancesLongSpecificationWithRecipient,
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
                            Global.Financial_InboundInvoiceSpecificationWithSender, invoice.Identity,
                            invoice.Supplier) + "</strong>");

                        subValidations.Add (GetObjectDetails (invoice));
                    }

                    if (payout.DependentSalariesNet.Count > 0)
                    {
                        // Assume one salary

                        Salary salary = payout.DependentSalariesNet[0];

                        subResults.Add ("<strong>" +
                                        String.Format (Global.Financial_SalaryDualSpecificationWithRecipient,
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
                                            String.Format (Global.Financial_SalaryTaxDualSpecification, salary0.Identity,
                                                salary0.PayoutDate) + "</strong>");
                        }
                        else
                        {
                            subResults.Add ("<strong>" +
                                            String.Format (Global.Financial_SalariesTaxSpecification, salary0.PayoutDate) +
                                            "</strong>");
                        }
                    }


                    result +=
                        String.Join (" " + Resources.Pages.Ledgers.InspectLedgers_TxDetail_CombinedWith + " ",
                            subResults) + ". " +
                        String.Format (Resources.Pages.Ledgers.InspectLedgers_TxDetail_PaidOutBy,
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
                if (validation.ValidationType == FinancialValidationType.Attestation)
                {
                    result += String.Format (Resources.Pages.Ledgers.InspectLedgers_TxDetail_AttestedByX + ". ",
                        validation.Person.Canonical,
                        validation.DateTime);
                }
                if (validation.ValidationType == FinancialValidationType.Validation)
                {
                    result += String.Format (Resources.Pages.Ledgers.InspectLedgers_TxDetail_ValidatedByX + ". ",
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
                           String.Format (Global.Financial_ExpenseClaimLongSpecification, claim.Identity) +
                           ":</strong> " + claim.Organization.Currency.Code + " " +
                           (claim.AmountCents/100.0).ToString ("N2") + ". " +
                           HttpUtility.HtmlEncode (GetValidationDetails (claim.Validations)) + " " +
                           GetDocumentDetails (claim.Documents, claim);

                case "CashAdvance":
                    CashAdvance advance = (CashAdvance) identifiableObject;

                    return "<strong>" +
                           String.Format (Global.Financial_CashAdvanceSpecification, advance.Identity) +
                           ":</strong> " + advance.Organization.Currency.Code + " " +
                           (advance.AmountCents/100.0).ToString ("N2") + ". " +
                           HttpUtility.HtmlEncode (GetValidationDetails (advance.Validations));

                case "InboundInvoice":
                    InboundInvoice invoice = (InboundInvoice) identifiableObject;

                    return "<strong>" +
                           String.Format (Global.Financial_InboundInvoiceSpecification, invoice.Identity) +
                           ":</strong> " + invoice.Organization.Currency.Code + " " +
                           (invoice.AmountCents/100.0).ToString ("N2") + ". " +
                           GetValidationDetails (invoice.Validations) + " " +
                           GetDocumentDetails (invoice.Documents, invoice);

                case "Salary":
                    Salary salary = (Salary) identifiableObject;

                    return "<strong>" +
                           String.Format (Global.Financial_SalaryIdentity, salary.Identity) +
                           ":</strong> " +
                           String.Format (Resources.Pages.Ledgers.InspectLedgers_TxDetail_SalaryDetail,
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
    }
}