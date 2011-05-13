using System.Collections.Generic;

using Activizr.Database;

namespace Activizr.Logic.Support
{
    /// <summary>
    /// This class contains one-off optimizations that don't fit anywhere.
    /// </summary>
    public class Optimizations
    {
        public static Dictionary<int, bool> GetPeopleWhoDeclineLocalMail (int[] personIds)
        {
            return PirateDb.GetDatabase().GetPeopleWhoDeclineLocalMail (personIds);
        }

        public static Dictionary<int, int> GetGeographyVoterCounts()
        {
            return PirateDb.GetDatabase().GetGeographyVoterCounts();
        }

        public static Dictionary<int,int> GetInternalPollVoteCountsPerGeography(int pollId)
        {
            return PirateDb.GetDatabase().GetInternalPollVoteCountsPerGeography(pollId);
        }
    }
}