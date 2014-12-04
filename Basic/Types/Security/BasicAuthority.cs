using System;

namespace Swarmops.Basic.Types.Security
{
    /// <summary>
    ///     Container of a person's authority within PirateWeb.
    /// </summary>
    public class BasicAuthority
    {
        public readonly BasicPersonRole[] AllPersonRoles;
        public readonly BasicPersonRole[] LocalPersonRoles;
        public readonly BasicPersonRole[] OrganizationPersonRoles;
        public readonly int PersonId;
        public readonly BasicPersonRole[] SystemPersonRoles;

        public BasicAuthority(int personId, BasicPersonRole[] systemPersonRoles,
            BasicPersonRole[] organizationPersonRoles,
            BasicPersonRole[] nodePersonRoles)
        {
            if (personId < 1)
            {
                throw new ArgumentOutOfRangeException("PersonId cannot be " + personId);
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

            this.AllPersonRoles =
                new BasicPersonRole[
                    this.SystemPersonRoles.Length + this.OrganizationPersonRoles.Length + this.LocalPersonRoles.Length];

            if (this.SystemPersonRoles.Length > 0)
                this.SystemPersonRoles.CopyTo(this.AllPersonRoles, 0);

            if (this.OrganizationPersonRoles.Length > 0)
                this.OrganizationPersonRoles.CopyTo(this.AllPersonRoles, this.SystemPersonRoles.Length);

            if (this.LocalPersonRoles.Length > 0)
                this.LocalPersonRoles.CopyTo(this.AllPersonRoles,
                    this.SystemPersonRoles.Length + this.OrganizationPersonRoles.Length);
        }
    }
}