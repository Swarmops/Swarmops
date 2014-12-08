using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        private const string outboundMailRecipientFieldSequence =
            " OutboundMailRecipientId,OutboundMailId,PersonId,AsOfficer,PersonType" + // 0-4
            " FROM OutboundMailRecipients ";


        private BasicOutboundMailRecipient ReadOutboundMailRecipientFromDataReader (DbDataReader reader)
        {
            int outboundMailRecipientId = reader.GetInt32 (0);
            int outboundMailId = reader.GetInt32 (1);
            int personId = reader.GetInt32 (2);
            bool asOfficer = reader.GetBoolean (3);
            int personType = reader.GetInt32 (4);

            return new BasicOutboundMailRecipient (outboundMailRecipientId, outboundMailId, personId, asOfficer,
                personType);
        }


        public BasicOutboundMailRecipient[] GetTopOutboundMailRecipients (int outboundMailId, int batchSize)
        {
            List<BasicOutboundMailRecipient> result = new List<BasicOutboundMailRecipient>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + outboundMailRecipientFieldSequence + " WHERE OutboundMailId=" +
                        outboundMailId + " LIMIT " + batchSize, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadOutboundMailRecipientFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public void DeleteOutboundMailRecipient (int outboundMailRecipientId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("DeleteOutboundMailRecipient", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundMailRecipientId", outboundMailRecipientId);

                command.ExecuteNonQuery();
            }
        }

        public void CreateOutboundMailRecipient (int outboundMailId, int personId, bool asOfficer, int personType)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateOutboundMailRecipient", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundMailId", outboundMailId);
                AddParameterWithName (command, "personId", personId);
                AddParameterWithName (command, "asOfficer", asOfficer);
                AddParameterWithName (command, "personType", personType);

                command.ExecuteNonQuery();
            }
        }
    }
}