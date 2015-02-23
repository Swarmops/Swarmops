using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Swarm
{
    public class BasicPositionAssignment: IHasIdentity
    {
        public BasicPositionAssignment (int positionAssignmentId, int organizationId, int geographyId, int positionId, int personId,
            DateTime createdDateTimeUtc, int createdByPersonId, int createdByPositionId, bool active, DateTime expiresDateTimeUtc,
            DateTime terminatedDateTimeUtc, int terminatedByPersonId, int terminatedByPositionId, string assignmentNotes, 
            string terminationNotes)
        {
            this.PositionAssignmentId = positionAssignmentId;
            this.OrganizationId = organizationId;
            this.GeographyId = geographyId;
            this.PositionId = positionId;
            this.PersonId = personId;
            this.CreatedDateTimeUtc = createdDateTimeUtc;
            this.CreatedByPersonId = createdByPersonId;
            this.CreatedByPositionId = createdByPositionId;
            this.Active = active;
            this.ExpiresDateTimeUtc = expiresDateTimeUtc;
            this.TerminatedDateTimeUtc = terminatedDateTimeUtc;
            this.TerminatedByPersonId = terminatedByPersonId;
            this.TerminatedByPositionId = terminatedByPositionId;
            this.AssignmentNotes = assignmentNotes;
            this.TerminationNotes = terminationNotes;
        }

        public BasicPositionAssignment (BasicPositionAssignment original)
            : this (original.PositionAssignmentId, original.OrganizationId, original.GeographyId, original.PositionId,
                original.PersonId, original.CreatedDateTimeUtc, original.CreatedByPersonId, original.CreatedByPositionId, 
                original.Active, original.ExpiresDateTimeUtc, original.TerminatedDateTimeUtc, original.TerminatedByPersonId,
                original.TerminatedByPositionId, original.AssignmentNotes, original.TerminationNotes)
        {
            // copy ctor
        }

        public int PositionAssignmentId { get; private set; }
        public int OrganizationId { get; private set; }
        public int GeographyId { get; private set; }
        public int PositionId { get; private set; }
        public int PersonId { get; private set; }
        public DateTime CreatedDateTimeUtc { get; private set; }
        public int CreatedByPersonId { get; private set; }
        public int CreatedByPositionId { get; private set; }
        public bool Active { get; protected set; }
        public DateTime ExpiresDateTimeUtc { get; protected set; }
        public DateTime TerminatedDateTimeUtc { get; protected set; }
        public int TerminatedByPersonId { get; protected set; }
        public int TerminatedByPositionId { get; protected set; }
        public string AssignmentNotes { get; private set; }
        public string TerminationNotes { get; protected set; }

        public int Identity { get { return this.PositionAssignmentId; } }
    }
}
