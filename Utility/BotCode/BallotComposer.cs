using System;
using System.Collections.Generic;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Governance;
using Swarmops.Logic.Structure;

namespace Swarmops.Utility.BotCode
{
    public class BallotComposer
    {
        private Dictionary<int, bool> defectionsLookup;
        private Dictionary<int, MeetingElectionCandidates> geographicLists;
        private MeetingElectionCandidates rawElectionResult;

        /// <summary>
        ///     Minimum fraction of either gender. Can be in the range 0.0 -- 0.5.
        /// </summary>
        public double GenderQuota { get; set; }

        /// <summary>
        ///     Sets the output from the election count, i.e. the Schulze output.
        /// </summary>
        public MeetingElectionCandidates RawElectionResult
        {
            set
            {
                this.rawElectionResult = value;
                this.geographicLists = new Dictionary<int, MeetingElectionCandidates>();
            }
        }

        /// <summary>
        ///     Sets the geographies for which to construct ballots.
        /// </summary>
        public Geographies BallotGeographies { set; get; }

        /// <summary>
        ///     Gets the final result.
        /// </summary>
        public Dictionary<int, MeetingElectionCandidates> FinalBallots { get; private set; }

        public MeetingElectionCandidates QuotedMasterList { get; private set; }


        public int[] DefectedPersonIds
        {
            set
            {
                this.defectionsLookup = new Dictionary<int, bool>();
                foreach (int defectedPersonId in value)
                {
                    this.defectionsLookup[defectedPersonId] = true;
                }
            }
        }


        public void ComposeBallots (BallotCompositionMethod method)
        {
            FinalBallots = new Dictionary<int, MeetingElectionCandidates>();

            QuotedMasterList = GenerateBallot (this.rawElectionResult, -1, BallotCompositionMethod.Unmixed);
            if (this.geographicLists.Count == 0)
            {
                GenerateGeographicLists();
            }

            foreach (Geography ballotGeography in BallotGeographies)
            {
                FinalBallots[ballotGeography.Identity] = GenerateBallot (this.geographicLists[ballotGeography.Identity],
                    22,
                    method);
            }
        }


        public bool CandidatePersonIdDefected (int personId)
        {
            return this.defectionsLookup.ContainsKey (personId);
        }


        private void GenerateGeographicLists()
        {
            foreach (MeetingElectionCandidate candidate in this.rawElectionResult)
            {
                foreach (Geography ballotGeography in BallotGeographies)
                {
                    if (candidate.Person.Geography.Inherits (ballotGeography))
                    {
                        // This candidate is under this particular geography, so add to list

                        if (!this.geographicLists.ContainsKey (ballotGeography.Identity))
                        {
                            this.geographicLists[ballotGeography.Identity] = new MeetingElectionCandidates();
                        }

                        this.geographicLists[ballotGeography.Identity].Add (candidate);
                    }
                }
            }
        }


        private MeetingElectionCandidates GenerateBallot (MeetingElectionCandidates rawList, int candidateCount,
            BallotCompositionMethod method)
        {
            MeetingElectionCandidates result = new MeetingElectionCandidates();
            Dictionary<int, bool> takenCandidates = new Dictionary<int, bool>();

            if (candidateCount == -1)
            {
                candidateCount = rawList.Count;
            }

            // First, cancel out the defected candidates. Expensive op, but don't care.

            for (int rawIndex = 0; rawIndex < rawList.Count; rawIndex++)
            {
                if (CandidatePersonIdDefected (rawList[rawIndex].PersonId))
                {
                    takenCandidates[rawIndex] = true;
                }
            }

            if (candidateCount == rawList.Count)
            {
                candidateCount -= takenCandidates.Count;
            }

            // Assemble list.

            while (result.Count < candidateCount)
            {
                // Add a candidate.
                //
                // Should we add a candidate from the raw list or from the quoted master list?

                int masterPosition = GetMasterListPosition (result.Count + 1, method);

                if (masterPosition > 0)
                {
                    // We should add a candidate from the master list, no quota calculations, BUT ONLY if this candidate
                    // hasn't already been added from the district list.

                    int rawListIndex = FindCandidateIndex (QuotedMasterList[masterPosition - 1].InternalPollCandidateId,
                        rawList);

                    if (rawListIndex >= 0 && !takenCandidates.ContainsKey (rawListIndex))
                    {
                        // The master list candidate is also on the district list, but has not been added. Add
                        // the candidate in the master list position.

                        result.Add (QuotedMasterList[masterPosition - 1]);
                        takenCandidates[rawListIndex] = true;
                        continue;
                    }

                    if (rawListIndex == -1)
                    {
                        // The master list candidate is not on the district list. Add the candidate.

                        result.Add (QuotedMasterList[masterPosition - 1]);
                        continue;
                    }

                    // Getting here, the candidate on the master position had already been placed on the list through his
                    // or her district, so fallthrough to local candidates for this master position
                }

                // Add a candidate from the local list. First, try adding the next available candidate.
                // If this puts us outside of the allowed gender quota, pick the next candidate of the 
                // underrepresented gender instead.

                int nextCandidateIndex = 0;

                while (takenCandidates.ContainsKey (nextCandidateIndex))
                {
                    nextCandidateIndex++;
                }

                PersonGender candidateGender = rawList[nextCandidateIndex].Person.Gender;

                double currentMaleQuota = CalculateListMaleQuota (result, candidateGender);

                // Here, we account an extremely rare situation where the correct quota is not even obtainable, e.g. 
                // for position #3 where #1 is male and #2 is female and with a quota of 40%. Either way, it will
                // fall outside of bounds (33% or 66%). If so, ignore the quota if it is not obtainable.

                PersonGender oppositeGender = (candidateGender == PersonGender.Male
                    ? PersonGender.Female
                    : PersonGender.Male);

                double alternateMaleQuota = CalculateListMaleQuota (result, oppositeGender);

                // If currentMaleQuota is outside of bounds AND the alternate quota IS inside of bounds...

                if (currentMaleQuota < GenderQuota || currentMaleQuota > (1.0 - GenderQuota))
                {
                    if (alternateMaleQuota >= GenderQuota && alternateMaleQuota <= (1.0 - GenderQuota))
                    {
                        // ...pick the alternate (underrepresented) candidate

                        try
                        {
                            nextCandidateIndex = GetNextSpecificGenderCandidate (rawList, takenCandidates,
                                oppositeGender);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            // there isn't any more candidate of the requested gender, so go with what we have
                        }
                    }
                }

                result.Add (rawList[nextCandidateIndex]);
                takenCandidates[nextCandidateIndex] = true;
            }

            return result;
        }


        private static int FindCandidateIndex (int candidateId, MeetingElectionCandidates list)
        {
            for (int index = 0; index < list.Count; index++)
            {
                if (list[index].Identity == candidateId)
                {
                    return index;
                }
            }

            return -1;
        }


        private static double CalculateListMaleQuota (MeetingElectionCandidates candidates,
            PersonGender nextCandidateGender)
        {
            if (candidates.Count < 1)
            {
                return 0.5; // Division by zero protection, here and in caller
            }

            double malesTotal = 0.0;

            foreach (MeetingElectionCandidate candidate in candidates)
            {
                if (candidate.Person.Gender == PersonGender.Male)
                {
                    malesTotal += 1.0;
                }
            }

            if (nextCandidateGender == PersonGender.Male)
            {
                malesTotal += 1.0;
            }

            return malesTotal/(candidates.Count + 1.0);
        }


        private static int GetNextSpecificGenderCandidate (MeetingElectionCandidates candidates,
            Dictionary<int, bool> takenCandidates, PersonGender desiredGender)
        {
            int index = 0;

            while (takenCandidates.ContainsKey (index) || candidates[index].Person.Gender != desiredGender)
            {
                index++; // potential out-of-range exception here, if there aren't enough candidates
            }

            return index;
        }

        /// <summary>
        ///     Gets the position of the master list to use for this position on the local list.
        /// </summary>
        /// <param name="currentPosition">The position of a particular ballot.</param>
        /// <param name="method">The composition method to use.</param>
        /// <returns>The position to pick from the master list, or zero if not from the master list.</returns>
        private int GetMasterListPosition (int currentPosition, BallotCompositionMethod method)
        {
            if (method == BallotCompositionMethod.Unmixed)
            {
                return 0; // always use local
            }

            if (method == BallotCompositionMethod.TopTenNational)
            {
                if (currentPosition <= 10)
                {
                    return currentPosition;
                }

                return 0;
            }

            if (method == BallotCompositionMethod.TopFiveNational)
            {
                if (currentPosition <= 5)
                {
                    return currentPosition;
                }

                return 0;
            }

            if (method == BallotCompositionMethod.InterleavedTenNational)
            {
                if ((currentPosition%2 == 1) && currentPosition < 20)
                {
                    return (currentPosition/2 + 1);
                }

                return 0;
            }

            throw new NotImplementedException ("Unimplemented method");
        }
    }

    public enum BallotCompositionMethod
    {
        /// <summary>
        ///     FxCop directive - unused
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     The national top ten names are the top ten on all geographic ballots.
        /// </summary>
        TopTenNational,

        /// <summary>
        ///     The national top five names are the top ten on all geographic ballots.
        /// </summary>
        TopFiveNational,

        /// <summary>
        ///     Odd positions 1..19 are the national top ten names on all geographic ballots.
        /// </summary>
        InterleavedTenNational,

        /// <summary>
        ///     No top national names; only names from this district.
        /// </summary>
        Unmixed
    }
}