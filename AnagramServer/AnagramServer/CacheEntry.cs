using System;

namespace AnagramServer
{
    public class CacheEntry
    {
        public required string Result { get; set; }

        public DateTime Expiration { get; set; }
    }
}