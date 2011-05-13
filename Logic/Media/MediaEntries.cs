using System;
using System.Collections.Generic;

using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Media
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
            return PirateDb.GetDatabase().GetBlogKeywords();
        }

        public static string[] GetOldMediaKeywords()
        {
            return PirateDb.GetDatabase().GetOldMediaKeywords();
        }

        public static MediaEntries FromBlogKeyword (string keyword, DateTime minimumAge)
        {
            return FromArray (PirateDb.GetDatabase().GetBlogEntriesForKeyword (keyword, minimumAge));
        }

        public static MediaEntries FromOldMediaKeyword (string keyword, DateTime minimumAge)
        {
            return FromArray (PirateDb.GetDatabase().GetOldMediaEntriesForKeyword (keyword, minimumAge));
        }

        public static Dictionary<int, bool> GetMediaTypeTable()
        {
            return PirateDb.GetDatabase().GetMediaTypeTable();
        }

        public static Dictionary<int, string> GetMediaKeywordTable()
        {
            return PirateDb.GetDatabase().GetMediaKeywordTable();
        }

        public static MediaEntries FromKeywordsSimplified (string[] keywords)
        {
            var keywordIds = new List<int>();

            foreach (string keyword in keywords)
            {
                keywordIds.Add (PirateDb.GetDatabase().GetMediaKeywordId (keyword));
            }

            return FromArray (PirateDb.GetDatabase().GetMediaEntriesForKeywordIdsSimplified (keywordIds.ToArray()));
        }

        public static int GetKeywordId (string keyword)
        {
            return PirateDb.GetDatabase().GetMediaKeywordId (keyword);
        }
    }
}