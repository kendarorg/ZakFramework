using System;

namespace ZakThread.Threading
{
	public interface IMessage: ICloneable
	{
		Guid Id { get; set; }
		DateTime TimeStamp { get; set; }
		String SourceThread { get; set; }
	}
}