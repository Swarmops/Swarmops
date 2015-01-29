using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Swarm;

namespace Swarmops.Pages.Security
{
    public partial class ResetPassword : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = Resources.Pages.Security.ResetPassword_PageTitle;
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginTop] = "-3px";
            this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginRight] = "3px";

            if (!Page.IsPostBack)
            {
                string suppliedTicket = Request.QueryString["Ticket"];
                if (!string.IsNullOrEmpty (suppliedTicket) && suppliedTicket.Trim().Length == 32)
                {
                    this.TextTicket.Text = suppliedTicket.Trim().ToUpperInvariant();
                    this.TextTicket.ReadOnly = true;
                }

                Localize();
            }

            this.TextMailAddress.Focus();
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
            this.LabelPassword1.Text = Resources.Pages.Security.ResetPassword_Password;
            this.LabelPassword2.Text = Resources.Pages.Security.ResetPassword_PasswordRepeat;
            this.LabelTicket.Text = Resources.Pages.Security.ResetPassword_Ticket;
            this.ButtonRequest.Text = Resources.Pages.Security.ResetPassword_Reset;
        }

        [WebMethod]
        public static bool PerformReset (string mailAddress, string ticket, string newPassword)
        {
            People people = People.FromMail (mailAddress.Trim());
            if (people.Count != 1)
            {
                return false;
            }

            Person resetPerson = people[0];
            string[] resetData = resetPerson.ResetPasswordTicket.Split (',');

            if (resetData.Length != 2)
            {
                return false; // invalid data
            }



            return false; // temp

            // do NOT NOT NOT trim password - this is deliberate. Starting/ending pwds in whitespace must be possible


        }
    }
}