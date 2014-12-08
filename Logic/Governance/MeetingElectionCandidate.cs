using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Governance
{
    public class MeetingElectionCandidate : BasicInternalPollCandidate
    {
        private static readonly Dictionary<int, MeetingElectionCandidate> cache;
        private Person personCache;

        static MeetingElectionCandidate()
        {
            cache = new Dictionary<int, MeetingElectionCandidate>();
        }

        private MeetingElectionCandidate (BasicInternalPollCandidate basic) : base (basic)
        {
            // private ctor
        }


        public string PersonName
        {
            get { return Person.Name; }
        }

        public Person Person
        {
            get
            {
                if (this.personCache == null)
                {
                    this.personCache = Person.FromIdentity (PersonId);
                }

                return this.personCache;
            }
        }

        public string PersonSubtitle
        {
            get
            {
                DateTime referenceDate = new DateTime (2010, 9, 19);

                int age = referenceDate.Year - Person.Birthdate.Year;
                if (referenceDate < Person.Birthdate.AddYears (age))
                {
                    age--;
                }

                return String.Format ("{0} år, {1}", age, Person.Geography.Name);
            }
        }

        public string PersonBlogHtml
        {
            get { return "Blogg: <a target=\"_blank\" href=\"" + Person.BlogUrl + "\">" + Person.BlogName + "</a>"; }
        }

        public string PersonPhotoHtml
        {
            get
            {
                return "<img src=\"http://data.piratpartiet.se/Handlers/DisplayPortrait.aspx?YSize=48&PersonId=" +
                       PersonId + "\" align=\"right\" height=\"48\" width=\"36\">";
            }
        }

        public new string CandidacyStatement
        {
            get { return base.CandidacyStatement; }

            set
            {
                base.CandidacyStatement = value;
                cache[Identity] = this; // Replace cache instance
                SwarmDb.GetDatabaseForWriting().SetInternalPollCandidateStatement (Identity, value);
            }
        }

        public static MeetingElectionCandidate FromBasic (BasicInternalPollCandidate basic)
        {
            return new MeetingElectionCandidate (basic);
        }

        public static MeetingElectionCandidate FromIdentity (int internalPollCandidateId)
        {
            if (!cache.ContainsKey (internalPollCandidateId))
            {
                cache[internalPollCandidateId] =
                    FromBasic (SwarmDb.GetDatabaseForReading().GetInternalPollCandidate (internalPollCandidateId));
            }

            return cache[internalPollCandidateId];
        }

        public static MeetingElectionCandidate FromPersonAndPoll (Person person, MeetingElection poll)
        {
            MeetingElectionCandidates candidates =
                MeetingElectionCandidates.FromArray (SwarmDb.GetDatabaseForReading()
                    .GetInternalPollCandidates (person, poll));

            if (candidates.Count == 0)
            {
                return null; // Or throw exception? Which is the most accurate?
            }

            if (candidates.Count > 1)
            {
                throw new InvalidOperationException (
                    "A specific person can not be a candidate more than once for a specific poll");
            }

            return candidates[0];
        }


        public static MeetingElectionCandidate Create (MeetingElection poll, Person person, string candidacyStatement)
        {
            return
                FromIdentity (SwarmDb.GetDatabaseForWriting()
                    .CreateInternalPollCandidate (poll.Identity, person.Identity, candidacyStatement));
        }

        public override string ToString()
        {
            return Identity.ToString();
        }

        public void SetSortOrder (string sortOrder)
        {
            SwarmDb.GetDatabaseForWriting().SetInternalPollCandidateSortOrder (Identity, sortOrder);
        }

        public void RandomizeSortOrder()
        {
            SwarmDb.GetDatabaseForWriting().SetInternalPollCandidateSortOrder (Identity, Guid.NewGuid().ToString());
        }
    }
}