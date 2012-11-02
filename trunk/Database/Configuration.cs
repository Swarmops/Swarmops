using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activizr.Database
{
    public class Configuration
    {
        public bool IsConfigured()
        {
            return String.IsNullOrEmpty(DatabaseConnect.Default.Admin);
        }

        public bool SetConfiguration (string readConnect, string writeConnect, string adminConnect)
        {
            DatabaseConnect.Default["Read"] = readConnect;
            DatabaseConnect.Default["Write"] = writeConnect;
            DatabaseConnect.Default["Admin"] = adminConnect;
            DatabaseConnect.Default.Save();

            // TODO: Test a connect string to see if we can read, and return true only on success

            return true;
        }
    }
}
