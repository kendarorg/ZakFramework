using System;
using ZakThread.HighPower;
using ZakThread.HighPower.Bases;

namespace ZakCache.CacheManager.CacheInternals
{
	public class CacheLoadItemTask : AsyncTask
	{
		
		public CacheLoadItemTask(AsyncCallback callback, CacheThreadMessage tag, object state, BaseAsyncHandler ash) :
			base(callback, tag, state, ash)
		{
			Message = tag;
			CacheExpirationMs = tag.CacheExpirationMs;
		}

		public long CacheExpirationMs {get;set;}
		public CacheThreadMessage Message	 { get; set; }

	}
}