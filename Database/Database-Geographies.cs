using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;
using Swarmops.Common.Enums;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string geographyFieldSequence =
            " GeographyId, ParentGeographyId, Name " + // 0-2
            " FROM Geographies ";


        private const string geographyDesignationFieldSequence =
            " GeographyId, CountryId, Designation, GeographyLevelId " + // 0-3
            " FROM GeographyDesignations ";

        private static BasicGeography ReadGeographyFromDataReader (DbDataReader reader)
        {
            int geographyId = reader.GetInt32 (0);
            int parentGeographyId = reader.GetInt32 (1);
            string name = reader.GetString (2);

            return new BasicGeography (geographyId, parentGeographyId, name);
        }

        private static BasicGeographyDesignation ReadGeographyDesignationFromDataReader (DbDataReader reader)
        {
            int geographyId = reader.GetInt32 (0);
            int countryId = reader.GetInt32 (1);
            string designation = reader.GetString (2);
            GeographyLevel level = (GeographyLevel) reader.GetInt32 (3);

            return new BasicGeographyDesignation (geographyId, countryId, designation, level);
        }

        #endregion

        public BasicGeography[] GetAllGeographies()
        {
            List<BasicGeography> result = new List<BasicGeography>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + geographyFieldSequence + " WHERE ParentGeographyId >= 0 ORDER BY \"Name\"",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadGeographyFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicGeography[] GetGeographies (int[] ids)
        {
            List<BasicGeography> result = new List<BasicGeography>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + geographyFieldSequence + " WHERE ParentGeographyId >= 0 AND GeographyId in (" +
                        JoinIds (ids) +
                        ") ORDER BY \"Name\"", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadGeographyFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicGeography[] GetGeographyChildren (int parentGeographyId)
        {
            List<BasicGeography> result = new List<BasicGeography>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + geographyFieldSequence + " WHERE ParentGeographyId=" + parentGeographyId +
                        " ORDER BY \"Name\"", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadGeographyFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public Dictionary<int, List<BasicGeography>> GetHashedGeographies()
        {
            // This generates a Dictionary <int,List<Node>>.
            // 
            // Keys are integers corresponding to NodeIds. At each key n,
            // the value is an List<Node> starting with the node n followed by
            // its children.
            //
            // (Later reflection:) O(n) complexity, instead of recursion. Nice!

            Dictionary<int, List<BasicGeography>> result = new Dictionary<int, List<BasicGeography>>();

            BasicGeography[] nodes = GetAllGeographies();

            // Add the nodes.

            foreach (BasicGeography node in nodes)
            {
                List<BasicGeography> newList = new List<BasicGeography>();
                newList.Add (node);

                result[node.GeographyId] = newList;
            }

            // Add the children.

            foreach (BasicGeography node in nodes)
            {
                if (node.ParentGeographyId != 0)
                {
                    result[node.ParentGeographyId].Add (node);
                }
            }

            return result;
        }


        public BasicGeography[] GetGeographyLine (int leafGeographyId)
        {
            List<BasicGeography> result = new List<BasicGeography>();

            Dictionary<int, List<BasicGeography>> nodes = GetHashedGeographies();

            BasicGeography currentNode = nodes[leafGeographyId][0];

            // This iterates until the zero-parentid root node is found

            while (currentNode.GeographyId != 0)
            {
                result.Add (currentNode);

                if (currentNode.ParentGeographyId != 0)
                {
                    currentNode = nodes[currentNode.ParentGeographyId][0];
                }
                else
                {
                    currentNode = new BasicGeography (0, 0, string.Empty);
                }
            }

            result.Reverse();

            return result.ToArray();
        }


        public BasicGeography[] GetGeographyTree()
        {
            return GetGeographyTree (1);
        }


        public BasicGeography[] GetGeographyTree (int startGeographyId)
        {
            Dictionary<int, List<BasicGeography>> nodes = GetHashedGeographies();

            return GetGeographyTree (nodes, startGeographyId, 0);
        }


        public Dictionary<int, BasicGeography> GetGeographyHashtable (int startGeographyId)
        {
            BasicGeography[] nodes = GetGeographyTree (startGeographyId);

            Dictionary<int, BasicGeography> result = new Dictionary<int, BasicGeography>();

            foreach (BasicGeography node in nodes)
            {
                result[node.GeographyId] = node;
            }

            return result;
        }


        private BasicGeography[] GetGeographyTree (Dictionary<int, List<BasicGeography>> geographies, int startNodeId,
            int generation)
        {
            List<BasicGeography> result = new List<BasicGeography>();

            List<BasicGeography> thisList = geographies[startNodeId];

            foreach (BasicGeography node in thisList)
            {
                if (node.GeographyId != startNodeId)
                {
                    result.Add (new BasicGeography (node.GeographyId, node.ParentGeographyId, node.Name, generation + 1));

                    // Add recursively

                    BasicGeography[] children = GetGeographyTree (geographies, node.GeographyId, generation + 1);

                    if (children.Length > 0)
                    {
                        foreach (BasicGeography child in children)
                        {
                            result.Add (child);
                        }
                    }
                }
                else if (generation == 0)
                {
                    // The top parent is special and should be added; the others shouldn't

                    result.Add (new BasicGeography (node.GeographyId, node.ParentGeographyId, node.Name, generation));
                }
            }

            return result.ToArray();
        }


        public BasicGeography GetGeography (int geographyId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand (
                    "SELECT " + geographyFieldSequence + " WHERE GeographyId=" + geographyId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadGeographyFromDataReader (reader);
                    }

                    throw new ArgumentException ("No such GeographyId: " + geographyId);
                }
            }
        }


        public BasicGeography GetGeographyByName (string geographyName)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + geographyFieldSequence + " WHERE Name ='" + geographyName.Replace ("'", "''") + "'",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadGeographyFromDataReader (reader);
                    }

                    throw new ArgumentException ("No such Geography: " + geographyName);
                }
            }
        }


        public void SetGeographyName (int geographyId, string name)
        {
            throw new NotImplementedException ("Renaming geographies is not migrated to MySQL");
            /*
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetGeographyName", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "name", name);

                command.ExecuteNonQuery();
            }*/
        }

        public Dictionary<int, int> GetGeographyVoterCounts()
        {
            throw new NotImplementedException ("Not Migrated to MySQL");
            /*
            Dictionary<int, int> result = new Dictionary<int, int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "select GeographyId, SUM(VoterCount) AS VoterCount from VotingDistricts GROUP BY GeographyId",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result[reader.GetInt32(0)] = reader.GetInt32(1);
                    }

                    return result;
                }
            }*/
        }

        public void CreateGeographyOfficialDesignation (int geographyId, GeographyLevel level, int countryId,
            string designation)
        {
            throw new NotImplementedException ("Not Migrated to MySQL");
            /*
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateGeographyOfficialDesignation", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "countryId", countryId);
                AddParameterWithName(command, "geographyLevelId", (int)level);
                AddParameterWithName(command, "designation", designation);

                command.ExecuteNonQuery();
            }*/
        }

        public int GetGeographyIdFromOfficialDesignation (int countryId, GeographyLevel level, string designation)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "select GeographyId FROM GeographyDesignations WHERE CountryId=" + countryId +
                        " AND GeographyLevelId= " + ((int) level) + " AND Designation='" +
                        designation.Replace ("'", "''") + "'", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32 (0);
                    }
                    throw new ArgumentException ("No such designation: " + designation);
                }
            }
        }

        public GeographyLevel[] GetGeographyLevelsAtGeographyId (int geographyId)
        {
            List<GeographyLevel> result = new List<GeographyLevel>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "select GeographyLevelId FROM GeographyDesignations WHERE GeographyId=" +
                        geographyId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add ((GeographyLevel) reader.GetInt32 (0));
                    }
                }
            }

            return result.ToArray();
        }

        public BasicGeographyDesignation[] GetGeographyDesignationsForGeographyId (int geographyId)
        {
            List<BasicGeographyDesignation> result = new List<BasicGeographyDesignation>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + geographyDesignationFieldSequence + " WHERE GeographyId=" + geographyId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadGeographyDesignationFromDataReader (reader));
                    }
                }
            }

            return result.ToArray();
        }

        public int[] GetGeographyIdsFromLevel (int countryId, GeographyLevel level)
        {
            List<int> result = new List<int>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "select GeographyId FROM GeographyDesignations WHERE GeographyLevelId=" +
                        ((int) level) + " AND CountryId=" + countryId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (reader.GetInt32 (0));
                    }
                }
            }

            return result.ToArray();
        }

        public int CreateGeography (string name, int parentGeographyId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateGeography", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "name", name);
                AddParameterWithName (command, "parentGeographyId", parentGeographyId);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }
    }
}