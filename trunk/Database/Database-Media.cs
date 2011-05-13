using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;

namespace Activizr.Database
{
    public partial class PirateDb
    {
        public int CreateMediaEntryFromKeyword (string keyword, string mediaName, bool isBlog, string url, string title,
                                                DateTime dateTime)
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();
                try
                {
                    DbCommand command = GetDbCommand("CreateMediaKeywordEntry", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameterWithName(command, "mediaKeyword", keyword);
                    AddParameterWithName(command, "mediaName", mediaName);
                    AddParameterWithName(command, "isBlog", isBlog);
                    AddParameterWithName(command, "mediaEntryTitle", title);
                    AddParameterWithName(command, "mediaEntryUrl", url);
                    AddParameterWithName(command, "mediaEntryDateTime", dateTime);

                    int mediaKeywordEntryId = Convert.ToInt32(command.ExecuteScalar());

                    return mediaKeywordEntryId;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed in sp CreateMediaKeywordEntry:" + e.Message, e);
                }
            }
        }


        public BasicMediaEntry[] GetBlogEntriesForKeyword (string keyword, DateTime minAge) // NOT ok
        {
            throw new NotImplementedException("Not yet migrated to MySQL: GetBlogEntriesForKeyword");

            return
                ExecuteMediaDbQuery("SELECT * FROM MediaEntryView WHERE MediaKeyword='" + keyword.Replace("'", "''") +
                                    "' AND MediaEntryDateTime > '" + minAge.ToString("yyyy-MM-dd HH:mm:ss") +
                                    "' AND IsBlog %ISTRUE% ORDER BY MediaEntryDateTime DESC");
        }

        public BasicMediaEntry[] GetOldMediaEntriesForKeyword (string keyword, DateTime minAge) // NOT ok
        {
            throw new NotImplementedException("Not yet migrated to MySQL: GetOldMediaEntriesForKeyword");


            return
                ExecuteMediaDbQuery("SELECT * FROM MediaEntryView WHERE MediaKeyword='" + keyword.Replace("'", "''") +
                                    "' AND MediaEntryDateTime > '" + minAge.ToString("yyyy-MM-dd HH:mm:ss") +
                                    "' AND IsBlog %ISFALSE% ORDER BY MediaEntryDateTime DESC");
        }

        private BasicMediaEntry[] ExecuteMediaDbQuery (string commandString)
        {
            List<BasicMediaEntry> result = new List<BasicMediaEntry>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand(commandString, connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadMediaEntryFromReader(reader));
                    }
                }
            }

            return result.ToArray();
        }


        public BasicMediaEntry[] GetMediaEntriesForKeywordIdsSimplified (int[] keywordIds) // migrated RF 2010-May-07
        {
            List<BasicMediaEntry> result = new List<BasicMediaEntry>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT MediaKeywordEntryId, MediaKeywordId, MediaId, MediaEntryDateTime FROM MediaKeywordEntries WHERE MediaKeywordId in (" +
                        JoinIds(keywordIds) + ") ORDER BY MediaEntryDateTime", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new BasicMediaEntry(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2).ToString(), false, "N/A", "N/A", reader.GetDateTime(3)));
                    }
                }
            }

            return result.ToArray();
        }


        private BasicMediaEntry ReadMediaEntryFromReader (DbDataReader reader) // NOT ok -- will need to migrate; any dependencies also won't work
        {
            int id = (int)reader["MediaKeywordEntryId"];
            int keywordId = (int)reader["MediaKeywordId"];
            string mediaName = (string)reader["MediaName"];
            bool isBlog = (bool)reader["IsBlog"];
            string mediaEntryTitle = (string)reader["MediaEntryTitle"];
            string mediaEntryUrl = (string)reader["MediaEntryUrl"];
            DateTime mediaEntryDateTime = (DateTime)reader["MediaEntryDateTime"];

            object translatedUrlObject = reader["TranslatedUrl"];

            if (!(translatedUrlObject is System.DBNull))
            {
                string translatedUrl = (string)translatedUrlObject;

                if (translatedUrl != null && translatedUrl.Trim().Length > 0)
                {
                    mediaEntryUrl = translatedUrl;
                }
            }

            return new BasicMediaEntry(id, keywordId, mediaName, isBlog, mediaEntryTitle, mediaEntryUrl,
                                       mediaEntryDateTime);
        }


        public string[] GetBlogKeywords () // OK
        {
            List<string> result = new List<string>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT MediaKeyword From MediaKeywords WHERE SearchBlogs=1",
                                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }
            return result.ToArray();
        }

        public string[] GetOldMediaKeywords ()   // OK
        {
            List<string> result = new List<string>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT MediaKeyword From MediaKeywords WHERE SearchOldMedia=1",
                                                 connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }
            return result.ToArray();
        }


        public Dictionary<int, bool> GetMediaTypeTable () // OK Todo: create enum instead of bool
        {
            Dictionary<int, bool> result = new Dictionary<int, bool>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT MediaId,IsBlog From Media", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result[reader.GetInt32(0)] = reader.GetBoolean(1);
                    }
                }
            }
            return result;
        }

        public int GetMediaKeywordId (string keyword) // OK
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT MediaKeywordId From MediaKeywords WHERE MediaKeyword='" + keyword.Replace("'", "''") +
                        "'", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }


        public Dictionary<int, string> GetMediaKeywordTable ()  // OK
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command = GetDbCommand("SELECT MediaKeywordId,MediaKeyword From MediaKeywords", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result[reader.GetInt32(0)] = reader.GetString(1);
                    }
                }
            }
            return result;
        }


        public BasicMediaCategory GetMediaCategory (int mediaCategoryId)  // OK
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT Name FROM MediaCategories WHERE MediaCategoryId=" + mediaCategoryId.ToString(),
                        connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BasicMediaCategory(mediaCategoryId, reader.GetString(0));
                    }

                    throw new ArgumentException("No such MediaCategoryId: " + mediaCategoryId.ToString());
                }
            }
        }


        public BasicMediaCategory GetMediaCategoryByName (string mediaCategoryName)  // OK
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT MediaCategoryId, Name FROM MediaCategories WHERE Name='" +
                        mediaCategoryName.Replace("'", "''") + "'", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BasicMediaCategory(reader.GetInt32(0), reader.GetString(1));
                    }

                    throw new ArgumentException("No such MediaCategory: " + mediaCategoryName);
                }
            }
        }


        public BasicMediaCategory[] GetMediaCategories (int[] mediaCategoryIds)
        {
            List<BasicMediaCategory> result = new List<BasicMediaCategory>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT MediaCategoryId, Name FROM MediaCategories WHERE MediaCategoryId in (" +
                        JoinIds(mediaCategoryIds) + ")", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new BasicMediaCategory(reader.GetInt32(0), reader.GetString(1)));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicMediaCategory[] GetMediaCategories ()
        {
            List<BasicMediaCategory> result = new List<BasicMediaCategory>();

            using (DbConnection connection = GetMySqlDbConnection())
            {
                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT MediaCategoryId, Name FROM MediaCategories WHERE MediaCategoryId", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new BasicMediaCategory(reader.GetInt32(0), reader.GetString(1)));
                    }

                    return result.ToArray();
                }
            }
        }


        public BasicMedium[] GetBlogTopList (DateTime date) // OK - migrated
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                int dateId = 0;

                connection.Open();

                DbCommand command =
                    GetDbCommand(
                        "SELECT BlogRankingDateId FROM BlogRankingDates WHERE Date='" + date.ToString("yyyy-MM-dd") +
                        "'", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        dateId = reader.GetInt32(0);
                    }
                    else
                    {
                        return null; // no list for this date. TODO: Throw exception instead?
                    }
                }

                List<BasicMedium> result = new List<BasicMedium>();

                command =
                    GetDbCommand(
                        "select BlogRankings.MediaId,Media.Name from BlogRankings JOIN Media USING (MediaId) WHERE BlogRankings.BlogRankingDateId=" +
                        dateId.ToString() + " ORDER BY Ranking", connection);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int mediaId = reader.GetInt32(0);
                        string mediaName = reader.GetString(1);
                        PoliticalAffiliation politicalAffiliation = PoliticalAffiliation.Unknown;

                        result.Add(new BasicMedium(mediaId, mediaName, PoliticalAffiliation.Unknown));
                    }

                    return result.ToArray();
                }
            }
        }

        public void StoreBlogTopList (DateTime date, string[] rankedBlogNames)  // OK - migrated
        {
            using (DbConnection connection = GetMySqlDbConnection())
            {
                int dateId = 0;
                connection.Open();

                DbCommand command = GetDbCommand("CreateBlogRankingDate", connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParameterWithName(command, "date", date);

                dateId = Convert.ToInt32(command.ExecuteScalar());

                for (int index = 0; index < rankedBlogNames.Length; index++)
                {
                    command = GetDbCommand("CreateBlogRankingEntry", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameterWithName(command, "blogRankingDateId", dateId);
                    AddParameterWithName(command, "mediaName", LimitStringLength(rankedBlogNames[index], 128));
                    AddParameterWithName(command, "ranking", index + 1);
                    command.ExecuteNonQuery();
                }
            }
        }


        private string LimitStringLength (string input, int maxLength)
        {
            if (input.Length > maxLength)
            {
                return input.Substring(0, maxLength - 3) + "...";
            }

            return input;
        }
    }
}