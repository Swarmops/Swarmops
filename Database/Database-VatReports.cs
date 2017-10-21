using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Common.Exceptions;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string vatReportFieldSequence =
            " VatReportId,OrganizationId,Guid,CreatedDateTime,YearMonthStart," + // 0-4
            "MonthCount,VatReports.Open,TurnoverCents,VatInboundCents,VatOutboundCents," + // 5-9
            "OpenTransactionId,CloseTransactionId,UnderConstruction " + // 10-12
            "FROM VatReports ";

        private const string vatReportItemFieldSequence =
            " VatReportItemId,VatReportId,FinancialTransactionId,ForeignId,FinancialDependencyTypes.Name AS FinancialDependencyType," +  // 0-4
            "TurnoverCents,VatInboundCents,VatOutboundCents " +
            "FROM VatReportItems " +
            "JOIN FinancialDependencyTypes ON (VatReportItems.FinancialDependencyTypeId=FinancialDependencyTypes.FinancialDependencyTypeId) ";

        private static BasicVatReport ReadVatReportFromDataReader (IDataRecord reader)
        {
            int vatReportId = reader.GetInt32 (0);
            int organizationId = reader.GetInt32(1);
            string guid = reader.GetString(2);
            DateTime createdDateTime = reader.GetDateTime (3);
            int yearMonthStart = reader.GetInt32 (4);
            int monthCount = reader.GetInt32 (5);
            bool open = reader.GetBoolean(6);
            Int64 turnoverCents = reader.GetInt64(7);
            Int64 vatInboundCents = reader.GetInt64(8);
            Int64 vatOutboundCents = reader.GetInt64(9);
            int openTransactionId = reader.GetInt32(10);
            int closeTransactionid = reader.GetInt32(11);
            bool underConstruction = reader.GetBoolean (12);

            return new BasicVatReport(vatReportId, organizationId, guid, createdDateTime, yearMonthStart, monthCount,
                open, turnoverCents, vatInboundCents, vatOutboundCents, openTransactionId, closeTransactionid, underConstruction);
        }


        private static BasicVatReportItem ReadVatReportItemFromDataReader(IDataRecord reader)
        {
            int vatReportItemId = reader.GetInt32(0);
            int vatReportId = reader.GetInt32(1);
            int financialTransactionId = reader.GetInt32(2);
            int foreignId = reader.GetInt32(3);
            FinancialDependencyType dependencyType =
                (FinancialDependencyType) Enum.Parse(typeof (FinancialDependencyType), reader.GetString(4));

            Int64 turnoverCents = reader.GetInt64(5);
            Int64 vatInboundCents = reader.GetInt64(6);
            Int64 vatOutboundCents = reader.GetInt64(7);

            return new BasicVatReportItem(vatReportItemId, vatReportId, financialTransactionId, foreignId,
                dependencyType, turnoverCents, vatInboundCents, vatOutboundCents);
        }

        #endregion

        #region Record reading - SELECT statements

        public BasicVatReport GetVatReport (int vatReportId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + vatReportFieldSequence + "WHERE VatReportId=" + vatReportId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadVatReportFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown VAT Report Id: " + vatReportId);
                }
            }
        }


        public int GetVatReportIdFromCloseTransaction(int closeTransactionId)
        {
            return GetVatReportIdFromIntegerField("CloseTransactionId", closeTransactionId);
        }

        public int GetVatReportIdFromOpenTransaction(int openTransactionId)
        {
            return GetVatReportIdFromIntegerField("OpenTransactionId", openTransactionId);
        }

        private int GetVatReportIdFromIntegerField(string integerFieldName, int integerValue)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT VatReportId FROM VatReports WHERE " + integerFieldName + "=" + integerValue + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }

                    return 0;
                }
            }
        }
    


        public BasicVatReport[] GetVatReports(params object[] conditions)
        {
            List<BasicVatReport> result = new List<BasicVatReport>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + vatReportFieldSequence + ConstructWhereClause("VatReports", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadVatReportFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicVatReportItem GetVatReportItem(int vatReportItemId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + vatReportItemFieldSequence + "WHERE VatReportItemId=" + vatReportItemId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadVatReportItemFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown VAT Report Item Id: " + vatReportItemId);
                }
            }
        }




        public BasicVatReportItem[] GetVatReportItems(params object[] conditions)
        {
            List<BasicVatReportItem> result = new List<BasicVatReportItem>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + vatReportItemFieldSequence + ConstructWhereClause("VatReportItems", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadVatReportItemFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion


        #region Database optimizations


        public BasicFinancialAccountRow[] GetAccountRowsNotInVatReport(int accountId, DateTime endDate)
        {
            List<BasicFinancialAccountRow> result = new List<BasicFinancialAccountRow>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + accountId.ToString(CultureInfo.InvariantCulture) + ", FinancialTransactions.FinancialTransactionId, FinancialTransactions.DateTime, FinancialTransactions.Comment, AmountCents, CreatedDateTime, CreatedByPersonId " +
                        " FROM FinancialTransactionRows " +
                        " JOIN FinancialTransactions ON (FinancialTransactionRows.FinancialTransactionId = FinancialTransactions.FinancialTransactionId) " +
                        " WHERE NOT EXISTS (SELECT * FROM VatReportItems WHERE VatReportItems.FinancialTransactionId = FinancialTransactions.FinancialTransactionId) " +
                        " AND FinancialAccountId = @accountId " +
                        " AND FinancialTransactions.DateTime < @endDateTime" , connection);

                AddParameterWithName(command, "accountId", accountId);
                AddParameterWithName(command, "endDateTime", endDate);

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

        #endregion


        #region Creation and manipulation - stored procedures

        public int CreateVatReport (int organizationId, string guid, int yearMonthStart, int monthCount)
        {
            guid = guid.Replace("-", ""); // remove noise before committing
            DateTime nowUtc = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateVatReport", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "organizationId", organizationId);
                AddParameterWithName (command, "guid", guid);
                AddParameterWithName (command, "createdDateTime", nowUtc);
                AddParameterWithName (command, "yearMonthStart", yearMonthStart);
                AddParameterWithName (command, "monthCount", monthCount);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        public int CreateVatReportItem (int vatReportId, int financialTransactionId, int foreignObjectId, FinancialDependencyType financialDependencyType, Int64 turnoverCents, Int64 vatInboundCents, Int64 vatOutboundCents)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateVatReportItem", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "vatReportId", vatReportId);
                AddParameterWithName(command, "financialTransactionId", financialTransactionId);
                AddParameterWithName(command, "foreignId", foreignObjectId);
                AddParameterWithName(command, "financialDependencyType", financialDependencyType.ToString());
                AddParameterWithName(command, "turnoverCents", turnoverCents);
                AddParameterWithName(command, "vatInboundCents", vatInboundCents);
                AddParameterWithName(command, "vatOutboundCents", vatOutboundCents);

                return Convert.ToInt32(command.ExecuteScalar());

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }

        
        public void SetVatReportReleased (int vatReportId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetVatReportReleased", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "vatReportId", vatReportId);

                command.ExecuteNonQuery();
            }
        }


        public void SetVatReportOpen (int vatReportId, bool open)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetVatReportOpen", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "vatReportId", vatReportId);
                AddParameterWithName (command, "open", open);

                command.ExecuteNonQuery();

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }


        public void SetVatReportOpenTransaction(int vatReportId, int financialTransactionId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetVatReportOpenTransaction", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "vatReportId", vatReportId);
                AddParameterWithName(command, "openTransactionId", financialTransactionId);

                if (Convert.ToInt32(command.ExecuteScalar()) != 1) // returns count of rows updated
                {
                    throw new DatabaseConcurrencyException();
                }
            }
        }



        public void SetVatReportCloseTransaction(int vatReportId, int financialTransactionId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetVatReportCloseTransaction", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "vatReportId", vatReportId);
                AddParameterWithName(command, "closeTransactionId", financialTransactionId);

                if (Convert.ToInt32(command.ExecuteScalar()) != 1) // returns count of rows updated
                {

                    throw new DatabaseConcurrencyException();
                }
            }
        }



        #endregion
    }
}
 
 