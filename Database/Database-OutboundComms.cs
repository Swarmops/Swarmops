using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types.Communications;
using Swarmops.Common.Enums;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Database field reading

        private const string outboundCommFieldSequence =
            " OutboundCommId,SenderPersonId,FromPersonId,OrganizationId,CreatedDateTime," + // 0-4
            "ResolverClassId,RecipientDataXml,Resolved,ResolvedDateTime,Priority," + // 5-9
            "TransmitterClassId,PayloadXml,Open,StartTransmitDateTime,ClosedDateTime," + // 10-14
            "RecipientCount,RecipientsSuccess,RecipientsFail " + // 15-17
            "FROM OutboundComms ";

        private BasicOutboundComm ReadOutboundCommFromDataReader (IDataRecord reader)
            // Not static -- accesses cache, requiring connection strings
        {
            int outboundCommId = reader.GetInt32 (0);
            int senderPersonId = reader.GetInt32 (1);
            int fromPersonId = reader.GetInt32 (2);
            int organizationId = reader.GetInt32 (3);
            DateTime createdDateTime = reader.GetDateTime (4);

            int resolverClassId = reader.GetInt32 (5);
            // Resolve to class name -- cached call more efficient than Join in Select
            string recipientDataXml = reader.GetString (6); // Interpreted by ResolverClass
            bool resolved = reader.GetBoolean (7);
            DateTime resolvedDateTime = reader.GetDateTime (8);
            OutboundCommPriority priority = (OutboundCommPriority) reader.GetInt32 (9);

            int transmitterClassId = reader.GetInt32 (10);
            // Resolve to class name -- cached call more efficient than Join in Select
            string payloadXml = reader.GetString (11); // Interpreted by TransmitterClass
            bool open = reader.GetBoolean (12);
            DateTime startTransmitDateTime = reader.GetDateTime (13);
            DateTime closedDateTime = reader.GetDateTime (14);

            int recipientCount = reader.GetInt32 (15); // Set by resolver
            int recipientSuccessCount = reader.GetInt32 (16);
            int recipientFailCount = reader.GetInt32 (17);

            // TODO: resolve resolverClass, transmitterClass

            string resolverClass = GetCachedResolverClassName (resolverClassId);
            string transmitterClass = GetCachedTransmitterClassName (transmitterClassId);

            return new BasicOutboundComm
                (outboundCommId, senderPersonId, fromPersonId, organizationId, createdDateTime,
                    resolverClass, recipientDataXml, resolved, resolvedDateTime, priority,
                    transmitterClass, payloadXml, open, startTransmitDateTime, closedDateTime,
                    recipientCount, recipientSuccessCount, recipientFailCount);
        }

        #endregion

        #region Database record reading -- SELECT clauses

        public BasicOutboundComm GetOutboundComm (int outboundCommId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand ("SELECT" + outboundCommFieldSequence +
                                  "WHERE OutboundCommId=" + outboundCommId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadOutboundCommFromDataReader (reader);
                    }

                    throw new ArgumentException ("No such OutboundCommId:" + outboundCommId);
                }
            }
        }


        public BasicOutboundComm[] GetOutboundComms (params object[] conditions)
        {
            List<BasicOutboundComm> result = new List<BasicOutboundComm>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + outboundCommFieldSequence + ConstructWhereClause ("OutboundComms", conditions) +
                        " ORDER BY Priority ASC", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadOutboundCommFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Database optimizations

        private static Dictionary<int, string> _resolverClassCache;
        private static Dictionary<int, string> _transmitterClassCache;

        protected string GetCachedResolverClassName (int resolverClassId)
        {
            if (_resolverClassCache == null)
            {
                _resolverClassCache = new Dictionary<int, string>();
                _resolverClassCache[0] = null; // special case for resolvers
            }

            if (_resolverClassCache.ContainsKey (resolverClassId))
            {
                return _resolverClassCache[resolverClassId];
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand ("SELECT Name FROM ResolverClasses WHERE ResolverClassId=" + resolverClassId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        _resolverClassCache[resolverClassId] = reader.GetString (0);
                        return _resolverClassCache[resolverClassId];
                    }

                    throw new ArgumentException ("No such ResolverClassId:" + resolverClassId);
                }
            }
        }

        protected string GetCachedTransmitterClassName (int transmitterClassId)
        {
            if (_transmitterClassCache == null)
            {
                _transmitterClassCache = new Dictionary<int, string>();
            }

            if (_transmitterClassCache.ContainsKey (transmitterClassId))
            {
                return _transmitterClassCache[transmitterClassId];
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand ("SELECT Name FROM TransmitterClasses WHERE TransmitterClassId=" + transmitterClassId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        _transmitterClassCache[transmitterClassId] = reader.GetString (0);
                        return _transmitterClassCache[transmitterClassId];
                    }

                    throw new ArgumentException ("No such TransmitterClassId:" + transmitterClassId);
                }
            }
        }

        #endregion

        #region Creation and manipulation -- stored procedures

        public int CreateOutboundComm (int senderPersonId, int fromPersonId, int organizationId, string resolverClass,
            string recipientDataXml, string transmitterClass, string payloadXml, OutboundCommPriority priority)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateOutboundComm", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "senderPersonId", senderPersonId);
                AddParameterWithName (command, "fromPersonId", fromPersonId);
                AddParameterWithName (command, "organizationId", organizationId);
                AddParameterWithName (command, "resolverClass",
                    String.IsNullOrEmpty (resolverClass) ? "0" : resolverClass);
                // Will be turned into ID by stored procedure
                AddParameterWithName (command, "recipientDataXml",
                    String.IsNullOrEmpty (recipientDataXml) ? string.Empty : recipientDataXml);
                AddParameterWithName (command, "transmitterClass", transmitterClass);
                // Will be turned into ID by stored procedure
                AddParameterWithName (command, "payloadXml", payloadXml);
                AddParameterWithName (command, "priority", (int) priority);
                // convert enum to integerized priority; convert back on field read
                AddParameterWithName (command, "createdDateTime", DateTime.UtcNow);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public void SetOutboundCommResolved (int outboundCommId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundCommResolved", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundCommId", outboundCommId);
                AddParameterWithName (command, "dateTime", DateTime.UtcNow);

                command.ExecuteNonQuery();
            }
        }

        public void SetOutboundCommTransmissionStart (int outboundCommId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundCommTransmissionStart", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundCommId", outboundCommId);
                AddParameterWithName (command, "dateTime", DateTime.UtcNow);

                command.ExecuteNonQuery();
            }
        }

        public void SetOutboundCommClosed (int outboundCommId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundCommClosed", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundCommId", outboundCommId);
                AddParameterWithName (command, "dateTime", DateTime.UtcNow);

                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Dead template code

        /*
        public BasicExpenseClaim GetExpenseClaim(int expenseClaimId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand(
                    "SELECT ExpenseClaimId,ClaimingPersonId,CreatedDateTime,Open,Attested," +
                    "Validated,Claimed,OrganizationId,GeographyId,BudgetId,BudgetYear," +
                    "ExpenseDate,Description,PreApprovedAmount,Amount,Repaid " +
                    "FROM ExpenseClaims WHERE ExpenseClaimId=" + expenseClaimId.ToString(),
                    connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadExpenseFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown ExpenseClaim Id");
                }
            }
        }

        public BasicExpenseClaim[] GetExpenseClaimsByClaimer(int claimingPersonId)
        {
            List<BasicExpenseClaim> result = new List<BasicExpenseClaim>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT ExpenseClaimId,ClaimingPersonId,CreatedDateTime,Open,Attested," +
                    "Validated,Claimed,OrganizationId,GeographyId,BudgetId,BudgetYear," +
                    "ExpenseDate,Description,PreApprovedAmount,Amount,Repaid " +
                    "FROM ExpenseClaims WHERE ClaimingPersonId=" + claimingPersonId.ToString(),
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadExpenseFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicExpenseClaim[] GetExpenseClaimsByBudgetAndYear(int budgetId, int budgetYear)
        {
            List<BasicExpenseClaim> result = new List<BasicExpenseClaim>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT ExpenseClaimId,ClaimingPersonId,CreatedDateTime,Open,Attested," +
                    "Validated,Claimed,OrganizationId,GeographyId,BudgetId,BudgetYear," +
                    "ExpenseDate,Description,PreApprovedAmount,Amount,Repaid " +
                    "FROM ExpenseClaims WHERE BudgetId=" + budgetId.ToString() + " AND BudgetYear=" + budgetYear.ToString(),
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadExpenseFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicExpenseClaim[] GetExpenseClaimsByOrganization(int organizationId)
        {
            List<BasicExpenseClaim> result = new List<BasicExpenseClaim>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT ExpenseClaimId,ClaimingPersonId,CreatedDateTime,Open,Attested," +
                    "Validated,Claimed,OrganizationId,GeographyId,BudgetId,BudgetYear," +
                    "ExpenseDate,Description,PreApprovedAmount,Amount,Repaid " +
                    "FROM ExpenseClaims WHERE OrganizationId=" + organizationId.ToString(),
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadExpenseFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        private BasicExpenseClaim ReadExpenseFromDataReader(DbDataReader reader)
        {
            int expenseClaimId = reader.GetInt32(0);
            int claimingPersonId = reader.GetInt32(1);
            DateTime createdDateTime = reader.GetDateTime(2);
            bool open = reader.GetBoolean(3);
            bool attested = reader.GetBoolean(4);
            bool documented = reader.GetBoolean(5);
            bool claimed = reader.GetBoolean(6);
            int organizationId = reader.GetInt32(7);
            int geographyId = reader.GetInt32(8); // obsolete field
            int budgetId = reader.GetInt32(9);
            int budgetYear = reader.GetInt32(10);
            DateTime expenseDate = reader.GetDateTime(11);
            string description = reader.GetString(12);
            double preApprovedAmount = reader.GetDouble(13);
            double amount = reader.GetDouble(14);
            bool repaid = reader.GetBoolean(15);

            return new BasicExpenseClaim(expenseClaimId, claimingPersonId, createdDateTime,
                                    open, attested, documented, claimed, organizationId, geographyId,
                                    budgetId, budgetYear, expenseDate, description, preApprovedAmount,
                                    amount, repaid);
        }


        public int CreateExpenseClaim(int claimingPersonId, int organizationId, int budgetId, int budgetYear,
                                  DateTime expenseDate, string description, double amount)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateExpenseClaim", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "claimingPersonId", claimingPersonId);
                AddParameterWithName(command, "createdDateTime", DateTime.Now);
                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "budgetId", budgetId);
                AddParameterWithName(command, "budgetYear", budgetYear);
                AddParameterWithName(command, "expenseDate", expenseDate);
                AddParameterWithName(command, "description", description);
                AddParameterWithName(command, "amount", amount);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int SetExpenseClaimDescription(int expenseClaimId, string description)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetExpenseClaimDescription", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseClaimId", expenseClaimId);
                AddParameterWithName(command, "description", description);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public int SetExpenseClaimBudget(int expenseClaimId, int budgetId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetExpenseClaimBudget", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseClaimId", expenseClaimId);
                AddParameterWithName(command, "budgetId", budgetId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public int SetExpenseClaimBudgetYear(int expenseClaimId, int budgetYear)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetExpenseClaimBudgetYear", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseClaimId", expenseClaimId);
                AddParameterWithName(command, "budgetYear", budgetYear);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public int SetExpenseClaimDate(int expenseClaimId, DateTime expenseDate)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetExpenseClaimDate", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseClaimId", expenseClaimId);
                AddParameterWithName(command, "expenseDate", expenseDate);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public int SetExpenseClaimAmount(int expenseClaimId, double amount)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetExpenseClaimAmount", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseClaimId", expenseClaimId);
                AddParameterWithName(command, "amount", amount);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }



        public int SetExpenseClaimAttested(int expenseClaimId, bool attested)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetExpenseClaimAttested", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseClaimId", expenseClaimId);
                AddParameterWithName(command, "attested", attested);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }



        public int SetExpenseClaimValidated(int expenseClaimId, bool validated)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetExpenseClaimValidated", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseClaimId", expenseClaimId);
                AddParameterWithName(command, "validated", validated);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }



        public int SetExpenseClaimOpen(int expenseClaimId, bool open)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetExpenseClaimOpen", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseClaimId", expenseClaimId);
                AddParameterWithName(command, "open", open);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }




        public int SetExpenseClaimRepaid(int expenseClaimId, bool repaid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetExpenseClaimRepaid", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseClaimId", expenseClaimId);
                AddParameterWithName(command, "repaid", repaid);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }




        public int CreateExpenseEvent(int expenseId, ExpenseEventType eventType, int personId)
        {
            // WARNING: NOT CONVERTED TO V4 YET

            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateExpenseEvent", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "expenseId", expenseId);
                AddParameterWithName(command, "dateTime", DateTime.Now);
                AddParameterWithName(command, "eventPersonId", personId);
                AddParameterWithName(command, "eventType", eventType.ToString());

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        */

        #endregion
    }
}