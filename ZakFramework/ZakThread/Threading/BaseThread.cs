﻿using System;
using System.Threading;
using ZakCore.Utils.Collections;
using ZakCore.Utils.Logging;
using ZakThread.Threading.Enums;

namespace ZakThread.Threading
{
	/// <summary>
	/// Base class for threading.
	/// </summary>
	public abstract class BaseThread : IBaseThread, IDisposable
	{
		/// <summary>
		/// Counter for thread identifiers
		/// </summary>
		private static UInt16 _threadCounter;

		/// <summary>
		/// Retrieve the thread counter
		/// </summary>
		internal static UInt16 ThreadCounter
		{
			get { return _threadCounter; }
		}

		/// <summary>
		/// Constructor that should not be implemented
		/// </summary>
		protected BaseThread()
		{
			throw new NotSupportedException();
		}

		protected BaseThread(ILogger logger, String threadName, bool restartOnError = true)
		{
			_threadCounter = 0;
			_continueRunning = 0;
			ThreadName = threadName.ToUpper();
			if (_threadCounter >= UInt16.MaxValue)
			{
				throw new IndexOutOfRangeException("Too much threads");
			}
			_threadCounter++;
			ThreadId = _threadCounter;
			_logger = logger;
			_restartOnError = restartOnError;
		}

		public ILogger Logger { get { return _logger; } }

		/// <summary>
		/// String identifier of the thread
		/// </summary>
		public String ThreadName { get; private set; }

		/// <summary>
		/// Integer identifier of the thread
		/// </summary>
		public UInt16 ThreadId { get; private set; }

		/// <summary>
		/// Thread running.
		/// </summary>
		private Thread _thread;

		private ILogger _logger;

		/// <summary>
		/// If should restart on error
		/// </summary>
		internal Boolean _restartOnError = false;

		/// <summary>
		/// List of current exceptions
		/// </summary>
		private LockFreeQueue<Exception> _exceptions = new LockFreeQueue<Exception>();

		/// <summary>
		/// Internal marker to check if should terminate the thread gracefully
		/// </summary>
		private Int64 _continueRunning;

		/// <summary>
		/// Thread status
		/// </summary>
		private Int64 _status = (Int64) RunningStatus.None;

		/// <summary>
		/// Status of the object.
		/// </summary>
		public RunningStatus Status
		{
			get { return (RunningStatus) Interlocked.Read(ref _status); }
			private set { Interlocked.Exchange(ref _status, (Int64) value); }
		}

		/// <summary>
		/// Rerieve the last errors. When an error is parsed, the exception is lost.
		/// </summary>
		public Exception LastError
		{
			get
			{
				var ex = new Exception();
				if (_exceptions.Dequeue(ref ex))
				{
					return ex;
				}
				return null;
			}
		}

		/// <summary>
		/// Start the thread
		/// </summary>
		public virtual void RunThread()
		{
			_thread = new Thread(StaticRunThread);
			_thread.Start(this);
		}

		/// <summary>
		/// Internal function to run the whole thing
		/// </summary>
		/// <param name="par"></param>
		private static void StaticRunThread(Object par)
		{
			var b = (BaseThread) par;
			Boolean exceptionThrown = false;
			Boolean threadAborted = false;
			try
			{
				Interlocked.CompareExchange(ref b._continueRunning, 1, 0);
				b.Initialize();
				b.Status = RunningStatus.Initialized;
				b.RunInternal();
			}
			catch (ThreadAbortException tae)
			{
				b._exceptions.Enqueue(tae);
				b.Status = RunningStatus.Aborted;
				threadAborted = true;
			}
			catch (Exception ex)
			{
				Boolean continueRunning = b.HandleException(ex);
				b._exceptions.Enqueue(ex);
				if (continueRunning && b._restartOnError)
				{
					Interlocked.Exchange(ref b._continueRunning, 0);
					b._thread = new Thread(StaticRunThread);
					b._thread.Start(b);
					return;
				}
				exceptionThrown = true;
			}


			Interlocked.Exchange(ref b._continueRunning, 0);

			if (threadAborted)
			{
				return;
			}
			if (exceptionThrown)
			{
				b.Status = RunningStatus.ExceptionThrown;
			}
			else
			{
				b.CleanUp();
				b.Status = RunningStatus.Halted;
				b._exceptions = new LockFreeQueue<Exception>();
			}
		}

		/// <summary>
		/// The lifecycle of the object
		/// </summary>
		private void RunInternal()
		{
			Status = RunningStatus.Running;
			while (Interlocked.Read(ref _continueRunning) == 1)
			{
				if (!CyclicExecution())
				{
					Status = RunningStatus.Halted;
					return;
				}
				Thread.Sleep(0);
			}
			Status = RunningStatus.Halted;
		}

		/// <summary>
		/// Terminate the thread
		/// </summary>
		/// <param name="force">If true, abort!!!</param>
		public virtual void Terminate(Boolean force = false)
		{
			if (_thread == null)
			{
				return;
			}

			if (Status != RunningStatus.Running)
			{
				return;
			}
			if (force && _thread != null)
			{
				_thread.Abort();
				Status = RunningStatus.Aborted;
			}
			else
			{
				Status = RunningStatus.Halting;
				Interlocked.Exchange(ref _continueRunning, 0);
			}
		}

		/// <summary>
		/// Wait for the thread to terminate
		/// </summary>
		/// <param name="timeoutMs">ms to wait for the thread termination.</param>
		public void WaitTermination(Int64 timeoutMs)
		{
			DateTime timeoutTime = DateTime.UtcNow;
			timeoutTime = timeoutTime.AddMilliseconds(timeoutMs);

			var tmp = RunningStatus.None;
			while (timeoutTime > DateTime.UtcNow)
			{
				tmp = Status;
				if (tmp != RunningStatus.Running && tmp != RunningStatus.Halting)
				{
					Thread.Sleep(0);
					Thread.Sleep(1);
					return;
				}
				Thread.Sleep(0);
				Thread.Sleep(1);
			}
			if (tmp != RunningStatus.Running && tmp != RunningStatus.Halting)
			{
				return;
			}
			throw new TimeoutException();
		}

		/// <summary>
		/// CleanUp the whole thing.
		/// </summary>
		protected virtual void CleanUp()
		{
		}

		/// <summary>
		/// Initialize the thread
		/// </summary>
		protected virtual void Initialize()
		{
		}

		/// <summary>
		/// Handle the exception.
		/// </summary>
		/// <param name="ex"></param>
		/// <returns>True if should continue running. restartOnError MUST be set.</returns>
		protected virtual Boolean HandleException(Exception ex)
		{
			return true;
		}

		protected virtual Boolean CyclicExecution()
		{
			return RunSingleCycle();
		}

		/// <summary>
		/// Do the cyclic execution.
		/// </summary>
		/// <returns></returns>
		protected abstract Boolean RunSingleCycle();

#if DEBUG
		internal Thread IntThread
		{
			get { return _thread; }
		}
#endif

		public virtual void Dispose()
		{
			if (Status == RunningStatus.Running)
			{
				Terminate(true);
			}
			ThreadId = 0;
			ThreadName = null;
			_logger = null;
			_exceptions.Clear();
			_thread = null;
		}
	}
}