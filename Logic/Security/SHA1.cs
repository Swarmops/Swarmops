using System.Security.Cryptography;
using System.Text;

namespace Swarmops.Logic.Security
{
    /// <summary>
    ///     Summary description for SHA1.
    /// </summary>
    public class SHA1
    {
        public static string Hash(string input)
        {
            byte[] data = Encoding.GetEncoding(1252).GetBytes(input);

            System.Security.Cryptography.SHA1 SHA1 = new SHA1CryptoServiceProvider();
            byte[] hash = SHA1.ComputeHash(data);

            // Write the resulting hash to a string of hex values.

            StringBuilder result = new StringBuilder();

            foreach (byte oneByte in hash)
            {
                result.Append(oneByte.ToString("X02") + " ");
            }

            return result.ToString().TrimEnd();
        }
    }
}