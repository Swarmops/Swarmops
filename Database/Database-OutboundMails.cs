using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        private const string outboundMailFieldSequence =
            " OutboundMailId,AuthorType,AuthorPersonId,Title,Body," + // 0-4
            "MailPriority,MailType,OrganizationId,GeographyId,CreatedDateTime," + // 5-9
            "ReleaseDateTime,ReadyForPickup,Resolved,Processed,ResolvedDateTime," + // 10-14
            "StartProcessDateTime,EndProcessDateTime,RecipientCount,RecipientsSuccess,RecipientsFail " + // 15-19
            " FROM OutboundMails ";

        private static BasicOutboundMail ReadOutboundMailFromDataReader (IDataRecord reader)
        {
            int outboundMailId = reader.GetInt32 (0);
            int authorType = reader.GetInt32 (1);
            int authorPersonId = reader.GetInt32 (2);
            string title = reader.GetString (3);
            string body = reader.GetString (4);

            int mailPriority = reader.GetInt32 (5);
            int mailTypeId = reader.GetInt32 (6);
            int organizationId = reader.GetInt32 (7);
            int geographyId = reader.GetInt32 (8);
            DateTime createdDateTime = reader.GetDateTime (9);

            DateTime releaseDateTime = reader.GetDateTime (10);
            bool readyForPickup = reader.GetBoolean (11);
            bool resolved = reader.GetBoolean (12);
            bool processed = reader.GetBoolean (13);
            DateTime resolvedDateTime = reader.GetDateTime (14);

            DateTime startProcessDateTime = reader.GetDateTime (15);
            DateTime endProcessDateTime = reader.GetDateTime (16);
            int recipientCount = reader.GetInt32 (17);
            int recipientsSuccess = reader.GetInt32 (18);
            int recipientsFail = reader.GetInt32 (19);

            return new BasicOutboundMail (outboundMailId, (MailAuthorType) authorType, authorPersonId, title, body,
                mailPriority, mailTypeId, organizationId, geographyId, createdDateTime,
                releaseDateTime, readyForPickup, resolved, processed, resolvedDateTime, startProcessDateTime,
                endProcessDateTime,
                recipientCount, recipientsSuccess, recipientsFail);
        }


        public BasicOutboundMail GetOutboundMail (int outboundMailId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + outboundMailFieldSequence + " WHERE OutboundMailId=" + outboundMailId.ToString(),
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadOutboundMailFromDataReader (reader);
                    }
                    else
                    {
                        throw new ArgumentException ("No such OutboundMailId: " + outboundMailId.ToString());
                    }
                }
            }
        }

        public BasicOutboundMail[] GetTopUnresolvedOutboundMail (int count)
        {
            if (count == 0)
            {
                return new BasicOutboundMail[0];
            }

            List<BasicOutboundMail> result = new List<BasicOutboundMail>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + outboundMailFieldSequence +
                        " WHERE ReadyForPickup=1 AND Resolved=0 AND Processed=0  ORDER BY MailPriority, OutboundMailId DESC LIMIT " +
                        count.ToString(),
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadOutboundMailFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicOutboundMail[] GetTopUnprocessedOutboundMail (int count)
        {
            if (count == 0)
            {
                return new BasicOutboundMail[0];
            }

            List<BasicOutboundMail> result = new List<BasicOutboundMail>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT " + outboundMailFieldSequence +
                        " WHERE ReadyForPickup=1 AND Resolved=1 AND Processed=0 " +
                        " AND ReleaseDateTime < NOW() " +
                        " ORDER BY MailPriority, ReleaseDateTime, OutboundMailId DESC LIMIT " + count.ToString(),
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadOutboundMailFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }

        public BasicOutboundMail[] GetOutboundMailQueue (int count)
        {
            // This may not survive the migration. SQL Syntax, etc.

            if (count == 0)
            {
                return new BasicOutboundMail[0];
            }

            List<BasicOutboundMail> result = new List<BasicOutboundMail>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        String.Format (@"
SELECT " + outboundMailFieldSequence + @"
where OutboundMailId in 
        (
            select * from (SELECT OutboundMailId 
                            From OutboundMails 
                            WHERE ReadyForPickup=1 AND Resolved>=0 AND Processed=0      
                            ORDER BY MailPriority, ReleaseDateTime, OutboundMailId DESC LIMIT {0}) A  
            UNION ALL
            select * from (SELECT  OutboundMailId 
                            From OutboundMails 
                            WHERE ReadyForPickup=1 AND Resolved>=0 AND Processed=1 
                                AND  EndProcessDateTime > DATE_ADD(Now(), INTERVAL - 2 HOUR) LIMIT {0}   ) B 
        ) 
order by EndProcessDateTime, MailPriority, ReleaseDateTime, OutboundMailId DESC 
", count),
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadOutboundMailFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }


        public int CreateOutboundMail (MailAuthorType authorType, int authorPersonId, string title,
            string body, int mailPriority, int mailType, int geographyId,
            int organizationId, DateTime releaseDateTime)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateOutboundMail", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "authorType", (int) authorType);
                AddParameterWithName (command, "authorPersonId", authorPersonId);
                AddParameterWithName (command, "title", title);
                AddParameterWithName (command, "body", body);
                AddParameterWithName (command, "mailPriority", mailPriority);
                AddParameterWithName (command, "mailType", mailType);
                AddParameterWithName (command, "geographyId", geographyId);
                AddParameterWithName (command, "organizationId", organizationId);
                AddParameterWithName (command, "createdDateTime", DateTime.Now);
                AddParameterWithName (command, "releaseDateTime", releaseDateTime);

                int result = Convert.ToInt32 (command.ExecuteScalar());

                if (result == 0)
                {
                    throw new Exception ("Unable to create outbound mail");
                }

                return result;
            }
        }

        public void SetOutboundMailProcessed (int outboundMailId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundMailProcessed", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundMailId", outboundMailId);
                AddParameterWithName (command, "datetime", DateTime.Now);

                command.ExecuteNonQuery();
            }
        }


        public void SetOutboundMailStartProcess (int outboundMailId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundMailStartProcess", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundMailId", outboundMailId);
                AddParameterWithName (command, "datetime", DateTime.Now);

                command.ExecuteNonQuery();
            }
        }


        public void SetOutboundMailResolved (int outboundMailId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundMailResolved", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundMailId", outboundMailId);
                AddParameterWithName (command, "datetime", DateTime.Now);

                command.ExecuteNonQuery();
            }
        }

        public void SetOutboundMailReadyForPickup (int outboundMailId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundMailReadyForPickup", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundMailId", outboundMailId);

                command.ExecuteNonQuery();
            }
        }

        public void SetOutboundMailRecipientCount (int outboundMailId, int recipientCount)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetOutboundMailRecipientCount", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundMailId", outboundMailId);
                AddParameterWithName (command, "recipientCount", recipientCount);

                command.ExecuteNonQuery();
            }
        }

        public void IncrementOutboundMailSuccesses (int outboundMailId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("IncrementOutboundMailSuccesses", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundMailId", outboundMailId);

                command.ExecuteNonQuery();
            }
        }

        public void IncrementOutboundMailFailures (int outboundMailId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("IncrementOutboundMailFailures", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "outboundMailId", outboundMailId);

                command.ExecuteNonQuery();
            }
        }


        public BasicOutboundMail[] GetDuplicateOutboundMail (MailAuthorType authorType, int authorPersonId, string title,
            string body, int mailPriority, int mailType, int geographyId,
            int organizationId, DateTime createDateTime, int recieverId)
        {
            List<BasicOutboundMail> result = new List<BasicOutboundMail>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();
                DbCommand command =
                    GetDbCommand (
                        "SELECT " + outboundMailFieldSequence +
                        @" INNER JOIN OutboundMailRecipients ON OutboundMailRecipients.OutboundMailId = OutboundMails.OutboundMailId
                            WHERE (OutboundMailRecipients.PersonId = " + recieverId + @") 
                            AND (OutboundMails.AuthorType = " + (int) authorType + @") 
                            AND (OutboundMails.AuthorPersonId = " + authorPersonId + @") 
                            AND (OutboundMails.Title = '" + title + @"') 
                            AND (OutboundMails.Body = '" + body + @"') 
                            AND (OutboundMails.MailPriority = " + mailPriority + @") 
                            AND (OutboundMails.MailType = " + mailType + @") 
                            AND (OutboundMails.OrganizationId = " + organizationId + @") 
                            AND (OutboundMails.GeographyId = " + geographyId + @") 
                            AND (OutboundMails.CreatedDateTime > TO_DATE('" +
                        createDateTime.ToString ("yyyy-MM-dd HH:mm:ss") + @"'))", connection);

                command.CommandType = CommandType.Text;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (ReadOutboundMailFromDataReader (reader));
                    }

                    return result.ToArray();
                }
            }
        }
    }
}