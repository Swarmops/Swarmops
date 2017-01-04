using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Resolution;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.Comms
{
    public partial class SendMassMessage : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageIcon = "iconshock-messages";
            PageTitle = Resources.Pages.Comms.SendMassMessage_Title;
            InfoBoxLiteral = Resources.Pages.Comms.SendMassMessage_Info;

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Correspondence, AccessType.Write);

            if (!Page.IsPostBack)
            {
                Localize();
            }

            this.TextMessage.Style[HtmlTextWriterStyle.Width] = "674px";
            this.TextMessage.Style["resize"] = "none";
        }

        private void Localize()
        {
            this.LabelGeography.Text = Resources.Pages.Comms.SendMassMessage_Geography;
            this.LabelRecipientType.Text = Resources.Pages.Comms.SendMassMessage_RecipientType;
            this.LabelHeaderMessage.Text = Resources.Pages.Comms.SendMassMessage_HeaderMessage;
            this.LabelSubject.Text = Resources.Pages.Comms.SendMassMessage_Subject;

            this.DropRecipientClasses.Items.Clear();
            this.DropRecipientClasses.Items.Add (new ListItem (Participant.Localized(CurrentOrganization.RegularLabel, TitleVariant.Plural), "1"));
            this.DropRecipientClasses.Items.Add (new ListItem (Participant.Localized(CurrentOrganization.ActivistLabel, TitleVariant.Plural), "2"));
            // TODO: Room for dynamic membership types here
            this.DropRecipientClasses.Items.Add (new ListItem (Global.Global_Officer_Plural, "101"));
            this.DropRecipientClasses.Items.Add (new ListItem (Global.Global_Volunteer_Plural, "102"));

            this.ButtonSend.Text = Resources.Pages.Comms.SendMassMessage_SendMessage;
            this.ButtonTest.Text = Resources.Pages.Comms.SendMassMessage_TestMessage;
            this.TextMessage.Attributes["Placeholder"] = Resources.Pages.Comms.SendMassMessage_MessageHint;
        }

        [WebMethod]
        public static string GetRecipientCount (int recipientTypeId, int geographyId)
        {
            int personCount = 0;

            AuthenticationData authData = GetAuthenticationDataAndCulture();
            Geography geography = Geography.FromIdentity (geographyId);
            Geographies geoTree = geography.ThisAndBelow();
            Organizations orgTree = authData.CurrentOrganization.ThisAndBelow();

            switch (recipientTypeId)
            {
                case 0: // "Select one"
                    personCount = 0;
                    break;
                case 1: // Regulars
                    personCount = orgTree.GetMemberCountForGeographies (geoTree);
                    break;
                case 2: // Agents
                    personCount = Activists.GetCountForGeography (geography);
                    break;

                    // TODO: Dynamic membership types

                case 101: // Officers
                    personCount = orgTree.GetRoleHolderCountForGeographies (geoTree);
                    break;
                case 102: // Volunteers
                    personCount = 0; // TODO
                    break;
                default:
                    throw new NotImplementedException();
            }

            string result;
            string[] resources = Resources.Pages.Comms.SendMassMessage_RecipientCount.Split ('|');

            switch (personCount)
            {
                case 0:
                    result = resources[0];
                    break;
                case 1:
                    result = resources[1];
                    break;
                default:
                    result = String.Format (resources[2], personCount);
                    break;
            }

            return result;
        }

        [WebMethod]
        public static AjaxCallResult ExecuteSend(int recipientTypeId, int geographyId, string mode, string subject,
            string body, string dummyMail, bool live)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (PilotInstallationIds.IsPilot(PilotInstallationIds.DevelopmentSandbox) && authData.CurrentUser.Identity == 1 && !live)
            {
                OutboundComm.CreateSandboxMail(subject, body, dummyMail);
                return new AjaxCallResult {Success = true};
            }
            else if (!live)
            {
                // Test mail

                OutboundComm.CreateParticipantMail(subject, body,
                    authData.CurrentUser.ParticipationOf(authData.CurrentOrganization), authData.CurrentUser);

                return new AjaxCallResult {Success = true};
            }
            else // Send live
            {
                // TODO: change resolver to match selected group

                OutboundComm.CreateParticipantMail(subject, body, authData.CurrentUser, authData.CurrentUser, authData.CurrentOrganization, Geography.FromIdentity(geographyId));
                return new AjaxCallResult { Success = true };
            }
        }

        public struct ConfirmPayoutResult
        {
            public int AssignedId;
            public string DisplayMessage;
        };

        public string RunningOnSandbox
        {
            get { return PilotInstallationIds.IsPilot(PilotInstallationIds.DevelopmentSandbox) ? "true" : "false"; }
        }

        // ReSharper disable once InconsistentNaming
        public string Localized_SendMessageResult
        {
            get { return JavascriptEscape (Resources.Pages.Comms.SendMassMessage_SendMessageResult); }
        }

        // ReSharper disable once InconsistentNaming
        public string Localized_TestMessageResult
        {
            get { return JavascriptEscape (Resources.Pages.Comms.SendMassMessage_TestMessageResult); }
        }
    }
}