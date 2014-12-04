using Swarmops.Database;

namespace Swarmops.Logic.Support
{
    public class UrlTranslations
    {
        public static bool Create(string url)
        {
            int id = SwarmDb.GetDatabaseForWriting().CreateUrlTranslation(url);

            if (id > 0)
            {
                return true;
            }
            return false;
        }

        public static void Set(string originalUrl, string translatedUrl)
        {
            SwarmDb.GetDatabaseForWriting().SetUrlTranslation(originalUrl, translatedUrl);
        }

        public static string[] GetUntranslated(int maxCount)
        {
            return SwarmDb.GetDatabaseForReading().GetUntranslatedUrls(maxCount);
        }
    }
}