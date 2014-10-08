using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Security;
using System.Text;
using MySql.Data.MySqlClient;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public DbConnection GetSqlServerDbConnection()   // Temporarily public for migratory purposes
        {
            DbConnection connection = ProviderFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        protected DbConnection GetMySqlDbConnection () // This is a temporary function used during transition to MySql.
        {
            DbConnection connection = new MySqlConnection();

            string specialConnectString = "";
            //Added possibility to have MySQL connectionstring in web.config as "MyPirateWeb"
            //
            foreach (ConnectionStringSettings configConnStr in ConfigurationManager.ConnectionStrings)
            {
                if (configConnStr.Name == "MyPirateWeb")
                {
                    specialConnectString = configConnStr.ConnectionString;
                    break;
                }
            }
            //-------- end change /JL

            if (specialConnectString == "")
                specialConnectString = ConnectionString;
            specialConnectString = specialConnectString.Replace("192.168.0.5", "192.168.0.7"); // for dev & debug at Rick's

            connection.ConnectionString = specialConnectString;

            return connection;
        }

        protected DbCommand GetDbCommand (string commandText, DbConnection connection)
        {
            DbCommand command = connection.CreateCommand();
            if (command.GetType().FullName == "System.Data.OleDb.OleDbCommand")
            {
                commandText = commandText.
                    Replace("%ISTRUE%", string.Empty).
                    Replace("DISTINCT", string.Empty);
                if (commandText.Contains("%ISFALSE%"))
                {
                    throw new NotImplementedException("%ISFALSE% has not been implemented for OleDbConnections");
                }
            }
            else
            {
                commandText = commandText.Replace("%ISTRUE%", "= 1");
                commandText = commandText.Replace("%ISFALSE%", "= 0");
            }

            command.CommandText = commandText;
            return command;
        }

        protected void AddParameterWithName (DbCommand command, string parameterName, object value)
        {
            if (command.Parameters.GetType().Name == "SqlParameterCollection")
            {
                // The SqlParameterCollection only accepts SqlParameter objects, can't add value types directly

                SqlDbType parameterType = SqlDbType.NText;

                if (value is Type)
                {
                    if ((Type) value == typeof(int))
                    {
                        parameterType = SqlDbType.Int;
                    }
                    else if ((Type)value == typeof(DateTime))
                    {
                        parameterType = SqlDbType.DateTime;
                    }
                    else if ((Type)value == typeof(bool))
                    {
                        parameterType = SqlDbType.Bit;
                    }
                    else if ((Type)value == typeof(double) || (Type)value == typeof(float))
                    {
                        parameterType = SqlDbType.Float;
                    }
                    else if ((Type)value == typeof(decimal))
                    {
                        parameterType = SqlDbType.Money;
                    }
                    else if (!((Type)value == typeof(string)))
                    {
                        throw new Exception("Unhandled parameter type in AddParameterWithName: " + value.GetType().Name);
                    }
                    value = DBNull.Value;
                }
                else if (value is int)
                {
                    parameterType = SqlDbType.Int;
                }
                else if (value is DateTime)
                {
                    parameterType = SqlDbType.DateTime;
                }
                else if (value is bool)
                {
                    parameterType = SqlDbType.Bit;
                }
                else if (value is double || value is float)
                {
                    parameterType = SqlDbType.Float;
                }
                else if (value is decimal)
                {
                    parameterType = SqlDbType.Money;
                }
                else if (!(value is string))
                {
                    throw new Exception("Unhandled parameter type in AddParameterWithName: " + value.GetType().Name);
                }

                System.Data.SqlClient.SqlParameter parameter = null;

                if (parameterType == SqlDbType.NText)
                {
                    int textLength = 256*2;
                    if (value != DBNull.Value)
                    {
                        textLength = (value as string).Length * 2; // ntext / unicode buffer

                        // establish a minimum amount of even 256-byte blocks that this string will fit in

                        textLength += 2; // terminating zero, in unicode
                        textLength -= (textLength % 256);
                        textLength += 256;
                    }
                    parameter = new System.Data.SqlClient.SqlParameter("@" + parameterName, parameterType, textLength);
                }
                else
                {
                    // For all other types, ignore the length parameter, let the default handle it

                    parameter = new System.Data.SqlClient.SqlParameter("@" + parameterName, parameterType);
                }

                parameter.Value = value;

                System.Data.SqlClient.SqlParameterCollection sqlCollection =
                    command.Parameters as System.Data.SqlClient.SqlParameterCollection;

                sqlCollection.Add(parameter);
            }
            else if (command.GetType().FullName == "MySql.Data.MySqlClient.MySqlCommand")
            {
                MySqlParameter newParameter = new MySqlParameter(parameterName, value);
                command.Parameters.Add(newParameter);
            }
            else
            {
                int i = command.Parameters.Add(value);

                if (command.GetType().FullName == "System.Data.OracleClient.OracleCommand")
                {
                    command.Parameters[i].ParameterName = ":" + parameterName;
                }
                else
                {
                    command.Parameters[i].ParameterName = "@" + parameterName;
                }
            }
        }

        // A common function used here and there. Placed here for convenience.
        private static string JoinIds (int[] ids)
        {
            if (ids.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(ids[0].ToString());

            for (int index = 1; index < ids.Length; index++)
            {
                builder.Append(",");
                builder.Append(ids[index].ToString());
            }

            return builder.ToString();
        }

        // A common function used here and there. Placed here for convenience.
        private static string JoinStrings (string[] strs)
        {
            if (strs.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("'" + strs[0] + "'");

            for (int index = 1; index < strs.Length; index++)
            {
                builder.Append(",");
                builder.Append("'" + strs[index] + "'");
            }

            return builder.ToString();
        }
        
        // A common function used here and there. Placed here for convenience.
        private static string MySqlDate (DateTime d)
        {
            return d.ToString("yyyyMMddHHmmss");
        }


        private string SqlSanitize(string input)
        {
            string[] forbiddenArray = {"\\", "--", ";", "/*", "*/", "select ", "drop ", "update ", "delete ", "insert ", "="};

            string output = input.Replace("'", "''"); // the typical case
            string inputLower = input.ToLowerInvariant();
            foreach (string forbidden in forbiddenArray)
            {
                if (inputLower.Contains(forbidden))
                {
                    throw new SecurityException("Attempt at SQL injection: parameter passed to SELECT was '" + input + "'");
                }
            }

            return output;
        }
    }
}