using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Swarm
{
    public class PersonRole : BasicPersonRole
    {
        #region Creation and Construction

        private PersonRole() : base(null)
        {
        }

        private PersonRole (BasicPersonRole basic) : base(basic)
        {
        }

        public static PersonRole Create (int personId, RoleType roleType, int organizationId, int nodeId)
        {
            return FromIdentityAggressive(SwarmDb.GetDatabaseForWriting().CreateRole(personId, roleType, organizationId, nodeId));
        }

        public static PersonRole FromBasic (BasicPersonRole basic)
        {
            return new PersonRole(basic);
        }

        public static PersonRole FromIdentity(int roleId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetRole(roleId));
        }

        public static PersonRole FromIdentityAggressive(int roleId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetRole(roleId)); // Note "for writing". Intentional. Queries master db and bypasses replication lag to avoid race conditions.
        }

        #endregion

        public Person Person
        {
            get { return Person.FromIdentity(base.PersonId); }
        }

        public Geography Geography
        {
            get { return Geography.FromIdentity(base.GeographyId); }
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity(base.OrganizationId); }
        }

        #region Manipulation

        public void Delete()
        {
            SwarmDb.GetDatabaseForWriting().DeleteRole(Identity);

            // After this call, the role will be invalid.
        }

        #endregion
    }
}