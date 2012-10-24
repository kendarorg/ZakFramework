using System;
using System.Collections.Generic;

namespace ZakCache.CacheManager.CacheInternals
{
	internal class CacheItem
	{
		public DateTime Expiration;
		public Object Content;
		public string[] Tags;
		public string Key;
		public List<TaggedCacheItems> TagsList = new List<TaggedCacheItems>();
	}
}
