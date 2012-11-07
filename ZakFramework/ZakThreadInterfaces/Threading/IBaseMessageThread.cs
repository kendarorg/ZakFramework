using System.Collections.Generic;

namespace ZakThread.Threading
{
	public interface IBaseMessageThread : IBaseThread
	{
		void SendMessageToThread(IMessage msg);
		IMessage PeekMessageFromThread();
		IEnumerable<IMessage> PeekMessagesFromThread();
		IThreadManager Manager { get; set; }
		void RegisterMessages();
	}
}