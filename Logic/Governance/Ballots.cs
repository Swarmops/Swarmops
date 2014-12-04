using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Governance
{
    public class Ballots : PluralBase<Ballots, Ballot, BasicBallot>
    {
        // nothing much here


        public static Ballots ForElection(Election election, Organization organization)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetBallots(election, organization));
        }

        public static Dictionary<int, int> GetBallotsForPerson(Person person)
        {
            return SwarmDb.GetDatabaseForReading().GetBallotsForPerson(person.Identity);
        }
    }
}