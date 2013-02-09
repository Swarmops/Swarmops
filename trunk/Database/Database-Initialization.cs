using System.Text;
using System.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Swarmops.Database
{
    partial class SwarmDb
    {
        // These functions are bloody dangerous, mmmkay?

        public void ExecuteAdminCommand(string sqlSequence)
        {
            // TODO: Verify that we're in admin mode

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand(sqlSequence, connection);
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }
        }

        public int ExecuteAdminCommandScalar (string sqlSequence)
        {
            // TODO: Verify that we're in admin mode

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand(sqlSequence, connection);
                command.CommandType = CommandType.Text;

                object result = command.ExecuteScalar();

                if (result == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToInt32(result);
            }
        }

    }
}