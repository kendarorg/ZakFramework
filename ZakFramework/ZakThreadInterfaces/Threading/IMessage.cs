using System;

namespace ZakThread.Threading
{
	public interface IMessage
	{
		Guid Id { get; set; }
		DateTime TimeStamp { get; set; }
	}
}