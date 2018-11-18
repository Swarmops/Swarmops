using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Swarmops.Database;

namespace Swarmops.Logic.Support
{
    public class SupportFunctions
    {
        public static void DisableSslCertificateChecks()
        {
            // X.509 is SO SO SO broken. The reason we do this, btw, is that mono doesn't come with root certs. At all.

            // This disables certificate checking completely and accepts ALL certificates.

            // TODO: Does this install a new handler every time it's been called? How do you verify that this short-circuit is in place?

            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;

            // Further, instantiate an AesCryptoServiceProvider to make sure the linker doesn't optimize it away -- apparently
            // another known bug. The warning disabled here is "b is assigned a value that is never used".

            #pragma warning disable 0219
            System.Security.Cryptography.AesCryptoServiceProvider b = new System.Security.Cryptography.AesCryptoServiceProvider();
            #pragma warning restore 0219

        }

        public static string GenerateSecureRandomKey(int byteCount)
        {
            byte[] buffer = GenerateSecureRandomBytes(byteCount);

            StringBuilder result = new StringBuilder(byteCount * 2);
            for (int loop = 0; loop < buffer.Length; loop++)
            {
                result.AppendFormat("{0:X2}", buffer[loop]);
            }
            return result.ToString();
        }

        public static byte[] GenerateSecureRandomBytes(int byteCount)
        {
            byte[] buffer = new byte[byteCount];
            RNGCryptoServiceProvider devRandom = new RNGCryptoServiceProvider();
            devRandom.GetBytes(buffer);

            return buffer;
        }

        // ReSharper disable once InconsistentNaming  -- IPAddress is the canonical writing
        public static string GetRemoteIPAddressChain()
        {
            string forwardedIps =
                HttpContext.Current.Request.Headers ["X-Forwarded-For"];

            string directIp =
                HttpContext.Current.Request.UserHostAddress; // may be IPv4 or IPv6

            string result = directIp;

            if (!string.IsNullOrEmpty(forwardedIps))
            {
                result = forwardedIps.Split (',').Last().Trim();
                result += " (" + directIp + ", " + forwardedIps + ")";
            }

            return result;
        }

        // ReSharper disable once InconsistentNaming  -- IPAddress is the canonical writing
        public static string GetMostLikelyRemoteIPAddress()
        {
            // All of this can be faked trivially by the client, so we can't rely on it for any kind of security.
            // While it is possible to set up a server environment that strips out such fakes, doing so should not
            // be a requirement for running Swarmops.

            string cloudFlareClient = HttpContext.Current.Request.Headers["CF-Connecting-IP"];

            if (!string.IsNullOrEmpty (cloudFlareClient))
            {
                return cloudFlareClient;
            }

            string forwardedIps =
                HttpContext.Current.Request.Headers["X-Forwarded-For"];

            string directIp =
                HttpContext.Current.Request.UserHostAddress;

            if (!string.IsNullOrEmpty (forwardedIps))
            {
                return forwardedIps.Split (',').First();
            }

            return directIp;
        }


        public static int DatabaseSchemaVersion
        {
            get { return SwarmDb.DbVersion; }
        }

        public static bool DatabaseConfigured
        {
            get { return SwarmDb.Configuration.IsConfigured(); }
        }

        public static void LogException (string source, Exception exception)
        {
            SwarmDb.GetDatabaseForWriting().CreateExceptionLogEntry (DateTime.UtcNow, source, exception);
        }

        public static OperatingTopology OperatingTopology { get; set; }
    }

    public enum OperatingTopology
    {
        Unknown = 0,
        Backend,
        FrontendSocket,
        FrontendWeb
    }
}
