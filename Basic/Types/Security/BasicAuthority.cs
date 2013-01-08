using System;
using System.Collections.Generic;
using System.Text;

namespace Swarmops.Basic.Types.Security
{
    /// <summary>
    /// Container of a person's authority within PirateWeb.
    /// </summary>
    public class BasicAuthority
    {
        public BasicAuthority (int personId, BasicPersonRole[] systemPersonRoles, BasicPersonRole[] organizationPersonRoles,
                               BasicPersonRole[] nodePersonRoles)
        {
            if (personId < 1)
            {
                throw new ArgumentOutOfRangeException("PersonId cannot be " + personId.ToString());
            }

            this.PersonId = personId;
            this.SystemPersonRoles = systemPersonRoles;
            this.OrganizationPersonRoles = organizationPersonRoles;
            this.LocalPersonRoles = nodePersonRoles;

            if (this.SystemPersonRoles == null)
            {
                this.SystemPersonRoles = new BasicPersonRole[0];
            }

            if (this.OrganizationPersonRoles == null)
            {
                this.OrganizationPersonRoles = new BasicPersonRole[0];
            }

            if (this.LocalPersonRoles == null)
            {
                this.LocalPersonRoles = new BasicPersonRole[0];
            }

            AllPersonRoles = new BasicPersonRole[SystemPersonRoles.Length + OrganizationPersonRoles.Length + LocalPersonRoles.Length];

            if (SystemPersonRoles.Length > 0)
                SystemPersonRoles.CopyTo(AllPersonRoles, 0);

            if (OrganizationPersonRoles.Length > 0)
                OrganizationPersonRoles.CopyTo(AllPersonRoles, SystemPersonRoles.Length);

            if (LocalPersonRoles.Length > 0)
                LocalPersonRoles.CopyTo(AllPersonRoles, SystemPersonRoles.Length + OrganizationPersonRoles.Length);

        }

        public readonly int PersonId;
        public readonly BasicPersonRole[] SystemPersonRoles;
        public readonly BasicPersonRole[] OrganizationPersonRoles;
        public readonly BasicPersonRole[] LocalPersonRoles;
        public readonly BasicPersonRole[] AllPersonRoles;
    }
}