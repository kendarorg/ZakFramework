using System;
using System.ComponentModel;
using System.Diagnostics;
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


		protected BaseThread(ILogger logger, String threadName, bool restartOnError = true)
		{
			_cyclesRun = 0;
			_threadCounter = 0;
			ContinueRuning = false;
			ThreadName = threadName.ToUpper();
			_threadCounter++;
			ThreadId = _threadCounter;
			_logger = logger;
			_restartOnError = restartOnError;
		}

		public ILogger Logger
		{
			get { return _logger; }
		}

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
		private Int64 _status = (Int64)RunningStatus.None;

		/// <summary>
		/// Status of the object.
		/// </summary>
		public RunningStatus Status
		{
			get { return (RunningStatus)Interlocked.Read(ref _status); }
			private set { Interlocked.Exchange(ref _status, (Int64)value); }
		}

		public bool ContinueRuning
		{
			get { return Interlocked.Read(ref _continueRunning)==1; }
			protected set { Interlocked.Exchange(ref _continueRunning, value ? 1 : 0); }
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
		public virtual void RunThread(int timeoutMs = 0)
		{
			_thread = new Thread(StaticRunThread);
			IntThread.Start(this);
			if (timeoutMs>1)
			{
				var sw = new Stopwatch();
				sw.Start();
				while (Status != RunningStatus.Running && sw.ElapsedMilliseconds < timeoutMs) Thread.Sleep(timeoutMs/10);
				sw.Stop();
			}
		}


		/// <summary>
		/// Internal function to run the whole thing
		/// </summary>
		/// <param name="par"></param>
		private static void StaticRunThread(Object par)
		{
			var b = (BaseThread)par;
			var exceptionThrown = false;
			var threadAborted = false;
			var shouldStop = false;

			try
			{
				b.Status = RunningStatus.None;
				b.Initialize();
				b.Status = RunningStatus.Initialized;
				b.RunInternal();
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
				b._exceptions.Enqueue(new Exception("ThreadAbortException"));
				b.Status = RunningStatus.Aborted;
				threadAborted = true;
				shouldStop = true;
			}
			catch (Exception ex)
			{
				var continueRunningAfterException = b.HandleException(ex);
				b._exceptions.Enqueue(ex);
				exceptionThrown = true;
				b.Status = RunningStatus.ExceptionThrown;
				if (!continueRunningAfterException || !b._restartOnError)
				{
					shouldStop = true;
				}
			}

			if (shouldStop || !b.ContinueRuning) 
			{
				b.ContinueRuning = false;
				if (threadAborted || exceptionThrown) return;
				b.Status = RunningStatus.Halting;
				try
				{
					b.CleanUp();
					b.Status = RunningStatus.Halted;
					b._exceptions = new LockFreeQueue<Exception>();
				}
				catch (Exception tae)
				{
					b.Status = RunningStatus.AbortedOnCleanup;
					b.HandleException(tae);
					b._exceptions.Enqueue(tae);
				}

				return;
			}
			b.ContinueRuning = false;
			b._thread = new Thread(StaticRunThread);
			b.IntThread.Start(b);
		}

		/// <summary>
		/// The lifecycle of the object
		/// </summary>
		private void RunInternal()
		{
			ContinueRuning = true;
			Status = RunningStatus.Running;
			var sw = new Stopwatch();
 			sw.Start();
			while (ContinueRuning)
			{
				var start = sw.ElapsedMilliseconds;
				if (!CyclicExecution())
				{
					ContinueRuning = false;
					break;
				}
				if (_cyclesRun == int.MaxValue) _cyclesRun = 0;
				_cyclesRun++;
				if ((sw.ElapsedMilliseconds - start) < 2)
				{
					Thread.Sleep(2);
				}
				else
				{
					Thread.Sleep(1);
				}
			}
			Status = RunningStatus.Halting;
		}

		private volatile int _cyclesRun;

		public int CyclesRun { get { return _cyclesRun; } }

		/// <summary> 
		/// Terminate the thread
		/// </summary>
		/// <param name="force">If true, abort!!!</param>
		public virtual void Terminate(Boolean force = false)
		{
			ContinueRuning = false;
			if (IntThread == null)
			{
				Status = RunningStatus.Halted;
				return;
			}
			if (force && IntThread.IsAlive)
			{
				IntThread.Abort();
				Status = RunningStatus.Aborted;
				return;
			}
			if (Status == RunningStatus.ExceptionThrown && !_restartOnError)
			{
				return;
			}
			if (Status != RunningStatus.Running && Status != RunningStatus.Halting)
			{
				return;
			}
			Status = RunningStatus.Halting;
		}

		/// <summary>
		/// Wait for the thread to terminate
		/// </summary>
		/// <param name="timeoutMs">ms to wait for the thread termination.</param>
		public void WaitTermination(Int64 timeoutMs)
		{
			var tmp = Status;
			if (tmp == RunningStatus.Running)
			{
				throw new InvalidAsynchronousStateException("Thread not halting");
			}
			if (tmp == RunningStatus.Halted || tmp == RunningStatus.ExceptionThrown || 
				tmp== RunningStatus.Aborted || tmp == RunningStatus.AbortedOnCleanup)
			{
				return;
			}
			DateTime timeoutTime = DateTime.UtcNow;
			timeoutTime = timeoutTime.AddMilliseconds(timeoutMs);

			int waitCycle = (int)timeoutMs / 100;
			if (waitCycle < 2) waitCycle = 2;

			while (timeoutTime > DateTime.UtcNow)
			{
				if (!IntThread.IsAlive)
				{
					break;
				}
				Thread.Sleep(waitCycle);
			}
			if (IntThread.IsAlive) throw new TimeoutException();
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

		//#if DEBUG
		internal Thread IntThread
		{
			get { return _thread; }
		}
		//#endif

		public virtual void Dispose()
		{
			Terminate(true);
			ThreadId = 0;
			ThreadName = null;
			Thread.Sleep(100);
			_logger = null;
			_exceptions.Clear();
			_thread = null;
		}
	}
}