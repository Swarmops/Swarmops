using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        private const string exceptionLogFieldSequence =
            " ExceptionID, ExceptionDateTime, Source,ExceptionText FROM  ExceptionLog ";

        private static BasicExceptionLog ExceptionLogFromDataReader (DbDataReader reader)
        {
            int ExceptionId = reader.GetInt32 (0);
            DateTime ExceptionDateTime = reader.GetDateTime (1);
            string Source = reader.GetString (2);
            string ExceptionText = reader.GetString (3);
            return new BasicExceptionLog (ExceptionId, ExceptionDateTime, Source, ExceptionText);
        }

        public void CreateExceptionLogEntry (DateTime dateTime, string source, Exception e)
        {
            try
            {
                using (DbConnection connection = GetMySqlDbConnection())
                {
                    connection.Open();

                    DbCommand command = GetDbCommand ("CreateExceptionLogEntry", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameterWithName (command, "exceptionDateTime", dateTime);
                    AddParameterWithName (command, "source", source);
                    AddParameterWithName (command, "exceptionText", e.ToString());

                    command.ExecuteNonQuery(); // no primary key for exception log
                }
            }
            catch
            {
                //never fail from this routine, it is used in exceptionhandlig, 
                // and if it isn't possible log the error then so be it.
            }
        }


        public BasicExceptionLog[] GetExceptionLogTopEntries (int count)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                List<BasicExceptionLog> result = new List<BasicExceptionLog>();
                connection.Open();

                DbCommand command = GetDbCommand (@"
                        SELECT  " + exceptionLogFieldSequence + @"
                        order by ExceptionID desc LIMIT " + count, connection);

                command.CommandType = CommandType.Text;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ExceptionLogFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }
    }
}