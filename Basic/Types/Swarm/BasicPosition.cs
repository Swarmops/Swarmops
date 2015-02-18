using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Swarm
{
    public class BasicPosition: IHasIdentity
    {
        public BasicPosition(int positionId, int organizationId, int positionLevel, int geographyId, bool overridable, 
            int overridesHigherPositionId, int createdByPersonId, DateTime createdDateTimeUtc, int positionTypeId,
            bool inheritsDownward, bool volunteerable, bool active, bool covert, int reportsToPositionId,
            int dotReportsToPositionId, int minCount, int maxCount)
        {
            this.PositionId = positionId;
            this.OrganizationId = organizationId;
            this.PositionLevel = positionLevel;
            this.GeographyId = geographyId;
            this.Overridable = overridable;
            this.OverridesHigherPositionId = overridesHigherPositionId;
            this.CreatedByPersonId = createdByPersonId;
            this.CreatedDateTimeUtc = createdDateTimeUtc;
            this.PositionTypeId = positionTypeId;
            this.InheritsDownward = inheritsDownward;
            this.Volunteerable = volunteerable;
            this.Active = active;
            this.Covert = covert;
            this.ReportsToPositionId = reportsToPositionId;
            this.DotReportsToPositionId = dotReportsToPositionId;
            this.MinCount = minCount;
            this.MaxCount = maxCount;
        }

        public BasicPosition (BasicPosition original)
            : this (
                original.PositionId, original.OrganizationId, original.PositionLevel, original.GeographyId, original.Overridable,
                original.OverridesHigherPositionId, original.CreatedByPersonId, original.CreatedDateTimeUtc, original.PositionTypeId,
                original.InheritsDownward, original.Volunteerable, original.Active,
                original.Covert, original.ReportsToPositionId, original.DotReportsToPositionId, original.MinCount,
                original.MaxCount)
        {
            // copy ctor
        }

        public int PositionId { get; private set; }
        public int PositionLevel { get; private set; }
        public int OrganizationId { get; private set; }
        public int GeographyId { get; private set; }
        public bool Overridable { get; protected set; }
        public int OverridesHigherPositionId { get; protected set; }
        public int CreatedByPersonId { get; private set; }
        public DateTime CreatedDateTimeUtc { get; private set; }
        public int PositionTypeId { get; private set; }
        public bool InheritsDownward { get; protected set; }
        public bool Volunteerable { get; protected set; }
        public bool Active { get; protected set; }
        public bool Covert { get; protected set; }
        public int ReportsToPositionId { get; protected set; }
        public int DotReportsToPositionId { get; protected set; }
        public int MinCount { get; protected set; }
        public int MaxCount { get; protected set; }

        public int Identity
        {
            get { return this.PositionId; }
        }
    }
}
