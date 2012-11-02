using System;
using System.Diagnostics;
using System.Threading;
using ZakThread.Threading.Enums;

namespace _001_A_simple_thread
{
	class Program
	{
// ReSharper disable UnusedParameter.Local
		static void Main(string[] args)
// ReSharper restore UnusedParameter.Local
		{
			var stopwatch = new Stopwatch();
			var stopwatchExternal = new Stopwatch();
			var testThread = new TestThread(stopwatch, "TestThread");

			testThread.RunThread();
			stopwatchExternal.Start();
			while (testThread.Status != RunningStatus.Halted)
			{
				Thread.Sleep(5);
			}
			stopwatchExternal.Stop();
			Console.WriteLine(
				string.Format(
					"Completed in {0} ms, lifecycle was of {1} ms.",
					stopwatch.ElapsedMilliseconds,
					stopwatchExternal.ElapsedMilliseconds));
			//Just to avoid the app closing...
			Console.ReadKey();
		}
	}
}