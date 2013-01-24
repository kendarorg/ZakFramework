using System.Collections.Generic;

namespace ZakThread.Async
{
	public interface ITasksHandlerThread
	{
		int BatchTimeoutMs { get; }
		int BatchSize { get; }

		long DoRequestAndWait(BaseRequestObject requestObject, int timeoutMs, string senderId = null);
		void FireAndForget(BaseRequestObject requestObject, int timeoutMs, string senderId = null);
		bool HandleTaskRequest(RequestObjectMessage container, BaseRequestObject requestObject);
		void HandleBatchCompleted(List<RequestObjectMessage> batchExecuted);
	}
}
