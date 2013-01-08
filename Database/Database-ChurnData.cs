using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class PirateDb
    {
        #region Field reading code

        private const string churnDataFieldSequence =
            " PersonId,OrganizationId,Churn,DecisionDateTime,ExpiryDateTime " +  // 0-4
            " FROM ChurnData ";

        static private BasicChurnDataPoint ReadChurnDataPointFromDataReader (DbDataReader reader)
        {
            int personId = reader.GetInt32(0);
            int organizationId = reader.GetInt32(1);
            bool churn = reader.GetBoolean(2);
            DateTime decisionDateTime = reader.GetDateTime(3);
            DateTime expiryDateTime = reader.GetDateTime(4);

            return new BasicChurnDataPoint(churn ? ChurnDataType.Churn : ChurnDataType.Retention, decisionDateTime,
                                           expiryDateTime, personId, organizationId);
        }

        #endregion


        #region Record reading code -- SELECT statements

        public BasicChurnDataPoint[] GetChurnData (params object[] conditions)
        {
            List<BasicChurnDataPoint> result = new List<BasicChurnDataPoint>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT " + churnDataFieldSequence + ConstructWhereClause("ChurnData", conditions) +
                        " ORDER BY ExpiryDateTime", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadChurnDataPointFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicChurnDataPoint[] GetChurnDataForOrganization (IHasIdentity organization, DateTime lowerDate,
                                                                  DateTime upperDate)
        {
            // Since I don't trust SQL Server to make correct date comparisons, especially given
            // that the dates are passed in text in SQL, we get ALL the data and do
            // the comparison in code instead. This is a run-seldom function, anyway, and getting
            // some 30k records with two unlinked fields isn't that expensive.

            DateTime minimumDateTime = lowerDate.Date;
            DateTime maximumDateTime = upperDate.Date.AddDays(1);

            List<BasicChurnDataPoint> result = new List<BasicChurnDataPoint>();
            BasicChurnDataPoint[] rawData = this.GetChurnData(organization);

            foreach (BasicChurnDataPoint churnPoint in rawData)
            {
                // It is important that the lower border is inclusive and the upper exclusive in this
                // comparison:

                if (churnPoint.ExpiryDate >= minimumDateTime && churnPoint.ExpiryDate < maximumDateTime)
                {
                    result.Add(churnPoint);
                }
            }

            return result.ToArray();
        }
        
        #endregion

        
        #region Record creation and manipulation code -- stored procedures

        public int LogChurnData(int personId, int organizationId, bool churn, DateTime expiryDateTime)
        {
            return LogChurnData(personId, organizationId, churn, expiryDateTime, DateTime.Now);
        }

        public int LogChurnData(int personId, int organizationId, bool churn, DateTime expiryDateTime,
                                 DateTime decisionDateTime)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("LogChurnData", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "p_personId", personId);
                AddParameterWithName(command, "p_organizationId", organizationId);
                AddParameterWithName(command, "p_churn", churn);
                AddParameterWithName(command, "p_decisionDateTime", decisionDateTime);
                AddParameterWithName(command, "p_expiryDateTime", expiryDateTime);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        #endregion
    }
}