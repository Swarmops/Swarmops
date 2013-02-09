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

        #region Database field reading

        private const string parleyOptionFieldSequence =
            " ParleyId, ParleyOptionId, Description, AmountCents, Active" +  // 0-4
            " FROM ParleyOptions ";

        private static BasicParleyOption ReadParleyOptionFromDataReader(IDataRecord reader)
        {
            int parleyId = reader.GetInt32(0);
            int parleyOptionId = reader.GetInt32(1);
            string description = reader.GetString(2);
            Int64 amountCents = reader.GetInt64(3);
            bool active = reader.GetBoolean(4);

            return new BasicParleyOption(parleyOptionId, parleyId, description, amountCents, active);
        }


        #endregion


        #region Database record reading -- SELECT clauses


        public BasicParleyOption GetParleyOption(int parleyOptionId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT" + parleyOptionFieldSequence +
                    "WHERE ParleyOptionId=" + parleyOptionId.ToString(),
                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadParleyOptionFromDataReader(reader);
                    }

                    throw new ArgumentException("No such ParleyOptionId:" + parleyOptionId.ToString());
                }
            }

        }


        public BasicParleyOption[] GetParleyOptions(params object[] conditions)
        {
            List<BasicParleyOption> result = new List<BasicParleyOption>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT" + parleyOptionFieldSequence + ConstructWhereClause("ParleyOptions", conditions), connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadParleyOptionFromDataReader(reader));
                    }

                    return result.ToArray();
                }
            }
        }


        #endregion


        #region Creation and manipulation -- stored procedures


        public int CreateParleyOption(int parleyId, string description, Int64 amountCents)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateParleyOption", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "parleyId", parleyId);
                AddParameterWithName(command, "description", description);
                AddParameterWithName(command, "amountCents", amountCents);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int SetParleyOptionActive(int parleyOptionId, bool active)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetParleyOptionActive", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "parleyOptionId", parleyOptionId);
                AddParameterWithName(command, "active", active);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        #endregion


    }
}