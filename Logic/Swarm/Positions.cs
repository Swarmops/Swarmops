using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin.Protocol;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Support;
using Swarmops.Common.Generics;

namespace Swarmops.Logic.Swarm
{
    public class Positions: PluralBase<Positions, Position, BasicPosition>
    {
        public static Positions ForSystem()
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetPositions (PositionLevel.SystemWide));
        }

        public PositionAssignments Assignments
        {
            get { return PositionAssignments.ForPositions(this); }
        }

        public static Positions GetChildren (Position forPosition)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetPositionChildren (forPosition.Identity));
        }

        public static void CreateSysadminPositions()
        {
            // first, verify there aren't actually any sysadmin positions. Defensive coding for the win.

            if (Positions.ForSystem().Count > 0)
            {
                throw new InvalidOperationException ("Can't initialize sysadmin positions - already there");
            }

            // Ok, so there are zero system-level positions. Create the Sysadmin positions.

            Position sysadminPrincipal = Position.Create (PositionLevel.SystemWide, null /* createdByPerson*/, null /*createdByPosition*/, PositionType.System_SysadminMain,
                PositionTitle.Default, false /*volunteerable*/, false /*overridable*/, null /*reportsTo*/, null /*dotReportsTo*/, 1 /*minCount*/, 1 /*maxCount*/);

            Position.Create(PositionLevel.SystemWide, null /* createdByPerson*/, null /*createdByPosition*/, PositionType.System_SysadminReadWrite, PositionTitle.Default,
                false, false, sysadminPrincipal, null /*dotReportsTo*/, 0 /*minCount*/, 0 /*maxCount*/);

            Position.Create(PositionLevel.SystemWide, null /* createdByPerson*/, null /*createdByPosition*/, PositionType.System_SysadminAssistantReadOnly, PositionTitle.Default,
                false, false, sysadminPrincipal, null /*dotReportsTo*/, 0 /*minCount*/, 0 /*maxCount*/);

            // If there's exactly one person in the system, we're undergoing Setup, so assign to Sysadmin Principal position.
            // Otherwise let grandfathering code handle it.

            People allPeople = People.GetAll();
            // calling People.GetAll() would be a killer on well-built-out systems, but this code only runs once, and in Setup

            if (allPeople.Count == 1)
            {
                sysadminPrincipal.Assign (allPeople[0], null, null, "Assigned initial sysadmin", null);
            }
        }

        public Tree<Position> Tree { get { return Tree<Position>.FromCollection (this); }}
    }
}
