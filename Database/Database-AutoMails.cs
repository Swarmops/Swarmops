using System;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types.Communications;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public BasicAutoMail[] GetAllAutoMailsForMigration()
        {
            return new BasicAutoMail[0];
        }

        public BasicAutoMail GetAutoMail (AutoMailType type, int organizationId, int geographyId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "select AutoMails.AutoMailId,AutoMailTypes.Name,AutoMails.OrganizationId,AutoMails.GeographyId,AutoMails.AuthorPersonId,Title,Body " +
                        "FROM AutoMails JOIN AutoMailTypes USING (AutoMailTypeId) " +
                        "WHERE AutoMailTypes.Name='" + type + "' AND AutoMails.OrganizationId=" + organizationId +
                        " AND AutoMails.GeographyId=" + geographyId, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadAutoMailFromDataReader (reader);
                    }

                    return null; // NULL returned - unusual; usually exception is thrown if not found
                }
            }
        }


        private static BasicAutoMail ReadAutoMailFromDataReader (DbDataReader reader)
        {
            int autoMailId = reader.GetInt32 (0);
            AutoMailType type = (AutoMailType) Enum.Parse (typeof (AutoMailType), reader.GetString (1));
            int geographyId = reader.GetInt32 (3);
            int organizationId = reader.GetInt32 (2);
            int authorPersonId = reader.GetInt32 (4); // appear to be unused?
            string title = reader.GetString (5);
            string body = reader.GetString (6);

            return new BasicAutoMail (autoMailId, type, organizationId, geographyId, authorPersonId,
                title, body);
        }


        public int SetAutoMail (AutoMailType type, int organizationId, int geographyId,
            int authorPersonId, string title, string body)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetAutoMail", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "autoMailType", type.ToString());
                AddParameterWithName (command, "organizationId", organizationId);
                AddParameterWithName (command, "geographyId", geographyId);
                AddParameterWithName (command, "authorPersonId", authorPersonId);
                AddParameterWithName (command, "title", title);
                AddParameterWithName (command, "body", body);

                return Convert.ToInt32 (command.ExecuteScalar());
            }
        }


        public int SetAutoMail (BasicAutoMail autoMail)
        {
            return SetAutoMail (autoMail.Type, autoMail.OrganizationId, autoMail.GeographyId,
                autoMail.AuthorPersonId, autoMail.Title, autoMail.Body);
        }
    }
}