using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using com.deltatre.common.drivers.mongodb;
using PubSubLoadSimulator.bo;
using PubSubSimulator.src;

namespace PubSubSimulator
{
	public class Global : System.Web.HttpApplication
	{
		public static BatchInserter _batchinsert;
		void Application_Start(object sender, EventArgs e)
		{
			MongoDb md = new MongoDb();
			try
			{
				int errorCode = 0;
				md.Initialize("mongodb://localhost:27017");
				//md.CreateDatabase("BigStuff");
				md.Connect("BigStuff", out errorCode);
				md.CreateTable(typeof(Application));
				md.CreateTable(typeof(SimpleActivityStream));
				md.CreateTable(typeof(SimpleActivityStreamList));
				md.CreateTable(typeof(User));
				md.CreateTable(typeof(Verb));

			}
			catch { }
			_batchinsert = new BatchInserter();
			_batchinsert.RunThread(true);
			System.Net.ServicePointManager.DefaultConnectionLimit = 100000;
			// Code that runs on application startup
			foreach (var k in PublishItem._singleExecutor)
			{
				k.RunThread(true);
			}

		}

		void Application_End(object sender, EventArgs e)
		{
			// Code that runs on application startup
			foreach (var k in PublishItem._singleExecutor)
			{
				k.Terminate(true);
			}
			_batchinsert.Terminate(true);
		}

		void Application_Error(object sender, EventArgs e)
		{
			// Code that runs when an unhandled error occurs

		}

		void Session_Start(object sender, EventArgs e)
		{
			// Code that runs when a new session is started

		}

		void Session_End(object sender, EventArgs e)
		{
			// Code that runs when a session ends. 
			// Note: The Session_End event is raised only when the sessionstate mode
			// is set to InProc in the Web.config file. If session mode is set to StateServer 
			// or SQLServer, the event is not raised.

		}

	}
}
