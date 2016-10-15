using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class PersonalDataChangedLogEntry: LogEntryBase<PersonalDataChangedLogEntry>
    {
        public int AffectedPersonId;
        public int ActingPersonId;
        public string IpAddress;
        public string OldValue;
        public string NewValue;
        public string Field;  // freetext field: "Name", "Mail", and so on
    }
}
