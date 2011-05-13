using Activizr.Logic.Structure;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Pirates
{
    public class VolunteerRole : BasicVolunteerRole
    {
        private VolunteerRole (BasicVolunteerRole basic) :
            base(basic)
        {
        }

        public Geography Geography
        {
            get { return Geography.FromIdentity(base.GeographyId); } // Cache later if necessary
        }

        public static VolunteerRole FromBasic (BasicVolunteerRole basic)
        {
            return new VolunteerRole(basic);
        }

        public static VolunteerRole FromIdentity (int identity)
        {
            return FromBasic(PirateDb.GetDatabase().GetVolunteerRole(identity));
        }

        public void Close (bool wasAssigned)
        {
            // TODO
        }
    }
}