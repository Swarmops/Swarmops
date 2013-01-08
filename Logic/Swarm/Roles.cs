using System;
using System.Collections.Generic;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Swarm
{
    public class Roles : List<PersonRole>
    {
        public static Roles FromArray (BasicPersonRole[] basicArray)
        {
            var result = new Roles();

            result.Capacity = basicArray.Length * 11 / 10;
            foreach (BasicPersonRole basic in basicArray)
            {
                result.Add(PersonRole.FromBasic(basic));
            }

            return result;
        }

        /// <summary>
        /// Gets a list of person IDs that should be notified of something happening in the org.
        /// Defaults to collecting Local roles with Permission.CanSeePeople
        /// </summary>
        /// <param name="organizationId">The organization where something happened.</param>
        /// <param name="geographyId">The geonode where something happened.</param>
        /// <returns>A list of person Ids that hold roles above this point.</returns>
        public static int[] GetAllUpwardRoles (int organizationId, int geographyId)
        {
            return GetAllUpwardRoles(organizationId, geographyId, Authorization.RoleTypesWithPermission(Permission.CanSeePeople));
        }

        /// <summary>
        /// Gets a list of person IDs that should be notified of something happening in the org.
        /// </summary>
        /// <param name="organizationId">The organization where something happened.</param>
        /// <param name="geographyId">The geonode where something happened.</param>
        /// <param name="roleTypes">The RoleTypes which are to be collected.</param>
        /// <returns>A list of person Ids that hold roles above this point.</returns>
        public static int[] GetAllUpwardRoles (int organizationId, int geographyId, RoleType[] roleTypes)
        {
            Organizations orgLine = Organization.FromIdentity(organizationId).GetLine();
            Geographies nodeLine = Geography.FromIdentity(geographyId).GetLine();

            int[] nodeIds = nodeLine.Identities;

            var peopleTable = new Dictionary<int, bool>();

            foreach (Organization org in orgLine)
            {
                BasicPersonRole[] upwardPersonRoles = PirateDb.GetDatabaseForReading().GetRolesForOrganizationGeographies(org.Identity,
                                                                                                    nodeIds);

                foreach (BasicPersonRole role in upwardPersonRoles)
                {
                    
                    if (Array.IndexOf(roleTypes, role.Type) > -1)
                    {
                        peopleTable[role.PersonId] = true;
                    }
                }
            }

            // Assemble result

            var result = new List<int>();

            foreach (int personId in peopleTable.Keys)
            {
                result.Add(personId);
            }

            return result.ToArray();
        }


        /// <summary>
        /// Gets a list of person IDs that are officers in the org/geography combo.
        /// </summary>
        public static int[] GetAllDownwardRoles (int organizationId, int geographyId)
        {
            Organizations orgTree = null;
            Geographies nodeTree = null;
            int[] nodeIds;
            orgTree = Organization.FromIdentity(organizationId).GetTree();
            if (geographyId > 0)
            {
                nodeTree = Geography.FromIdentity(geographyId).GetTree();
                nodeIds = nodeTree.Identities;
            }
            else
            {
                nodeIds = new int[0];
            }

            var peopleTable = new Dictionary<int, bool>();

            foreach (Organization org in orgTree)
            {
                BasicPersonRole[] downwardPersonRoles = PirateDb.GetDatabaseForReading().GetRolesForOrganizationGeographies(org.Identity,
                                                                                                      nodeIds);

                foreach (BasicPersonRole role in downwardPersonRoles)
                {
                    peopleTable[role.PersonId] = true;
                }
            }

            // Assemble result

            var result = new List<int>();

            foreach (int personId in peopleTable.Keys)
            {
                result.Add(personId);
            }

            return result.ToArray();
        }

        public static BasicPersonRole[] GetAllDownwardRolesRoles (int organizationId, int geographyId)
        {
            Organizations orgTree = null;
            Geographies nodeTree = null;

            int[] nodeIds;
            int[] orgIds;

            orgTree = Organization.FromIdentity(organizationId).GetTree();
            orgIds=orgTree.Identities;

            if (geographyId > 0)
            {
                nodeTree = Geography.FromIdentity(geographyId).GetTree();
                nodeIds = nodeTree.Identities;
            }
            else
            {
                nodeIds = new int[0];
            }

            return PirateDb.GetDatabaseForReading().GetRolesForOrganizationsGeographies(orgIds, nodeIds);;
        }


        public static Roles FromOrganization (Organization organization)
        {
            return FromOrganization(organization.Identity);
        }


        public static Roles FromOrganization (int organizationId)
        {
            BasicPersonRole[] basicPersonRoles = PirateDb.GetDatabaseForReading().GetRolesForOrganization (organizationId);

            return FromArray(basicPersonRoles);
        }

        public static Person GetLocalLead (Organization organization, Geography geography)
        {
            return GetLocalLead(organization.Identity, geography.Identity);
        }

        public static Person GetLocalLead(int organizationId, int geographyId)
        {
            BasicPersonRole[] personRoles = PirateDb.GetDatabaseForReading().GetRolesForOrganizationGeography(organizationId, geographyId);

            foreach (BasicPersonRole role in personRoles)
            {
                if (role.Type == RoleType.LocalLead)
                {
                    return Person.FromIdentity(role.PersonId);
                }
            }

            throw new ArgumentException("No Local Lead for org/geo");
        }

        public static Person GetChairman (Organization organization)
        {
            BasicPersonRole[] personRoles = PirateDb.GetDatabaseForReading().GetRolesForOrganization(organization.Identity);

            foreach (BasicPersonRole role in personRoles)
            {
                if (role.Type == RoleType.OrganizationChairman)
                {
                    return Person.FromIdentity(role.PersonId);
                }
            }

            throw new ArgumentException("No Chairman for organization");
        }

        public static People GetLocalDeputies (Organization org, Geography geo)
        {
            return GetLocalDeputies(org.Identity, geo.Identity);
        }

        public static People GetLocalDeputies(int organizationId, int geographyId)
        {
            BasicPersonRole[] personRoles = PirateDb.GetDatabaseForReading().GetRolesForOrganizationGeography(organizationId, geographyId);
            People result = new People();

            foreach (BasicPersonRole role in personRoles)
            {
                if (role.Type == RoleType.LocalDeputy)
                {
                    result.Add(Person.FromIdentity(role.PersonId));
                }
            }

            return result;
        }


        public static People GetActivists (Organization organization, Geography geography)
        {
            var people = new People();

            // Expensive op:
            BasicPersonRole[] basicPersonRoles =
                PirateDb.GetDatabaseForReading().GetRolesForOrganizationsGeographies(organization.GetTree().Identities,
                                                                           geography.GetTree().Identities);

            var lookup = new Dictionary<int, bool>();
            foreach (BasicPersonRole basicRole in basicPersonRoles)
            {
                if (basicRole.Type == RoleType.LocalActive || basicRole.Type == RoleType.LocalLead ||
                    basicRole.Type == RoleType.LocalDeputy)
                {
                    if (!lookup.ContainsKey(basicRole.PersonId))
                    {
                        people.Add(Person.FromIdentity(basicRole.PersonId));
                        lookup[basicRole.PersonId] = true;
                    }
                }
            }

            people.Sort();
            return people;
        }


        public static Roles GetAll()
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetRoles());
        }
    }
}