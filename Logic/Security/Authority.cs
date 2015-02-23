using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Security;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Security
{
    public class Authority : BasicAuthority
    {
        #region Construction and Creation

        private Authority()
            : base (0, null, null, null)
        {
        }

        private Authority (BasicAuthority basic)
            : base (basic.PersonId, basic.SystemPersonRoles, basic.OrganizationPersonRoles, basic.LocalPersonRoles)
        {
        }


        public static Authority FromBasic (BasicAuthority basic)
        {
            return new Authority (basic);
        }

        #endregion

        public Organizations GetOrganizations (RoleType[] forRoles)
        {
            if (HasRoleType (RoleType.SystemAdmin))
            {
                return Organizations.GetAll();
            }

            List<int> orgIdentities = new List<int>();

            foreach (BasicPersonRole role in OrganizationPersonRoles)
            {
                if (Array.IndexOf (forRoles, role.Type) >= 0)
                    orgIdentities.Add (role.OrganizationId);
            }

            foreach (BasicPersonRole role in LocalPersonRoles)
            {
                if (Array.IndexOf (forRoles, role.Type) >= 0)
                    orgIdentities.Add (role.OrganizationId);
            }

            return Organizations.FromIdentities (orgIdentities.ToArray());
        }


        public bool CanSeePerson (Person person)
        {
            People initialList = People.FromArray (new[] {person});

            People filteredList = Authorization.FilterPeopleToMatchAuthority (initialList, this);

            if (filteredList.Count == 0)
            {
                return false;
            }

            return true;
        }


        public Geographies GetGeographiesForOrganization (Organization organization)
        {
            return
                GetGeographiesForOrganization (organization, RoleTypes.AllRoleTypes);
        }

        public Geographies GetGeographiesForOrganization (Organization organization, RoleType[] roleTypes)
        {
            return
                Geographies.FromArray (Authorization.GetNodesInAuthorityForOrganization (this, organization.Identity,
                    roleTypes));
        }


        public bool HasLocalRoleAtOrganizationGeography (Organization organization, Geography geography,
            Authorization.Flag flags)
        {
            return HasLocalRoleAtOrganizationGeography (organization, geography, RoleTypes.AllLocalRoleTypes, flags);
        }


        public bool HasLocalRoleAtOrganizationGeography (Organization organization, Geography geography,
            RoleType roleType, Authorization.Flag flags)
        {
            return HasLocalRoleAtOrganizationGeography (organization, geography,
                new[] {roleType}, flags);
        }


        public bool HasLocalRoleAtOrganizationGeography (Organization organization, Geography geography,
            RoleType[] roleTypes, Authorization.Flag flags)
        {
            if (organization != null && organization.Identity == Organization.SandboxIdentity)
            {
                return true; // UGLY UGLY HACK
            }

            if (organization == null && (flags & Authorization.Flag.AnyOrganization) == 0)
                return false;

            if (geography == null && (flags & Authorization.Flag.AnyGeography) == 0)
                return false;


            foreach (BasicPersonRole role in LocalPersonRoles)
            {
                foreach (RoleType type in roleTypes)
                {
                    if (type == role.Type)
                    {
                        // Expensive op. The org/geo lookups need a cache at the logic layer.

                        bool organizationClear = false;
                        bool geographyClear = false;

                        // First, check if the organization and geography match identically.

                        if ((flags & Authorization.Flag.AnyOrganization) != 0)
                        {
                            // then it is used to check if they have a local role for any org
                            organizationClear = true;
                        }
                        else if (organization != null && organization.Identity == role.OrganizationId)
                        {
                            organizationClear = true;
                        }

                        if ((flags & Authorization.Flag.AnyGeography) != 0)
                        {
                            // then it is used to check if they have a local role anywhere
                            geographyClear = true;
                        }
                        else if (geography != null && geography.Identity == role.GeographyId)
                        {
                            geographyClear = true;
                        }


                        // If not clear, then check if there is inherited authority.

                        if (!organizationClear
                            && (flags & Authorization.Flag.ExactOrganization) == 0)
                        {
                            if (organization != null && organization.Inherits (role.OrganizationId))
                            {
                                organizationClear = true;
                            }
                        }

                        if (!geographyClear && (flags & Authorization.Flag.ExactGeography) == 0)
                        {
                            if (geography != null && geography.Inherits (role.GeographyId))
                            {
                                geographyClear = true;
                            }
                        }

                        // If both are ok, return that there is authority at this org & geo.

                        if (organizationClear && geographyClear)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool HasRoleAtOrganization (Organization organization, Authorization.Flag flags)
        {
            return HasRoleAtOrganization (organization, RoleTypes.AllOrganizationalRoleTypes, flags);
        }

        public bool HasRoleAtOrganization (Organization organization, RoleType roleType, Authorization.Flag flags)
        {
            return HasRoleAtOrganization (organization, new[] {roleType}, flags);
        }

        public bool HasRoleAtOrganization (Organization organization, RoleType[] roleTypes, Authorization.Flag flags)
        {
            if (organization == null)
            {
                if ((flags & Authorization.Flag.AnyOrganization) != 0)
                    return true;
                return false;
            }

            if (organization != null && organization.Identity == Organization.SandboxIdentity)
            {
                return true; // UGLY UGLY HACK
            }

            foreach (BasicPersonRole role in OrganizationPersonRoles)
            {
                foreach (RoleType type in roleTypes)
                {
                    if (type == role.Type)
                    {
                        if ((flags & Authorization.Flag.AnyOrganization) != 0)
                            return true;

                        if (organization.Identity == role.OrganizationId)
                        {
                            return true;
                        }

                        if ((flags & Authorization.Flag.ExactOrganization) == 0)
                        {
                            if (organization.Inherits (role.OrganizationId))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }


        public bool HasRoleType (RoleType roleType)
        {
            return HasAnyRoleType (new[] {roleType});
        }

        public bool HasAnyRoleType (RoleType[] roleTypes)
        {
            foreach (RoleType r in roleTypes)
            {
                foreach (BasicPersonRole r2 in LocalPersonRoles)
                    if (r2.Type == r) return true;
                foreach (BasicPersonRole r2 in OrganizationPersonRoles)
                    if (r2.Type == r) return true;
                foreach (BasicPersonRole r2 in SystemPersonRoles)
                    if (r2.Type == r) return true;
            }

            return false;
        }

        public bool CanEditSystemRoles()
        {
            return HasPermission (Permission.CanEditSystemRoles, -1, -1,
                Authorization.Flag.ExactGeography | Authorization.Flag.ExactOrganization);
        }

        public bool CanEditOrgRolesForOrg (int orgId)
        {
            return HasPermission (Permission.CanEditOrganisationalRoles, orgId, -1, Authorization.Flag.Default);
        }


        public bool HasAnyPermission (Permission perm)
        {
            return Authorization.CheckAuthorization (new PermissionSet (perm), -1, -1, this,
                Authorization.Flag.AnyGeographyAnyOrganization);
        }

        /* -- unused, apparently
        private bool HasPermission (Permission perm, int organizationId, Authorization.Flag flags)
        {
            return Authorization.CheckAuthorization(new PermissionSet(perm), organizationId, Organization.RootIdentity, this, flags);
        } */

        public bool HasPermission (Permission perm, int organizationId, int geographyId, Authorization.Flag flags)
        {
            return Authorization.CheckAuthorization (new PermissionSet (perm, organizationId, geographyId),
                organizationId,
                geographyId, this, flags);
        }

        public bool HasPermission (PermissionSet perm, Authorization.Flag flags)
        {
            return Authorization.CheckAuthorization (perm, -1, -1, this, flags);
        }

        public bool HasPermission (PermissionSet perm, int organizationId, int geographyId, Authorization.Flag flags)
        {
            return Authorization.CheckAuthorization (perm, organizationId, geographyId, this, flags);
        }


        public Organizations OrganisationsWithPermission (Permission perm, RoleType[] roles)
        {
            Organizations orgList = GetOrganizations (roles);
            Organizations retList = new Organizations();
            foreach (Organization org in orgList)
            {
                if (HasPermission (perm, org.Identity, -1, Authorization.Flag.Default))
                    if (!retList.Contains (org))
                        retList.Add (org);
            }
            return retList;
        }
    }
}