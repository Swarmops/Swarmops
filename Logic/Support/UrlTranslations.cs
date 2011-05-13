using Activizr.Database;

namespace Activizr.Logic.Support
{
    public class UrlTranslations
    {
        public static bool Create (string url)
        {
            int id = PirateDb.GetDatabase().CreateUrlTranslation(url);

            if (id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Set (string originalUrl, string translatedUrl)
        {
            PirateDb.GetDatabase().SetUrlTranslation(originalUrl, translatedUrl);
        }

        public static string[] GetUntranslated (int maxCount)
        {
            return PirateDb.GetDatabase().GetUntranslatedUrls(maxCount);
        }
    }
}