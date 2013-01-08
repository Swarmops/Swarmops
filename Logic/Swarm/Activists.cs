using System;
using System.Collections.Generic;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Pirates
{
    public class Activists
    {
        #region Creation and Construction

        private Activists (int[] personIds)
        {
            this.personIds = personIds;
        }

        #endregion

        private readonly int[] personIds;
        private People people;

        public int Count
        {
            get { return this.personIds.Length; }
        }
        
        public int[] Identities
        {
            get
            {
                return personIds;
            }
        }

        public People People
        {
            get
            {
                VerifyPeoplePopulated();

                return this.people;
            }
        }

        public static int GetCountForGeography (Geography geography)
        {
            Geographies geographies = geography.GetTree();

            return PirateDb.GetDatabaseForReading().GetActivistCountForGeographies(geographies.Identities);
        }

        public static Activists FromGeography (Geography geography)
        {
            Geographies geographies = geography.GetTree();
            return new Activists(PirateDb.GetDatabaseForReading().GetActivistPersonIds(geographies.Identities));
        }

        public void SendPhoneMessage (string message)
        {
            VerifyPeoplePopulated();

            /* Fix for Ticket #56 - Multiple sms'es to same activist */
            /* Changed List to Dictionary - performance boost - RF */
            Dictionary<string, bool> dupeCheck = new Dictionary<string, bool>();

            foreach (Person person in this.people)
            {
                // Check if we've already sent an sms to this phonenumber during this session
                if (!dupeCheck.ContainsKey(person.Phone))
                {
                    dupeCheck[person.Phone] = true;

                    try
                    {
                        person.SendPhoneMessage(message);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void SendNotice (string subject, string body)
        {
            VerifyPeoplePopulated();

            foreach (Person person in this.people)
            {
                person.SendNotice(subject, body, 1);
            }
        }


        private void VerifyPeoplePopulated ()
        {
            if (this.people == null)
            {
                this.people = People.FromIdentities(this.personIds);
            }
        }
    }
}