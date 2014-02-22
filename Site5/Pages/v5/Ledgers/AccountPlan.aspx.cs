using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class AccountPlan : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.PageIcon = "iconshock-openbook";
            this.PageTitle = Resources.Pages.Ledgers.AccountPlan_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Ledgers.AccountPlan_Info;
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read);
        }


        [WebMethod]
        public static JsonAccountData GetAccountData(int accountId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            FinancialAccount account = FinancialAccount.FromIdentity(accountId);

            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException("A million nopes");
            }

            JsonAccountData result = new JsonAccountData();

            result.AccountName = account.Name;
            result.ParentAccountId = account.ParentIdentity;
            result.Expensable = account.Expensable;
            result.Administrative = false;
            result.Open = account.Open;
            result.AccountOwnerName = account.OwnerPersonId != 0 ? account.Owner.Name : Resources.Global.Global_NoOwner;
            result.AccountOwnerAvatarUrl = account.OwnerPersonId != 0
                                               ? account.Owner.GetSecureAvatarLink(16)
                                               : "/Images/Icons/iconshock-warning-16x12px.png";
            result.Budget = (account.GetTree().GetBudgetSumCents(DateTime.Today.Year)/100L).ToString("N0");

            return result;
        }

        public struct JsonAccountData
        {
            public string AccountName;
            public int ParentAccountId;
            public string ClosedYear;
            public bool Administrative;
            public bool Expensable;
            public bool Open;
            public string AccountOwnerName;
            public string Budget;
            public string AccountOwnerAvatarUrl;
        }

    }
}