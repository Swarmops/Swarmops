using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Structure
{
    public class Organizations : List<Organization>
    {
        #region Creation and Construction

        public static Organizations FromSingle (Organization organization)
        {
            Organizations result = new Organizations {organization};

            return result;
        }

        public static Organizations FromArray (Organization[] organizationArray)
        {
            Organizations result = new Organizations {Capacity = (organizationArray.Length*11/10)};

            foreach (Organization organization in organizationArray)
            {
                result.Add (organization);
            }

            return result;
        }

        public static Organizations FromArray (BasicOrganization[] organizationArray)
        {
            Organizations result = new Organizations {Capacity = (organizationArray.Length*11/10)};

            foreach (BasicOrganization basic in organizationArray)
            {
                result.Add (Organization.FromBasic (basic));
            }

            return result;
        }

        public static Organizations GetAll()
        {
            return FromArray (OrganizationCache.GetAll());
        }

        public static Organizations FromIdentities (int[] organizationIds)
        {
            List<BasicOrganization> foundOrgs = new List<BasicOrganization>();
            foreach (int id in organizationIds)
            {
                foundOrgs.Add (Organization.FromIdentity (id));
            }
            return FromArray (foundOrgs.ToArray());
        }

        public static Organizations GetOrganizationsAvailableAtGeography (int geographyId)
        {
            BasicGeography[] nodeLine = GeographyCache.GetGeographyLine (geographyId);
            return FromIdentities (OrganizationCache.GetOrganizationsIdsInGeographies (nodeLine));
        }

        public static Organizations GetAllOrganizationsAvailableAtGeography (int geographyId)
        {
            List<BasicGeography> nodeList = new List<BasicGeography>();
            nodeList.AddRange (GeographyCache.GetGeographyLine (geographyId));
            nodeList.AddRange (GeographyCache.GetGeographyTree (geographyId));

            return FromIdentities (OrganizationCache.GetOrganizationsIdsInGeographies (nodeList.ToArray()));
        }

        #endregion

        #region Manipulation

        public Organizations RemoveRedundant()
        {
            Dictionary<int, Organization> remaining = new Dictionary<int, Organization>();

            foreach (Organization organization in this)
            {
                remaining[organization.OrganizationId] = organization;
            }

            // O(n^3).

            // For each organization, get the organization line, and if any other org in collection
            // is a parent of the current org, the take the current org out of the collection.

            foreach (Organization currentOrg in this)
            {
                Organizations currentLine = currentOrg.GetRootLineage();

                foreach (Organization comparedOrg in this)
                {
                    if (comparedOrg.Identity == currentOrg.Identity)
                    {
                        // Do not compare to ourselves
                        continue;
                    }

                    // When comparing to another organization, check against all parents of
                    // the current organization.

                    for (int index = 0; index < currentLine.Count - 1; index++)
                    {
                        if (currentLine[index].Identity == comparedOrg.Identity)
                        {
                            // the compared organization is a parent of the current organization.
                            // Therefore, the current organization is already present in the collection,
                            // as a child of the compared organization. It can be safely removed.

                            remaining.Remove (currentOrg.Identity);
                            break;
                        }
                    }
                }
            }

            // Assemble result

            Organizations result = new Organizations();

            foreach (int organizationId in remaining.Keys)
            {
                result.Add (remaining[organizationId]);
            }

            return result;
        }

        public Organizations ExpandAll()
        {
            Dictionary<int, Organization> table = new Dictionary<int, Organization>();

            // Build table, eliminating duplicates

            foreach (Organization orgMajor in this)
            {
                Organizations orgTree = orgMajor.ThisAndBelow();

                foreach (Organization orgMinor in orgTree)
                {
                    table[orgMinor.Identity] = orgMinor;
                }
            }

            // Assemble result

            Organizations result = new Organizations();

            foreach (Organization org in table.Values)
            {
                result.Add (org);
            }

            return result;
        }

        #endregion

        #region Logical Operators (And, Or)

        public static Organizations LogicalOr (Organizations set1, Organizations set2)
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

            Dictionary<int, Organization> table = new Dictionary<int, Organization>();

            foreach (Organization org in set1)
            {
                table[org.Identity] = org;
            }

            foreach (Organization org in set2)
            {
                table[org.Identity] = org;
            }

            // Assemble result

            Organizations result = new Organizations();

            foreach (Organization org in table.Values)
            {
                result.Add (org);
            }

            return result;
        }

        public static Organizations LogicalAnd (Organizations set1, Organizations set2)
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

            Dictionary<int, bool> set2Lookup = new Dictionary<int, bool>();

            // Build set2's lookup table

            foreach (Organization org in set2)
            {
                set2Lookup[org.Identity] = true;
            }

            // Build result

            Organizations result = new Organizations();
            foreach (Organization org in set1)
            {
                if (set2Lookup.ContainsKey (org.Identity))
                {
                    result.Add (org);
                }
            }

            return result;
        }

        public Organizations LogicalAnd (Organizations set2)
        {
            return LogicalAnd (this, set2);
        }

        public Organizations LogicalOr (Organizations set2)
        {
            return LogicalOr (this, set2);
        }

        #endregion

        #region Properties and Get Methods

        /// <summary>
        ///     Gets all economy-enabled organizations.
        /// </summary>
        public static Organizations EconomyEnabled
        {
            get
            {
                Organizations result = new Organizations();
                Organizations allOrganizations = GetAll();

                foreach (Organization organization in allOrganizations)
                {
                    if (organization.IsEconomyEnabled)
                    {
                        result.Add (organization);
                    }
                }

                return result;
            }
        }

        public int[] Identities
        {
            get { return LogicServices.ObjectsToIdentifiers (ToArray()); }
        }


        public int GetMembershipCount()
        {
            return Memberships.GetMemberCountForOrganizations (this);
        }

        public int GetMemberCount()
        {
            return SwarmDb.GetDatabaseForReading().GetMemberCountForOrganizations (Identities);
        }

        public int GetMemberCountForGeographies (Geographies geographies)
        {
            return SwarmDb.GetDatabaseForReading().GetMemberCountForOrganizationsAndGeographies (Identities,
                geographies.Identities);
        }

        public int GetRoleHolderCountForGeographies (Geographies geographies)
        {
            Dictionary<int, bool> result = new Dictionary<int, bool>();

            BasicPersonRole[] personRoles =
                SwarmDb.GetDatabaseForReading().GetRolesForOrganizationsGeographies (Identities,
                    geographies.Identities);

            foreach (BasicPersonRole role in personRoles)
            {
                result[role.PersonId] = true;
            }

            return result.Count;
        }


        public People GetRoleHoldersForGeographies (Geographies geographies)
        {
            Dictionary<int, bool> result = new Dictionary<int, bool>();

            BasicPersonRole[] personRoles =
                SwarmDb.GetDatabaseForReading().GetRolesForOrganizationsGeographies (Identities,
                    geographies.Identities);

            foreach (BasicPersonRole role in personRoles)
            {
                result[role.PersonId] = true;
            }

            List<int> list = new List<int>();

            foreach (int key in result.Keys)
            {
                list.Add (key);
            }

            return People.FromIdentities (list.ToArray());
        }

        #endregion

        // --------------------------------------------
        //          WORKSPACE BELOW THIS MARK 
        // --------------------------------------------


        public static Organization GetMostLocalOrganization (int geographyId, int rootOrganizationId)
        {
            // This function returns the most local organization in a tree under "rootOrganizationId".

            // First, find all possible organizations at the particular geography:

            Organizations orgList = GetOrganizationsAvailableAtGeography (geographyId);

            // Second, only note those that inherit from the supplied root:

            Dictionary<int, Organization> table = new Dictionary<int, Organization>();

            foreach (Organization org in orgList)
            {
                if (org.IsOrInherits (rootOrganizationId))
                {
                    table[org.OrganizationId] = org;
                }
            }

            // For each remaining organization, find the one with the longest organization line.
            // This is slightly inefficient as the queries for Geography.GetRootLineage() are repeated.

            if (table.Count == 0)
            {
                // No organizations!

                throw new ArgumentException (
                    string.Format (
                        "No organizations eligible under supplied organization ({0}) for supplied geography({1})",
                        rootOrganizationId, geographyId));
            }

            int maxLength = 0;
            Organization bestFit = null;

            foreach (Organization org in table.Values)
            {
                foreach (Geography uptakeGeo in org.UptakeGeographies)
                {
                    Geographies geoLine = Geography.FromIdentity (org.AnchorGeographyId).GetRootLineage();
                    if (geoLine.Count > maxLength && org.AutoAssignNewMembers)
                    {
                        bestFit = org;
                        maxLength = geoLine.Count;
                    }
                    if (geoLine.Count == maxLength && org.AutoAssignNewMembers && bestFit != null)
                    {
                        if (bestFit.GetRootLineage().Count < org.GetRootLineage().Count)
                        {
                            bestFit = org;
                            maxLength = geoLine.Count;
                        }
                    }
                }
            }

            return bestFit;
        }
    }
}