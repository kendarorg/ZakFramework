using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using PubSubLoadSimulator.bo;
using com.deltatre.common.drivers.mongodb;
using PubSubLoadSimulator.dal;

namespace PubSubSimulator
{
	/// <summary>
	/// Summary description for PublishItemStandard
	/// </summary>
	public class PublishItemStandard : IHttpHandler
	{

		public void ProcessRequest(HttpContext ctx)
		{
			MongoDb md = new MongoDb();
			int errorCode = 0;
			md.Initialize("mongodb://localhost:27017");
			md.Connect("BigStuff", out errorCode);

			UserDal udal = new UserDal(md);
			VerbDal vdal = new VerbDal(md);
			ApplicationDal adal = new ApplicationDal(md);
			
			byte[] bt = new byte[ctx.Request.InputStream.Length];
			ctx.Request.InputStream.Read(bt, 0, bt.Length);
			string s = ctx.Request.ContentEncoding.GetString(bt);
			JavaScriptSerializer jss = new JavaScriptSerializer();
			SimpleActivityStream msg = jss.Deserialize<SimpleActivityStream>(s);

			if (vdal.GetById(msg.AI, out errorCode) == null || adal.GetById(msg.SA, out errorCode) == null || udal.GetById(msg.SI, out errorCode) == null)
			{
				ctx.Response.StatusCode = 404;
				return;
			}
			SimpleActivityStreamDal psmmd = new SimpleActivityStreamDal(md);
			psmmd.Insert(msg, out errorCode);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}