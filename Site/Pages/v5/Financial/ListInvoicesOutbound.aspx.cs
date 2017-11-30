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
    public partial class ListInvoicesOutbound : PageV5Base
    {
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

            this._documentList = new List<RepeatedDocument>();

            PopulateOutboundInvoices();

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
            PageTitle = Resources.Pages.Financial.ListOutboundInvoices_Title;
            InfoBoxLiteral = Resources.Pages.Financial.ListOutboundInvoices_Info;
            this.LabelListOutboundInvoicesHeader.Text = Resources.Pages.Financial.ListOutboundInvoices_Header;
            this.LabelGridHeaderItem.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Item;
            this.LabelGridHeaderDueDate.Text = Resources.Global.Financial_DueDate;
            this.LabelGridHeaderCustomer.Text = Resources.Pages.Financial.ListOutboundInvoices_CustomerReceivingInvoice;
            this.LabelGridHeaderCreated.Text = Resources.Pages.Financial.ListOutboundInvoices_CreatedDateTime;
            this.LabelGridHeaderAmountTotal.Text =
                Resources.Pages.Financial.ListOutboundInvoices_InvoiceAmountTotalIncludingTaxes;
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



        private void PopulateOutboundInvoices()
        {
            OutboundInvoices invoices = OutboundInvoices.ForOrganization(CurrentOrganization, true);

            foreach (OutboundInvoice invoice in invoices)
            {
                Documents dox = invoice.Documents;
                bool hasDox = (dox.Count > 0 ? true : false);

                if (hasDox)
                {
                    AddDocuments(invoice.Documents, "O" + invoice.Identity.ToString(CultureInfo.InvariantCulture),
                        String.Format(Global.Financial_OutboundInvoiceSpecificationWithCustomer + " - ",
                            invoice.OrganizationSequenceId, invoice.CustomerName) + Global.Global_ImageSpecification);
                }
            }
        }


        private void AddDocuments(Documents docs, string baseId, string titleBase)
        {
            int countTotal = docs.Count;
            int count = 0;

            foreach (Document document in docs)
            {
                RepeatedDocument newDoc = new RepeatedDocument
                {
                    DocId = document.Identity,
                    BaseId = baseId,
                    Title = String.Format(titleBase, ++count, countTotal)
                };

                this._documentList.Add(newDoc);
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