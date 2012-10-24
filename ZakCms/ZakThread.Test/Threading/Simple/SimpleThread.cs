using System;
using System.Threading;
using ZakThread.Threading;
using ZakThread.Logging;

namespace ZQueue.Test.Threading.Simple
{
	public class SimpleThread : BaseThread
	{
		private readonly int _sleepTime;

		public bool IsInitialized { get; private set; }

		public bool IsCleanedUp { get; private set; }

		public bool IsExceptionHandled { get; private set; }

		public bool ResetExceptionAfterThrow { get; set; }

		public Exception ThrowExceptionOnCyclicExecution { set; get; }

		public Exception ThrowExceptionOnInitialization { set; get; }

		public Exception ThrowExceptionOnCleanUp { set; get; }

		public SimpleThread(int sleepTime, string threadName, bool restartOnError=true)
			: base(NullLogger.Create(), threadName, restartOnError)
		{
			ThrowExceptionOnInitialization = null;
			ThrowExceptionOnCyclicExecution = null;
			ThrowExceptionOnCleanUp = null;
			ResetExceptionAfterThrow = false;
			IsInitialized = false;
			IsCleanedUp = false;
			IsExceptionHandled = false;
			_sleepTime = sleepTime;
		}

		protected override bool RunSingleCycle()
		{
			if (ThrowExceptionOnCyclicExecution != null)
			{ 
				Exception toThrow = ThrowExceptionOnCyclicExecution;
				if (ResetExceptionAfterThrow) ThrowExceptionOnCyclicExecution = null;
				if (toThrow != null) throw toThrow;
			}

			Thread.Sleep(_sleepTime);
			return true;
		}

		protected override void Initialize()
		{
			if (ThrowExceptionOnInitialization != null) throw ThrowExceptionOnInitialization;
			IsInitialized = true;
		}

		protected override bool HandleException(Exception ex)
		{
			IsExceptionHandled = true;
			return true;
		}

		protected override void CleanUp()
		{
			if (ThrowExceptionOnCleanUp != null) throw ThrowExceptionOnCleanUp;
			IsCleanedUp = true;
		}
	}
}
