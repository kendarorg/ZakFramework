using System;
using System.Collections.Generic;

namespace ZakThread.HighPower.Interfaces
{
	public interface IAsyncHandler
	{
		bool ExecuteAsyncProcessing(AsyncTask asyncTask);
		IAsyncResult RunAsyncOperation(AsyncCallback callback, Object tag, Object state, int timeoutMillisec = 0);
		void FinalizeBatchElements(List<AsyncTask> atl);
	}
}