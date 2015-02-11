using System;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Services;
using Resources;
using Resources.Pages;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.Swarm
{
    public partial class ListFindPeople : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageIcon = "iconshock-group-search";

            if (!Page.IsPostBack)
            {
                Localize();
            }

            RegisterControl (EasyUIControl.Tree | EasyUIControl.DataGrid);
        }

        private void Localize()
        {
            PageTitle = Resources.Pages.Swarm.ListFindPeople_Title;
            InfoBoxLiteral = Resources.Pages.Swarm.ListFindPeople_Info;
            this.LabelGeography.Text = Global.Global_Geography;
            this.LabelNamePattern.Text = Resources.Pages.Swarm.ListFindPeople_NamePattern;
            this.LabelMatchingPeopleInX.Text = String.Format (Resources.Pages.Swarm.ListFindPeople_MatchingPeopleInX,
                CurrentOrganization.Name);

            this.LabelGridHeaderAction.Text = Global.Global_Action;
            this.LabelGridHeaderGeography.Text = Global.Global_Geography;
            this.LabelGridHeaderMail.Text = Global.Global_Mail;
            this.LabelGridHeaderName.Text = Global.Global_Name;
            this.LabelGridHeaderPhone.Text = Global.Global_Phone;
            this.LabelGridHeaderNotes.Text = Global.Global_Notes;
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
            result.DisplayMessage = String.Format (Financial.PayOutMoney_PayoutCreated, payout.Identity,
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
                result.DisplayMessage = String.Format (Financial.PayOutMoney_PayoutCannotUndo,
                    databaseId);

                return result;
            }

            payout.UndoPayout();

            result.DisplayMessage =
                HttpUtility.UrlEncode (String.Format (Financial.PayOutMoney_PayoutUndone, databaseId))
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