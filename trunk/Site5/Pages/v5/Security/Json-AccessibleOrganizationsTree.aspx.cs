using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Pages.Security
{
    public partial class AccessibleOrganizationsTree : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";


            /*
             * ignore cache - reconstruct every time, this is too individual and volatile
             * 
            // Is this stuff in cache already?

            string cacheKey = "ExpensableBudgets-Json-" +
                              this.CurrentOrganization.Identity.ToString((CultureInfo.InvariantCulture));

            string accountsJson =
                (string) Cache[cacheKey];

            if (accountsJson != null)
            {
                Response.Output.WriteLine(accountsJson);
                Response.End();
                return;
            }
            */


            // Not in cache. Construct.

            // Get organizations

            Organizations allOrganizations = Organizations.GetAll();

            // List accessible organizations. That would be those where this person:
            // a) has a role of responsibility;
            // b) is a member or other form of minion;
            // c) has a role of responsibility in a parent organization;
            // d) has a system-wide role that makes it necessary to access orgs.

            // Build tree (there should be a template for this)

            Dictionary<int, Organizations> treeMap = new Dictionary<int, Organizations>();

            foreach (Organization organization in allOrganizations)
            {
                if (!treeMap.ContainsKey(organization.ParentIdentity))
                {
                    treeMap[organization.ParentIdentity] = new Organizations();
                }

                treeMap[organization.ParentIdentity].Add(organization);
            }

            Response.Output.WriteLine(RecurseTreeMap(treeMap, 0));

            /* TODO: Cache, if master installation grows large

            Cache.Insert(cacheKey, accountsJson, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
            // cache lasts for five minutes, no sliding expiration 
            Response.Output.WriteLine(accountsJson); */

            Response.End();
        }

        private string RecurseTreeMap(Dictionary<int, Organizations> treeMap, int nodeId)
        {
            List<string> elements = new List<string>();

            foreach (Organization organization in treeMap[nodeId])
            {
                string element = string.Format("\"id\":{0},\"text\":\"{1}\"", organization.Identity,
                                               JsonSanitize(organization.Name));

                if (treeMap.ContainsKey(organization.Identity))
                {
                    element += ",\"state\":\"closed\",\"children\":" + RecurseTreeMap(treeMap, organization.Identity);
                }

                elements.Add("{" + element + "}");
            }

            return "[" + String.Join(",", elements.ToArray()) + "]";

        }
    }
}