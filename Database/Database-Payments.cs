using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Database field reading

        private const string paymentFieldSequence =
            " PaymentId,PaymentGroupId,AmountCents,Reference,FromAccount," + // 0-4
            "PaymentKey,HasImage,OutboundInvoiceId " + // 5-7
            "FROM Payments ";

        private const string paymentInformationFieldSequence =
            " PaymentInformationId,PaymentId,PaymentInformationTypes.Name,Data " + // 0-4
            " FROM PaymentInformation JOIN PaymentInformationTypes USING (PaymentInformationTypeId) ";

        private static BasicPayment ReadPaymentFromDataReader (IDataRecord reader)
        {
            int paymentId = reader.GetInt32 (0);
            int paymentGroupId = reader.GetInt32 (1);
            Int64 amountCents = reader.GetInt64 (2);
            string reference = reader.GetString (3);
            string fromAccount = reader.GetString (4);
            string key = reader.GetString (5);
            bool hasImage = reader.GetBoolean (6);
            int outboundInvoiceId = reader.GetInt32 (7);

            return new BasicPayment (paymentId, paymentGroupId, amountCents, reference, fromAccount, key, hasImage,
                outboundInvoiceId);
        }

        private static BasicPaymentInformation ReadPaymentInformationFromDataReader (IDataRecord reader)
        {
            string dataTypeString = reader.GetString (2);
            string data = reader.GetString (3);

            PaymentInformationType dataType =
                (PaymentInformationType) Enum.Parse (typeof (PaymentInformationType), dataTypeString, true);

            return new BasicPaymentInformation (dataType, data);
        }

        #endregion

        #region Database record reading -- SELECT clauses

        public BasicPayment GetPayment (int paymentId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand ("SELECT" + paymentFieldSequence +
                                  "WHERE PaymentId=" + paymentId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadPaymentFromDataReader (reader);
                    }

                    throw new ArgumentException ("No such PaymentId:" + paymentId);
                }
            }
        }


        public BasicPayment[] GetPayments (params object[] conditions)
        {
            List<BasicPayment> result = new List<BasicPayment>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + paymentFieldSequence + ConstructWhereClause ("Payments", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadPaymentFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicPaymentInformation[] GetPaymentInformation (int paymentId)
        {
            List<BasicPaymentInformation> result = new List<BasicPaymentInformation>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + paymentInformationFieldSequence + " WHERE PaymentId=" + paymentId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadPaymentInformationFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Creation and manipulation -- stored procedures

        public int CreatePayment (int paymentGroupId, double amount, string reference, string fromAccount, string key,
            bool hasImage, int outboundInvoiceId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreatePayment", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "paymentGroupId", paymentGroupId);
                AddParameterWithName (command, "amount", amount);
                AddParameterWithName (command, "reference", reference);
                AddParameterWithName (command, "fromAccount", fromAccount);
                AddParameterWithName (command, "paymentKey", key);
                AddParameterWithName (command, "hasImage", hasImage);
                AddParameterWithName (command, "outboundInvoiceId", outboundInvoiceId);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        public int CreatePayment (int paymentGroupId, Int64 amountCents, string reference, string fromAccount,
            string key,
            bool hasImage, int outboundInvoiceId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreatePaymentPrecise", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "paymentGroupId", paymentGroupId);
                AddParameterWithName (command, "amountCents", amountCents);
                AddParameterWithName (command, "reference", reference);
                AddParameterWithName (command, "fromAccount", fromAccount);
                AddParameterWithName (command, "paymentKey", key);
                AddParameterWithName (command, "hasImage", hasImage);
                AddParameterWithName (command, "outboundInvoiceId", outboundInvoiceId);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        public int CreatePaymentInformation (int paymentId, PaymentInformationType type, string data)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreatePaymentInformation", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "paymentId", paymentId);
                AddParameterWithName (command, "dataTypeString", type.ToString());
                AddParameterWithName (command, "data", data);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        [Obsolete ("This should never be called, ever: it ruins trackability.")]
        public void DeletePayment (int paymentId)
        {
            // This function is necessary since the Swedish bank system only allows for a dupe check AFTER all payments
            // have been communicated, so they are created as data is received. If the data was a dupe, it is therefore
            // deleted after the dupecheck triggers.

            // That code was later re-written to do in-memory dupechecking. This delete function should never be called.

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("DeletePayment", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "paymentId", paymentId);

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