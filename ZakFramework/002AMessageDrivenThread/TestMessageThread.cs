using ZakThread.Logging;
using ZakThread.Threading;

namespace _002AMessageDrivenThread
{
	internal class TestMessageThread : BaseMessageThread
	{
		public TestMessageThread(string threadName, bool restartOnError = true) :
			base(NullLogger.Create(), threadName, restartOnError)
		{
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