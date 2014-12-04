using System;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Media
{
    public class MediaEntry : BasicMediaEntry
    {
        private MediaEntry(BasicMediaEntry basic) :
            base(basic)
        {
        }

        public static MediaEntry FromBasic(BasicMediaEntry basic)
        {
            return new MediaEntry(basic);
        }

        public static bool CreateFromKeyword(string keyword, string mediaName, bool isBlog, string url, string title,
            DateTime dateTime)
        {
            int id = SwarmDb.GetDatabaseForWriting().CreateMediaEntryFromKeyword(keyword, mediaName, isBlog, url, title,
                dateTime);

            if (id != 0)
            {
                return true; // an object was created
            }
            return false;
        }
    }
}