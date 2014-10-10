using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Swarmops.Logic.Support
{

    /// <summary>
    /// Provide encoding and decoding of Quoted-Printable.
    /// </summary>
    public class QuotedPrintable
    {
        public QuotedPrintable (Encoding encoding)
        {
            _encoding = encoding;
        }

        Encoding _encoding = Encoding.Default;
        /// <summary>
        /// // so including the = connection, the length will be 76
        /// </summary>
        private const int RFC_1521_MAX_CHARS_PER_LINE = 75;

        /// <summary>
        /// Return quoted printable string with 76 characters per line.
        /// </summary>
        /// <param name="textToEncode"></param>
        /// <returns></returns>
        public  string Encode (string textToEncode)
        {
            if (textToEncode == null)
                throw new ArgumentNullException();

            return Encode(textToEncode, RFC_1521_MAX_CHARS_PER_LINE);
        }

        private  string Encode (string textToEncode, int charsPerLine)
        {
            if (textToEncode == null)
                throw new ArgumentNullException();

            if (charsPerLine <= 0)
                throw new ArgumentOutOfRangeException();

            return FormatEncodedString(EncodeString(textToEncode), charsPerLine);
        }

        /// <summary>
        /// Return quoted printable to be used in mail headers, if encoding is necessary
        /// </summary>
        /// <param name="textToEncode"></param>
        /// <returns></returns>
        public  String EncodeMailHeaderString (string textToEncode)
        {
            String encChars = EncodeString(textToEncode);
            String displayName = "";
            if (encChars != textToEncode)
            {
                displayName = "=?";
                displayName += _encoding.BodyName;
                displayName +="?Q?" + encChars.Replace(" ", "_") + "?=";
            }
            else
                displayName = textToEncode;
            return displayName;
        }

        /// <summary>
        /// Return quoted printable string, all in one line.
        /// </summary>
        /// <param name="textToEncode"></param>
        /// <returns></returns>
        public  string EncodeString (string textToEncode)
        {
            if (textToEncode == null)
                throw new ArgumentNullException();

            byte[] bytes = _encoding.GetBytes(textToEncode);
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                if (b != 0)
                    if ((b < 32) || (b > 126))
                        builder.Append(String.Format("={0}", b.ToString("X2")));
                    else
                    {
                        switch (b)
                        {
                            case 13:
                                builder.Append("=0D");
                                break;
                            case 10:
                                builder.Append("=0A");
                                break;
                            case 61:
                                builder.Append("=3D");
                                break;
                            default:
                                builder.Append(Convert.ToChar(b));
                                break;
                        }
                    }
            }

            return builder.ToString();
        }

        private string FormatEncodedString (string qpstr, int maxcharlen)
        {
            if (qpstr == null)
                throw new ArgumentNullException();

            StringBuilder builder = new StringBuilder();
            char[] charArray = qpstr.ToCharArray();
            int i = 0;
            foreach (char c in charArray)
            {
                builder.Append(c);
                i++;
                if (i == maxcharlen)
                {
                    builder.AppendLine("=");
                    i = 0;
                }
            }

            return builder.ToString();
        }

        private string HexDecoderEvaluator (Match m)
        {
            if (String.IsNullOrEmpty(m.Value))
                return null;

            CaptureCollection captures = m.Groups[3].Captures;
            byte[] bytes = new byte[captures.Count];

            for (int i = 0; i < captures.Count; i++)
            {
                bytes[i] = Convert.ToByte(captures[i].Value, 16);
            }

            return _encoding.GetString(bytes);
        }

        private  string HexDecoder (string line)
        {
            if (line == null)
                throw new ArgumentNullException();

            Regex re = new Regex("((\\=([0-9A-F][0-9A-F]))*)", RegexOptions.IgnoreCase);
            return re.Replace(line, new MatchEvaluator(HexDecoderEvaluator));
        }


        public  string Decode (string encodedText)
        {
            if (encodedText == null)
                throw new ArgumentNullException();

            using (StringReader sr = new StringReader(encodedText))
            {
                StringBuilder builder = new StringBuilder();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.EndsWith("="))
                        builder.Append(line.Substring(0, line.Length - 1));
                    else
                        builder.Append(line);
                }

                return HexDecoder(builder.ToString());
            }
        }

    }
}
