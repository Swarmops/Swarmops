using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Logic.Support;

namespace Activizr.Logic.Governance
{
    public class MeetingElectionVoters: List<MeetingElectionVoter>
    {
        static public MeetingElectionVoters FromArray(BasicInternalPollVoter[] basicArray)
        {
            var result = new MeetingElectionVoters { Capacity = basicArray.Length * 11 / 10 };

            foreach (BasicInternalPollVoter basic in basicArray)
            {
                result.Add(MeetingElectionVoter.FromBasic(basic));
            }

            return result;
        }

        static public MeetingElectionVoters ForPoll(MeetingElection poll, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray(PirateDb.GetDatabase().GetInternalPollVoters(poll));
            }
            else
            {
                return FromArray(PirateDb.GetDatabase().GetInternalPollVoters(poll, DatabaseCondition.OpenTrue));
            }
        }

        static public MeetingElectionVoters ForPoll(MeetingElection poll)
        {
            return ForPoll(poll, false);
        }

        static public MeetingElectionVoters ForPollClosed (MeetingElection poll)
        {
            return FromArray(PirateDb.GetDatabase().GetInternalPollVoters(poll, DatabaseCondition.OpenFalse));
        }

    }
}