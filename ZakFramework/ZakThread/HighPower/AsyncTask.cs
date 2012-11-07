using System;
using System.Threading;
using ZakThread.HighPower.Interfaces;

namespace ZakThread.HighPower
{
	public class AsyncTask : IAsyncResult, IDisposable
	{
		protected WaitHandle _waitHandle;

		public Guid TaskId { get; internal set; }

		public bool IsReallyAsync { get; set; }

		public AsyncTask(AsyncCallback callback, object tag, object state, IAsyncHandler asyncHandler,
		                 bool isReallyAsync = true)
		{
			IsReallyAsync = isReallyAsync;
			Callback = callback;
			Tag = tag;
			State = state;
			AsyncHandler = asyncHandler;
			_waitHandle = new ManualResetEvent(false);
			RunCallbackInsideCompleteTask = false;
		}

		/// <summary>
		/// The object that will execute the operations
		/// </summary>
		public IAsyncHandler AsyncHandler { get; internal set; }

		/// <summary>
		/// The "State" object
		/// </summary>
		public Object State { get; internal set; }

		/// <summary>
		/// Extra element
		/// </summary>
		public Object Tag { get; set; }

		public Object Result { get; set; }

		/// <summary>
		/// Used internally to handle all the elaborations
		/// </summary>
		public AsyncCallback Callback { get; internal set; }

		public IQueuedExecutor Executor { get; set; }

		public bool RunCallbackInsideCompleteTask { get; set; }

		public object AsyncState
		{
			get { return State; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		public bool IsCompleted
		{
			get { return _waitHandle.WaitOne(0, false); }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return _waitHandle; }
		}

		WaitHandle IAsyncResult.AsyncWaitHandle
		{
			get { return AsyncWaitHandle; }
		}

		internal volatile bool DoAbort = false;

		public void StartAsyncWork()
		{
			if (IsReallyAsync)
			{
				ThreadPool.QueueUserWorkItem(StartAsyncTask, null);
			}
			else
			{
				AsyncHandler.ExecuteAsyncProcessing(this);
			}
		}

		private void StartAsyncTask(Object workItemState)
		{
			if (DoAbort)
			{
				return;
			}
			if (Executor == null)
			{
				AsyncHandler.ExecuteAsyncProcessing(this);
				CompleteTask();
			}
			else
			{
				Executor.EnqueTask(this);
			}
		}

		public virtual void ExecuteCleanup()
		{
		}

		public virtual void CompleteTask()
		{
			try
			{
				if (!DoAbort)
				{
					if (null != Callback && RunCallbackInsideCompleteTask)
						Callback(this);
				}

				ExecuteCleanup();
				((ManualResetEvent) _waitHandle).Set();
			}
				// ReSharper disable EmptyGeneralCatchClause
			catch
			{
			}
			// ReSharper restore EmptyGeneralCatchClause
		}

		public void Dispose()
		{
			ExecuteCleanup();
			State = null;
			Callback = null;
			if (null != _waitHandle)
			{
				_waitHandle.Close();
				_waitHandle = null;
			}
		}
	}
}