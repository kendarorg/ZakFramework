using System;

namespace _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals
{
	public class ConcurrentTreeMessageResult
	{
		public ConcurrentTreeMessageResult()
		{
			Result = null;
			Success = false;
		}

		public object Result { get; set; }
		public Exception ExceptionToThrow { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }
	}
}
