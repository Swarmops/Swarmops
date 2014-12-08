using System;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Services;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

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

            PageIcon = "iconshock-money-envelope";

            if (!Page.IsPostBack)
            {
                Localize();
            }

            EasyUIControlsUsed = EasyUIControl.DataGrid;
        }

        private void Localize()
        {
            PageTitle = Resources.Pages.Financial.PayOutMoney_PageTitle;
            InfoBoxLiteral = Resources.Pages.Financial.PayOutMoney_Info;
            this.LabelPayOutMoneyHeader.Text = Resources.Pages.Financial.PayOutMoney_Header;
            this.LabelGridHeaderAccount.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_BankAccount;
            this.LabelGridHeaderAmount.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_Amount;
            this.LabelGridHeaderBank.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_BankName;
            this.LabelGridHeaderDue.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_DueDate;
            this.LabelGridHeaderPaid.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_PaidOut;
            this.LabelGridHeaderRecipient.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_Recipient;
            this.LabelGridHeaderReference.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_Reference;
        }

        [WebMethod]
        public static ConfirmPayoutResult ConfirmPayout (string protoIdentity)
        {
            protoIdentity = HttpUtility.UrlDecode (protoIdentity);

            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.CurrentUser.HasAccess (new Access (authData.CurrentOrganization, AccessAspect.Financials,
                    AccessType.Write)))
            {
                throw new SecurityAccessDeniedException ("Insufficient privileges for operation");
            }

            ConfirmPayoutResult result = new ConfirmPayoutResult();

            Payout payout = Payout.CreateFromProtoIdentity (authData.CurrentUser, protoIdentity);
            PWEvents.CreateEvent (EventSource.PirateWeb, EventType.PayoutCreated,
                authData.CurrentUser.Identity, 1, 1, 0, payout.Identity,
                protoIdentity);

            // Create result and return it

            result.AssignedId = payout.Identity;
            result.DisplayMessage = String.Format (Resources.Pages.Financial.PayOutMoney_PayoutCreated, payout.Identity,
                payout.Recipient);

            result.DisplayMessage = HttpUtility.UrlEncode (result.DisplayMessage).Replace ("+", "%20");

            return result;
        }

        [WebMethod]
        public static UndoPayoutResult UndoPayout (int databaseId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            UndoPayoutResult result = new UndoPayoutResult();

            Payout payout = Payout.FromIdentity (databaseId);

            if (!payout.Open)
            {
                // this payout has already been settled, or picked up for settling

                result.Success = false;
                result.DisplayMessage = String.Format (Resources.Pages.Financial.PayOutMoney_PayoutCannotUndo,
                    databaseId);

                return result;
            }

            payout.UndoPayout();

            result.DisplayMessage =
                HttpUtility.UrlEncode (String.Format (Resources.Pages.Financial.PayOutMoney_PayoutUndone, databaseId))
                    .Replace ("+", "%20");
            result.Success = true;
            return result;
        }

        public struct ConfirmPayoutResult
        {
            public int AssignedId;
            public string DisplayMessage;
        };

        public struct UndoPayoutResult
        {
            public string DisplayMessage;
            public bool Success;
        }
    }
}