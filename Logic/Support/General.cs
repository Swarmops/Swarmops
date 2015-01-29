using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Support
{
    public class SupportFunctions
    {
        public static void DisableSslCertificateChecks()
        {
            // X.509 is SO SO SO broken. The reason we do this, btw, is that mono doesn't come with root certs. At all.

            // This disables certificate checking completely and accepts ALL certificates.

            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        public static string GenerateSecureRandomKey(int bytelength)
        {
            byte[] buffer = new byte[bytelength];
            RNGCryptoServiceProvider devRandom = new RNGCryptoServiceProvider();
            devRandom.GetBytes(buffer);

            StringBuilder result = new StringBuilder(bytelength * 2);
            for (int loop = 0; loop < buffer.Length; loop++)
            {
                result.AppendFormat("{0:X2}", buffer[loop]);
            }
            return result.ToString();
        }

    }
}
