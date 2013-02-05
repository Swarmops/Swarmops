using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Pages.v5.Finance
{
    public partial class Json_ExpensableBudgetsTree : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Current authentication

            string identity = HttpContext.Current.User.Identity.Name;
            string[] identityTokens = identity.Split(',');

            string userIdentityString = identityTokens[0];
            string organizationIdentityString = identityTokens[1];

            int currentUserId = Convert.ToInt32(userIdentityString);
            int currentOrganizationId = Convert.ToInt32(organizationIdentityString);

            Person currentUser = Person.FromIdentity(currentUserId);
            Authority authority = currentUser.GetAuthority();
            Organization currentOrganization = Organization.FromIdentity(currentOrganizationId);

            Response.ContentType = "application/json";

            // Is this stuff in cache already?

            string cacheKey = "ExpensableAccounts-Json-" +
                              currentOrganizationId.ToString((CultureInfo.InvariantCulture));

            string accountsJson =
                (string) Cache[cacheKey];

            if (accountsJson != null)
            {
                Response.Output.WriteLine(accountsJson);
                Response.End();
                return;
            }

            // Not in cache. Construct.

            // Get accounts

            FinancialAccounts accounts = currentOrganization.FinancialAccounts.ExpensableAccounts;

            // Build tree (there should be a template for this)

            Dictionary<int, List<FinancialAccount>> treeMap = new Dictionary<int, List<FinancialAccount>>();

            foreach (FinancialAccount account in accounts)
            {
                if (!treeMap.ContainsKey(account.ParentIdentity))
                {
                    treeMap[account.ParentIdentity] = new List<FinancialAccount>();
                }

                treeMap[account.ParentIdentity].Add(account);
            }

            int renderRootNodeId = 0;

            if (treeMap[0].Count == 1)
            {
                // assume there's a master root like "Costs"; bypass it

                renderRootNodeId = treeMap[0][0].Identity;
            }

            accountsJson = RecurseTreeMap(treeMap, renderRootNodeId);

            Cache.Insert(cacheKey, accountsJson, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
                // cache lasts for five minutes, no sliding expiration
            Response.Output.WriteLine(accountsJson);

            Response.End();
        }

        private string RecurseTreeMap(Dictionary<int, List<FinancialAccount>> treeMap, int node)
        {
            List<string> elements = new List<string>();

            foreach (FinancialAccount account in treeMap[node])
            {
                string element = string.Format("\"id\":{0},\"text\":\"{1}\"", account.Identity,
                                               account.Name.Replace("\"", "'"));

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