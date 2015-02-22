/*

 * 
 * Created in DbUpdate 0012. Updated in Db0013-Db0015.
 *
 
CREATE TABLE `Positions` (
  `PositionId` int(11) NOT NULL AUTO_INCREMENT,
  `PositionLevel` int(11) NOT NULL,
  `OrganizationId` int(11) NOT NULL,
  `GeographyId` int(11) NOT NULL,
  `OverridesHigherPositionId` int(11) NOT NULL,
  `CreatedByPersonId` int(11) NOT NULL,
  `CreatedByPositionId` int(11) NOT NULL,
  `CreatedDateTimeUtc` datetime NOT NULL,
  `PositionTypeId` int(11) NOT NULL,
  `PositionTitleId` int(11) NOT NULL,
  `InheritsDownward` tinyint(4) NOT NULL,
  `Active` tinyint(4) NOT NULL DEFAULT '1',
  `Volunteerable` tinyint(4) NOT NULL,
  `Overridable` tinyint(4) NOT NULL,
  `Covert` tinyint(4) NOT NULL DEFAULT '0',
  `ReportsToPositionId` int(11) NOT NULL,
  `DotReportsToPositionId` int(11) NOT NULL,
  `MinCount` int(11) NOT NULL,
  `MaxCount` int(11) NOT NULL,
  PRIMARY KEY (`PositionId`),
  KEY `Index_Org` (`OrganizationId`),
  KEY `Index_Geo` (`GeographyId`),
  KEY `Index_Report1` (`ReportsToPositionId`),
  KEY `Index_Report2` (`DotReportsToPositionId`),
  KEY `Index_Level` (`PositionLevel`)
);

 
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
);

*/


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Database field reading

        private const string positionFieldSequence =
            " Positions.PositionId,Positions.PositionLevel,Positions.OrganizationId,Positions.GeographyId,Positions.OverridesHigherPositionId," +  //  0-4
            "Positions.CreatedByPersonId,Positions.CreatedByPositionId,Positions.CreatedDateTimeUtc,PositionTypes.Name,PositionTitles.Name," +     //  5-9
            "Positions.InheritsDownward,Positions.Active,Positions.Volunteerable,Positions.Overridable,Positions.Covert," +                        // 10-14
            "Positions.ReportsToPositionId,Positions.DotReportsToPositionId,Positions.MinCount,Positions.MaxCount" +                               // 15-18
            " FROM Positions " +
            " JOIN PositionTypes ON (Positions.PositionTypeId=PositionTypes.PositionTypeId) " +
            " JOIN PositionTitles ON (Positions.PositionTitleId=PositionTitles.PositionTitleId) ";

        private const string positionAssignmentFieldSequence =
            " PositionAssignmentId,OrganizationId,GeographyId,PositionId,PersonId," +                              //  0-4
            "CreatedDateTimeUtc,CreatedByPersonId,CreatedByPositionId,Active,ExpiresDateTimeUtc," +                //  5-9
            "TerminatedDateTimeUtc,TerminatedByPersonId,TerminatedByPositionId,AssignmentNotes,TerminationNotes" + // 10-14
            " FROM PositionAssignments ";

        private static BasicPosition ReadPositionFromDataReader(IDataRecord reader)
        {
            // groups of five to match the field sequence string lines (maintainability)

            int positionId = reader.GetInt32 (0);
            PositionLevel positionLevel = (PositionLevel) reader.GetInt32 (1);
            int organizationId = reader.GetInt32 (2);
            int geographyId = reader.GetInt32 (3);
            int overridesHigherPositionId = reader.GetInt32 (4);

            int createdByPersonId = reader.GetInt32 (5);
            int createdByPositionId = reader.GetInt32 (6);
            DateTime createdDateTimeUtc = reader.GetDateTime (7);
            string positionTypeName = reader.GetString (8);
            string positionTitleName = reader.GetString (9);

            bool inheritsDownward = reader.GetBoolean (10);
            bool active = reader.GetBoolean(11);
            bool volunteerable = reader.GetBoolean(12);
            bool overridable = reader.GetBoolean(13);
            bool covert = reader.GetBoolean(14);

            int reportsToPositionId = reader.GetInt32 (15);
            int dotReportsToPositionId = reader.GetInt32 (16);
            int minCount = reader.GetInt32 (17);
            int maxCount = reader.GetInt32 (18);

            return new BasicPosition (
                positionId, positionLevel, organizationId, geographyId, overridesHigherPositionId,
                createdByPersonId, createdByPositionId, createdDateTimeUtc, positionTypeName, positionTitleName,
                inheritsDownward, active, volunteerable, overridable, covert,
                reportsToPositionId, dotReportsToPositionId, minCount, maxCount);

        }

        private static BasicPositionAssignment ReadPositionAssignmentFromDataReader(IDataRecord reader)
        {
            int positionAssigmentId = reader.GetInt32 (0);
            int organizationId = reader.GetInt32 (1); // this field is necessary because assignment can be to a suborg of the position record
            int geographyId = reader.GetInt32 (2);
            int positionId = reader.GetInt32 (3);
            int personId = reader.GetInt32 (4);
            DateTime createdDateTimeUtc = reader.GetDateTime (5);
            int createdByPersonId = reader.GetInt32 (6);
            int createdByPositionId = reader.GetInt32 (7);
            bool active = reader.GetBoolean (8);
            DateTime expiresDateTimeUtc = reader.GetDateTime (9);
            DateTime terminatedDateTimeUtc = reader.GetDateTime (10);
            int terminatedByPersonId = reader.GetInt32 (11);
            int terminatedByPositionId = reader.GetInt32 (12);
            string assignmentNotes = reader.GetString (13);
            string terminationNotes = reader.GetString (14);

            return new BasicPositionAssignment (positionAssigmentId, organizationId, geographyId, positionId,
                personId, createdDateTimeUtc, createdByPersonId, createdByPositionId, active, expiresDateTimeUtc,
                terminatedDateTimeUtc, terminatedByPersonId, terminatedByPositionId, assignmentNotes, terminationNotes);
        }

        #endregion

        #region Database record reading -- SELECT clauses

        #region Positions

        public BasicPosition GetPosition(int positionId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT" + positionFieldSequence +
                                  "WHERE PositionId=" + positionId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadPositionFromDataReader(reader);
                    }

                    throw new ArgumentException("No such PositionId:" + positionId);
                }
            }
        }


        public BasicPosition[] GetPositionChildren (int positionId)
        {
            List<BasicPosition> result = new List<BasicPosition>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + positionFieldSequence + "WHERE Positions.ReportsToPositionId=" + positionId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPositionFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicPosition[] GetPositions(params object[] conditions)
        {
            List<BasicPosition> result = new List<BasicPosition>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + positionFieldSequence + ConstructWhereClause("Positions", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPositionFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion


        #region Position Assignments

        public BasicPositionAssignment GetPositionAssignment(int positionAssignmentId)
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
                        return ReadPositionAssignmentFromDataReader(reader);
                    }

                    throw new ArgumentException("No such PositionAssignmentId:" + positionAssignmentId);
                }
            }
        }


        public BasicPositionAssignment[] GetPositionAssignments(params object[] conditions)
        {
            List<BasicPositionAssignment> result = new List<BasicPositionAssignment>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + positionAssignmentFieldSequence + ConstructWhereClause("PositionAssignments", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPositionAssignmentFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion


        #endregion

        #region Creation and manipulation -- stored procedures


        public int CreatePosition(
            PositionLevel positionLevel, int organizationId, int geographyId, int overridesHigherPositionId, 
            int createdByPersonId, int createdByPositionId, /* DateTimeCreatedUtc is measured below, */ string positionTypeName, string positionTitleName,
            bool inheritsDownward, /* active defaults true, */ bool volunteerable, bool overridable, /* covert defaults false, */ 
            int reportsToPositionId, int dotReportsToPositionId,  int minCount, int maxCount)
        {
            DateTime utcNow = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreatePosition", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "positionLevel", (int) positionLevel);
                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "overridesHigherPositionId", overridesHigherPositionId);

                AddParameterWithName(command, "createdByPersonId", createdByPersonId);
                AddParameterWithName(command, "createdByPositionId", createdByPositionId);
                AddParameterWithName(command, "createdDateTimeUtc", utcNow);
                AddParameterWithName(command, "positionType", positionTypeName);
                AddParameterWithName(command, "positionTitle", positionTitleName);

                AddParameterWithName(command, "inheritsDownward", volunteerable);
                // active defaults to true
                AddParameterWithName(command, "volunteerable", volunteerable);
                AddParameterWithName(command, "overridable", volunteerable);
                // covert defaults to false

                AddParameterWithName(command, "reportsToPositionId", reportsToPositionId);
                AddParameterWithName(command, "dotReportsToPositionId", dotReportsToPositionId);
                AddParameterWithName(command, "minCount", minCount);
                AddParameterWithName(command, "maxCount", maxCount);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }



        public int CreatePositionAssignment (int organizationId, int geographyId, int positionId, int personId, int createdByPersonId, int createdByPositionId, DateTime expiresDateTimeUtc, string assignmentNotes)
        {
            DateTime utcNow = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreatePositionAssignment", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "positionId", positionId);
                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "createdDateTimeUtc", utcNow);
                AddParameterWithName(command, "createdByPersonId", createdByPersonId);
                AddParameterWithName(command, "createdByPositionId", createdByPositionId);
                AddParameterWithName(command, "expiresDateTimeUtc", expiresDateTimeUtc);
                AddParameterWithName(command, "assignmentNotes", assignmentNotes);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


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