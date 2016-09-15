using System;

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
