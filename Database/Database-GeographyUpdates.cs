using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string geographyUpdateFieldSequence =
            " GeographyUpdates.GeographyUpdateId, GeographyUpdates.GeographyUpdateTypeId, " + // 0-1
            "GeographyUpdates.GeographyUpdateSourceId, GeographyUpdates.Guid, GeographyUpdates.CountryCode, " + // 2-4
            "GeographyUpdates.ChangeDataXml, GeographyUpdates.CreatedDateTime, " + // 5-6
            "GeographyUpdates.EffectiveDateTime,GeographyUpdates.Processed, " + // 7-8
            "GeographyUpdateTypes.Name AS GeographyUpdateType, " +    // 9
            "GeographyUpdateSources.Name AS GeographyUpdateSource " + // 10
            "FROM GeographyUpdates " +
            "JOIN GeographyUpdateTypes ON (GeographyUpdateTypes.GeographyUpdateTypeId=GeographyUpdates.GeographyUpdateTypeId) " +
            "JOIN GeographyUpdateSources ON (GeographyUpdateSources.GeographyUpdateSourceId=GeographyUpdates.GeographyUpdateSourceId) ";

        private static BasicGeographyUpdate ReadGeographyUpdateFromDataReader (DbDataReader reader)
        {
            int geographyUpdateId = reader.GetInt32 (0);
            // Fields# 1 and 2 are indexes into other tables, contents of which joined in #9 and #10 instead
            string guid = reader.GetString (3);
            string countryCode = reader.GetString (4);
            string changeDataXml = reader.GetString (5);
            DateTime createdDateTime = reader.GetDateTime (6);
            DateTime effectiveDateTime = reader.GetDateTime (7);
            bool processed = reader.GetBoolean (8);
            string updateType = reader.GetString (9);
            string updateSource = reader.GetString (10);

            return new BasicGeographyUpdate (geographyUpdateId, updateType, updateSource, guid, countryCode,
                changeDataXml, createdDateTime, effectiveDateTime, processed);
        }

        #endregion

        public BasicGeographyUpdate[] GetAllGeographyUpdates()
        {
            List<BasicGeographyUpdate> result = new List<BasicGeographyUpdate>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + geographyUpdateFieldSequence + " ORDER BY CreatedDateTime", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadGeographyUpdateFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicGeographyUpdate[] GetGeographyUpdatesSince(DateTime since)
        {
            List<BasicGeographyUpdate> result = new List<BasicGeographyUpdate>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + geographyUpdateFieldSequence + " WHERE CreatedDateTime >= @sinceDateTime", connection);
                AddParameterWithName(command, "@sinceDateTime", since);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadGeographyUpdateFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicGeographyUpdate[] GetUnprocessedGeographyUpdates()
        {
            List<BasicGeographyUpdate> result = new List<BasicGeographyUpdate>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + geographyUpdateFieldSequence + " WHERE EffectiveDateTime >= @dateTimeNow AND Processed=0", connection);
                AddParameterWithName(command, "@dateTimeNow", DateTime.UtcNow);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadGeographyUpdateFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public bool ExistGeographyUpdateWithGuid(string guid)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT Guid FROM GeographyUpdates WHERE Guid = @testGuid", connection);
                AddParameterWithName(command, "@testGuid", guid);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }

                    return false;
                }
            }
        }


        public int CreateGeographyUpdate (string updateType, string updateSource, string guid, string countryCode, string changeDataXml, DateTime createdDateTime, DateTime effectiveDateTime)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateGeographyUpdate", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "geographyUpdateType", updateType);
                AddParameterWithName(command, "geographyUpdateSource", updateSource);
                AddParameterWithName(command, "guid", guid);
                AddParameterWithName(command, "countryCode", countryCode);
                AddParameterWithName(command, "changeDataXml", changeDataXml);
                AddParameterWithName(command, "createdDateTime", createdDateTime);
                AddParameterWithName(command, "effectiveDateTime", effectiveDateTime);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public void SetGeographyUpdateProcessed (int geographyUpdateId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetGeographyUpdateProcessed", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "geographyUpdateId", geographyUpdateId);
                command.ExecuteNonQuery();
            }
        }
    }
}