using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common
{
    public class Constants
    {
        // These constants are necessary because MySql keeps vomiting on Constants.DateTimeLow and DateTime.MaxValue, so we
        // need some other definition of endpoints of the valid DateTime space

        public static DateTime DateTimeLow
        {  get { return new DateTime(1800, 1, 1); } }

        public static DateTime DateTimeLowThreshold
        {
            get { return new DateTime(1801, 1, 1); }
        }

        /// <summary>
        /// Used to set a high date
        /// </summary>
        public static DateTime DateTimeHigh
        { get { return new DateTime(2300, 12, 1); } }

        /// <summary>
        /// Used in comparisons to see if date is high
        /// </summary>
        public static DateTime DateTimeHighThreshold
        { get { return new DateTime(2299, 1, 1); } }
    }
}
