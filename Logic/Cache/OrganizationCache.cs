using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;
using Swarmops.Common;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Cache
{
    public class OrganizationCache
    {
        private static DateTime lastRefresh;
        private static object loadCacheLock = new object();
        public static bool __loadCache = true;
        private static Dictionary<int, List<BasicOrganization>> __organizationCache = null;
        private static readonly int cacheLifeSpanMinutes = 2;
        private static Dictionary<int, List<int>> __cachedUptakeGeographies = null;

        static OrganizationCache()
        {
            lastRefresh = Constants.DateTimeLow;
        }

        public static bool needsReload
        {
            get
            {
                lock (loadCacheLock)
                {
                    return (__loadCache || lastRefresh.AddMinutes (cacheLifeSpanMinutes) < DateTime.Now);
                }
            }
            private set
            {
                lock (loadCacheLock)
                {
                    __loadCache = value;
                }
            }
        }

        private static Dictionary<int, List<BasicOrganization>> GetHashedOrganizations()
        {
            if (needsReload)
            {
                lock (loadCacheLock)
                {
                    if (needsReload)
                    {
                        __organizationCache = SwarmDb.GetDatabaseForReading().GetHashedOrganizations();
                        __cachedUptakeGeographies = null;
                        lastRefresh = DateTime.Now;
                        needsReload = false;
                    }
                }
            }
            return __organizationCache;
        }

        public static Organization FromCache (int OrganizationId)
        {
            lock (loadCacheLock)
            {
                return Organization.FromBasic (GetOrganization (OrganizationId));
            }
        }


        internal static int CreateOrganization (int ParentOrganizationId, string NameInternational, string Name,
            string NameShort, string Domain, string MailPrefix,
            int AnchorGeographyId, bool AcceptsMembers,
            bool AutoAssignNewMembers, int DefaultCountryId)
        {
            int Identity = SwarmDb.GetDatabaseForWriting().CreateOrganization (ParentOrganizationId,
                NameInternational,
                Name,
                NameShort,
                Domain,
                MailPrefix,
                AnchorGeographyId,
                AcceptsMembers,
                AutoAssignNewMembers,
                DefaultCountryId);
            needsReload = true;
            return Identity;
        }

        public static void UpdateOrganization (int ParentOrganizationId, string NameInternational, string Name,
            string NameShort, string Domain, string MailPrefix, int AnchorGeographyId, bool AcceptsMembers,
            bool AutoAssignNewMembers, int DefaultCountryId, int OrganizationId)
        {
            SwarmDb.GetDatabaseForWriting()
                .UpdateOrganization (ParentOrganizationId, NameInternational, Name, NameShort, Domain,
                    MailPrefix, AnchorGeographyId, AcceptsMembers, AutoAssignNewMembers, DefaultCountryId,
                    OrganizationId);

            needsReload = true;
        }

        internal static void Reload (int objectId)
        {
            __loadCache = true;

            // Let this be for the moment, new and old parents need to be loaded as well. Better right now to reload the whole cache.
            //lock (loadCacheLock)
            //{
            //    __organizationCache[objectId][0] = Organization.FromBasic(SwarmDb.GetDatabaseForReading().GetOrganization(objectId));
            //}
        }

        public static BasicOrganization[] GetAll()
        {
            lock (loadCacheLock)
            {
                Dictionary<int, List<BasicOrganization>> hashedOrganisations = GetHashedOrganizations();
                List<BasicOrganization> returnList = new List<BasicOrganization>();
                foreach (int id in hashedOrganisations.Keys)
                {
                    returnList.Add (hashedOrganisations[id][0]);
                }
                return returnList.ToArray();
            }
        }

        public static BasicOrganization GetOrganization (int OrganizationId)
        {
            lock (loadCacheLock)
            {
                Dictionary<int, List<BasicOrganization>> hashedOrganisations = GetHashedOrganizations();
                if (hashedOrganisations.ContainsKey (OrganizationId))
                    return hashedOrganisations[OrganizationId][0];
                else
                {
                    //Didn't find, strange, id's should exist, try reloading the cache, it could be newly added.

                    needsReload = true;
                    hashedOrganisations = GetHashedOrganizations();
                    if (hashedOrganisations.ContainsKey (OrganizationId))
                        return hashedOrganisations[OrganizationId][0];
                    else
                    {
                        throw new ArgumentException ("No such OrganizationId: " + OrganizationId.ToString());
                    }
                }
            }
        }

        public static BasicOrganization[] GetOrganizationTree (int startOrganizationId)
        {
            lock (loadCacheLock)
            {
                Dictionary<int, List<BasicOrganization>> organizations = GetHashedOrganizations();

                return GetOrganizationTree (organizations, startOrganizationId, 0);
            }
        }


        public static Dictionary<int, BasicOrganization> GetOrganizationHashtable (int startOrganizationId)
        {
            BasicOrganization[] organizations = GetOrganizationTree (startOrganizationId);

            Dictionary<int, BasicOrganization> result = new Dictionary<int, BasicOrganization>();

            foreach (BasicOrganization organization in organizations)
            {
                result[organization.OrganizationId] = organization;
            }

            return result;
        }


        private static BasicOrganization[] GetOrganizationTree (Dictionary<int, List<BasicOrganization>> organizations,
            int startOrganizationId, int generation)
        {
            List<BasicOrganization> result = new List<BasicOrganization>();

            //Prime the cache
            BasicOrganization start = GetOrganization (startOrganizationId);

            List<BasicOrganization> thisList = organizations[startOrganizationId];

            foreach (BasicOrganization organization in thisList)
            {
                if (organization.OrganizationId != startOrganizationId)
                {
                    result.Add (organization);
                    // new Organization(organization.OrganizationId, organization.ParentOrganizationId, organization.Name, generation + 1));

                    // Add recursively

                    BasicOrganization[] children = GetOrganizationTree (organizations, organization.OrganizationId,
                        generation + 1);

                    if (children.Length > 0)
                    {
                        foreach (BasicOrganization child in children)
                        {
                            result.Add (child);
                        }
                    }
                }
                else if (generation == 0)
                {
                    // The top parent is special and should be added; the others shouldn't

                    result.Add (organization);
                    //  (new Organization(organization.OrganizationId, organization.ParentOrganizationId, organization.Name, generation));
                }
            }

            return result.ToArray();
        }

        public static BasicOrganization[] GetOrganizationChildren (int parentOrgId)
        {
            List<BasicOrganization> result = new List<BasicOrganization>();
            lock (loadCacheLock)
            {
                BasicOrganization parent = GetOrganization (parentOrgId);
                //TODO: It is possible to miss a child here if that child was added after the last cache load.

                Dictionary<int, List<BasicOrganization>> hashedOrganizations = GetHashedOrganizations();
                foreach (BasicOrganization b in hashedOrganizations[parentOrgId])
                {
                    if (b.Identity != parentOrgId)
                        result.Add (b);
                }
                return result.ToArray();
            }
        }

        public static BasicOrganization[] GetOrganizationLine (int leafOrganizationId)
        {
            lock (loadCacheLock)
            {
                List<BasicOrganization> result = new List<BasicOrganization>();

                BasicOrganization currentOrganization = GetOrganization (leafOrganizationId);

                // This iterates until the zero-parentid root Organization is found

                while (currentOrganization.OrganizationId > 0)
                {
                    result.Add (currentOrganization);

                    if (currentOrganization.ParentOrganizationId > 0)
                    {
                        currentOrganization = GetOrganization (currentOrganization.ParentOrganizationId);
                    }
                    else
                    {
                        currentOrganization = new BasicOrganization();
                    }
                }

                result.Reverse();

                return result.ToArray();
            }
        }

        [Obsolete ("Never use this function. Mark the org as unused, deleted. Records are needed for historic reasons.",
            true)]
        internal static void DeleteOrganization (int p)
        {
            SwarmDb.GetDatabaseForWriting().DeleteOrganization (p);
            needsReload = true;
        }

        /*
         * Caching of Uptake Geographies 
        */

        // this is nulled when a main cache load is done, so it don't need a timing parameter really 
        // since that will be governed by the timeout of organisations.

        private static Dictionary<int, List<int>> GetUptakes()
        {
            //to make sure we dont end up loading uptakes, loding organisations, then load uptakes again.
            lock (loadCacheLock)
            {
                GetHashedOrganizations();

                if (__cachedUptakeGeographies == null)
                {
                    __cachedUptakeGeographies = SwarmDb.GetDatabaseForReading().GetAllOrganizationUptakeGeographyIds();
                }
            }
            return __cachedUptakeGeographies;
        }

        internal static int[] GetOrganizationsIdsInGeographies (BasicGeography[] nodes)
        {
            if (nodes == null || nodes.Length == 0)
            {
                return new int[0];
            }

            List<int> nodeIdList = new List<int>();

            foreach (BasicGeography node in nodes)
            {
                nodeIdList.Add (node.GeographyId);
            }

            return GetOrganizationIdsInGeographies (nodeIdList.ToArray());
        }


        public static int[] GetOrganizationIdsInGeographies (int[] nodeIds)
        {
            if (nodeIds == null || nodeIds.Length == 0)
            {
                return new int[0];
            }

            Dictionary<int, bool> resultKeys = new Dictionary<int, bool>();
            Dictionary<int, bool> idsToFind = new Dictionary<int, bool>();
            foreach (int i in nodeIds) idsToFind[i] = true;

            //fist add those that has the geo as anchor
            lock (loadCacheLock)
            {
                Dictionary<int, List<BasicOrganization>> hashedOrgs = GetHashedOrganizations();
                foreach (int id in hashedOrgs.Keys)
                {
                    BasicOrganization bo = hashedOrgs[id][0];
                    if (idsToFind.ContainsKey (bo.AnchorGeographyId))
                    {
                        resultKeys[bo.Identity] = true;
                    }
                }


                //then add those that has the geo as uptake
                Dictionary<int, List<int>> allUptakes = GetUptakes();
                foreach (int orgId in allUptakes.Keys)
                {
                    //connected to valid org?
                    if (hashedOrgs.ContainsKey (orgId))
                    {
                        foreach (int geoId in allUptakes[orgId])
                        {
                            if (idsToFind.ContainsKey (geoId))
                            {
                                resultKeys[orgId] = true;
                            }
                        }
                    }
                }
            }


            return (new List<int> (resultKeys.Keys)).ToArray();
            ;
        }

        internal static void AddOrgUptakeGeography (int p, int geoId)
        {
            SwarmDb.GetDatabaseForWriting().AddOrgUptakeGeography (p, geoId);
            lock (loadCacheLock)
            {
                __cachedUptakeGeographies = null;
            }
        }

        internal static void DeleteOrgUptakeGeography (int p, int geoId)
        {
            SwarmDb.GetDatabaseForWriting().DeleteOrgUptakeGeography (p, geoId);
            lock (loadCacheLock)
            {
                __cachedUptakeGeographies = null;
            }
        }
    }
}