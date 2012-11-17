using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Activizr.Database;
using Activizr.Logic.Pirates;

namespace Activizr.Logic.Support
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
                sql = client.DownloadString("http://packages.activizr.com/schemas/initialize.sql");  // Hardcoded as security measure - don't want to pass arbitrary sql in from web layer
            }

            string[] sqlCommands = sql.Split('#');  // in the file, the commands are split by a single # sign. (Semicolons are an integral part of storedprocs, so they can't be used.)

            foreach (string sqlCommand in sqlCommands)
            {
                PirateDb.GetDatabaseForAdmin().ExecuteAdminCommand(sqlCommand.Trim());
            }
        }
    }
}
