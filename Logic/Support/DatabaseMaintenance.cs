using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Swarmops.Logic.Swarm;
using Swarmops.Database;
using System.Globalization;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;

namespace Swarmops.Logic.Support
{
    public class DatabaseMaintenance
    {
        /// <summary>
        /// Feeds a newly-created database with an empty structure. Do not use on populated databases; it's like a Genesis Device.
        /// </summary>
        static public void FirstInitialization()
        {
            // Security check: make sure we throw an exception if we try to get person 1.

            bool initialized = false;
            Person personOne = null;
            string sql = string.Empty;

            try
            {
                personOne = Person.FromIdentity(1);
                initialized = true;
            }
            catch (Exception)
            {
                // Do nothing, the exception is expected
            }

            if (initialized == true && personOne != null) // double checks in case Database behavior should change to return null rather than throw later
            {
                throw new InvalidOperationException("Cannot run Genesis Device style initalization on already-populated database");
            }

            using (WebClient client = new WebClient())
            {
                sql = client.DownloadString("http://packages.swarmops.com/schemata/initialize.sql");  // Hardcoded as security measure - don't want to pass arbitrary sql in from web layer
            }

            string[] sqlCommands = sql.Split('#');  // in the file, the commands are split by a single # sign. (Semicolons are an integral part of storedprocs, so they can't be used.)

            foreach (string sqlCommand in sqlCommands)
            {
                SwarmDb.GetDatabaseForAdmin().ExecuteAdminCommand(sqlCommand.Trim());
            }

            SwarmDb.GetDatabaseForWriting().SetKeyValue("DbVersion", "1"); // We're at baseline
        }

        static public void UpgradeSchemata()
        {
            int currentDbVersion = Int32.Parse(SwarmDb.GetDatabaseForReading().GetKeyValue("DbVersion"));
            int expectedDbVersion = SwarmDb.DbVersion;
            string sql;
            bool upgraded = false;

            while (currentDbVersion < expectedDbVersion)
            {
                currentDbVersion++;

                string fileName = String.Format("http://packages.swarmops.com/schemata/upgrade-{0:D4}.sql", currentDbVersion);

                using (WebClient client = new WebClient())
                {
                    sql = client.DownloadString(fileName);
                }

                string[] sqlCommands = sql.Split('#');  // in the file, the commands are split by a single # sign. (Semicolons are an integral part of storedprocs, so they can't be used.)

                foreach (string sqlCommand in sqlCommands)
                {
                    SwarmDb.GetDatabaseForAdmin().ExecuteAdminCommand(sqlCommand.Trim());
                }

                upgraded = true;
                SwarmDb.GetDatabaseForWriting().SetKeyValue("DbVersion", currentDbVersion.ToString(CultureInfo.InvariantCulture)); // Increment after each successful run
            }

            if (upgraded)
            {
                OutboundComm.CreateNotification(null, NotificationResource.System_DatabaseSchemaUpgraded);
            }
        }
    }
}
