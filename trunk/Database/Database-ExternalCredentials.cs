using System;
using System.Data.Common;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class PirateDb
    {
        public BasicExternalCredential GetExternalCredential (string serviceName)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT ExternalCredentialId,ServiceName,Login,Password From ExternalCredentials WHERE ServiceName='" + serviceName.Replace("'", "''") + "'",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadExternalCredentialFromDataReader(reader);
                    }

                    throw new ArgumentException("Unknown Service Name");
                }
            }
        }


        private BasicExternalCredential ReadExternalCredentialFromDataReader (DbDataReader reader)
        {
            int externalCredentialId = reader.GetInt32(0);
            string serviceName = reader.GetString(1);
            string login = reader.GetString(2);
            string password = reader.GetString(3);

            return new BasicExternalCredential(externalCredentialId, serviceName, login, password);
        }
    }
}