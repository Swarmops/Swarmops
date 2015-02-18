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

        private const string standardPositionFieldSequence =
            " PositionsStandard.StandardPositionId,PositionsStandard.OrganizationId,PositionsStandard.PositionLevel,PositionTypes.Name,PositionsStandard.Active," +                       //  0-4
            "PositionsStandard.Volunteerable,PositionsStandard.Overridable,PositionsStandard.Covert,PositionsStandard.ReportsToStandardPositionId,PositionsStandard.DotReportsToStandardPositionId," +  //  5-9
            "PositionsStandard.MinCount,PositionsStandard.MaxCount" +                                                                             // 10-11
            " FROM PositionsStandard JOIN PositionTypes ON (PositionsStandard.PositionTypeId=PositionTypes.PositionTypeId) ";

        private const string additionalPositionFieldSequence =
            " PositionsAdditional.AdditionalPositionId,PositionsAdditional.OrganizationId,PositionsAdditional.GeographyId,PositionsAdditional.OverridesHigherPositionId,PositionsAdditional.CreatedByPersonId," +      //  0-4
            "PositionsAdditional.CreatedDateTimeUtc,PositionTypes.Name,PositionsAdditional.InheritsDownward,PositionsAdditional.Volunteerable,PositionsAdditional.Active," +                           //  5-9
            "PositionsAdditional.Covert,PositionsAdditional.ReportsToStandardPositionId,PositionsAdditional.ReportsToAdditionalPositionId,PositionsAdditional.DotReportsToPositionId,PositionsAdditional.MinCount," +  // 10-14
            "PositionsAdditional.MaxCount" +                                                                                           // 15
            " FROM PositionsAdditional JOIN PositionTypes ON (PositionsAdditional.PositionTypeId=PositionTypes.PositionTypeId) ";

        private const string positionAssignmentFieldSequence =
            " PositionAssignmentId,OrganizationId,GeographyId,PositionId,PersonId," +                              //  0-4
            "CreatedDateTimeUtc,CreatedByPersonId,CreatedByPositionId,Active,ExpiresDateTimeUtc," +                //  5-9
            "TerminatedDateTimeUtc,TerminatedByPersonId,TerminatedByPositionId,AssignmentNotes,TerminationNotes" + // 10-14
            " FROM PositionAssignments ";

        private static BasicPosition ReadStandardPositionFromDataReader(IDataRecord reader)
        {
            int standardPositionId = reader.GetInt32(0);
            int organizationId = reader.GetInt32 (1);
            PositionLevel positionLevel = (PositionLevel) reader.GetInt32 (2);
            string positionTypeName = reader.GetString (3);
            bool active = reader.GetBoolean (4);
            bool volunteerable = reader.GetBoolean (5);
            bool overridable = reader.GetBoolean (6);
            bool covert = reader.GetBoolean (7);
            int reportsToStandardPositionId = reader.GetInt32 (8);
            int dotReportsToStandardPositionId = reader.GetInt32 (9);
            int minCount = reader.GetInt32 (10);
            int maxCount = reader.GetInt32 (11);

            return new BasicPosition (standardPositionId, organizationId, positionLevel, 0 /*geographyId*/, overridable, 
                0 /*overridesId*/, 0 /*createdByPerson*/, DateTime.MinValue /*createdDateTimeUtc*/, positionTypeName, 
                true /*inheritsDownward*/, volunteerable, active, covert, reportsToStandardPositionId,
                dotReportsToStandardPositionId, minCount, maxCount);
        }

        private static BasicPosition ReadAdditionalPositionFromDataReader(IDataRecord reader)
        {
            int additionalPositionId = reader.GetInt32 (0);
            int organizationId = reader.GetInt32 (1);
            int geographyId = reader.GetInt32 (2);
            int overridesHigherPositionId = reader.GetInt32 (3);
            int createdByPersonId = reader.GetInt32 (4);
            DateTime createdDateTimeUtc = reader.GetDateTime (5);
            string positionTypeName = reader.GetString (6);
            bool inheritsDownward = reader.GetBoolean (7);
            bool volunteerable = reader.GetBoolean (8);
            bool active = reader.GetBoolean (9);
            bool covert = reader.GetBoolean (10);
            int reportsToStandardPositionId = reader.GetInt32 (11);
            int reportsToAdditionalPositionId = reader.GetInt32 (12);
            int dotReportsToStandardPositionId = reader.GetInt32 (13);
            int minCount = reader.GetInt32 (14);
            int maxCount = reader.GetInt32 (15);

            int reportsToPositionId = (reportsToAdditionalPositionId != 0)
                ? -reportsToAdditionalPositionId
                : reportsToStandardPositionId;

            return new BasicPosition (-additionalPositionId, organizationId, PositionLevel.Geography, 
                geographyId, false /*overridable*/, overridesHigherPositionId,
                createdByPersonId, createdDateTimeUtc, positionTypeName, inheritsDownward, volunteerable, active, covert,
                reportsToPositionId, dotReportsToStandardPositionId, minCount, maxCount);

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

        public BasicPosition GetPosition (int positionId)
        {
            if (positionId > 0)
            {
                return GetStandardPosition (positionId);
            }
            else
            {
                return GetAdditionalPosition (-positionId);
            }
        }

        public BasicPosition[] GetPositions (params object[] conditions)
        {
            BasicPosition[] standardHits = GetStandardPositions (conditions);
            BasicPosition[] additionalHits = GetAdditionalPositions (conditions);

            return standardHits.Concat (additionalHits).ToArray();

        }

        #region Standard Positions

        public BasicPosition GetStandardPosition (int standardPositionId)
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
                        return ReadStandardPositionFromDataReader(reader);
                    }

                    throw new ArgumentException("No such StandardPositionId:" + standardPositionId);
                }
            }
        }


        public BasicPosition[] GetStandardPositions(params object[] conditions)
        {
            List<BasicPosition> result = new List<BasicPosition>();

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
                        result.Add(ReadStandardPositionFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion


        #region Additional Positions

        public BasicPosition GetAdditionalPosition(int additionalPositionId)
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
                        return ReadAdditionalPositionFromDataReader(reader);
                    }

                    throw new ArgumentException("No such StandardPositionId:" + additionalPositionId);
                }
            }
        }


        public BasicPosition[] GetAdditionalPositions(params object[] conditions)
        {
            List<BasicPosition> result = new List<BasicPosition>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + additionalPositionFieldSequence + ConstructWhereClause("PositionsAdditional", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadAdditionalPositionFromDataReader(reader));
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
                        "SELECT" + standardPositionFieldSequence + ConstructWhereClause("PositionsStandard", conditions), connection);

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


        public int CreateStandardPosition(int organizationId, PositionLevel positionLevel, string positionType,
            bool volunteerable, bool overridable, int reportsToPositionId, int dotReportsToPositionId, int minCount,
            int maxCount)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateStandardPosition", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "positionLevel", (int)positionLevel);
                AddParameterWithName(command, "positionType", positionType);
                AddParameterWithName(command, "volunteerable", volunteerable);
                AddParameterWithName(command, "overridable", positionType);
                AddParameterWithName(command, "reportsToStandardPositionId", reportsToPositionId);
                AddParameterWithName(command, "dotReportsToStandardPositionId", dotReportsToPositionId);
                AddParameterWithName(command, "minCount", minCount);
                AddParameterWithName(command, "maxCount", maxCount);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int CreateAdditionalPosition(int organizationId, int geographyId, string positionType,
            int overridesStandardPositionId, bool volunteerable, bool inheritsDownward, int reportsToPositionId, int dotReportsToPositionId, 
            int createdByPersonId, int minCount, int maxCount)
        {
            DateTime utcNow = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateAdditionalPosition", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "positionType", positionType);
                AddParameterWithName(command, "volunteerable", volunteerable);
                AddParameterWithName(command, "overridable", positionType);
                AddParameterWithName(command, "reportsToStandardPositionId", reportsToPositionId > 0 ? reportsToPositionId : 0);
                AddParameterWithName(command, "reportsToAdditionalPositionId", reportsToPositionId < 0 ? -reportsToPositionId : 0);
                AddParameterWithName(command, "dotReportsToStandardPositionId", dotReportsToPositionId);
                AddParameterWithName(command, "createdDateTimeUtc", utcNow);
                AddParameterWithName(command, "minCount", minCount);
                AddParameterWithName(command, "maxCount", maxCount);

                return -Convert.ToInt32(command.ExecuteScalar());  // Do note the negative! That's intentional.
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