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
	public abstract class BaseSyncHandlerThread : MainHandler
	{
		private Queue<RequestObjectMessage> _batchExecuted;

		protected BaseSyncHandlerThread(ILogger logger, string threadName, bool restartOnError = true) :
			base(logger, threadName, restartOnError)
		{
			_batchExecuted = new Queue<RequestObjectMessage>();
		}

		internal override void HandleInternalTaskRequest(IMessage msg)
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

		internal override void SetBatchCompleted()
		{
			if (BatchSize > 1)
			{
				if (_batchExecuted.Count > 0)
				{
					//if (_batchTimeout.ElapsedMilliseconds > BatchTimeoutMs || _batchExecuted.Count >= BatchSize)
					{
						_batchTimeout.Stop();
						HandleBatchCompleted(_batchExecuted);

						var item = _batchExecuted.Dequeue();
						while (item != null)
						{

							item.SetCompleted(_batchId);
							item = _batchExecuted.Count > 0 ? _batchExecuted.Dequeue() : null;
						}

						_batchExecuted = new Queue<RequestObjectMessage>();
						Interlocked.Increment(ref _batchId);
						_batchTimeout.Reset();
						_batchTimeout.Start();
					}
				}
			}
		}
	}
}
