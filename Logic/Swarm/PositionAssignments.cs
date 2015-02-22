using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class PositionAssignments: PluralBase<PositionAssignments, PositionAssignment, BasicPositionAssignment>
    {
        public static PositionAssignments ForPosition (Position position, bool includeTerminated = false)
        {
            return ForPositions (Positions.FromSingle (position), includeTerminated);
        }

        public static PositionAssignments ForPositions (Positions positions, bool includeTerminated = false)
        {
            if (!includeTerminated)
            {
                return FromArray(SwarmDb.GetDatabaseForReading().GetPositionAssignments(positions, DatabaseCondition.ActiveTrue));
            }

            return FromArray (SwarmDb.GetDatabaseForReading().GetPositionAssignments (positions));
        }
    }
}
