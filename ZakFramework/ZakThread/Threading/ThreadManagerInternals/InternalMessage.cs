using System;

namespace ZakThread.Threading.ThreadManagerInternals
{
	public class InternalMessage : IMessage
	{
		public InternalMessage(InternalMessageTypes messageType, object content)
		{
			Id = Guid.NewGuid();
			TimeStamp = DateTime.UtcNow;
			MessageType = messageType;
			Content = content;
			SourceThread = null;
		}

		public String SourceThread { get; set; }
		public Guid Id { get; set; }
		public DateTime TimeStamp { get; set; }
		public object Content { get; set; }
		public InternalMessageTypes MessageType { get; set; }

		public object Clone()
		{
			var msg = new InternalMessage(MessageType, Content)
				{
					SourceThread = SourceThread, 
					TimeStamp = TimeStamp, 
					Id = Id
				};
			return msg;
		}
	}
}