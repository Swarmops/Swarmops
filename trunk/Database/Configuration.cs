using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace Activizr.Database
{
    public partial class PirateDb
    {
        [Serializable]
        public class Configuration
        {
            public Configuration() {}
            public Configuration (Credentials read, Credentials write, Credentials admin)
            {
                this.Read = read;
                this.Write = write;
                this.Admin = admin;
            }

            public static bool IsConfigured()
            {
                if (Configuration._configuration == null)
                {
                    try
                    {
                        Configuration.Load();
                    }
                    catch(Exception)
                    {
                        return false;
                    }
                }

                string testString = _configuration.Admin.Username;

                return !String.IsNullOrEmpty(DatabaseConnect.Default.Admin);
            }

            public Credentials Read { get; set; }
            public Credentials Write { get; set; }
            public Credentials Admin { get; set; }

            private static Configuration _configuration;

            public static void Load()
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));

                using (FileStream readFileStream = new FileStream(GetConfigurationFileName(), FileMode.Open, FileAccess.Read, FileShare.Read))
                {

                    _configuration = (Configuration) serializer.Deserialize(readFileStream);
                }
            }

            public static void Set(Configuration configuration)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));

                using (TextWriter writeFileStream = new StreamWriter(GetConfigurationFileName()))
                {
                    serializer.Serialize(writeFileStream, configuration);
                }

                _configuration = configuration;
            }

            public static string GetConfigurationFileName()
            {
                // If we're running in a HTTP context, use the Mono path mapper, as we're in an Apache process.
                // Otherwise, use a relative path from the binary.

                if (HttpContext.Current != null)
                {
                    // Apache process, so use MapPath

                    return HttpContext.Current.Server.MapPath("~/database.config");
                }
                else
                {
                    // Backend process - just use the simple filename (and copy frontend's config to backend's on daemon start).

                    return "database.config";
                }
            }
        }

        [Serializable]
        public class ServerSet
        {
            public ServerSet()
            {
                // paramless ctor to enable serialization
                ServerPriorities = new List<string>();
            }

            public ServerSet (string singleServer)
            {
                ServerPriorities = new List<string>();
                ServerPriorities.Add(singleServer);
            }

            public List<string> ServerPriorities { get; set; } // array of semicolon-delimited hostnames; topmost string in array is first-priority servers, and so on.
        }

        [Serializable]
        public class Credentials
        {
            public Credentials()
            {
                // paramless ctor to enable serialization
            }

            public Credentials (string database, ServerSet servers, string username, string password)
            {
                this.Database = database;
                this.ServerSet = servers;
                this.Username = username;
                this.Password = password;
            }

            public string Database { get; set; }
            public ServerSet ServerSet { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
       }
    }
}
