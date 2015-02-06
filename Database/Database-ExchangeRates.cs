using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string exchangeRateSnapshotFieldSequence =
            " CurrencyId,Code,Name,Sign " + // 0-3
            "FROM Currencies ";

        private const string exchangeRateDatapointSequence =
            " ...";

        /*
        private static BasicCurrency ReadCurrencyFromDataReader(IDataRecord reader)
        {
            int currencyId = reader.GetInt32(0);
            string code = reader.GetString(1);
            string name = reader.GetString(2);
            string sign = reader.GetString(3);

            return new BasicCurrency(currencyId, code, name, sign);
        }*/

        #endregion

        #region Record reading - SELECT statements
        
        public double GetCurrencyExchangeRate (int fromCurrencyId, int toCurrencyId, DateTime valuationDateTime)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(String.Format (
                        "SELECT CurrencyExchangeRateSnapshotData.CurrencyAId AS CurrencyAId, " +
                        "  CurrencyExchangeRateSnapshotData.CurrencyBId AS CurrencyBId, " +
                        "  CurrencyExchangeRateSnapshotData.APerB AS Rate, " +
                        "  CurrencyExchangeRateSnapshots.DateTime AS RateTime " +
                        "  FROM CurrencyExchangeRateSnapshotData " +
                        "  JOIN CurrencyExchangeRateSnapshots USING (CurrencyExchangeRateSnapshotId)" +
                        "  WHERE CurrencyAId = {0} AND CurrencyBId = {1} AND CurrencyExchangeRateSnapshots.DateTime < @valuationDateTime " +
                        "  ORDER BY CurrencyExchangeRateSnapshots.DateTime DESC " +
                        "  LIMIT 1;", fromCurrencyId, toCurrencyId), connection);

                AddParameterWithName (command, "valuationDateTime", valuationDateTime);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        DateTime testDate = reader.GetDateTime (3);

                        return reader.GetDouble(2);
                    }

                    throw new ArgumentException("Unknown Exchange Rate");
                }
            }
        }
        
        /*

        public BasicCurrency GetCurrency(string currencyCode)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + currencyFieldSequence + "WHERE Code='" +
                        currencyCode.Replace("'", "''").ToUpperInvariant() + "';", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadCurrencyFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Currency Code: " + currencyCode);
                }
            }
        }


        public BasicCurrency[] GetCurrencies(params object[] conditions)
        {
            List<BasicCurrency> result = new List<BasicCurrency>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + currencyFieldSequence + ConstructWhereClause("Currencies", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadCurrencyFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }*/

        #endregion

        #region Creation and manipulation - stored procedures

        public int CreateExchangeRateSnapshot()
        {
            DateTime now = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateExchangeRateSnapshot", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "dateTimeUtc", now);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public int CreateExchangeRateDatapoint (int exchangeRateSnapshotId, int currencyAId, int currencyBId, double aPerB)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateExchangeRateDatapoint", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "exchangeRateSnapshotId", exchangeRateSnapshotId);
                AddParameterWithName (command, "currencyAId", currencyAId);
                AddParameterWithName (command, "currencyBId", currencyBId);
                AddParameterWithName (command, "aPerB", aPerB);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /*

        public void SetSalaryNetPaid(int salaryId, bool netPaid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetSalaryNetPaid", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "salaryId", salaryId);
                AddParameterWithName(command, "netPaid", netPaid);

                command.ExecuteNonQuery();

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }


        public void SetSalaryNetSalary(int salaryId, double netSalary)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetSalaryNetSalary", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "salaryId", salaryId);
                AddParameterWithName(command, "netSalary", netSalary);

                command.ExecuteNonQuery();

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }


        public void SetSalaryTaxPaid(int salaryId, bool taxPaid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetSalaryTaxPaid", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "salaryId", salaryId);
                AddParameterWithName(command, "taxPaid", taxPaid);

                command.ExecuteNonQuery();

                // "Open" is set in the stored procedure to "NOT (TaxPaid AND NetPaid)".
                // So if both are paid, Open is set to false.
            }
        }


        public void SetSalaryAttested(int salaryId, bool attested)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetSalaryAttested", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "salaryId", salaryId);
                AddParameterWithName(command, "attested", attested);

                command.ExecuteNonQuery();
            }
        }
        */

        #endregion
    }
}