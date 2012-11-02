using System;
using ZakThread.Threading;
namespace _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals
{
	/// <summary>
	/// The message that will be used to communicate between the various threads
	/// </summary>
	internal class ConcurrentTreeMessage:IMessage
	{
		public ConcurrentTreeMessage(ConcurrentTreeMessageTypes messageType,params object[] parameters)
		{
			Id = Guid.NewGuid();
			TimeStamp = DateTime.Now;
			MessageType = messageType;
			Parameters = parameters;
		}

		public Guid Id { get; set; }
		public DateTime TimeStamp { get; set; }
		public ConcurrentTreeMessageTypes MessageType { get; private set; }
		public object[] Parameters { get; private set; }
	}
}
