using System;
using System.Globalization;
using System.Net;
using MySql.Data.MySqlClient;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public class DatabaseMaintenance
    {
        /// <summary>
        ///     Feeds a newly-created database with an empty structure. Do not use on populated databases; 
        ///     it's like a Genesis Device.
        /// </summary>
        public static void FirstInitialization()
        {
            // Security check: make sure we throw an exception if we try to get person 1.

            bool initialized = false;
            Person personOne = null;
            string sql = string.Empty;

            try
            {
                personOne = Person.FromIdentity (1);
                initialized = true;
            }
            catch (Exception)
            {
                // Do nothing, the exception is expected
            }

            if (initialized && personOne != null)
                // double checks in case Database behavior should change to return null rather than throw later
            {
                throw new InvalidOperationException (
                    "Cannot run Genesis Device style initalization on already-populated database");
            }

            using (WebClient client = new WebClient())
            {
                sql = client.DownloadString ("http://packages.swarmops.com/schemata/initialize.sql");
                // Hardcoded as security measure - don't want to pass arbitrary sql in from web layer
            }

            string[] sqlCommands = sql.Split ('#');
            // in the file, the commands are split by a single # sign. (Semicolons are an integral part of storedprocs, so they can't be used.)

            foreach (string sqlCommand in sqlCommands)
            {
                SwarmDb.GetDatabaseForAdmin().ExecuteAdminCommand (sqlCommand.Trim());
            }

            SwarmDb.GetDatabaseForWriting().SetKeyValue ("DbVersion", "1"); // We're at baseline
        }

        public static void UpgradeSchemata()
        {
            int currentDbVersion = SwarmDb.DbVersion;
            int expectedDbVersion = SwarmDb.DbVersionExpected;
            string sql;
            bool upgraded = false;

            if (currentDbVersion < expectedDbVersion)
            {
                Console.WriteLine("Swarmops: Current DB version is {0}, but expected is {1}. A schema upgrade will take place.", currentDbVersion, expectedDbVersion);
            }

            while (currentDbVersion < expectedDbVersion)
            {
                currentDbVersion++;

                Console.Write("Schema {0} diff: Fetching...", currentDbVersion);

                string fileName = String.Format ("http://packages.swarmops.com/schemata/upgrade-{0:D4}.sql",
                    currentDbVersion);

                try
                {
                    using (WebClient client = new WebClient())
                    {
                        sql = client.DownloadString (fileName);
                    }
                }
                catch (Exception outerException)
                {
                    Console.WriteLine(" trying fallback...");

                    // Because Mono installs with an insufficient certificate store, we must disable certificate checking before accessing github

                    SupportFunctions.DisableSslCertificateChecks();

                    fileName = String.Format("https://raw.githubusercontent.com/Swarmops/Swarmops/master/Database/Schemata/upgrade-{0:D4}.sql",
                        currentDbVersion);

                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            sql = client.DownloadString(fileName);
                        }
                    }
                    catch (Exception middleException)
                    {
                        try
                        {
                            OutboundComm.CreateNotification (null, NotificationResource.System_DatabaseUpgradeFailed);
                        }
                        catch (ArgumentException)
                        {
                            // if this happens during setup:

                            throw new Exception ("Failed fetching upgrade packages:\r\n" + middleException.ToString() +
                                                 "\r\n" + outerException.ToString());
                        }
                        Console.WriteLine(" FAILED! Aborting.");

                        return;
                    }
                }

                string[] sqlCommands = sql.Split ('#');
                // in the file, the commands are split by a single # sign. (Semicolons are an integral part of storedprocs, so they can't be used.)

                Console.Write(" applying...");

                foreach (string sqlCommand in sqlCommands)
                {
                    try
                    {
                        SwarmDb.GetDatabaseForAdmin().ExecuteAdminCommand (sqlCommand.Trim().TrimEnd (';'));  // removes whitespace first, then any ; at the end (if left in by mistake)
                    }
                    catch (MySqlException exception)
                    {
                        SwarmDb.GetDatabaseForWriting()
                            .CreateExceptionLogEntry (DateTime.UtcNow, "DatabaseUpgrade",
                                new Exception (string.Format ("Exception upgrading to Db{0:D4}", currentDbVersion),
                                    exception));

                        Console.Write(" EXCEPTION (see log)!");
                        // Continue processing after logging error.
                        // TODO: Throw and abort? Tricky decision
                    }
                }

                upgraded = true;
                SwarmDb.GetDatabaseForWriting()
                    .SetKeyValue ("DbVersion", currentDbVersion.ToString (CultureInfo.InvariantCulture));
                // Increment after each successful run

                Console.WriteLine(" done.");
            }

            if (upgraded)
            {
                Console.WriteLine("Swarmops database schema upgrade completed.\r\n");
                try
                {
                    OutboundComm.CreateNotification (null, NotificationResource.System_DatabaseSchemaUpgraded);
                }
                catch (ArgumentException)
                {
                    // this is ok - if we're in the install process, person 1 or other notification targets won't exist yet
                }
            }
        }
    }
}