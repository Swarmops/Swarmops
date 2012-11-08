﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activizr.Database
{
    public partial class PirateDb
    {
        public class Configuration
        {
            public static bool IsConfigured()
            {
                string testString = DatabaseConnect.Default.Admin;

                return !String.IsNullOrEmpty(DatabaseConnect.Default.Admin);
            }

            public static bool SetConfiguration(string readConnect, string writeConnect, string adminConnect)
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
}
