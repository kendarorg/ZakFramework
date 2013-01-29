using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZakCore.Utils.Logging;
using ZakThread.Threading;

namespace ZakThread.Async
{
	public abstract class MainHandler : BaseMessageThread, ITasksHandlerThread
	{
		internal long _batchId;
		internal Stopwatch _batchTimeout;
		private int _batchSize;

		protected MainHandler(ILogger logger, string threadName, bool restartOnError = true) :
			base(logger, threadName, restartOnError)
		{
			_batchId = 0;
			BatchTimeoutMs = -1;
			BatchSize = 1;
		}

		private int _batchTimeoutMs;

		public int BatchTimeoutMs
		{
			get { return _batchTimeoutMs; }
			protected set
			{
				if (value == 1 || value == 0) _batchTimeoutMs = 2;
				else _batchTimeoutMs = value;
			}
		}

		public int BatchSize
		{
			get { return _batchSize; }
			protected set
			{
				_batchSize = value <= 0 ? 1 : value;
				if (BatchTimeoutMs == -1)
				{
					BatchTimeoutMs = 10;
				}
				if (BatchSize > 1)
				{
					//MaxMesssagesPerCycle = BatchSize;
					_batchTimeout = new Stopwatch();
				}
			}
		}

		protected override bool CyclicExecution()
		{
			var toret = base.CyclicExecution();
			if (toret) SetBatchCompleted();
			return toret;
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

		internal override bool HandleMessageInternal(IMessage msg)
		{
			var toret = true;
			if (msg.GetType().Name.EndsWith("RequestObjectMessage"))
			{
				HandleInternalTaskRequest(msg);
			}
			else
			{
				toret = base.HandleMessageInternal(msg);
			}
			return toret;
		}

		internal abstract void SetBatchCompleted();

		public override void RegisterMessages()
		{
			RegisterMessage(typeof(RequestObjectMessage));
		}

		public abstract bool HandleTaskRequest(RequestObjectMessage container, BaseRequestObject requestObject);
		public abstract void HandleBatchCompleted(IEnumerable<RequestObjectMessage> batchExecuted);
		internal abstract void HandleInternalTaskRequest(IMessage msg);
	}
}
