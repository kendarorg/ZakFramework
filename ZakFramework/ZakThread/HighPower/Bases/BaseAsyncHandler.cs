using System;
using System.Collections.Generic;
using ZakThread.HighPower.Interfaces;

namespace ZakThread.HighPower.Bases
{
	public abstract class BaseAsyncHandler : IAsyncHandler
	{
		protected virtual AsyncTask GenerateTask(AsyncCallback callback, Object tag, Object state, BaseAsyncHandler ash)
		{
			return new AsyncTask(callback, tag, state, ash);
		}

		public IAsyncResult RunAsyncOperation(AsyncCallback callback, Object tag, Object state, int timeoutMillisec = 0)
		{
			AsyncTask asynch = GenerateTask(callback, tag, state, this);

			if (asynch.AsyncWaitHandle != null)
			{
				asynch.AsyncWaitHandle.WaitOne(timeoutMillisec, false);
			}
			else
			{
				asynch.StartAsyncWork();
			}
			return asynch;
		}

		/// <summary>
		/// This will be implemented by the ones that will actually do something on the objects.
		/// This one is executed in batch or in single mode
		/// </summary>
		/// <param name="asyncTask"></param>
		public abstract bool ExecuteAsyncProcessing(AsyncTask asyncTask);

		/// <summary>
		/// This function will (if needed) finalize the whole batch of data elaborated
		/// This function is execute in batch only
		/// </summary>
		/// <param name="atl"></param>
		public abstract void FinalizeBatchElements(List<AsyncTask> atl);
	}
}