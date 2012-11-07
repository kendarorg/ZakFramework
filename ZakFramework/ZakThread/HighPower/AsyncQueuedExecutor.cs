using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ZakCore.Utils.Collections;
using ZakCore.Utils.Logging;
using ZakThread.HighPower.Interfaces;
using ZakThread.Threading;

namespace ZakThread.HighPower
{
	/// <summary>
	/// This class is a thread that should handle all the elaboration of things added on the queue
	/// The aim is to wait for all tasks to be completed before doing further actions
	/// </summary>
	public abstract class AsyncQueuedExecutor : BaseMessageThread, IQueuedExecutor
	{
		protected AsyncQueuedExecutor(ILogger logger, string threadName)
			: base(logger, threadName)
		{
			BatchTimeoutMs = 0;
		}

		/// <summary>
		/// If true means that all tasks will be considered completed only when ALL tasks have been processed
		/// If false means that a task is considered completed atomically
		/// </summary>
		protected abstract bool IsBatch { get; }

		protected abstract bool RemoveExpiredTasks { get; }

		/// <summary>
		/// When passed the batch timeout all still running task will be completed without success
		/// </summary>
		protected int BatchTimeoutMs { get; set; }

		private readonly LockFreeQueue<AsyncTask> _tasksToExecute = new LockFreeQueue<AsyncTask>();

		/// <summary>
		/// Add a task to execute
		/// </summary>
		public object EnqueTask(AsyncTask newTask, int msTimeout = 5000)
		{
			_tasksToExecute.Enqueue(newTask);
			newTask.AsyncWaitHandle.WaitOne(msTimeout);
			if (newTask.IsCompleted)
			{
				return newTask.Result;
			}
			return null;
		}

		private readonly Dictionary<Guid, AsyncTask> _runningTasks = new Dictionary<Guid, AsyncTask>();

		protected override bool RunSingleCycle()
		{
			var sw = new Stopwatch();
			// ReSharper disable RedundantAssignment
			// ReSharper disable TooWideLocalVariableScope
			bool hseret = true;
			// ReSharper restore TooWideLocalVariableScope
			// ReSharper restore RedundantAssignment
			int msgCount = 0;
			sw.Start();
			foreach (AsyncTask at in _tasksToExecute.Dequeue())
			{
				if (ShouldHandleTask(at))
				{
					at.TaskId = Guid.NewGuid();
					if (at.IsReallyAsync)
					{
						at.StartAsyncWork();
					}
					_runningTasks.Add(at.TaskId, at);
					hseret = CheckIfShouldStop(at, sw.ElapsedMilliseconds, msgCount);
					if (!hseret)
					{
						break;
					}
					msgCount++;
				}
			}

			int jobDone = 0;
			if (!IsBatch)
			{
				var vals = new List<AsyncTask>(_runningTasks.Values);
				foreach (var task in vals)
				{
					//And had already run the callback function
					if (task.IsCompleted || !task.IsReallyAsync)
					{
						HandleTaskCompleted(task);
						if (!task.IsReallyAsync)
						{
							if (!task.RunCallbackInsideCompleteTask && task.Callback != null)
							{
								task.Callback(task);
							}
							task.CompleteTask();
						}
						_runningTasks[task.TaskId] = null;
						_runningTasks.Remove(task.TaskId);
						jobDone++;
					}
				}
			}
			else
			{
				var completedTasks = new List<AsyncTask>();
				while (_runningTasks.Count > 0 && sw.ElapsedMilliseconds < BatchTimeoutMs)
				{
					var vals = _runningTasks.Values;
					foreach (var task in vals)
					{
						//And had already run the callback function
						if (task.IsCompleted)
						{
							HandleTaskCompleted(task);
							completedTasks.Add(task);
							_runningTasks[task.TaskId] = new AsyncTask(null, null, null, null);
							_runningTasks.Remove(task.TaskId);
							jobDone++;
						}
					}
				}

				HandleBatchCompleted(completedTasks);
				foreach (var task in completedTasks)
				{
					if (!task.RunCallbackInsideCompleteTask)
					{
						task.Callback(task);
					}
				}
			}

			if (RemoveExpiredTasks)
			{
				foreach (var task in _runningTasks)
				{
					//And had already run the callback function
					task.Value.DoAbort = true;
				}
				_runningTasks.Clear();
			}

			sw.Stop();
			OnExecutionCompleted();
			if (jobDone > 0)
			{
				Thread.Sleep(0);
			}
			else
			{
				Thread.Sleep(0);
				if (WaitCycle > 0) Thread.Sleep(WaitCycle);
			}
			return true;
		}

		protected virtual void OnExecutionCompleted()
		{
		}

		protected virtual void HandleBatchCompleted(List<AsyncTask> completedTasks)
		{
		}

		/// <summary>
		/// When the task is completed do some other work
		/// </summary>
		/// <param name="asyncTask"></param>
		protected abstract void HandleTaskCompleted(AsyncTask asyncTask);

		/// <summary>
		/// Return true if the task should be run
		/// </summary>
		/// <param name="at"></param>
		/// <returns></returns>
		protected abstract bool ShouldHandleTask(AsyncTask at);


		/// <summary>
		/// Function to check if should do something
		/// </summary>
		/// <param name="at"></param>
		/// <param name="msSinceStart"></param>
		/// <param name="msgsElaborated"></param>
		/// <returns>Returns true if should continue to elaborate. The purpouse of this is to avoid clogging
		/// the firehose :P if should elaborate only a certain number of messages or for a ceratain time</returns>
		protected abstract bool CheckIfShouldStop(AsyncTask at, long msSinceStart, int msgsElaborated);
	}
}