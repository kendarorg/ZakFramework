using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.deltatre.common.web.asynch;
using com.deltatre.common.drivers.mongodb;
using PubSubSimulator.src;
using PubSubLoadSimulator.dal;
using com.deltatre.common.dal;
using PubSubLoadSimulator.bo;
using System.Web.Script.Serialization;

namespace PubSubSimulator
{
	/// <summary>
	/// Summary description for ReadItemds
	/// </summary>
	public class ReadItemds : HttpAsyncHandler
	{
		private static com.deltatre.common.interfaces.IDatabase SetupDb()
		{
			int errorCode = 0;
			MongoDb md = new MongoDb();
			try
			{
				md.Initialize("mongodb://localhost:27017");
				//	md.CreateDatabase("BigStuff");
				md.Connect("BigStuff", out errorCode);

			}
			catch { }
			return md;
		}
		[ThreadStatic]
		SimpleActivityStreamDal _mdal = new SimpleActivityStreamDal(SetupDb());

		[ThreadStatic]
		UserDal _udal = new UserDal(SetupDb());

		[ThreadStatic]
		JavaScriptSerializer _jss = new JavaScriptSerializer();

		public override bool ExecuteAsyncProcessing(com.deltatre.common.hp.AsyncTask asyncTask)
		{
			int errorCode;
			HttpContext ctx = (HttpContext)asyncTask.Tag;
			string guid = ctx.Request.Params["userId"];
			Guid gg = Guid.Parse(guid);
			if (_udal.GetById(gg, out errorCode) == null)
			{
				ctx.Response.StatusCode = 404;
				asyncTask.CompleteTask();
				return true;
			}

			List<SimpleActivityStream> sas = new List<SimpleActivityStream>(_mdal.Search(BQuery.EQ("SI", gg), 0, 10));
			
			
			string msg = _jss.Serialize(sas.ToArray());

			ctx.Response.StatusCode = 200;
			ctx.Response.Write(msg);
			asyncTask.CompleteTask();
			return true;
		}

		public override void FinalizeBatchElements(List<com.deltatre.common.hp.AsyncTask> atl)
		{
			
		}
	}
}