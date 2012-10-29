using System;
using System.Collections.Generic;
using ZakCore.Utils.Logging;
using ZakThread.HighPower;
using ZakThread.Threading;

namespace ZakCache.CacheManager.CacheInternals
{
	public class CacheCallbacksExecutor : AsyncQueuedExecutor
	{
		private readonly Dictionary<string, CacheItem> _cachedItems;

		private const long MAX_MS_SINCE_START = 100;
		private const long MAX_MSGS_ELABORATED = 100;

		public CacheCallbacksExecutor(ILogger logger, string threadName) :
			base(logger, threadName)
		{
			_cachedItems = new Dictionary<string, CacheItem>();
			_tasksToComplete = new Dictionary<string, WaitingCacheLoadItemTask>();
			_taggedItems = new Dictionary<string, TaggedCacheItems>();
		}

		protected override bool IsBatch
		{
			get { return false; }
		}

		protected override bool CheckIfShouldStop(AsyncTask at, long msSinceStart, int msgsElaborated)
		{
			if (msSinceStart > MAX_MS_SINCE_START) return true;
			if (msgsElaborated > MAX_MSGS_ELABORATED) return true;
			return false;
		}

		protected override bool HandleMessage(IMessage msg)
		{
			var ctm = msg as CacheThreadMessage;
			if (ctm == null) return true;
			switch (ctm.MessageType)
			{
				case (CacheMessageType.Store):
					{
						BuildCacheItem(ctm.Key, ctm.Content, ctm.Tags, ctm.CacheExpirationMs);
					}
					break;
			}
			return true;
		}


		private readonly Dictionary<string, TaggedCacheItems> _taggedItems;

		private readonly Dictionary<string, WaitingCacheLoadItemTask> _tasksToComplete;

		protected override bool ShouldHandleTask(AsyncTask at)
		{
			var lit = (CacheLoadItemTask) at;
			if (lit != null)
			{
				if (_cachedItems.ContainsKey(lit.Message.Key))
				{
					lit.Result = _cachedItems[lit.Message.Key].Content;
					if (!lit.RunCallbackInsideCompleteTask && lit.Callback != null)
					{
						lit.Callback(lit);
					}
					lit.CompleteTask();
					return false;
				}
				if (!_tasksToComplete.ContainsKey(lit.Message.Key))
				{
					_tasksToComplete[lit.Message.Key] = new WaitingCacheLoadItemTask
						{CacheExpirationMs = DateTime.Now + TimeSpan.FromMilliseconds(CacheExpirationMs), Key = lit.Message.Key};
				}
				_tasksToComplete[lit.Message.Key].LoadItemTasks.Add(lit);
				return true;
			}
			return false;
		}

		public override void RegisterMessages()
		{
		}

		protected override bool RemoveExpiredTasks
		{
			get { return false; }
		}

		protected override void HandleTaskCompleted(AsyncTask at)
		{
			var lit = (CacheLoadItemTask) at;
			BuildCacheItem(lit.Message.Key, lit.Result, lit.Message.Tags, lit.CacheExpirationMs);
			if (_tasksToComplete.ContainsKey(lit.Message.Key))
			{
				foreach (var item in _tasksToComplete[lit.Message.Key].LoadItemTasks)
				{
					if (item.Callback != null) item.Callback(item);
				}
				_tasksToComplete[lit.Message.Key].LoadItemTasks.Clear();
				_tasksToComplete.Remove(lit.Message.Key);
			}
		}

		public int CacheExpirationMs { get; set; }

		protected override void OnExecutionCompleted()
		{
			var itemsKeys = new List<string>(_cachedItems.Keys);
			foreach (var key in itemsKeys)
			{
				if (DateTime.Now > _cachedItems[key].Expiration)
				{
					_cachedItems.Remove(key);
				}
			}
			itemsKeys = new List<string>(_taggedItems.Keys);
			foreach (var key in itemsKeys)
			{
				if (DateTime.Now > _taggedItems[key].Expiration)
				{
					var subItemsKeys = new List<string>(_taggedItems[key].CacheItems.Keys);
					foreach (var subkey in subItemsKeys)
					{
						if (_cachedItems.ContainsKey(subkey))
						{
							_cachedItems.Remove(subkey);
						}
					}
					_taggedItems[key].Expiration = DateTime.Now + TimeSpan.FromMilliseconds(_taggedItems[key].CacheExpirationMs == 0
						                                                                        ? CacheExpirationMs
						                                                                        : _taggedItems[key].CacheExpirationMs);
				}
			}
		}

		private void BuildCacheItem(string key, object content, string[] tags, long msTimeExpiration)
		{
			var ci = new CacheItem
				{
					Tags = tags,
					Content = content,
					Key = key,
					Expiration = DateTime.Now + TimeSpan.FromMilliseconds(msTimeExpiration == 0 ? CacheExpirationMs : msTimeExpiration)
				};

			if (_cachedItems.ContainsKey(key))
			{
				_cachedItems[key] = ci;
			}
			else
			{
				_cachedItems.Add(key, ci);
			}
			foreach (var tag in ci.Tags)
			{
				if (!_taggedItems.ContainsKey(tag))
				{
					_taggedItems.Add(tag, new TaggedCacheItems());
				}
				if (!_taggedItems[tag].CacheItems.ContainsKey(ci.Key))
				{
					_taggedItems[tag].CacheItems.Add(ci.Key, ci);
				}
				else
				{
					_taggedItems[tag].CacheItems[ci.Key] = ci;
				}
			}
		}
	}
}