using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ZakCore.Utils.Logging;
using ZakThread.Threading;

namespace ZakThread.Async
{
	/// <summary>
	/// Base class to run tasks synchronously
	/// </summary>
	public abstract class BaseSyncHandlerThread : BaseMessageThread, ITasksHandlerThread
	{
		private long _batchId;
		private Stopwatch _batchTimeout;
		private int _batchSize;
		private Queue<RequestObjectMessage> _batchExecuted;

		protected BaseSyncHandlerThread(ILogger logger, string threadName, bool restartOnError = true) :
			base(logger, threadName, restartOnError)
		{
			_batchId = 0;
			BatchTimeoutMs = 0;
			BatchSize = 1;
		}

		public int BatchTimeoutMs { get; protected set; }

		public int BatchSize
		{
			get { return _batchSize; }
			protected set
			{
				_batchSize = value <= 0 ? 1 : value;
				if (BatchTimeoutMs == 0)
				{
					BatchTimeoutMs = 100;
				}
				if (BatchSize > 1)
				{
					//MaxMesssagesPerCycle = BatchSize;
					_batchTimeout = new Stopwatch();
					_batchExecuted = new Queue<RequestObjectMessage>();
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

		public abstract bool HandleTaskRequest(RequestObjectMessage container, BaseRequestObject requestObject);
		public abstract void HandleBatchCompleted(IEnumerable<RequestObjectMessage> batchExecuted);

		protected override bool HandleMessage(IMessage msg)
		{
			var toret = true;
			if (msg.GetType().Name.EndsWith("RequestObjectMessage"))
			{
				var msgTask = (RequestObjectMessage)msg;

				HandleTaskRequest(msgTask, msgTask.Content);
				if (BatchSize == 1)
				{
					msgTask.SetCompleted();
				}
				else
				{
					if (!_batchTimeout.IsRunning) _batchTimeout.Start();
					_batchExecuted.Enqueue(msgTask);
				}
			}
			else
			{
				toret = base.HandleMessageInternal(msg);
			}
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
						_batchTimeout.Stop();
						var ms = _batchTimeout.ElapsedMilliseconds;
						var sz = _batchExecuted.Count;
						Debug.WriteLine("");
						Debug.WriteLine(DateTime.Now + " Ms " + ms + " Sz " + sz);
						Debug.WriteLine("");
						HandleBatchCompleted(_batchExecuted);
						var be = _batchExecuted;
						//Task.Factory.StartNew(() =>
							{
								var item = _batchExecuted.Dequeue();
								while (item!=null)
								{
									
									item.SetCompleted(_batchId);
									item = _batchExecuted.Dequeue();
								}
							}
							//);
						
						_batchExecuted = new Queue<RequestObjectMessage>();
						Interlocked.Increment(ref _batchId);
						_batchTimeout.Reset();
						_batchTimeout.Start();
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
