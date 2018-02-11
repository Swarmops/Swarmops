using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Swarmops.Common;

namespace Swarmops.Logic.Cache
{
    /// <summary>
    ///     This class stores arbitrary objects, keyed by a GUID, for 24 hours. Primarily intended for progress bars and
    ///     similar data that
    ///     needs to be stored in a separate, global namespace for accessibility reasons from JavaScript. DO NOT STORE
    ///     SENSITIVE STUFF here
    ///     as it's generally accessible, at least not without accompanying access control.
    ///     TODO: Share this data across a web farm some bloody how. Put it in database? Too slow for progress bars...
    ///     Memcached perhaps?
    /// </summary>
    public class GuidCache
    {
        private static readonly Dictionary<string, CachedObject> _cache;
        private static DateTime _nextGarbageCollect;

        static GuidCache()
        {
            _cache = new Dictionary<string, CachedObject>();
            _nextGarbageCollect = Constants.DateTimeLow;
        }

        public static void Set (string guidString, object objectToCache)
        {
            ConditionalGarbageCollect();

            _cache[guidString] = new CachedObject {StoredDateTime = DateTime.UtcNow, Object = objectToCache};
        }

        public static object Get (string guidString)
        {
            if (!_cache.ContainsKey (guidString))
            {
                return null;
            }
            return _cache[guidString].Object;
        }

        public static void Delete (string guidString)
        {
            if (_cache.ContainsKey (guidString))
            {
                _cache.Remove (guidString);
            }
        }


        public static void SetProgress(string guid, int progress)
        {
            GuidCache.Set(guid + "-Progress", progress.ToString(CultureInfo.InvariantCulture));
        }


        private static void ConditionalGarbageCollect()
        {
            lock (_cache) // thread safety
            {
                if (_nextGarbageCollect > DateTime.UtcNow)
                {
                    return; // not time yet
                }

                // Do a garbage collect and set the next one for, say, an hour out

                _nextGarbageCollect = DateTime.UtcNow.AddHours (1); // do this first for thread reasons
            }

            DateTime expired = DateTime.UtcNow.AddHours (-24);

            foreach (string key in _cache.Keys.ToList())
                // ToList() is crucial - it takes a copy of the list, which may be modified in-loop
            {
                if (_cache[key].StoredDateTime < expired)
                {
                    _cache.Remove (key);
                }
            }
        }

        private class CachedObject
        {
            public object Object;
            public DateTime StoredDateTime { get; set; }
        }
    }
}