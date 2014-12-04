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

        private const string swarmopsLogFieldSequence =
            " SwarmopsLogEntryId,DateTime,PersonId,EntryTypeId,EntryXml " + // 0-4
            "FROM SwarmopsLog ";

        private static BasicSwarmopsLogEntry ReadSwarmopsLogEntryFromDataReader(IDataRecord reader)
        {
            int swarmopsLogEntryId = reader.GetInt32(0);
            DateTime dateTime = reader.GetDateTime(1);
            int personId = reader.GetInt32(2);
            int entryTypeId = reader.GetInt32(3);
            string entryXml = reader.GetString(4);

            return new BasicSwarmopsLogEntry(swarmopsLogEntryId, personId, dateTime, entryTypeId, entryXml);
        }

        #endregion

        #region Record reading - SELECT statements

        public BasicSwarmopsLogEntry GetSwarmopsLogEntry(int swarmopsLogEntryId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + swarmopsLogFieldSequence + "WHERE SwarmopsLogEntryId=" + swarmopsLogEntryId + ";",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadSwarmopsLogEntryFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown SwarmopsLogEntryId: " + swarmopsLogEntryId);
                }
            }
        }

        public BasicSwarmopsLogEntry[] GetSwarmopsLogEntries(DateTime startDate, DateTime endDate)
        {
            List<BasicSwarmopsLogEntry> result = new List<BasicSwarmopsLogEntry>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + swarmopsLogFieldSequence + " WHERE DateTime >= '" +
                        startDate.ToString("yyyy-MM-dd HH:mm:ss") + "' AND DateTime < '" +
                        endDate.ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY DateTime", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadSwarmopsLogEntryFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Creation and manipulation - stored procedures

        public int CreateSwarmopsLogEntry(int personId, string entryType, string entryXml)
        {
            DateTime now = DateTime.UtcNow;

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateSwarmopsLogEntry", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "entryType", entryType);
                AddParameterWithName(command, "entryXml", entryXml);
                AddParameterWithName(command, "dateTime", DateTime.UtcNow);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void CreateSwarmopsLogEntryAffectedObject(int swarmopsLogEntryId, string affectedObjectType,
            int affectedObjectId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateSwarmopsLogEntryAffectedObject", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "swarmopsLogEntryId", swarmopsLogEntryId);
                AddParameterWithName(command, "affectedObjectType", affectedObjectType);
                AddParameterWithName(command, "affectedObjectId", affectedObjectId);

                command.ExecuteNonQuery(); // no return value - table has no identity
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