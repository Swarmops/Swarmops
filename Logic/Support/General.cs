﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

        // ReSharper disable once InconsistentNaming  -- IPAddress is the canonical writing
        public static string GetRemoteIPAddressChain()
        {
            string forwardedIps =
                HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            string directIp = 
                HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

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
            // All of this can be faked trivially by the client, so you can't rely on it for any kind of security.

            string forwardedIps =
                HttpContext.Current.Request.Headers["X-Forwarded-For"];

            string directIp =
                HttpContext.Current.Request.UserHostAddress;

            if (!string.IsNullOrEmpty (forwardedIps))
            {
                return forwardedIps.Split (',').Last();
            }

            return directIp;
        }
    }
}
