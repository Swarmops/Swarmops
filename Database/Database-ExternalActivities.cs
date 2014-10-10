using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Field reading code

        private const string externalActivityFieldSequence =
            " ExternalActivityId,OrganizationId,GeographyId,DateTime,ExternalActivityTypes.Name," +   // 0-4
            " Description,CreatedDateTime,CreatedByPersonId,DupeOfActivityId" +
            " FROM ExternalActivities JOIN ExternalActivityTypes USING (ExternalActivityTypeId) ";

        private static BasicExternalActivity ReadExternalActivityFromDataReader (IDataRecord reader)
        {
            int externalActivityId = reader.GetInt32(0);
            int organizationId = reader.GetInt32(1);
            int geographyId = reader.GetInt32(2);
            DateTime dateTime = reader.GetDateTime(3);
            ExternalActivityType type = (ExternalActivityType)Enum.Parse(typeof(ExternalActivityType), reader.GetString(4));
            string description = reader.GetString(5);
            DateTime createdDateTime = reader.GetDateTime(6);
            int createdByPersonId = reader.GetInt32(7);
            int dupeOfActivityId = reader.GetInt32(8);

            return new BasicExternalActivity(externalActivityId, organizationId, geographyId, type, dateTime, description, createdByPersonId, createdDateTime, dupeOfActivityId);
        }

        #endregion



        #region Record reading - SELECT statements

        /// <summary>
        /// Gets an external activity from the database.
        /// </summary>
        /// <param name="externalActivityId">The external activity database identity.</param>
        /// <returns>The requested external activity.</returns>
        /// <exception cref="ArgumentException">Thrown if there is no such identity.</exception>
        public BasicExternalActivity GetExternalActivity (int externalActivityId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + externalActivityFieldSequence + "WHERE ExternalActivityId=" + externalActivityId + ";", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadExternalActivityFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown External Activity Id: " + externalActivityId.ToString());
                }
            }
        }

        /// <summary>
        /// Gets ExternalActivities from the database.
        /// </summary>
        /// <param name="conditions">An optional combination of a Person and/or Organization object and/or DatabaseCondition specifiers.</param>
        /// <returns>A list of matching ExternalActivities.</returns>
        public BasicExternalActivity[] GetExternalActivities (params object[] conditions)
        {
            return GetExternalActivitiesSorted("", 0, conditions);
        }

        /// <summary>
        /// Gets ExternalActivities from the database.
        /// </summary>
        /// <param name="conditions">An optional combination of a Person and/or Organization object and/or DatabaseCondition specifiers.</param>
        /// <returns>A list of matching ExternalActivities.</returns>
        public BasicExternalActivity[] GetExternalActivitiesSorted (string sortOrder, int limitCount, params object[] conditions)
        {
            List<BasicExternalActivity> result = new List<BasicExternalActivity>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                string sort = "";
                if (sortOrder == "DateDescending")
                {
                    sort = " order by DateTime DESC";
                }
                else if (sortOrder == "CreationDateDescending")
                {
                    sort = " order by CreatedDateTime DESC, DateTime DESC";
                }

                string limit = "";
                if (limitCount > 0)
                {
                    limit = " LIMIT " + limitCount;
                }


                DbCommand command =
                    GetDbCommand(
                        "SELECT" + externalActivityFieldSequence + ConstructWhereClause("ExternalActivities", conditions) + sort + limit, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadExternalActivityFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        #endregion



        #region Creation and manipulation - stored procedures

        public int CreateExternalActivity (int organizationId, int geographyId, DateTime dateTime, ExternalActivityType type, string description, int createdByPersonId)
        {
            if (description.Length > 256)
            {
                description = description.Substring(0, 250) + "...";
            }

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateExternalActivity", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "organizationId", organizationId);
                AddParameterWithName(command, "geographyId", geographyId);
                AddParameterWithName(command, "dateTime", dateTime);
                AddParameterWithName(command, "externalActivityType", type.ToString());
                AddParameterWithName(command, "description", description);
                AddParameterWithName(command, "createdByPersonId", createdByPersonId);
                AddParameterWithName(command, "createdDateTime", DateTime.Now);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }



        #endregion

    }
}