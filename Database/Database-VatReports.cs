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
            " SalaryId,PayrollItemId,PayoutDate,Salaries.BaseSalaryCents,NetSalaryCents," + // 0-4
            "SubtractiveTaxCents,AdditiveTaxCents,Attested,NetPaid,TaxPaid," + // 5-9
            "Salaries.Open " + // 10
            "FROM Salaries ";

        private const string vatReportItemFieldSequence =
            "" +
            "FROM VatReportItems ";

        private static BasicSalary ReadVatReportFromDataReader (IDataRecord reader)
        {
            int salaryId = reader.GetInt32 (0);
            int payrollItemId = reader.GetInt32 (1);
            DateTime payoutDate = reader.GetDateTime (2);
            Int64 baseSalaryCents = reader.GetInt64 (3);
            Int64 netSalaryCents = reader.GetInt64 (4);
            Int64 subtractiveTaxCents = reader.GetInt64 (5);
            Int64 additiveTaxCents = reader.GetInt64 (6);
            bool attested = reader.GetBoolean (7);
            bool netPaid = reader.GetBoolean (8);
            bool taxPaid = reader.GetBoolean (9);
            bool open = reader.GetBoolean (10);

            return new BasicSalary (salaryId, payrollItemId, payoutDate, baseSalaryCents, netSalaryCents,
                subtractiveTaxCents, additiveTaxCents,
                attested, netPaid, taxPaid, open);
        }

        #endregion

        #region Record reading - SELECT statements

        /// <summary>
        ///     Gets a salary from the database.
        /// </summary>
        /// <param name="salaryId">The salary database identity.</param>
        /// <returns>The requested salary.</returns>
        /// <exception cref="ArgumentException">Thrown if there is no such identity.</exception>
        public BasicSalary GetVatReport (int salaryId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + salaryFieldSequence + "WHERE SalaryId=" + salaryId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadSalaryFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown Salary Id: " + salaryId);
                }
            }
        }

        /// <summary>
        ///     Gets salaries from the database.
        /// </summary>
        /// <param name="conditions">
        ///     An optional combination of a Person and/or Organization object and/or DatabaseCondition
        ///     specifiers.
        /// </param>
        /// <returns>A list of matching salaries.</returns>
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


        public void CreateVatReportItem (int salaryId, bool netPaid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetSalaryNetPaid", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "salaryId", salaryId);
                AddParameterWithName (command, "netPaid", netPaid);

                command.ExecuteNonQuery();

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }


        public void SetVatReportReleased (int salaryId, Int64 netSalaryCents)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetSalaryNetSalaryPrecise", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "salaryId", salaryId);
                AddParameterWithName (command, "netSalaryCents", netSalaryCents);

                command.ExecuteNonQuery();
            }
        }


        public void SetVatReportOpen (int salaryId, bool taxPaid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetSalaryTaxPaid", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "salaryId", salaryId);
                AddParameterWithName (command, "taxPaid", taxPaid);

                command.ExecuteNonQuery();

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }


        #endregion
    }
}