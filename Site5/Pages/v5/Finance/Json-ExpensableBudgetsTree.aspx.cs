using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

public partial class Pages_v5_Finance_Json_ExpensableBudgetsTree : System.Web.UI.Page
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

        Response.ContentType = "application/json";

        int renderRootNodeId = 0;

        if (treeMap[0].Count == 1)
        {
            // assume there's a master root like "Costs"; bypass it

            renderRootNodeId = treeMap[0][0].Identity;
        }

        Response.Output.WriteLine(RecurseTreeMap (treeMap, renderRootNodeId));

        Response.End();
    }

    private string RecurseTreeMap (Dictionary<int, List<FinancialAccount>> treeMap, int node)
    {
        List<string> elements = new List<string>();

        foreach (FinancialAccount account in treeMap[node])
        {
            string element = string.Format("\"id\":{0},\"text\":\"{1}\"", account.Identity,
                                           account.Name.Replace("\"", "'"));

            if (treeMap.ContainsKey(account.Identity))
            {
                element += ",\"status\":\"closed\",\"children\":" + RecurseTreeMap(treeMap, account.Identity);
            }

            elements.Add("{" + element + "}");
        }

        return "[" + String.Join(",", elements.ToArray()) + "]";

    }
}