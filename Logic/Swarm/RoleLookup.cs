using System.Collections.Generic;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Swarm
{
    public class RoleLookup
    {
        private readonly Dictionary<RoleType, Roles> lookup;

        internal RoleLookup (Roles initialSet)
        {
            this.lookup = new Dictionary<RoleType, Roles>();

            foreach (PersonRole role in initialSet)
            {
                if (!this.lookup.ContainsKey (role.Type))
                {
                    this.lookup[role.Type] = new Roles();
                }

                this.lookup[role.Type].Add (role);
            }
        }

        public Roles this [RoleType roleType]
        {
            get
            {
                if (this.lookup.ContainsKey (roleType))
                {
                    return this.lookup[roleType];
                }

                return new Roles();
            }
        }

        public static RoleLookup FromOrganization (int organizationId)
        {
            Roles roles =
                Roles.FromArray (SwarmDb.GetDatabaseForReading().GetRolesForOrganization (organizationId));

            return new RoleLookup (roles);
        }

        public static RoleLookup FromGeographyAndOrganization (int geographyId, int organizationId)
        {
            Roles roles =
                Roles.FromArray (SwarmDb.GetDatabaseForReading()
                    .GetRolesForOrganizationGeography (organizationId, geographyId));

            return new RoleLookup (roles);
        }

        public static RoleLookup FromGeographyAndOrganization (Geography geography, Organization organization)
        {
            return FromGeographyAndOrganization (geography.Identity, organization.Identity);
        }

        public static RoleLookup FromOrganization (Organization organization)
        {
            return FromOrganization (organization.Identity);
        }
    }
}