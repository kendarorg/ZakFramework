using System;
using ZakThread.Threading;

namespace ZakThread.Test.Threading.Simple
{
	public class TestMessage : IMessage
	{
		public TestMessage()
		{
			Id = Guid.NewGuid();
			TimeStamp = DateTime.UtcNow;
			SourceThread = null;
		}

		public String SourceThread { get; set; }
		public Guid Id { get; set; }
		public DateTime TimeStamp { get; set; }


		public object Clone()
		{
			var msg = new TestMessage
			{
				SourceThread = SourceThread,
				TimeStamp = TimeStamp,
				Id = Id
			};
			return msg;
		}
	}
}