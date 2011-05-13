using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;


namespace Activizr.Database
{
    public partial class PirateDb
    {
        public BasicVolunteer[] GetOpenVolunteers()
        {
            List<BasicVolunteer> result = new List<BasicVolunteer>();

            using (DbConnection connection = this.GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT VolunteerId,PersonId,OwnerPersonId,OpenedDateTime,Open,ClosedDateTime,ClosingComments FROM Volunteers WHERE Open=1;",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadVolunteerFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicVolunteer GetVolunteer (int volunteerId)
        {
            using (DbConnection connection = this.GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT VolunteerId,PersonId,OwnerPersonId,OpenedDateTime,Open,ClosedDateTime,ClosingComments FROM Volunteers WHERE VolunteerId=" +
                        volunteerId.ToString() + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadVolunteerFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown VolunteerId");
                }
            }
        }

        public BasicVolunteerRole GetVolunteerRole (int volunteerRoleId)
        {
            using (DbConnection connection = this.GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT VolunteerRoleId,VolunteerId,OrganizationId,GeographyId,RoleTypeId,Open,Assigned FROM VolunteerRoles WHERE VolunteerRoleId=" +
                        volunteerRoleId.ToString() + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadVolunteerRoleFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown VolunteerId");
                }
            }
        }


        public BasicVolunteerRole[] GetVolunteerRolesByVolunteer (int volunteerId)
        {
            List<BasicVolunteerRole> result = new List<BasicVolunteerRole>();

            using (DbConnection connection = this.GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT VolunteerRoleId,VolunteerId,OrganizationId,GeographyId,RoleTypeId,Open,Assigned FROM VolunteerRoles WHERE VolunteerId=" +
                        volunteerId.ToString() + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadVolunteerRoleFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }


        private BasicVolunteer ReadVolunteerFromDataReader (DbDataReader reader)
        {
            int volunteerId = reader.GetInt32(0);
            int personId = reader.GetInt32(1);
            int ownerPersonId = reader.GetInt32(2);
            DateTime openedDateTime = reader.GetDateTime(3);
            bool open = reader.GetBoolean(4);
            DateTime closedDateTime = reader.GetDateTime(5);
            string closingComments = reader.GetString(6);

            return new BasicVolunteer(volunteerId, personId, ownerPersonId, openedDateTime, open, closedDateTime,
                                      closingComments);
        }


        private BasicVolunteerRole ReadVolunteerRoleFromDataReader (DbDataReader reader)
        {
            int volunteerRoleId = reader.GetInt32(0);
            int volunteerId = reader.GetInt32(1);
            int organizationId = reader.GetInt32(2);
            int geographyId = reader.GetInt32(3);
            int roleTypeId = reader.GetInt32(4);
            bool open = reader.GetBoolean(5);
            bool assigned = reader.GetBoolean(6);

            return new BasicVolunteerRole(volunteerRoleId, volunteerId, organizationId, geographyId,
                                          (RoleType) roleTypeId, open, assigned);
        }


        public int CreateVolunteer (int personId, int ownerPersonId)
        {
            using (DbConnection connection = this.GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateVolunteer", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "ownerPersonId", ownerPersonId);
                AddParameterWithName(command, "openedDateTime", DateTime.Now);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int CreateVolunteerRole (int volunteerId, int organizationId, int geographyId, RoleType roleType)
        {
            using (DbConnection connection = this.GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateVolunteerRole", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "volunteerId", volunteerId);
                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "roleTypeId", (int) roleType);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void SetVolunteerOwnerPersonId (int volunteerId, int ownerPersonId)
        {
            using (DbConnection connection = this.GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetVolunteerOwnerPersonId", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "volunteerId", volunteerId);
                AddParameterWithName(command, "ownerPersonId", ownerPersonId);

                command.ExecuteNonQuery();
            }
        }

        public void CloseVolunteer (int volunteerId, string closingComments)
        {
            using (DbConnection connection = this.GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CloseVolunteer", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "volunteerId", volunteerId);
                AddParameterWithName(command, "closedDateTime", DateTime.Now);
                AddParameterWithName(command, "closingComments", closingComments);

                command.ExecuteNonQuery();
            }
        }

        public void CloseVolunteerRole (int volunteerRoleId, bool wasAssigned)
        {
            using (DbConnection connection = this.GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CloseVolunteerRole", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "volunteerRoleId", volunteerRoleId);
                AddParameterWithName(command, "assigned", wasAssigned);

                command.ExecuteNonQuery();
            }
        }
    }
}