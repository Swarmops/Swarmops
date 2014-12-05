using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        #region Database field reading

        private const string parleyFieldSequence =
            " ParleyId, OrganizationId, PersonId, BudgetId, CreatedDateTime, " + // 0-4
            "Open, Attested, Name, GeographyId, Description, " + // 5-9
            "InformationUrl, StartDate, EndDate, BudgetCents, GuaranteeCents, " + // 10-14
            "AttendanceFeeCents, ClosedDateTime" + // 15-16
            " FROM Parleys ";

        private static BasicParley ReadParleyFromDataReader (IDataRecord reader)
        {
            int parleyId = reader.GetInt32 (0);
            int organizationId = reader.GetInt32 (1);
            int personId = reader.GetInt32 (2);
            int budgetId = reader.GetInt32 (3);
            DateTime createdDateTime = reader.GetDateTime (4);

            bool open = reader.GetBoolean (5);
            bool attested = reader.GetBoolean (6);
            string name = reader.GetString (7);
            int geographyId = reader.GetInt32 (8);
            string description = reader.GetString (9);

            string informationUrl = reader.GetString (10);
            DateTime startDate = reader.GetDateTime (11);
            DateTime endDate = reader.GetDateTime (12);
            Int64 budgetCents = reader.GetInt64 (13);
            Int64 guaranteeCents = reader.GetInt64 (14);

            Int64 attendanceFeeCents = reader.GetInt64 (15);
            DateTime closedDateTime = reader.GetDateTime (16);

            return new BasicParley (
                parleyId, organizationId, personId, budgetId, createdDateTime,
                open, attested, name, geographyId, description,
                informationUrl, startDate, endDate, budgetCents, guaranteeCents,
                attendanceFeeCents, closedDateTime);
        }

        #endregion

        #region Database record reading -- SELECT clauses

        public BasicParley GetParley (int parleyId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand ("SELECT" + parleyFieldSequence +
                                  "WHERE ParleyId=" + parleyId,
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadParleyFromDataReader (reader);
                    }

                    throw new ArgumentException ("No such ParleyId:" + parleyId);
                }
            }
        }


        public BasicParley[] GetParleys (params object[] conditions)
        {
            List<BasicParley> result = new List<BasicParley>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT" + parleyFieldSequence + ConstructWhereClause ("Parleys", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadParleyFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }

        #endregion

        #region Creation and manipulation -- stored procedures

        public int CreateParley (int organizationId, int personId, int budgetId, string name, int geographyId,
            string description, string informationUrl, DateTime startDate, DateTime endDate, Int64 budgetCents,
            Int64 guaranteeCents, Int64 attendanceFeeCents)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateParley", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "organizationId", organizationId);
                AddParameterWithName (command, "personId", personId);
                AddParameterWithName (command, "budgetId", budgetId);
                AddParameterWithName (command, "name", name);
                AddParameterWithName (command, "geographyId", geographyId);
                AddParameterWithName (command, "description", description);
                AddParameterWithName (command, "informationUrl", informationUrl);
                AddParameterWithName (command, "startDate", startDate);
                AddParameterWithName (command, "endDate", endDate);
                AddParameterWithName (command, "budgetCents", budgetCents);
                AddParameterWithName (command, "guaranteeCents", guaranteeCents);
                AddParameterWithName (command, "attendanceFeeCents", attendanceFeeCents);
                AddParameterWithName (command, "createdDateTime", DateTime.Now);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public int SetParleyAttested (int parleyId, bool attested)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetParleyAttested", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "parleyId", parleyId);
                AddParameterWithName (command, "attested", attested);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public int SetParleyBudget (int parleyId, int budgetId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetParleyBudget", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "parleyId", parleyId);
                AddParameterWithName (command, "budgetId", budgetId);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        public int SetParleyOpen (int parleyId, bool open)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetParleyOpen", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "parleyId", parleyId);
                AddParameterWithName (command, "open", open);
                AddParameterWithName (command, "closedDateTime", DateTime.Now); // ignored if open=true

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }

        #endregion
    }
}