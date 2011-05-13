using System;
using System.Data;
using System.Data.Common;
using Activizr.Basic.Types;

namespace Activizr.Database
{
    public partial class PirateDb
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