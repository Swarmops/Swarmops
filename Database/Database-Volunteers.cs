using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public BasicVolunteer[] GetOpenVolunteers()
        {
            List<BasicVolunteer> result = new List<BasicVolunteer>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT VolunteerId,PersonId,OwnerPersonId,OpenedDateTime,Open,ClosedDateTime,ClosingComments FROM Volunteers WHERE Open=1;",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadVolunteerFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicVolunteer GetVolunteer (int volunteerId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT VolunteerId,PersonId,OwnerPersonId,OpenedDateTime,Open,ClosedDateTime,ClosingComments FROM Volunteers WHERE VolunteerId=" +
                        volunteerId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadVolunteerFromDataReader (reader);
                    }

                    throw new ArgumentException ("Unknown VolunteerId");
                }
            }
        }



        private BasicVolunteer ReadVolunteerFromDataReader (DbDataReader reader)
        {
            int volunteerId = reader.GetInt32 (0);
            int personId = reader.GetInt32 (1);
            int ownerPersonId = reader.GetInt32 (2);
            DateTime openedDateTime = reader.GetDateTime (3);
            bool open = reader.GetBoolean (4);
            DateTime closedDateTime = reader.GetDateTime (5);
            string closingComments = reader.GetString (6);

            return new BasicVolunteer (volunteerId, personId, ownerPersonId, openedDateTime, open, closedDateTime,
                closingComments);
        }


        private BasicVolunteerPosition ReadVolunteerPositionFromDataReader (DbDataReader reader)
        {
            throw new NotImplementedException();
        }


        public int CreateVolunteer (int personId, int ownerPersonId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateVolunteer", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "personId", personId);
                AddParameterWithName (command, "ownerPersonId", ownerPersonId);
                AddParameterWithName (command, "openedDateTime", DateTime.UtcNow);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public int CreateVolunteerPosition (int volunteerId, int positionId, int geographyId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateVolunteerPosition", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "volunteerId", volunteerId);
                AddParameterWithName (command, "positionId", positionId);
                AddParameterWithName (command, "geographyId", geographyId);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public void SetVolunteerOwnerPersonId (int volunteerId, int ownerPersonId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetVolunteerOwnerPersonId", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "volunteerId", volunteerId);
                AddParameterWithName (command, "ownerPersonId", ownerPersonId);

                command.ExecuteNonQuery();
            }
        }

        public void CloseVolunteer (int volunteerId, string closingComments)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CloseVolunteer", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "volunteerId", volunteerId);
                AddParameterWithName (command, "closedDateTime", DateTime.Now);
                AddParameterWithName (command, "closingComments", closingComments);

                command.ExecuteNonQuery();
            }
        }

        public void CloseVolunteerRole (int volunteerRoleId, bool wasAssigned)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CloseVolunteerRole", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "volunteerRoleId", volunteerRoleId);
                AddParameterWithName (command, "assigned", wasAssigned);

                command.ExecuteNonQuery();
            }
        }
    }
}