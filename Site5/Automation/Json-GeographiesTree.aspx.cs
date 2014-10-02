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

namespace Swarmops.Frontend.Automation
{
    public partial class Json_GeographiesTree : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            int rootGeographyId = Geography.RootIdentity;

            // What's our root GeographyId? (probably Geography.RootIdentity...)

            string rootIdString = Request.QueryString["RootGeographyId"];
            if (!string.IsNullOrEmpty(rootIdString))
            {
                rootGeographyId = Int32.Parse(rootIdString); // will throw if invalid Int32, but who cares
            }

            // Is this stuff in cache already?

            string cacheKey = "Geographies-Json-" + rootGeographyId.ToString(CultureInfo.InvariantCulture);

            string accountsJson =
                (string) Cache[cacheKey];

            if (accountsJson != null)
            {
                Response.Output.WriteLine(accountsJson);
                Response.End();
                return;
            }

            // Not in cache. Construct.

            Geography rootGeography = Geography.FromIdentity(rootGeographyId);

            // Get geography tree

            Geographies geographies = rootGeography.GetTree();

            // Build tree (there should be a template for this)

            Dictionary<int, List<Geography>> treeMap = new Dictionary<int, List<Geography>>();

            foreach (Geography geography in geographies)
            {
                if (!treeMap.ContainsKey(geography.ParentIdentity))
                {
                    treeMap[geography.ParentIdentity] = new List<Geography>();
                }

                treeMap[geography.ParentIdentity].Add(geography);
            }

            int renderRootNodeId = rootGeography.ParentGeographyId; // This works as rootGeography will be the only present child in the collection; other children won't be there

            accountsJson = RecurseTreeMap(treeMap, renderRootNodeId, true);

            Cache.Insert(cacheKey, accountsJson, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
                // cache lasts for five minutes, no sliding expiration
            Response.Output.WriteLine(accountsJson);

            Response.End();
        }

        private string RecurseTreeMap(Dictionary<int, List<Geography>> treeMap, int node, bool expanded)
        {
            List<string> elements = new List<string>();

            foreach (Geography geography in treeMap[node])
            {
                string element = string.Format("\"id\":{0},\"text\":\"{1}\"", geography.Identity,
                                               JsonSanitize(geography.Name));

                if (treeMap.ContainsKey(geography.Identity))
                {
                    element += ",\"state\":\"" + (expanded? "open":"closed") + "\",\"children\":" + RecurseTreeMap(treeMap, geography.Identity, false);
                }

                elements.Add("{" + element + "}");
            }

            return "[" + String.Join(",", elements.ToArray()) + "]";

        }
    }
}