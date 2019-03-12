using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class ApplicantAddedLogEntry: LogEntryBase<ApplicantAddedLogEntry>
    {
        [Obsolete("Do not call empty ctor directly. Intended for serialization only.", true)]
        public ApplicantAddedLogEntry()
        {
            // Do not call empty ctor directly. Intended for serialization only.
        }

        public ApplicantAddedLogEntry (Applicant newApplicant)
        {
            this.ApplicantId = newApplicant.Identity;
            if (HttpContext.Current != null)
            {
                ActingIPAddress = SupportFunctions.GetMostLikelyRemoteIPAddress();
            }
        }

        public int ApplicantId { get; set; }
        public string ActingIPAddress { get; set; }
    }
}
