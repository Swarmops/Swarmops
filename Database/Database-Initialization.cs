using System;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Swarmops.Common.Exceptions;

namespace Swarmops.Database
{
    partial class SwarmDb
    {
        // These functions are bloody dangerous, mmmkay?

        public void ExecuteAdminCommand(string sql)
        {
            // TODO: Verify that we're in admin mode

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand(sql, connection);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        public void ExecuteAdminCommands(string[] sqlSequence)
        {
            // TODO: Verify that we're in admin mode

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                foreach (string sql in sqlSequence)
                {
                    DbCommand command = GetDbCommand (sql, connection);
                    command.CommandType = CommandType.Text;

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (MySqlException innerException)
                    {
                        throw new DatabaseExecuteException(sql, innerException);
                    }
                }
            }
        }

        public int ExecuteAdminCommandScalar(string sqlSequence)
        {
            // TODO: Verify that we're in admin mode

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand (sqlSequence, connection);
                command.CommandType = CommandType.Text;

                object result = command.ExecuteScalar();

                if (result == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToInt32 (result);
            }
        }
    }
}