using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Common.Interfaces;

namespace Swarmops.Logic.Support
{
    internal class LogicServices
    {
        /// <summary>
        ///     Determines if a string can be parsed as an Int32.
        /// </summary>
        /// <param name="input">String to test.</param>
        /// <returns>True if safely parsable by Int32.Parse.</returns>
        internal static bool IsNumber (string input)
        {
            try
            {
                Int32.Parse (input);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Removes any non-digits from a string.
        /// </summary>
        /// <param name="input">The string to clean.</param>
        /// <returns>The sanitized result.</returns>
        /// <remarks>This means that an input string of "abc153ûû" would come back as "153".</remarks>
        public static string CleanNumber (string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            foreach (char acter in input)
            {
                if (acter >= '0' && acter <= '9')
                {
                    result.Append (acter);
                }
            }

            return result.ToString();
        }

        internal static int[] ObjectsToIdentifiers (IHasIdentity[] identifiables)
        {
            List<int> result = new List<int>();
            result.Capacity = identifiables.Length*11/10;

            foreach (IHasIdentity identifiable in identifiables)
            {
                result.Add (identifiable.Identity);
            }

            return result.ToArray();
        }
    }
}