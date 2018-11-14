using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Basic.Types.Common
{
    public enum ReportRequirement
    {
        Unknown = 0,
        /// <summary>
        /// a report (of the discussed type) is required for this time period
        /// </summary>
        Required,
        /// <summary>
        /// a report (of the discussed type) is NOT required for this time period
        /// </summary>
        NotRequired,
        /// <summary>
        /// A report (of the discussed type) has already been completed for this time period
        /// </summary>
        Completed
    }
}
