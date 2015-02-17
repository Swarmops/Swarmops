/*

 * 
 * Created in DbUpdate 0012.
 *
 
CREATE TABLE `PositionsStandard` (
  `StandardPositionId` int(11) NOT NULL AUTO_INCREMENT,
  `OrganizationId` int(11) NOT NULL,
  `PositionLevel` int(11) NOT NULL,
  `PositionTypeId` int(11) NOT NULL,
  `Active` tinyint(4) NOT NULL DEFAULT '1',
  `Volunteerable` tinyint(4) NOT NULL,
  `Overridable` tinyint(4) NOT NULL,
  `Covert` tinyint(4) NOT NULL DEFAULT '0',
  `ReportsToStandardPositionId` int(11) NOT NULL,
  `DotReportsToStandardPositionId` int(11) NOT NULL,
  `MinCount` int(11) NOT NULL,
  `MaxCount` int(11) NOT NULL,
  PRIMARY KEY (`StandardPositionId`),
  KEY `Index_Org` (`OrganizationId`),
  KEY `Index_Level` (`PositionLevel`),
  KEY `Index_Type` (`PositionTypeId`),
  KEY `Index_Reports1` (`ReportsToStandardPositionId`),
  KEY `Index_Reports2` (`DotReportsToStandardPositionId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


CREATE TABLE `PositionsAdditional` (
  `AdditionalPositionId` int(11) NOT NULL AUTO_INCREMENT,
  `OrganizationId` int(11) NOT NULL,
  `GeographyId` int(11) NOT NULL,
  `OverridesHigherPositionId` int(11) NOT NULL,
  `CreatedByPersonId` int(11) NOT NULL,
  `CreatedDateTimeUtc` datetime NOT NULL,
  `PositionTypeId` int(11) NOT NULL,
  `InheritsDownward` tinyint(4) NOT NULL,
  `Volunteerable` tinyint(4) NOT NULL,
  `Active` tinyint(4) NOT NULL DEFAULT '1',
  `Covert` tinyint(4) NOT NULL DEFAULT '0',
  `ReportsToStandardPositionId` int(11) NOT NULL,
  `ReportsToAdditionalPositionId` int(11) NOT NULL,
  `DotReportsToPositionId` int(11) NOT NULL,
  `MinCount` int(11) NOT NULL,
  `MaxCount` int(11) NOT NULL,
  PRIMARY KEY (`AdditionalPositionId`),
  KEY `Index_Org` (`OrganizationId`),
  KEY `Index_Geo` (`GeographyId`),
  KEY `Index_Report1` (`ReportsToStandardPositionId`),
  KEY `Index_Report2` (`ReportsToAdditionalPositionId`),
  KEY `Index_Report3` (`DotReportsToPositionId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

 
 CREATE TABLE `PositionAssignments` (
  `PositionAssignmentId` int(11) NOT NULL AUTO_INCREMENT,
  `OrganizationId` int(11) NOT NULL,
  `GeographyId` int(11) NOT NULL,
  `PositionId` int(11) NOT NULL,
  `PersonId` int(11) NOT NULL,
  `ExpiresDateTimeUtc` datetime NOT NULL,
  `CreatedDateTimeUtc` datetime NOT NULL,
  `CreatedByPersonId` int(11) NOT NULL,
  `CreatedByPositionId` int(11) NOT NULL,
  `Active` tinyint(4) NOT NULL DEFAULT '1',
  `TerminatedDateTimeUtc` datetime NOT NULL DEFAULT '1800-01-01 00:00:00',
  `TerminatedByPersonId` int(11) NOT NULL DEFAULT '0',
  `TerminatedByPositionId` int(11) NOT NULL DEFAULT '0',
  `AssignmentNotes` text NOT NULL,
  `TerminationNotes` text NOT NULL,
  PRIMARY KEY (`PositionAssignmentId`),
  KEY `Index_Org` (`OrganizationId`),
  KEY `Index_Geo` (`GeographyId`),
  KEY `Index_Person` (`PersonId`),
  KEY `Index_Active` (`Active`),
  KEY `Index_Pos` (`PositionId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

*/


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Database field reading

        private const string standardPositionFieldSequence =
            " StandardPositionId,OrganizationId,PositionLevel,PositionTypeId,Active," +                       //  0-4
            "Volunteerable,Overridable,Covert,ReportsToStandardPositionId,DotReportsToStandardPositionId," +  //  5-9
            "MinCount,MaxCount" +                                                                             // 10-11
            " FROM PositionsStandard ";

        private const string additionalPositionFieldSequence =
            " AdditionalPositionId,OrganizationId,GeographyId,OverridesHigherPositionId,CreatedByPersonId," +      //  0-4
            "CreatedDateTimeUtc,PositionTypeId,InheritsDownward,Volunteerable,Active," +                           //  5-9
            "Covert,ReportsToStandardPositionId,ReportsToAdditionalPositionId,DotReportsToPositionId,MinCount," +  // 10-14
            "MaxCount" +                                                                                           // 15
            " FROM PositionsAdditional ";

        private const string positionAssignmentFieldSequence =
            " PositionAssignmentId,OrganizationId,GeographyId,PositionId,PersonId," +                              //  0-4
            "ExpiresDateTimeUtc,CreatedDateTimeUtc,CreatedByPersonId,CreatedByPositionId,Active," +                //  5-9
            "TerminatedDateTimeUtc,TerminatedByPersonId,TerminatedByPositionId,AssignmentNotes,TerminationNotes" + // 10-14
            " FROM PositionAssignments ";

        private static BasicRefund ReadStandardPositionFromDataReader(IDataRecord reader)
        {
            int refundId = reader.GetInt32(0);
            int paymentId = reader.GetInt32(1);
            bool open = reader.GetBoolean(2);
            Int64 amountCents = reader.GetInt64(3);
            DateTime createdDateTime = reader.GetDateTime(4);
            DateTime closedDateTime = reader.GetDateTime(5);
            int createdByPersonId = reader.GetInt32(6);

            return new BasicRefund(refundId, paymentId, open, amountCents, createdByPersonId, createdDateTime,
                closedDateTime);
        }

        private static BasicRefund ReadAdditionalPositionFromDataReader(IDataRecord reader)
        {
            int refundId = reader.GetInt32(0);
            int paymentId = reader.GetInt32(1);
            bool open = reader.GetBoolean(2);
            Int64 amountCents = reader.GetInt64(3);
            DateTime createdDateTime = reader.GetDateTime(4);
            DateTime closedDateTime = reader.GetDateTime(5);
            int createdByPersonId = reader.GetInt32(6);

            return new BasicRefund(refundId, paymentId, open, amountCents, createdByPersonId, createdDateTime,
                closedDateTime);
        }

        private static BasicRefund ReadPositionAssignmentFromDataReader(IDataRecord reader)
        {
            int refundId = reader.GetInt32(0);
            int paymentId = reader.GetInt32(1);
            bool open = reader.GetBoolean(2);
            Int64 amountCents = reader.GetInt64(3);
            DateTime createdDateTime = reader.GetDateTime(4);
            DateTime closedDateTime = reader.GetDateTime(5);
            int createdByPersonId = reader.GetInt32(6);

            return new BasicRefund(refundId, paymentId, open, amountCents, createdByPersonId, createdDateTime,
                closedDateTime);
        }


        #endregion

        #region Database record reading -- SELECT clauses

        #region Standard Positions

        public BasicRefund GetStandardPosition (int standardPositionId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT" + standardPositionFieldSequence +
                                  "WHERE StandardPositionId=" + standardPositionId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadRefundFromDataReader(reader);
                    }

                    throw new ArgumentException("No such StandardPositionId:" + standardPositionId);
                }
            }
        }


        public BasicRefund[] GetStandardPositions(params object[] conditions)
        {
            List<BasicRefund> result = new List<BasicRefund>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + standardPositionFieldSequence + ConstructWhereClause("PositionsStandard", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadRefundFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion


        #region Additional Positions

        public BasicRefund GetAdditionalPosition(int additionalPositionId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT" + additionalPositionFieldSequence +
                                  "WHERE AdditionalPositionId=" + additionalPositionId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadRefundFromDataReader(reader);
                    }

                    throw new ArgumentException("No such StandardPositionId:" + additionalPositionId);
                }
            }
        }


        public BasicRefund[] GetAdditionalPositions(params object[] conditions)
        {
            List<BasicRefund> result = new List<BasicRefund>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + standardPositionFieldSequence + ConstructWhereClause("PositionsAdditional", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadRefundFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion


        #region Position Assignments

        public BasicRefund GetPositionAssignment(int positionAssignmentId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT " + positionAssignmentFieldSequence +
                                  "WHERE PositionAssignmentId=" + positionAssignmentId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadRefundFromDataReader(reader);
                    }

                    throw new ArgumentException("No such PositionAssignmentId:" + positionAssignmentId);
                }
            }
        }


        public BasicRefund[] GetPositionAssignments(params object[] conditions)
        {
            List<BasicRefund> result = new List<BasicRefund>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + standardPositionFieldSequence + ConstructWhereClause("PositionsStandard", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadRefundFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion


        #endregion

        #region Creation and manipulation -- stored procedures
        /*
        public int CreateRefund(int paymentId, int createdByPersonId)
        {
            return CreateRefund(paymentId, createdByPersonId, 0L); // zero is default; it means refund entire payment
        }


        public int CreateRefund(int paymentId, int createdByPersonId, Int64 amountCents)
        {
            DateTime now = DateTime.Now;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateRefund", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "paymentId", paymentId);
                AddParameterWithName(command, "amountCents", amountCents);
                AddParameterWithName(command, "createdByPersonId", createdByPersonId);
                AddParameterWithName(command, "createdDateTime", now);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public void SetRefundOpen(int refundId, bool open)
        {
            DateTime now = DateTime.Now;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetRefundOpen", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "refundId", refundId);
                AddParameterWithName(command, "open", open);
                AddParameterWithName(command, "closedDateTime", now); // always set, but only valid if open=false

                command.ExecuteNonQuery();
            }
        }*/

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