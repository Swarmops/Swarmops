using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;


namespace Swarmops.Database
{
    public partial class PirateDb
    {
        #region Field reading code

        private const string communicationTurnaroundFieldSequence =
            " OrganizationId,CommunicationTypeId,CommunicationId,DateTimeOpened,DateTimeFirstResponse," +  // 0-4
            "PersonIdFirstResponse,DateTimeClosed,PersonIdClosed,Open,Responded " +                 // 5-9
            "FROM CommunicationTurnarounds ";

        private static BasicCommunicationTurnaround ReadCommunicationTurnaroundFromDataReader(IDataRecord reader)
        {
            int organizationId = reader.GetInt32(0);
            int communicationTypeId = reader.GetInt32(1);
            int communicationId = reader.GetInt32(2);
            DateTime dateTimeOpened = reader.GetDateTime(3);
            DateTime dateTimeFirstResponse = reader.GetDateTime(4);
            int personIdFirstResponse = reader.GetInt32(5);
            DateTime dateTimeClosed = reader.GetDateTime(6);
            int personIdClosed = reader.GetInt32(7);
            bool open = reader.GetBoolean(8);
            bool awaitingResponse = reader.GetBoolean(9);

            return new BasicCommunicationTurnaround(organizationId, communicationTypeId, communicationId, dateTimeOpened, dateTimeFirstResponse, personIdFirstResponse, dateTimeClosed, personIdClosed, open, awaitingResponse);
        }

        #endregion



        #region Record reading - SELECT statements


        public BasicCommunicationTurnaround GetCommunicationTurnaround (int organizationId, int communicationTypeId, int communicationId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + communicationTurnaroundFieldSequence + "WHERE OrganizationId=" + organizationId.ToString() + " AND CommunicationTypeId=" + communicationTypeId.ToString() + " AND CommunicationId=" + communicationId.ToString() + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadCommunicationTurnaroundFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Communication Id: " + communicationId.ToString());
                }
            }
        }


        public BasicCommunicationTurnaround[] GetCommunicationTurnarounds(params object[] conditions)
        {
            List<BasicCommunicationTurnaround> result = new List<BasicCommunicationTurnaround>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + communicationTurnaroundFieldSequence + ConstructWhereClause("CommunicationTurnarounds", conditions) + " ORDER BY DateTimeOpened", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadCommunicationTurnaroundFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        #endregion



        #region Creation and manipulation - stored procedures

        public void CreateCommunicationTurnaround (int organizationId, int communicationTypeId, int communicationId, DateTime dateTimeOpened)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateCommunicationTurnaround", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "communicationTypeId", communicationTypeId);
                AddParameterWithName(command, "communicationId", communicationId);
                AddParameterWithName(command, "dateTimeOpened", dateTimeOpened);

                command.ExecuteNonQuery();
            }
        }


        public void SetCommunicationTurnaroundResponded(int organizationId, int communicationTypeId, int communicationId, DateTime dateTimeResponded, int personIdResponded)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetCommunicationTurnaroundResponded", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "communicationTypeId", communicationTypeId);
                AddParameterWithName(command, "communicationId", communicationId);
                AddParameterWithName(command, "dateTimeResponded", dateTimeResponded);
                AddParameterWithName(command, "personIdResponded", personIdResponded);

                command.ExecuteNonQuery();

            }
        }


        public void SetCommunicationTurnaroundClosed(int organizationId, int communicationTypeId, int communicationId, DateTime dateTimeClosed, int personIdClosed)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetCommunicationTurnaroundClosed", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "communicationTypeId", communicationTypeId);
                AddParameterWithName(command, "communicationId", communicationId);
                AddParameterWithName(command, "dateTimeClosed", dateTimeClosed);
                AddParameterWithName(command, "personIdClosed", personIdClosed);

                command.ExecuteNonQuery();

            }
        }




        #endregion

    }
}