using System;
using System.Security;
using System.Web;
using System.Web.Services;
using Resources;

using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.Admin.Debug
{
    public partial class DebugSockets : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageIcon = "iconshock-group-search";
            this.PageAccessRequired = new Access (CurrentOrganization, AccessAspect.PersonalData, AccessType.Read);

            if (!Page.IsPostBack)
            {
                Localize();
            }

            RegisterControl (EasyUIControl.Tree | EasyUIControl.DataGrid);
        }

        private void Localize()
        {
            PageTitle = @"Debug Sockets";
            InfoBoxLiteral = @"This troubleshoots socket connections by showing which connections are live.";

            /* no localization for debug pages */
        }

        [WebMethod]
        public static ConfirmPayoutResult ConfirmPayout (string protoIdentity)
        {
            protoIdentity = HttpUtility.UrlDecode (protoIdentity);

            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.Authority.HasAccess (new Access (AccessAspect.System, AccessType.Read)))
            {
                throw new SecurityException("Insufficient privileges for operation");
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

            result.DisplayMessage = CommonV5.JavascriptEscape(result.DisplayMessage);

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


        // ReSharper disable once InconsistentNaming
        public string Localized_NoticeTooManyHits
        {
            get { return JavascriptEscape(Resources.Pages.Swarm.ListFindPeople_TooManyHits); }
        }
    }
}