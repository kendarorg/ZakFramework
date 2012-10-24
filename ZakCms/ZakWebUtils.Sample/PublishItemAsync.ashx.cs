using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.deltatre.common.web.asynch;
using System.Web.Script.Serialization;
using PubSubLoadSimulator.bo;
using com.deltatre.common.drivers.mongodb;
using PubSubLoadSimulator.dal;
using PubSubSimulator.src;

namespace PubSubSimulator
{
	/// <summary>
	/// Summary description for PublishItemAsync
	/// </summary>
	public class PublishItemAsync : HttpAsyncHandler
	{
		[ThreadStatic]
		JavaScriptSerializer _jss = new JavaScriptSerializer();

		[ThreadStatic]
		SimpleActivityStreamListDal _mmdal = new SimpleActivityStreamListDal(SetupDb());

		[ThreadStatic]
		SimpleActivityStreamDal _mdal = new SimpleActivityStreamDal(SetupDb());

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
		UserDal _udal = new UserDal(SetupDb());

		[ThreadStatic]
		VerbDal _vdal = new VerbDal(SetupDb());

		[ThreadStatic]
		ApplicationDal _adal = new ApplicationDal(SetupDb());

		public override bool ExecuteAsyncProcessing(com.deltatre.common.hp.AsyncTask asyncTask)
		{
			int errorCode;
			PubSubTask asop = (PubSubTask)asyncTask;
			HttpContext ctx = (HttpContext)asop.Tag;
			byte[] bt = new byte[ctx.Request.InputStream.Length];
			ctx.Request.InputStream.Read(bt, 0, bt.Length);
			string s = ctx.Request.ContentEncoding.GetString(bt);
			JavaScriptSerializer jss = new JavaScriptSerializer();
			SimpleActivityStream msg = jss.Deserialize<SimpleActivityStream>(s);

			if (_vdal.GetById(msg.AI, out errorCode) == null || _adal.GetById(msg.SA, out errorCode) == null || _udal.GetById(msg.SI, out errorCode) == null)
			{
				asop.Result = null;
				asop.CompleteTask();
				return true;
			}

			MongoDb md = new MongoDb();
			SimpleActivityStreamDal psmd = new SimpleActivityStreamDal(md);
			psmd.Insert(msg, out errorCode);
			if(errorCode==0 )
				asop.Result="";
			asop.CompleteTask();
			return true;
		}

		public override void FinalizeBatchElements(List<com.deltatre.common.hp.AsyncTask> atl)
		{
			
		}
	}
}