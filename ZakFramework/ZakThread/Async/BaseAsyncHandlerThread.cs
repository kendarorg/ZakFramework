using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ZakCore.Utils.Logging;
using ZakThread.Threading;

namespace ZakThread.Async
{
	public abstract class BaseAsyncHandlerThread : BaseMessageThread, ITasksHandlerThread
	{
		private long _batchId;
		private Stopwatch _batchTimeout;
		private int _batchSize;
		private List<RequestObjectMessage> _batchExecuted;

		protected BaseAsyncHandlerThread(ILogger logger, string threadName, bool restartOnError = true) :
			base(logger, threadName, restartOnError)
		{
			_batchId = 0;
			BatchTimeoutMs = 0;
			BatchSize = 1;
			TasksOptions = new ParallelOptions {MaxDegreeOfParallelism = -1};
		}

		public ParallelOptions TasksOptions { get; protected set; }

		public int BatchTimeoutMs { get; protected set; }

		public int BatchSize
		{
			get { return _batchSize; }
			protected set
			{
				_batchSize = value <= 0 ? 1 : value;
				if (BatchSize > 1)
				{
					if (BatchTimeoutMs == 0)
					{
						BatchTimeoutMs = 100;
					}
					_batchTimeout = new Stopwatch();
					_batchExecuted = new List<RequestObjectMessage>();
				}
			}
		}

		protected override bool RunSingleCycle()
		{
			return true;
		}

		public long DoRequestAndWait(BaseRequestObject requestObject, int timeoutMs, string senderId = null)
		{
			if (requestObject == null) throw new ArgumentNullException("requestObject");
			if (timeoutMs < 0) throw new ArgumentException("Parameter must be greater than 0", "timeoutMs");

			var msg = new RequestObjectMessage(requestObject, timeoutMs) { SourceThread = senderId };
			SendMessageToThread(msg);
			var sw = new Stopwatch();
			sw.Start();
			msg.Wait();
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		public void FireAndForget(BaseRequestObject requestObject, int timeoutMs, string senderId = null)
		{
			if (requestObject == null) throw new ArgumentNullException("requestObject");
			if (timeoutMs < 0) throw new ArgumentException("Parameter must be greater than 0", "timeoutMs");

			var msg = new RequestObjectMessage(requestObject, timeoutMs) { SourceThread = senderId };
			SendMessageToThread(msg);
		}

		public abstract bool HandleTaskRequest(RequestObjectMessage container, BaseRequestObject requestObject);
		public abstract void HandleBatchCompleted(IEnumerable<RequestObjectMessage> batchExecuted);

		protected override bool HandleMessage(IMessage msg)
		{
			var toret = false;
			var msgToSend = msg as RequestObjectMessage;
			if (msgToSend != null)
			{
				Task.Factory.StartNew(() =>
					{
						if (HandleTaskRequest(msgToSend, msgToSend.Content))
						{
							msgToSend.SetCompleted();
						}
					});
			}
			else
			{
				toret = base.HandleMessageInternal(msg);
			}
			SetBatchCompleted();
			return toret;
		}

		private void SetBatchCompleted()
		{
			if (BatchSize > 1)
			{
				if (_batchExecuted.Count > 0)
				{
					if (_batchTimeout.ElapsedMilliseconds > BatchTimeoutMs || _batchExecuted.Count >= BatchSize)
					{
						foreach (var item in _batchExecuted)
						{
							item.SetCompleted(_batchId);
						}
						_batchExecuted = new List<RequestObjectMessage>();
						Interlocked.Increment(ref _batchId);
						_batchTimeout.Reset();
					}
				}
			}
		}


		public override void RegisterMessages()
		{
			RegisterMessage(typeof(RequestObjectMessage));
		}
	}
}
