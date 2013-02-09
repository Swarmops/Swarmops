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

        private const string organizationFieldSequence =
            " OrganizationId, ParentOrganizationId, Name, NameInternational, NameShort, " +  // 0-4
            "Domain, MailPrefix, AnchorGeographyId, AcceptsMembers, AutoAssignNewMembers, " + // 5-9
            "DefaultCountryId " + // 10
            "FROM Organizations ";

        private static BasicOrganization ReadOrganizationFromDataReader (DbDataReader reader)
        {
            int organizationId = reader.GetInt32(0);
            int parentOrganizationId = reader.GetInt32(1);
            string name = reader.GetString(2);
            string nameInternational = reader.GetString(3);
            string nameShort = reader.GetString(4);
            string domain = reader.GetString(5);
            string mailPrefix = reader.GetString(6);
            int anchorGeographyId = reader.GetInt32(7);
            bool acceptsMembers = reader.GetBoolean(8);
            bool autoAssignNewMembers = reader.GetBoolean(9);
            int defaultCountryId = reader.GetInt32(10);

            return new BasicOrganization(organizationId, parentOrganizationId, name, nameInternational, nameShort,
                                         domain, mailPrefix, anchorGeographyId, acceptsMembers, autoAssignNewMembers,
                                         defaultCountryId);
        }


        #endregion


        public int CreateOrganization (int ParentOrganizationId, string NameInternational, string Name, string NameShort, string Domain, string MailPrefix, int AnchorGeographyId, bool AcceptsMembers, bool AutoAssignNewMembers, int DefaultCountryId)
        {

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateOrganization", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "pParentOrganizationId", ParentOrganizationId);
                AddParameterWithName(command, "pNameInternational", NameInternational);
                AddParameterWithName(command, "pName", Name);
                AddParameterWithName(command, "pNameShort", NameShort);
                AddParameterWithName(command, "pDomain", Domain);
                AddParameterWithName(command, "pMailPrefix", MailPrefix);
                AddParameterWithName(command, "pAnchorGeographyId", AnchorGeographyId);
                AddParameterWithName(command, "pAcceptsMembers", AcceptsMembers);
                AddParameterWithName(command, "pAutoAssignNewMembers", AutoAssignNewMembers);
                AddParameterWithName(command, "pDefaultCountryId", DefaultCountryId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void DeleteOrganization (int OrganizationId)
        {
            int deletedOrganizationsID = 98;
            BasicOrganization org = GetOrganization(OrganizationId);
            //Delete is done by setting parent ID to deletedOrganizationsID
            UpdateOrganization(
                deletedOrganizationsID,
                org.NameInternational,
                org.Name,
                org.NameShort,
                org.Domain,
                org.MailPrefix,
                org.AnchorGeographyId,
                org.AcceptsMembers,
                org.AutoAssignNewMembers,
                org.DefaultCountryId,
                OrganizationId);

        }

        public void UpdateOrganization (int ParentOrganizationId, string NameInternational, string Name, string NameShort, string Domain, string MailPrefix, int AnchorGeographyId, bool AcceptsMembers, bool AutoAssignNewMembers, int DefaultCountryId, int OrganizationId)
        {

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("UpdateOrganization", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "p_ParentOrganizationId", ParentOrganizationId);
                AddParameterWithName(command, "p_NameIntl", NameInternational == null ? "" : NameInternational);
                AddParameterWithName(command, "p_Name", Name == null ? "" : Name);
                AddParameterWithName(command, "p_NameShort", NameShort == null ? "" : NameShort);
                AddParameterWithName(command, "p_Domain", Domain == null ? "" : Domain);
                AddParameterWithName(command, "p_MailPrefix", MailPrefix == null ? "" : MailPrefix);
                AddParameterWithName(command, "p_AnchorGeographyId", AnchorGeographyId);
                AddParameterWithName(command, "p_AcceptsMembers", AcceptsMembers);
                AddParameterWithName(command, "p_AutoAssignNewMembers", AutoAssignNewMembers);
                AddParameterWithName(command, "p_DefaultCountryId", DefaultCountryId);
                AddParameterWithName(command, "p_OrganizationId", OrganizationId);
                command.ExecuteNonQuery();
            }
        }


        public BasicOrganization GetOrganization (int organizationId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT " + organizationFieldSequence + " WHERE Active=1 AND OrganizationId=" + organizationId.ToString(),
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadOrganizationFromDataReader(reader);
                    }

                    throw new ArgumentException("No such OrganizationId: " + organizationId.ToString());
                }
            }
        }


        public int[] GetOrganizationsIdsInGeographies (BasicGeography[] nodes)
        {
            if (nodes == null || nodes.Length == 0)
            {
                return new int[0];
            }

            List<int> nodeIdList = new List<int>();

            foreach (BasicGeography node in nodes)
            {
                nodeIdList.Add(node.GeographyId);
            }

            return GetOrganizationIdsInGeographies(nodeIdList.ToArray());
        }


        public int[] GetOrganizationIdsInGeographies (int[] nodeIds)
        {
            if (nodeIds == null || nodeIds.Length == 0)
            {
                return new int[0];
            }

            Dictionary<int, bool> resultKeys = new Dictionary<int, bool>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT OrganizationId FROM Organizations Where Active=1 AND AnchorGeographyId in (" + JoinIds(nodeIds) +
                        ")", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultKeys[reader.GetInt32(0)] = true;
                    }
                }

                command =
                    GetDbCommand(
                        "SELECT OrganizationId FROM OrganizationUptakeGeographies where GeographyId in (" +
                        JoinIds(nodeIds) + ")", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultKeys[reader.GetInt32(0)] = true;
                    }
                }
            }

            int[] resultArray = new int[resultKeys.Keys.Count];
            resultKeys.Keys.CopyTo(resultArray, 0);
            return resultArray;

        }


        public Dictionary<int, List<int>> GetAllOrganizationUptakeGeographyIds ()
        {
            Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT OrganizationId,GeographyId FROM OrganizationUptakeGeographies",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int uptakeOrgId = reader.GetInt32(0);
                        int uptakeGeoId = reader.GetInt32(1);
                        if (!result.ContainsKey(uptakeOrgId))
                        {
                            result[uptakeOrgId] = new List<int>();
                        }
                        result[uptakeOrgId].Add(uptakeGeoId);
                    }
                }
            }

            return result;
        }
        public int[] GetOrganizationUptakeGeographyIds (int organizationId)
        {
            List<int> result = new List<int>();

            BasicOrganization basicOrg = this.GetOrganization(organizationId);
            result.Add(basicOrg.AnchorGeographyId);

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT GeographyId FROM OrganizationUptakeGeographies WHERE OrganizationId=" + organizationId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int uptakeGeoId = reader.GetInt32(0);

                        if (uptakeGeoId != basicOrg.AnchorGeographyId)
                        {
                            result.Add(uptakeGeoId);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        public BasicUptakeGeography[] GetOrganizationUptakeGeographies (int organizationId, bool others)
        {
            List<BasicUptakeGeography> result = new List<BasicUptakeGeography>();
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                    "SELECT OrganizationId, GeographyId FROM OrganizationUptakeGeographies WHERE OrganizationId" + (others ? "<>" : "=") + organizationId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new BasicUptakeGeography(reader.GetInt32(0), reader.GetInt32(1)));
                    }
                }
            }

            return result.ToArray();
        }

        public void AddOrgUptakeGeography (int OrganizationId, int GeographyId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateUptakeGeography", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "p_OrganizationId", OrganizationId);
                AddParameterWithName(command, "p_GeographyId", GeographyId);
                command.ExecuteNonQuery();
            }

        }

        public void DeleteOrgUptakeGeography (int OrganizationId, int GeographyId)
        {

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("DeleteUptakeGeography", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "p_OrganizationId", OrganizationId);
                AddParameterWithName(command, "p_GeographyId", GeographyId);
                command.ExecuteNonQuery();
            }

        }

        public BasicOrganization[] GetAllOrganizations ()
        {
            List<BasicOrganization> result = new List<BasicOrganization>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT " + organizationFieldSequence + " WHERE Active=1 AND ParentOrganizationId >= 0", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadOrganizationFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }


        public BasicOrganization[] GetOrganizations (int[] organizationIds)
        {
            if (organizationIds == null || organizationIds.Length == 0)
            {
                return new BasicOrganization[0];
            }

            List<BasicOrganization> result = new List<BasicOrganization>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + organizationFieldSequence + " WHERE Active=1 AND OrganizationId IN (" + JoinIds(organizationIds) +
                        ")", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadOrganizationFromDataReader(reader));
                    }
                }
            }

            return result.ToArray();
        }


        public Dictionary<int, List<BasicOrganization>> GetHashedOrganizations ()
        {
            // This generates a Dictionary <int,List<Organization>>.
            // 
            // Keys are integers corresponding to OrganizationIds. At each key n,
            // the value is an List<Organization> starting with the Organization n followed by
            // its children.
            //
            // (Later reflection:) O(n) complexity, instead of recursion. Nice!

            Dictionary<int, List<BasicOrganization>> result = new Dictionary<int, List<BasicOrganization>>();

            BasicOrganization[] organizations = GetAllOrganizations();

            // Add the organizations.

            foreach (BasicOrganization organization in organizations)
            {
                if (organization.ParentOrganizationId >= 0)
                {
                    List<BasicOrganization> newList = new List<BasicOrganization>();
                    newList.Add(organization);

                    result[organization.OrganizationId] = newList;
                }
            }

            // Add the children.

            foreach (BasicOrganization organization in organizations)
            {
                if (organization.ParentOrganizationId > 0)
                {
                    result[organization.ParentOrganizationId].Add(organization);
                }
            }

            return result;
        }


        public BasicOrganization[] GetOrganizationTree (int startOrganizationId)
        {
            Dictionary<int, List<BasicOrganization>> organizations = GetHashedOrganizations();

            return GetOrganizationTree(organizations, startOrganizationId, 0);
        }


        public Dictionary<int, BasicOrganization> GetOrganizationHashtable (int startOrganizationId)
        {
            BasicOrganization[] organizations = GetOrganizationTree(startOrganizationId);

            Dictionary<int, BasicOrganization> result = new Dictionary<int, BasicOrganization>();

            foreach (BasicOrganization organization in organizations)
            {
                result[organization.OrganizationId] = organization;
            }

            return result;
        }


        private BasicOrganization[] GetOrganizationTree (Dictionary<int, List<BasicOrganization>> organizations,
                                                         int startOrganizationId, int generation)
        {
            List<BasicOrganization> result = new List<BasicOrganization>();

            List<BasicOrganization> thisList = organizations[startOrganizationId];

            foreach (BasicOrganization organization in thisList)
            {
                if (organization.OrganizationId != startOrganizationId)
                {
                    result.Add(organization);
                    // new Organization(organization.OrganizationId, organization.ParentOrganizationId, organization.Name, generation + 1));

                    // Add recursively

                    BasicOrganization[] children = GetOrganizationTree(organizations, organization.OrganizationId,
                                                                       generation + 1);

                    if (children.Length > 0)
                    {
                        foreach (BasicOrganization child in children)
                        {
                            result.Add(child);
                        }
                    }
                }
                else if (generation == 0)
                {
                    // The top parent is special and should be added; the others shouldn't

                    result.Add(organization);
                    //  (new Organization(organization.OrganizationId, organization.ParentOrganizationId, organization.Name, generation));
                }
            }

            return result.ToArray();
        }


        public BasicOrganization[] GetOrganizationLine (int leafOrganizationId)
        {
            List<BasicOrganization> result = new List<BasicOrganization>();

            Dictionary<int, List<BasicOrganization>> organizations = GetHashedOrganizations();

            BasicOrganization currentOrganization = organizations[leafOrganizationId][0];

            // This iterates until the zero-parentid root Organization is found

            while (currentOrganization.OrganizationId > 0)
            {
                result.Add(currentOrganization);

                if (currentOrganization.ParentOrganizationId > 0)
                {
                    currentOrganization = organizations[currentOrganization.ParentOrganizationId][0];
                }
                else
                {
                    currentOrganization = new BasicOrganization();
                }
            }

            result.Reverse();

            return result.ToArray();
        }
    }
}