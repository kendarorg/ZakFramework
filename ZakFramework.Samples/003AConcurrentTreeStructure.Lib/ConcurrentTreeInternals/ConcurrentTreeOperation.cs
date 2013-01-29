using System;
using ZakThread.Async;

namespace _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals
{
	/// <summary>
	/// The message that will be used to communicate between the various threads
	/// </summary>
	internal class ConcurrentTreeOperation : BaseRequestObject
	{
		public ConcurrentTreeOperation(ConcurrentTreeOperationTypes messageType, params object[] parameters)
		{
			TimeStamp = DateTime.Now;
			MessageType = messageType;
			Parameters = parameters;
		}

		public DateTime TimeStamp { get; set; }
		public ConcurrentTreeOperationTypes MessageType { get; private set; }
		public object[] Parameters { get; private set; }

		public string SourceThread { get; set; }
	}
}