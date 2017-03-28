using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Frontend;
using Swarmops.Interface.Support;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.LogEntries;
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
                if (!string.IsNullOrEmpty (suppliedTicket) && suppliedTicket.Trim().Length == 21)
                {
                    this.TextTicket.Text = suppliedTicket.Trim().ToUpperInvariant();
                    this.TextTicket.ReadOnly = true;
                    this.TextTicket.Enabled = false;
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
            this.LabelMail.Text = Resources.Pages.Security.ResetPassword_Mail2;
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
                return false; // invalid data or no ticket
            }

            // This may throw on invalid data, which will give a slightly different error but that's fine too for now.
            DateTime ticketExpiresUtc = DateTime.Parse(resetData[0]);
            if (DateTime.UtcNow > ticketExpiresUtc)
            {
                // Ticket expired.
                return false;
            }

            if (ticket != resetData[1])
            {
                // Bad ticket.
                return false;
            }

            // When we get here, all checks to reset the password have completed. Change the password, log the change,
            // notify the user that the password was changed, set a new authentication cookie, and have the web page
            // redirect to Dashboard (by returning true).

            // Clear password reset ticket

            resetPerson.ResetPasswordTicket = string.Empty;

            // Create lockdown code, notify

            string lockdownTicket = SupportFunctions.GenerateSecureRandomKey (16);
            OutboundComm.CreateSecurityNotification (resetPerson, null, null, lockdownTicket,
                NotificationResource.Password_Changed);
            resetPerson.AccountLockdownTicket = DateTime.UtcNow.AddDays (1).ToString (CultureInfo.InvariantCulture) + "," +
                                         lockdownTicket;

            // Set new password

            resetPerson.SetPassword (newPassword);

            // Log the password reset

            SwarmopsLog.CreateEntry (resetPerson,
                new PasswordResetLogEntry (resetPerson, SupportFunctions.GetRemoteIPAddressChain()));

            // Set authentication cookies

            int lastOrgId = resetPerson.LastLogonOrganizationId;

            if (lastOrgId == 0)
            {
                lastOrgId = Organization.SandboxIdentity;
            }

            if (!resetPerson.ParticipatesInOrganizationOrParent(lastOrgId))
            {
                // If the person doesn't have access to the last organization (anymore), log on to Sandbox

                lastOrgId = 1;
            }

            // Set cookies

            FormsAuthentication.SetAuthCookie(Authority.FromLogin (resetPerson).ToEncryptedXml(), true);
            DashboardMessage.Set (Resources.Pages.Security.ResetPassword_Success);

            return true; // temp

            // do NOT NOT NOT trim password - this is deliberate. Passwords starting/ending in whitespace must be possible
        }


        public string Localize_NewPasswordsDontMatch
        {
            get { return CommonV5.JavascriptEscape(Resources.Pages.Security.ResetPassword_NewPasswordsDontMatch); }
        }

        public string Localize_NoEmpty
        {
            get { return CommonV5.JavascriptEscape(Resources.Pages.Security.ResetPassword_NoEmpty); }            
        }

        public string Localize_ResetPasswordFailed
        {
            get { return CommonV5.JavascriptEscape(Resources.Pages.Security.ResetPassword_Failed); }
        }

        
    }
}