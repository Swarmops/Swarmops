using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MySql.Data.Types;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        /*
        public int[] GetMembersForOrganization (int organizationId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT PersonId From Memberships WHERE Active=1 AND OrganizationId=" +
                        organizationId.ToString(), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    return ReadIntegersAtZeroFromDataReader(reader);
                }
            }
        }*/

        private const string membershipFieldSequence =
            " Memberships.MembershipId, Memberships.PersonId, Memberships.OrganizationId, Memberships.Active, Memberships.Expires, " +
            // 0-4
            " Memberships.MemberSince, Memberships.DateTimeTerminated, Memberships.TerminatedAsInvalid " + // 5-7
            " FROM Memberships ";

        private const string membershipPaymentStatusFieldSequence =
            " MembershipPayments.MembershipId, MembershipPayments.MembershipPaymentStatusId, MembershipPayments.StatusDateTime " +
            " FROM MembershipPayments ";

        public int[] GetMembersForOrganizations (int[] organizationIds)
        {
            if (organizationIds == null || organizationIds.Length == 0)
            {
                return new int[0];
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT PersonId From Memberships WHERE Active=1 AND OrganizationId IN (" +
                        JoinIds (organizationIds) + ")", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    return ReadIntegersAtZeroFromDataReader (reader);
                }
            }
        }


        public int[] GetParticipantsForOrganizationsAndGeographies (int[] organizationIds, int[] geographyIds)
        {
            if (organizationIds == null || organizationIds.Length == 0)
            {
                return new int[0];
            }

            if (geographyIds == null || geographyIds.Length == 0)
            {
                return new int[0];
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand (
                    "select DISTINCT People.PersonId from Memberships,People WHERE " +
                    "People.PersonId=Memberships.PersonId AND Memberships.Active=1 AND " +
                    "Memberships.OrganizationId IN (" + JoinIds (organizationIds) + ") AND " +
                    "People.GeographyId IN (" + JoinIds (geographyIds) + ")", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    return ReadIntegersAtZeroFromDataReader (reader);
                }
            }
        }


        public BasicParticipation[] GetParticipations (params object[] conditions)
        {
            List<BasicParticipation> result = new List<BasicParticipation>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + membershipFieldSequence + ConstructWhereClause ("Memberships", conditions),
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadMembershipFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicParticipation[] GetActiveParticipationsForOrganizationWithPaymentStatus (int[] organizationIds,
            int paymentStatus)
        {
            string sqlCommand =
                @"SELECT    " + membershipFieldSequence +
                @"       LEFT OUTER JOIN
                          MembershipPayments ON Memberships.MembershipId = MembershipPayments.MembershipId
                    WHERE     (Memberships.Active = 1) 
                    AND (coalesce(MembershipPayments.MembershipPaymentStatusId, 0) = " + paymentStatus + @") 
                    AND (Memberships.OrganizationId IN (" + JoinIds (organizationIds) + @")) 
                    AND (MembershipPayments.StatusDateTime =
                            ( select MAX(StatusDateTime) from MembershipPayments M2 where Memberships.MembershipId = M2.MembershipId   ) )";

            List<BasicParticipation> result = new List<BasicParticipation>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand (sqlCommand, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadMembershipFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public int GetParticipationCountForOrganizations (int[] organizationIds)
        {
            if (organizationIds == null || organizationIds.Length == 0)
            {
                return 0;
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT Count(PersonId) From Memberships WHERE Active=1 AND OrganizationId IN (" +
                        JoinIds (organizationIds) + ")", connection);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public int GetParticipantCountForOrganizationsSince (int[] organizationIds, DateTime sinceWhen)
        {
            if (organizationIds == null || organizationIds.Length == 0)
            {
                return 0;
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT Count(PersonId) From Memberships WHERE Active=1 AND OrganizationId IN (" +
                        JoinIds (organizationIds) + ") AND MemberSince > '" + sinceWhen.ToString ("yyyy-MM-dd HH:mm") +
                        "'", connection);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public int GetParticipantCountForOrganizations (int[] organizationIds)
        {
            if (organizationIds == null || organizationIds.Length == 0)
            {
                return 0;
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT Count(DISTINCT PersonId) From Memberships WHERE Active=1 AND OrganizationId IN (" +
                        JoinIds (organizationIds) + ")", connection);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public int GetParticipantCountForOrganizationsAndGeographies (int[] organizationIds, int[] geographyIds)
        {
            if (organizationIds == null || organizationIds.Length == 0)
            {
                return 0;
            }

            if (geographyIds == null || geographyIds.Length == 0)
            {
                return 0;
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand (
                    "select count(DISTINCT People.PersonId) from Memberships, People WHERE " +
                    "People.PersonId=Memberships.PersonId AND Memberships.Active=1 AND " +
                    "Memberships.OrganizationId IN (" + JoinIds (organizationIds) + ") AND " +
                    "People.GeographyId IN (" + JoinIds (geographyIds) + ")", connection);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        private static int[] ReadIntegersAtZeroFromDataReader (DbDataReader reader)
        {
            List<int> result = new List<int>();

            while (reader.Read())
            {
                result.Add (reader.GetInt32 (0));
            }

            return result.ToArray();
        }


        public Dictionary<int, List<BasicParticipation>> GetParticipationsForPeople (int[] personIds)
        {
            return GetParticipationsForPeople (personIds, 0);
        }


        public Dictionary<int, List<BasicParticipation>> GetParticipationsForPeople (int[] personIds, int gracePeriod)
        {
            Dictionary<int, List<BasicParticipation>> result = new Dictionary<int, List<BasicParticipation>>();

            if (personIds == null || personIds.Length == 0)
            {
                return result;
            }

            // Optimization: If more than 128 personIds, build a hash table, get _all_ memberships, and return
            // the matching ones logic wise. We've had _queries_ that exceed 64k text in length...

            if (personIds.Length > 128)
            {
                return GetParticipationsForPeopleOptimizedForLargeSets (personIds, gracePeriod);
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                string commandString = "SELECT " + membershipFieldSequence + " WHERE "
                                       + (gracePeriod == 0
                                           ? "Active=1"
                                           : " (Expires > DATE_ADD(NOW(),INTERVAL -" + gracePeriod +
                                             " Day) AND (Expires < DATE_ADD(DateTimeTerminated,INTERVAL 1 Day) OR Active=1))")
                                       + " AND PersonId in (" + JoinIds (personIds) + ")";

                DbCommand command = GetDbCommand (commandString, connection);
                command.CommandTimeout = 30; // If 30s is too short, something needs optimization.

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BasicParticipation participation = ReadMembershipFromDataReader (reader);
                        int personId = participation.PersonId;

                        if (!result.ContainsKey (personId))
                        {
                            result[personId] = new List<BasicParticipation>();
                        }

                        result[personId].Add (ReadMembershipFromDataReader (reader));
                        // Add to this person's list of memberships
                    }
                }
            }

            return result;
        }

        private Dictionary<int, List<BasicParticipation>> GetParticipationsForPeopleOptimizedForLargeSets (int[] personIds,
            int gracePeriod)
        {
            // This version is optimized for larger data sets. Rather than asking the database
            // for the correct set of people, ALL memberships are retrieved, and the filtering is
            // done in logic.

            Dictionary<int, List<BasicParticipation>> result = new Dictionary<int, List<BasicParticipation>>();

            // Build lookup

            Dictionary<int, bool> personIdLookup = new Dictionary<int, bool>();

            foreach (int personId in personIds)
            {
                personIdLookup[personId] = true;
            }

            // Get all memberships

            BasicParticipation[] allParticipations = GetParticipations();

            foreach (BasicParticipation membership in allParticipations)
            {
                if (membership.Active)
                {
                    if (personIdLookup.ContainsKey (membership.PersonId))
                    {
                        if (!result.ContainsKey (membership.PersonId))
                        {
                            result[membership.PersonId] = new List<BasicParticipation>();
                        }

                        result[membership.PersonId].Add (membership);
                    }
                }
                else if (gracePeriod != 0)
                {
                    //Within graceperiod?
                    if (membership.Expires.AddDays (gracePeriod) > DateTime.Now &&
                        membership.DateTerminated.AddDays (1) > membership.Expires)
                    {
                        if (personIdLookup.ContainsKey (membership.PersonId))
                        {
                            if (!result.ContainsKey (membership.PersonId))
                            {
                                result[membership.PersonId] = new List<BasicParticipation>();
                            }

                            result[membership.PersonId].Add (membership);
                        }
                    }
                }
            }


            return result;
        }


        public BasicParticipation GetActiveParticipation (int personId, int organizationId)
        {
            if (personId == 0)
            {
                throw new ArgumentOutOfRangeException ("PersonId cannot be 0");
            }

            if (organizationId == 0)
            {
                throw new ArgumentOutOfRangeException ("OrganizationId cannot be 0");
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + membershipFieldSequence + " WHERE Active=1 AND PersonId=" + personId +
                        " AND OrganizationId=" + organizationId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadMembershipFromDataReader (reader);
                    }

                    throw new ArgumentException ("No active membership for personid " + personId +
                                                 ", organizationid " + organizationId);
                }
            }
        }


        public bool IsPersonParticipant (int personId, int organizationId)
        {
            try
            {
                GetActiveParticipation (personId, organizationId);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }


        public BasicParticipation GetParticipation (int participationId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand ("SELECT " + membershipFieldSequence + " WHERE MembershipId=" + participationId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadMembershipFromDataReader (reader);
                    }

                    throw new ArgumentOutOfRangeException ("No such MembershipId: " + participationId);
                }
            }
        }


        private static BasicParticipation ReadMembershipFromDataReader (DbDataReader reader)
        {
            int membershipId = reader.GetInt32 (0);
            int personId = reader.GetInt32 (1);
            int organizationId = reader.GetInt32 (2);
            bool active = reader.GetBoolean (3);
            DateTime expires = Constants.DateTimeHigh;
            try
            {
                expires = reader.GetDateTime(4);
            }
            catch (MySqlConversionException) // invalid MySql date is possible
            {
                // ignore exception
            }
            DateTime memberSince = reader.GetDateTime (5);
            DateTime dateTerminated = DateTime.MaxValue;

            try
            {
                dateTerminated = reader.GetDateTime(6);
            }
            catch (MySqlConversionException)
            {
                // as above, ignore exception and go with Constants.DateTimeHigh
            }
            
            // bool terminatedAsInvalid = reader.GetBoolean(7);  Ignore this field for now

            return new BasicParticipation (membershipId, personId, organizationId, memberSince, expires, active,
                dateTerminated);
        }


        public BasicMembershipPaymentStatus GetMembershipPaymentStatus (int membershipId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + membershipPaymentStatusFieldSequence + " WHERE MembershipPayments.MembershipId=" +
                        membershipId + " ORDER BY StatusDateTime DESC LIMIT 1", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadMembershipPaymentStatusFromDataReader (reader);
                    }

                    // No payment status -- return undefined

                    return new BasicMembershipPaymentStatus (membershipId, MembershipPaymentStatus.Unknown,
                        new DateTime (1900, 1, 1));
                }
            }
        }

        public Dictionary<int, BasicMembershipPaymentStatus> GetMembershipPaymentStatuses (int[] membershipIds)
        {
            Dictionary<int, BasicMembershipPaymentStatus> retVal = new Dictionary<int, BasicMembershipPaymentStatus>();
            if (membershipIds.Length == 0)
                return retVal;
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + membershipPaymentStatusFieldSequence + " WHERE MembershipPayments.MembershipId IN (" +
                        JoinIds (membershipIds) + ") ORDER BY StatusDateTime ASC ", connection);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BasicMembershipPaymentStatus bms = ReadMembershipPaymentStatusFromDataReader (reader);
                        retVal[bms.MembershipId] = ReadMembershipPaymentStatusFromDataReader (reader);
                    }
                    foreach (int i in membershipIds)
                    {
                        if (!retVal.ContainsKey (i))
                            retVal[i] = new BasicMembershipPaymentStatus (i, MembershipPaymentStatus.Unknown,
                                new DateTime (1900, 1, 1));
                    }
                    return retVal;
                }
            }
        }


        private BasicMembershipPaymentStatus ReadMembershipPaymentStatusFromDataReader (DbDataReader reader)
        {
            int membershipId = reader.GetInt32 (0);
            int membershipPaymentStatusId = reader.GetInt32 (1);
            DateTime statusDateTime = reader.GetDateTime (2);

            return new BasicMembershipPaymentStatus (membershipId, (MembershipPaymentStatus) membershipPaymentStatusId,
                statusDateTime);
        }


        public void SetParticipationExpires (int membershipId, DateTime newExpiry)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetMembershipExpires", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "membershipId", membershipId);
                AddParameterWithName (command, "expires", newExpiry);

                command.ExecuteNonQuery();
            }
        }

        public void SetMembershipPaymentStatus (int membershipId, MembershipPaymentStatus paymentStatus,
            DateTime statusDateTime)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();
                DbCommand command = GetDbCommand ("SetMembershipPaymentStatus", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "membershipId", membershipId);
                AddParameterWithName (command, "paymentStatusId", (int) paymentStatus);
                AddParameterWithName (command, "statusDateTime", statusDateTime);

                command.ExecuteNonQuery();
            }
        }


        public int CreateParticipation (int personId, int organizationId, DateTime expires)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateMembership", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "personId", personId);
                AddParameterWithName (command, "organizationId", organizationId);
                AddParameterWithName (command, "expires", expires);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public int ImportParticipation (int personId, int organizationId, DateTime memberSince, DateTime expires)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("ImportMembership", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "personId", personId);
                AddParameterWithName (command, "organizationId", organizationId);
                AddParameterWithName (command, "expires", expires);
                AddParameterWithName (command, "membersince", expires);

                int retval = Convert.ToInt32 (command.ExecuteScalar());
                return retval;
            }
        }


        public void TerminateParticipation (int membershipId)
        {
            TerminateParticipation (membershipId, false);
        }


        public void TerminateParticipation (int membershipId, bool asInvalid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("TerminateMembership", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "membershipId", membershipId);
                AddParameterWithName (command, "terminatedAsInvalid", asInvalid);

                command.ExecuteNonQuery();
            }
        }

        public BasicParticipation[] GetExpiringParticipations (IHasIdentity organization, DateTime expiry)
        {
            return GetExpiringParticipations (organization, expiry, expiry);
        }

        public BasicParticipation[] GetExpiringParticipations (IHasIdentity organization, DateTime lowerBound,
            DateTime upperBound)
        {
            return GetExpiringParticipations (organization, lowerBound, upperBound, DatabaseCondition.ActiveTrue);
        }

        public BasicParticipation[] GetExpiringParticipations (IHasIdentity organization, DateTime lowerBound,
            DateTime upperBound, DatabaseCondition condition)
        {
            // Since I don't trust SQL Server to make correct date comparisons, especially given
            // that the dates are passed in text in SQL, we get ALL the memberships and do
            // the comparison in code instead. This is a run-seldom function, anyway, and getting
            // some 50k records with four short unlinked fields isn't that expensive.

            BasicParticipation[] participations = GetParticipations (organization, condition);

            DateTime minimumDateTime = lowerBound.Date;
            DateTime maximumDateTime = upperBound.Date.AddDays (1);

            List<BasicParticipation> result = new List<BasicParticipation>();

            foreach (BasicParticipation participation in participations)
            {
                // It is important that the lower border is inclusive and the upper exclusive in this
                // comparison:

                if (participation.Expires >= minimumDateTime && participation.Expires < maximumDateTime)
                {
                    result.Add (participation);
                }
            }

            return result.ToArray();
        }
    }
}