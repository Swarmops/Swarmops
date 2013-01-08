using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class PirateDb
    {
        #region Field reading code

        private const string internalPollFieldSequence =
            " InternalPollId,OrganizationId,GeographyId,Name,RunningOpen," + // 0-4
            " VotingOpen,MaxVoteLength,CreatedByPersonId,RunningOpens,RunningCloses," + // 5-9
            " VotingOpens,VotingCloses,ResultsTypeId " + // 10-12
            " FROM InternalPolls ";

        private BasicInternalPoll ReadInternalPollFromDataReader(DbDataReader reader)
        {
            int internalPollId = reader.GetInt32(0);
            int organizationId = reader.GetInt32(1);
            int geographyId = reader.GetInt32(2);
            string name = reader.GetString(3);
            bool runningOpen = reader.GetBoolean(4);
            bool votingOpen = reader.GetBoolean(5);
            int maxVoteLength = reader.GetInt32(6);
            int createdByPersonId = reader.GetInt32(7);
            DateTime runningOpens = reader.GetDateTime(8);
            DateTime runningCloses = reader.GetDateTime(9);
            DateTime votingOpens = reader.GetDateTime(10);
            DateTime votingCloses = reader.GetDateTime(11);
            int resultsTypeId = reader.GetInt32(12);

            InternalPollResultsType resultsType = (InternalPollResultsType) resultsTypeId;

            return new BasicInternalPoll(internalPollId, createdByPersonId, organizationId, geographyId, name, runningOpen, votingOpen, maxVoteLength, runningOpens, runningCloses, votingOpens, votingCloses, resultsType);
        }

        #endregion




        #region Record reading - SELECT statements

        public BasicInternalPoll GetInternalPoll (int internalPollId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + internalPollFieldSequence + "WHERE InternalPollId=" + internalPollId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadInternalPollFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown InternalPollId: " + internalPollId.ToString());
                }
            }
        }


        /// <summary>
        /// Gets a list of internal polls.
        /// </summary>
        /// <param name="conditions">Optional Organization object and/or DatabaseConditions.</param>
        /// <returns>The inbound invoice list.</returns>
        public BasicInternalPoll[] GetInternalPolls(params object[] conditions)
        {
            List<BasicInternalPoll> result = new List<BasicInternalPoll>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + internalPollFieldSequence + ConstructWhereClause("InternalPoll", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadInternalPollFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        #endregion




        #region Creation and manipulation - stored procedures

        public int CreateInternalPoll(int organizationId, int geographyId, string name, int maxVoteLength,
                                      InternalPollResultsType resultsType, int createdByPersonId, 
                                      DateTime runningOpens, DateTime runningCloses, DateTime votingOpens, 
                                      DateTime votingCloses)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateInternalPoll", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "name", name);
                AddParameterWithName(command, "maxVoteLength", maxVoteLength);
                AddParameterWithName(command, "resultsTypeId", (int) resultsType);
                AddParameterWithName(command, "createdByPersonId", createdByPersonId);
                AddParameterWithName(command, "runningOpens", runningOpens);
                AddParameterWithName(command, "runningCloses", runningCloses);
                AddParameterWithName(command, "votingOpens", votingOpens);
                AddParameterWithName(command, "votingCloses", votingCloses);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public void SetInternalPollVotingOpen(int internalPollId, bool open)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetInternalPollVotingOpen", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "internalPollId", internalPollId);
                AddParameterWithName(command, "open", open);

                command.ExecuteNonQuery();
            }
        }

        public void SetInternalPollRunningOpen(int internalPollId, bool open)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetInternalPollRunningOpen", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "internalPollId", internalPollId);
                AddParameterWithName(command, "open", open);

                command.ExecuteNonQuery();
            }
        }

        #endregion

    }
}