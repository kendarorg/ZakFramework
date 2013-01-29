using ZakTestUtils;
using ZakThread.Test.Threading.Simple;

namespace ZakThread.Test.Async.SampleObjects
{
	class SyncTaskHandlerWithMessageRegistration : SampleAsyncTasksHandler
	{
		public SyncTaskHandlerWithMessageRegistration(string threadName, int waitTimeMs,
		int batchSize = 0, int batchTimeoutMs = 0) :
			base(threadName, waitTimeMs, batchSize, batchTimeoutMs)
		{
			MessagesCount = new CounterContainer();
		}


		public CounterContainer MessagesCount { get; set; }

		public override void RegisterMessages()
		{
			RegisterMessage(typeof(TestMessage));
			base.RegisterMessages();
		}

		protected override bool HandleMessage(ZakThread.Threading.IMessage msg)
		{
			if (msg is TestMessage)
			{
				MessagesCount.Increment();
			}
			return true;
		}

	}
}
