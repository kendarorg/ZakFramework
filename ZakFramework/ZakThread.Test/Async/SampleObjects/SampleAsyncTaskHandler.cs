using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZakCore.Utils.Logging;
using ZakThread.Async;

namespace ZakThread.Test.Async.SampleObjects
{
	public class SampleAsyncTasksHandler : BaseAsyncHandlerThread
	{
		private readonly int _waitTimeMs;
		private long _callsCount;

		public long CallsCount { get { return Interlocked.Read(ref _callsCount); } }

		public SampleAsyncTasksHandler(string threadName, int waitTimeMs,ParallelOptions parallelOptions, int batchSize = 0, int batchTimeoutMs = 0) :
			base(NullLogger.Create(), threadName, true)
		{
			BatchSize = batchSize;
			BatchTimeoutMs = batchTimeoutMs;
			TasksOptions = parallelOptions;
			_waitTimeMs = waitTimeMs;
			_callsCount = 0;
		}

		public override bool HandleTaskRequest(RequestObjectMessage container, BaseRequestObject requestObject)
		{
			if (BatchSize == 1)
			{
				var request = (RequestObject)requestObject;
				request.Return = -request.RequestId;
				Interlocked.Increment(ref _callsCount);
				Thread.Sleep(_waitTimeMs);
			}
			return true;
		}

		public override void HandleBatchCompleted(List<RequestObjectMessage> batchExecuted)
		{
			foreach (var item in batchExecuted)
			{
				var request = (RequestObject)item.Content;
				request.Return = -request.RequestId;
				Interlocked.Increment(ref _callsCount);
				Thread.Sleep(_waitTimeMs);
			}
		}
	}
}
