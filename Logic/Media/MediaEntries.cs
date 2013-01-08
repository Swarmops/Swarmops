using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Media
{
    public class MediaEntries : List<MediaEntry>
    {
        public static MediaEntries FromArray (BasicMediaEntry[] basicArray)
        {
            var result = new MediaEntries();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicMediaEntry basic in basicArray)
            {
                result.Add (MediaEntry.FromBasic (basic));
            }

            return result;
        }

        public void Add (MediaEntries moreMediaEntries)
        {
            foreach (MediaEntry entry in moreMediaEntries)
            {
                Add (entry);
            }
        }

        public static string[] GetBlogKeywords()
        {
            return PirateDb.GetDatabaseForReading().GetBlogKeywords();
        }

        public static string[] GetOldMediaKeywords()
        {
            return PirateDb.GetDatabaseForReading().GetOldMediaKeywords();
        }

        public static MediaEntries FromBlogKeyword (string keyword, DateTime minimumAge)
        {
            return FromArray (PirateDb.GetDatabaseForReading().GetBlogEntriesForKeyword (keyword, minimumAge));
        }

        public static MediaEntries FromOldMediaKeyword (string keyword, DateTime minimumAge)
        {
            return FromArray (PirateDb.GetDatabaseForReading().GetOldMediaEntriesForKeyword (keyword, minimumAge));
        }

        public static Dictionary<int, bool> GetMediaTypeTable()
        {
            return PirateDb.GetDatabaseForReading().GetMediaTypeTable();
        }

        public static Dictionary<int, string> GetMediaKeywordTable()
        {
            return PirateDb.GetDatabaseForReading().GetMediaKeywordTable();
        }

        public static MediaEntries FromKeywordsSimplified (string[] keywords)
        {
            var keywordIds = new List<int>();

            foreach (string keyword in keywords)
            {
                keywordIds.Add (PirateDb.GetDatabaseForReading().GetMediaKeywordId (keyword));
            }

            return FromArray (PirateDb.GetDatabaseForReading().GetMediaEntriesForKeywordIdsSimplified (keywordIds.ToArray()));
        }

        public static int GetKeywordId (string keyword)
        {
            return PirateDb.GetDatabaseForReading().GetMediaKeywordId (keyword);
        }
    }
}