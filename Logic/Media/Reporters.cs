using System.Collections.Generic;

using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Media
{
    public class Reporters : List<Reporter>
    {
        public static Reporters FromArray (BasicReporter[] basicArray)
        {
            var result = new Reporters();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicReporter basic in basicArray)
            {
                result.Add (Reporter.FromBasic (basic));
            }

            return result;
        }

        public static Reporters GetAll()
        {
            BasicReporter[] basicReporters = PirateDb.GetDatabase().GetReporters();
            return FromArray (basicReporters);
        }

        public static Reporters FromMediaCategories (MediaCategories categories)
        {
            BasicReporter[] basicReporters =
                PirateDb.GetDatabase().GetReportersFromMediaCategories (categories.Identities);
            return FromArray (basicReporters);
        }
    }
}