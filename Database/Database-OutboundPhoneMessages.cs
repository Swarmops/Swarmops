namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        /*
        public void LogTransmittedPhoneMessage (int personId, string phoneNumber, string message)
        {
            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("LogTransmittedPhoneMessage", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "dateTime", DateTime.Now);
                AddParameterWithName(command, "personId", personId);
                AddParameterWithName(command, "phoneNumber", phoneNumber);
                AddParameterWithName(command, "message", message);

                command.ExecuteNonQuery();
            }
        }*/
    }
}