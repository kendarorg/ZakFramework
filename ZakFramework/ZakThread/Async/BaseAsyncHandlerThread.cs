using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ZakCore.Utils.Logging;
using ZakThread.Threading;
using ZakCore.Utils.Collections;

namespace ZakThread.Async
{
	public abstract class BaseAsyncHandlerThread : MainHandler
	{
		public ParallelOptions TasksOptions { get; protected set; }
		private LockFreeQueue<RequestObjectMessage> _batchExecuted;

		protected BaseAsyncHandlerThread(ILogger logger, string threadName, bool restartOnError = true) :
			base(logger, threadName, restartOnError)
		{
			_batchExecuted = new LockFreeQueue<RequestObjectMessage>();
		}

		internal override void HandleInternalTaskRequest(IMessage msg)
		{
			var msgTask = (RequestObjectMessage)msg;
			if (BatchSize == 1)
			{
				Task.Factory.StartNew(() =>
				{
					HandleTaskRequest(msgTask, msgTask.Content);
					msgTask.SetCompleted();
				}, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
			}
			else
			{
				if (!_batchTimeout.IsRunning) _batchTimeout.Start();
				Task.Factory.StartNew(() =>
				{
					HandleTaskRequest(msgTask, msgTask.Content);
					_batchExecuted.Enqueue(msgTask);
				}, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
			}
		}

		internal override void SetBatchCompleted()
		{
			if (BatchSize > 1)
			{
				if (_batchExecuted.Count > 0)
				{
					//if (_batchTimeout.ElapsedMilliseconds > BatchTimeoutMs || _batchExecuted.Count >= BatchSize)
					{
						_batchTimeout.Stop();

						var batchId = _batchId;
						Task.Factory.StartNew(() =>
						{
							foreach (var item in _batchExecuted.Dequeue())
							{
								item.SetCompleted(batchId);
							}
						});

						Interlocked.Increment(ref _batchId);
						_batchTimeout.Reset();
						_batchTimeout.Start();
					}
				}
			}
		}
	}
}
