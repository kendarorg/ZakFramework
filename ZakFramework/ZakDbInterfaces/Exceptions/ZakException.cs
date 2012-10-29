using System;

namespace ZakDb.Exceptions
{
	public class ZakException : Exception
	{
		public ZakException(string message)
			: base(message)
		{
		}

		public ZakException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}