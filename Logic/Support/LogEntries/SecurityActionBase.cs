using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class SecurityActionBase: LogEntryBase<SecurityActionBase>
    {
        public int OrganizationId { get; set; }
        public int ActingPersonId { get; set; }
        public int ConcernedPersonId { get; set; }
        public string RemoteIPAddresses { get; set; }
        /// <summary>
        /// Always in UTC
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}
