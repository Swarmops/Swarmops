using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;
using Swarmops.Common.Enums;
using Swarmops.Common.Generics;
using Swarmops.Database;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Structure
{
    public class Geographies : List<Geography>
    {
        #region Creation and Construction

        public static Geographies FromArray (BasicGeography[] basicArray)
        {
            Geographies result = new Geographies();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicGeography basic in basicArray)
            {
                result.Add (Geography.FromBasic (basic));
            }

            return result;
        }

        public static Geographies FromSingle (Geography geography)
        {
            Geographies result = new Geographies();
            result.Add (geography);
            return result;
        }


        public static Geographies FromIdentities (int[] geoIds)
        {
            return FromArray (GeographyCache.GetGeographies (geoIds));
            //           return FromArray(SwarmDb.GetDatabaseForReading().GetGeographies(geoIds));
        }


        public static Geographies FromLevel (Country country, GeographyLevel level)
        {
            return FromLevel (country.Identity, level);
        }

        public static Geographies FromLevel (int countryId, GeographyLevel level)
        {
            return FromIdentities (SwarmDb.GetDatabaseForReading().GetGeographyIdsFromLevel (countryId, level));
        }

        #endregion

        /*
		public static BasicGeography GetGeography (int geographyId)
		{
			return LogicServices.GetDatabase().GetGeography(geographyId);
		}

		public static BasicGeography[] GetGeographyLine(int leafNodeId)
		{
			return LogicServices.GetDatabase().GetGeographyLine(leafNodeId);
		}

		public static BasicGeography[] GetGeographyTree(int leafNodeId)
		{
			return LogicServices.GetDatabase().GetGeographyTree(leafNodeId);
		}*/


        public int[] Identities
        {
            get { return LogicServices.ObjectsToIdentifiers (ToArray()); }
        }


        /// <summary>
        ///     For every node in the list, pushes the node down in the tree to
        ///     at highest be "filterNode". For example, if World and Sweden was
        ///     passed with Europe as filterNode, Europe and Sweden would be
        ///     returned.
        /// </summary>
        /// <param name="nodes">The list of nodes.</param>
        /// <param name="topGeography">The highest node to return.</param>
        /// <returns>The filtered list.</returns>
        public Geographies FilterAbove (Geography topGeography)
        {
            Geographies result = new Geographies();

            // Build lookup

            Geographies filterLine = topGeography.GetLine();
            Dictionary<int, bool> lineLookup = new Dictionary<int, bool>();
            foreach (Geography node in filterLine)
            {
                lineLookup[node.Identity] = true;
            }

            // Filter

            foreach (Geography node in this)
            {
                if (lineLookup.ContainsKey (node.Identity))
                {
                    // Above cutoff point, add filter node

                    result.Add (topGeography);
                }
                else
                {
                    result.Add (node);
                }
            }

            return result;
        }


        public Geographies RemoveRedundant()
        {
            Dictionary<int, Geography> remaining = new Dictionary<int, Geography>();

            foreach (Geography geo in this)
            {
                remaining[geo.Identity] = geo;
            }

            // O(n^3).

            // For each node, get the node line, and if any other org in collection
            // is a parent of the current org, the take the current org out of the collection.

            foreach (Geography currentNode in this)
            {
                Geographies currentLine = Geography.FromIdentity (currentNode.GeographyId).GetLine();

                foreach (Geography comparedNode in this)
                {
                    if (comparedNode.Identity == currentNode.Identity)
                    {
                        // Do not compare to ourselves
                        continue;
                    }

                    // When comparing to another node, check against all parents of
                    // the current node.

                    for (int index = 0; index < currentLine.Count - 1; index++)
                    {
                        if (currentLine[index].Identity == comparedNode.Identity)
                        {
                            // the compared node is a parent of the current node.
                            // Therefore, the current node is already present in the collection,
                            // as a child of the compared node. It can be safely removed.

                            remaining.Remove (currentNode.GeographyId);
                            break;
                        }
                    }
                }
            }

            // Assemble result

            Geographies result = new Geographies();

            foreach (int nodeId in remaining.Keys)
            {
                result.Add (remaining[nodeId]);
            }

            return result;
        }


        public static Geographies LogicalOr (Geographies set1, Geographies set2)
        {
            // If either set is invalid, return the other
            // (a null is different from an empty set)

            if (set1 == null)
            {
                return set2;
            }

            if (set2 == null)
            {
                return set1;
            }

            // Build table, eliminating duplicates

            Dictionary<int, Geography> table = new Dictionary<int, Geography>();

            foreach (Geography geography in set1)
            {
                table[geography.Identity] = geography;
            }

            foreach (Geography geography in set2)
            {
                table[geography.Identity] = geography;
            }

            // Assemble result

            Geographies result = new Geographies();

            foreach (Geography geography in table.Values)
            {
                result.Add (geography);
            }

            return result;
        }

        public Geographies LogicalOr (Geographies set2)
        {
            return LogicalOr (this, set2);
        }

        public static Dictionary<int, HierSortOrder> GetHierarchicalSortOrder()
        {
            Dictionary<int, HierSortOrder> resultDictionary = new Dictionary<int, HierSortOrder>();
            BuildHierarchicalSortOrder (resultDictionary, Geography.Root, 0);
            return resultDictionary;
        }

        private static void BuildHierarchicalSortOrder (Dictionary<int, HierSortOrder> resultDictionary,
            Geography currentParent, int level)
        {
            if (level > 20)
            {
                throw new Exception ("Detected loop in Geographic data at geography id:" + currentParent.Identity + ", " +
                                     currentParent.Name);
            }

            if (!resultDictionary.ContainsKey (currentParent.Identity))
            {
                resultDictionary.Add (currentParent.Identity, new HierSortOrder (resultDictionary.Count, level));
            }

            foreach (Geography subgeo in currentParent.Children)
                BuildHierarchicalSortOrder (resultDictionary, subgeo, level + 1);
        }

        public class HierSortOrder
        {
            private readonly int level;
            private readonly int order;

            internal HierSortOrder (int pOrder, int pLevel)
            {
                this.order = pOrder;
                this.level = pLevel;
            }

            public int Level
            {
                get { return this.level; }
            }

            public int Order
            {
                get { return this.order; }
            }
        }

        public static Tree<Geography> Tree
        {
            get
            {
                const string cacheKey = "FullGeographyTree";

                object testObject = GuidCache.Get(cacheKey);
                if (testObject == null)
                {
                    Geographies geographies = Geography.Root.GetTree();
                    Tree<Geography> geoTree = Tree<Geography>.FromCollection(geographies);

                    GuidCache.Set(cacheKey, geoTree);
                    return geoTree;
                }
                else
                {
                    return (Tree<Geography>)testObject;
                }
            }
        }
    }
}