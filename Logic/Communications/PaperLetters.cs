using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Support;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Communications
{
    public class PaperLetters: List<PaperLetter>
    {
        internal static PaperLetters FromArray(BasicPaperLetter[] basicArray)
        {
            // TODO: This function exists in too many places. It's GOT to be possible to make it generic,
            // through an interface or something like that.

            PaperLetters result = new PaperLetters() { Capacity = (basicArray.Length * 11 / 10) };

            foreach (BasicPaperLetter basic in basicArray)
            {
                result.Add(PaperLetter.FromBasic(basic));
            }

            return result;
        }

        public static PaperLetters ForOrganization (Organization organization)
        {
            return ForOrganization(organization.Identity);
        }

        public static PaperLetters ForOrganization (int organizationId)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetPaperLettersForOrganization(organizationId));
        }

        public static PaperLetters ForPerson (Person person)
        {
            return ForPerson(person.Identity);
        }

        public static PaperLetters ForPerson (int personId)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetPaperLettersForPerson(personId));
        }
    }
}
