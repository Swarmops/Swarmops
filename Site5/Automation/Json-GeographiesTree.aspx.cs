using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Resources;
using Swarmops.Common.Generics;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_GeographiesTree : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            GetAuthenticationDataAndCulture();

            Response.ContentType = "application/json";

            int parentGeographyId = 0;

            bool initialExpand = true;

            if (Request.QueryString["InitialExpand"] == "false")
            {
                initialExpand = false;
            }

            string parentIdString = Request.QueryString["ParentGeographyId"];
            if (!string.IsNullOrEmpty (parentIdString))
            {
                parentGeographyId = Int32.Parse (parentIdString); // will throw if invalid Int32, but who cares
            }

            // Is this stuff in cache already?

            string cacheKey = "Geographies-Json-" + parentGeographyId.ToString (CultureInfo.InvariantCulture) + "-" +
                              Thread.CurrentThread.CurrentCulture.Name;

            string accountsJson = (string) Cache[cacheKey];

            if (accountsJson != null)
            {
                Response.Output.WriteLine (accountsJson);
                Response.End();
                return;
            }

            // Not in cache. Construct.

            _geoTree = Geographies.Tree;
            Countries countries = Countries.GetAll();
            _countryLookup = new Dictionary<int, Country>();

            foreach (Country country in countries)
            {
                if (country.GeographyId > Geography.RootIdentity)
                {
                    _countryLookup[country.GeographyId] = country;
                }
            }

            List<TreeNode<Geography>> rootNodes = _geoTree.RootNodes;

            if (parentGeographyId != 0)
            {
                rootNodes = _geoTree[parentGeographyId].Children;
            }

            accountsJson = RecurseTreeMap (rootNodes, false);

            Cache.Insert (cacheKey, accountsJson, null, DateTime.Now.AddMinutes (15), TimeSpan.Zero);
            // cache lasts for fifteen minutes, no sliding expiration
            Response.Output.WriteLine (accountsJson);

            Response.End();
        }

        private Tree<Geography> _geoTree;
        private Dictionary<int, Country> _countryLookup;

        private string RecurseTreeMap (List<TreeNode<Geography>> geoBranch, bool expanded)
        {
            List<string> elements = new List<string>();

            foreach (TreeNode<Geography> geographyNode in geoBranch)
            {
                Geography geography = geographyNode.Data;

                string element = string.Format ("\"id\":{0},\"text\":\"{1}\"", geography.Identity,
                    JsonSanitize (TestLocalization (geography.Name)));

                if (_countryLookup.ContainsKey (geography.Identity))
                {
                    // this is a country node. Populate with stuff to enable lazy-load of the tree.
                    Country country = _countryLookup[geography.Identity];

                    element += ",\"children1\":\"\",\"state\":\"closed\"," +
                    string.Format ("\"countryId\":\"{0}\",\"countryNode\":\"{1}\"", country.Code, geography.Identity);
                } 
                else if (geographyNode.Children.Count > 0)
                {
                    element += ",\"state\":\"" + (expanded ? "open" : "closed") + "\",\"children\":" +
                               RecurseTreeMap (geographyNode.Children, false);
                }

                elements.Add ("{" + element + "}");
            }

            return "[" + String.Join (",", elements.ToArray()) + "]";
        }

        private string TestLocalization (string name)
        {
            if (name.StartsWith ("[LOC]"))
            {
                return GeographyNames.ResourceManager.GetString (name.Substring (5));
            }
            return name;
        }
    }
}