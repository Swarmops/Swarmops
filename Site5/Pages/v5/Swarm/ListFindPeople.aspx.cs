using System;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Services;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.Swarm
{
    public partial class ListFindPeople : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageIcon = "iconshock-group-search";

            if (!Page.IsPostBack)
            {
                Localize();
            }

            this.EasyUIControlsUsed = EasyUIControl.Tree;
        }

        private void Localize()
        {
            this.PageTitle = Resources.Pages_People.ListFindPeople_Title;
            this.InfoBoxLiteral = Resources.Pages_People.ListFindPeople_Info;
            this.LabelGeography.Text = Resources.Global.Global_Geography;
            this.LabelNamePattern.Text = Resources.Pages_People.ListFindPeople_NamePattern;
            this.LabelMatchingPeopleInX.Text = String.Format(Resources.Pages_People.ListFindPeople_MatchingPeopleInX,
                this.CurrentOrganization.Name);

            this.LabelGridHeaderAction.Text = Resources.Global.Global_Action;
            this.LabelGridHeaderGeography.Text = Resources.Global.Global_Geography;
            this.LabelGridHeaderMail.Text = Resources.Global.Global_Mail;
            this.LabelGridHeaderName.Text = Resources.Global.Global_Name;
            this.LabelGridHeaderPhone.Text = Resources.Global.Global_Phone;
            this.LabelGridHeaderNotes.Text = Resources.Global.Global_Notes;

        }

        public struct ConfirmPayoutResult
        {
            public int AssignedId;
            public string DisplayMessage;
        };

        [WebMethod]
        public static ConfirmPayoutResult ConfirmPayout (string protoIdentity)
        {
            protoIdentity = HttpUtility.UrlDecode(protoIdentity);

            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (!authData.CurrentUser.HasAccess(new Access(authData.CurrentOrganization, AccessAspect.Financials, AccessType.Write)))
            {
                throw new SecurityAccessDeniedException("Insufficient privileges for operation");
            }

            ConfirmPayoutResult result = new ConfirmPayoutResult();

            Payout payout = Payout.CreateFromProtoIdentity(authData.CurrentUser, protoIdentity);
            Swarmops.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.PayoutCreated,
                                                       authData.CurrentUser.Identity, 1, 1, 0, payout.Identity,
                                                       protoIdentity);

            // Create result and return it

            result.AssignedId = payout.Identity;
            result.DisplayMessage = String.Format(Resources.Pages_Financial.PayOutMoney_PayoutCreated, payout.Identity,
                                                  payout.Recipient);

            result.DisplayMessage = HttpUtility.UrlEncode(result.DisplayMessage).Replace("+", "%20");

            return result;
        }

        public struct UndoPayoutResult
        {
            public bool Success;
            public string DisplayMessage;
        }

        [WebMethod]
        public static UndoPayoutResult UndoPayout (int databaseId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            UndoPayoutResult result = new UndoPayoutResult();

            Payout payout = Payout.FromIdentity(databaseId);

            if (!payout.Open)
            {
                // this payout has already been settled, or picked up for settling

                result.Success = false;
                result.DisplayMessage = String.Format(Resources.Pages_Financial.PayOutMoney_PayoutCannotUndo,
                                                      databaseId);

                return result;
            }

            payout.UndoPayout();

            result.DisplayMessage = HttpUtility.UrlEncode(String.Format(Resources.Pages_Financial.PayOutMoney_PayoutUndone, databaseId)).Replace("+","%20");
            result.Success = true;
            return result;
        }
    }

}