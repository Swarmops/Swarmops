using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Security;

namespace Swarmops.Logic.Support
{
    public static class Formatting
    {
        private static string _swarmopsVersion;
        private static DateTime? _swarmopsBuildDateTime = null;

        public static string SwarmopsVersion
        {
            get
            {
                if (string.IsNullOrEmpty (_swarmopsVersion))
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        _swarmopsVersion = "Debug Environment";
                        return _swarmopsVersion;
                    }
                    // Read build number if not loaded, or set to "Private" if none
                    string buildIdentity = (string)GuidCache.Get("_buildIdentity");

                    if (buildIdentity == null)
                    {
                        try
                        {
                            if (HttpContext.Current != null) // frontend: use friendly format
                            {
                                using (
                                    StreamReader reader =
                                        File.OpenText (HttpContext.Current.Request.MapPath ("~/BuildIdentity.txt")))
                                {
                                    buildIdentity = "Build " + reader.ReadLine();
                                }

                                using (
                                    StreamReader reader =
                                        File.OpenText (HttpContext.Current.Request.MapPath ("~/SprintName.txt")))
                                {
                                    buildIdentity += " (" + reader.ReadLine() + ")";
                                }
                            }
                            else // backend: use build ID only
                            {
                                using (
                                    StreamReader reader =
                                        File.OpenText("/usr/share/swarmops/backend/BuildIdentity.txt"))
                                {
                                    buildIdentity = reader.ReadLine();
                                }
                            }
                        }
                        catch (Exception)
                        {
                            buildIdentity = "Private Build";
                        }

                        GuidCache.Set("_buildIdentity", buildIdentity);
                        _swarmopsVersion = buildIdentity;
                    }
                }

                return _swarmopsVersion;
            }
        }

        public static DateTime SwarmopsBuildTime
        {
            get
            {
                if (_swarmopsBuildDateTime != null)
                {
                    return (DateTime) _swarmopsBuildDateTime;
                }

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    _swarmopsBuildDateTime = DateTime.Now;
                    GuidCache.Set("_swarmopsBuildDateTime", _swarmopsBuildDateTime);
                    return (DateTime) _swarmopsBuildDateTime;
                }

                // if we still don't have it, check the cache and possibly return

                _swarmopsBuildDateTime = (DateTime?) GuidCache.Get("_swarmopsBuildDateTime");

                if (_swarmopsBuildDateTime != null)
                {
                    return (DateTime) _swarmopsBuildDateTime;
                }

                try
                {
                    if (HttpContext.Current != null) // frontend: use friendly format
                    {
                        using (
                            StreamReader reader =
                                File.OpenText(HttpContext.Current.Request.MapPath("~/BuildDateTime.txt")))
                        {
                            _swarmopsBuildDateTime = DateTime.Parse(reader.ReadLine());
                        }
                    }
                    else // backend: use build ID only
                    {
                        using (
                            StreamReader reader =
                                File.OpenText("/usr/share/swarmops/backend/BuildDateTime.txt"))
                        {
                            _swarmopsBuildDateTime = DateTime.Parse(reader.ReadLine());
                        }
                    }
                }
                catch (Exception)
                {
                    _swarmopsBuildDateTime = DateTime.Now;
                }

                GuidCache.Set("_swarmopsBuildDateTime", _swarmopsBuildDateTime);
                return (DateTime) _swarmopsBuildDateTime;
            }
        }

        public static string CleanNumber (string input)
        {
            return LogicServices.CleanNumber (input);
        }


        public enum ParsingScale
        {
            Unknown = 0,
            Cents,
            Metacents
        }


        public static Int64 ParseDoubleStringAsCents (string input, CultureInfo culture = null, ParsingScale scale = ParsingScale.Cents)
        {
            // Parses first according to culture, and if fails, according to neutral culture
            // Throws ArgumentException if fails

            double outputHolder;
            bool success = false;
            input = input.Trim();

            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }

            // If current culture has decimal point, but decimal comma is entered, allow that

            if (culture.NumberFormat.CurrencyDecimalSeparator == ".")
            {
                // If decimal comma in decimal point position, replace with decimal point

                if (input.Length > 3 && input[input.Length - 3] == ',')
                {
                    input = input.Substring(0, input.Length - 3) + "." + input.Substring(input.Length - 2, 2);
                }
            }


            // Try current or provided culture first

            success = Double.TryParse(input,
                NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                culture, out outputHolder);

            if (success)
            {
                return DoubleRoundToCents(outputHolder, scale);
            }

            // Try invariant (US) culture as fallback

            success = Double.TryParse(input,
                NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out outputHolder);

            if (success)
            {
                return DoubleRoundToCents(outputHolder, scale);
            }

            // No joy, could not convert, so throw

            throw new ArgumentException("Could not parse input as cents: " + input);
        }

        private static Int64 DoubleRoundToCents(double input, ParsingScale scale)
        {
            double epsilonLimit = 0.004;
            double scalingFactor = 100.0;

            if (scale == ParsingScale.Metacents)  // factor 10,000 instead of 100
            {
                epsilonLimit /= 100.0;
                scalingFactor *= 100.0;
            }

            // Epsilon and true-zero values
            if (input > -epsilonLimit && input < epsilonLimit)
            {
                return 0;
            }

            // Positive values
            if (input > 0.0)
            {
               return Convert.ToInt64(input * scalingFactor + 0.2);   // +0.2 avoids rounding errors from epsilon losses of precision
            }

            // Negative values

            return Convert.ToInt64(input * scalingFactor - 0.2);  // -0.2 avoids rounding errors from epsilon losses of precision
        }

        public static string GeneratePassword (int length)
        {
            return Authentication.CreateWeakSecret (length);
        }


        public static bool ValidateEmailFormat (string email)
        {
            string gTLD = "aero|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|"
                          + "mobi|museum|name|net|org|pro|tel|travel";
            string ccTLD = "ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|"
                           + "ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|"
                           + "ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cu|cv|cx|cy|cz|"
                           + "de|dj|dk|dm|do|dz|"
                           + "ec|ee|eg|er|es|et|eu|"
                           + "fi|fj|fk|fm|fo|fr|"
                           + "ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|"
                           + "hk|hm|hn|hr|ht|hu|"
                           + "id|ie|il|im|in|io|iq|ir|is|it|"
                           + "je|jm|jo|jp|"
                           + "ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|"
                           + "la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|"
                           + "ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|"
                           + "na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|"
                           + "pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|"
                           + "re|ro|rs|ru|rw|"
                           + "sa|sb|sc|sd|se|sg|sh|si|sj|sk|sl|sm|sn|so|sr|st|su|sv|sy|sz|"
                           + "tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|"
                           + "ua|ug|uk|us|uy|uz|"
                           + "va|vc|ve|vg|vi|vn|vu|"
                           + "wf|ws|ye|yt|yu|za|zm|zw";
            Regex re =
                new Regex (
                    @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:" +
                    ccTLD + "|" + gTLD + ")$", RegexOptions.IgnoreCase);
            return re.IsMatch (email);
        }


        public static string GenerateRangeString (List<int> ids)
        {
            return GenerateRangeString (ids.ToArray());
        }


        public static string GenerateRangeString (int[] ids)
        {
            if (ids.Length == 0)
            {
                return string.Empty;
            }

            int indexOfLastDiscontinuity = 0;
            string result = string.Empty;

            // Go through the (assumed sorted) list of ids, and look for continuities and discontinuities.
            // A list of {1,2,3,5,6,8} becomes "#1-3, #5, #6, #8".

            for (int index = 1; index <= ids.Length; index++)
            {
                if (index == ids.Length || ids[index] != ids[index - 1] + 1)
                {
                    // We have a break in the continuity.

                    if (indexOfLastDiscontinuity == index - 1)
                    {
                        // The last sequence was just a single number.

                        result += ", #" + ids[index - 1].ToString("N0");
                    }
                    else if (indexOfLastDiscontinuity == index - 2)
                    {
                        // Two numbers in last sequence. Treat as discontinuous.
                        result += ", #" + ids[indexOfLastDiscontinuity].ToString("N0") + ", #" + ids[index - 1].ToString("N0");
                    }
                    else
                    {
                        // Contiguity.
                        result += ", #" + ids[indexOfLastDiscontinuity].ToString("N0") + "-" + ids[index - 1].ToString("N0");
                    }

                    indexOfLastDiscontinuity = index;
                }
            }

            return result.Substring (2);
        }

        public static string AddLuhnChecksum (string number)
        {
            return number + GetLuhnChecksum (number);
        }

        public static string GetLuhnChecksum (string number)
        {
            int sum = 0;
            for (int i = 0; i < number.Length; i++)
            {
                int temp = (number[i] - '0') * (((number.Length - i) % 2) == 1 ? 2 : 1);
                if (temp > 9) temp -= 9;
                sum += temp;
            }

            return ((10 - (sum % 10)) % 10).ToString(CultureInfo.InvariantCulture);
        }

        public static bool CheckLuhnChecksum (string number)
        {
            number = CleanNumber (number);

            if (number.Length < 2)
            {
                return false; // requires at least data and checksum
            }

            if (!number.EndsWith (GetLuhnChecksum (number.Substring (0, number.Length - 1))))
            {
                return false;
            }

            return true;
        }


        public static string ReverseString (string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse (charArray);
            return new string (charArray);
        }

        public static string JoinIdentities (List<int> ids)
        {
            return JoinIdentities (ids.ToArray());
        }


        public static string JoinIdentities (int[] ids)
        {
            if (ids.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            builder.Append (ids[0].ToString());

            for (int index = 1; index < ids.Length; index++)
            {
                builder.Append (",");
                builder.Append (ids[index].ToString());
            }

            return builder.ToString();
        }

        public static string[] SupportedCultures
        {
            get
            {
                return new string[] { "ar-AE", "de-DE", "yo-NG" /* Yoruba writes as Èbè Yoruba */, "el-GR" /* Greek writes as Ellenika */ , "en-US", "es-ES", "es-VE", "fr-FR", "fil-PH", "it-IT", "nl-NL", "pl-PL", "pt-PT", "ru-RU", "sr-Cyrl-RS", "sr-Latn-RS", "sv-SE", "tr-TR", "zh-CN" };

                // the above locales are sorted by the language NATIVE name, to make the list maximally useful
            }
        }


    }
}