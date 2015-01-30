using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This class is just for readability of other code. Doesn't add any logic at all, really - just readability, like SystemSettings.SmtpHost instead of Persistence.blahblah.

namespace Swarmops.Logic.Support
{
    public class SystemSettings
    {
        static public string SmtpHost
        {
            get
            {
                return GetDefaultedPersistedKey("SmtpHost", "localhost");
            }
            set
            {
                Persistence.Key["SmtpHost"] = value;
            }
        }

        static public int SmtpPort
        {
            get
            {
                string smtpPortString = Persistence.Key["SmtpPort"];
                int smtpPort = 25;

                if (!string.IsNullOrEmpty(smtpPortString))
                {
                    smtpPort = Int32.Parse(smtpPortString); // bad place to get out of if this throws
                }

                return smtpPort;
            }
            set
            {
                Persistence.Key["SmtpPort"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        static public string SmtpUser
        {
            get
            {
                return Persistence.Key["SmtpUser"];
            }
            set
            {
                Persistence.Key["SmtpUser"] = value;
            }
        }

        static public string SmtpPassword
        {
            get
            {
                return Persistence.Key["SmtpPassword"];
            }
            set
            {
                Persistence.Key["SmtpPassword"] = value;
            }
        }

        static public string ExternalUrl
        {
            get
            {
                return Persistence.Key["ExternalUrl"];
            }
            set
            {
                Persistence.Key["ExternalUrl"] = value;
            }
        }


        static public string AdminNotificationSender
        {
            get { return GetDefaultedPersistedKey ("AdminMailFrom", "Swarmops Admin"); }
            set { Persistence.Key["AdminMailFrom"] = value; }
        }

        static public string AdminNotificationAddress
        {
            get { return GetDefaultedPersistedKey ("AdminMailAddress", "swarmops-admin@example.com"); }
            set { Persistence.Key["AdminMailAddress"] = value; }
        }


        static public string InstallationId
        {
            get { return Persistence.Key["SwarmopsInstallationId"]; } // no setter - intentional
        }

        static public string InstallationName
        {
            get { return GetDefaultedPersistedKey ("InstallationName", "Swarmops"); }
            set { Persistence.Key["InstallationName"] = value;  }
        }

        static private string GetDefaultedPersistedKey (string key, string defaultValue)
        {
            string result = Persistence.Key[key];
            if (string.IsNullOrEmpty(result))
            {
                return defaultValue;
            }
            return result;
        }
    }
}
