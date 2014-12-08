using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class MembershipActionBase: LogEntryBase<MembershipActionBase>
    {
        public MembershipActionBase()
        {
            // do not call public ctor directly. Intended for serialization only.
        }

        public int MembershipId { get; set; }
        /// <summary>
        /// Always in UTC
        /// </summary>
        public DateTime DateTime { get; set; }
        public int ActingPersonId { get; set; }
    }
}
