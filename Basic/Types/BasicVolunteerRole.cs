using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicVolunteerRole : IHasIdentity
    {
        private readonly bool assigned;
        private readonly int geographyId;
        private readonly bool open;
        private readonly int organizationId;
        private readonly RoleType roleType;
        private readonly int volunteerId;
        private readonly int volunteerRoleId;

        public BasicVolunteerRole(int volunteerRoleId, int volunteerId, int organizationId, int geographyId,
            RoleType roleType, bool open, bool assigned)
        {
            this.volunteerRoleId = volunteerRoleId;
            this.volunteerId = volunteerId;
            this.organizationId = organizationId;
            this.geographyId = geographyId;
            this.roleType = roleType;
            this.open = open;
            this.assigned = assigned;
        }

        public BasicVolunteerRole(BasicVolunteerRole original)
            : this(original.volunteerRoleId, original.volunteerId, original.organizationId, original.geographyId,
                original.roleType, original.open, original.assigned)
        {
        }

        public int VolunteerRoleId
        {
            get { return this.volunteerRoleId; }
        }

        public int VolunteerId
        {
            get { return this.volunteerId; }
        }

        public int OrganizationId
        {
            get { return this.organizationId; }
        }

        public int GeographyId
        {
            get { return this.geographyId; }
        }

        public RoleType RoleType
        {
            get { return this.roleType; }
        }

        public bool Open
        {
            get { return this.open; }
        }

        public bool Assigned
        {
            get { return this.assigned; }
        }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.volunteerRoleId; }
        }

        #endregion
    }
}