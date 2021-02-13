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

        [Obsolete ("Do not use. Hardwireto MySQL.")]
        public SwarmDb (DbProviderFactory ProviderFactory, string ConnectionString)
        {
            this.ProviderFactory = ProviderFactory;
            this.ConnectionString = ConnectionString;
        }

        public SwarmDb (string connectionString)
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

                string dbVersionString = GetDatabaseForReading().GetKeyValue ("DbVersion");

                if (string.IsNullOrEmpty (dbVersionString))
                {
                    GetDatabaseForWriting().SetKeyValue ("DbVersion", "1");
                    return 1;
                }

                return Int32.Parse (dbVersionString);
            }
        }

        public static SwarmDb GetDatabaseForReading()
        {
            return new SwarmDb (ConstructConnectString (new Configuration().Get().Read));
        }

        public static SwarmDb GetDatabaseForWriting()
        {
            return new SwarmDb (ConstructConnectString (new Configuration().Get().Write));
        }

        public static SwarmDb GetDatabaseForAdmin()
        {
            return new SwarmDb (ConstructConnectString (new Configuration().Get().Admin));
        }

        public static SwarmDb GetTestDatabase (Credentials credentials)
        {
            // For security reasons, this function is only available BEFORE the database has been initialized.

            if (Configuration.IsConfigured())
            {
                throw new UnauthorizedAccessException (
                    "Cannot probe arbitrary database credentials once database initialized");
            }

            return new SwarmDb (ConstructConnectString (credentials));
        }

        private static string ConstructConnectString (Credentials credentials)
        {
            return "server=" + credentials.ServerSet.ServerPriorities[0].Split (';')[0] + ";database=" +
                   credentials.Database +
                   ";uid=" + credentials.Username + ";pwd=" + credentials.Password + ";SslMode=Preferred;";
        }


        [Obsolete ("Do not use. Use SwarmDb.GetDatabaseForReading(), ...ForWriting() or ...ForAdmin().", true)]
        public static SwarmDb GetDatabase()
        {
            throw new NotImplementedException ("GetDatabase() is obsolete and has been deleted.");
        }

        [Obsolete ("Do not use. Use GetDatabaseForAdmin().", true)]
        public static SwarmDb GetDatabaseAsAdmin()
        {
            throw new InvalidOperationException();
        }
    }
}