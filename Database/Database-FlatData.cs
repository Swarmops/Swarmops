using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public string GetKeyValue (string key)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand ("SELECT DataValue FROM FlatData WHERE DataKey=@dataKey",
                        connection);
                AddParameterWithName(command, "dataKey", key);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString (0);
                    }

                    return string.Empty;
                }
            }
        }


        public void SetKeyValue (string key, string value)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetKeyValue", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "dataKey", key);
                AddParameterWithName (command, "dataValue", value);

                command.ExecuteNonQuery();
            }
        }


        public Dictionary<string, string> GetOldKeyValues()
        {
            /*
            Dictionary<string, string> result = new Dictionary<string, string>();

            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand("SELECT [Key],[Value] FROM FlatData", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result [reader.GetString(0)] = reader.GetString(1);
                    }

                    return result;
                }
            }*/

            return new Dictionary<string, string>();
        }
    }
}