using System;
using ZakThread.Threading;

namespace _002AMessageDrivenThread
{
	class TestMessage : IMessage
	{
		public TestMessage(Guid id, int runBlocks)
		{
			Id = id;
			RunBlock = runBlocks;
			TimeStamp = DateTime.Now;
		}
		public Guid Id { get; set; }
		public int RunBlock { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
