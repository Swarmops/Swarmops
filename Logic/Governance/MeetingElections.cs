using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Governance
{
    public class MeetingElections: PluralBase<MeetingElections,MeetingElection,BasicInternalPoll>
    {
        public static MeetingElections ForOrganization (Organization organization)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetInternalPolls(organization));
        }

        public static MeetingElections ForOrganizations (Organizations organizations)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetInternalPolls(organizations));
        }

        public static MeetingElections GetAll()
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetInternalPolls());
        }
    }
}