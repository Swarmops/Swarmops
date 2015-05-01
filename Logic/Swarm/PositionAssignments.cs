using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class PositionAssignments: PluralBase<PositionAssignments, PositionAssignment, BasicPositionAssignment>
    {
        public static PositionAssignments ForPosition (Position position, bool includeTerminated = false)
        {
            if (position.PositionLevel == PositionLevel.GeographyDefault ||
                position.PositionLevel == PositionLevel.Geography)
            {
                // a bit complex; we need to ask the database directly for these geographies

                if (!includeTerminated)
                {
                    return
                        FromArray (
                            SwarmDb.GetDatabaseForReading()
                                .GetPositionAssignments (position, position.Geography,
                                    DatabaseCondition.ActiveTrue));
                }

                // otherwise, if asking for all assignments (even clsoed), remove "DatabaseCondition.ActiveTrue"

                return
                    FromArray(
                        SwarmDb.GetDatabaseForReading()
                            .GetPositionAssignments(position, position.Geography));

            }

            // otherwise, global (geo-unbound) position

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

        public static PositionAssignments ForPerson (Person person, bool includeTerminated = false)
        {
            if (!includeTerminated)
            {
                return FromArray (SwarmDb.GetDatabaseForReading().GetPositionAssignments (person, DatabaseCondition.ActiveTrue));
            }
            return FromArray(SwarmDb.GetDatabaseForReading().GetPositionAssignments(person));
        }
    }
}
