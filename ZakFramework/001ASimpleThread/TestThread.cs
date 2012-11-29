using System;
using System.Diagnostics;
using ZakCore.Utils.Logging;
using ZakThread.Logging;
using ZakThread.Threading;

namespace _001_A_simple_thread
{
	public class TestThread : BaseThread
	{
		private readonly Stopwatch _stopwatch;
		private int _counter;
		private const int MAX_COUNT = 100;

		public TestThread(Stopwatch stopwatch, string threadName, bool restartOnError = true) :
			base(NullLogger.Create(), threadName, restartOnError)
		{
			_stopwatch = stopwatch;
			_counter = 0;
		}

		//Overridden, returns true when all goes well
		protected override bool RunSingleCycle()
		{
			if (_counter == 0) _stopwatch.Start();
			Console.WriteLine(string.Format("Executing thread {0} for the {1}th time.", ThreadId, _counter));
			_counter++;
			if (_counter >= MAX_COUNT)
			{
				_stopwatch.Stop();
				Terminate();
			}
			return true;
		}
	}
}