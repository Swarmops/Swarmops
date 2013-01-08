using System.Collections.Generic;
using Swarmops.Database;

namespace Swarmops.Logic.Support
{
    public class Persistence
    {
        // Usage: Persistence.Key ["Foobar"] = "42"; Console.WriteLine (Persistence.Key ["Barcode"]);
        // Bad class name and location, refactor later

        public string this [string key]
        {
            get { return PirateDb.GetDatabaseForReading().GetKeyValue (key); }

            set { PirateDb.GetDatabaseForWriting().SetKeyValue (key, value); }
        }

        public static Persistence Key
        {
            get { return new Persistence(); }
        }

        /* -- Obsolete, used only once in migration MSSQL => MySQL
        public static Dictionary<string,string> All
        {
            get { return PirateDb.GetDatabaseForReading().GetOldKeyValues(); }  // Migration-only function -- returns all key/value pairs
        }*/
    }
}