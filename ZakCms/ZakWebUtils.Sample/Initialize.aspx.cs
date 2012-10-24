using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.deltatre.common.drivers.mongodb;
using PubSubLoadSimulator.dal;
using PubSubLoadSimulator.bo;
using com.deltatre.common.utils;

namespace PubSubSimulator
{
	public partial class Initialize : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btInitialize_Click(object sender, EventArgs e)
		{
			MongoDb md = new MongoDb();
			int errorCode = 0;
			md.Initialize("mongodb://localhost:27017");
			md.Connect("BigStuff", out errorCode);

			UserDal udal = new UserDal(md);
			VerbDal vdal = new VerbDal(md);
			ApplicationDal adal = new ApplicationDal(md);

			vdal.Insert(new Verb() { Id = GuidBuilder.GenerateGuid(0), VerbName = "LIKE" },out errorCode);
			vdal.Insert(new Verb() { Id = GuidBuilder.GenerateGuid(1), VerbName = "COMMENT" }, out errorCode);
			vdal.Insert(new Verb() { Id = GuidBuilder.GenerateGuid(2), VerbName = "PHOTO" }, out errorCode);
			vdal.Insert(new Verb() { Id = GuidBuilder.GenerateGuid(3), VerbName = "NEWS" }, out errorCode);
			vdal.Insert(new Verb() { Id = GuidBuilder.GenerateGuid(4), VerbName = "FRIEND" }, out errorCode);
			vdal.Insert(new Verb() { Id = GuidBuilder.GenerateGuid(5), VerbName = "ADD" }, out errorCode);

			adal.Insert(new Application() { Id = GuidBuilder.GenerateGuid(0), ApplicationId = "FIFA" }, out errorCode);
			adal.Insert(new Application() { Id = GuidBuilder.GenerateGuid(1), ApplicationId = "UMOBO" }, out errorCode);
			adal.Insert(new Application() { Id = GuidBuilder.GenerateGuid(2), ApplicationId = "FANTASY FOOTBALL" }, out errorCode);
			adal.Insert(new Application() { Id = GuidBuilder.GenerateGuid(3), ApplicationId = "FANTASY GOLF" }, out errorCode);

			for (int i = 0; i < 1000; i++)
			{
				udal.Insert(new User() { Id = GuidBuilder.GenerateGuid(i) }, out errorCode);
			}
		}
	}
}