using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class AccountPlanData : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            _authenticationData = GetAuthenticationDataAndCulture();
            _year = DateTime.Today.Year;

            if (!_authenticationData.CurrentUser.HasAccess(new Access(_authenticationData.CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read)))
            {
                throw new UnauthorizedAccessException();
            }

            // TODO: Get and cache account tree and account balances

            Response.ContentType = "application/json";

            string response = string.Empty;

            _hashedAccounts = FinancialAccounts.GetHashedAccounts(_authenticationData.CurrentOrganization);

            response += GetAccountGroup(FinancialAccountType.Asset, Resources.Pages.Ledgers.BalanceSheet_Assets) + ",";
            response += GetAccountGroup(FinancialAccountType.Debt, Resources.Pages.Ledgers.BalanceSheet_Debt) + ",";
            response += GetAccountGroup(FinancialAccountType.Income, Resources.Pages.Ledgers.ProfitLossStatement_Income) + ",";
            response += GetAccountGroup(FinancialAccountType.Cost, Resources.Pages.Ledgers.ProfitLossStatement_Costs);

            Response.Output.WriteLine("[" + response + "]");
            Response.End();
        }

        private int _year = 2012;
        private CultureInfo _renderCulture;
        private AuthenticationData _authenticationData;
        private Dictionary<int, FinancialAccounts> _hashedAccounts; 


        private string GetAccountGroup (FinancialAccountType type, string localizedGroupName)
        {
            string childrenString = GetAccountsRecurse(type, 0);

            return "{\"id\":\"" + -((int) type) + "\",\"accountName\":\"" + JsonSanitize(localizedGroupName) +
                   "\",\"state\":\"open\",\"children\":[" + childrenString + "]}";
        }

        private string GetAccountsRecurse (FinancialAccountType accountType, int rootNodeId)
        {
            List<string> childStrings = new List<string>();

            if (!_hashedAccounts.ContainsKey(rootNodeId))
            {
                return string.Empty;
            }

            if (_hashedAccounts [rootNodeId].Count == 1)
            {
                // No children

                return string.Empty;
            }

            foreach (FinancialAccount account in _hashedAccounts[rootNodeId])
            {
                if (account.Identity == rootNodeId || account.AccountType != accountType)
                {
                    continue;
                }

                string grandChildren = GetAccountsRecurse(accountType, account.Identity);
                if (!string.IsNullOrEmpty(grandChildren))
                {
                    grandChildren = ",\"state\":\"closed\",\"children\":[" + grandChildren + "]";
                }


                childStrings.Add('{' + String.Format("\"id\":\"{0}\",\"accountName\":\"{1}\"", account.Identity, JsonSanitize(account.Name)) + grandChildren + '}');
            }

            return String.Join(",", childStrings.ToArray());
        }

        private class DisplayedAccount
        {
            public string Name;
            public int Identity;
        };

        private class DisplayedAccounts : List<DisplayedAccount>
        {
            // typedef
        };
    }
}