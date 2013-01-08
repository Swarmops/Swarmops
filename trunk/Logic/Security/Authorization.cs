using System.Collections.Generic;
using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Security;
using Swarmops.Database;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Pirates;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Security
{
    public class Authorization
    {
        [Flags]
        public enum Flag
        {
            NotSpecified = 0,
            /// <summary>
            /// Default equals NotSpecified, and means that Inheritance paths will be checked
            /// </summary>
            Default = 0,
            /// <summary>
            /// AnyGeography means that Geograpy of found role is not checked. Use to find PersonRole with permission, regardless of type and geograpy
            /// </summary>
            AnyGeography = 1 << 0,
            /// <summary>
            /// ExactGeography means that Inheritance paths will NOT be checked, only exact hit is found
            /// </summary>
            ExactGeography = 1 << 1,
            /// <summary>
            /// AnyGeography means that Organization of found role is not checked. Use to find PersonRole with permission, regardless of type and organisation
            /// </summary>
            AnyOrganization = 1 << 3,
            /// <summary>
            /// ExactOrganization means that Inheritance paths will NOT be checked, only exact hit is found
            /// </summary>
            ExactOrganization = 1 << 4,
            /// <summary>
            /// Combined for ease of use
            /// </summary>
            AnyGeographyAnyOrganization = AnyGeography | AnyOrganization,
            /// <summary>
            /// Combined for ease of use
            /// </summary>
            AnyGeographyExactOrganization = AnyGeography | ExactOrganization,
            /// <summary>
            /// Combined for ease of use
            /// </summary>
            ExactGeographyAnyOrganization = ExactGeography | AnyOrganization,
            /// <summary>
            /// Combined for ease of use
            /// </summary>
            ExactGeographyExactOrganization = ExactGeography | ExactOrganization
        };

        static Dictionary<Permission, RoleType[]> _PermissonsDict
                = new Dictionary<Permission, RoleType[]>();

        static Dictionary<RoleType, Dictionary<Permission, bool>> _PermissonsPerRoleTypeDict
                = new Dictionary<RoleType, Dictionary<Permission, bool>>();

        private static Dictionary<Permission, RoleType[]> PermissonsDict
        {
            get
            {
                if (flagReload)
                    InitializeStaticData();
                return Authorization._PermissonsDict;
            }
            set { Authorization._PermissonsDict = value; }
        }

        public static bool RoleTypeHasPermission (RoleType r, Permission p)
        {
            if (flagReload)
                InitializeStaticData();
            if (_PermissonsPerRoleTypeDict.ContainsKey(r))
            {
                if (_PermissonsPerRoleTypeDict[r].ContainsKey(p))
                {
                    return _PermissonsPerRoleTypeDict[r][p];
                }
            }
            return false;
        }


        public static RoleType[] RoleTypesWithPermission (Permission p)
        {
            return RoleTypesWithPermission(p, RoleTypes.AllLocalRoleTypes);
        }

        public static RoleType[] RoleTypesWithPermission (Permission p, RoleType[] rt)
        {
            List<RoleType> resList = new List<RoleType>();
            foreach (RoleType r in rt)
            {
                if (RoleTypeHasPermission(r, p))
                {
                    resList.Add(r);
                }
            }
            return resList.ToArray();
        }



        static Authorization ()
        {
            InitializeStaticData();
        }

        static public Boolean flagReload = true;

        static public DateTime lastReload = DateTime.Now;

        static object reloadLock = new object();

        public static void InitializeStaticData ()
        {
            lock (reloadLock)
            {
                if (flagReload)
                {
                    flagReload = false;
                    Dictionary<Permission, List<RoleType>> tempDict
                        = new Dictionary<Permission, List<RoleType>>();
                    _PermissonsPerRoleTypeDict = new Dictionary<RoleType, Dictionary<Permission, bool>>();
                    BasicPermission[] allPermissions = PirateDb.GetDatabaseForReading().GetPermissionsTable();
                    foreach (BasicPermission bp in allPermissions)
                    {
                        RoleType role = bp.RoleType;
                        Permission perm = bp.PermissionType;
                        if (!_PermissonsPerRoleTypeDict.ContainsKey(role))
                        {
                            _PermissonsPerRoleTypeDict[role] = new Dictionary<Permission, bool>();
                        }
                        _PermissonsPerRoleTypeDict[role][perm] = true;

                        if (!tempDict.ContainsKey(perm))
                        {
                            List<RoleType> tmpList = new List<RoleType>();
                            tempDict.Add(perm, tmpList);
                        }
                        tempDict[perm].Add(role);
                    }
                    Dictionary<Permission, RoleType[]> newPermissonsDict
                           = new Dictionary<Permission, RoleType[]>();
                    foreach (Permission p in tempDict.Keys)
                    {
                        RoleType[] roles = tempDict[p].ToArray();
                        newPermissonsDict.Add(p, roles);
                    }
                    PermissonsDict = newPermissonsDict;
                    lastReload = DateTime.Now;
                }
            }
        }


        internal class FoundItException : Exception
        {
        }


        public static Authority GetPersonAuthority (int personId)
        {
            return Authority.FromBasic(PirateDb.GetDatabaseForReading().GetPersonAuthority(Person.FromIdentity(personId)));
        }


        public static BasicGeography[] GetNodesInAuthorityForOrganization (BasicAuthority authority, int organizationId)
        {
            return GetNodesInAuthorityForOrganization(authority, organizationId, RoleTypes.AllRoleTypes);
        }

        public static BasicGeography[] GetNodesInAuthorityForOrganization (BasicAuthority authority, int organizationId, RoleType[] roleTypes)
        {
            Organizations organizationLine = Organization.FromIdentity(organizationId).GetLine();

            // Build lookup tables

            var orgLookup = new Dictionary<int, BasicOrganization>();
            foreach (BasicOrganization organization in organizationLine)
            {
                orgLookup[organization.OrganizationId] = organization;
            }

            var roleLookup = new Dictionary<RoleType, bool>();
            foreach (RoleType roleType in roleTypes)
            {
                roleLookup[roleType] = true;
            }

            // Create list

            var result = new List<BasicGeography>();

            if (authority.AllPersonRoles.Length > 0)
            {
                Dictionary<int, BasicGeography> geoDict = GeographyCache.GetGeographyHashtable(Geography.RootIdentity);

                foreach (BasicPersonRole role in authority.AllPersonRoles)
                {
                    if (orgLookup.ContainsKey(role.OrganizationId))
                    {
                        if (roleLookup.ContainsKey(role.Type))
                        {
                            result.Add(geoDict[role.GeographyId]);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /*
		public static bool ViewerMayWatchPerson (int viewingPersonId, int watchedPersonId)
		{
			Authority authority = GetPersonAuthority(viewingPersonId);

			int[] resultingList = FilterPersonList(new int[] { watchedPersonId }, authority);

			resultingList = FilterUnlistedPeople(resultingList);

			if (resultingList.Length == 0)
			{
				return false;
			}

			return true;
		}*/


        /// <summary>
        /// Checks for authorization for a specific activity.
        /// </summary>
        /// <param name="permissionsNeeded">The permissions to allow or disallow.</param>
        /// <param name="organizationId">The organization the activity is applied to.</param>
        /// <param name="geographyId">The node the activity is applied to, or zero for world.</param>
        /// <param name="checkedPersonId">The person performing the activity (NOT the victim of it).</param>
        /// <returns>True if allowed under current authority.</returns>
        public static bool CheckAuthorization (PermissionSet permissionsNeeded, int organizationId, int geoNodeId, int checkedPersonId, Flag flags)
        {
            return CheckAuthorization(permissionsNeeded, organizationId, geoNodeId, GetPersonAuthority(checkedPersonId), flags);
        }

        /// <summary>
        /// Checks for authorization for a specific activity.
        /// </summary>
        /// <param name="permissionsNeeded">The permissions to allow or disallow.</param>
        /// <param name="organizationId">The organization the activity is applied to.</param>
        /// <param name="geographyId">The node the activity is applied to, or zero for world.</param>
        /// <param name="authority">The authority to test against.</param>
        /// <returns>True if allowed under current authority.</returns>
        public static bool CheckAuthorization (PermissionSet permissionsNeeded, int organizationId, int geoNodeId, Authority authority, Flag flags)
        {
            if (permissionsNeeded == null)
                return false;
            Geography currentGeo = null;
            if (geoNodeId > 0)
            {
                try { currentGeo = Geography.FromIdentity(geoNodeId); }
                catch { }
            }

            Organization currentOrg = null;
            if (organizationId > 0)
            {
                try { currentOrg = Organization.FromIdentity(organizationId); }
                catch { }
            }


            foreach (PermissionSet.Item perm in permissionsNeeded)
            {
                int thisFound = 0;
                Geography innerCurrentGeo = null;
                Organization innerCurrentOrg = null;

                if (perm.geographyId > 0)
                {
                    try { innerCurrentGeo = Geography.FromIdentity(perm.geographyId); }
                    catch { }
                }
                else
                    innerCurrentGeo = currentGeo;

                if (perm.orgId > 0)
                {
                    try { innerCurrentOrg = Organization.FromIdentity(perm.orgId); }
                    catch { }
                }
                else
                    innerCurrentOrg = currentOrg;

                RoleType[] rolesForPerm = new RoleType[] { };
                if (PermissonsDict.ContainsKey(perm.perm))
                    rolesForPerm = PermissonsDict[perm.perm];

                if (perm.perm == Permission.CanSeeSelf)
                    thisFound = 1;
                else if (rolesForPerm.Length > 0)
                    thisFound = CheckSpecificValidRoles(authority, rolesForPerm, innerCurrentGeo, innerCurrentOrg, thisFound, flags);


                if (permissionsNeeded.NeedOne && thisFound == 1)
                    return true;
                if (permissionsNeeded.NeedAll && thisFound == -1)
                    return false;

            }

            //If Need all and not already returned, no false one was found
            if (permissionsNeeded.NeedAll)
                return true;
            else

                return false;
        }

        private static int CheckSpecificValidRoles (Authority authority, RoleType[] roles,
                                                    Geography currentGeo, Organization currentOrg,
                                                    int thisFound, Flag flags)
        {
            if (authority.HasAnyRoleType(roles))
            {
                //Yes, we have one of the roles
                foreach (RoleType r in roles)
                {
                    if (authority.HasRoleType(r) && (Array.IndexOf(RoleTypes.AllLocalRoleTypes, r) > -1))
                    {
                        thisFound = authority.HasLocalRoleAtOrganizationGeography(currentOrg, currentGeo, r, flags) ? 1 : thisFound;
                    }
                    else if (authority.HasRoleType(r) && ((Array.IndexOf(RoleTypes.AllOrganizationalRoleTypes, r) > -1)))
                    {
                        thisFound = authority.HasRoleAtOrganization(currentOrg, r, flags) ? 1 : thisFound;
                    }
                    else if (authority.HasRoleType(r) && ((Array.IndexOf(RoleTypes.AllSystemRoleTypes, r) > -1)))
                    {   //System PersonRole
                        thisFound = 1;
                    }
                    // break if we found one that gives access
                    if (thisFound == 1)
                        return thisFound;
                }
                // if we found one valid it would already have returned; return -1
                thisFound = -1;
            }
            else
                thisFound = -1; //No, we don't have any of the roles


            return thisFound;
        }


        public static Memberships FilterMembershipsToMatchAuthority (Memberships memberships, Geography personGeography,
                                                                     Authority authority)
        {
            // First: If sysadmin, return the whole list uncensored.

            if (IsSystemAdministrator(authority))
            {
                return memberships;
            }

            var clearedMemberships = new Dictionary<int, Membership>();

            //
            foreach (BasicPersonRole role in authority.OrganizationPersonRoles)
            {
                Dictionary<int, BasicOrganization> clearedOrganizations =
                    OrganizationCache.GetOrganizationHashtable(role.OrganizationId);

                foreach (Membership membership in memberships)
                {
                    bool organizationClear = clearedOrganizations.ContainsKey(membership.OrganizationId);

                    if (organizationClear
                        && authority.HasPermission(Permission.CanViewMemberships, membership.OrganizationId, membership.Person.GeographyId, Flag.Default))
                    {
                        clearedMemberships[membership.Identity] = membership;
                    }
                }
            }


            foreach (BasicPersonRole role in authority.LocalPersonRoles)
            {
                Dictionary<int, BasicGeography> clearedGeographies = GeographyCache.GetGeographyHashtable(role.GeographyId);
                Dictionary<int, BasicOrganization> clearedOrganizations =
                    OrganizationCache.GetOrganizationHashtable(role.OrganizationId);

                bool geographyClear = clearedGeographies.ContainsKey(personGeography.Identity);
                geographyClear = geographyClear && authority.HasPermission(Permission.CanViewMemberships, role.OrganizationId, personGeography.Identity, Flag.Default);

                if (geographyClear)
                {
                    foreach (Membership membership in memberships)
                    {
                        bool organizationClear = clearedOrganizations.ContainsKey(membership.OrganizationId);

                        if (organizationClear)
                        {
                            clearedMemberships[membership.Identity] = membership;
                        }
                    }
                }
            }


            // Assemble the array

            var result = new Memberships();

            foreach (Membership membership in clearedMemberships.Values)
            {
                result.Add(membership);
            }

            return result;
        }


        public static People FilterPeopleToMatchAuthority (People people, Authority authority)
        {
            return FilterPeopleToMatchAuthority(people, authority, -1);
            // -1 indicates to respect grace period
        }


        public static People FilterPeopleToMatchAuthority (People people, Authority authority, int gracePeriod)
        {
            // First: If sysadmin, return the whole list uncensored.

            if (IsSystemAdministrator(authority))
            {
                return people;
            }

            PirateDb databaseRead = PirateDb.GetDatabaseForReading();

            if (gracePeriod == -1)
                gracePeriod = Membership.GracePeriod;

            Dictionary<int, List<BasicMembership>> membershipTable = databaseRead.GetMembershipsForPeople(people.Identities, gracePeriod);
            Dictionary<int, int> geographyTable = databaseRead.GetPeopleGeographies(people.Identities);

            var clearedPeople = new Dictionary<int, Person>();


            // Clear by organization roles 
            bool CanSeeNonMembers = authority.HasPermission(Permission.CanSeeNonMembers,Organization.PPSEid,-1,Flag.AnyGeographyExactOrganization);

            foreach (BasicPersonRole role in authority.OrganizationPersonRoles)
            {
                Dictionary<int, BasicOrganization> clearedOrganizations =
                    OrganizationCache.GetOrganizationHashtable(role.OrganizationId);

                foreach (Person person in people)
                {
                    // Is the organization cleared in this officer's role for this to-be-viewed member?

                    if (membershipTable.ContainsKey(person.Identity))
                    {
                        foreach (BasicMembership membership in membershipTable[person.Identity])
                        {
                            if (clearedOrganizations.ContainsKey(membership.OrganizationId)
                                && authority.HasPermission(Permission.CanSeePeople, membership.OrganizationId, person.GeographyId, Flag.Default))
                            {
                                if (membership.Active
                                    || (membership.Expires > DateTime.Now.AddDays(-gracePeriod)
                                        && membership.Expires.AddDays(1) > membership.DateTerminated
                                        && authority.HasPermission(Permission.CanSeeExpiredDuringGracePeriod, membership.OrganizationId, person.GeographyId, Flag.Default)))
                                {
                                    clearedPeople[person.Identity] = person;
                                    break;
                                }
                            }
                        }
                    }
                    else if (CanSeeNonMembers)
                    { //person isn't member anywhere
                        clearedPeople[person.Identity] = person;
                    }
                }
            }


            // Clear by node roles:
            //
            // For each node role, check if each member is in a cleared geography AND a cleared organization.
            // If so, permit view of this member. (A person in a branch of a geographical area for organizations X and Z
            // should see only people of those organizations only on those nodes.)


            foreach (BasicPersonRole role in authority.LocalPersonRoles)
            {
                Dictionary<int, BasicGeography> clearedGeographies = GeographyCache.GetGeographyHashtable(role.GeographyId);
                Dictionary<int, BasicOrganization> clearedOrganizations = OrganizationCache.GetOrganizationHashtable(role.OrganizationId);

                foreach (Person person in people)
                {

                    // Is the node AND the organization cleared in this officer's role for this to-be-viewed member?

                    if (membershipTable.ContainsKey(person.Identity))
                    {
                        foreach (BasicMembership membership in membershipTable[person.Identity])
                        {
                            int organizationClear = 0;
                            int geographyClear = 0;
                            if (clearedOrganizations.ContainsKey(membership.OrganizationId))
                            {
                                organizationClear = membership.OrganizationId;

                                if (clearedGeographies.ContainsKey(geographyTable[person.Identity]))
                                {
                                    geographyClear = geographyTable[person.Identity];
                                }

                                if (organizationClear > 0
                                    && geographyClear > 0
                                    && authority.HasPermission(Permission.CanSeePeople, organizationClear, geographyClear, Flag.Default))
                                {
                                    if (membership.Active
                                        || (membership.Expires > DateTime.Now.AddDays(-gracePeriod)
                                            && membership.Expires.AddDays(1) > membership.DateTerminated
                                            && authority.HasPermission(Permission.CanSeeExpiredDuringGracePeriod, membership.OrganizationId, person.GeographyId, Flag.Default)))
                                    {
                                        clearedPeople[person.Identity] = person;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }



            // End: Assemble an array of the resulting cleared people

            var result = new People();

            foreach (Person clearedPerson in clearedPeople.Values)
            {
                result.Add(clearedPerson);
            }

            return result;
        }


        public static BasicPerson[] FilterUnlistedPeople (BasicPerson[] people)
        {
            var unlistedTable = new Dictionary<int, bool>();
            var result = new List<BasicPerson>();

            // If list is empty, return empty

            if (people == null || people.Length == 0)
            {
                return result.ToArray();
            }

            // Get ALL unlisted people from database. They're not that many. Build a hashtable with them.

            int[] anonymousPersonIds = PirateDb.GetDatabaseForReading().GetObjectsByOptionalData(ObjectType.Person, ObjectOptionalDataType.Anonymous, "1");

            foreach (int anonymousPersonId in anonymousPersonIds)
            {
                unlistedTable[anonymousPersonId] = true;
            }

            // Build list

            foreach (BasicPerson person in people)
            {
                if (!unlistedTable.ContainsKey(person.Identity))
                {
                    result.Add(person);
                }
            }

            return result.ToArray();
        }


        public static BasicPerson[] FilterUniquePeople (BasicPerson[] people)
        {
            // Somewhat crude dupelist, but atleast it can give some hints

            // changed to 3 different lists, should be more efficient // JL
            var uniqueTableName = new Dictionary<string, List<Person>>();
            var uniqueTablePhone = new Dictionary<string, List<Person>>();
            var uniqueTableEmail = new Dictionary<string, List<Person>>();

            var result = new Dictionary<int, BasicPerson>();
            var inputLookup = new Dictionary<int, BasicPerson>();

            // If list is empty, return empty

            if (people == null || people.Length == 0)
            {
                return (new List<BasicPerson>(result.Values)).ToArray();
            }

            foreach (Person p in people)
            {

                if (uniqueTableName.ContainsKey(p.Name))
                {
                    foreach (var sameperson in uniqueTableName[p.Name])
                    {
                        MatchPerson(result, p, sameperson);
                    }
                    uniqueTableName[p.Name].Add(p);
                }
                else
                {
                    uniqueTableName[p.Name] = new List<Person>();
                    uniqueTableName[p.Name].Add(p);
                }

                if (uniqueTablePhone.ContainsKey(p.Phone) && p.Phone.Replace("0", "").Trim() != "")
                {
                    foreach (var sameperson in uniqueTablePhone[p.Phone])
                    {
                        MatchPerson(result, p, sameperson);
                    }
                    uniqueTablePhone[p.Phone].Add(p);
                }
                else
                {
                    uniqueTablePhone[p.Phone] = new List<Person>();
                    uniqueTablePhone[p.Phone].Add(p);
                }

                if (uniqueTableEmail.ContainsKey(p.Email))
                {
                    foreach (var sameperson in uniqueTableEmail[p.Email])
                    {
                        MatchPerson(result, p, sameperson);
                    }
                    uniqueTableEmail[p.Email].Add(p);
                }
                else
                {
                    uniqueTableEmail[p.Email] = new List<Person>();
                    uniqueTableEmail[p.Email].Add(p);
                }
            }

            return (new List<BasicPerson>(result.Values)).ToArray();
        }

        private static void MatchPerson (Dictionary<int, BasicPerson> result, Person p, BasicPerson sameperson)
        {
            var matches = 0;
            if (p.Name.Trim().ToLower() == sameperson.Name.Trim().ToLower())
                ++matches;
            if (p.Phone.Trim() == sameperson.Phone.Trim())
                ++matches;
            if (p.Email.Trim().ToLower() == sameperson.Email.Trim().ToLower())
                ++matches;
            if (matches > 1)
            {
                if (!result.ContainsKey(p.PersonId))
                    result.Add(p.PersonId, p);
                if (!result.ContainsKey(sameperson.PersonId))
                    result.Add(sameperson.PersonId, sameperson);
            }
        }

        private static bool IsSystemAdministrator (BasicAuthority authority)
        {
            foreach (BasicPersonRole role in authority.SystemPersonRoles)
            {
                if (role.Type == RoleType.SystemAdmin)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// List of the roles which make a member visible outwards.
        /// </summary>
        public static RoleType[] VisibleRoles
        {
            get
            {
                return new RoleType[]
                           {
                               RoleType.LocalLead, RoleType.LocalDeputy, RoleType.OrganizationChairman,
                               RoleType.OrganizationSecretary, RoleType.OrganizationTreasurer,
                               RoleType.OrganizationVice1, RoleType.OrganizationVice2,
                               RoleType.OrganizationBoardMember, RoleType.OrganizationBoardDeputy
                           };
            }
        }

        /// <summary>
        /// Like VisibleRoles, but in dictionary form.
        /// </summary>
        public static Dictionary<RoleType, bool> VisibleRolesDictionary
        {
            get
            {
                Dictionary<RoleType, bool> result = new Dictionary<RoleType, bool>();

                RoleType[] roleTypes = VisibleRoles;

                foreach (RoleType roleType in roleTypes)
                {
                    result[roleType] = true;
                }

                return result;
            }
        }

        public static int[] PersonsWithRoleInOrg (RoleType roleType, int p, bool includeChildOrgs)
        {
            Organizations line = new Organizations();
            if (includeChildOrgs)
                line = Organization.FromIdentity(p).GetLine();
            else
                line.Add(Organization.FromIdentity(p));

            BasicPersonRole[] basPersons
                = PirateDb.GetDatabaseForReading().GetPeopleWithRoleType(roleType, line.Identities, new int[] { });

            List<int> retList = new List<int>();

            foreach (BasicPersonRole b in basPersons)
                retList.Add(b.PersonId);

            return retList.ToArray();
        }

    }
}