using System.ComponentModel;
using Activizr.Logic.Pirates;
using Activizr.Database;

namespace Activizr.Logic.DataObjects
{
#if !__MonoCS__
    [DataObject (true)]
#endif
    public class MembershipsDataObject
    {
#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static Membership[] Select (int personId)
        {
            return Memberships.FromArray(PirateDb.GetDatabaseForReading().GetMemberships(Person.FromIdentity(personId))).ToArray();
        }

#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static Membership[] Select (Person person)
        {
            return Select (person.Identity);
        }

#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static Membership[] SelectStatic (Memberships memberships)
        {
            return memberships.ToArray();
        }
    }
}