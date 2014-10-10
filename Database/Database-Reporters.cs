using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public int CreateReporter(string name, string email)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateReporter", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "name", name);
                AddParameterWithName(command, "email", email);

                int reporterId = Convert.ToInt32(command.ExecuteScalar());

                return reporterId;
            }
        }

        public void DeleteReporter(int reporterId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("DeleteReporter", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "reporterId", reporterId);

                command.ExecuteNonQuery();
            }
        }

        public void CreateReporterMediaCategory(int reporterId, string mediaCategory)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateReporterMediaCategory", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "reporterId", reporterId);
                AddParameterWithName(command, "mediaCategoryName", mediaCategory);

                command.ExecuteNonQuery();
            }
        }

        public BasicReporter GetReporter (int reporterId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + reporterFieldSequence + " WHERE ReporterId=" + reporterId.ToString(),
                                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadReporterFromReader(reader);
                    }

                    throw new ArgumentException("No such ReporterId: " + reporterId.ToString());
                }
            }
        }


        public BasicReporter[] GetReporters(params object[] conditions)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + reporterFieldSequence + ConstructWhereClause("Reporters", conditions), connection);
                List<BasicReporter> result = new List<BasicReporter>();

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadReporterFromReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicReporter[] GetReportersFromMediaCategories (int[] mediaCategoryIds)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();
                List<int> reporterIds = new List<int>();

                DbCommand command =
                    GetDbCommand(
                        "SELECT ReporterId From ReportersMediaCategories WHERE MediaCategoryId in (" +
                        JoinIds(mediaCategoryIds) + ")", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reporterIds.Add(reader.GetInt32(0));
                    }
                }
                   
                List<BasicReporter> result = new List<BasicReporter>();
                if (reporterIds.Count > 0)
                {
                    command =
                        GetDbCommand(
                            "SELECT " + reporterFieldSequence + " WHERE ReporterId in (" + JoinIds(reporterIds.ToArray()) + ")",
                            connection);

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(ReadReporterFromReader(reader));
                        }

                        return result.ToArray();
                    }
                }
                else
                    return result.ToArray();
            }
        }


        public int[] GetReporterMediaCategories (int reporterId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();
                List<int> categoryIds = new List<int>();

                DbCommand command =
                    GetDbCommand(
                        "SELECT MediaCategoryId From ReportersMediaCategories WHERE ReporterId=" + reporterId.ToString(),
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categoryIds.Add(reader.GetInt32(0));
                    }
                }

                return categoryIds.ToArray();
            }
        }


        private const string reporterFieldSequence =
            " ReporterId,Name,Email " +  // 0-2
            " FROM Reporters ";


        private static BasicReporter ReadReporterFromReader (DbDataReader reader)
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            string email = reader.GetString(2);

            return new BasicReporter(id, name, email, null);
        }
    }
}