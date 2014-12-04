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

        private const string internalPollCandidateFieldSequence =
            " InternalPollCandidateId,InternalPollId,PersonId,CandidacyStatement " + // 0-3
            "FROM InternalPollCandidates ";

        private static BasicInternalPollCandidate ReadInternalPollCandidateFromDataReader(IDataRecord reader)
        {
            int internalPollCandidateId = reader.GetInt32(0);
            int internalPollId = reader.GetInt32(1);
            int personId = reader.GetInt32(2);
            string candidacyStatement = reader.GetString(3);

            return new BasicInternalPollCandidate(internalPollCandidateId, internalPollId, personId, candidacyStatement);
        }

        #endregion

        #region Record reading - SELECT statements

        public BasicInternalPollCandidate GetInternalPollCandidate(int internalPollCandidateId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + internalPollCandidateFieldSequence + "WHERE InternalPollCandidateId=" +
                        internalPollCandidateId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadInternalPollCandidateFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Id: " + internalPollCandidateId);
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
        public BasicInternalPollCandidate[] GetInternalPollCandidates(params object[] conditions)
        {
            List<BasicInternalPollCandidate> result = new List<BasicInternalPollCandidate>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + internalPollCandidateFieldSequence +
                        ConstructWhereClause("InternalPollCandidates", conditions) + " ORDER BY SortOrder", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadInternalPollCandidateFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Creation and manipulation - stored procedures

        public int CreateInternalPollCandidate(int internalPollId, int personId, string candidacyStatement)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateInternalPollCandidate", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "internalPollId", internalPollId);
                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "candidacyStatement", candidacyStatement);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void SetInternalPollCandidateStatement(int internalPollCandidateId, string candidacyStatement)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetInternalPollCandidateStatement", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "internalPollCandidateId", internalPollCandidateId);
                AddParameterWithName(command, "candidacyStatement", candidacyStatement);

                command.ExecuteNonQuery();
            }
        }

        public int SetInternalPollCandidateSortOrder(int internalPollCandidateId, string sortOrder)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetInternalPollCandidateSortOrder", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "internalPollCandidateId", internalPollCandidateId);
                AddParameterWithName(command, "sortOrder", sortOrder);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        #endregion
    }
}