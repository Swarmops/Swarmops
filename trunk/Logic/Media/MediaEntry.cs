using System;

using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Media
{
    public class MediaEntry : BasicMediaEntry
    {
        private MediaEntry (BasicMediaEntry basic) :
            base (basic)
        {
        }

        public static MediaEntry FromBasic (BasicMediaEntry basic)
        {
            return new MediaEntry (basic);
        }

        public static bool CreateFromKeyword (string keyword, string mediaName, bool isBlog, string url, string title,
                                              DateTime dateTime)
        {
            int id = PirateDb.GetDatabaseForWriting().CreateMediaEntryFromKeyword (keyword, mediaName, isBlog, url, title,
                                                                         dateTime);

            if (id != 0)
            {
                return true; // an object was created
            }
            else
            {
                return false;
            }
        }
    }
}