using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicPersonRole : IHasIdentity
    {
        private readonly int geographyId;
        private readonly int organizationId;
        private readonly int personId;
        private readonly int roleId;
        private readonly RoleType type;

        #region IHasIdentity Members

        public int Identity
        {
            get { return RoleId; }
        }

        #endregion

        protected BasicPersonRole()
        {
        }

        /// <summary>
        ///     Normal constructor.
        /// </summary>
        /// <param name="personId">The person Id.</param>
        /// <param name="roleType">The node-specific role.</param>
        /// <param name="organizationId">The organization Id.</param>
        /// <param name="geographyId">The node Id.</param>
        public BasicPersonRole (int roleId, int personId, RoleType type, int organizationId, int geographyId)
        {
            this.roleId = roleId;
            this.personId = personId;
            this.type = type;
            this.organizationId = organizationId;
            this.geographyId = geographyId;
        }

        /// <summary>
        ///     Copy constructor.
        /// </summary>
        public BasicPersonRole (BasicPersonRole original)
            : this (original.roleId, original.personId, original.type, original.organizationId, original.geographyId)
        {
        }


        public int RoleId
        {
            get { return this.roleId; }
        }

        public int PersonId
        {
            get { return this.personId; }
        }

        public RoleType Type
        {
            get { return this.type; }
        }

        public int OrganizationId
        {
            get { return this.organizationId; }
        }

        public int GeographyId
        {
            get { return this.geographyId; }
        }
    }
}