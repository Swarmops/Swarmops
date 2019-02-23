using System;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Services;
using Swarmops.Common.Enums;
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
            PageTitle = Resources.Pages.Financial.PayOutMoney_PageTitle;
            InfoBoxLiteral = Resources.Pages.Financial.PayOutMoney_Info;
            this.LabelPayOutMoneyHeader.Text = Resources.Pages.Financial.PayOutMoney_Header;
            this.LabelGridHeaderAmount.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_Amount;
            this.LabelGridHeaderDue.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_DueDate;
            this.LabelGridHeaderPay.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_PayThis;
            this.LabelGridHeaderRecipient.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_Recipient;
            this.LabelGridHeaderCurrencyMethod.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_CurrencyMethod;

            this.LabelPayOutMoneyOcrHeader.Text = Resources.Pages.Financial.PayoutMoney_Header_Ocr;
            this.LabelGridHeaderAccountOcr.Text = Resources.Pages.Financial.PayoutMoney_GridHeader_Account_Ocr;
            this.LabelGridHeaderDue2.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_DueDate; // same as above
            this.LabelGridHeaderReferenceOcr.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_Reference_Ocr;
            this.LabelGridHeaderAmountOcr.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_Amount_Ocr;
            this.LabelGridHeaderPaid2.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_PaidOut;

            this.LabelSidebarOptions.Text = Resources.Global.Sidebar_Options;
            this.LabelOptionsShowOcr.Text = Resources.Pages.Financial.PayoutMoney_OptionShowOcr;
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
                DisplayMessage = String.Format(Resources.Pages.Financial.PayOutMoney_PayoutCreated, payout.Identity,
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
                // this payout has already been settled, or picked up for settling. This is a concurrency error, only detected earlier than the database layer.

                return new AjaxCallResult
                {
                    Success = false,
                    DisplayMessage = String.Format(Resources.Pages.Financial.PayOutMoney_PayoutCannotUndo,
                        databaseId)
                };
            }

            payout.UndoPayout();   // TODO: catch ConcurrencyException

            return new AjaxCallResult
            {
                DisplayMessage = String.Format(Resources.Pages.Financial.PayOutMoney_PayoutUndone, databaseId),
                Success = true
            };
        }

        public class ConfirmPayoutResult: AjaxCallResult
        {
            public int AssignedId;
        };

        public class PayoutInformationResult : AjaxCallResult
        {
            public string Recipient { get; set; }
            public string CurrencyAmount { get; set; }
            public string Reference { get; set; }
            public string TransferMethod { get; set; }
            public string[] TransferMethodLabels { get; set; }
            public string[] TransferMethodData { get; set; }
        }


        // --------- Localization strings UX-side ------------

        public string Localized_ComfirmDialog_ConfirmPaid
        {
            get { return CommonV5.JavascriptEscape(Resources.Pages.Financial.PayOutMoney_Modal_ConfirmPaid); }
        }
    }
}