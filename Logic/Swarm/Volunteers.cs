using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Swarm
{
    [Serializable]
    public class Volunteers : List<Volunteer>
    {
        public static Volunteers GetOpen()
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetOpenVolunteers());
        }

        public static Volunteers FromArray (BasicVolunteer[] basicArray)
        {
            Volunteers result = new Volunteers();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicVolunteer basic in basicArray)
            {
                result.Add (Volunteer.FromBasic (basic));
            }

            return result;
        }

        public static Volunteers FromSingle (Volunteer volunteer)
        {
            Volunteers volunteers = new Volunteers();
            volunteers.Add (volunteer);

            return volunteers;
        }

        public void Remove (Volunteers volunteersToRemove)
        {
            Dictionary<int, bool> identityLookup = new Dictionary<int, bool>();

            foreach (Volunteer volunteer in volunteersToRemove)
            {
                identityLookup[volunteer.Identity] = true;
            }

            int index = 0;

            while (index < Count)
            {
                if (identityLookup.ContainsKey (this[index].Identity))
                {
                    // This volunteer should be removed from the list.

                    RemoveAt (index);
                }
                else
                {
                    // If nothing was removed, increment index.

                    index++;
                }
            }
        }

        public new void Remove (Volunteer volunteerToRemove)
        {
            Remove (FromSingle (volunteerToRemove));
        }

        public DataSet ToDataSet()
        {
            // This function takes a detour around in-memory XML for maintainability of this code.
            // It's possible to write it more optimized, and build the DataSet by hand, but that takes
            // around 100 LoC.

            XmlSerializer serializer = new XmlSerializer (typeof (Volunteers));
            StringWriter sw = new StringWriter();
            serializer.Serialize (sw, this);
            DataSet ds = new DataSet();
            ds.ReadXml (sw.ToString());

            return ds;
        }
    }
}