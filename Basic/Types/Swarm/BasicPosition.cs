using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Swarm
{
    public class BasicPosition: IHasIdentity, IHasParentIdentity
    {
        public BasicPosition(
            int positionId, PositionLevel positionLevel, int organizationId, int geographyId, int overridesHigherPositionId,
            int createdByPersonId, int createdByPositionId, DateTime createdDateTimeUtc, string positionTypeName, string positionTitleName,
            bool inheritsDownward, bool active, bool volunteerable, bool overridable, bool covert, 
            int reportsToPositionId, int dotReportsToPositionId, int minCount, int maxCount)
        {
            // Mind the order, for maintainability and risk reduction. Same order everywhere - 
            // here and in database and in Swarmops.Database, in groups of five.

            this.PositionId = positionId;
            this.PositionLevel = positionLevel;
            this.OrganizationId = organizationId;
            this.GeographyId = geographyId;
            this.OverridesHigherPositionId = overridesHigherPositionId;

            this.CreatedByPersonId = createdByPersonId;
            this.CreatedByPositionId = createdByPositionId;
            this.CreatedDateTimeUtc = createdDateTimeUtc;
            this.PositionTypeName = positionTypeName;
            this.PositionTitleName = positionTitleName;

            this.InheritsDownward = inheritsDownward;
            this.Active = active;
            this.Volunteerable = volunteerable;
            this.Overridable = overridable;
            this.Covert = covert;

            this.ReportsToPositionId = reportsToPositionId;
            this.DotReportsToPositionId = dotReportsToPositionId;
            this.MinCount = minCount;
            this.MaxCount = maxCount;
        }

        public BasicPosition (BasicPosition original)
            : this (
                original.PositionId, original.PositionLevel, original.OrganizationId, original.GeographyId, original.OverridesHigherPositionId,
                original.CreatedByPersonId, original.CreatedByPositionId, original.CreatedDateTimeUtc, original.PositionTypeName, original.PositionTitleName,
                original.InheritsDownward, original.Active, original.Volunteerable, original.Overridable, original.Covert, 
                original.ReportsToPositionId, original.DotReportsToPositionId, original.MinCount, original.MaxCount)
        {
            // copy ctor
        }

        public int PositionId { get; private set; }
        public PositionLevel PositionLevel { get; private set; }
        public int OrganizationId { get; private set; }
        public int GeographyId { get; private set; }
        public int OverridesHigherPositionId { get; protected set; }

        public int CreatedByPersonId { get; private set; }
        public int CreatedByPositionId { get; private set; }
        public DateTime CreatedDateTimeUtc { get; private set; }
        public string PositionTypeName { get; private set; }
        public string PositionTitleName { get; private set; } // strings not enums; plugins may extend from stock enum

        public bool InheritsDownward { get; protected set; }
        public bool Active { get; protected set; }
        public bool Volunteerable { get; protected set; }
        public bool Overridable { get; protected set; }
        public bool Covert { get; protected set; }

        public int ReportsToPositionId { get; protected set; }
        public int DotReportsToPositionId { get; protected set; }
        public int MinCount { get; protected set; }
        public int MaxCount { get; protected set; }


        public int Identity { get { return this.PositionId; } }
        public int ParentIdentity { get { return this.ReportsToPositionId; } }
    }
}
