using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;

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


        public int[] GetMembersForOrganizationsAndGeographies (int[] organizationIds, int[] geographyIds)
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


        public BasicMembership[] GetMemberships (params object[] conditions)
        {
            List<BasicMembership> result = new List<BasicMembership>();

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


        public BasicMembership[] GetValidMembershipsForOrganizationAndPaymentStatus (int[] organizationIds,
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

            List<BasicMembership> result = new List<BasicMembership>();

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


        public int GetMembershipCountForOrganizations (int[] organizationIds)
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

        public int GetMembershipCountForOrganizationsSince (int[] organizationIds, DateTime sinceWhen)
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

        public int GetMemberCountForOrganizations (int[] organizationIds)
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

        public int GetMemberCountForOrganizationsAndGeographies (int[] organizationIds, int[] geographyIds)
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


        public Dictionary<int, List<BasicMembership>> GetMembershipsForPeople (int[] personIds)
        {
            return GetMembershipsForPeople (personIds, 0);
        }


        public Dictionary<int, List<BasicMembership>> GetMembershipsForPeople (int[] personIds, int gracePeriod)
        {
            Dictionary<int, List<BasicMembership>> result = new Dictionary<int, List<BasicMembership>>();

            if (personIds == null || personIds.Length == 0)
            {
                return result;
            }

            // Optimization: If more than 128 personIds, build a hash table, get _all_ memberships, and return
            // the matching ones logic wise. We've had _queries_ that exceed 64k text in length...

            if (personIds.Length > 64)
            {
                return GetMembershipsForPeopleOptimizedForLargeSets (personIds, gracePeriod);
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
                        BasicMembership membership = ReadMembershipFromDataReader (reader);
                        int personId = membership.PersonId;

                        if (!result.ContainsKey (personId))
                        {
                            result[personId] = new List<BasicMembership>();
                        }

                        result[personId].Add (ReadMembershipFromDataReader (reader));
                        // Add to this person's list of memberships
                    }
                }
            }

            return result;
        }

        private Dictionary<int, List<BasicMembership>> GetMembershipsForPeopleOptimizedForLargeSets (int[] personIds,
            int gracePeriod)
        {
            // This version is optimized for larger data sets. Rather than asking the database
            // for the correct set of people, ALL memberships are retrieved, and the filtering is
            // done in logic.

            Dictionary<int, List<BasicMembership>> result = new Dictionary<int, List<BasicMembership>>();

            // Build lookup

            Dictionary<int, bool> personIdLookup = new Dictionary<int, bool>();

            foreach (int personId in personIds)
            {
                personIdLookup[personId] = true;
            }

            // Get all memberships

            BasicMembership[] allMemberships = GetMemberships();

            foreach (BasicMembership membership in allMemberships)
            {
                if (membership.Active)
                {
                    if (personIdLookup.ContainsKey (membership.PersonId))
                    {
                        if (!result.ContainsKey (membership.PersonId))
                        {
                            result[membership.PersonId] = new List<BasicMembership>();
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
                                result[membership.PersonId] = new List<BasicMembership>();
                            }

                            result[membership.PersonId].Add (membership);
                        }
                    }
                }
            }


            return result;
        }


        public BasicMembership GetActiveMembership (int personId, int organizationId)
        {
            if (personId == 0)
            {
                throw new ArgumentOutOfRangeException ("PersonId cannot be 0");
            }

            if (organizationId == 0)
            {
                throw new ArgumentOutOfRangeException ("OrganizationId cannot be 0");
            }

            List<BasicMembership> result = new List<BasicMembership>();

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


        public bool IsPersonMember (int personId, int organizationId)
        {
            try
            {
                GetActiveMembership (personId, organizationId);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }


        public BasicMembership GetMembership (int membershipId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand ("SELECT " + membershipFieldSequence + " WHERE MembershipId=" + membershipId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadMembershipFromDataReader (reader);
                    }

                    throw new ArgumentOutOfRangeException ("No such MembershipId: " + membershipId);
                }
            }
        }


        private static BasicMembership ReadMembershipFromDataReader (DbDataReader reader)
        {
            int membershipId = reader.GetInt32 (0);
            int personId = reader.GetInt32 (1);
            int organizationId = reader.GetInt32 (2);
            bool active = reader.GetBoolean (3);
            DateTime expires = reader.GetDateTime (4);
            DateTime memberSince = reader.GetDateTime (5);
            DateTime dateTerminated = reader.GetDateTime (6);
            // bool terminatedAsInvalid = reader.GetBoolean(7);  Ignore this field for now

            return new BasicMembership (membershipId, personId, organizationId, memberSince, expires, active,
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


        public void SetMembershipExpires (int membershipId, DateTime newExpiry)
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


        public int CreateMembership (int personId, int organizationId, DateTime expires)
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

        public int ImportMembership (int personId, int organizationId, DateTime memberSince, DateTime expires)
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


        public void TerminateMembership (int membershipId)
        {
            TerminateMembership (membershipId, false);
        }


        public void TerminateMembership (int membershipId, bool asInvalid)
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

        public BasicMembership[] GetExpiringMemberships (IHasIdentity organization, DateTime expiry)
        {
            return GetExpiringMemberships (organization, expiry, expiry);
        }

        public BasicMembership[] GetExpiringMemberships (IHasIdentity organization, DateTime lowerBound,
            DateTime upperBound)
        {
            return GetExpiringMemberships (organization, lowerBound, upperBound, DatabaseCondition.ActiveTrue);
        }

        public BasicMembership[] GetExpiringMemberships (IHasIdentity organization, DateTime lowerBound,
            DateTime upperBound, DatabaseCondition condition)
        {
            // Since I don't trust SQL Server to make correct date comparisons, especially given
            // that the dates are passed in text in SQL, we get ALL the memberships and do
            // the comparison in code instead. This is a run-seldom function, anyway, and getting
            // some 50k records with four short unlinked fields isn't that expensive.

            BasicMembership[] memberships = GetMemberships (organization, condition);

            DateTime minimumDateTime = lowerBound.Date;
            DateTime maximumDateTime = upperBound.Date.AddDays (1);

            List<BasicMembership> result = new List<BasicMembership>();

            foreach (BasicMembership membership in memberships)
            {
                // It is important that the lower border is inclusive and the upper exclusive in this
                // comparison:

                if (membership.Expires >= minimumDateTime && membership.Expires < maximumDateTime)
                {
                    result.Add (membership);
                }
            }

            return result.ToArray();
        }
    }
}