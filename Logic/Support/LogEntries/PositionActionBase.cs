using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.Expressions;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class PositionActionBase : LogEntryBase<PositionActionBase>
    {
        public int PositionId { get; set; }
        public int ActingPersonId { get; set; }
        public int ConcernedPersonId { get; set; }
        public DateTime DateTime { get; set; }  // Always UTC
    }
}
