using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Database;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class People : List<Person>, IHasIdentities, IHasSingularPluralTypes
    {
        public int[] Identities
        {
            get { return LogicServices.ObjectsToIdentifiers (ToArray()); }
        }

        public static People FromMemberships (Memberships memberships)
        {
            List<int> personIds = new List<int>();

            foreach (Membership membership in memberships)
            {
                personIds.Add (membership.PersonId);
            }

            return FromIdentities (personIds.ToArray());
        }

        public static People FromIdentities (int[] personIds)
        {
            if (personIds.Length > 500)
            {
                // If over fivehundred identities are requested, we won't pass the array to
                // SQL Server. Rather, we'll get ALL the people and parse the list ourselves.

                Dictionary<int, bool> lookup = new Dictionary<int, bool>();

                foreach (int key in personIds)
                {
                    lookup[key] = true;
                }

                BasicPerson[] basicArray = SwarmDb.GetDatabaseForReading().GetAllPeople();
                People result = new People();

                for (int index = 0; index < basicArray.Length; index++)
                {
                    if (lookup.ContainsKey (basicArray[index].Identity))
                    {
                        result.Add (Person.FromBasic (basicArray[index]));
                    }
                }

                return result;
            }
            return People.FromArray (SwarmDb.GetDatabaseForReading().GetPeople (personIds));
        }

        public static People GetAll()
        {
            return People.FromArray (SwarmDb.GetDatabaseForReading().GetAllPeople());
        }

        public static People FromNamePattern (string namePattern)
        {
            if (namePattern.Trim().Length < 3)
            {
                return new People();
            }

            return People.FromArray (SwarmDb.GetDatabaseForReading().GetPeopleFromNamePattern (namePattern));
        }

        public static People FromBirthDatePattern (DateTime fromdate, DateTime todate)
        {
            return People.FromArray (SwarmDb.GetDatabaseForReading().GetPeopleFromBirthdate (fromdate, todate));
        }

        public static People FromEmailPattern (string emailPattern)
        {
            // Remove any injection codes
            emailPattern = emailPattern.Replace ("%", String.Empty).Trim();

            if (emailPattern.Length > 0)
            {
                emailPattern = "%" + emailPattern + "%";

                return People.FromArray (SwarmDb.GetDatabaseForReading().GetPeopleFromEmailPattern (emailPattern));
            }

            return null; // If no valid search string was supplied
        }

        public static People FromCityPattern (string cityPattern)
        {
            // Remove any injection codes
            cityPattern = cityPattern.Replace ("%", String.Empty).Trim();

            if (cityPattern.Length > 0)
            {
                cityPattern = "%" + cityPattern + "%";

                return People.FromArray (SwarmDb.GetDatabaseForReading().GetPeopleFromCityPattern (cityPattern));
            }

            return null; // If no valid search string was supplied
        }

        public static People FromPostalCodePattern (string pcPattern)
        {
            // Remove any injection codes
            pcPattern = pcPattern.Replace ("%", String.Empty).Trim();

            if (pcPattern.Length > 0)
            {
                pcPattern = pcPattern + "%";

                return People.FromArray (SwarmDb.GetDatabaseForReading().GetPeopleFromPostalCodePattern (pcPattern));
            }

            return null; // If no valid search string was supplied
        }

        public static People FromPostalCodes (string[] pcodes)
        {
            // Remove any injection codes
            if (pcodes.Length > 0)
            {
                return People.FromArray (SwarmDb.GetDatabaseForReading().GetPeopleFromPostalCodes (pcodes));
            }

            return null; // If no valid search string was supplied
        }

        public static People FromMail (string email)
        {
            if (email.Length > 0)
            {
                return People.FromArray (SwarmDb.GetDatabaseForReading().GetPeopleFromEmail (email));
            }

            return null; // If no valid search string was supplied
        }

        public static People LogicalAnd (People set1, People set2)
        {
            // If either set is invalid, return the other
            // (a null is different from an empty set)

            if (set1 == null)
            {
                return set2;
            }

            if (set2 == null)
            {
                return set1;
            }

            People result = new People();
            Dictionary<int, bool> set2Lookup = new Dictionary<int, bool>();

            // Build set2's lookup table

            foreach (Person person in set2)
            {
                set2Lookup[person.Identity] = true;
            }

            // Build result

            foreach (Person person in set1)
            {
                if (set2Lookup.ContainsKey (person.Identity))
                {
                    result.Add (person);
                }
            }

            return result;
        }

        public static People LogicalOr (People set1, People set2)
        {
            // If either set is invalid, return the other
            // (a null is different from an empty set)

            if (set1 == null)
            {
                return set2;
            }

            if (set2 == null)
            {
                return set1;
            }

            // Build table, eliminating duplicates

            Dictionary<int, Person> table = new Dictionary<int, Person>();

            foreach (Person person in set1)
            {
                if (person == null) continue;

                table[person.Identity] = person;
            }

            foreach (Person person in set2)
            {
                if (person == null) continue;

                table[person.Identity] = person;
            }

            // Assemble result, without any nulls in the original sets

            People result = new People();

            foreach (Person person in table.Values)
            {
                result.Add (person);
            }

            return result;
        }

        public People LogicalAnd (People set2)
        {
            return LogicalAnd (this, set2);
        }

        public People LogicalOr (People set2)
        {
            return LogicalOr (this, set2);
        }

        public People Filter (Predicate<Person> match)
        {
            People retlist = new People();

            ForEach (delegate (Person p)
            {
                if (match (p))
                    retlist.Add (p);
            });

            return retlist;
        }


        public static People FromOrganizationAndGeographyWithPattern (Organization organization, Geography geography,
            string pattern)
        {
            Geographies geoTree = geography.GetTree();

            pattern = pattern.Trim();
            if (pattern.Length == 0)
            {
                return FromOrganizationAndGeography (organization, geography);
            }
            if (pattern.Length < 3)
            {
                // too short pattern! Return empty set.
                return new People();
            }

            // First, get list of people in the geography, then filter on memberships

            BasicPerson[] candidates =
                SwarmDb.GetDatabaseForReading().GetPeopleInGeographiesWithPattern (geoTree.Identities, pattern);

            return FilterByMembership (candidates, organization);
        }


        public static People FromOrganizationAndGeography (Organization organization, Geography geography)
        {
            Geographies geoTree = geography.GetTree();

            // First, get list of people in the geography, then filter on memberships

            BasicPerson[] people = SwarmDb.GetDatabaseForReading().GetPeopleInGeographies (geoTree.Identities);

            // Filter on memberships

            return FilterByMembership (people, organization);
        }


        private static People FilterByMembership (BasicPerson[] candidates, Organization organization)
        {
            // Get the organization tree

            Organizations orgTree = organization.GetTree();

            // Build a lookup table of this organization tree. For each person in 'people' 
            // that has at least one membership in an organization in the lookup table,
            // add that person to the final result.

            Dictionary<int, BasicOrganization> lookup = new Dictionary<int, BasicOrganization>();

            foreach (Organization org in orgTree)
            {
                lookup[org.Identity] = org;
            }

            // Get the list of all memberships

            Dictionary<int, List<BasicMembership>> memberships =
                SwarmDb.GetDatabaseForReading()
                    .GetMembershipsForPeople (LogicServices.ObjectsToIdentifiers (candidates));

            People result = new People();

            foreach (BasicPerson basicPerson in candidates)
            {
                bool memberOfOrganization = false;

                if (memberships.ContainsKey (basicPerson.Identity))
                {
                    foreach (BasicMembership membership in memberships[basicPerson.Identity])
                    {
                        if (lookup.ContainsKey (membership.OrganizationId))
                        {
                            memberOfOrganization = true;
                        }
                    }
                }

                if (memberOfOrganization)
                {
                    result.Add (Person.FromBasic (basicPerson));
                }
            }

            return result;
        }

        public static People FromGeography (int geographyId)
        {
            Geographies geoTree = Geography.FromIdentity (geographyId).GetTree();

            // First, get list of people in the geography, then filter on memberships

            BasicPerson[] people = SwarmDb.GetDatabaseForReading().GetPeopleInGeographies (geoTree.Identities);

            People result = new People();

            foreach (BasicPerson basicPerson in people)
            {
                result.Add (Person.FromBasic (basicPerson));
            }

            return result;
        }

        public static People FromSingle (Person person)
        {
            People result = new People();
            result.Add (person);

            return result;
        }


        public static People FromArray (Person[] personArray)
        {
            People result = new People();

            result.Capacity = personArray.Length*11/10;
            foreach (Person person in personArray)
            {
                result.Add (person);
            }

            return result;
        }

        public static People FromArray (BasicPerson[] personArray)
        {
            People result = new People();

            result.Capacity = personArray.Length*11/10;
            foreach (BasicPerson basic in personArray)
            {
                result.Add (Person.FromBasic (basic));
            }

            return result;
        }


        public static People FromOptionalData (ObjectOptionalDataType dataType, string data)
        {
            return
                FromIdentities (SwarmDb.GetDatabaseForReading()
                    .GetObjectsByOptionalData (ObjectType.Person, dataType, data));
        }

        public static People FromPhoneNumber (int countryId, string phoneNumber)
        {
            return People.FromArray (SwarmDb.GetDatabaseForReading().GetPeopleFromPhoneNumber (countryId, phoneNumber));
        }

        public static People FromPhoneNumber (string countryCode, string phoneNumber)
        {
            return People.FromArray (SwarmDb.GetDatabaseForReading().GetPeopleFromPhoneNumber (countryCode, phoneNumber));
        }

        /*
        public People GetVisiblePeople (Organization organization)
        {
            People result = new People();

            // First, get all the currently visible people for this org

            Dictionary<RoleType, bool> roleTypeLookup = Authorization.VisibleRolesDictionary;
            Roles orgRoles = Roles.FromOrganization (organization);

            Dictionary<int, bool> personIdLookup = new Dictionary<int, bool>();

            foreach (PersonRole orgRole in orgRoles)
            {
                if (roleTypeLookup.ContainsKey (orgRole.Type))
                {
                    personIdLookup[orgRole.PersonId] = true;
                }
            }

            // Now that we have all the visible people in the org, match them against the current list

            foreach (Person person in this)
            {
                if (personIdLookup.ContainsKey (person.Identity))
                {
                    result.Add (person);
                }
            }

            return result;
        }

        public People RemoveUnique()
        {
            BasicPerson[] arrayToFilter = ToArray();
            BasicPerson[] filtered = Authorization.FilterUniquePeople (arrayToFilter);
            return FromArray (filtered);
        }

        public People RemoveUnlisted()
        {
            BasicPerson[] arrayToFilter = ToArray();
            BasicPerson[] filtered = Authorization.FilterUnlistedPeople (arrayToFilter);
            return FromArray (filtered);
        }*/


        /*
        private void Repopulate(BasicPerson[] personArray)
        {
            this.Clear();

            this.Capacity = personArray.Length * 11 / 10;
            foreach (PirateWeb.Basic.Types.BasicPerson person in personArray)
            {
                this.Add((Person)person);
            }
        }*/

        public static People FromNewsletterFeed (int feedId)
        {
            int[] subscriberIds = SwarmDb.GetDatabaseForReading().GetSubscribersForNewsletterFeed (2);

            return FromIdentities (subscriberIds);
        }

        public new void Remove (Person personToRemove)
        {
            for (int index = 0; index < Count; index++)
            {
                if (this[index].Identity == personToRemove.Identity)
                {
                    RemoveAt (index);
                    index--;
                }
            }
        }


        public static Dictionary<int, int> GetPeopleGeographies()
        {
            return SwarmDb.GetDatabaseForReading().GetPeopleGeographies();
        }

        public static People FromIdentities (int[] personIds, bool preserveOrder)
        {
            People set = FromIdentities (personIds);

            if (preserveOrder == false)
            {
                return set;
            }

            Dictionary<int, Person> lookup = new Dictionary<int, Person>();

            foreach (Person person in set)
            {
                lookup[person.Identity] = person;
            }

            People result = new People();

            foreach (int identity in personIds)
            {
                result.Add (lookup[identity]);
            }

            return result;
        }

        public Type SingularType { get { return typeof(Person); } }
        public Type PluralType { get { return typeof(People); } }
    }
}