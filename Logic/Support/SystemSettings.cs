using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

// This class is just for readability of other code. Doesn't add any logic at all, really - just readability, like SystemSettings.SmtpHost instead of Persistence.blahblah.
using System.Web;
using NBitcoin;
using Swarmops.Logic.Security;

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

        static public string BackendHostnameOverride
        {
            get { return Persistence.Key["BackendHostnameOverride"]; }
            set { Persistence.Key["BackendHostnameOverride"] = value; }
        }


        static public string BackendHostname
        {
            get { return Persistence.Key["BackendHostname"]; }
            set { Persistence.Key["BackendHostname"] = value; }
        }

        static public int WebsocketPortFrontend
        {
            get { return Int32.Parse(GetDefaultedPersistedKey("WebsocketPortFrontend", "12172")); }
            set { Persistence.Key["WebsocketPortFrontend"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        static public int WebsocketPortBackend
        {
            get { return Int32.Parse(GetDefaultedPersistedKey("WebsocketPortBackend", "10944")); }
            set { Persistence.Key["WebsocketPortBackend"] = value.ToString(CultureInfo.InvariantCulture); }
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


        public static byte[] SymmetricEncryptionKeyCombined
        {
            get
            {
                byte[] dbKey = SymmetricEncryptionKeyDatabase;
                byte[] fsKey = SymmetricEncryptionKeyFileSystem;

                byte[] result = new byte[dbKey.Length];

                for (int index = 0; index < dbKey.Length; index++)
                {
                    result[index] = (byte) (dbKey[index] ^ fsKey[index]); // combine the two keys through xor
                }

                return result;
            }
        }


        static public byte[] SymmetricEncryptionKeyDatabase
        {
            get
            {
                if (_cacheDatabaseSymmetricKey != null)
                {
                    return _cacheDatabaseSymmetricKey;
                }

                string keyString = Persistence.Key["SymmetricEncryptionKey"];
                if (string.IsNullOrEmpty (keyString))
                {
                    _cacheDatabaseSymmetricKey = Authentication.InitializeSymmetricDatabaseKey(); // Creates a new key - remove this by Alpha-12; should be in Init // TODO
                }
                else
                {
                    _cacheDatabaseSymmetricKey = Convert.FromBase64String (keyString);
                }

                return _cacheDatabaseSymmetricKey;
            }
        }

        private static byte[] _cacheDatabaseSymmetricKey = null;
        private static byte[] _cacheFileSystemSymmetricKey = null;

        static public byte[] SymmetricEncryptionKeyFileSystem
        {
            get {
                if (_cacheFileSystemSymmetricKey != null)
                {
                    return _cacheFileSystemSymmetricKey;
                }

                if (Debugger.IsAttached && Path.DirectorySeparatorChar != '/')
                {
                    // If we're debugging, return a zero key - can't read from /etc/swarmops in debug code

                    _cacheFileSystemSymmetricKey = new byte[32];  // return 256 bits all zero
                }
                else
                {
                    string keyString = File.ReadAllText ("/etc/swarmops/symmetricKey.config", Encoding.ASCII);
                    if (keyString.StartsWith ("UNINIT"))
                    {
                        _cacheFileSystemSymmetricKey = Authentication.InitializeSymmetricFileSystemKey();
                    }
                    else
                    {
                        _cacheFileSystemSymmetricKey = Convert.FromBase64String(keyString);
                    }
                }

                return _cacheFileSystemSymmetricKey;
            }
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


        public static string BlockchainSwarmopsApiKey
        {
            get { return "77e11665-14a9-4d72-9111-7dce7a6748f6"; }
        }


        public static UInt64 HeartbeatBackend
        {
            get
            {
                string result = Persistence.Key["HeartbeatBackend"];
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        return UInt64.Parse(result);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }

                return 0;
            }
            set
            {
                Persistence.Key["HeartbeatBackend"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }



        public static UInt64 HeartbeatFrontend
        {
            get
            {
                string result = Persistence.Key["HeartbeatFrontend"];
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        return UInt64.Parse(result);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }

                return 0;
            }
            set
            {
                Persistence.Key["HeartbeatFrontend"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }


        public static bool DatabaseInitialized
        {
            get { return !String.IsNullOrEmpty(InstallationId); }
        }


        public static string EtcFolder
        {
            get
            {
                if (Debugger.IsAttached && Path.DirectorySeparatorChar == '\\')
                {
                    // Windows environment - dev environment

                    if (HttpContext.Current != null)
                    {
                        // If in HTTP context, return the root frontend folder

                        return HttpContext.Current.Server.MapPath ("~");
                    }
                    else
                    {
                        // Not in HTTP context - debugging backend or something not considered at time of writing this

                        return "."; // current directory (execution directory)
                    }
                }

                return "/etc/swarmops"; // live etc folder
            }
        }

        public static bool RequireSsl
        {
            get { return Boolean.Parse(GetDefaultedPersistedKey("SslRequired", "true")); }  // default to true for legacy installs
            set { Persistence.Key["SslRequired"] = value.ToString(); }
        }
    }
}
