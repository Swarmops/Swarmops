using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Common.Generics;
using Swarmops.Database;
using Swarmops.Logic.Resources;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Swarm
{
    public class Position: BasicPosition
    {
        [Obsolete("Do not use the parameterless constructor. Use Create() or read from database.", true)]
        private Position() : base (null)
        {
            // do not use
        }

        private Position (BasicPosition basic) : base (basic)
        {
            // private ctor
        }

        public static Position FromBasic (BasicPosition basic)
        {
            return new Position (basic); // accesses private ctor
        }

        public static Position FromIdentity (int positionId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetPosition (positionId));
        }

        public static Position FromIdentityAggressive (int positionId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetPosition (positionId));
        }



        public static Position Create(PositionLevel level, Person createdByPerson, Position createdByPosition, PositionType positionType,
            PositionTitle positionTitle, bool volunteerable, bool overridable, Position reportsTo, Position dotReportsTo,
            int minCount, int maxCount)
        {
            if (level != PositionLevel.SystemWide)
            {
                throw new ArgumentException (
                    "Can only create system-wide positions (e.g. sysadmin) with this organizationless Position.Create() version.");
            }

            // Use more general Create() do to the actual work

            return Create (null, level, createdByPerson, createdByPosition, positionType, positionTitle, volunteerable,
                overridable, reportsTo, dotReportsTo, minCount, maxCount);
        }

        public static Position Create(Organization organization, PositionLevel level, Person createdByPerson, Position createdByPosition, PositionType positionType,
            PositionTitle positionTitle, bool volunteerable, bool overridable, Position reportsTo, Position dotReportsTo,
            int minCount, int maxCount)
        {
            if (level == PositionLevel.Geography)
            {
                throw new ArgumentException(
                    "Cannot use the geographyless creator to create geography-specific positions.");
            }

            if (organization == null && level != PositionLevel.SystemWide)
            {
                throw new ArgumentException("Cannot use null organization with any other level than PositionLevel.SystemWide");
            }

            int positionId = SwarmDb.GetDatabaseForWriting()
                .CreatePosition(
                    level, organization == null? 0 : organization.Identity, 0 /* geographyId */, 0 /*overridesHigherId */,
                    createdByPerson == null ? 0 : createdByPerson.Identity, createdByPosition == null ? 0 : createdByPosition.Identity,
                    positionType.ToString(), positionTitle.ToString(),
                    false /*inheritsDownward*/, volunteerable, overridable,
                    reportsTo == null ? 0 : reportsTo.Identity,
                    dotReportsTo == null ? 0 : dotReportsTo.Identity,
                    minCount, maxCount);

            return FromIdentityAggressive(positionId);
        }


        public PositionAssignment Assign(Person person, Geography geography, Person assignedByPerson, Position assignedByPosition, string assignmentNotes, DateTime? expiresUtc)
        {
            return PositionAssignment.Create (this, geography, person, assignedByPerson,
                assignedByPosition, expiresUtc, assignmentNotes);
        }

        public PositionAssignment Assign(Person person, Person assignedByPerson, Position assignedByPosition,
            string assignmentNotes, DateTime? expiresUtc)
        {
            return PositionAssignment.Create(this, null, person, assignedByPerson,
                assignedByPosition, expiresUtc, assignmentNotes);
        }

        public Organization Organization
        {
            get { return base.OrganizationId == 0 ? null : Organization.FromIdentity (base.OrganizationId); }
        }

        public Geography Geography
        {
            get { return base.GeographyId == 0 ? null : Geography.FromIdentity (base.GeographyId); }
        }


        public PositionType PositionType
        {
            get { return (PositionType) Enum.Parse (typeof (PositionType), this.PositionTypeName); }
        }

        public PositionTitle PositionTitle
        {
            get { return (PositionTitle) Enum.Parse (typeof (PositionTitle), this.PositionTitleName); }
        }


        public static Position RootSysadmin
        {
            // Gets the root position.
            get
            {
                Positions systemLevelPositions = Positions.ForSystem();

                foreach (Position position in systemLevelPositions)
                {
                    if (position.PositionType == PositionType.System_SysadminMain)
                    {
                        return position;
                    }
                }

                throw new InvalidOperationException("No root sysadmin position defined - this is an invalid state");
            }
        }

        public Positions Children
        {
            get { return Positions.GetChildren (this); }
        }


        public void AssignGeography (Geography geography)
        {
            // This fakes a geography base. Used for locking a position to a specific geo. Not good architecture as it
            // creates a different meaning than the database one.

            base.GeographyId = geography.Identity;
        }


        public PositionAssignments Assignments
        {
            get { return PositionAssignments.ForPosition (this); }
        }


        public string Localized(bool plural = false)
        {
            if (this.PositionTitle == PositionTitle.Custom)
            {
                return SwarmDb.GetDatabaseForReading().GetPositionCustomTitle(this.Identity);
            }

            return Position.Localized (this.PositionType, this.PositionTitle, plural);
        }


        public static string Localized(PositionType type, bool plural = false)
        {
            string titleString = type.ToString();
            if (plural)
            {
                titleString += "_Plural";
            }

            return Logic_Swarm_Position.ResourceManager.GetString("Position_" + titleString);
        }

        public static string Localized(PositionType type, PositionTitle title, bool plural = false)
        {
            if (title == PositionTitle.Custom)
            {
                throw new InvalidOperationException ("Can't fetch generic name for customized title");
            }

            string titleStringUntyped = type.ToString();
            string titleStringTyped = titleStringUntyped + "_" + title.ToString();

            if (plural)
            {
                titleStringUntyped += "_Plural";
                titleStringTyped += "_Plural";
            }

            string testResult = Logic_Swarm_Position.ResourceManager.GetString("Position_" + titleStringTyped);

            if (!String.IsNullOrEmpty (testResult))
            {
                return testResult;
            }

            return Logic_Swarm_Position.ResourceManager.GetString("Position_" + titleStringUntyped);
        }


        public bool HasAccess(Access access)
        {
            // TODO TODO TODO

            if (access == null)
            {
                throw new ArgumentNullException ("access", @"Access requested must always be specified. Use AccessAspect.Null if null access is desired (and access should always be true).");
            }

            if (this.PositionLevel == PositionLevel.SystemWide)
            {
                if (access.Type == AccessType.Write &&
                    this.PositionType == PositionType.System_SysadminAssistantReadOnly)
                {
                    return false;
                }

                return true;
            }

            if (this.PositionLevel == PositionLevel.GeographyDefault || this.PositionLevel == PositionLevel.Geography)
            {
                // This Position.GeographyId has been set to the Assignments's GeographyId for this purpose
                // (bad architecture, fix)

                if (access.Organization.IsOrInherits (this.Organization) &&
                    access.Geography.IsOrInherits (this.Geography))
                {
                    // we have an Access with applicable Org and Geo, but is the Access good enough? Cheat for now.

                    if (this.PositionType == PositionType.Geographic_Leader ||
                        this.PositionType == PositionType.Geographic_Deputy ||
                        this.PositionType == PositionType.Geographic_Assistant)
                    {
                        // yes, we have admin privileges and all other privileges too

                        return true;
                    }
                }

                // otherwise FO
                return false;
            }

            // the positions left are at org level

            if (access.Organization.IsOrInherits (this.OrganizationId))
            {
                // is the Position privilege sufficient to meet the Access required?
                // for now, cheat and assume all positions at the Org level have Admin access

                return true;
            }

            return false;
        }
    }
}
