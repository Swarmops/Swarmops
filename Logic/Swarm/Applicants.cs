using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class Applicants: PluralBase<Applicants, Applicant, BasicApplicant>
    {

        public static Dictionary<int, List<BasicApplicant>> GetApplicantsFromPeople(People people)
        {
            return SwarmDb.GetDatabaseForReading().GetApplicantsFromPeople(people.Identities);
        }

        public static Applicants FromPeopleInOrganization(People people, Organization organization)
        {
            Organizations orgTree = organization.ThisAndBelow();

            return FromArray(SwarmDb.GetDatabaseForReading().GetApplicants(people, orgTree));
        }
    }
}
