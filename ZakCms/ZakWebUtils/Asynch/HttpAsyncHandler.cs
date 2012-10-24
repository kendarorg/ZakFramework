using System;
using System.Web;
using ZakThread.HighPower;
using ZakThread.HighPower.Bases;

namespace ZakWeb.Utils.Asynch
{
	public abstract class HttpAsyncHandler : BaseAsyncHandler, IHttpAsyncHandler
	{
		public bool IsReusable { get { return true; } }

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, Object extraData)
		{
			return RunAsyncOperation(cb, context, extraData);
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			var asop = (AsyncTask)result;
			((HttpContext)asop.Tag).Response.End();
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new InvalidOperationException();
		}
	}

}
