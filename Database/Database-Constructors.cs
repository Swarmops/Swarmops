using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.IO;
using System.Web;
using MySql.Data;
using Swarmops.Basic.Diagnostics;

namespace Swarmops.Database
{
    /// <summary>
    /// A generic database handler
    /// </summary>
    public partial class SwarmDb
    {
        public static SwarmDb GetDatabaseForReading()
        {
            Console.WriteLine("In GetDatabaseForReading()");

            return new SwarmDb(ConstructConnectString(new Configuration().Get().Read));
        }

        public static SwarmDb GetDatabaseForWriting()
        {
            return new SwarmDb(ConstructConnectString(new Configuration().Get().Write));
        }

        public static SwarmDb GetDatabaseForAdmin()
        {
            return new SwarmDb(ConstructConnectString(new Configuration().Get().Admin));
        }

        private static string ConstructConnectString (Credentials credentials)
        {
            return "server=" + credentials.ServerSet.ServerPriorities[0].Split(';')[0] + ";database=" + credentials.Database + 
                ";user=" + credentials.Username + ";password=" + credentials.Password;
        }


        [Obsolete ("Do not use. Use SwarmDb.GetDatabaseForReading(), ...ForWriting() or ...ForAdmin().", true)]
        public static SwarmDb GetDatabase()
        {
            Console.WriteLine("Entering GetDatabase()");

            // First, check if a previous invocation has put anything in the cashe.
            string connectionString = _cachedConnectionString;

            // During a cacheless invocation, the app/web config has priority.
            if (connectionString == null && ConfigurationManager.ConnectionStrings["Activizr"] != null)
            {
                Console.WriteLine("Initing from ConfigManager");

                connectionString = ConfigurationManager.ConnectionStrings["Activizr"].ConnectionString;

                Logging.LogInformation(LogSource.PirateDb,
                                       "SwarmDb initialized from Config ConnectionString: [" + connectionString +
                                       "]");
            }

            // If the app/web config is empty, check the database config file on disk
            if (connectionString == null)
            {
                Console.WriteLine("Point 2");

                try
                {
                    if (Path.DirectorySeparatorChar == '/' && HttpContext.Current == null)
                    {
                        Console.WriteLine("Attempting to initialize using MonoConfigFile, which is " + MonoConfigFile);

                        // We are running under mono in a backend environment

                        using (StreamReader reader = new StreamReader(MonoConfigFile))
                        {
                            connectionString = reader.ReadLine();

                            Logging.LogInformation(LogSource.PirateDb,
                                                   "SwarmDb initialized for Linux: [" + connectionString + "]");
                        }
                    }
                    else if (HttpContext.Current != null)
                    {
                        Console.WriteLine("Point 3");

                        // We are running a web application, under Mono (production) or Windows (development)
                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(WebConfigFile))
                            )
                        {
                            connectionString = reader.ReadLine();

                            Logging.LogInformation(LogSource.PirateDb,
                                                   "SwarmDb initialized for web: [" + connectionString + "]");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Point 4");

                        // We are running an application, presumably directly from Visual Studio.
                        // If so, the current working directory is "PirateWeb/30/Console/bin".
                        using (StreamReader reader = new StreamReader(AppConfigFile))
                        {
                            connectionString = reader.ReadLine();

                            Logging.LogInformation(LogSource.PirateDb,
                                                   "SwarmDb initialized for application: [" + connectionString +
                                                   "]");
                        }
                    }
                }
                catch (Exception)
                {
                    Logging.LogWarning(LogSource.PirateDb, "Unable to read Database.Config - defaulting");
                    // Ignore if we can't read the Database.config
                }

                // To simplify future checks
                if (connectionString != null && connectionString.Length == 0)
                {
                    connectionString = null;
                }
            }

            // If we still have nothing, and we're running from web, then assume we have a dev environment and use the hostname as db, user, and pass.
            if (connectionString == null)
            {
                Console.WriteLine("Point 5");

                if (HttpContext.Current != null)
                {
                    string hostName = HttpContext.Current.Request.Url.Host;

                    connectionString = "server=peregrine;database=" + hostName + ";user=" + hostName + ";password=" + hostName;  // TODO: Replace "peregrine" with "localhost"
                }
                else
                {
                    throw new InvalidOperationException("No database connection string found -- write a connect string on one line into a file named database.config; see connectionstrings.com for examples");  // TODO: Replace with custom exception to present config screen
                }
            }

            // Now write the correct data to the cache, for faster lookup next time.
            if (_cachedConnectionString == null)
            {
                _cachedConnectionString = connectionString;
            }

            SwarmDb db = null;

            try
            {
                db = new SwarmDb(DbProviderFactories.GetFactory(DefaultProviderName), connectionString);
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Unable to initalize SwarmDb using connection string \"" + connectionString + "\"", e);    // TODO: REMOVE ON PRODUCTION -- CREDENTIALS LEAKAGE 
            }

            return db;
        }

        [Obsolete ("Do not use. Use GetDatabaseForAdmin().", true)]
        public static SwarmDb GetDatabaseAsAdmin()
        {
            string connectionString = string.Empty;

            try
            {
                if (Path.DirectorySeparatorChar == '/' && HttpContext.Current == null)
                {
                    // We are running under mono in a backend environment

                    using (StreamReader reader = new StreamReader(MonoConfigFile.Replace(".config", "-admin.config")))
                    {
                        connectionString = reader.ReadLine();

                        Logging.LogInformation(LogSource.PirateDb,
                                                "SwarmDb initialized for Linux Backend: [" + connectionString + "]");
                    }
                }
                else if (HttpContext.Current != null)
                {
                    // We are running a web application, under Mono (production) or Windows (development)
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(WebConfigFile.Replace(".config", "-admin.config")))
                        )
                    {
                        connectionString = reader.ReadLine();

                        Logging.LogInformation(LogSource.PirateDb,
                                                "SwarmDb initialized for web: [" + connectionString + "]");
                    }

                }
                else
                {
                    // We are running an application, presumably directly from Visual Studio.
                    // If so, the current working directory is "PirateWeb/30/Console/bin".
                    using (StreamReader reader = new StreamReader(AppConfigFile.Replace(".config", "-admin.config")))
                    {
                        connectionString = reader.ReadLine();

                        Logging.LogInformation(LogSource.PirateDb,
                                                "SwarmDb initialized for application: [" + connectionString +
                                                "]");
                    }
                }
            }
            catch (Exception)
            {
                Logging.LogWarning(LogSource.PirateDb, "Unable to read Database.Config - defaulting");
                // Ignore if we can't read the Database.config
            }

            // To simplify future checks
            if (connectionString != null && connectionString.Length == 0)
            {
                connectionString = null;
            }

            // If we still have nothing, and we're running from web, then assume we have a dev environment and use the hostname as db, user, and pass.
            if (String.IsNullOrEmpty(connectionString))
            {
                if (HttpContext.Current != null) // dummy comment to force build, remove on sight
                {
                    string hostName = HttpContext.Current.Request.Url.Host;

                    connectionString = "server=peregrine;database=" + hostName + ";user=" + hostName + "-admin;password=" + hostName +"-admin";  // TODO: Replace "peregrine" with "localhost"
                }
                else
                {
                    throw new InvalidOperationException("No database-as-admin connection string found -- write a connect string into the \"ActivizrAdminConnect\" environment var, or on one line into a file named database-admin.config; see connectionstrings.com for examples");  // TODO: Replace with custom exception to present config screen
                }
            }

            // Now write the correct data to the cache, for faster lookup next time
            if (_cachedConnectionString == null)
            {
                _cachedConnectionString = connectionString;
            }

            return new SwarmDb(DbProviderFactories.GetFactory(DefaultProviderName), connectionString);
        }

        [Obsolete ("Do not use. Hardwireto MySQL.")]
        public SwarmDb(DbProviderFactory ProviderFactory, string ConnectionString)
        {
            this.ProviderFactory = ProviderFactory;
            this.ConnectionString = ConnectionString;
        }

        public SwarmDb(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        // The instance connection details.
        private DbProviderFactory ProviderFactory;
        private string ConnectionString;

        // config file used by GetDatabase()
        private const string AppConfigFile = @"database.config";
        private const string WebConfigFile = @"~/database.config";
        private const string MonoConfigFile = @"/etc/swarmops/database.config";
        
        private const string DefaultProviderName = "MySql.Data.MySqlClient";

        // The cached values used by GetDatabase()
        private static string _cachedConnectionString = null;
    }
}