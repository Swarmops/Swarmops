using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.IO;
using System.Web;
using Activizr.Basic.Diagnostics;

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

                        using (StreamReader reader = new StreamReader(MonoConfigFile))
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
                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(WebConfigFile))
                            )
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
                        using (StreamReader reader = new StreamReader(AppConfigFile))
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

            // Now write the correct data to the cache, for faster lookup next time.
            if (CachedConnectionString == null)
            {
                CachedConnectionString = ConnectionString;
                CachedProviderName = ProviderName;
            }

            return new PirateDb(DbProviderFactories.GetFactory(ProviderName), ConnectionString);
        }

        public PirateDb (DbProviderFactory ProviderFactory, string ConnectionString)
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

        // The default values used by GetDatabase()
        private const string DefaultAppDataSource = @"..\..\Site\DevDataBase\PirateWeb-DevDatabase.mdb";
        private const string DefaultWebDataSource = @"~/DevDatabase/PirateWeb-DevDatabase.mdb";

        private const string DefaultConnectionString =
            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=%DataSource%;User Id=admin;Password=;";

        private const string DefaultProviderName = "System.Data.OleDb";

        // The cached values used by GetDatabase()
        private static string CachedConnectionString = null;
        private static string CachedProviderName = null;
    }
}