using System.ComponentModel;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.DataObjects
{
#if !__MonoCS__
    [DataObject (true)]
#endif
    public class MembershipsDataObject
    {
#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static Participation[] Select (int personId)
        {
            return
                Participations.FromArray (SwarmDb.GetDatabaseForReading().GetParticipations (Person.FromIdentity (personId)))
                    .ToArray();
        }

#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static Participation[] Select (Person person)
        {
            return Select (person.Identity);
        }

#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static Participation[] SelectStatic (Participations participations)
        {
            return participations.ToArray();
        }
    }
}