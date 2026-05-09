using System;
using System.Collections.Generic;

namespace AnagramServer
{
    public class CacheManager
    {
        private Dictionary<string, CacheEntry> cache =
            new Dictionary<string, CacheEntry>();

        private Dictionary<string, object> keyLocks =
            new Dictionary<string, object>();

        private readonly object cacheLock = new object();

        private readonly TimeSpan ttl =
            TimeSpan.FromMinutes(5);

        public string GetOrAdd(string key, Func<string> valueFactory)
        {
            lock (cacheLock)
            {
                if (cache.ContainsKey(key))
                {
                    CacheEntry entry = cache[key];

                    if (entry.Expiration > DateTime.Now)
                    {
                        Logger.Log($"CACHE HIT: {key}");

                        return entry.Result;
                    }

                    cache.Remove(key);
                }

                if (!keyLocks.ContainsKey(key))
                {
                    keyLocks[key] = new object();
                }
            }

            object keyLock;

            lock (cacheLock)
            {
                keyLock = keyLocks[key];
            }

            lock (keyLock)
            {
                lock (cacheLock)
                {
                    if (cache.ContainsKey(key))
                    {
                        CacheEntry entry = cache[key];

                        if (entry.Expiration > DateTime.Now)
                        {
                            return entry.Result;
                        }
                    }
                }

                Logger.Log($"CACHE MISS: {key}");

                string result = valueFactory();

                CacheEntry newEntry = new CacheEntry
                {
                    Result = result,
                    Expiration = DateTime.Now.Add(ttl)
                };

                lock (cacheLock)
                {
                    cache[key] = newEntry;
                }

                return result;
            }
        }
    }
}