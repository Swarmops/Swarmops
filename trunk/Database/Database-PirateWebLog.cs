using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public int CreatePWLogEntry (DateTime dateTimeUtc, int actingPersonId, string affectedItemType,
                                               int affectedItemId, string actionType, string actionDescription,
                                               string changedField, string valueBefore,
                                               string valueAfter, string comment, string ipAddress)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();
                DbCommand command = GetDbCommand("CreatePWLogEntry2", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "dateTimeUtc", dateTimeUtc);
                AddParameterWithName(command, "actingPersonId", actingPersonId);
                AddParameterWithName(command, "affectedItemType", affectedItemType);
                AddParameterWithName(command, "affectedItemId", affectedItemId);
                AddParameterWithName(command, "actionType", actionType);
                AddParameterWithName(command, "actionDescription", actionDescription);
                AddParameterWithName(command, "changedField", changedField);
                AddParameterWithName(command, "valueBefore", valueBefore);
                AddParameterWithName(command, "valueAfter", valueAfter);
                AddParameterWithName(command, "comment", comment);
                AddParameterWithName(command, "ipAddress", ipAddress);

                int logEntryId = Convert.ToInt32(command.ExecuteScalar());
                return logEntryId;
            }
        }

        public DateTime CheckLogEntry (string affectedItemType, int affectedItemId, string actionType)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT Max(DateTimeUtc) from PWLog WHERE AffectedItemId=" + affectedItemId + " AND AffectedItemType='" + affectedItemType + "' AND ActionType='" + actionType + "'",
                                 connection);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            return reader.GetDateTime(0);
                        }
                    }
                    return DateTime.MinValue;
                }
            }

        }

        public BasicPWLog[] GetLatestEvents (string affectedItemType, DateTime beforeDate, int[] affectedIds, string[] actionTypes)
        {
            List<BasicPWLog> retlist = new List<BasicPWLog>();
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT DateTimeUtc, ActingPersonId, AffectedItemType, AffectedItemId, ActionType, " +
                                    " ActionDescription, ChangedField, ValueBefore, ValueAfter, Comment, IpAddress " +
                                    " FROM PWLog  " +
                                    " WHERE AffectedItemId in (" + JoinIds(affectedIds) + " ) " +
                                        " AND AffectedItemType='" + affectedItemType + "' " +
                                        " AND ActionType in (" + JoinStrings(actionTypes)+ ")" +
                                        " AND DateTimeUtc < " + MySqlDate(beforeDate) + " order by DateTimeUtc desc",
                                 connection);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime dateTimeUtc = reader.GetDateTime(0);
                        int actingPersonId = reader.GetInt32(1);
                        string affectedItemTyp = reader.GetString(2);
                        int affectedItemId = reader.GetInt32(3);
                        string actionType = reader.GetString(4);
                        string actionDescription = reader.IsDBNull(5) ? "" :reader.GetString(5);
                        string changedField = reader.IsDBNull(6) ? "" : reader.GetString(6);
                        string valueBefore = reader.IsDBNull(7) ? "" : reader.GetString(7);
                        string valueAfter = reader.IsDBNull(8) ? "" : reader.GetString(8);
                        string comment = reader.IsDBNull(9) ? "" : reader.GetString(9);
                        string ipAddress = reader.IsDBNull(10) ? "" : reader.GetString(10);

                        retlist.Add(new BasicPWLog(dateTimeUtc,
                                                    actingPersonId,
                                                    affectedItemType,
                                                    affectedItemId,
                                                    actionType,
                                                    actionDescription,
                                                    changedField,
                                                    valueBefore,
                                                    valueAfter,
                                                    comment,
                                                    ipAddress));
                    }
                }
            }
            return retlist.ToArray();
        }

    }
}