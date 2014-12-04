using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicSwarmopsLogEntry : IHasIdentity
    {
        public BasicSwarmopsLogEntry(int swarmopsLogEntryId, int personId, DateTime dateTime, int entryTypeId,
            string entryXml)
        {
            SwarmopsLogEntryId = swarmopsLogEntryId;
            PersonId = personId;
            DateTime = dateTime;
            EntryTypeId = entryTypeId;
            EntryXml = entryXml;
        }

        public BasicSwarmopsLogEntry(BasicSwarmopsLogEntry original)
            : this(
                original.SwarmopsLogEntryId, original.PersonId, original.DateTime, original.EntryTypeId,
                original.EntryXml)
        {
            // copy ctor
        }

        public int SwarmopsLogEntryId { get; private set; }
        public int PersonId { get; private set; }
        public DateTime DateTime { get; private set; }
        public int EntryTypeId { get; private set; }
        public string EntryXml { get; private set; }

        public int Identity
        {
            get { return SwarmopsLogEntryId; }
        }
    }
}