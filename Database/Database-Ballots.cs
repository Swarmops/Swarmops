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

        private const string ballotFieldSequence =
            " BallotId,ElectionId,Name,OrganizationId,GeographyId," +   // 0-4
            "BallotCount,DeliveryAddress " +             // 5-6
            "FROM Ballots ";

        private static BasicBallot ReadBallotFromDataReader(IDataRecord reader)
        {
            int ballotId = reader.GetInt32(0);
            int electionId = reader.GetInt32(1);
            string name = reader.GetString(2);
            int organizationId = reader.GetInt32(3);
            int geographyId = reader.GetInt32(4);
            int ballotCount = reader.GetInt32(5);
            string deliveryAddress = reader.GetString(6);

            return new BasicBallot(ballotId, electionId, organizationId, geographyId, name, ballotCount, deliveryAddress);
        }

        #endregion



        #region Record reading - SELECT statements

        public BasicBallot GetBallot(int ballotId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + ballotFieldSequence + "WHERE BallotId=" + ballotId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadBallotFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Ballot Id: " + ballotId.ToString());
                }
            }
        }

        public BasicBallot[] GetBallots(params object[] conditions)
        {
            List<BasicBallot> result = new List<BasicBallot>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + ballotFieldSequence + ConstructWhereClause("Ballots", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadBallotFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public int[] GetBallotCandidates(int ballotId)
        {
            List<int> result = new List<int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT PersonId FROM BallotCandidates WHERE BallotId=" + ballotId + " ORDER BY SortOrder", connection);

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


        public int[] GetDocumentedCandidates(int electionId, int organizationId)
        {
            List<int> result = new List<int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT PersonId FROM CandidateDocumentation WHERE ElectionId=" + electionId + " AND OrganizationId=" + organizationId, connection);

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

        public Dictionary<int,int> GetBallotsForPerson (int personId)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT BallotId,SortOrder FROM BallotCandidates WHERE PersonId=" + personId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result [reader.GetInt32(0)] = reader.GetInt32(1);
                    }

                    return result;
                }
            }
        }


        #endregion



        #region Creation and manipulation - stored procedures

        public int CreateBallot (int electionId, string name, int organizationId, int geographyId, int ballotCount, string deliveryAddress)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateBallot", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "electionId", electionId);
                AddParameterWithName(command, "name", name);
                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "ballotCount", ballotCount);
                AddParameterWithName(command, "deliveryAddress", deliveryAddress);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void SetBallotCount(int ballotId, int ballotCount)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetBallotCount", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "ballotId", ballotId);
                AddParameterWithName(command, "ballotCount", ballotCount);

                command.ExecuteNonQuery();
            }
        }


        public void SetBallotDeliveryAddress(int ballotId, string deliveryAddress)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetBallotDeliveryAddress", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "ballotId", ballotId);
                AddParameterWithName(command, "deliveryAddress", deliveryAddress);

                command.ExecuteNonQuery();
            }
        }



        public void CreateBallotCandidate(int ballotId, int personId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateBallotCandidate", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "ballotId", ballotId);
                AddParameterWithName(command, "personId", personId);

                command.ExecuteNonQuery();
            }
        }


        public void ClearBallotCandidates(int ballotId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("ClearBallotCandidates", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "ballotId", ballotId);
                command.ExecuteNonQuery();
            }
        }


        public void SetCandidateDocumentationReceived(int electionId, int organizationId, int personId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetCandidateDocumentationReceived", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "electionId", electionId);
                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "dateTimeReceived", DateTime.Now);

                command.ExecuteNonQuery();
            }
        }

        #endregion

    }
}