using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Swarm
{
    public class BasicExternalActivity : IHasIdentity
    {
        public BasicExternalActivity (int externalActivityId, int organizationId, int geographyId,
            ExternalActivityType type, DateTime dateTime, string description, int createdByPersonId,
            DateTime createdDateTime,
            int dupeOfActivityId)
        {
            this.ExternalActivityId = externalActivityId;
            this.OrganizationId = organizationId;
            this.GeographyId = geographyId;
            this.Type = type;
            this.DateTime = dateTime;
            this.Description = description;
            this.CreatedByPersonId = createdByPersonId;
            this.CreatedDateTime = createdDateTime;
            this.DupeOfActivityId = dupeOfActivityId;
        }

        public BasicExternalActivity (BasicExternalActivity original) :
            this (
            original.ExternalActivityId, original.OrganizationId, original.GeographyId, original.Type, original.DateTime,
            original.Description, original.CreatedByPersonId, original.CreatedDateTime, original.DupeOfActivityId)
        {
            // empty copy ctor
        }

        public int ExternalActivityId { get; private set; }
        public int OrganizationId { get; private set; }
        public int GeographyId { get; private set; }
        public ExternalActivityType Type { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Description { get; private set; }
        public int CreatedByPersonId { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public int DupeOfActivityId { get; protected set; }

        public int Identity
        {
            get { return this.ExternalActivityId; }
        }
    }
}