using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Cache
{
    public class GeographyCache
    {
        private static DateTime lastRefresh;
        private static object loadCacheLock = new object();
        public static bool loadCache = true;
        private static Dictionary<int, List<BasicGeography>> __geographyCache = null;
        private static readonly int cacheLifeSpanMinutes = 2;

        static GeographyCache()
        {
            lastRefresh = DateTime.MinValue;
        }

        private static Dictionary<int, List<BasicGeography>> GetHashedGeographies()
        {
            lock (loadCacheLock)
            {
                if (loadCache || lastRefresh.AddMinutes (cacheLifeSpanMinutes) < DateTime.Now)
                {
                    __geographyCache = SwarmDb.GetDatabaseForReading().GetHashedGeographies();
                    lastRefresh = DateTime.Now;
                    loadCache = false;
                }
                return __geographyCache;
            }
        }


        public static Geography FromCache (int geographyId)
        {
            lock (loadCacheLock)
            {
                return Geography.FromBasic (GetGeography (geographyId));
            }
        }

        public static BasicGeography[] GetAllGeographies()
        {
            lock (loadCacheLock)
            {
                List<BasicGeography> result = new List<BasicGeography>();
                Dictionary<int, List<BasicGeography>> hashedGeographies = GetHashedGeographies();
                foreach (int entry in hashedGeographies.Keys)
                {
                    result.Add (hashedGeographies[entry][0]);
                }

                return result.ToArray();
            }
        }


        public static BasicGeography[] GetGeographies (int[] ids)
        {
            List<BasicGeography> result = new List<BasicGeography>();
            lock (loadCacheLock)
            {
                foreach (int i in ids)
                {
                    result.Add (GetGeography (i));
                }
                return result.ToArray();
            }
        }


        public static BasicGeography[] GetGeographyChildren (int parentGeographyId)
        {
            List<BasicGeography> result = new List<BasicGeography>();
            lock (loadCacheLock)
            {
                // Prime the cache
                BasicGeography partent = GetGeography (parentGeographyId);

                //TODO: Possible to miss a child geography here if it was added since last cache reload.

                Dictionary<int, List<BasicGeography>> hashedGeographies = GetHashedGeographies();
                foreach (BasicGeography b in hashedGeographies[parentGeographyId])
                {
                    if (b.Identity != parentGeographyId)
                        result.Add (b);
                }
                return result.ToArray();
            }
        }


        public static int CountGeographyChildren (int parentGeographyId)
        {
            lock (loadCacheLock)
            {
                // Prime the cache
                BasicGeography partent = GetGeography (parentGeographyId);

                return GetHashedGeographies()[parentGeographyId].Count - 1;
            }
        }


        public static BasicGeography[] GetGeographyLine (int leafGeographyId)
        {
            List<BasicGeography> result = new List<BasicGeography>();


            BasicGeography currentNode = GetGeography (leafGeographyId);

            // This iterates until the zero-parentid root node is found

            while (currentNode.GeographyId != 0)
            {
                result.Add (currentNode);

                if (currentNode.ParentGeographyId != 0)
                {
                    currentNode = GetGeography (currentNode.ParentGeographyId);
                }
                else
                {
                    currentNode = new BasicGeography (0, 0, string.Empty);
                }
            }

            result.Reverse();

            return result.ToArray();
        }


        public static BasicGeography[] GetGeographyTree()
        {
            return GetGeographyTree (1);
        }


        public static Dictionary<int, BasicGeography> GetGeographyHashtable (int startGeographyId)
        {
            BasicGeography[] nodes = GetGeographyTree (startGeographyId);

            Dictionary<int, BasicGeography> result = new Dictionary<int, BasicGeography>();

            foreach (BasicGeography node in nodes)
            {
                result[node.GeographyId] = node;
            }

            return result;
        }

        public static BasicGeography[] GetGeographyTree (int startGeographyId)
        {
            Dictionary<int, List<BasicGeography>> nodes = GetHashedGeographies();

            return GetGeographyTree (nodes, startGeographyId, 0);
        }


        private static BasicGeography[] GetGeographyTree (Dictionary<int, List<BasicGeography>> geographies,
            int startNodeId,
            int generation)
        {
            List<BasicGeography> result = new List<BasicGeography>();

            List<BasicGeography> thisList = geographies[startNodeId];

            foreach (BasicGeography node in thisList)
            {
                if (node.GeographyId != startNodeId)
                {
                    result.Add (new BasicGeography (node.GeographyId, node.ParentGeographyId, node.Name, generation + 1));

                    // Add recursively

                    BasicGeography[] children = GetGeographyTree (geographies, node.GeographyId, generation + 1);

                    if (children.Length > 0)
                    {
                        foreach (BasicGeography child in children)
                        {
                            result.Add (child);
                        }
                    }
                }
                else if (generation == 0)
                {
                    // The top parent is special and should be added; the others shouldn't

                    result.Add (new BasicGeography (node.GeographyId, node.ParentGeographyId, node.Name, generation));
                }
            }

            return result.ToArray();
        }


        public static BasicGeography GetGeography (int geographyId)
        {
            lock (loadCacheLock)
            {
                Dictionary<int, List<BasicGeography>> hashedGeographies = GetHashedGeographies();
                if (hashedGeographies.ContainsKey (geographyId))
                    return hashedGeographies[geographyId][0];
                else
                {
                    loadCache = true;
                    hashedGeographies = GetHashedGeographies();
                    if (hashedGeographies.ContainsKey (geographyId))
                        return hashedGeographies[geographyId][0];
                    else
                    {
                        throw new ArgumentException ("No such GeographyId: " + geographyId.ToString());
                    }
                }
            }
        }


        public static BasicGeography GetGeographyByName (string geographyName)
        {
            string cmpName = geographyName.ToLower().Trim();
            Dictionary<int, List<BasicGeography>> hashedGeographies = GetHashedGeographies();
            foreach (int entry in hashedGeographies.Keys)
            {
                if (hashedGeographies[entry][0].Name.ToLower().Trim() == cmpName)
                    return hashedGeographies[entry][0];
            }
            //TODO: This can miss due to that the geography was added after last cache reload
            throw new ArgumentException ("No such GeographyName: " + geographyName);
        }


        public static void SetGeographyName (int geographyId, string name)
        {
            SwarmDb.GetDatabaseForWriting().SetGeographyName (geographyId, name);
            BasicGeography geo = SwarmDb.GetDatabaseForReading().GetGeography (geographyId);
            Dictionary<int, List<BasicGeography>> hashedGeographies = GetHashedGeographies();
            hashedGeographies[geographyId][0] = geo;
        }

        /**********************
         * Might be handled....
                public GeographyLevel[] GetGeographyLevelsAtGeographyId (int geographyId)
                {
                    return SwarmDb.GetDatabaseForReading().GetGeographyLevelsAtGeographyId(geographyId);
                }

                public int[] GetGeographyIdsFromLevel (int countryId, GeographyLevel level)
                {
                    return SwarmDb.GetDatabaseForReading().GetGeographyIdsFromLevel(countryId, level);
                }
        ************************/
    }
}