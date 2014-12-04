using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Governance
{
    public class MeetingElections : PluralBase<MeetingElections, MeetingElection, BasicInternalPoll>
    {
        public static MeetingElections ForOrganization(Organization organization)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetInternalPolls(organization));
        }

        public static MeetingElections ForOrganizations(Organizations organizations)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetInternalPolls(organizations));
        }

        public static MeetingElections GetAll()
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetInternalPolls());
        }
    }
}