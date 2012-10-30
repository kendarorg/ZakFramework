
using ZakThread.Threading;
using System.Diagnostics;
using ZakThread.Logging;
using System;

namespace _002AMessageDrivenThread
{
	class TestMessageThread: BaseMessageThread
	{
		private Stopwatch _stopwatch;

		public TestMessageThread(Stopwatch stopwatch, string threadName,bool restartOnError=true):
		 base(NullLogger.Create(),threadName,restartOnError)
		{
			_stopwatch = stopwatch;
		}
		protected override bool HandleMessage(IMessage msg)
		{
			SendMessage(msg);
			return true;
		}

		public override void RegisterMessages()
		{
			
		}

		protected override bool RunSingleCycle()
		{
			//Should do nothing
			return true;
		}
	}
}
