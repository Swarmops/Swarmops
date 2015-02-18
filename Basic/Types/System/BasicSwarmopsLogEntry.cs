using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.System
{
    public class BasicSwarmopsLogEntry : IHasIdentity
    {
        public BasicSwarmopsLogEntry (int swarmopsLogEntryId, int personId, DateTime dateTime, int entryTypeId,
            string entryXml)
        {
            this.SwarmopsLogEntryId = swarmopsLogEntryId;
            this.PersonId = personId;
            this.DateTime = dateTime;
            this.EntryTypeId = entryTypeId;
            this.EntryXml = entryXml;
        }

        public BasicSwarmopsLogEntry (BasicSwarmopsLogEntry original)
            : this (
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
            get { return this.SwarmopsLogEntryId; }
        }
    }
}