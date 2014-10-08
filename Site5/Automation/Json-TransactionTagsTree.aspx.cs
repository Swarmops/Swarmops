using System;
using System.Collections.Generic;
using System.Globalization;
using Swarmops.Logic.Financial;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class Json_TransactionTagsTree : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            int tagSetId = Int32.Parse(Request.QueryString["TagSetId"]);

            FinancialTransactionTagSet tagSet = FinancialTransactionTagSet.FromIdentity(tagSetId);

            if (tagSet.OrganizationId != this.CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException();
            }

            // Is this stuff in cache already?

            string cacheKey = "FinancialTransactionTagTypes-Json-" +
                              tagSetId.ToString((CultureInfo.InvariantCulture));

            string tagsJson = 
                (string) Cache[cacheKey];

            if (tagsJson != null)
            {
                Response.Output.WriteLine(tagsJson);
                Response.End();
                return;
            }

            // Not in cache. Construct.

            // Get accounts

            FinancialTransactionTagTypes tagTypes = FinancialTransactionTagTypes.ForSet(tagSet);

            // Build tree (there should be a template for this)

            Dictionary<int, List<FinancialTransactionTagType>> treeMap = new Dictionary<int, List<FinancialTransactionTagType>>();

            foreach (FinancialTransactionTagType tagType in tagTypes)
            {
                if (!treeMap.ContainsKey(tagType.ParentIdentity))
                {
                    treeMap[tagType.ParentIdentity] = new List<FinancialTransactionTagType>();
                }

                treeMap[tagType.ParentIdentity].Add(tagType);
            }

            int renderRootNodeId = 0;

            if (treeMap[0].Count == 1 && treeMap.ContainsKey(treeMap[0][0].Identity))
            {
                // assume there's a master root like "Costs"; bypass it

                renderRootNodeId = treeMap[0][0].Identity;
            }

            tagsJson = RecurseTreeMap(treeMap, renderRootNodeId);

            Cache.Insert(cacheKey, tagsJson, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
                // cache lasts for five minutes, no sliding expiration
            Response.Output.WriteLine(tagsJson);

            Response.End();
        }

        private string RecurseTreeMap(Dictionary<int, List<FinancialTransactionTagType>> treeMap, int node)
        {
            List<string> elements = new List<string>();

            foreach (FinancialTransactionTagType tagType in treeMap[node])
            {
                string element = string.Format("\"id\":{0},\"text\":\"{1}\"", tagType.Identity,
                                               JsonSanitize(tagType.Name));

                if (treeMap.ContainsKey(tagType.Identity))
                {
                    element += ",\"state\":\"closed\",\"children\":" + RecurseTreeMap(treeMap, tagType.Identity);
                }

                elements.Add("{" + element + "}");
            }

            return "[" + String.Join(",", elements.ToArray()) + "]";

        }

    }
}