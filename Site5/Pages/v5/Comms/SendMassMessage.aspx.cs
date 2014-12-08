using System;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Swarmops.Logic.Structure;
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

            this.DropRecipientClasses.Items.Clear();
            this.DropRecipientClasses.Items.Add (new ListItem (Global.Global_SelectOne, "0"));
            this.DropRecipientClasses.Items.Add (new ListItem ("[Regulars]", "1"));
            this.DropRecipientClasses.Items.Add (new ListItem ("[Agents]", "2"));
            // TODO: Room for dynamic membership types here
            this.DropRecipientClasses.Items.Add (new ListItem ("Officers", "101"));
            this.DropRecipientClasses.Items.Add (new ListItem ("Volunteers", "102"));

            this.ButtonSend.Text = Resources.Pages.Comms.SendMassMessage_SendMessage;
            this.ButtonTest.Text = Resources.Pages.Comms.SendMassMessage_TestMessage;
        }

        [WebMethod]
        public static string GetRecipientCount (int recipientTypeId, int geographyId)
        {
            int personCount = 0;

            AuthenticationData authData = GetAuthenticationDataAndCulture();
            Geography geography = Geography.FromIdentity (geographyId);
            Geographies geoTree = geography.GetTree();
            Organizations orgTree = authData.CurrentOrganization.GetTree();

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

        public struct ConfirmPayoutResult
        {
            public int AssignedId;
            public string DisplayMessage;
        };
    }
}