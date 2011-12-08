using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.IO;
using System.Web;
using Activizr.Basic.Diagnostics;
using MySql.Data;

namespace Activizr.Database
{
    /// <summary>
    /// A generic database handler
    /// </summary>
    public partial class PirateDb
    {
        public static PirateDb GetDatabase()
        {
            // First, check if a previous invocation has put anything in the cashe.
            string connectionString = _cachedConnectionString;

            // During a cacheless invocation, the app/web config has priority.
            if (connectionString == null && ConfigurationManager.ConnectionStrings["Activizr"] != null)
            {
                connectionString = ConfigurationManager.ConnectionStrings["Activizr"].ConnectionString;

                Logging.LogInformation(LogSource.PirateDb,
                                       "PirateDb initialized from Config ConnectionString: [" + connectionString +
                                       "]");
            }

            // If the app/web config is empty, check the database config file on disk
            if (connectionString == null)
            {
                try
                {
                    if (Path.DirectorySeparatorChar == '/' && HttpContext.Current == null)
                    {
                        // We are running under mono in a backend environment

                        using (StreamReader reader = new StreamReader(MonoConfigFile))
                        {
                            connectionString = reader.ReadLine();

                            Logging.LogInformation(LogSource.PirateDb,
                                                   "PirateDb initialized for Linux: [" + connectionString + "]");
                        }
                    }
                    else if (HttpContext.Current != null)
                    {
                        // We are running a web application, under Mono (production) or Windows (development)
                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(WebConfigFile))
                            )
                        {
                            connectionString = reader.ReadLine();

                            Logging.LogInformation(LogSource.PirateDb,
                                                   "PirateDb initialized for web: [" + connectionString + "]");
                        }

                    }
                    else
                    {
                        // We are running an application, presumably directly from Visual Studio.
                        // If so, the current working directory is "PirateWeb/30/Console/bin".
                        using (StreamReader reader = new StreamReader(AppConfigFile))
                        {
                            connectionString = reader.ReadLine();

                            Logging.LogInformation(LogSource.PirateDb,
                                                   "PirateDb initialized for application: [" + connectionString +
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
                if (HttpContext.Current != null)
                {
                    string hostName = HttpContext.Current.Request.Url.Host;

                    connectionString = "server=peregrine;database=" + hostName + ";user=" + hostName + ";pass=" + hostName;  // TODO: Replace "peregrine" with "localhost"
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

            PirateDb db = null;

            try
            {
                db = new PirateDb(DbProviderFactories.GetFactory(DefaultProviderName), connectionString);
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Unable to initalize PirateDb using connection string \"" + connectionString + "\"", e);    // TODO: REMOVE ON PRODUCTION -- CREDENTIALS LEAKAGE 
            }

            return db;
        }

        public static PirateDb GetDatabaseAsAdmin()
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
                                                "PirateDb initialized for Linux Backend: [" + connectionString + "]");
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
                                                "PirateDb initialized for web: [" + connectionString + "]");
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
                                                "PirateDb initialized for application: [" + connectionString +
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

                    connectionString = "server=peregrine;database=" + hostName + ";user=" + hostName + "-admin;pass=" + hostName +"-admin";  // TODO: Replace "peregrine" with "localhost"
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

            return new PirateDb(DbProviderFactories.GetFactory(DefaultProviderName), connectionString);
        }

        public PirateDb(DbProviderFactory ProviderFactory, string ConnectionString)
        {
            this.ProviderFactory = ProviderFactory;
            this.ConnectionString = ConnectionString;
        }

        // The instance connection details.
        private DbProviderFactory ProviderFactory;
        private string ConnectionString;

        // config file used by GetDatabase()
        private const string AppConfigFile = @"database.config";
        private const string WebConfigFile = @"~/database.config";
        private const string MonoConfigFile = @"./database.config";
        
        private const string DefaultProviderName = "MySql.Data.SqlClient";

        // The cached values used by GetDatabase()
        private static string _cachedConnectionString = null;
    }
}