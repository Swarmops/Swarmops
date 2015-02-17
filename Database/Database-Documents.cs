using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Common;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public BasicDocument GetDocument (int documentId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT Documents.DocumentId,Documents.ServerFileName,Documents.ClientFileName,Documents.Description,DocumentTypes.Name AS DocumentType,Documents.ForeignId,Documents.FileSize,Documents.UploadedByPersonId,Documents.UploadedDateTime From Documents,DocumentTypes " +
                        "WHERE Documents.DocumentTypeId=DocumentTypes.DocumentTypeId AND " +
                        "Documents.DocumentId = " + documentId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadDocumentFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown Document Id");
                }
            }
        }


        public BasicDocument[] GetDocumentsForForeignObject (DocumentType documentType, int foreignId)
        {
            List<BasicDocument> result = new List<BasicDocument>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT Documents.DocumentId,Documents.ServerFileName,Documents.ClientFileName,Documents.Description,DocumentTypes.Name AS DocumentType,Documents.ForeignId,Documents.FileSize,Documents.UploadedByPersonId,Documents.UploadedDateTime From Documents,DocumentTypes " +
                        "WHERE Documents.DocumentTypeId=DocumentTypes.DocumentTypeId AND " +
                        "Documents.ForeignId = " + foreignId + " AND " +
                        "DocumentTypes.Name = '" + documentType + "';", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadDocumentFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicDocument[] GetDocumentsRecentByDescription (string description)
        {
            List<BasicDocument> result = new List<BasicDocument>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                string sqlQuery =
                    "SELECT Documents.DocumentId,Documents.ServerFileName,Documents.ClientFileName,Documents.Description,DocumentTypes.Name AS DocumentType,Documents.ForeignId,Documents.FileSize,Documents.UploadedByPersonId,Documents.UploadedDateTime From Documents,DocumentTypes " +
                    "WHERE Documents.DocumentTypeId=DocumentTypes.DocumentTypeId AND " +
                    "Documents.Description = '" + description.Replace ("'", "''") + "' AND " +
                    "Documents.UploadedDateTime > '" + DateTime.UtcNow.AddDays (-1).ToString ("yyyy-MM-dd HH:mm") + "'";

                DbCommand command = GetDbCommand (sqlQuery, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadDocumentFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        private BasicDocument ReadDocumentFromDataReader (DbDataReader reader)
        {
            int documentId = reader.GetInt32 (0);
            string serverFileName = reader.GetString (1);
            string clientFileName = reader.GetString (2);
            string description = reader.GetString (3);
            DocumentType docType = (DocumentType) Enum.Parse (typeof (DocumentType), reader.GetString (4));
            int foreignId = reader.GetInt32 (5);
            Int64 fileSize = reader.GetInt64 (6);
            int uploadedByPersonId = reader.GetInt32 (7);
            DateTime uploadedDateTime = reader.GetDateTime (8);

            return new BasicDocument (documentId, serverFileName, clientFileName, description, docType,
                foreignId, fileSize, uploadedByPersonId, uploadedDateTime);
        }


        public int CreateDocument (string serverFileName, string clientFileName, long fileSize, string description,
            DocumentType documentType, int foreignId, int uploadedByPersonId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateDocument", connection);
                command.CommandType = CommandType.StoredProcedure;

                // HACK - this bypasses the DB abstraction layer -- debug code to find out why
                // CreateDocument only gets 4 parameters from the DB abstraction layer, when
                // we give it 8

                command.Parameters.Add (new MySqlParameter ("serverFileName", serverFileName));
                command.Parameters.Add (new MySqlParameter ("clientFileName", clientFileName));
                command.Parameters.Add (new MySqlParameter ("description", description));
                command.Parameters.Add (new MySqlParameter ("docTypeString", documentType.ToString()));
                command.Parameters.Add (new MySqlParameter ("foreignId", foreignId));
                command.Parameters.Add (new MySqlParameter ("fileSize", fileSize));
                command.Parameters.Add (new MySqlParameter ("uploadedByPersonId", uploadedByPersonId));
                command.Parameters.Add (new MySqlParameter ("uploadedDateTime", DateTime.UtcNow));

                /*
                AddParameterWithName(command, "serverFileName", serverFileName);
                AddParameterWithName(command, "clientFileName", clientFileName);
                AddParameterWithName(command, "description", description);
                AddParameterWithName(command, "docTypeString", documentType.ToString());
                AddParameterWithName(command, "foreignId", foreignId);
                AddParameterWithName(command, "fileSize", fileSize);
                AddParameterWithName(command, "uploadedByPersonId", uploadedByPersonId);
                AddParameterWithName(command, "uploadedDateTime", DateTime.Now);*/

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        public void SetDocumentServerFileName (int documentId, string serverFileName)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetDocumentServerFileName", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "documentId", documentId);
                AddParameterWithName (command, "newServerFileName", serverFileName);

                command.ExecuteNonQuery();
            }
        }


        public void SetDocumentDescription (int documentId, string newDescription)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetDocumentDescription", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "documentId", documentId);
                AddParameterWithName (command, "newDescription", newDescription);

                command.ExecuteNonQuery();
            }
        }


        public void SetDocumentForeignObject (int documentId, DocumentType newDocumentType, int newForeignId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetDocumentForeignObject", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "documentId", documentId);
                AddParameterWithName (command, "newDocumentTypeString", newDocumentType.ToString());
                AddParameterWithName (command, "newForeignId", newForeignId);

                command.ExecuteNonQuery();
            }
        }


        // The rest was copied from Database-Financials as template code.

        /*
        public BasicFinancialAccount GetFinancialAccount(int financialAccountId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT FinancialAccountId,Name,OrganizationId,AccountType From FinancialAccounts WHERE FinancialAccountId=" +
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
                        "SELECT FinancialAccountId,Name,OrganizationId,AccountType From FinancialAccounts WHERE OrganizationId=" +
                        organizationId + ";", connection);

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
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT FinancialTransactionId,OrganizationId,DateTime,Comment,ImportHash From FinancialTransactions WHERE FinancialTransactionId=" +
                        financialTransactionId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadFinancialTransactionFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Transaction Id");
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


        public int GetBudgetId(int organizationId, int year)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "select OrganizationBudgetId from OrganizationBudgets where OrganizationId=" + organizationId +
                        " AND OrganizationBudgets.Year=" + year + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }

                    throw new ArgumentException("No budget created for this org and year");
                }
            }
        }


        public double GetBudgetAmount(int budgetId, int financialAccountId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "select Amount from OrganizationBudgetAccounts where OrganizationBudgetId=" + budgetId +
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

            return new BasicFinancialAccount(accountId, name, accountType, organizationId);
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
        }*/
    }
}