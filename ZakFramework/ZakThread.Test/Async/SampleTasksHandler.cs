
using System.Threading;
using ZakCore.Utils.Logging;
using ZakThread.Test.Async.Utils;

namespace ZakThread.Test.Async
{
	public class SampleTasksHandler:BaseSyncHandlerThread
	{
		private readonly int _waitTimeMs;
		private long _callsCount;

		public long CallsCount { get { return Interlocked.Read(ref _callsCount); }}

		public SampleTasksHandler(string threadName, int waitTimeMs, int batchSize = 0, int batchTimeoutMs = 0) : 
			base(NullLogger.Create(), threadName, true)
		{
			BatchSize = batchSize;
			BatchTimeoutMs = batchTimeoutMs;
			_waitTimeMs = waitTimeMs;
			_callsCount = 0;
		}

		protected override bool HandleSyncTaskRequest(RequestObjectMessage container, BaseRequestObject requestObject)
		{
			var request = (RequestObject) requestObject;
			request.Return = -request.RequestId;
			Interlocked.Increment(ref _callsCount);
			Thread.Sleep(_waitTimeMs);
			return true;
		}
	}
}
