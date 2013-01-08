using System.Collections.Generic;
using Swarmops.Database;

namespace Swarmops.Logic.Support
{
    /// <summary>
    /// This class contains one-off optimizations that don't fit anywhere.
    /// </summary>
    public class Optimizations
    {
        public static Dictionary<int, bool> GetPeopleWhoDeclineLocalMail (int[] personIds)
        {
            return PirateDb.GetDatabaseForReading().GetPeopleWhoDeclineLocalMail (personIds);
        }

        public static Dictionary<int, int> GetGeographyVoterCounts()
        {
            return PirateDb.GetDatabaseForReading().GetGeographyVoterCounts();
        }

        public static Dictionary<int,int> GetInternalPollVoteCountsPerGeography(int pollId)
        {
            return PirateDb.GetDatabaseForReading().GetInternalPollVoteCountsPerGeography(pollId);
        }
    }
}