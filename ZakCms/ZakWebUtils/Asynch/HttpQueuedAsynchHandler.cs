using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using ZakThread.HighPower;
using ZakThread.HighPower.Bases;

namespace ZakWeb.Utils.Asynch
{
	public abstract class HttpQueuedAsynchHandler : BaseAsyncHandler, IHttpAsyncHandler
	{
		public bool IsReusable { get { return true; } }

		public abstract AsyncQueuedExecutor[] StaticAsyncExecutors { get; }

		protected static int MaxWorkerThread
		{
			get
			{
				var swg = (ProcessModelSection)ConfigurationManager.GetSection("system.web/processModel");
				return swg.MaxWorkerThreads;
			}
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			var asop = (AsyncTask)RunAsyncOperation(cb, context, extraData);
			int count = StaticAsyncExecutors.Length;
			var what = (int)(DateTime.Now.Ticks % count);
			asop.Executor = StaticAsyncExecutors[what];
			StaticAsyncExecutors[what].EnqueTask(asop);	
			return asop;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			var asop = (AsyncTask)result;
			((HttpContext)asop.Tag).Response.End();
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new NotImplementedException();
		}
	}
}
