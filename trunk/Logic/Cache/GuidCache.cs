using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Swarmops.Logic.Cache
{
    /// <summary>
    ///  This class stores arbitrary objects, keyed by a GUID, for 24 hours. Primarily intended for progress bars and similar data that
    /// needs to be stored in a separate, global namespace for accessibility reasons from JavaScript. DO NOT STORE SENSITIVE STUFF here
    /// as it's generally accessible, at least not without accompanying access control.
    /// 
    /// TODO: Share this data across a web farm some bloody how. Put it in database? Too slow for progress bars... Memcached perhaps?
    /// </summary>
    public class GuidCache
    {
        static GuidCache()
        {
            _cache = new Dictionary<string, CachedObject>();
            _nextGarbageCollect = DateTime.MinValue;
        }

        static public void Set(string guidString, object objectToCache)
        {
            ConditionalGarbageCollect();

            _cache[guidString] = new CachedObject() {StoredDateTime = DateTime.UtcNow, Object = objectToCache};
        }

        static public object Get(string guidString)
        {
            if (!_cache.ContainsKey(guidString))
            {
                return null;
            }
            return _cache[guidString].Object;
        }

        static private void ConditionalGarbageCollect()
        {
            lock (_cache) // thread safety
            {
                if (_nextGarbageCollect > DateTime.UtcNow)
                {
                    return; // not time yet
                }

                // Do a garbage collect and set the next one for, say, an hour out

                _nextGarbageCollect = DateTime.UtcNow.AddHours(1); // do this first for thread reasons
            }

            DateTime expired = DateTime.UtcNow.AddHours(-24);

            foreach (string key in _cache.Keys.ToList())  // ToList() is crucial - it takes a copy of the list, which may be modified in-loop
            {
                if (_cache[key].StoredDateTime < expired)
                {
                    _cache.Remove(key);
                }
            }
        }

        static private Dictionary<string, CachedObject> _cache;
        private static DateTime _nextGarbageCollect;

        private class CachedObject
        {
            public DateTime StoredDateTime { get; set; }
            public object Object;
        }
    }


}
