using System;
using ZakThread.Threading;
using ZakThread.Async;

namespace _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals
{
	/// <summary>
	/// The message that will be used to communicate between the various threads
	/// </summary>
	internal class ConcurrentTreeMessage : BaseRequestObject
	{
		public ConcurrentTreeMessage(ConcurrentTreeMessageTypes messageType, params object[] parameters)
		{
			TimeStamp = DateTime.Now;
			MessageType = messageType;
			Parameters = parameters;
		}

		public DateTime TimeStamp { get; set; }
		public ConcurrentTreeMessageTypes MessageType { get; private set; }
		public object[] Parameters { get; private set; }

		public string SourceThread { get; set; }
	}
}