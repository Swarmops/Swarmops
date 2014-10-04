using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Enums;
using Swarmops.Database;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.Comms
{
    public partial class SendMassMessage : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageIcon = "iconshock-group-search";

            if (!Page.IsPostBack)
            {
                Localize();
            }

            this.TextMessage.Style[HtmlTextWriterStyle.Width] = "674px";
            this.TextMessage.Style["resize"] = "none";
        }

        private void Localize()
        {
            this.PageTitle = Resources.Pages.Comms.SendMassMessage_Title;
            this.InfoBoxLiteral = Resources.Pages.People.ListFindPeople_Info;
            this.LabelGeography.Text = Resources.Pages.Comms.SendMassMessage_Geography;
            this.LabelRecipientType.Text = Resources.Pages.Comms.SendMassMessage_RecipientType;
            this.LabelHeaderMessage.Text = Resources.Pages.Comms.SendMassMessage_HeaderMessage;

            this.DropRecipientClasses.Items.Clear();
            this.DropRecipientClasses.Items.Add(new ListItem(Resources.Global.Global_SelectOne));
            this.DropRecipientClasses.Items.Add(new ListItem("[Regulars]", "1"));
            this.DropRecipientClasses.Items.Add(new ListItem("[Agents]", "2"));
            this.DropRecipientClasses.Items.Add(new ListItem("Officers", "3"));
            this.DropRecipientClasses.Items.Add(new ListItem("Volunteers", "4"));
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
            result.DisplayMessage = String.Format(Resources.Pages.Financial.PayOutMoney_PayoutCreated, payout.Identity,
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
                result.DisplayMessage = String.Format(Resources.Pages.Financial.PayOutMoney_PayoutCannotUndo,
                                                      databaseId);

                return result;
            }

            payout.UndoPayout();

            result.DisplayMessage = HttpUtility.UrlEncode(String.Format(Resources.Pages.Financial.PayOutMoney_PayoutUndone, databaseId)).Replace("+","%20");
            result.Success = true;
            return result;
        }
    }

}