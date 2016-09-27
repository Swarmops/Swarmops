using System;
using System.Collections.Generic;
using Resources.Pages;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_FinancialAccountsTree : DataV5Base
    {
        private Dictionary<int, List<FinancialAccount>> _hashedAccounts;

        protected void Page_Load (object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            FinancialAccountType accountType =
                (FinancialAccountType) (Enum.Parse (typeof (FinancialAccountType), Request.QueryString["AccountType"]));

            string response = string.Empty;

            int excludeId = -1;

            string excludeIdString = Request.QueryString["ExcludeId"];

            if (!String.IsNullOrEmpty (excludeIdString))
            {
                excludeId = Int32.Parse (excludeIdString);
            }

            int resultsAccountId = CurrentOrganization.FinancialAccounts.CostsYearlyResult.Identity;

            // Get accounts

            FinancialAccounts accounts = FinancialAccounts.ForOrganization (CurrentOrganization, accountType);

            // Build tree (there should be a template for this)

            Dictionary<int, List<FinancialAccount>> treeMap = new Dictionary<int, List<FinancialAccount>>();

            foreach (FinancialAccount account in accounts)
            {
                if (account.Identity == excludeId || account.Identity == resultsAccountId)
                {
                    continue;
                }

                if (!treeMap.ContainsKey (account.ParentIdentity))
                {
                    treeMap[account.ParentIdentity] = new List<FinancialAccount>();
                }

                treeMap[account.ParentIdentity].Add (account);
            }

            this._hashedAccounts = treeMap;

            if (accountType == FinancialAccountType.Asset || accountType == FinancialAccountType.Balance ||
                accountType == FinancialAccountType.All)
            {
                response += GetAccountGroup (FinancialAccountType.Asset, Resources.Global.Financial_Asset) +
                            ",";
            }

            if (accountType == FinancialAccountType.Debt || accountType == FinancialAccountType.Balance ||
                accountType == FinancialAccountType.All)
            {
                response += GetAccountGroup (FinancialAccountType.Debt, Resources.Global.Financial_Debt) + ",";
            }

            if (accountType == FinancialAccountType.Income || accountType == FinancialAccountType.Result ||
                accountType == FinancialAccountType.All)
            {
                response +=
                    GetAccountGroup(FinancialAccountType.Income, Resources.Global.Financial_Income) +
                    ",";
            }

            if (accountType == FinancialAccountType.Cost || accountType == FinancialAccountType.Result ||
                accountType == FinancialAccountType.All)
            {
                response += GetAccountGroup(FinancialAccountType.Cost, Resources.Global.Financial_Cost);
            }

            response = response.TrimEnd (',');
            // removes last comma regardless of where it came from or if it's even there

            Response.Output.WriteLine ("[" + response + "]");
            Response.End();
        }

        private string GetAccountGroup (FinancialAccountType type, string localizedGroupName)
        {
            string childrenString = GetAccountsRecurse (type, 0);

            return "{\"id\":\"" + -((int) type) + "\",\"text\":\"" + JsonSanitize (localizedGroupName) +
                   "\",\"state\":\"open\",\"children\":[" + childrenString + "]}";
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
                if (account.Identity == rootNodeId || account.AccountType != accountType)
                {
                    continue;
                }

                string grandChildren = GetAccountsRecurse (accountType, account.Identity);
                if (!string.IsNullOrEmpty (grandChildren))
                {
                    grandChildren = ",\"state\":\"closed\",\"children\":[" + grandChildren + "]";
                }

                childStrings.Add ('{' + String.Format (
                    "\"id\":\"{0}\",\"text\":\"{1}\"",
                    account.Identity,
                    JsonSanitize (account.Name)
                    ) + grandChildren + '}');
            }

            return String.Join (",", childStrings.ToArray());
        }


        private string RecurseTreeMap (Dictionary<int, List<FinancialAccount>> treeMap, int node)
        {
            List<string> elements = new List<string>();

            foreach (FinancialAccount account in treeMap[node])
            {
                string element = string.Format ("\"id\":{0},\"text\":\"{1}\"", account.Identity,
                    JsonSanitize (account.Name));

                if (treeMap.ContainsKey (account.Identity))
                {
                    element += ",\"state\":\"closed\",\"children\":" + RecurseTreeMap (treeMap, account.Identity);
                }

                elements.Add ("{" + element + "}");
            }

            return "[" + String.Join (",", elements.ToArray()) + "]";
        }
    }
}