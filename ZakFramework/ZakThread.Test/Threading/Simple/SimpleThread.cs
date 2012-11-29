using System;
using System.Threading;
using ZakCore.Utils.Logging;
using ZakThread.Threading;

namespace ZakThread.Test.Threading.Simple
{
	public class SimpleThread : BaseThread
	{
		private readonly int _sleepTime;

		public bool IsInitialized { get; private set; }

		public bool IsCleanedUp { get; private set; }

		public bool IsExceptionHandled { get; private set; }
		public bool ExitAfterFirstCycle { private get; set; }

		public bool ResetExceptionAfterThrow { private get; set; }

		public Exception ThrowExceptionOnCyclicExecution { set; get; }

		public bool ThrowThreadAbortException { set; private get; }

		public Exception ThrowExceptionOnInitialization { set; get; }

		public Exception ThrowExceptionOnCleanUp { set; get; }

		public SimpleThread(int sleepTime, string threadName, bool restartOnError = true)
			: base(NullLogger.Create(), threadName, restartOnError)
		{
			ThrowThreadAbortException = false;
			ExitAfterFirstCycle = false;
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
			if (ExitAfterFirstCycle) return false;
			if (ThrowExceptionOnCyclicExecution != null)
			{
				Exception toThrow = ThrowExceptionOnCyclicExecution;
				if (ResetExceptionAfterThrow) ThrowExceptionOnCyclicExecution = null;
				if (toThrow != null) throw toThrow;
			}
			if (ThrowThreadAbortException)
			{
				Thread.Sleep(10000);
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
			base.HandleException(ex);
			IsExceptionHandled = true;
			return true;
		}

		protected override void CleanUp()
		{
			base.CleanUp();
			if (ThrowExceptionOnCleanUp != null) throw ThrowExceptionOnCleanUp;
			IsCleanedUp = true;
		}
	}
}