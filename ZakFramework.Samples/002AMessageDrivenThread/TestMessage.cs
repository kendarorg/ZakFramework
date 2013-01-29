using System;
using ZakThread.Threading;

namespace _002AMessageDrivenThread
{
	internal class TestMessage : IMessage
	{
		public TestMessage(Guid id, int runBlock)
		{
			Id = id;
			RunBlock = runBlock;
			TimeStamp = DateTime.Now;
		}

		public Guid Id { get; set; }
		public int RunBlock { get; set; }
		public DateTime TimeStamp { get; set; }

		public string SourceThread { get; set; }
		public object Clone()
		{
			var res = new TestMessage(Id, RunBlock) {TimeStamp = TimeStamp, SourceThread = SourceThread};
			return res;
		}
	}
}