using System;
using System.Collections.Generic;

namespace ZakCache.CacheManager.CacheInternals
{
	internal class WaitingCacheLoadItemTask
	{
		public List<CacheLoadItemTask> LoadItemTasks = new List<CacheLoadItemTask>();
		public string Key;
		public DateTime CacheExpirationMs;
	}
}
