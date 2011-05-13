using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Security;
using Activizr.Logic.Support;
using System.Text.RegularExpressions;

namespace Activizr.Logic.Support
{
    public static class Formatting
    {
        public static string CleanNumber (string input)
        {
            return LogicServices.CleanNumber(input);
        }


        public static string GeneratePassword (int length)
        {
            return Authentication.CreateRandomPassword(length);
        }


        public static bool ValidateEmailFormat (string email)
        {
            string gTLD = "aero|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|"
                        + "mobi|museum|name|net|org|pro|tel|travel";
            string ccTLD =  "ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|"
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
            Regex re = new Regex(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:" + ccTLD + "|" + gTLD + ")$", RegexOptions.IgnoreCase);
            return re.IsMatch(email);
        }


        public static string GenerateRangeString(List<int> ids)
        {
            return GenerateRangeString(ids.ToArray());
        }


        public static string GenerateRangeString(int[] ids)
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

                        result += ", #" + ids[index - 1].ToString();
                    }
                    else if (indexOfLastDiscontinuity == index - 2)
                    {
                        // Two numbers in last sequence. Treat as discontinuous.
                        result += ", #" + ids[indexOfLastDiscontinuity].ToString() + ", #" + ids[index - 1].ToString();
                    }
                    else
                    {
                        // Contiguity.
                        result += ", #" + ids[indexOfLastDiscontinuity].ToString() + "-" + ids[index - 1].ToString();
                    }

                    indexOfLastDiscontinuity = index;
                }
            }

            return result.Substring(2);
        }

        public static string AddLuhnChecksum (string number)
        {
            int sum = 0;
            for (int i = 0; i < number.Length; i++){
                int temp = (number[i] - '0') * (((number.Length - i) % 2) == 1 ? 2 : 1);
                if (temp > 9) temp -= 9;
                sum += temp;
            }

            return number + ((10 - (sum % 10)) % 10).ToString();

        }


        public static string ReverseString (string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse( charArray );
            return new string( charArray );
        }

        public static string JoinIdentities (List<int> ids)
        {
            return JoinIdentities(ids.ToArray());
        }


        public static string JoinIdentities (int[] ids)
        {
            if (ids.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(ids[0].ToString());

            for (int index = 1; index < ids.Length; index++)
            {
                builder.Append(",");
                builder.Append(ids[index].ToString());
            }

            return builder.ToString();
        }
    }
}