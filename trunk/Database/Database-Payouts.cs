using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {

        #region Database field reading

        private const string payoutFieldSequence = 
            " PayoutId,OrganizationId,Bank,Account,Reference," +                                      // 0-4
            "AmountCents,ExpectedTransactionDate,Open,CreatedDateTime,CreatedByPersonId " +                // 5-9
            "FROM Payouts ";

        private static BasicPayout ReadPayoutFromDataReader (IDataRecord reader)
        {
            int payoutId = reader.GetInt32(0);
            int organizationId = reader.GetInt32(1);
            string bank = reader.GetString(2);
            string account = reader.GetString(3);
            string reference = reader.GetString(4);
            Int64 amountCents = reader.GetInt64(5);
            DateTime expectedTransactionDate = reader.GetDateTime(6);
            bool open = reader.GetBoolean(7);
            DateTime createdDateTime = reader.GetDateTime(8);
            int createdByPersonId = reader.GetInt32(9);

            return new BasicPayout(payoutId, organizationId, bank, account, reference, amountCents,
                expectedTransactionDate, open, createdDateTime, createdByPersonId);
        }

        private const string payoutDependencyFieldSequence =
            " PayoutDependencies.PayoutId,FinancialDependencyTypes.Name,PayoutDependencies.ForeignId " +  // 0-2
            "FROM PayoutDependencies,FinancialDependencyTypes ";

        private const string payoutDependencyTableJoin =
            " FinancialDependencyTypes.FinancialDependencyTypeId=PayoutDependencies.FinancialDependencyTypeId ";

        private static BasicFinancialDependency ReadPayoutDependencyFromDataReader(IDataRecord reader)
        {
            int payoutId = reader.GetInt32(0);
            string financialDependencyTypeString = reader.GetString(1);
            int foreignId = reader.GetInt32(2);

            return new BasicFinancialDependency(payoutId, (FinancialDependencyType) Enum.Parse(typeof (FinancialDependencyType), financialDependencyTypeString), foreignId);
        }


        #endregion


        #region Database record reading -- SELECT clauses


        public BasicPayout GetPayout (int payoutId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT" + payoutFieldSequence +
                    "WHERE PayoutId=" + payoutId.ToString(),
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadPayoutFromDataReader(reader);
                    }

                    throw new ArgumentException("No such PayoutId:" + payoutId.ToString());
                }
            }

        }


        public BasicFinancialDependency[] GetPayoutDependencies(int payoutId)
        {
            List<BasicFinancialDependency> result = new List<BasicFinancialDependency>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT" + payoutDependencyFieldSequence +
                    "WHERE PayoutDependencies.PayoutId=" + payoutId.ToString() + " AND " + payoutDependencyTableJoin,
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPayoutDependencyFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }


        public int GetPayoutIdFromDependency(IHasIdentity foreignObject, FinancialDependencyType typeName)
        {
            return GetPayoutIdFromDependency(foreignObject.Identity, typeName.ToString());
        }

        private int GetPayoutIdFromDependency(int foreignObjectId, string financialDependencyTypeName)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT" + payoutDependencyFieldSequence +
                    "WHERE PayoutDependencies.ForeignId=" + foreignObjectId.ToString(CultureInfo.InvariantCulture) +
                    " AND FinancialDependencyTypes.Name='" + financialDependencyTypeName + "'" + // enum - no sanitation necessary
                    " AND " + payoutDependencyTableJoin,
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }


        public int GetPayoutIdFromDependency(IHasIdentity foreignObject)
        {
            return GetPayoutIdFromDependency(foreignObject.Identity, GetForeignTypeString(foreignObject));
        }


        public BasicPayout[] GetPayouts(params object[] conditions)
        {
            List<BasicPayout> result = new List<BasicPayout>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + payoutFieldSequence + ConstructWhereClause("Payouts", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPayoutFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        #endregion


        #region Creation and manipulation -- stored procedures


        public int CreatePayout(int organizationId, string bank, string account, string reference,
            double amount, DateTime expectedTransactionDate, int createdByPersonId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreatePayout", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "bank", bank);
                AddParameterWithName(command, "account", account);
                AddParameterWithName(command, "reference", reference);
                AddParameterWithName(command, "amount", amount);
                AddParameterWithName(command, "expectedTransactionDate", expectedTransactionDate);
                AddParameterWithName(command, "createdDateTime", DateTime.Now);
                AddParameterWithName(command, "createdByPersonId", createdByPersonId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public int CreatePayout(int organizationId, string bank, string account, string reference,
            Int64 amountCents, DateTime expectedTransactionDate, int createdByPersonId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreatePayoutPrecise", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "bank", bank);
                AddParameterWithName(command, "account", account);
                AddParameterWithName(command, "reference", reference);
                AddParameterWithName(command, "amountCents", amountCents);
                AddParameterWithName(command, "expectedTransactionDate", expectedTransactionDate);
                AddParameterWithName(command, "createdDateTime", DateTime.Now);
                AddParameterWithName(command, "createdByPersonId", createdByPersonId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public void CreatePayoutDependency(int payoutId, FinancialDependencyType dependencyType, int foreignId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreatePayoutDependency", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "payoutId", payoutId);
                AddParameterWithName(command, "financialDependencyType", dependencyType.ToString());
                AddParameterWithName(command, "foreignId", foreignId);

                command.ExecuteNonQuery();
            }
        }


        public void MovePayoutDependencies(int fromPayoutId, int toPayoutId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("MovePayoutDependencies", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "fromPayoutId", fromPayoutId);
                AddParameterWithName(command, "toPayoutId", toPayoutId);

                command.ExecuteNonQuery();
            }
        }


        public void ClearPayoutDependencies(int payoutId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("ClearPayoutDependencies", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "payoutId", payoutId);

                command.ExecuteNonQuery();
            }
        }


        public void SetPayoutOpen(int payoutId, bool open)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetPayoutOpen", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "payoutId", payoutId);
                AddParameterWithName(command, "open", open);

                command.ExecuteNonQuery();
            }
        }


        public void SetPayoutAmount(int payoutId, double amount)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetPayoutAmount", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "payoutId", payoutId);
                AddParameterWithName(command, "amount", amount);

                command.ExecuteNonQuery();
            }
        }



        public void SetPayoutAmount(int payoutId, Int64 amountCents)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetPayoutAmountPrecise", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "payoutId", payoutId);
                AddParameterWithName(command, "amountCents", amountCents);

                command.ExecuteNonQuery();
            }
        }


        public void SetPayoutReference(int payoutId, string reference)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetPayoutReference", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "payoutId", payoutId);
                AddParameterWithName(command, "reference", reference);

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