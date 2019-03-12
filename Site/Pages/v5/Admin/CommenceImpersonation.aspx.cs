using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using Swarmops.Interface.Support;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.LogEntries;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Admin
{
    public partial class CommenceImpersonation : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Participant, AccessType.Write);

            if (!Page.IsPostBack)
            {
                Localize();
            }
        }


        private void Localize()
        {
            this.LabelImpersonationHeader.Text = Resources.Pages.Admin.CommenceImpersonation_Header;
            this.LiteralImpersonationWarning.Text = Resources.Pages.Admin.CommenceImpersonation_Warning;
            this.LabelPerson.Text = Resources.Pages.Admin.CommenceImpersonation_Person;
            this.ButtonImpersonate.Text = Resources.Global.Global_Go;
        }


        [WebMethod]
        public static AjaxCallResult Commence (int personId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (!authData.Authority.HasSystemAccess())
            {
                // Restrict impersonation to system-level access for now: it's a debugging tool

                return new AjaxCallResult
                {
                    Success = false,
                    DisplayMessage = CommonV5.JavascriptEscape(Resources.Pages.Admin.CommenceImpersonation_Failed)
                };
            }

            // BEGIN IMPERSONATION

            Person impersonatedPerson = Person.FromIdentity(personId);

            SwarmopsLogEntry newEntry = SwarmopsLog.CreateEntry(impersonatedPerson,
                new ImpersonationLogEntry {ImpersonatorPersonId = authData.CurrentUser.PersonId, Started = true});
            newEntry.CreateAffectedObject(authData.CurrentUser); // link impersonator to log entry for searchability

            // Someone who has system level access can always impersonate => no further access control at this time

            // SECURITY CONSIDERATIONS: If somebody replaces/fires a superior? Trivially undoable at the database level

            DateTime utcNow = DateTime.UtcNow;
            Authority impersonatingAuthority = Authority.FromLogin(impersonatedPerson, authData.CurrentOrganization);
            impersonatingAuthority.Impersonation = new Impersonation
            {
                ImpersonatedByPersonId = authData.CurrentUser.PersonId,
                ImpersonationStarted = utcNow
            };

            FormsAuthentication.SetAuthCookie(impersonatingAuthority.ToEncryptedXml(), false);
            HttpContext.Current.Response.AppendCookie(new HttpCookie("DashboardMessage", CommonV5.JavascriptEscape(String.Format(Resources.Pages.Admin.CommenceImpersonation_Success, utcNow))));
            return new AjaxCallResult {Success = true};
        }



        // ReSharper disable InconsistentNaming
        public string Localized_ConfirmDialog_Text
        {
            get { return JavascriptEscape(Resources.Pages.Admin.CommenceImpersonation_Confirm); }
        }

        public string Localized_Impersonate
        {
            get { return JavascriptEscape(Resources.Pages.Admin.CommenceImpersonation_Impersonate); }
        }



        /*
        public string Localized_ValidationError_BankAccount
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankAccount); }
        }

        public string Localized_ValidationError_BankClearing
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankClearing); }
        }

        public string Localized_ValidationError_BankName
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankName); }
        }

        public string Localized_ValidationError_Purpose
        {
            get { return JavascriptEscape(Resources.Pages.Financial.FileExpenseClaim_ValidationError_Purpose); }
        }

        public string Localized_ValidationError_Budget
        {
            get { return JavascriptEscape(Resources.Pages.Financial.RequestCashAdvance_ValidationError_Budget); }
        }

        public string Localized_ValidationError_Amount
        {
            get { return JavascriptEscape(String.Format(Resources.Pages.Financial.FileExpenseClaim_ValidationError_Amount, CurrentOrganization.Currency.DisplayCode)); }
        }

        public string Localized_ValidationError_Documents
        {
            get { return JavascriptEscape(Resources.Pages.Financial.FileExpenseClaim_ValidationError_Documents); }
        }*/

    }
}