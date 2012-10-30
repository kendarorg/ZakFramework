using System;
using System.Collections.Generic;
using ZakCache.CacheManager.CacheInternals;
using ZakCore.Utils.Logging;
using ZakThread.HighPower;
using ZakThread.HighPower.Bases;

namespace ZakCache.CacheManager
{
	public delegate string AsyncMethodCaller(int callDuration, out int threadId);

	public class CacheFacade :BaseAsyncHandler, IDisposable
	{

		protected override AsyncTask GenerateTask(AsyncCallback callback, Object tag, Object state, BaseAsyncHandler ash)
		{
			return new CacheLoadItemTask(callback, (CacheThreadMessage)tag, state, ash);
		}

		private readonly CacheCallbacksExecutor _queuedExecutor;

		public CacheFacade(ILogger logger, string threadName, int cacheExpirationMs)
		{
			_queuedExecutor = new CacheCallbacksExecutor(logger, threadName + "_QueuedExecutor")
				{CacheExpirationMs = cacheExpirationMs};
			_queuedExecutor.RunThread();
		}

		public void RegisterItem(string key, object content,
			long msTimeExpiration =0,
			params string[] tags)
		{
			_queuedExecutor.SendMessageToThread(new CacheThreadMessage(key, CacheMessageType.Store,tags) { 
				CacheExpirationMs = msTimeExpiration,
				Content = content });
		}

		public object RequireItem(string key,
			long msTimeExpiration = 0,
			string[] tags = null,
			RetrieveDataCallback retrieveDataFunction = null,
			params object[] retrieveDataParams)
		{
			var ctm = new CacheThreadMessage(key, CacheMessageType.Retrieve,tags) 
			{
				DataCallback = retrieveDataFunction,
				DataCallbackParameters = retrieveDataParams,
				CacheExpirationMs = msTimeExpiration
			};
			var asop = (CacheLoadItemTask)RunAsyncOperation(null, ctm, null);
			_queuedExecutor.EnqueTask(asop);
			asop.AsyncWaitHandle.WaitOne(5000, false);
			return asop.Result;
		}

		public override bool ExecuteAsyncProcessing(AsyncTask asyncTask)
		{
			var msg = asyncTask.Tag as CacheThreadMessage;
			if (msg == null) return true;
			if (msg.DataCallbackParameters != null && msg.DataCallback != null)
			{
				asyncTask.Result = msg.DataCallback(msg.DataCallbackParameters);
				return true;
			}
			return false;
		}

		public override void FinalizeBatchElements(List<AsyncTask> atl)
		{
			
		}

		public void Dispose()
		{
			_queuedExecutor.Terminate();
		}
	}
}
