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

        private const string internalPollVoteFieldSequence =
            " InternalPollVoteId,InternalPollId,VerificationCode " +             // 0-3
            "FROM InternalPollVotes ";

        private static BasicInternalPollVote ReadInternalPollVoteFromDataReader(IDataRecord reader)
        {
            int internalPollVoteId = reader.GetInt32(0);
            int internalPollId = reader.GetInt32(1);
            string verificationCode = reader.GetString(2);

            return new BasicInternalPollVote(internalPollVoteId, internalPollId, verificationCode);
        }

        private const string internalPollVoterFieldSequence =
            " PersonId,InternalPollId,Open,ClosedDateTime " +             // 0-3
            "FROM InternalPollVoters ";

        private static BasicInternalPollVoter ReadInternalPollVoterFromDataReader(IDataRecord reader)
        {
            int personId = reader.GetInt32(0);
            int internalPollId = reader.GetInt32(1);
            bool open = reader.GetBoolean(2);
            DateTime closedDateTime = reader.GetDateTime(3);

            return new BasicInternalPollVoter(personId, internalPollId, open, closedDateTime);
        }

        #endregion



        #region Record reading - SELECT statements

        public BasicInternalPollVote GetInternalPollVote(int internalPollVoteId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + internalPollVoteFieldSequence + "WHERE InternalPollVoteId=" + internalPollVoteId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadInternalPollVoteFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Id: " + internalPollVoteId.ToString());
                }
            }
        }


        public BasicInternalPollVote GetInternalPollVote (string verificationCode)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + internalPollVoteFieldSequence + "WHERE VerificationCode='" + verificationCode.Replace("'", "''") + "';", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadInternalPollVoteFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Verification Code: " + verificationCode.ToString());
                }
            }
        }


        public BasicInternalPollVote[] GetInternalPollVotes(params object[] conditions)
        {
            List<BasicInternalPollVote> result = new List<BasicInternalPollVote>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + internalPollVoteFieldSequence + ConstructWhereClause("InternalPollVotes", conditions) + " ORDER BY VerificationCode", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadInternalPollVoteFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public InternalPollVoterStatus GetInternalPollVoterStatus (int internalPollId, int personId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT Open FROM InternalPollVoters WHERE InternalPollId=" + internalPollId.ToString() + " AND PersonId=" + personId.ToString(), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetBoolean(0)? InternalPollVoterStatus.CanVote : InternalPollVoterStatus.HasAlreadyVoted;
                    }

                    return InternalPollVoterStatus.NotEligibleForPoll;
                }
            }
        }


        public int[] GetInternalPollVoteDetails (int internalPollVoteId)
        {
            List<int> result = new List<int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT InternalPollCandidateId FROM InternalPollVoteDetails WHERE InternalPollVoteId=" + 
                        internalPollVoteId.ToString() + " ORDER BY Position", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetInt32(0));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicInternalPollVoter[] GetInternalPollVoters (params object[] conditions)
        {
            List<BasicInternalPollVoter> result = new List<BasicInternalPollVoter>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand(
                    "SELECT" + internalPollVoterFieldSequence + ConstructWhereClause("InternalPollVoters", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadInternalPollVoterFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
            
        }


        public Dictionary<int,int> GetCandidateIdPersonIdMap (int pollId)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand(
                    "SELECT InternalPollCandidateId,PersonId FROM InternalPollCandidates WHERE InternalPollId=" +
                    pollId.ToString(), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result[reader.GetInt32(0)] = reader.GetInt32(1);
                    }

                    return result;
                }
            }
        }


        #endregion



        #region Creation and manipulation - stored procedures

        public int CreateInternalPollVote(int internalPollId, int voteGeographyId, string verificationCode)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateInternalPollVote", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "internalPollId", internalPollId);
                AddParameterWithName(command, "voteGeographyId", voteGeographyId);
                AddParameterWithName(command, "verificationCode", verificationCode);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public int CreateInternalPollVoteDetail(int internalPollVoteId, int internalPollCandidateId, int position)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateInternalPollVoteDetail", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "internalPollVoteId", internalPollVoteId);
                AddParameterWithName(command, "internalPollCandidateId", internalPollCandidateId);
                AddParameterWithName(command, "position", position);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }



        public void ClearInternalPollVote(int internalPollVoteId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("ClearInternalPollVote", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "internalPollVoteId", internalPollVoteId);

                command.ExecuteNonQuery();
            }
        }




        public int CreateInternalPollVoter(int internalPollId, int personId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateInternalPollVoter", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "internalPollId", internalPollId);
                AddParameterWithName(command, "personId", personId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public int CloseInternalPollVoter(int internalPollId, int personId, string ipAddress)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CloseInternalPollVoter", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "internalPollId", internalPollId);
                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "closedDateTime", DateTime.Now);
                AddParameterWithName(command, "ipAddress", ipAddress);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }



        #endregion

    }
}