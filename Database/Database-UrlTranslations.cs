using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public int CreateUrlTranslation (string url)
        {
            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("CreateUrlTranslation", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "originalUrl", url);

                int urlTranslationId = Convert.ToInt32 (command.ExecuteScalar());

                return urlTranslationId;
            }
        }


        public void SetUrlTranslation (string originalUrl, string translatedUrl)
        {
            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand ("SetUrlTranslation", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName (command, "originalUrl", originalUrl);
                AddParameterWithName (command, "translatedUrl", translatedUrl);

                command.ExecuteNonQuery();
            }
        }


        public string[] GetUntranslatedUrls (int max)
        {
            List<string> result = new List<string>();

            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT TOP " + max + " OriginalUrl FROM UrlTranslations WHERE Translated=0",
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add (reader.GetString (0));
                    }

                    return result.ToArray();
                }
            }
        }


        public string GetUrlTranslation (string url)
        {
            using (DbConnection connection = GetSqlServerDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand (
                        "SELECT TranslatedUrl FROM UrlTranslations WHERE OriginalUrl='" + url.Replace ("'", "''") +
                        "' AND Translated=1", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString (0);
                    }

                    return null;
                }
            }
        }
    }
}