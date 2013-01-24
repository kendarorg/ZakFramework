using ZakThread.Async;

namespace ZakThread.Test.Async.SampleObjects
{
	public class RequestObject : BaseRequestObject
	{
		private readonly long _requestId;
		public long RequestId { get { return _requestId; } }
		public RequestObject(long requestId)
		{
			_requestId = requestId;
		}
	}
}