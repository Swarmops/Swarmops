using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class Participations : PluralBase<Participations, Participation, BasicParticipation>
    {

        public static int GetParticipantCountForOrganization (Organization organization)
        {
            return SwarmDb.GetDatabaseForReading().GetParticipantCountForOrganizations (new[] {organization.Identity});
        }

        public static int GetParticipantCountForOrganizations (int[] organizationIds)
        {
            return SwarmDb.GetDatabaseForReading().GetParticipantCountForOrganizations (organizationIds);
        }

        public static int GetParticipantCountForOrganizationSince (Organization org, DateTime sinceDateTime)
        {
            return GetParticipantCountForOrganizationSince (org.Identity, sinceDateTime);
        }

        public static int GetParticipantCountForOrganizationSince (int orgId, DateTime sinceDateTime)
        {
            return SwarmDb.GetDatabaseForReading()
                .GetParticipantCountForOrganizationsSince (new[] {orgId}, sinceDateTime);
        }

        public static int GetParticipantCountForOrganizations (Organizations organizations)
        {
            List<int> idArray = new List<int>();

            foreach (Organization organization in organizations)
            {
                idArray.Add (organization.Identity);
            }

            return GetParticipantCountForOrganizations (idArray.ToArray());
        }

        public static Participations ForOrganization (Organization organization, bool includeTerminated)
        {
            if (!includeTerminated)
            {
                return ForOrganization (organization);
            }

            return FromArray (SwarmDb.GetDatabaseForReading().GetParticipations (organization));
        }

        public static Participations ForOrganization (Organization organization)
        {
            return
                FromArray (SwarmDb.GetDatabaseForReading().GetParticipations (organization, DatabaseCondition.ActiveTrue));
        }

        public static Participations ForOrganizations (Organizations organizations, bool includeTerminated)
        {
            if (!includeTerminated)
            {
                return ForOrganizations (organizations);
            }

            return FromArray (SwarmDb.GetDatabaseForReading().GetParticipations (organizations));
        }

        public static Participations ForOrganizations (Organizations organizations)
        {
            return
                FromArray (SwarmDb.GetDatabaseForReading().GetParticipations (organizations, DatabaseCondition.ActiveTrue));
        }

        public static Participations GetExpiring (Organization organization, DateTime dateExpiry)
        {
            return GetExpiring (organization, dateExpiry, dateExpiry);
        }

        public static Participations GetExpired (Organization organization)
        {
            return
                FromArray (SwarmDb.GetDatabaseForReading().GetExpiringParticipations (organization, new DateTime(1800,1,1),
                    DateTime.Now));
        }

        public static Participations GetExpired (Organization organization, DateTime lowerBound, DateTime upperBound)
        {
            return
                FromArray (SwarmDb.GetDatabaseForReading()
                    .GetExpiringParticipations (organization, lowerBound, upperBound, DatabaseCondition.None));
        }

        public static Participations GetExpiring (Organization organization, DateTime lowerBound, DateTime upperBound)
        {
            return
                FromArray (SwarmDb.GetDatabaseForReading().GetExpiringParticipations (organization, lowerBound, upperBound));
        }


        public static Dictionary<int, List<BasicParticipation>> GetParticipationsForPeople (People people, int gracePeriod)
        {
            return SwarmDb.GetDatabaseForReading().GetParticipationsForPeople (people.Identities, gracePeriod);
        }
    }
}