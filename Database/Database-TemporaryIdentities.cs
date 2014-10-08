using System;
using System.Data;
using System.Data.Common;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public int GetTemporaryIdentity()
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("CreateTemporaryIdentity", connection);
                command.CommandType = CommandType.StoredProcedure;

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

    }
}