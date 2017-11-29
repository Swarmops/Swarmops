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
        #region Field reading code

        private const string outboundInvoiceFieldSequence =
            " OutboundInvoiceId,CustomerName,InvoiceAddressPaper,InvoiceAddressMail,CurrencyId," + // 0-4
            "OrganizationId,OrganizationSequenceId,BudgetId,CreatedDateTime,CreatedByPersonId," + // 5-9
            "DueDate,ReminderCount,Reference,Domestic,Open, " + // 10-14
            "Sent,SecurityCode,TheirReference " +
            "FROM OutboundInvoices ";

        private const string outboundInvoiceItemFieldSequence =
            " OutboundInvoiceItemId,OutboundInvoiceId,Description,AmountCents " +
            "FROM OutboundInvoiceItems ";

        private const string outboundInvoiceItemSortOrder =
            " ORDER BY SortOrder,OutboundInvoiceItemId ";

        private BasicOutboundInvoice ReadOutboundInvoiceFromDataReader (DbDataReader reader)
        {
            int outboundInvoiceId = reader.GetInt32 (0);
            string customerName = reader.GetString (1);
            string invoiceAddressPaper = reader.GetString (2);
            string invoiceAddressMail = reader.GetString (3);
            int currencyId = reader.GetInt32 (4);
            int organizationId = reader.GetInt32 (5);
            int organizationSequenceId = reader.GetInt32(6);
            int budgetId = reader.GetInt32 (7);
            DateTime createdDateTime = reader.GetDateTime (8);
            int createdByPersonId = reader.GetInt32 (9);
            DateTime dueDate = reader.GetDateTime (10);
            int reminderCount = reader.GetInt32 (11);
            string reference = reader.GetString (12);
            bool domestic = reader.GetBoolean (13);
            bool open = reader.GetBoolean (14);
            bool sent = reader.GetBoolean (15);
            string securityCode = reader.GetString (16);
            string theirReference = reader.GetString (17);

            return new BasicOutboundInvoice (outboundInvoiceId, customerName, invoiceAddressPaper, invoiceAddressMail,
                currencyId, organizationId, organizationSequenceId, budgetId, createdDateTime,
                createdByPersonId, dueDate, reminderCount, reference, domestic, open, sent, securityCode, theirReference);
        }

        private BasicOutboundInvoiceItem ReadOutboundInvoiceItemFromDataReader (DbDataReader reader)
        {
            int outboundInvoiceItemId = reader.GetInt32 (0);
            int outboundInvoiceId = reader.GetInt32 (1);
            string description = reader.GetString (2);
            Int64 amountCents = reader.GetInt64 (3);

            return new BasicOutboundInvoiceItem (outboundInvoiceItemId, outboundInvoiceId, description, amountCents);
        }

        #endregion

        #region Record reading - SELECT statements

        public BasicOutboundInvoice GetOutboundInvoice (int outboundInvoiceId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + outboundInvoiceFieldSequence + "WHERE OutboundInvoiceId=" + outboundInvoiceId + ";",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadOutboundInvoiceFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown OutboundInvoiceId");
                }
            }
        }


        /// <summary>
        ///     Gets a list of outbound invoices.
        /// </summary>
        /// <param name="conditions">Optional Organization object and/or DatabaseConditions.</param>
        /// <returns>The inbound invoice list.</returns>
        public BasicOutboundInvoice[] GetOutboundInvoices (params object[] conditions)
        {
            List<BasicOutboundInvoice> result = new List<BasicOutboundInvoice>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + outboundInvoiceFieldSequence + ConstructWhereClause ("OutboundInvoices", conditions) +
                        " ORDER BY DueDate;", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadOutboundInvoiceFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicOutboundInvoiceItem GetOutboundInvoiceItem (int outboundInvoiceItemId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + outboundInvoiceItemFieldSequence + "WHERE OutboundInvoiceItemId=" +
                        outboundInvoiceItemId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadOutboundInvoiceItemFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown OutboundInvoiceId");
                }
            }
        }


        /// <summary>
        ///     Gets a list of outbound invoice items.
        /// </summary>
        /// <param name="conditions">OutboundInvoice object.</param>
        /// <returns>The inbound invoice list.</returns>
        public BasicOutboundInvoiceItem[] GetOutboundInvoiceItems (params object[] conditions)
        {
            List<BasicOutboundInvoiceItem> result = new List<BasicOutboundInvoiceItem>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + outboundInvoiceItemFieldSequence +
                        ConstructWhereClause ("OutboundInvoiceItems", conditions) + outboundInvoiceItemSortOrder + ";",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadOutboundInvoiceItemFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Creation and manipulation - stored procedures

        public int CreateOutboundInvoice (int organizationId, int createdByPersonId, DateTime dueDate,
            int budgetId, string customerName, string invoiceAddressPaper, string invoiceAddressMail,
            int currencyId, string reference, bool domestic, string securityCode, string theirReference)

        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateOutboundInvoice", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "organizationId", organizationId);
                AddParameterWithName (command, "createdDateTime", DateTime.Now);
                AddParameterWithName (command, "createdByPersonId", createdByPersonId);
                AddParameterWithName (command, "dueDate", dueDate);
                AddParameterWithName (command, "budgetId", budgetId);
                AddParameterWithName (command, "customerName", customerName);
                AddParameterWithName (command, "invoiceAddressPaper", invoiceAddressPaper);
                AddParameterWithName (command, "invoiceAddressMail", invoiceAddressMail);
                AddParameterWithName (command, "currencyId", currencyId);
                AddParameterWithName (command, "reference", reference);
                AddParameterWithName (command, "domestic", domestic);
                AddParameterWithName (command, "securityCode", securityCode);
                AddParameterWithName (command, "TheirReference", theirReference);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        public int SetOutboundInvoiceSequence(int outboundInvoiceId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetOutboundInvoiceOrganizationSequenceId", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "outboundInvoiceId", outboundInvoiceId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }




        public int CreateOutboundInvoiceItem (int outboundInvoiceId, string description, double amount)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateOutboundInvoiceItem", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundInvoiceId", outboundInvoiceId);
                AddParameterWithName (command, "description", description);
                AddParameterWithName (command, "amount", amount);
                AddParameterWithName (command, "sortOrder", 1);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        public int CreateOutboundInvoiceItem (int outboundInvoiceId, string description, Int64 amountCents)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateOutboundInvoiceItemPrecise", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundInvoiceId", outboundInvoiceId);
                AddParameterWithName (command, "description", description);
                AddParameterWithName (command, "amountCents", amountCents);
                AddParameterWithName (command, "sortOrder", 1);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        public void SetOutboundInvoiceOpen (int outboundInvoiceId, bool open)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundInvoiceOpen", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundInvoiceId", outboundInvoiceId);
                AddParameterWithName (command, "open", open);

                command.ExecuteNonQuery();
            }
        }


        public void SetOutboundInvoiceSent (int inboundInvoiceId, bool sent)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundInvoiceSent", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundInvoiceId", inboundInvoiceId);
                AddParameterWithName (command, "sent", sent);

                command.ExecuteNonQuery();
            }
        }


        public void SetOutboundInvoiceReference (int inboundInvoiceId, string reference)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundInvoiceReference", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundInvoiceId", inboundInvoiceId);
                AddParameterWithName (command, "reference", reference);

                command.ExecuteNonQuery();
            }
        }


        public void SetOutboundInvoiceBudget (int outboundInvoiceId, int budgetId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundInvoiceBudget", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundInvoiceId", outboundInvoiceId);
                AddParameterWithName (command, "budgetId", budgetId);

                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Dead template code

        /*
        public int CreateFinancialAccount(int pOrganizationId, string pName, FinancialAccountType pAccountType, int pParentFinancialAccountId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateFinancialAccount", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "pOrganizationId", pOrganizationId);
                AddParameterWithName(command, "pName", pName);
                AddParameterWithName(command, "pAccountType", (int)pAccountType);
                AddParameterWithName(command, "pParentFinancialAccountId", pParentFinancialAccountId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public BasicFinancialAccount GetFinancialAccount(int financialAccountId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT FinancialAccountId,Name,OrganizationId,AccountType,ParentFinancialAccountId,OwnerPersonId From FinancialAccounts WHERE FinancialAccountId=" +
                        financialAccountId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadFinancialAccountFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Account Id");
                }
            }
        }

        // This is WAAAAY too much repetition of code. Couldn't you create two generic functions with
        // this structure, call them GetDatabaseObject and GetDatabaseCollection, and pass the query,
        // the reader delegate, and the expected type?

        public BasicFinancialAccount[] GetFinancialAccountsForOrganization(int organizationId)
        {
            var result = new List<BasicFinancialAccount>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT FinancialAccountId,Name,OrganizationId,AccountType,ParentFinancialAccountId,OwnerPersonId From FinancialAccounts WHERE OrganizationId=" +
                        organizationId + " ORDER BY AccountType, Name;", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadFinancialAccountFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicFinancialTransaction GetFinancialTransaction(int financialTransactionId)
        {
            var array = GetFinancialTransactions(new int[] { financialTransactionId });

            if (array.Length == 0)
            {
                throw new ArgumentException("No such FinancialTransactionId");
            }

            return array[0];
        }

        public BasicFinancialTransaction[] GetFinancialTransactions(int[] financialTransactionIds)
        {
            List<BasicFinancialTransaction> result = new List<BasicFinancialTransaction>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT FinancialTransactionId,OrganizationId,DateTime,Comment,ImportHash From FinancialTransactions WHERE FinancialTransactionId IN (" +
                        JoinIds(financialTransactionIds) + ");", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadFinancialTransactionFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public double GetFinancialAccountBalanceTotal(int financialAccountId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT Sum(Amount) FROM FinancialTransactionRows WHERE FinancialAccountId=" +
                        financialAccountId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetDouble(0);
                    }

                    throw new ArgumentException("Unknown Account Id");
                }
            }
        }

        public double GetFinancialAccountBalanceDelta(int financialAccountId, DateTime startDate, DateTime endDate)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "select Sum(FinancialTransactionRows.Amount),Count(FinancialTransactions.FinancialTransactionId) from FinancialTransactionRows,FinancialTransactions Where FinancialTransactionRows.FinancialAccountId=" +
                        financialAccountId +
                        " AND FinancialTransactionRows.FinancialTransactionId=FinancialTransactions.FinancialTransactionId AND FinancialTransactionRows.Deleted=0 AND FinancialTransactions.DateTime >= '" +
                        startDate.ToString("yyyy-MM-dd HH:mm:ss") + "' AND FinancialTransactions.DateTime < '" +
                        endDate.ToString("yyyy-MM-dd HH:mm:ss") + "';", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                        {
                            // No rows, so no delta

                            return 0.0;
                        }

                        return reader.GetDouble(0);
                    }

                    throw new ArgumentException("Unknown Account Id");
                }
            }
        }

        public BasicFinancialAccountRow[] GetFinancialAccountRows(int financialAccountId, DateTime startDateTime,
                                                                   DateTime endDateTime)
        {
            return GetFinancialAccountRows(new int[] { financialAccountId }, startDateTime, endDateTime);
        }


        public BasicFinancialAccountRow[] GetFinancialAccountRows(int[] financialAccountIds, DateTime startDateTime,
                                                                   DateTime endDateTime)
        {
            var result = new List<BasicFinancialAccountRow>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "select FinancialTransactionRows.FinancialAccountId,FinancialTransactionRows.FinancialTransactionId,FinancialTransactions.DateTime,FinancialTransactions.Comment,FinancialTransactionRows.Amount FROM FinancialTransactions,FinancialTransactionRows WHERE FinancialTransactionRows.Deleted=0 AND FinancialTransactions.FinancialTransactionId=FinancialTransactionRows.FinancialTransactionId AND FinancialTransactionRows.FinancialAccountId IN (" +
                        JoinIds(financialAccountIds) + ") AND DateTime >= '" +
                        startDateTime.ToString("yyyy-MM-dd HH:mm:ss") +
                        "' AND DateTime < '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY DateTime;",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadFinancialAccountRowFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicFinancialTransactionRow[] GetFinancialTransactionRows(int financialTransactionId)
        {
            var result = new List<BasicFinancialTransactionRow>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "select FinancialTransactionRowId, FinancialAccountId," + financialTransactionId +
                        " AS FinancialTransactionId,Amount FROM FinancialTransactionRows WHERE FinancialTransactionId=" +
                        financialTransactionId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadFinancialTransactionRowFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicFinancialTransactionRow GetFinancialTransactionRow(int financialTransactionRowId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "select FinancialTransactionRowId,FinancialAccountId,FinancialTransactionId,Amount FROM FinancialTransactionRows WHERE FinancialTransactionRowId=" +
                        financialTransactionRowId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadFinancialTransactionRowFromDataReader(reader);
                    }

                    throw new ArgumentException("Invalid FinancialTransactionRowId");
                }
            }
        }


        public BasicFinancialTransaction[] GetUnbalancedFinancialTransactions(int organizationId)
        {
            var result = new List<BasicFinancialTransaction>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                string commandString = "select FinancialTransactions.FinancialTransactionId, " +
                                       "FinancialTransactions.OrganizationId, FinancialTransactions.DateTime, " +
                                       "FinancialTransactions.Comment, FinancialTransactions.ImportHash, " +
                                       "SUM(FinancialTransactionRows.Amount) AS Delta " +
                                       "FROM FinancialTransactions,FinancialTransactionRows " +
                                       "WHERE FinancialTransactionRows.FinancialTransactionId=FinancialTransactions.FinancialTransactionId AND " +
                                       "FinancialTransactions.OrganizationId=" + organizationId + " AND " +
                                       "FinancialTransactionRows.Deleted=0 " +
                                       "GROUP BY FinancialTransactions.FinancialTransactionId HAVING Delta <> 0.0 " +
                                       "ORDER BY FinancialTransactions.DateTime;";

                DbCommand command = GetDbCommand(commandString, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadFinancialTransactionFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public double GetFinancialAccountBudget(int financialAccountId, int year)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "select Amount from FinancialAccountBudgets where Year=" + year +
                        " AND FinancialAccountId=" + financialAccountId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetDouble(0);
                    }

                    return 0.0;
                }
            }
        }

        public void SetFinancialAccountBudget(int financialAccountId, int year, double amount)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetFinancialAccountBudget", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "financialAccountId", financialAccountId);
                AddParameterWithName(command, "year", year);
                AddParameterWithName(command, "amount", amount);

                command.ExecuteNonQuery();
            }
        }


        public void SetFinancialAccountOwner(int financialAccountId, int ownerPersonId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetFinancialAccountOwner", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "financialAccountId", financialAccountId);
                AddParameterWithName(command, "ownerPersonId", ownerPersonId);

                command.ExecuteNonQuery();
            }
        }


        private BasicFinancialTransactionRow ReadFinancialTransactionRowFromDataReader(DbDataReader reader)
        {
            int rowId = reader.GetInt32(0);
            int accountId = reader.GetInt32(1);
            int transactionId = reader.GetInt32(2);
            double amount = reader.GetDouble(3);

            return new BasicFinancialTransactionRow(rowId, accountId, transactionId, amount);
        }

        private BasicFinancialAccount ReadFinancialAccountFromDataReader(DbDataReader reader)
        {
            int accountId = reader.GetInt32(0);
            string name = reader.GetString(1);
            int organizationId = reader.GetInt32(2);
            var accountType = (FinancialAccountType)reader.GetInt32(3);
            int parentFinancialAccountId = reader.GetInt32(4);
            int ownerPersonId = reader.GetInt32(5);

            return new BasicFinancialAccount(accountId, name, accountType, organizationId, parentFinancialAccountId, ownerPersonId);
        }

        private BasicFinancialAccountRow ReadFinancialAccountRowFromDataReader(DbDataReader reader)
        {
            int accountId = reader.GetInt32(0);
            int transactionId = reader.GetInt32(1);
            DateTime dateTime = reader.GetDateTime(2);
            string comment = reader.GetString(3);
            double amount = reader.GetDouble(4);

            return new BasicFinancialAccountRow(accountId, transactionId, dateTime, comment, amount);
        }


        private BasicFinancialTransaction ReadFinancialTransactionFromDataReader(DbDataReader reader)
        {
            int transactionId = reader.GetInt32(0);
            int organizationId = reader.GetInt32(1);
            DateTime dateTime = reader.GetDateTime(2);
            string comment = reader.GetString(3);
            string importHash = reader.GetString(4);

            return new BasicFinancialTransaction(transactionId, organizationId, dateTime, comment, importHash);
        }

        public int CreateFinancialTransaction(int organizationId, DateTime dateTime, string comment)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateFinancialTransaction", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "dateTime", dateTime);
                AddParameterWithName(command, "comment", comment);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int CreateFinancialTransactionStub(int organizationId, DateTime dateTime, int financialAccountId,
                                                   double amount, string comment, string importHash, int personId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateFinancialTransactionStub", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "dateTime", dateTime);
                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "financialAccountId", financialAccountId);
                AddParameterWithName(command, "comment", comment);
                AddParameterWithName(command, "importHash", importHash);
                AddParameterWithName(command, "amount", amount);
                AddParameterWithName(command, "personId", personId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void CreateFinancialTransactionRow(int financialTransactionId, int financialAccountId, double amount,
                                                   int personId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateFinancialTransactionRow", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "financialTransactionId", financialTransactionId);
                AddParameterWithName(command, "financialAccountId", financialAccountId);
                AddParameterWithName(command, "amount", amount);
                AddParameterWithName(command, "dateTime", DateTime.Now);
                AddParameterWithName(command, "personId", personId);

                command.ExecuteNonQuery();
            }
        }

        public void SetFinancialTransactionDescription(int financialTransactionId, string description)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetFinancialTransactionDescription", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "financialTransactionId", financialTransactionId);
                AddParameterWithName(command, "description", description);

                command.ExecuteNonQuery();
            }
        }


        public void SetFinancialTransactionDependency(int financialTransactionId, FinancialDependencyType dependencyType, int foreignId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetFinancialTransactionDependency", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "financialTransactionId", financialTransactionId);
                AddParameterWithName(command, "financialDependencyType", dependencyType.ToString());
                AddParameterWithName(command, "foreignId", foreignId);

                command.ExecuteNonQuery();
            }
        }


        public void ClearFinancialTransactionDependency(int financialTransactionId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("ClearFinancialTransactionDependency", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "financialTransactionId", financialTransactionId);

                command.ExecuteNonQuery();
            }
        }


        public BasicFinancialTransaction[] GetDependentFinancialTransactions(FinancialDependencyType dependencyType, int foreignId)
        {
            List<int> transactionIds = new List<int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT FinancialTransactionId FROM FinancialTransactionDependencies,FinancialDependencyTypes " +
                        "WHERE FinancialDependencyTypes.Name='" + dependencyType.ToString() + "' AND " +
                        "FinancialDependencyTypes.FinancialDependencyTypeId=FinancialTransactionDependencies.FinancialDependencyTypeId AND " +
                        "FinancialTransactionDependencies.ForeignId=" + foreignId.ToString(), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactionIds.Add(reader.GetInt32(0));
                    }
                }
            }

            if (transactionIds.Count == 0)
            {
                return new BasicFinancialTransaction[0];
            }

            return GetFinancialTransactions(transactionIds.ToArray());
        }


        // The function below uses OUT parameters, which is a no-go according to .Net Design Guidelines.
        // Fix this by introducing the BasicFinancialDependency type in the semi-near future, and
        // using it as a return type.

        public void GetFinancialTransactionDependency(int financialTransactionId, out FinancialDependencyType dependencyType, out int foreignId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT FinancialDependencyTypes.Name,FinancialTransactionDependencies.ForeignId " +
                        "FROM FinancialTransactionDependencies,FinancialDependencyTypes " +
                        "WHERE FinancialDependencyTypes.FinancialDependencyTypeId=FinancialTransactionDependencies.FinancialDependencyTypeId " +
                        "AND FinancialTransactionDependencies.FinancialTransactionId=" + financialTransactionId.ToString(), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Set OUT parameters

                        dependencyType = (FinancialDependencyType)Enum.Parse(typeof(FinancialDependencyType), reader.GetString(0));
                        foreignId = reader.GetInt32(1);
                    }
                    else
                    {
                        // Set OUT parameters

                        dependencyType = FinancialDependencyType.Unknown;
                        foreignId = 0;
                    }
                }
            }

        }



        // TODO: Return BasicFinancialValidation object

        public void CreateFinancialValidation(FinancialValidationType validationType, FinancialDependencyType dependencyType, int foreignId,
            DateTime validatedDateTime, int personId, double amount)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateFinancialValidation", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "validationType", validationType.ToString());
                AddParameterWithName(command, "dependencyType", dependencyType.ToString());
                AddParameterWithName(command, "foreignId", foreignId);
                AddParameterWithName(command, "validatedDateTime", validatedDateTime);
                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "amount", amount);

                command.ExecuteNonQuery();
            }
        }






        // -- lines and trees and children --


        public BasicFinancialAccount[] GetFinancialAccountChildren(int parentFinancialAccountId)
        {
            List<BasicFinancialAccount> result = new List<BasicFinancialAccount>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT FinancialAccountId,Name,OrganizationId,AccountType,ParentFinancialAccountId,OwnerPersonId FROM FinancialAccounts WHERE ParentFinancialAccountId = " + parentFinancialAccountId.ToString() +
                        " ORDER BY \"Name\"", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadFinancialAccountFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public Dictionary<int, List<BasicFinancialAccount>> GetHashedFinancialAccounts(int organizationId)
        {
            // This generates a Dictionary <int,List<Node>>.
            // 
            // Keys are integers corresponding to NodeIds. At each key n,
            // the value is an List<Node> starting with the node n followed by
            // its children.
            //
            // (Later reflection:) O(n) complexity, instead of recursion. Nice!

            Dictionary<int, List<BasicFinancialAccount>> result = new Dictionary<int, List<BasicFinancialAccount>>();

            BasicFinancialAccount[] nodes = GetFinancialAccountsForOrganization(organizationId);

            // Add the root.

            result[0] = new List<BasicFinancialAccount>();

            // Add the nodes.

            foreach (BasicFinancialAccount node in nodes)
            {
                List<BasicFinancialAccount> newList = new List<BasicFinancialAccount>();
                newList.Add(node);

                result[node.FinancialAccountId] = newList;
            }

            // Add the children.

            foreach (BasicFinancialAccount node in nodes)
            {
                result[node.ParentFinancialAccountId].Add(node);
            }

            return result;
        }


        public BasicFinancialAccount[] GetFinancialAccountLine(int leafFinancialAccountId)
        {
            int orgId = GetFinancialAccount(leafFinancialAccountId).OrganizationId;

            List<BasicFinancialAccount> result = new List<BasicFinancialAccount>();

            Dictionary<int, List<BasicFinancialAccount>> nodes = GetHashedFinancialAccounts(orgId);

            BasicFinancialAccount currentNode = nodes[leafFinancialAccountId][0];

            // This iterates until the zero-parentid root node is found

            while (currentNode != null && currentNode.ParentFinancialAccountId != 0)
            {
                result.Add(currentNode);

                if (currentNode.ParentFinancialAccountId != 0)
                {
                    currentNode = nodes[currentNode.ParentFinancialAccountId][0];
                }
                else
                {
                    currentNode = null;
                }
            }

            result.Reverse();

            return result.ToArray();
        }


        public BasicFinancialAccount[] GetFinancialAccountTreeForOrganization(int organizationId)
        {
            Dictionary<int, List<BasicFinancialAccount>> nodes = GetHashedFinancialAccounts(organizationId);

            return GetFinancialAccountTree(nodes, 0, 0);
        }

        private BasicFinancialAccount[] GetFinancialAccountTree(Dictionary<int, List<BasicFinancialAccount>> FinancialAccounts, int startNodeId,
                                                   int generation)
        {
            List<BasicFinancialAccount> result = new List<BasicFinancialAccount>();

            List<BasicFinancialAccount> thisList = FinancialAccounts[startNodeId];

            foreach (BasicFinancialAccount node in thisList)
            {
                if (node.FinancialAccountId != startNodeId)
                {
                    result.Add(new BasicFinancialAccount(node));

                    // Add recursively

                    BasicFinancialAccount[] children = GetFinancialAccountTree(FinancialAccounts, node.FinancialAccountId, generation + 1);

                    if (children.Length > 0)
                    {
                        foreach (BasicFinancialAccount child in children)
                        {
                            result.Add(child);
                        }
                    }
                }
                else if (generation == 0 && startNodeId != 0)
                {
                    // The top parent is special and should be added (unless null); the others shouldn't

                    result.Add(new BasicFinancialAccount(node));
                }
            }

            return result.ToArray();
        }
        */

        #endregion
    }
}