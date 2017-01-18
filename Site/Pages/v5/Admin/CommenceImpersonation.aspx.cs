using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.Services;
using Swarmops.Interface.Support;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

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
        public static AjaxCallResult BeginImpersonation (int personId)
        {
            throw new NotImplementedException();
        }



        // ReSharper disable InconsistentNaming
        public string Localized_ValidationError_MissingTag
        {
            get { return JavascriptEscape(Resources.Pages.Financial.FileExpenseClaim_ValidationError_MissingTag); }
        }

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
        }

    }
}