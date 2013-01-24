﻿namespace ZakThread.Test.Async.Utils
{
	public class BaseRequestObject
	{
		public object Return { get; set; }

		public T GetReturnAs<T>()
		{
			if (Return == null) return default(T);
			return (T) Return;
		}
	}
}