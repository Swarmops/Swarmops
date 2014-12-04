using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Swarmops.Basic.Types;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public BasicMailTemplate[] GetMailTemplatesByName(string templatename)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                templatename = templatename.Replace("'", "''");

                string sql = string.Format("SELECT * FROM MailTemplates WHERE (TemplateName = '{0}')", templatename);

                DbCommand command = GetDbCommand(sql, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        List<BasicMailTemplate> retlist = new List<BasicMailTemplate>();
                        retlist.Add(ReadMailTemplate(reader));
                        while (reader.Read())
                        {
                            retlist.Add(ReadMailTemplate(reader));
                        }
                        return retlist.ToArray();
                    }
                    throw new ArgumentException("No mailtemplate named: " + templatename + " exists.");
                }
            }
        }

        public BasicMailTemplate GetMailTemplateById(int templateId)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                string sql = "SELECT  * from MailTemplates where TemplateId={0}";

                sql = string.Format(sql, templateId);

                DbCommand command = GetDbCommand(sql, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadMailTemplate(reader);
                    }
                    throw new ArgumentException("No mailtemplate with id: " + templateId + " exists.");
                }
            }
        }

        public BasicMailTemplate[] GetAllMailTemplates()
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                string sql =
                    "SELECT * from MailTemplates order by templateName, CountryCode, LanguageCode, OrganizationId";

                DbCommand command = GetDbCommand(sql, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    List<BasicMailTemplate> retlist = new List<BasicMailTemplate>();
                    while (reader.Read())
                    {
                        retlist.Add(ReadMailTemplate(reader));
                    }
                    return retlist.ToArray();
                }
            }
        }

        private static BasicMailTemplate ReadMailTemplate(DbDataReader reader)
        {
            int templateId = (int) reader["templateId"];
            string templateName = reader["templateName"] != DBNull.Value ? (string) reader["templateName"] : "";
            string langCode = reader["languageCode"] != DBNull.Value ? (string) reader["languageCode"] : "";
            string crtyCode = reader["countryCode"] != DBNull.Value ? (string) reader["countryCode"] : "";
            int orgId = reader["organizationId"] != DBNull.Value ? (int) reader["organizationId"] : 0;
            string templateBody = reader["templateBody"] != DBNull.Value ? (string) reader["templateBody"] : "";
            return new BasicMailTemplate(templateId, templateName, langCode, crtyCode, orgId, templateBody);
        }

        public int SetMailTemplate(int templateId,
            string templateName,
            string languageCode,
            string countryCode,
            int organizationId,
            string templateBody)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SetMailTemplate", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "p_templateId", templateId);
                AddParameterWithName(command, "p_organizationId", organizationId);
                AddParameterWithName(command, "p_templateName", templateName);
                AddParameterWithName(command, "p_languageCode", languageCode);
                AddParameterWithName(command, "p_countryCode", countryCode);
                AddParameterWithName(command, "p_templateBody", templateBody);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        public int SetMailTemplate(BasicMailTemplate mailTemplate)
        {
            return SetMailTemplate(mailTemplate.TemplateId,
                mailTemplate.TemplateName,
                mailTemplate.LanguageCode,
                mailTemplate.CountryCode,
                mailTemplate.OrganizationId,
                mailTemplate.TemplateBody);
        }
    }
}