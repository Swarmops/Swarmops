using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Swarm
{
    public class PositionAssignment: BasicPositionAssignment
    {
        private PositionAssignment() : base (null)
        {
            // do not call private null ctor
        }

        private PositionAssignment (BasicPositionAssignment basic) : base (basic)
        {
            // private ctor
        }

        public static PositionAssignment FromBasic (BasicPositionAssignment basic)
        {
            return new PositionAssignment (basic); // calls private ctor
        }

        public static PositionAssignment FromIdentity (int positionAssignmentId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetPositionAssignment (positionAssignmentId));
        }

        public static PositionAssignment FromIdentityAggressive (int positionAssignmentId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetPositionAssignment(positionAssignmentId)); // "ForWriting" is intentional - avoids race conditions in Create()
        }


        public Person Person
        {
            get { return Person.FromIdentity (base.PersonId); }
        }

        public Position Position
        {
            get { return Position.FromIdentity (base.PositionId); }
        }

        public Geography Geography
        {
            get { return base.GeographyId == 0 ? null : Geography.FromIdentity (base.GeographyId); }
        }


        public static PositionAssignment Create(Position position, Geography geography, 
            Person person, Person createdByPerson, Position createdByPosition, DateTime? expiresDateTimeUtc,
            string assignmentNotes)
        {
            int geographyId = 0;

            if (position.PositionLevel == PositionLevel.Geography || position.PositionLevel == PositionLevel.GeographyDefault)
            {
                geographyId = geography.Identity;
            }
            else
            {
                if (geography != null)
                {
                    throw new ArgumentException ("Geography cannot be defined when position is global");
                }
            }

            int createdByPersonId = createdByPerson == null ? 0 : createdByPerson.Identity;
            int createdByPositionId = createdByPosition == null ? 0 : createdByPosition.Identity;

            // TODO: Verify constraints of max/min counts for position

            int positionAssignmentId = SwarmDb.GetDatabaseForWriting()
                .CreatePositionAssignment (position.OrganizationId, geographyId, position.Identity, person.Identity,
                    createdByPersonId, createdByPositionId,
                    expiresDateTimeUtc == null ? Constants.DateTimeHigh : (DateTime) expiresDateTimeUtc, assignmentNotes);  // DateTime.MaxValue kills MySql layer

            return FromIdentityAggressive (positionAssignmentId);
        }

        public void Terminate (Person terminatingPerson, Position terminatingPosition, string terminationNotes)
        {
            // TODO: ADD ACCESS CONTROL AT LOGIC LAYER

            SwarmDb.GetDatabaseForWriting()
                .TerminatePositionAssignment (this.Identity, terminatingPerson.Identity, terminatingPosition.Identity,
                    terminationNotes);

            base.TerminatedByPersonId = terminatingPerson.Identity;
            base.TerminatedByPositionId = terminatingPosition.Identity;
            base.Active = false;
            base.TerminationNotes = terminationNotes;
            base.TerminatedDateTimeUtc = DateTime.UtcNow; // may differ by milliseconds from actual value set, but shouldn't matter for practical purposes
        }

    }
}
