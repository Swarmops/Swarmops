using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Pirates;
using Swarmops.Logic.Structure;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Governance
{
    public class Ballots: PluralBase<Ballots,Ballot,BasicBallot>
    {
        // nothing much here

        public static Ballots ForElection (Election election)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetBallots(election, Organization.PPSE)); // HACK HACK HACK -- PPSE hardcoded
        }

        public static Dictionary<int,int> GetBallotsForPerson (Person person)
        {
            return PirateDb.GetDatabaseForReading().GetBallotsForPerson(person.Identity);
        }
    }
}
