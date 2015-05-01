using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin.Protocol;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Structure;
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

        public static Positions ForOrganization (Organization organization)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetPositions (organization));
        }

        public static Tree<Position> ForOrganizationGeography (Organization organization, Geography geography)
        {
            Positions result = new Positions();
            string localizedLeaderTitle = null;
            Position geographyLeaderTemplate = null;

            if (geography.Identity == Geography.RootIdentity)
            {
                result = ForOrganization (organization).AtLevel (PositionLevel.OrganizationExecutive);
            }
            else
            {
                result = ForOrganization (organization).AtLevel (PositionLevel.GeographyDefault);
                localizedLeaderTitle = result.Tree.RootNodes[0].Data.Localized(); // indexer will throw if no root nodes exist
                geographyLeaderTemplate = result.Tree.RootNodes[0].Data;

                // TODO: Apply custom geographic positions from executive level downward
            }
            foreach (Position geoPosition in result)
            {
                geoPosition.AssignGeography (geography); // used for template geographies
            }

            if (localizedLeaderTitle == null)
            {
                geographyLeaderTemplate =
                    ForOrganization (organization).AtLevel (PositionLevel.GeographyDefault).Tree.RootNodes[0].Data;
                localizedLeaderTitle = geographyLeaderTemplate.Localized();
            }

            // Add leader positions of the geographies immediately below (in X generations?).

            Tree<Position> treeResult = result.Tree;

            Geographies children = geography.Children;

            foreach (Geography childGeo in children)
            {
                // Create copies of the leader position and use as templates for the children

                Position childPosition = Position.FromBasic (geographyLeaderTemplate);
                childPosition.AssignGeography (childGeo);

                treeResult.RootNodes[0].AddChild (childPosition);
            }

            return treeResult;
        }

        public Positions AtLevel (PositionLevel level)
        {
            return FromArray (this.Where (position => position.PositionLevel == level).ToArray());
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

        public static void CreateOrganizationDefaultPositions (Organization organization, PositionTitle titleType = PositionTitle.Default)
        {
            // Verify this is indeed a bootstrap - must fail if there are already positions at this level

            if (Positions.ForOrganization (organization).Count > 0)
            {
                throw new InvalidOperationException("Can't re-initialize organization positions - already initialized");
            }

            /* STRATEGIC POSITIONS */

            Position chairperson = Position.Create (organization, PositionLevel.OrganizationStrategic, null, null,
                PositionType.Strategic_BoardChairperson, titleType, false, false, null, null, 1, 1);

            /*Position deputyChairs =*/ Position.Create (organization, PositionLevel.OrganizationStrategic, null, null,
                PositionType.Strategic_BoardChairpersonDeputy, titleType, false, false, chairperson, null, 0,
                0);

            /*Position members =*/ Position.Create (organization, PositionLevel.OrganizationStrategic, null, null,
                PositionType.Strategic_BoardMember, titleType, false, false, null, null, 0, 0);

            /*Position deputies =*/ Position.Create (organization, PositionLevel.OrganizationStrategic, null, null,
                PositionType.Strategic_BoardDeputy, titleType, false, false, null, null, 0, 0);

            Position auditors = Position.Create (organization, PositionLevel.OrganizationStrategic, null, null,
                PositionType.Strategic_Auditor, titleType, false, false, null, null, 1, 0);

            /*Position auditorDeputies =*/ Position.Create(organization, PositionLevel.OrganizationStrategic, null, null,
                PositionType.Strategic_AuditorDeputy, titleType, false, false, auditors, null, 0, 0);

            /*Position auditorAssistants =*/ Position.Create(organization, PositionLevel.OrganizationStrategic, null, null,
                PositionType.Strategic_AuditorAssistant, titleType, false, false, auditors, null, 0, 0);

            /*Position observers=*/ Position.Create (organization, PositionLevel.OrganizationStrategic, null, null,
                PositionType.Strategic_Observer, titleType, false, false, null, null, 0, 0);


            /* EXECUTIVE POSITIONS (positions at the "Global" geographic level) */

            Position ceo = Position.Create (organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_ChiefExecutiveOfficer, titleType, false, false, null, null, 1, 1);

            /*Position vp =*/ Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_CeoAssistant, titleType, false, false, ceo, null, 0, 0);

            /*Position vp =*/ Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_VicePresident, titleType, false, false, ceo, null, 0, 0);

            /*Position cto =*/
            Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_ChiefTechnologyOfficer, titleType, false, false, ceo, null, 0, 1);

            Position cmo = Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_ChiefMarketingOfficer, titleType, false, false, ceo, null, 0, 1);

            /*Position cmoAssistant =*/ Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_MarketingAssistant, titleType, false, false, cmo, null, 0, 0);

            Position cfo = Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_ChiefFinancialOfficer, titleType, false, false, ceo, null, 0, 1);

            /*Position cfoAssistant =*/ Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_CfoAssistant, titleType, false, false, cfo, null, 0, 0);

            /*Position cio =*/ Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_ChiefInformationOfficer, titleType, false, false, ceo, null, 0, 1);

            Position coo = Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_ChiefOperationsOfficer, titleType, false, false, ceo, null, 0, 1);

            Position chiefHr = Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_ChiefHrOfficer, titleType, false, false, ceo, null, 0, 1);

            /*Position staffAssistant =*/ Position.Create(organization, PositionLevel.OrganizationExecutive, null, null,
                PositionType.Executive_HrAssistant, titleType, false, false, chiefHr, null, 0, 0);


            /* LOCAL DEFAULT POSITIONS */

            Position localLeader = Position.Create (organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_Leader, titleType, true, false, null, null, 1, 1);

            /*Position localDeputies =*/ Position.Create(organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_Deputy, titleType, true, false, localLeader, null, 1, 0);

            /*Position localAssistants =*/ Position.Create(organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_Assistant, titleType, false, false, localLeader, null, 0, 0);

            Position localOutreachLead = Position.Create(organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_OutreachLeader, titleType, true, true, localLeader, cmo, 0, 1);

            /*Position localOutreachAssistant =*/ Position.Create (organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_OutreachLeaderAssistant, titleType, true, true, localOutreachLead, null, 0, 0);

            Position localActivismLead = Position.Create (organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_ActivismLeader, titleType, true, true, localLeader, coo, 0, 1);

            /*Position localActivismAssistant =*/ Position.Create (organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_ActivismLeaderAssistant, titleType, true, true, localActivismLead, null, 0, 0);

            Position localSwarmLead = Position.Create(organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_SwarmLeader, titleType, true, true, localLeader, chiefHr, 0, 1);

            /*Position localSwarmLeadAssistant =*/ Position.Create(organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_SwarmLeaderAssistant, titleType, true, true, localSwarmLead, null, 0, 0);

            Position localInfoLead = Position.Create(organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_InfoLeader, titleType, true, true, localLeader, chiefHr, 0, 1);

            /*Position localInfoLeadAssistant =*/ Position.Create(organization, PositionLevel.GeographyDefault, null, null,
                PositionType.Geographic_InfoLeaderAssistant, titleType, true, true, localInfoLead, null, 0, 0);


        }

        public Tree<Position> Tree { get { return Tree<Position>.FromCollection (this); }}
    }
}
