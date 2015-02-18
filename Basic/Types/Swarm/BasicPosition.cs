using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Swarm
{
    public class BasicPosition: IHasIdentity
    {
        public BasicPosition(int positionId, int organizationId, PositionLevel positionLevel, string positionTypeName, string positionTitleName, 
            int geographyId, bool overridable, int overridesHigherPositionId, int createdByPersonId, DateTime createdDateTimeUtc,
            bool inheritsDownward, bool volunteerable, bool active, bool covert, int reportsToPositionId,
            int dotReportsToPositionId, int minCount, int maxCount)
        {
            this.PositionId = positionId;
            this.OrganizationId = organizationId;
            this.PositionLevel = positionLevel;
            this.PositionTypeName = positionTypeName;
            this.PositionTitleName = positionTitleName;
            this.GeographyId = geographyId;
            this.Overridable = overridable;
            this.OverridesHigherPositionId = overridesHigherPositionId;
            this.CreatedByPersonId = createdByPersonId;
            this.CreatedDateTimeUtc = createdDateTimeUtc;
            this.PositionTypeName = positionTypeName;
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
                original.PositionId, original.OrganizationId, original.PositionLevel, original.PositionTypeName, original.PositionTitleName, 
                original.GeographyId, original.Overridable, original.OverridesHigherPositionId, original.CreatedByPersonId, original.CreatedDateTimeUtc,
                original.InheritsDownward, original.Volunteerable, original.Active,
                original.Covert, original.ReportsToPositionId, original.DotReportsToPositionId, original.MinCount,
                original.MaxCount)
        {
            // copy ctor
        }

        public int PositionId { get; private set; }
        public PositionLevel PositionLevel { get; private set; }
        public string PositionTypeName { get; private set; }
        public string PositionTitleName { get; private set; } // strings not enums; plugins may extend from stock enum
        public int OrganizationId { get; private set; }
        public int GeographyId { get; private set; }
        public bool Overridable { get; protected set; }
        public int OverridesHigherPositionId { get; protected set; }
        public int CreatedByPersonId { get; private set; }
        public DateTime CreatedDateTimeUtc { get; private set; }
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
