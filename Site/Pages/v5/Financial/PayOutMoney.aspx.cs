using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Resources;
using System.Security;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using Swarmops.Common;
using Swarmops.Common.Enums;
using Swarmops.Localization;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.LogEntries;

namespace Swarmops.Frontend.Pages.Financial
{
    public partial class PayOutMoney : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Financials, AccessType.Write);

            PageIcon = "iconshock-money-envelope";

            if (!Page.IsPostBack)
            {
                Localize();
            }

            RegisterControl (EasyUIControl.DataGrid);
        }

        private void Localize()
        {
            PageTitle = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_PageTitle");
            InfoBoxLiteral = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_Info");
            this.LabelPayOutMoneyHeader.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_Header");
            this.LabelGridHeaderAmount.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_GridHeader_Amount");
            this.LabelGridHeaderDue.Text = LocalizedStrings.Get(LocDomain.Global, "Financial_DueDateShort");
            this.LabelGridHeaderPay.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_GridHeader_PayThis");
            this.LabelGridHeaderRecipient.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_GridHeader_Recipient");
            this.LabelGridHeaderCurrencyMethod.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_GridHeader_CurrencyMethod");

            this.LabelModalOcr1.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_Modal_LabelOcr1");
            this.ToggleModalMachineReadable.Label = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_Modal_LabelOcr2");
            this.LabelModalRecipient.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_GridHeader_Recipient");
            this.LabelModalReference.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_GridHeader_Reference");
            this.LabelModalHeader.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_Modal_Header");
            this.LabelModalCurrencyAmount.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_Modal_CurrencyAmount");
            this.LabelModalTransferMethod.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_Modal_TransferMethod");
            this.LabelModalHeaderDue.Text = LocalizedStrings.Get(LocDomain.Global, "Financial_DueDateShort");

            this.LabelModalAutomation1.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_AutomationFieldStart");
            this.LabelModalAutomation2.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_AutomationFieldNext");
            this.LabelModalAutomation3.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_AutomationFieldNext");

            this.LabelSidebarOptions.Text = LocalizedStrings.Get(LocDomain.Global, "Sidebar_Options");
            this.LabelOptionsShowOcr.Text = LocalizedStrings.Get(LocDomain.PagesFinancial, "PayoutMoney_OptionShowOcr");
        }


        [WebMethod]
        public static ConfirmPayoutResult ConfirmPayout (string protoIdentity)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.Authority.HasAccess (new Access (authData.CurrentOrganization, AccessAspect.Financials, AccessType.Write)))
            {
                throw new UnauthorizedAccessException("Insufficient privileges for operation");
            }

            Payout payout = Payout.CreateFromProtoIdentity (authData.CurrentUser, protoIdentity); // TODO: Catch ConcurrencyException

            // Create result and return it

            return new ConfirmPayoutResult
            {
                AssignedId = payout.Identity,
                DisplayMessage = String.Format(LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_PayoutCreated, payout.Identity"),
                    payout.Recipient),
                Success = true
            };
        }

        [WebMethod]
        public static AjaxCallResult UndoPayout (int databaseId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.Authority.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.Financials)))
            {
                throw new UnauthorizedAccessException("Insufficient privileges for operation");
            }

            Payout payout = Payout.FromIdentity (databaseId);

            if (!payout.Open)
            {
                // this payout has already been settled, or picked up for settling. This is a concurrency error, detected before actually trying to change it.

                return new AjaxCallResult
                {
                    Success = false,
                    DisplayMessage = String.Format(LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_PayoutCannotUndo"),
                        databaseId)
                };
            }

            payout.UndoPayout();   // TODO: catch ConcurrencyException

            return new AjaxCallResult
            {
                DisplayMessage = String.Format(LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_PayoutUndone"), databaseId),
                Success = true
            };
        }

        [WebMethod]
        public static PaymentTransferInfoResult GetPaymentTransferInfo(string prototypeId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            // TODO: Authentication check

            string[] payoutComponents = prototypeId.Split('|');

            if (payoutComponents.Length < 1)
            {
                throw new InvalidOperationException("Prototype ID can't be empty");
            }

            PaymentTransferInfo info = new PaymentTransferInfo();

            // Some payouts are composites of multiple objects, but all these will share the same
            // payout data, so we can safely use just the first object to determine payment
            // target information
            //
            // with one exception -- we need to determine the amount by adding all the objects
            // together, if applicable

            DateTime paymentDueBy = Constants.DateTimeLow;

            switch (Char.ToUpperInvariant(payoutComponents[0][0]))
            {
                case 'C': // expense claim
                    info =
                        PaymentTransferInfo.FromObject(
                            ExpenseClaim.FromIdentity(Int32.Parse(payoutComponents[0].Substring(1))),
                            new Money(GetSumCentsTotal(prototypeId), authData.CurrentOrganization.Currency));
                    break;
                case 'A': // cash advance (payout or payback, same logic either way)
                    info =
                        PaymentTransferInfo.FromObject(
                            CashAdvance.FromIdentity(Int32.Parse(payoutComponents[0].Substring(1))),
                            new Money(GetSumCentsTotal(prototypeId), authData.CurrentOrganization.Currency));
                    break;
                case 'S': // salary
                    Salary salary = Salary.FromIdentity(Int32.Parse(payoutComponents[0].Substring(1)));
                    info = PaymentTransferInfo.FromObject(salary);
                    paymentDueBy = salary.PayoutDate;
                    break;
                case 'I': // inbound invoice
                    InboundInvoice invoice = InboundInvoice.FromIdentity(Int32.Parse(payoutComponents[0].Substring(1)));
                    info = PaymentTransferInfo.FromObject(invoice);
                    paymentDueBy = invoice.DueDate;
                    break;
                default:
                    throw new NotImplementedException("Unrecognized payment type");
            }

            PaymentTransferInfoResult result = new PaymentTransferInfoResult
            {
                Success = true,
                CurrencyAmount = info.CurrencyAmount,
                DisplayMessage = string.Empty,
                Recipient = info.Recipient,
                Reference = info.Reference,
                TransferMethod = info.LocalizedPaymentMethodName
            };

            if (paymentDueBy < Constants.DateTimeLowThreshold)
            {
                result.DueBy = LocalizedStrings.Get(LocDomain.Global, "Global_ASAP");
            }
            else
            {
                DateTime nowUtc = DateTime.UtcNow;

                if (paymentDueBy.Year != nowUtc.Year || paymentDueBy < nowUtc.AddMonths(-3))
                {
                    result.DueBy = paymentDueBy.ToString(LocalizedStrings.Get(LocDomain.Global, "Global_DateFormatLongSansWeekday"));
                }
                else
                {
                    result.DueBy = paymentDueBy.ToString(LocalizedStrings.Get(LocDomain.Global, "Global_DateFormatLongDateMonth"));
                }

                if (paymentDueBy < nowUtc.AddDays(-1))
                {
                    result.DueBy += " - " + LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_PaymentLate");
                }
            }

            List<string> listTransferMethodLabels = new List<string>();
            List<string> listTransferMethodData = new List<string>();

            foreach (string label in info.LocalizedPaymentInformation.Keys)
            {
                listTransferMethodLabels.Add(HttpUtility.HtmlEncode(label));
                listTransferMethodData.Add(HttpUtility.HtmlEncode(info.LocalizedPaymentInformation [label]));
            }

            result.TransferMethodLabels = listTransferMethodLabels.ToArray();
            result.TransferMethodData = listTransferMethodData.ToArray();
            result.OcrData = info.OcrData;  // can be null and that's ok

            return result;
        }


        static private Int64 GetSumCentsTotal(string prototypeId)
        {
            string[] payoutComponents = prototypeId.Split('|');

            Int64 amountCentsTotal = 0;

            foreach (string payoutComponent in payoutComponents)
            {
                switch (payoutComponent[0])
                {
                    case 'C':
                        amountCentsTotal += ExpenseClaim.FromIdentity(Int32.Parse(payoutComponent.Substring(1))).AmountCents;
                        break;
                    case 'A':  // Advance pay-OUT
                        amountCentsTotal += CashAdvance.FromIdentity(Int32.Parse(payoutComponent.Substring(1))).AmountCents;
                        break;
                    case 'a':  // Advance pay-BACK
                        amountCentsTotal -= CashAdvance.FromIdentity(Int32.Parse(payoutComponent.Substring(1))).AmountCents;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return amountCentsTotal;
        }

        public class ConfirmPayoutResult: AjaxCallResult
        {
            public int AssignedId;
        };

        public class PaymentTransferInfoResult : AjaxCallResult
        {
            public string Recipient { get; set; }
            public string CurrencyAmount { get; set; }
            public string DueBy { get; set; }
            public string Reference { get; set; }
            public string TransferMethod { get; set; }
            public string[] TransferMethodLabels { get; set; }
            public string[] TransferMethodData { get; set; }
            public string[] OcrData { get; set; }
        }


        // --------- Localization strings UX-side ------------

        public string Localized_ConfirmDialog_ConfirmPaid
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.PagesFinancial, "PayOutMoney_Modal_ConfirmPaid")); }
        }

        public string Localized_IconTooltip_Barcode
        {
            get { return CommonV5.JavascriptEscape(LocalizedStrings.Get(LocDomain.Global, "IconTooltip_Barcode_Scanning")); }
        }
    }
}