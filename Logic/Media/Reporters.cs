using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Media
{
    public class Reporters : List<Reporter>
    {
        public static Reporters FromArray (BasicReporter[] basicArray)
        {
            Reporters result = new Reporters();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicReporter basic in basicArray)
            {
                result.Add (Reporter.FromBasic (basic));
            }

            return result;
        }

        public static Reporters GetAll()
        {
            BasicReporter[] basicReporters = SwarmDb.GetDatabaseForReading().GetReporters();
            return FromArray (basicReporters);
        }

        public static Reporters FromMediaCategories (MediaCategories categories)
        {
            BasicReporter[] basicReporters =
                SwarmDb.GetDatabaseForReading().GetReportersFromMediaCategories (categories.Identities);
            return FromArray (basicReporters);
        }
    }
}