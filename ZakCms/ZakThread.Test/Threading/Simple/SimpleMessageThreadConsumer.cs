using System;
using ZQueue.Threading;
using ZQueue.Threading.Model;

namespace ZQueue.Test.Threading.Simple
{
	public class SimpleMessageThreadConsumer : SimpleThread, IMessageThreadBehaviour
	{
		public Int64 HandledMessages;

		public Exception ThrowExceptionOnMessageHandling { get; set; }

		public SimpleMessageThreadConsumer(int sleepTime)
			:base(sleepTime)
		{
		}

		public bool HandleMessage(Message msg)
		{
			HandledMessages++;
			if (ThrowExceptionOnMessageHandling!=null) throw ThrowExceptionOnMessageHandling;
			return true;
		}

		public IMessageThread Manager { get; set; }
	}
}
