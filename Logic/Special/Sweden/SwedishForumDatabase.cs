using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Activizr.Logic.Pirates;
using Activizr.Logic.Support;
using Activizr.Basic.Diagnostics;
using Activizr.Basic.Enums;
using MySql.Data.MySqlClient;
using System.Net;

namespace Activizr.Logic.Special.Sweden
{

    public interface IForumDatabase
    {
        int CreateNewPost (int forumId, Person poster, string title, string description, string post);
        int GetAccountId (string nick);
        int[] GetAccountList ();
        string GetAccountName (int accountId);
        int GetPollIdFromTopicId (int topicId);
        System.Collections.Generic.Dictionary<string, People> GetPollVotes (int pollId);
        bool IsPartyMember (int accountId);
        void SetPartyMember (int accountId);
        void SetPartyNonmember (int accountId);
        bool TestMode { get; set; }
    }

    public class SwedishForumDatabase
    {
        public static IForumDatabase GetDatabase (int type, string connectionstring)
        {
            if (type == 1)
                return SwedishForumDatabaseInstantForum.GetDatabase(connectionstring);
            else
                return SwedishForumDatabaseVBulletin.GetDatabase(connectionstring);
        }

        public static IForumDatabase GetDatabase (int type)
        {
            if (type == 1)
                return SwedishForumDatabaseInstantForum.GetDatabase();
            else
                return SwedishForumDatabaseVBulletin.GetDatabase();
        }

        public static IForumDatabase GetDatabase ()
        {
            if (Path.DirectorySeparatorChar == '/')
            {
                if (string.IsNullOrEmpty(Persistence.Key["SwedishForumDatabaseVBulletin_ConnectionString_Bot"]))
                    return GetDatabase(1);
                else
                    return GetDatabase(2);
            }
            else
            {
                if (string.IsNullOrEmpty(Persistence.Key["SwedishForumDatabaseVBulletin_ConnectionString_Web"]))
                    return GetDatabase(1);
                else
                    return GetDatabase(2);
            }

        }
    }

    public class SwedishForumDatabaseInstantForum : IForumDatabase
    {
        const int ForumIdTestPost = 326;
        private int forceForumId = 0;

        #region Connectionstring definitions
        // config file used by GetDatabase()
        private const string AppConfigFile = @"./database.config";

        // The default values used by GetDatabase()
        private const string DefaultAppDataSource = @"..\..\Site\DevDataBase\PirateWeb-DevDatabase.mdb";

        private const string DefaultConnectionString =
            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=%DataSource%;User Id=admin;Password=;";

        private const string DefaultProviderName = "System.Data.OleDb";
        private const string DefaultWebDataSource = @"~/DevDatabase/PirateWeb-DevDatabase.mdb";
        private const string MonoConfigFile = @"./database.config";
        private const string WebConfigFile = @"~/Database.config";

        // The cached values used by GetDatabase()
        private static string CachedConnectionString;
        private static string CachedProviderName;
        private readonly string connectString = string.Empty;
        #endregion

        public SwedishForumDatabaseInstantForum (string connectString)
        {
            this.connectString = connectString;
        }

        public static IForumDatabase GetDatabase (string connectionstring)
        {
            return new SwedishForumDatabaseInstantForum(CachedConnectionString);
        }

        public static IForumDatabase GetDatabase ()
        {
            // First, check if a previous invocation has put anything in the cashe.
            string ConnectionString = CachedConnectionString;
            string ProviderName = CachedProviderName;

            // During a cacheless invokation, the app/web config has priority.
            if (ConnectionString == null && ConfigurationManager.ConnectionStrings["PirateWeb"] != null)
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["PirateWeb"].ConnectionString;
                ProviderName = ConfigurationManager.ConnectionStrings["PirateWeb"].ProviderName;

                Logging.LogInformation(LogSource.PirateDb,
                                       "PirateDb initialized from Config ConnectionString: [" + ConnectionString +
                                       "] / [" + ProviderName + "]");
            }

            // If the app/web config is empty, check the database config file on disk
            if (ConnectionString == null)
            {
                try
                {
                    if (Path.DirectorySeparatorChar == '/')
                    {
                        // We are running under mono

                        using (var reader = new StreamReader(MonoConfigFile))
                        {
                            ConnectionString = reader.ReadLine();
                            ProviderName = reader.ReadLine();

                            Logging.LogInformation(LogSource.PirateDb,
                                                   "PirateDb initialized for Linux: [" + ConnectionString + "] / [" +
                                                   ProviderName + "]");
                        }
                    }
                    else if (HttpContext.Current != null)
                    {
                        // We are running a web application
                        using (var reader = new StreamReader(HttpContext.Current.Server.MapPath(WebConfigFile)))
                        {
                            ConnectionString = reader.ReadLine();
                            ProviderName = reader.ReadLine();

                            Logging.LogInformation(LogSource.PirateDb,
                                                   "PirateDb initialized for web: [" + ConnectionString + "] / [" +
                                                   ProviderName + "]");
                        }
                    }
                    else
                    {
                        // We are running an application, presumably directly from Visual Studio.
                        // If so, the current working directory is "PirateWeb/30/Console/bin".
                        using (var reader = new StreamReader(AppConfigFile))
                        {
                            ConnectionString = reader.ReadLine();
                            ProviderName = reader.ReadLine();

                            Logging.LogInformation(LogSource.PirateDb,
                                                   "PirateDb initialized for application: [" + ConnectionString +
                                                   "] / [" + ProviderName + "]");
                        }
                    }
                }
                catch (Exception)
                {
                    Logging.LogWarning(LogSource.PirateDb, "Unable to read Database.Config - defaulting");
                    // Ignore if we can't read the Database.config
                }

                // To simplify future checks
                if (ConnectionString != null && ConnectionString.Length == 0)
                {
                    ConnectionString = null;
                }

                // For backwards compability with one line MS Sql database config file
                // To be removed when Rick feels like it.
                if (ConnectionString != null && (ProviderName == null || ProviderName.Length == 0))
                {
                    ProviderName = "System.Data.SqlClient";
                }
            }

            // If we still have nothing, use the hardcodedd default
            if (ConnectionString == null)
            {
                string DataSource = DefaultAppDataSource;
                if (HttpContext.Current != null)
                {
                    DataSource = HttpContext.Current.Server.MapPath(DefaultWebDataSource);
                }
                ConnectionString = DefaultConnectionString.Replace("%DataSource%", DataSource);
                ProviderName = DefaultProviderName;
            }

            ConnectionString = ConnectionString.Replace("database=pirateweb", "database=pirateforums");

            // Now write the correct data to the cache, for faster lookup next time.
            if (CachedConnectionString == null)
            {
                CachedConnectionString = ConnectionString;
                CachedProviderName = ProviderName;
            }

            return new SwedishForumDatabaseInstantForum(CachedConnectionString);
        }

        public int GetAccountId (string nick)
        {
            if (nick == null)
            {
                throw new ArgumentNullException("nick");
            }

            using (var connection = new SqlConnection(this.connectString))
            {
                connection.Open();

                var command =
                    new SqlCommand(
                        String.Format("Select UserId from InstantASP_Users Where Username='{0}'",
                                      nick.Replace("'", "''")), connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    else
                    {
                        throw new ArgumentException("No such nick in database");
                    }
                }
            }
        }

        public string GetAccountName (int accountId)
        {
            using (var connection = new SqlConnection(this.connectString))
            {
                connection.Open();

                var command =
                    new SqlCommand(String.Format("Select Username from InstantASP_Users Where UserId=" + accountId),
                                   connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                    else
                    {
                        throw new ArgumentException("No such nick in database");
                    }
                }
            }
        }

        public int[] GetAccountList ()
        {
            var result = new List<int>();

            using (var connection = new SqlConnection(this.connectString))
            {
                connection.Open();

                var command = new SqlCommand(String.Format("Select UserId from InstantASP_Users"), connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetInt32(0));
                    }

                    return result.ToArray();
                }
            }
        }

        #region Party Membership
        public bool IsPartyMember (int accountId)
        {
            using (var connection = new SqlConnection(this.connectString))
            {
                connection.Open();

                var command =
                    new SqlCommand(
                        String.Format("Select PrimaryRoleID from InstantASP_Users where UserID=" + accountId),
                        connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int roleId = reader.GetInt16(0);
                        if (roleId == 7)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    throw new Exception("Account Id does not exist");
                }
            }
        }


        public void SetPartyMember (int accountId)
        {
            using (var connection = new SqlConnection(this.connectString))
            {
                connection.Open();

                var command = new SqlCommand("PirateSetMember", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@accountId", accountId);
                command.ExecuteNonQuery();
            }
        }


        public void SetPartyNonmember (int accountId)
        {
            using (var connection = new SqlConnection(this.connectString))
            {
                connection.Open();

                var command = new SqlCommand("PirateSetNonmember", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@accountId", accountId);
                command.ExecuteNonQuery();
            }
        }


        #endregion

        #region Polls

        public int GetPollIdFromTopicId (int topicId)
        {
            using (var connection = new SqlConnection(this.connectString))
            {
                connection.Open();

                var command =
                    new SqlCommand(String.Format("Select PollID from InstantForum_Polls where PostID=" + topicId),
                                   connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }

                    throw new ArgumentOutOfRangeException();
                }
            }
        }


        public Dictionary<string, People> GetPollVotes (int pollId)
        {
            // First, let's get the list of alternatives.

            Dictionary<int, string> pollAlternatives = GetPollAlternatives(pollId);

            // Construct a comma-separated list of the answer IDs.

            var altStringBuilder = new StringBuilder();

            foreach (int pollAlternativeId in pollAlternatives.Keys)
            {
                altStringBuilder.Append("," + pollAlternativeId);
            }

            string altString = altStringBuilder.ToString().Substring(1);

            var result = new Dictionary<string, People>();

            using (var connection = new SqlConnection(this.connectString))
            {
                connection.Open();

                var command =
                    new SqlCommand(
                        String.Format(
                            "Select PollAnswerID,UserID from InstantForum_PollVotes WHERE PollAnswerId IN (" + altString +
                            ")"), connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int voteCount = 0;

                    while (reader.Read())
                    {
                        voteCount++;
                        int forumUserId = reader.GetInt32(1);

                        People people = People.FromOptionalData(ObjectOptionalDataType.ForumAccountId,
                                                                forumUserId.ToString());
                        if (people.Count > 1)
                        {
                            Person[] array = people.ToArray();
                            foreach (Person testPerson in array)
                            {
                                if (testPerson.GetMemberships().Count == 0)
                                {
                                    people.Remove(testPerson);
                                }
                            }

                            if (people.Count > 1)
                            {
                                throw new Exception("This is not possible -- check userid " + forumUserId +
                                                    ", appears to be tied to several personIds");
                            }
                        }

                        Person person = null;

                        if (people.Count > 0)
                        {
                            person = people[0];
                        }

                        int pollAnswerId = reader.GetInt32(0);
                        string pollAnswer = pollAlternatives[pollAnswerId];

                        if (!result.ContainsKey(pollAnswer))
                        {
                            result[pollAnswer] = new People();
                        }

                        result[pollAnswer].Add(person);
                    }

                    return result;
                }
            }
        }


        private Dictionary<int, string> GetPollAlternatives (int pollId)
        {
            var result = new Dictionary<int, string>();

            using (var connection = new SqlConnection(this.connectString))
            {
                connection.Open();

                var command =
                    new SqlCommand(
                        String.Format("Select PollAnswerID,AnswerText from InstantForum_PollAnswers WHERE PollId=" +
                                      pollId), connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result[reader.GetInt32(0)] = reader.GetString(1);
                    }

                    return result;
                }
            }
        }


        #endregion

        #region Create Post

        public int CreateNewPost (int forumId, Person poster, string title, string description, string post)
        {
            // Returns the new post id
            if (forceForumId != 0) forumId = forceForumId;

            using (var connection = new SqlConnection(this.connectString))
            {
                connection.Open();

                // First, insert the post itself

                var command = new SqlCommand("if_sp_InsertPost", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@intForumID", forumId);
                command.Parameters.AddWithValue("@intTopicID", 0);
                command.Parameters.AddWithValue("@intParentID", 0);
                command.Parameters.AddWithValue("@intUserID", poster.SwedishForumAccountId);
                command.Parameters.AddWithValue("@strMessageIcon", string.Empty);
                command.Parameters.AddWithValue("@strTitle", title);
                command.Parameters.AddWithValue("@strDescription", description);
                command.Parameters.AddWithValue("@bitForumModerated", false);
                command.Parameters.AddWithValue("@bitIsPoll", false);
                command.Parameters.AddWithValue("@strIPAddress", "127.0.0.1");

                var parameterText = new SqlParameter("@strMessage", SqlDbType.NText, post.Length * 2 + 2048);
                parameterText.Value = post;

                command.Parameters.Add(parameterText);

                var parameterResult = new SqlParameter("@intIdentity", SqlDbType.Int, 4);
                parameterResult.Direction = ParameterDirection.Output;

                command.Parameters.Add(parameterResult);

                command.ExecuteNonQuery();

                var postId = (int)parameterResult.Value;

                // Then, update the LastRead for this forum

                command = new SqlCommand("if_sp_UpdateForumLastPostInformation", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@intForumID", forumId));
                command.Parameters.Add(new SqlParameter("@intLastPosterPostID", postId));
                command.Parameters.Add(new SqlParameter("@intLastPosterUserID", poster.SwedishForumAccountId));
                command.Parameters.Add(new SqlParameter("@strLastPosterUsername", poster.Name));
                command.Parameters.Add(new SqlParameter("@strLastPosterSubject", title));
                command.Parameters.Add(new SqlParameter("@dtLastPosterDate", DateTime.Now));

                command.ExecuteNonQuery();

                return postId;
            }
        }
        #endregion

        public bool TestMode
        {
            get
            {
                return forceForumId != 0;
            }
            set
            {
                if (value)
                    forceForumId = ForumIdTestPost;
                else
                    forceForumId = 0;
            }
        }

    }

    public class ErrorIgnorerPolicy : ICertificatePolicy
    {

        public bool CheckValidationResult(ServicePoint sp, X509Certificate certificate, WebRequest request, int error)
        {
            if (error == 0)
                return true;
            // only ask for trust failure (you may want to handle more cases)
            if (error != -2146762486)
                return false;

            return true;
        }
    }

    public class SwedishForumDatabaseVBulletin : IForumDatabase
    {
        const int ForumIdTestPost = 262;
        private int forceForumId = 0;
        // The cached values used by GetDatabase()
        private static string CachedConnectionString;
        private readonly string connectString = string.Empty;

        private readonly static int partymembergroupid = 16;
        private readonly static int nonmembergroupid = 2;

        public SwedishForumDatabaseVBulletin (string connectString)
        {
            this.connectString = connectString;
        }

        public static IForumDatabase GetDatabase (string connectionstring)
        {
            return new SwedishForumDatabaseVBulletin(connectionstring);
        }

        public static IForumDatabase GetDatabase ()
        {
            // First, check if a previous invocation has put anything in the cashe.
            string ConnectionString = CachedConnectionString;

            if (ConnectionString == null)
            {
                try
                {
                    if (Path.DirectorySeparatorChar == '/')
                    {
                        ConnectionString = Persistence.Key["SwedishForumDatabaseVBulletin_ConnectionString_Bot"];
                    }
                    else
                    {
                        ConnectionString = Persistence.Key["SwedishForumDatabaseVBulletin_ConnectionString_Web"];
                    }
                }
                catch (Exception)
                {
                    Logging.LogWarning(LogSource.PirateDb, "Unable to read Forum Connectionstring");
                }

                // To simplify future checks
                if (ConnectionString != null && ConnectionString.Length == 0)
                {
                    ConnectionString = null;
                }

            }

            // Now write the correct data to the cache, for faster lookup next time.
            if (CachedConnectionString == null)
            {
                CachedConnectionString = ConnectionString;
            }

            return new SwedishForumDatabaseVBulletin(CachedConnectionString);
        }


        public int GetAccountId (string nick)
        {
            if (nick == null)
            {
                throw new ArgumentNullException("nick");
            }

            using (var connection = new MySqlConnection(this.connectString))
            {
                connection.Open();

                var command =
                    new MySqlCommand(
                        String.Format("Select userid from vb_user Where username='{0}'",
                                      nick.Replace("'", "''")), connection);
                command.CommandTimeout=20;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    else
                    {
                        throw new ArgumentException("No such nick in database");
                    }
                }
            }
        }


        public string GetAccountName (int accountId)
        {
            using (var connection = new MySqlConnection(this.connectString))
            {
                connection.Open();

                var command =
                    new MySqlCommand(String.Format("Select username from vb_user Where userid=" + accountId),
                                   connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                    else
                    {
                        throw new ArgumentException("No such nick in database");
                    }
                }
            }
        }

        public int[] GetAccountList ()
        {
            var result = new List<int>();

            using (var connection = new MySqlConnection(this.connectString))
            {
                connection.Open();

                var command = new MySqlCommand(String.Format("Select userid from vb_user"), connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetInt32(0));
                    }

                    return result.ToArray();
                }
            }
        }


        #region Party Membership
        public bool IsPartyMember (int accountId)
        {
            using (var connection = new MySqlConnection(this.connectString))
            {
                connection.Open();

                var command =
                    new MySqlCommand(
                        String.Format("Select membergroupids from vb_user where userid=" + accountId),
                        connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string allGroups = ",";

                        if (!reader.IsDBNull(0))
                            allGroups += "" + reader.GetString(0).ToString() + ",";

                        allGroups = allGroups.Replace(" ", "");

                        if (allGroups.Contains("," + partymembergroupid + ","))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    throw new Exception("Account Id does not exist");
                }
            }
        }


        public void SetPartyMember (int accountId)
        {
            using (var connection = new MySqlConnection(this.connectString))
            {
                connection.Open();
                //TODO: Convert back to stored procedure
                //var command = new MySqlCommand("PirateSetMember", connection);
                //command.CommandType = CommandType.StoredProcedure;

                //command.Parameters.AddWithValue("@accountId", accountId);
                //command.ExecuteNonQuery();

                string allGroups = ",";

                var command = new MySqlCommand(
                        String.Format("Select membergroupids from vb_user where userid=" + accountId),
                        connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            allGroups += "" + reader.GetString(0).ToString() + ",";
                    }
                }

                if (!allGroups.Contains("," + partymembergroupid + ","))
                {
                    allGroups = (allGroups
                        .Replace(" ", "")
                        .Replace(",,", ",")
                        .Trim(',') + "," + partymembergroupid).Trim(',');

                    command = new MySqlCommand(String.Format("Update vb_user set  membergroupids='{0}', displaygroupid={1} where userid={2}",
                                                            allGroups, partymembergroupid, accountId),
                                                connection);

                    command.ExecuteNonQuery();

                }
            }
        }


        public void SetPartyNonmember (int accountId)
        {
            using (var connection = new MySqlConnection(this.connectString))
            {
                connection.Open();

                //TODO: Convert back to stored procedure
                //var command = new MySqlCommand("PirateSetNonmember", connection);
                //command.CommandType = CommandType.StoredProcedure;

                //command.Parameters.AddWithValue("@accountId", accountId);
                //command.ExecuteNonQuery();

                string allGroups = ",";

                var command = new MySqlCommand(
                        String.Format("Select membergroupids from vb_user where userid=" + accountId),
                        connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            allGroups += "" + reader.GetString(0).ToString() + ",";
                    }
                }

                if (allGroups.Contains("," + partymembergroupid + ","))
                {

                    allGroups = allGroups
                        .Replace(" ", "")
                        .Replace("," + partymembergroupid + ",", ",")
                        .Replace(",,", ",").Trim(',');

                    command = new MySqlCommand(String.Format("Update vb_user set membergroupids='{0}',displaygroupid={1} where userid={2}",
                                                            allGroups, nonmembergroupid, accountId
                                                            ),
                                                connection);

                    command.ExecuteNonQuery();

                }
            }
        }

        #endregion

        #region Get Polls

        public int GetPollIdFromTopicId (int threadId)
        {
            using (var connection = new MySqlConnection(this.connectString))
            {
                connection.Open();

                var command = new MySqlCommand(
                        String.Format("Select pollid from vb_thread where threadid=" + threadId),
                        connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }

                    throw new ArgumentOutOfRangeException();
                }
            }
        }


        public Dictionary<string, People> GetPollVotes (int pollId)
        {

            //// First, let's get the list of alternatives.

            Dictionary<int, string> pollAlternatives = GetPollAlternatives(pollId);

            var result = new Dictionary<string, People>();
            using (var connection = new MySqlConnection(this.connectString))
            {
                connection.Open();

                var command = new MySqlCommand(
                        String.Format("Select userid, voteoption from pw_pollvote where pollid=" + pollId),
                        connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    int voteCount = 0;

                    while (reader.Read())
                    {
                        voteCount++;
                        int forumUserId = reader.GetInt32(0);

                        People people = People.FromOptionalData(ObjectOptionalDataType.ForumAccountId,
                                                                forumUserId.ToString());
                        if (people.Count > 1)
                        {
                            Person[] array = people.ToArray();
                            foreach (Person testPerson in array)
                            {
                                if (testPerson.GetMemberships().Count == 0)
                                {
                                    people.Remove(testPerson);
                                }
                            }

                            if (people.Count > 1)
                            {
                                string ids = "";
                                foreach (Person p in people)
                                    ids += " " + p.Identity;
                                throw new Exception("This is not possible -- check userid " + forumUserId +
                                                    ", appears to be tied to several personIds:"+ids);
                            }
                        }

                        Person person = null;

                        if (people.Count > 0)
                        {
                            person = people[0];
                        }

                        int pollAnswerId = reader.GetInt32(1);
                        pollAnswerId--;  //One based
                        if (!pollAlternatives.ContainsKey(pollAnswerId))
                            pollAlternatives[pollAnswerId] = "" + pollAnswerId;

                        string pollAnswer = pollAlternatives[pollAnswerId];

                        if (!result.ContainsKey(pollAnswer))
                        {
                            result[pollAnswer] = new People();
                        }

                        result[pollAnswer].Add(person);
                    }

                    return result;
                }
            }
        }


        private Dictionary<int, string> GetPollAlternatives (int pollId)
        {
            var result = new Dictionary<int, string>();

            //Förslag A |||Förslag B |||Förslag C |||Förslag D |||Förslag E |||Avstår
            using (var connection = new MySqlConnection(this.connectString))
            {
                connection.Open();

                var command = new MySqlCommand(
                        String.Format("Select options from pw_poll where pollid=" + pollId),
                        connection);
                string options = "";
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        options = reader.GetString(0);
                    }
                    else

                        throw new ArgumentOutOfRangeException();
                }
                string[] results = options.Replace("|||", "\x12").Split('\x12');
                for (int i = 0; i < results.Length; ++i)
                    result[i] = results[i].Trim();
            }
            return result;
        }

        #endregion

        #region Create Post
        private string UrlEncode (string toEncode)
        {
            return HttpUtility.UrlEncode(toEncode, Encoding.GetEncoding("iso-8859-1"));
        }

        public static bool Validator (object sender, X509Certificate certificate, X509Chain chain, 
                                      SslPolicyErrors sslPolicyErrors)
	    {
		    return true;
	    }
		

        public int CreateNewPost (int forumId, Person poster, string title, string description, string post)
        {
            // call to vBulletin plugin to add post
            //key = special value secret key / selecting function to perform
            //forumid = id:t för forumet, tex 49
            //userid = användarid:t, tex 3
            //title = trådtiteln, tex Hej
            //pagetext = texten i inlägget, <strong>html</strong> och [b]bb-kod[/b] fungerar

            if (forceForumId != 0)
                forumId = forceForumId;

            string postTopicKey = Persistence.Key["ForumPostTopicKey"];
            string personAccountId = "" + poster.SwedishForumAccountId;
            string url = "http://vBulletin.piratpartiet.se/";

            try
            {
                ServicePointManager.CertificatePolicy = new ErrorIgnorerPolicy();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                //string proxy = null;
                Encoding recieverEncoding = Encoding.GetEncoding("iso-8859-1");

                string data = String.Format("key={0}&forumid={1}&userid={2}&title={3}&pagetext={4}",
                                            HttpUtility.UrlEncode(postTopicKey, recieverEncoding), 
                                            HttpUtility.UrlEncode("" + forumId, recieverEncoding), 
                                            HttpUtility.UrlEncode("" + personAccountId, recieverEncoding),
                                            HttpUtility.UrlEncode(title + ": " + description, recieverEncoding), 
                                            HttpUtility.UrlEncode(post, recieverEncoding));

                byte[] buffer = recieverEncoding.GetBytes(data);//Forum was set up for Latin-1

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = buffer.Length;

                //req.Proxy = new WebProxy(proxy, true); // ignore for local addresses

                //req.CookieContainer = new CookieContainer(); // enable cookies

                req.ServicePoint.Expect100Continue = false; //Disable stupid feature

                req.Headers.Add("Pragma", "no-cache");

                Stream reqst = req.GetRequestStream(); // add form data to request stream

                reqst.Write(buffer, 0, buffer.Length);
                reqst.Flush();
                reqst.Close();


                HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                Stream resst = res.GetResponseStream();
                StreamReader sr = new StreamReader(resst);
                string response = sr.ReadToEnd();


                int postId = 0;
                if (int.TryParse(response, out postId))
                {
                    return postId;
                }
                else
                {
                    throw new Exception("Error when calling CreateNewPost:" + response);
                }
            }
            catch (WebException ex)
            {
                throw new Exception("WebException when calling CreateNewPost:" + ex.Message);

            }
        }

        #endregion



        public bool TestMode
        {
            get
            {
                return forceForumId != 0;
            }
            set
            {
                if (value)
                    forceForumId = ForumIdTestPost;
                else
                    forceForumId = 0;
            }
        }


    }

}
