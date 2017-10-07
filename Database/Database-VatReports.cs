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

        private const string vatReportFieldSequence =
            " VatReportId,OrganizationId,Guid,CreatedDateTime,YearMonthStart," + // 0-4
            "MonthCount,VatReports.Open,TurnoverCents,VatInboundCents,VatOutboundCents," + // 5-9
            "UnderConstruction " + // 10
            "FROM VatReports ";

        private const string vatReportItemFieldSequence =
            " VatReportItemId,VatReportId,FinancialTransactionId,ForeignId,FinancialDependencyTypes.Name AS FinancialDependencyType," +  // 0-4
            "TurnoverCents,VatInboundCents,VatOutboundCents " +
            "FROM VatReportItems " +
            "JOIN FinancialDependencyTypes ON (VatReportItemId.FinancialDependencyTypeId=FinancialDependencyTypes.FinancialDependencyTypeId) ";

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
            bool underConstruction = reader.GetBoolean (10);

            return new BasicVatReport(vatReportId, organizationId, guid, createdDateTime, yearMonthStart, monthCount,
                open, turnoverCents, vatInboundCents, vatOutboundCents, underConstruction);
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


        public BasicSalary[] GetVatReports (params object[] conditions)
        {
            List<BasicSalary> result = new List<BasicSalary>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + salaryFieldSequence + "JOIN Payroll USING (PayrollItemId)" +
                        ConstructWhereClause ("Salaries", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadSalaryFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Creation and manipulation - stored procedures

        public int CreateVatReport (int payrollItemId, DateTime payoutDate, Int64 baseSalaryCents, Int64 netSalaryCents,
            Int64 subtractiveTaxCents, Int64 additiveTaxCents)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateSalaryPrecise", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "payrollItemId", payrollItemId);
                AddParameterWithName (command, "payoutDate", payoutDate);
                AddParameterWithName (command, "baseSalaryCents", baseSalaryCents);
                AddParameterWithName (command, "netSalaryCents", netSalaryCents);
                AddParameterWithName (command, "subtractiveTaxCents", subtractiveTaxCents);
                AddParameterWithName (command, "additiveTaxCents", additiveTaxCents);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        public void CreateVatReportItem (int vatReportId, int financialTransactionId, int foreignObjectId, string financialDependencyType, Int64 turnoverCents, Int64 vatInboundCents, Int64 vatOutboundCents)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateVatReportItem", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "vatReportId", vatReportId);
                AddParameterWithName(command, "financialTransactionId", financialTransactionId);
                AddParameterWithName(command, "foreignObjectId", foreignObjectId);
                AddParameterWithName(command, "financialDependencyType", financialDependencyType);
                AddParameterWithName(command, "turnoverCents", turnoverCents);
                AddParameterWithName(command, "vatInboundCents", vatInboundCents);
                AddParameterWithName(command, "vatOutboundCents", vatOutboundCents);

                command.ExecuteNonQuery();

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


        #endregion
    }
}
 