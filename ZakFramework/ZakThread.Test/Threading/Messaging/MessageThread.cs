
using System.Threading;
using ZakCore.Utils.Logging;
using ZakThread.Test.Threading.Simple;
using ZakThread.Threading;

namespace ZakThread.Test.Threading.Messaging
{
	public class MessageThread : BaseMessageThread
	{
		private readonly bool _doRegistration;
		private long _testMessagesCount;
		private long _sendMessages;
		private long _terminateMessage;

		public long MessagesCount { get { return Interlocked.Read(ref _testMessagesCount); } }

		public long SendingMessage
		{
			get { return Interlocked.Read(ref _sendMessages); }
			set { Interlocked.Exchange(ref _sendMessages, value); }
		}

		public long TerminateMessage
		{
			get { return Interlocked.Read(ref _terminateMessage); }
			set { Interlocked.Exchange(ref _terminateMessage, value); }
		}

		public MessageThread(bool doRegistration, ILogger logger, string threadName, bool restartOnError = true) :
			base(logger, threadName, restartOnError)
		{
			_doRegistration = doRegistration;
		}

		protected override bool RunSingleCycle()
		{
			if (SendingMessage > 0)
			{
				SendMessage(new TestMessage());
				Interlocked.Decrement(ref _sendMessages);
			}
			Thread.Sleep(10);

			if (TerminateMessage != 0)
			{
				TerminateMessage = 0;
				Terminate();
			}
			return true;
		}

		protected override bool HandleMessage(IMessage msg)
		{
			if (msg is TestMessage)
			{
				Interlocked.Increment(ref _testMessagesCount);
			}
			return true;
		}

		public override void RegisterMessages()
		{
			if (_doRegistration)
			{
				RegisterMessage(typeof(TestMessage));
			}
		}

		internal void SetMaxMessagePerCycle(int p)
		{
			MaxMesssagesPerCycle = p;
		}
	}
}
