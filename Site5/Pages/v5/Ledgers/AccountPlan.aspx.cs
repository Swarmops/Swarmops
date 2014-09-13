using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Enums;
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
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Write);
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

            FinancialAccounts accountTree = account.GetTree();
            int year = DateTime.Today.Year;

            JsonAccountData result = new JsonAccountData();

            result.AccountName = account.Name;
            result.ParentAccountId = account.ParentIdentity;
            result.ParentAccountName = account.ParentFinancialAccountId == 0
                                           ? Resources.Global.ResourceManager.GetString("Financial_" +
                                                                                        account.AccountType.ToString())
                                           : account.Parent.Name;
            result.Expensable = account.Expensable;
            result.Administrative = account.Administrative;
            result.Open = account.Open;
            result.AccountOwnerName = account.OwnerPersonId != 0 ? account.Owner.Name : Resources.Global.Global_NoOwner;
            result.AccountOwnerAvatarUrl = account.OwnerPersonId != 0
                                               ? account.Owner.GetSecureAvatarLink(24)
                                               : "/Images/Icons/iconshock-warning-24px.png";
            result.Budget = (accountTree.GetBudgetSumCents(year)/100L).ToString("N0");

            if (account.AccountType == FinancialAccountType.Asset || account.AccountType == FinancialAccountType.Debt)
            {
                result.Balance = (accountTree.GetDeltaCents(new DateTime(1900, 1, 1), new DateTime(year + 1, 1, 1))/100L).ToString("N0");
            }
            else
            {
                result.Balance = (-accountTree.GetDeltaCents(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1)) / 100L).ToString("N0");
            }

            return result;
        }

        [WebMethod]
        public static bool SetAccountName(int accountId, string name)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            FinancialAccount account = FinancialAccount.FromIdentity(accountId);

            if (account.OrganizationId != authData.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException("A million nopes");
            }

            // TODO - CRITICAL: CHECK ACCOUNTOPENEDYEAR

            account.Name = HttpContext.Current.Server.UrlDecode(name);
            return true;
        }

        public struct JsonAccountData
        {
            public string AccountName;
            public int ParentAccountId;
            public string ParentAccountName;
            public string ClosedYear;
            public bool Administrative;
            public bool Expensable;
            public bool Open;
            public string AccountOwnerName;
            public string Budget;
            public string AccountOwnerAvatarUrl;
            public string Balance;
        }

    }
}