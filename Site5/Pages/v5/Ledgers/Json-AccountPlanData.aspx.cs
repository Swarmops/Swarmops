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

            PopulateLookups(FinancialAccounts.ForOrganization(_authenticationData.CurrentOrganization));
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

                string ownerString = "&nbsp;";
                if (account.AccountType == FinancialAccountType.Income || account.AccountType == FinancialAccountType.Cost)
                {
                    if (account.OwnerPersonId != 0)
                    {
                        // TODO: add zoom, write capability

                        ownerString =
                            "<span style=\\\"padding-left:20px;background-repeat:no-repeat;background-image:url('" +
                            account.Owner.GetSecureAvatarLink(16) + "')\\\">" +
                            JsonSanitize(Server.HtmlEncode(account.Owner.Canonical)) + "</span>";
                    }
                    else
                    {
                        // TODO: add write capability

                        ownerString =
                            "<span style=\\\"padding-left:20px;background-repeat:no-repeat;background-position:center left;background-image:url('/Images/Icons/iconshock-warning-16x12px.png')\\\">" +
                            JsonSanitize(Server.HtmlEncode(Resources.Global.Global_NoOwner)) + "</span>";
                    }
                }

                string grandChildren = GetAccountsRecurse(accountType, account.Identity);
                if (!string.IsNullOrEmpty(grandChildren))
                {
                    grandChildren = ",\"state\":\"closed\",\"children\":[" + grandChildren + "]";
                }

                string editString = String.Format("<img class=\\\"IconEdit\\\" accountId=\\\"{0}{1}\\\" src=\\\"/Images/Icons/iconshock-wrench-16px.png\\\" />", accountType.ToString().Substring(0,1), account.Identity);


                childStrings.Add('{' + String.Format("\"id\":\"{0}\",\"accountName\":\"{1}\",\"owner\":\"{2}\",\"balance\":\"{3}\",\"budget\":\"{4}\",\"action\":\"{5}\"", 
                    account.Identity, 
                    Server.HtmlEncode(JsonSanitize(account.Name)), 
                    ownerString,
                    _hashedAccounts[account.Identity].Count > 1 ? JsonDualString(account.Identity, _treeBalanceLookup[account.Identity], _singleBalanceLookup[account.Identity]) : (_singleBalanceLookup [account.Identity] / 100.0).ToString("N0", CultureInfo.CurrentCulture),
                    account.AccountType == FinancialAccountType.Income || account.AccountType == FinancialAccountType.Cost?
                        _hashedAccounts[account.Identity].Count > 1 ? (JsonDualString(account.Identity, _treeBudgetLookup[account.Identity], _singleBudgetLookup[account.Identity])) : 
                        (_singleBudgetLookup[account.Identity] / 100.0).ToString("N0", CultureInfo.CurrentCulture) :
                    string.Empty,
                    editString
                 ) + grandChildren + '}');
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

        private Dictionary<int, List<FinancialAccount>> _treeMap;
        private Dictionary<int, Int64> _singleBalanceLookup;
        private Dictionary<int, Int64> _treeBalanceLookup;
        private Dictionary<int, Int64> _singleBudgetLookup;
        private Dictionary<int, Int64> _treeBudgetLookup;


        private void PopulateLookups(FinancialAccounts accounts)
        {
            _singleBalanceLookup = new Dictionary<int, Int64>();
            _treeBalanceLookup = new Dictionary<int, Int64>();
            _singleBudgetLookup = new Dictionary<int, Int64>();
            _treeBudgetLookup = new Dictionary<int, Int64>();

            // 1) Actually, the accounts are already sorted. Or are supposed to be, anyway,
            // since FinancialAccounts.ForOrganization gets the _tree_ rather than the flat list.

            // 2) Add all values to the accounts.

            foreach (FinancialAccount account in accounts)
            {
                // Get current balances

                // TODO: There must be a more optimized way to do this, like with a database optimization. This
                // is a HORRIBLY expensive operation, as it performs one complex database query PER ACCOUNT.

                if (account.AccountType == FinancialAccountType.Cost || account.AccountType == FinancialAccountType.Income)
                {
                    _singleBalanceLookup[account.Identity] = -account.GetDeltaCents(new DateTime(_year, 1, 1),
                                                                                new DateTime(_year + 1, 1, 1));
                    _singleBudgetLookup[account.Identity] = account.GetBudgetCents(_year);
                }
                else if (account.AccountType == FinancialAccountType.Asset || account.AccountType == FinancialAccountType.Debt)
                {
                    _singleBalanceLookup[account.Identity] = account.GetDeltaCents(new DateTime(1900, 1, 1),
                                                                                new DateTime(_year + 1, 1, 1));
                    _singleBudgetLookup[account.Identity] = 0; // balance accounts don't have budgets
                }
                else
                {
                    throw new InvalidOperationException("Account with invalid type encountered - " + account.AccountType.ToString());
                }

                // copy to treeLookups

                _treeBalanceLookup[account.Identity] = _singleBalanceLookup[account.Identity];
                _treeBudgetLookup[account.Identity] = _singleBudgetLookup[account.Identity];
            }

            // 3) Add all children's values to parents

            AddChildrenValuesToParents(_treeBalanceLookup, accounts);
            AddChildrenValuesToParents(_treeBudgetLookup, accounts);

            // Done.
        }


        private void AddChildrenValuesToParents(Dictionary<int, Int64> lookup, FinancialAccounts accounts)
        {
            // Iterate backwards and add any value to its parent's value, as they are sorted in tree order.

            for (int index = accounts.Count - 1; index >= 0; index--)
            {
                int parentFinancialAccountId = accounts[index].ParentFinancialAccountId;
                int accountId = accounts[index].Identity;

                if (parentFinancialAccountId != 0)
                {
                    lookup[parentFinancialAccountId] += lookup[accountId];
                }
            }
        }



        private string JsonDualString(int accountId, Int64 treeValue, Int64 singleValue)
        {
            if (treeValue != 0 && singleValue == 0)
            {
                return string.Format(_renderCulture,
                                     "<span class=\\\"accountplandata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N0}</span><span class=\\\"accountplandata-expanded-{0}\\\" style=\\\"display:none\\\">&nbsp;</span>",
                                     accountId, treeValue / 100.00);
            }
            return string.Format(_renderCulture,
                                 "<span class=\\\"accountplandata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N0}</span><span class=\\\"accountplandata-expanded-{0}\\\" style=\\\"display:none\\\">{2:N0}</span>",
                                 accountId, treeValue / 100.0, singleValue / 100.0);
        }

    }
}