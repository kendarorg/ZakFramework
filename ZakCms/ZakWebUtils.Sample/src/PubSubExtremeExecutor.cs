using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.deltatre.common.web.asynch;
using System.Threading;
using PubSubLoadSimulator.bo;
using PubSubLoadSimulator.dal;
using com.deltatre.common.drivers.mongodb;
using System.Text;
using System.Web.Script.Serialization;
using com.deltatre.common.dal;
using com.deltatre.common.hp;

namespace PubSubSimulator.src
{
	public class PubSubExtremeExecutor:AsyncQueuedExecutor
	{
		
		public PubSubExtremeExecutor():base("XXX")
		{
		}

		protected int MaxMessagesPerSecond
		{
			get { return 3500; }
		}

		protected int MaxMillisecResponseTime
		{
			get { return 600; }
		}

		
		

		
		protected override bool IsBatch
		{
			get { return true; }
		}

		protected override bool CheckIfShouldStop(AsyncTask at, long msSinceStart, int msgsElaborated)
		{
			if(msSinceStart>MaxMillisecResponseTime || msgsElaborated >MaxMessagesPerSecond)
				return false;
			return true;
		}

	}
}