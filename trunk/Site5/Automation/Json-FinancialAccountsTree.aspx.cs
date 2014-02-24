using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class Json_FinancialAccountsTree : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            FinancialAccountType accountType =
                (FinancialAccountType) (Enum.Parse(typeof (FinancialAccountType), Request.QueryString["AccountType"]));

            string accountsJson = string.Empty;

            int excludeId = -1;

            string excludeIdString = Request.QueryString["ExcludeId"];

            if (!String.IsNullOrEmpty(excludeIdString))
            {
                excludeId = Int32.Parse(excludeIdString);
            }

            // Get accounts

            FinancialAccounts accounts = FinancialAccounts.ForOrganization(this.CurrentOrganization, accountType);

            // Build tree (there should be a template for this)

            Dictionary<int, List<FinancialAccount>> treeMap = new Dictionary<int, List<FinancialAccount>>();

            foreach (FinancialAccount account in accounts)
            {
                if (account.Identity == excludeId)
                {
                    continue;
                }

                if (!treeMap.ContainsKey(account.ParentIdentity))
                {
                    treeMap[account.ParentIdentity] = new List<FinancialAccount>();
                }

                treeMap[account.ParentIdentity].Add(account);
            }

            accountsJson = RecurseTreeMap(treeMap, 0);

            accountsJson = "[{\"id\":\"0\",\"text\":\"" +
                           JsonSanitize(Resources.Global.ResourceManager.GetString("Financial_" + accountType.ToString())) +
                           "\",\"state\":\"open\",\"children\":" +
                           accountsJson +
                           "}]";
                
            Response.Output.WriteLine(accountsJson);

            Response.End();
        }

        private string RecurseTreeMap(Dictionary<int, List<FinancialAccount>> treeMap, int node)
        {
            List<string> elements = new List<string>();

            foreach (FinancialAccount account in treeMap[node])
            {
                string element = string.Format("\"id\":{0},\"text\":\"{1}\"", account.Identity,
                                               JsonSanitize(account.Name));

                if (treeMap.ContainsKey(account.Identity))
                {
                    element += ",\"state\":\"closed\",\"children\":" + RecurseTreeMap(treeMap, account.Identity);
                }

                elements.Add("{" + element + "}");
            }

            return "[" + String.Join(",", elements.ToArray()) + "]";

        }
    }
}