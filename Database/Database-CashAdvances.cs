using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {

        #region Database field reading

        private const string cashAdvanceFieldSequence =
            " CashAdvanceId,OrganizationId,PersonId,CreatedDateTime,CreatedByPersonId," +            // 0-4
            "FinancialAccountId,AmountCents,Description,Open,Attested," +                            // 5-9
            "PaidOut,AttestedByPersonId,AttestedDateTime " +                                         // 10-12
            "FROM CashAdvances ";

        private static BasicCashAdvance ReadCashAdvanceFromDataReader(IDataRecord reader)
        {
            int cashAdvanceId = reader.GetInt32(0);
            int organizationId = reader.GetInt32(1);
            int personId = reader.GetInt32(2);
            DateTime createdDateTime = reader.GetDateTime(3);
            int createdByPersonId = reader.GetInt32(4);

            int financialAccountId = reader.GetInt32(5);
            Int64 amountCents = reader.GetInt64(6);
            string description = reader.GetString(7);
            bool open = reader.GetBoolean(8);
            bool attested = reader.GetBoolean(9);

            bool paidOut = reader.GetBoolean(10);
            int attestedByPersonId = reader.GetInt32(11);
            DateTime attestedDateTime = reader.GetDateTime(12);

            return new BasicCashAdvance(cashAdvanceId, organizationId, personId, createdDateTime, createdByPersonId,
                financialAccountId, amountCents, description, open, attested,
                paidOut, attestedByPersonId, attestedDateTime);
        }


        #endregion


        #region Database record reading -- SELECT clauses


        public BasicCashAdvance GetCashAdvance (int cashAdvanceId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT " + cashAdvanceFieldSequence +
                    " WHERE CashAdvanceId=" + cashAdvanceId.ToString(),
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadCashAdvanceFromDataReader(reader);
                    }

                    throw new ArgumentException("No such CashAdvanceId:" + cashAdvanceId.ToString());
                }
            }

        }


        public BasicCashAdvance[] GetCashAdvances(params object[] conditions)
        {
            List<BasicCashAdvance> result = new List<BasicCashAdvance>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + cashAdvanceFieldSequence + ConstructWhereClause("CashAdvances", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadCashAdvanceFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        #endregion


        #region Creation and manipulation -- stored procedures


        public int CreateCashAdvance(int personId, int createdByPersonId, int organizationId, int financialAccountId,
            Int64 amountCents, string description)
        {
            if (description.Length > 128)
            {
                description = description.Substring(0, 128); // varchar(128) field
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateCashAdvance", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "createdByPersonId", createdByPersonId);
                AddParameterWithName(command, "createdDateTime", DateTime.Now);
                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "amountCents", amountCents);
                AddParameterWithName(command, "financialAccountId", financialAccountId);
                AddParameterWithName(command, "description", description);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        // TODO: SetCashAdvanceAttested


        public void SetCashAdvanceOpen(int cashAdvanceId, bool open)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetCashAdvanceOpen", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "cashAdvanceId", cashAdvanceId);
                AddParameterWithName(command, "open", open);

                command.ExecuteNonQuery();
            }
        }


        public void SetCashAdvancePaidOut(int cashAdvanceId, bool paidOut)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetCashAdvancePaidOut", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "cashAdvanceId", cashAdvanceId);
                AddParameterWithName(command, "paidOut", paidOut);

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