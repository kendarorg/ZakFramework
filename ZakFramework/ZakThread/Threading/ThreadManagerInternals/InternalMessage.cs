using System;

namespace ZakThread.Threading.ThreadManagerInternals
{
	public class InternalMessage : IMessage
	{
		public InternalMessage(InternalMessageTypes messageType, object content)
		{
			Id = Guid.NewGuid();
			TimeStamp = DateTime.Now;
			MessageType = messageType;
			Content = content;
		}

		public Guid Id { get; set; }
		public DateTime TimeStamp { get; set; }
		public object Content { get; set; }
		public InternalMessageTypes MessageType { get; set; }
	}
}
