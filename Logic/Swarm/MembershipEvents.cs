using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;

namespace Swarmops.Logic.Swarm
{
    [Serializable]
    public class MembershipEvents : List<MembershipEvent>
    {
        public static MembershipEvents LoadAll()
        {
            // This is a very expensive op - heavy databasery and some five to ten seconds of execution time.

            // This function works in three steps. First, we load all people and build a hash of their geographies, genders.
            // Second, we load all memberships.
            // Third, we generate membership events from the membership and hash combo.

            // (and fourth, we sort the events by date)

            // In the person-gathering phase, we use BasicPerson in order to avoid the unneeded translation to Person.

            BasicPerson[] allPeople = SwarmDb.GetDatabaseForReading().GetAllPeople();

            Dictionary<int, int> geoLookup = new Dictionary<int, int>();
            Dictionary<int, PersonGender> genderLookup = new Dictionary<int, PersonGender>();
            Dictionary<int, int> birthYearLookup = new Dictionary<int, int>();

            foreach (BasicPerson person in allPeople)
            {
                geoLookup[person.Identity] = person.GeographyId;
                genderLookup[person.Identity] = person.IsMale ? PersonGender.Male : PersonGender.Female;
                birthYearLookup[person.Identity] = person.Birthdate.Year;
            }

            // Second phase, load all memberships

            BasicMembership[] allMemberships = SwarmDb.GetDatabaseForReading().GetMemberships();

            // Third phase - for every membership, generate one or two membership events

            MembershipEvents result = new MembershipEvents();

            foreach (BasicMembership membership in allMemberships)
            {
                int geographyId = 1;
                int birthYear = 0;
                PersonGender gender = PersonGender.Unknown;

                if (geoLookup.ContainsKey (membership.PersonId))
                {
                    geographyId = geoLookup[membership.PersonId];
                    gender = genderLookup[membership.PersonId];
                    birthYear = birthYearLookup[membership.PersonId];
                }

                result.Add (new MembershipEvent (membership.MemberSince, membership.PersonId, membership.OrganizationId,
                    geographyId, birthYear, gender, 1));

                if (!membership.Active)
                {
                    TimeSpan safetyDelta = new TimeSpan (0);

                    // A few records in the database have had their memberships terminated at the exact time of creation. This means that sorting will
                    // be unpredictable, when it relies on the termination coming at a later time than the creation.

                    // To solve this, make sure they are more than five seconds apart.

                    if (membership.DateTerminated.Date == membership.MemberSince.Date)
                        // First simple check
                    {
                        if ((membership.DateTerminated - membership.MemberSince) < new TimeSpan (0, 0, 5))
                        {
                            safetyDelta = new TimeSpan (0, 0, 5);
                        }
                    }

                    result.Add (new MembershipEvent (membership.DateTerminated + safetyDelta,
                        membership.PersonId, membership.OrganizationId, geographyId,
                        birthYear, gender, -1));
                }
            }

            // Fourth - sort

            result.Sort (new MembershipEventSorter());

            return result;
        }


        public static MembershipEvents FromXml (string xml)
        {
            XmlSerializer serializer = new XmlSerializer (typeof (MembershipEvents));

            MemoryStream stream = new MemoryStream();
            byte[] xmlBytes = Encoding.Default.GetBytes (xml);
            stream.Write (xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            MembershipEvents result = (MembershipEvents) serializer.Deserialize (stream);
            stream.Close();

            return result;
        }


        public string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer (typeof (MembershipEvents));

            MemoryStream stream = new MemoryStream();
            serializer.Serialize (stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            return Encoding.Default.GetString (xmlBytes);
        }
    }

    internal class MembershipEventSorter : IComparer<MembershipEvent>
    {
        #region IComparer<MembershipEvent> Members

        public int Compare (MembershipEvent x, MembershipEvent y)
        {
            if (x.DateTime < y.DateTime)
            {
                return -1;
            }
            if (x.DateTime > y.DateTime)
            {
                return 1;
            }

            return 0;
        }

        #endregion
    }
}