using System;
using System.Collections.Generic;
using System.ComponentModel;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.DataObjects
{
#if !__MonoCS__
    [DataObject (true)]
#endif
    public class PeopleDataObject
    {
#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static Person[] Select (int[] personIds)
        {
            if (personIds == null)
            {
                return new Person[0];
            }

            return People.FromIdentities (personIds).ToArray();
        }

#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static Person[] SelectStatic (Person[] personArray)
        {
            if (personArray == null)
            {
                return new Person[0];
            }

            return personArray;
        }

#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static Person[] SelectStatic (People people)
        {
            if (people == null)
            {
                return new Person[0];
            }

            return people.ToArray();
        }


#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static Person[] SelectSortedStatic (People people, string sort)
        {
            if (people == null)
            {
                return new Person[0];
            }


            List<Person> pList = new List<Person>();
            Person[] foundPersons = SelectStatic (people);
            foreach (Person pers in foundPersons)
            {
                pList.Add (pers);
            }

            switch (sort)
            {
                case "PersonId":
                    pList.Sort (PersonIdComparison);
                    break;
                case "Name":
                    pList.Sort (NameComparison);
                    break;
                case "PostalCode":
                    pList.Sort (PostalCodeComparison);
                    break;
                case "CityName":
                    pList.Sort (CityComparison);
                    break;
                case "Birthdate":
                    pList.Sort (BirthdateComparison);
                    break;

                    // New sortexpressions
                case "Email":
                    pList.Sort (EmailComparison);
                    break;
                case "Phone":
                    pList.Sort (PhoneComparison);
                    break;
            }

            return pList.ToArray();
        }

        public static Comparison<Person> PersonIdComparison =
            delegate (Person p1, Person p2) { return p1.PersonId.CompareTo (p2.PersonId); };

        public static Comparison<Person> NameComparison =
            delegate (Person p1, Person p2) { return p1.Name.CompareTo (p2.Name); };

        public static Comparison<Person> CityComparison = delegate (Person p1, Person p2)
        {
            int cmpRes = p1.CityName.CompareTo (p2.CityName);
            if (cmpRes == 0)
                cmpRes = p1.Name.CompareTo (p2.Name);
            return cmpRes;
        };

        public static Comparison<Person> PostalCodeComparison = delegate (Person p1, Person p2)
        {
            int cmpRes = p1.PostalCode.CompareTo (p2.PostalCode);
            if (cmpRes == 0)
                cmpRes = p1.Name.CompareTo (p2.Name);
            return cmpRes;
        };

        public static Comparison<Person> BirthdateComparison =
            delegate (Person p1, Person p2) { return p1.Birthdate.CompareTo (p2.Birthdate); };

        // Added for more sorting opportunities
        public static Comparison<Person> EmailComparison = delegate (Person p1, Person p2)
        {
            int cmpRes = p1.Mail.CompareTo (p2.Mail);
            if (cmpRes == 0)
                cmpRes = p1.Name.CompareTo (p2.Name);
            return cmpRes;
        };

        public static Comparison<Person> PhoneComparison = delegate (Person p1, Person p2)
        {
            int cmpRes = p1.Phone.CompareTo (p2.Phone);
            if (cmpRes == 0)
                cmpRes = p1.Name.CompareTo (p2.Name);
            return cmpRes;
        };
    }
}