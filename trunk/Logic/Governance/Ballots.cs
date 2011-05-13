using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Governance
{
    public class Ballots: PluralBase<Ballots,Ballot,BasicBallot>
    {
        // nothing much here

        public static Ballots ForElection (Election election)
        {
            return FromArray(PirateDb.GetDatabase().GetBallots(election, Organization.PPSE)); // HACK HACK HACK -- PPSE hardcoded
        }

        public static Dictionary<int,int> GetBallotsForPerson (Person person)
        {
            return PirateDb.GetDatabase().GetBallotsForPerson(person.Identity);
        }
    }
}
