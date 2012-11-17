using System.Text;
using System.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Activizr.Database
{
    partial class PirateDb
    {
        // This function is bloody dangerous, mmkay?

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
    }
}