using System;
using ZakThread.Threading;

namespace ZakThread.Test.Threading.Simple
{
	public class TestMessage : IMessage
	{
		public Guid Id { get; set; }

		public DateTime TimeStamp { get; set; }
	}
}