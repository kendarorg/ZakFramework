using System;
using System.Threading;
using ZQueue.Threading;

namespace ZQueue.Test.Threading.Simple
{
	public class SimpleThread : IStandardThreadBehaviour
	{
		public string ThreadName { get; set; }

		public bool RestartOnError { get; set; }

		private readonly int _sleepTime;

		public bool IsInitialized { get; private set; }

		public bool IsCleanedUp { get; private set; }

		public bool IsExceptionHandled { get; private set; }

		public bool ResetExceptionAfterThrow { get; set; }

		public Exception ThrowExceptionOnCyclicExecution { set; get; }

		public Exception ThrowExceptionOnInitialization { set; get; }

		public Exception ThrowExceptionOnCleanUp { set; get; }

		public SimpleThread(int sleepTime)
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

		public bool CyclicExecution()
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

		public void Initialize()
		{
			if (ThrowExceptionOnInitialization != null) throw ThrowExceptionOnInitialization;
			IsInitialized = true;
		}

		public bool HandleException(Exception ex)
		{
			IsExceptionHandled = true;
			return true;
		}

		public void CleanUp()
		{
			if (ThrowExceptionOnCleanUp != null) throw ThrowExceptionOnCleanUp;
			IsCleanedUp = true;
		}

		public void ForceTermination()
		{
			
		}
	}
}
