using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common.ExtensionMethods
{
    public static partial class DummyWrapper
    {
        public static Int32 ToUnix (this DateTime source)
        {
            // Assumes DateTime is already UTC

            return (Int32)(source.Subtract(new DateTime(1970, 1, 1, 0, 0, 0,DateTimeKind.Utc))).TotalSeconds;
        }
    }
}
