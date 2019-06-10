using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Security
{
    public class Sha256
    {
        public static string Compute(string input)
        {
            byte[] data = Encoding.GetEncoding(1252).GetBytes(input);

            System.Security.Cryptography.SHA256 Sha256 = new SHA256CryptoServiceProvider();
            byte[] hash = Sha256.ComputeHash(data);

            // Write the resulting hash to a string of hex values.

            StringBuilder result = new StringBuilder();

            foreach (byte oneByte in hash)
            {
                result.Append(oneByte.ToString("X02"));
            }

            return result.ToString().TrimEnd();
        }
    }
}
