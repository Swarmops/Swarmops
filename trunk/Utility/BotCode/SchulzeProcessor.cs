using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Governance;

namespace Swarmops.Utility.BotCode
{
    /// <summary>
    /// This class computes a Condorcet/Single-Transferable-Vote result according to the Schulze Method.
    /// </summary>
    public class SchulzeProcessor
    {
        public MeetingElectionVotes Votes { set; private get; }

        public MeetingElectionCandidates Candidates { set; private get; }

        public void Process()
        {
            CreateCandidateIdList();
            CalculateIntercandidatePreferences();
            CalculateLinkStrengths();
            AssembleFinalOrder();
        }


        private void CreateCandidateIdList()
        {
            // This function creates a list of all the candidate Ids in the election, as
            // well as a reverse lookup into the list in the form of a dictionary. So,
            // if candidate #4711 is placed into the fifth spot:
            //
            // candidateIds[4] = 4711;
            // indexOfCandidateId[4711] = 4;
            //
            // We use this translation later as we use indexes as identities in the preference matrix.

            Console.Write("Creating list of candidates and index lookup...");

            candidateIds = new List<int>();
            indexOfCandidateId = new Dictionary<int, int>();
            int index = 0;
            foreach (MeetingElectionCandidate candidate in Candidates)
            {
                candidateIds.Add(candidate.Identity);
                indexOfCandidateId[candidate.Identity] = index;
                index++;
            }

            Console.WriteLine(" done.");
        }


        private void CalculateIntercandidatePreferences()
        {
            // Iterate over all votes and set the preferences, as in "how many voters prefer candidate X over
            // candidate Y?". This data is needed for the next step, the link strengths.
            //
            // We do this by, for each vote, getting the list of candidates (in order of voting). We go from
            // the #1 candidate on the vote to the #n candidate on the vote, checking them off as we go, and
            // recording all ALREADY-CHECKED-OFF candidates as superior to the one we are CURRENTLY at.
            //
            // At the end of the iteration, we set all the NOT-checked-off candidates as inferior to those
            // checked off (as they were not voted for).

            preferenceXtoY = new int[candidateIds.Count,candidateIds.Count];  // huuuuuge matrix for 1000+ candidates, zero inited

            int count = 1;

            Console.WriteLine("Calculating raw preference matrix.");

            foreach (MeetingElectionVote vote in Votes)
            {
                Console.Write("\r - vote {0:D5} of {1:D5}...", count, Votes.Count);

                Dictionary<int, bool> candidateIdSeen = new Dictionary<int, bool>();
                int[] voteCandidateIds = vote.SelectedCandidateIdsInOrder;
                
                foreach (int candidateId in voteCandidateIds)
                {
                    // for each candidate, record all previously iterated as superior for this vote:

                    foreach (int previousCandidateId in candidateIdSeen.Keys)
                    {
                        preferenceXtoY[indexOfCandidateId[previousCandidateId], indexOfCandidateId[candidateId]]++;
                    }

                    // check this candidate off as iterated

                    candidateIdSeen[candidateId] = true;
                }

                // We have now gone through the list of candidates on the vote cast. The final step is that these are all
                // superior to the candidates NOT on the votes cast. Iterate through all the candidates to find which ones
                // were not mentioned.

                foreach (int candidateId in candidateIds)
                {
                    if (!candidateIdSeen.ContainsKey(candidateId))
                    {
                        // Not on vote, so inferior to all candidates on the vote

                        foreach (int voteCandidateId in voteCandidateIds)
                        {
                            preferenceXtoY[indexOfCandidateId[voteCandidateId], indexOfCandidateId[candidateId]]++;
                        }
                    }
                }

                count++;
            }

            Console.WriteLine(" done.");
        }


        private void CalculateLinkStrengths()
        {
            // This code is copied from the Wikipedia article on the Schulze method to avoid misimplementation.
            // 
            // http://en.wikipedia.org/wiki/Schulze_method

            linkStrengthXtoY = new int[candidateIds.Count,candidateIds.Count];

            // First part of magic. Establish the direction of pairwise defeats. I think.

            Console.WriteLine("Calculating link strengths.");

            for (int outerIndex = 0; outerIndex < candidateIds.Count; outerIndex++)
            {
                Console.Write("\r - Step 1: {0:D4} of {1:D4}...", outerIndex+1, candidateIds.Count);

                for (int innerIndex = 0; innerIndex < candidateIds.Count; innerIndex++)
                {
                    if (innerIndex == outerIndex)
                    {
                        continue;
                    }

                    if (preferenceXtoY[outerIndex, innerIndex] > preferenceXtoY[innerIndex, outerIndex])
                    {
                        linkStrengthXtoY[outerIndex, innerIndex] = preferenceXtoY[outerIndex, innerIndex];
                    }
                    // else set to zero, but this is handled by the framework on initialization

                }
            }

            Console.WriteLine(" done.");

            // Second part of magic, an O(n^3) algorithm. In the famous words of Alfred E. Neuman: Yecch!

            for (int outerIndex = 0; outerIndex < candidateIds.Count; outerIndex++)
            {
                Console.Write("\r - Step 2: {0:D4} of {1:D4}...", outerIndex + 1, candidateIds.Count);

                for (int middleIndex = 0; middleIndex < candidateIds.Count; middleIndex++)
                {
                    if (outerIndex == middleIndex)
                    {
                        continue;
                    }

                    for (int innerIndex = 0; innerIndex < candidateIds.Count; innerIndex++)
                    {

                        if (outerIndex == innerIndex || middleIndex == innerIndex)
                        {
                            continue;
                        }

                        linkStrengthXtoY[middleIndex, innerIndex] =
                            Math.Max(linkStrengthXtoY[middleIndex, innerIndex],
                                     Math.Min(linkStrengthXtoY[middleIndex, outerIndex],
                                              linkStrengthXtoY[outerIndex, innerIndex]));
                    }
                }
            }

            Console.WriteLine(" done.");
        }


        private void AssembleFinalOrder()
        {
            Console.WriteLine("Assembling final list.");

            MeetingElectionCandidates result = new MeetingElectionCandidates();

            Dictionary<int, bool> candidateAdded = new Dictionary<int, bool>();
            int candidatesRemaining = candidateIds.Count;

            int count = 1;

            while (candidatesRemaining > 0)
            {
                Console.Write("\r - remaining: {0:D4}", candidatesRemaining);

                // Find the first nontaken candidate

                int winningCandidateIndex = 0;

                while (candidateAdded.ContainsKey(winningCandidateIndex))
                {
                    winningCandidateIndex++;
                }

                // Iterate over all linkStrength[X,*] and [*,X]; this candidate is only a winner if
                // all [X,*] are >= all [*,X].

                bool allSuperior = false;


                while (!allSuperior)
                {
                    allSuperior = true;

                    for (int compareCandidateIndex = 0; compareCandidateIndex < candidateIds.Count; compareCandidateIndex++)
                    {
                        if (candidateAdded.ContainsKey(compareCandidateIndex))
                        {
                            continue;
                        }

                        if (linkStrengthXtoY[winningCandidateIndex, compareCandidateIndex] <
                            linkStrengthXtoY[compareCandidateIndex,winningCandidateIndex])
                        {
                            // the compareCandidateIndex had a greater link strength, so jump there and take it as
                            // a new potential winner, restarting the comparison

                            winningCandidateIndex = compareCandidateIndex;
                            allSuperior = false;
                            break;
                        }
                    }
                }

                // We have the winning candidate among the not-yet-ranked candidates:

                MeetingElectionCandidate candidate = MeetingElectionCandidate.FromIdentity(candidateIds[winningCandidateIndex]);

                candidateAdded[winningCandidateIndex] = true;
                result.Add(candidate);
                candidatesRemaining--;
                count++;
            }

            FinalOrder = result;

            Console.WriteLine(", done.");
        }


        private int[,] preferenceXtoY;
        private int[,] linkStrengthXtoY;
        private List<int> candidateIds;
        private Dictionary<int, int> indexOfCandidateId;

        public MeetingElectionCandidates FinalOrder { get; private set; }

        public int GetCandidateDistance (int finalOrderIndex)
        {
            // Gets the distance, in preference counts, between candidate [finalOrderIndex] and candidate [finalOrderIndex+1].
            // This is purely a diagnostic function and does not contribute to the results calculations.

            if (finalOrderIndex >= FinalOrder.Count -1)
            {
                return 0;
            }

            int candidateIdX = FinalOrder[finalOrderIndex].Identity;
            int candidateIdY = FinalOrder[finalOrderIndex + 1].Identity;

            int indexX = indexOfCandidateId[candidateIdX];
            int indexY = indexOfCandidateId[candidateIdY];

            int distance = preferenceXtoY[indexX, indexY] - preferenceXtoY[indexY, indexX];

            return distance;
        }
    }
}
