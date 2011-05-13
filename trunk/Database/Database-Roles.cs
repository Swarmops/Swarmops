using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Activizr.Basic.Enums;
using Activizr.Basic.Interfaces;
using Activizr.Basic.Types;
using Activizr.Basic.Types.Security;

namespace Activizr.Database
{
    public partial class PirateDb
    {
        #region Field reading code

        private const string personRoleFieldSequence =
            " PeopleRoles.PersonRoleId, PersonRoleTypes.Name, PeopleRoles.PersonId, PeopleRoles.OrganizationId, PeopleRoles.GeographyId " + // 0-4
            " FROM PeopleRoles JOIN PersonRoleTypes ON (PeopleRoles.PersonRoleTypeId=PersonRoleTypes.PersonRoleTypeId) ";

        static private BasicPersonRole ReadPersonRoleFromDataReader (DbDataReader reader)
        {
            int personRoleId = reader.GetInt32(0);
            string personRoleTypeName = reader.GetString(1);
            int personId = reader.GetInt32(2);
            int organizationId = reader.GetInt32(3);
            int geographyId = reader.GetInt32(4);

            RoleType roleType = (RoleType) Enum.Parse(typeof (RoleType), personRoleTypeName);

            return new BasicPersonRole(personRoleId, personId, roleType, organizationId, geographyId);
        }

        #endregion


        public BasicAuthority GetPersonAuthority (IHasIdentity person)
        {
            List<BasicPersonRole> systemRoles = new List<BasicPersonRole>();
            List<BasicPersonRole> organizationRoles = new List<BasicPersonRole>();
            List<BasicPersonRole> localRoles = new List<BasicPersonRole>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + personRoleFieldSequence + ConstructWhereClause("PersonRoles", person),
                                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BasicPersonRole newPersonRole = ReadPersonRoleFromDataReader(reader);

                        switch (RoleTypes.ClassOfRole(newPersonRole.Type))
                        {
                            case RoleClass.System:
                                systemRoles.Add(newPersonRole);
                                break;

                            case RoleClass.Organization:
                                organizationRoles.Add(newPersonRole);
                                break;

                            case RoleClass.Local:
                                localRoles.Add(newPersonRole);
                                break;

                            default:
                                throw new InvalidOperationException("Invalid RoleTypeId (" + newPersonRole.Type +
                                                                    ") in database for PersonId " + person.Identity.ToString());
                        }
                    }

                    return new BasicAuthority(person.Identity, systemRoles.ToArray(), organizationRoles.ToArray(),
                                              localRoles.ToArray());
                }
            }
        }

        public BasicPersonRole GetRole (int roleId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + personRoleFieldSequence + " WHERE PersonRoleId=" + roleId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadPersonRoleFromDataReader(reader);
                    }
                    else
                    {
                        throw new ArgumentException("No such RoleId: " + roleId.ToString());
                    }
                }
            }
        }

        public BasicPersonRole[] GetPeopleWithRoleType (RoleType r, int[] orgId, int[] geoId)   // TODO: Refactor to use ConstructWhereClause
        {
            List<BasicPersonRole> retlist = new List<BasicPersonRole>();
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();
                string cmd = "SELECT " + personRoleFieldSequence + " WHERE Name= '" + r.ToString()+"'";
                if (orgId.Length > 0)
                    cmd += " AND OrganzationId IN (" + JoinIds(orgId) + ")";
                if (geoId.Length > 0)
                    cmd += " AND GeographyId IN (" + JoinIds(geoId) + ")";

                DbCommand command = GetDbCommand(cmd, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retlist.Add(ReadPersonRoleFromDataReader(reader));
                    }
                }
            }
            return retlist.ToArray();
        }


        public BasicPersonRole[] GetRoles()
        {
            return GetRolesForOrganizationsGeographies(new int[] {}, new int[] {});
        }


        public BasicPersonRole[] GetRolesForOrganization (int organizationId)
        {
            return GetRolesForOrganizationsGeographies(new int[] {organizationId}, new int[0]);
        }

        public BasicPersonRole[] GetRolesForOrganizationGeography (int organizationId, int geographyId)
        {
            return GetRolesForOrganizationGeographies(organizationId, new int[] { geographyId });
        }

        public BasicPersonRole[] GetRolesForOrganizationGeographies (int organizationId, int[] geographyIds)
        {
            return GetRolesForOrganizationsGeographies(new int[] { organizationId }, geographyIds);
        }

        public BasicPersonRole[] GetRolesForOrganizationsGeographies (int[] organizationIds, int[] geographyIds)  // TODO: Refactor to use ConstructWhereClause
        {
            List<BasicPersonRole> result = new List<BasicPersonRole>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                string selectString = "SELECT " + personRoleFieldSequence;
                if (organizationIds.Length > 0)
                    selectString += " WHERE OrganizationId IN (" + JoinIds(organizationIds) + ")";

                if (geographyIds.Length > 0)
                {
                    selectString += (organizationIds.Length > 0) ? " AND " : " WHERE ";

                    selectString += " GeographyId IN ( " + JoinIds(geographyIds) + ")";
                }

                selectString += " ORDER BY PersonId";

                DbCommand command =
                    GetDbCommand(selectString, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadPersonRoleFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }



        public int CreateRole (int personId, RoleType roleType, int organizationId, int nodeId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreatePersonRole", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "personRoleType", roleType.ToString());
                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "geographyId", nodeId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void DeleteRole (int roleId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("DeletePersonRole", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "personRoleId", roleId);
                command.ExecuteNonQuery();
            }
        }
    }
}