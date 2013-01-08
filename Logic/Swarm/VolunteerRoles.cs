using System.Collections.Generic;
using Swarmops.Basic.Types;

namespace Swarmops.Logic.Swarm
{
    public class VolunteerRoles : List<VolunteerRole>
    {
        public static VolunteerRoles FromArray (BasicVolunteerRole[] basicArray)
        {
            var result = new VolunteerRoles();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicVolunteerRole basic in basicArray)
            {
                result.Add(VolunteerRole.FromBasic(basic));
            }

            return result;
        }
    }
}