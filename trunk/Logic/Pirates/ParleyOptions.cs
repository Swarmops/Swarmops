using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Pirates
{
    public class ParleyOptions: PluralBase<ParleyOptions,ParleyOption,BasicParleyOption>
    {
        public static ParleyOptions ForParley (Parley parley)
        {
            return ForParley(parley, false);
        }

        public static ParleyOptions ForParley (Parley parley, bool includeInactive)
        {
            if (includeInactive)
            {
                return FromArray(PirateDb.GetDatabase().GetParleyOptions(parley));
            }
            else
            {
                return FromArray(PirateDb.GetDatabase().GetParleyOptions(parley, DatabaseCondition.ActiveTrue));
            }
        }

        public static ParleyOptions ForParleyAttendee (ParleyAttendee attendee)
        {
            int[] parleyOptionIds = PirateDb.GetDatabase().GetParleyAttendeeOptions(attendee.Identity);

            return FromIdentities(parleyOptionIds);
        }

        public static ParleyOptions FromIdentities (int[] identities)
        {
            ParleyOptions result = new ParleyOptions();

            foreach (int parleyId in identities)
            {
                result.Add(ParleyOption.FromIdentity(parleyId));
            }

            return result;
        }
    }
}
