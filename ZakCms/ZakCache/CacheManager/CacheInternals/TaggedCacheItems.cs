using System;
using System.Collections.Generic;

namespace ZakCache.CacheManager.CacheInternals
{
	internal class TaggedCacheItems
	{
		public Dictionary<string, CacheItem> CacheItems = new Dictionary<string, CacheItem>();
		public DateTime Expiration;
		public long CacheExpirationMs;
	}
}
