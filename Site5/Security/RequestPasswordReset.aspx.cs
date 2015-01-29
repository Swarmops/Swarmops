using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Pages.Security
{
    public partial class RequestPasswordReset : DataV5Base // "Data" because we don't have a master page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = Resources.Pages.Security.ResetPassword_PageTitle;
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginTop] = "-3px";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginRight] = "3px";

            if (!Page.IsPostBack)
            {
                Localize();
            }
        }

        private void Localize()
        {
            // Normal template
            this.LabelSidebarInfoContent.Text = Resources.Pages.Security.ResetPassword_Info;
            this.LabelSidebarInfoHeader.Text = Resources.Global.Sidebar_Information;
            this.LabelCurrentOrganizationName.Text = Resources.Global.Global_Organization;
            this.LabelCurrentUserName.Text = Resources.Global.Global_NoOwner;
            this.LabelPreferences.Text = Resources.Global.CurrentUserInfo_Preferences;

            // Page specific
            this.LabelContentTitle.Text = Resources.Pages.Security.ResetPassword_PageTitle;
            this.LabelMail.Text = Resources.Pages.Security.ResetPassword_Email;
            this.LabelSuccessMaybe.Text = Resources.Pages.Security.ResetPassword_TicketSentMaybe;
            this.ButtonRequest.Text = Resources.Pages.Security.ResetPassword_Reset;
        }

        [WebMethod]
        public static bool RequestTicket (string mailAddress)
        {
            mailAddress = mailAddress.Trim();

            if (string.IsNullOrEmpty (mailAddress))
            {
                return false; // this is the only case when we return false: a _syntactically_invalid_ address
            }

            People concernedPeople = People.FromMail (mailAddress); // Should result in exactly 1


            string resetTicket = SupportFunctions.GenerateSecureRandomKey (16); // 16 bytes = 128 bits random key, more than good enough



            Outbound

            // Create log entry

            return true;
        }

    }
}