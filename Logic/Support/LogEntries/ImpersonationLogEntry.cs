using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class ImpersonationLogEntry: LogEntryBase<ImpersonationLogEntry>
    {
        public int ImpersonatorPersonId;
        public bool Started; // false means it ended at this mark
    }
}
