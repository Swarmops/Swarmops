using System;
using System.Net;

namespace Swarmops.Utility.Special
{
    public class UrlTranslator
    {
        public static string Translate (string url)
        {
            if (url.StartsWith("http://knuff.se/k/"))
            {
                return TranslateKnuffUrl(url);
            }
            else
            {
                throw new NotSupportedException("Can't translate URL: " + url);
            }
        }

        private static string TranslateKnuffUrl (string url)
        {
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9b5) Gecko/2008050509 Firefox/3.0b5";

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            string newUrl = response.ResponseUri.AbsoluteUri;
            response.Close();

            return newUrl;
        }
    }
}