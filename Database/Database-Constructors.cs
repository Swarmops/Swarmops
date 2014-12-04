using System;
using System.Data.Common;
using System.IO;
using System.Web;
using Swarmops.Basic.Diagnostics;

namespace Swarmops.Database
{
    /// <summary>
    ///     A generic database handler
    /// </summary>
    public partial class SwarmDb
    {
        private const string AppConfigFile = @"database.config";
        private const string WebConfigFile = @"~/database.config";
        private const string MonoConfigFile = @"/etc/swarmops/database.config";

        private const string DefaultProviderName = "MySql.Data.MySqlClient";

        // The cached values used by GetDatabase()
        private static string _cachedConnectionString;
        private readonly string ConnectionString;
        private readonly DbProviderFactory ProviderFactory;

        [Obsolete("Do not use. Hardwireto MySQL.")]
        public SwarmDb(DbProviderFactory ProviderFactory, string ConnectionString)
        {
            this.ProviderFactory = ProviderFactory;
            this.ConnectionString = ConnectionString;
        }

        public SwarmDb(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        ///     The actual revision of the database (as told by the database).
        /// </summary>
        public static int DbVersion
        {
            get
            {
                // TODO: Cache for some ten minutes, perhaps?

                string dbVersionString = GetDatabaseForReading().GetKeyValue("DbVersion");

                if (string.IsNullOrEmpty(dbVersionString))
                {
                    GetDatabaseForWriting().SetKeyValue("DbVersion", "1");
                    return 1;
                }

                return Int32.Parse(dbVersionString);
            }
        }

        public static SwarmDb GetDatabaseForReading()
        {
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

        public static SwarmDb GetTestDatabase(Credentials credentials)
        {
            // For security reasons, this function is only available BEFORE the database has been initialized.

            if (Configuration.IsConfigured())
            {
                throw new UnauthorizedAccessException(
                    "Cannot probe arbitrary database credentials once database initialized");
            }

            return new SwarmDb(ConstructConnectString(credentials));
        }

        private static string ConstructConnectString(Credentials credentials)
        {
            return "server=" + credentials.ServerSet.ServerPriorities[0].Split(';')[0] + ";database=" +
                   credentials.Database +
                   ";user=" + credentials.Username + ";password=" + credentials.Password;
        }


        [Obsolete("Do not use. Use SwarmDb.GetDatabaseForReading(), ...ForWriting() or ...ForAdmin().", true)]
        public static SwarmDb GetDatabase()
        {
            throw new NotImplementedException("GetDatabase() is obsolete and has been deleted.");
        }

        [Obsolete("Do not use. Use GetDatabaseForAdmin().", true)]
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
                    using (
                        StreamReader reader =
                            new StreamReader(
                                HttpContext.Current.Server.MapPath(WebConfigFile.Replace(".config", "-admin.config")))
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

                    connectionString = "server=peregrine;database=" + hostName + ";user=" + hostName +
                                       "-admin;password=" + hostName + "-admin";
                        // TODO: Replace "peregrine" with "localhost"
                }
                else
                {
                    throw new InvalidOperationException(
                        "No database-as-admin connection string found -- write a connect string into the \"ActivizrAdminConnect\" environment var, or on one line into a file named database-admin.config; see connectionstrings.com for examples");
                        // TODO: Replace with custom exception to present config screen
                }
            }

            // Now write the correct data to the cache, for faster lookup next time
            if (_cachedConnectionString == null)
            {
                _cachedConnectionString = connectionString;
            }

            return new SwarmDb(DbProviderFactories.GetFactory(DefaultProviderName), connectionString);
        }
    }
}