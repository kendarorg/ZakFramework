using System;
using System.Threading;
using ZakCore.Utils.Collections;
using ZakThread.Threading;

namespace ZakThread.Async
{
	public class RequestObjectMessage : IMessage
	{
		private static LockFreeQueue<ManualResetEventSlim> _resetEvents;

		static RequestObjectMessage()
		{
			_resetEvents = new LockFreeQueue<ManualResetEventSlim>();
			int count = 100;
			while (count >= 0)
			{
				_resetEvents.Enqueue(new ManualResetEventSlim(false));
				count--;
			}
		}

		private readonly BaseRequestObject _requestObject;
		private readonly int _timeoutMs;
		private ManualResetEventSlim _autoResetEvent;

		public RequestObjectMessage(BaseRequestObject requestObject, int timeoutMs)
		{
			if (requestObject == null) throw new ArgumentNullException("requestObject");
			if (timeoutMs < 0) throw new ArgumentException("Parameter must be greater than 0", "timeoutMs");

			_requestObject = requestObject;
			_autoResetEvent = _resetEvents.DequeueSingle();
			if (null == _autoResetEvent)
			{
				_autoResetEvent = new ManualResetEventSlim(false);
			}
			_autoResetEvent.Reset();
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
			_resetEvents.Enqueue(_autoResetEvent);
			_autoResetEvent = null;
		}

		public void Wait()
		{
			if (_autoResetEvent!=null && !_autoResetEvent.Wait(_timeoutMs))
			{
				throw new TimeoutException(string.Format("Timeout waiting for answers expired ({0} ms)", _timeoutMs));
			}
		}
	}
}
