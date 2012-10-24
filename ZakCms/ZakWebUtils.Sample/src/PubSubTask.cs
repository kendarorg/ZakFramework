using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.deltatre.common.hp;
using PubSubLoadSimulator.bo;

namespace PubSubSimulator.src
{
	public class PubSubTask:AsyncTask
	{

		public PubSubTask(AsyncCallback callback, object tag, object state, BaseAsyncHandler ash):
		base(callback,tag,state,ash)
		{
		}

		public string Result{get;set;}
		public SimpleActivityStream Message {get;set;}

		public override void ExecuteCleanup()
		{
			if(Result == null)
				((HttpContext)Tag).Response.StatusCode = 404;
			else
				((HttpContext)Tag).Response.StatusCode = 200;
		}
	}
}