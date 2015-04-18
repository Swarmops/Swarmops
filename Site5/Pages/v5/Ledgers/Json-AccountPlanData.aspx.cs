using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Resources;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class AccountPlanData : DataV5Base
    {
        private AuthenticationData _authenticationData;
        private Dictionary<int, FinancialAccounts> _hashedAccounts;


        private Dictionary<int, Int64> _singleBalanceLookup;
        private Dictionary<int, Int64> _singleBudgetLookup;
        private Dictionary<int, Int64> _treeBalanceLookup;
        private Dictionary<int, Int64> _treeBudgetLookup;
        private int _year = 2012;
        private int _resultAccountId;

        private string _nearEdge;

        protected void Page_Load (object sender, EventArgs e)
        {
            this._authenticationData = GetAuthenticationDataAndCulture();
            this._year = DateTime.Today.Year;

            if (
                !this._authenticationData.Authority.HasAccess (new Access (
                    this._authenticationData.CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read)))
            {
                throw new UnauthorizedAccessException();
            }

            // TODO: Get and cache account tree and account balances

            Response.ContentType = "application/json";

            _nearEdge = "left";

            if (Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
            {
                _nearEdge = "right";
            }

            string response = string.Empty;

            PopulateLookups (FinancialAccounts.ForOrganization (this._authenticationData.CurrentOrganization));
            this._hashedAccounts = FinancialAccounts.GetHashedAccounts (this._authenticationData.CurrentOrganization);
            this._resultAccountId = this._authenticationData.CurrentOrganization.FinancialAccounts.CostsYearlyResult.Identity;

            response += GetAccountGroup (FinancialAccountType.Asset, Resources.Global.Financial_Asset) + ",";
            response += GetAccountGroup (FinancialAccountType.Debt, Resources.Global.Financial_Debt) + ",";
            response +=
                GetAccountGroup (FinancialAccountType.Income, Resources.Global.Financial_Income) + ",";
            response += GetAccountGroup (FinancialAccountType.Cost, Resources.Global.Financial_Cost);

            Response.Output.WriteLine ("[" + response + "]");
            Response.End();
        }

        private string GetAccountGroup (FinancialAccountType type, string localizedGroupName)
        {
            string childrenString = GetAccountsRecurse (type, 0);

            Int64 projectedResults = _treeBudgetLookup.ContainsKey (_resultAccountId) ? _treeBudgetLookup[_resultAccountId] : 0;
            if (type == FinancialAccountType.Income && projectedResults <= 0) // expected profit or zero
            {
                childrenString += "," + GetProfitLossNode("ProjectedProfit", -projectedResults);
            }
            else if (type == FinancialAccountType.Cost && projectedResults >= 0) // expected loss or zero
            {
                childrenString += "," + GetProfitLossNode("ProjectedLoss", -projectedResults);
            }

            string addString =
                String.Format (
                    "<img class=\\\"IconAdd\\\" accountType=\\\"{0}\\\" src=\\\"/Images/Icons/iconshock-add-16px.png\\\" />",
                    type);

            return "{\"id\":\"" + -((int) type) + "\",\"accountName\":\"<span class=\\\"SpanGroupName\\\">" +
                   JsonSanitize (localizedGroupName) + "</span> (<a class=\\\"LinkAdd\\\" accountType=\\\"" + type +
                   "\\\" href=\\\"#\\\">" + JsonSanitize (Resources.Pages.Ledgers.AccountPlan_AddAccount) +
                   "</a>)\",\"state\":\"open\",\"children\":[" + childrenString + "],\"action\":\"" + addString + "\"}";
        }

        private string GetProfitLossNode(string resource, Int64 amount)
        {
            string node = '{' + string.Format(
                "\"id\":\"{0}\",\"accountName\":\"{1}\",\"rowCssClass\":\"{2}\",\"budget\":\"{3:N0}\"",
                "Row" + resource, string.Format (Resources.Pages.Ledgers.ResourceManager.GetString("AccountPlan_" + resource), _year), "Row" + resource, amount / 100.0) + '}';

            return node;
        }


        private string GetAccountsRecurse (FinancialAccountType accountType, int rootNodeId)
        {
            List<string> childStrings = new List<string>();

            if (!this._hashedAccounts.ContainsKey (rootNodeId))
            {
                return string.Empty;
            }

            if (this._hashedAccounts[rootNodeId].Count == 1)
            {
                // No children

                return string.Empty;
            }

            foreach (FinancialAccount account in this._hashedAccounts[rootNodeId])
            {
                if (account.Identity == rootNodeId || account.AccountType != accountType || account.Identity == _resultAccountId)
                {
                    continue;
                }

                string ownerString = "&nbsp;";
                if (account.AccountType == FinancialAccountType.Income ||
                    account.AccountType == FinancialAccountType.Cost)
                {
                    if (account.OwnerPersonId != 0)
                    {
                        // TODO: add zoom, write capability

                        ownerString =
                            "<span style=\\\"padding-" + _nearEdge + ":20px;background-repeat:no-repeat;background-position:center " + _nearEdge + ";background-image:url('" +
                            account.Owner.GetSecureAvatarLink (16) + "')\\\">" +
                            JsonSanitize (Server.HtmlEncode (account.Owner.Canonical)) + "</span>";
                    }
                    else
                    {
                        // TODO: add write capability

                        ownerString =
                            "<span style=\\\"padding-" + _nearEdge + ":20px;background-repeat:no-repeat;background-position:center " + _nearEdge + ";background-image:url('/Images/Icons/iconshock-warning-16x12px.png')\\\">" +
                            JsonSanitize (Server.HtmlEncode (Global.Global_NoOwner)) + "</span>";
                    }
                }

                string grandChildren = GetAccountsRecurse (accountType, account.Identity);
                if (!string.IsNullOrEmpty (grandChildren))
                {
                    grandChildren = ",\"state\":\"closed\",\"children\":[" + grandChildren + "]";
                }

                string editString =
                    String.Format (
                        "<img class=\\\"IconEdit\\\" accountId=\\\"{0}{1}\\\" src=\\\"/Images/Icons/iconshock-wrench-16px.png\\\" />",
                        accountType.ToString().Substring (0, 1), account.Identity);


                childStrings.Add ('{' +
                                  String.Format (
                                      "\"id\":\"{0}\",\"accountName\":\"{1}\",\"owner\":\"{2}\",\"balance\":\"{3}\",\"budget\":\"{4}\",\"action\":\"{5}\",\"inactive\":\"{6}\"",
                                      account.Identity,
                                      JsonSanitize (account.Name),
                                      ownerString,
                                      this._hashedAccounts[account.Identity].Count > 1
                                          ? JsonDualString (account.Identity, this._treeBalanceLookup[account.Identity],
                                              this._singleBalanceLookup[account.Identity])
                                          : (this._singleBalanceLookup[account.Identity]/100.0).ToString ("N0",
                                              CultureInfo.CurrentCulture),
                                      account.AccountType == FinancialAccountType.Income ||
                                      account.AccountType == FinancialAccountType.Cost
                                          ? this._hashedAccounts[account.Identity].Count > 1
                                              ? (JsonDualString (account.Identity,
                                                  this._treeBudgetLookup[account.Identity],
                                                  this._singleBudgetLookup[account.Identity]))
                                              : (this._singleBudgetLookup[account.Identity]/100.0).ToString ("N0",
                                                  CultureInfo.CurrentCulture)
                                          : string.Empty,
                                      editString,
                                      account.Active? "false":"true" // reverses condition on purpose
                                      ) + grandChildren + '}');
            }

            return String.Join (",", childStrings.ToArray());
        }


        private void PopulateLookups (FinancialAccounts accounts)
        {
            this._singleBalanceLookup = new Dictionary<int, Int64>();
            this._treeBalanceLookup = new Dictionary<int, Int64>();
            this._singleBudgetLookup = new Dictionary<int, Int64>();
            this._treeBudgetLookup = new Dictionary<int, Int64>();

            // 1) Actually, the accounts are already sorted. Or are supposed to be, anyway,
            // since FinancialAccounts.ForOrganization gets the _tree_ rather than the flat list.

            // 2) Add all values to the accounts.

            foreach (FinancialAccount account in accounts)
            {
                // Get current balances

                // TODO: There must be a more optimized way to do this, like with a database optimization. This
                // is a HORRIBLY expensive operation, as it performs one complex database query PER ACCOUNT.

                if (account.AccountType == FinancialAccountType.Cost ||
                    account.AccountType == FinancialAccountType.Income)
                {
                    this._singleBalanceLookup[account.Identity] =
                        -account.GetDeltaCents (new DateTime (this._year, 1, 1),
                            new DateTime (this._year + 1, 1, 1));
                    this._singleBudgetLookup[account.Identity] = account.GetBudgetCents (this._year);
                }
                else if (account.AccountType == FinancialAccountType.Asset ||
                         account.AccountType == FinancialAccountType.Debt)
                {
                    this._singleBalanceLookup[account.Identity] = account.GetDeltaCents (new DateTime (1900, 1, 1),
                        new DateTime (this._year + 1, 1, 1));
                    this._singleBudgetLookup[account.Identity] = 0; // balance accounts don't have budgets
                }
                else
                {
                    throw new InvalidOperationException ("Account with invalid type encountered - " +
                                                         account.AccountType);
                }

                // copy to treeLookups

                this._treeBalanceLookup[account.Identity] = this._singleBalanceLookup[account.Identity];
                this._treeBudgetLookup[account.Identity] = this._singleBudgetLookup[account.Identity];
            }

            // 3) Add all children's values to parents

            AddChildrenValuesToParents (this._treeBalanceLookup, accounts);
            AddChildrenValuesToParents (this._treeBudgetLookup, accounts);

            // Done.
        }


        private void AddChildrenValuesToParents (Dictionary<int, Int64> lookup, FinancialAccounts accounts)
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


        private string JsonDualString (int accountId, Int64 treeValue, Int64 singleValue)
        {
            if (treeValue != 0 && singleValue == 0)
            {
                return string.Format (CultureInfo.CurrentCulture,
                    "<span class=\\\"accountplandata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N0}</span><span class=\\\"accountplandata-expanded-{0}\\\" style=\\\"display:none\\\">&nbsp;</span>",
                    accountId, treeValue/100.00);
            }
            return string.Format (CultureInfo.CurrentCulture,
                "<span class=\\\"accountplandata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N0}</span><span class=\\\"accountplandata-expanded-{0}\\\" style=\\\"display:none\\\">{2:N0}</span>",
                accountId, treeValue/100.0, singleValue/100.0);
        }
    }
}