using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Governance
{
    public class MeetingElectionCandidate: BasicInternalPollCandidate
    {
        private MeetingElectionCandidate (BasicInternalPollCandidate basic): base (basic)
        {
            // private ctor
        }

        public static MeetingElectionCandidate FromBasic (BasicInternalPollCandidate basic)
        {
            return new MeetingElectionCandidate(basic);
        }

        public static MeetingElectionCandidate FromIdentity (int internalPollCandidateId)
        {
            if (!cache.ContainsKey(internalPollCandidateId))
            {
                cache [internalPollCandidateId] = FromBasic(PirateDb.GetDatabaseForReading().GetInternalPollCandidate(internalPollCandidateId));
            }

            return cache[internalPollCandidateId];
        }

        public static MeetingElectionCandidate FromPersonAndPoll (Person person, MeetingElection poll)
        {
            MeetingElectionCandidates candidates =
                MeetingElectionCandidates.FromArray(PirateDb.GetDatabaseForReading().GetInternalPollCandidates(person, poll));

            if (candidates.Count == 0)
            {
                return null; // Or throw exception? Which is the most accurate?
            }

            if (candidates.Count > 1)
            {
                throw new InvalidOperationException(
                    "A specific person can not be a candidate more than once for a specific poll");
            }

            return candidates[0];
        }


        public static MeetingElectionCandidate Create (MeetingElection poll, Person person, string candidacyStatement)
        {
            return FromIdentity (PirateDb.GetDatabaseForWriting().CreateInternalPollCandidate(poll.Identity, person.Identity, candidacyStatement));
        }

        private static Dictionary<int, MeetingElectionCandidate> cache;

        static MeetingElectionCandidate()
        {
            cache = new Dictionary<int, MeetingElectionCandidate>();
        }


        public string PersonName
        {
            get { return this.Person.Name; }
        }

        public Person Person
        {
            get
            {
                if (personCache == null)
                {
                    personCache = Person.FromIdentity(this.PersonId);
                }

                return personCache;
            }
        }

        private Person personCache;

        public string PersonSubtitle
        {
            get
            {
                DateTime referenceDate = new DateTime(2010, 9, 19);

                int age = referenceDate.Year - Person.Birthdate.Year;
                if (referenceDate < Person.Birthdate.AddYears(age))
                {
                    age--;
                }

                return String.Format("{0} år, {1}", age, Person.Geography.Name);
            }
        }

        public string PersonBlogHtml
        {
            get
            {
                return "Blogg: <a target=\"_blank\" href=\"" + Person.BlogUrl + "\">" + Person.BlogName + "</a>";
            }
        }

        public string PersonPhotoHtml
        {
            get
            {
                return "<img src=\"http://data.piratpartiet.se/Handlers/DisplayPortrait.aspx?YSize=48&PersonId=" + this.PersonId.ToString() + "\" align=\"right\" height=\"48\" width=\"36\">";
            }
        }

        public override string ToString()
        {
            return this.Identity.ToString();
        }

        public void SetSortOrder(string sortOrder)
        {
            PirateDb.GetDatabaseForWriting().SetInternalPollCandidateSortOrder(this.Identity, sortOrder);
        }

        public void RandomizeSortOrder()
        {
            PirateDb.GetDatabaseForWriting().SetInternalPollCandidateSortOrder(this.Identity, System.Guid.NewGuid().ToString());
        }

        public new string CandidacyStatement
        { 
            get { return base.CandidacyStatement; }

            set
            {
                base.CandidacyStatement = value;
                cache[this.Identity] = this;  // Replace cache instance
                PirateDb.GetDatabaseForWriting().SetInternalPollCandidateStatement(this.Identity, value);
            }
        }
    }
}