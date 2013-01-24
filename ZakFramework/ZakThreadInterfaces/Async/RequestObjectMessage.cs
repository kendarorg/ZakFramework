using System;
using System.Threading;
using ZakThread.Threading;

namespace ZakThread.Async
{
	public class RequestObjectMessage : IMessage
	{
		private readonly BaseRequestObject _requestObject;
		private readonly int _timeoutMs;
		private readonly AutoResetEvent _autoResetEvent;

		public RequestObjectMessage(BaseRequestObject requestObject, int timeoutMs)
		{
			if (requestObject == null) throw new ArgumentNullException("requestObject");
			if (timeoutMs < 0) throw new ArgumentException("Parameter must be greater than 0", "timeoutMs");

			_requestObject = requestObject;
			_autoResetEvent = new AutoResetEvent(false);
			_timeoutMs = timeoutMs == 0 ? -1 : timeoutMs;
		}

		public BaseRequestObject Content { get { return _requestObject; } }
		public Guid Id { get; set; }
		public DateTime TimeStamp { get; set; }
		public string SourceThread { get; set; }
		public long BatchId { get; private set; }

		public object Clone()
		{
			return this;
		}

		public void SetCompleted(long batchId = -1)
		{
			BatchId = batchId;
			_autoResetEvent.Set();
		}

		public void Wait()
		{
			if (!_autoResetEvent.WaitOne(_timeoutMs))
			{
				throw new TimeoutException(string.Format("Timeout waiting for answers expired ({0} ms)", _timeoutMs));
			}
		}
	}
}
