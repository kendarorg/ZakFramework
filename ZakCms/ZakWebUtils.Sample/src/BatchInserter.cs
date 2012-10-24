//#define NO_DB 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.deltatre.common.threading;
using com.deltatre.common.collections;
using PubSubLoadSimulator.bo;
using System.Diagnostics;
using System.Threading;
using PubSubLoadSimulator.dal;
using com.deltatre.common.drivers.mongodb;
using com.deltatre.common.dal;
using System.Web.Configuration;
using System.Configuration;

namespace PubSubSimulator.src
{
	public class BatchInserter:BaseThread
	{
		public BatchInserter():base("BatchInserter")
		{

		}

		private static com.deltatre.common.interfaces.IDatabase SetupDb()
		{
			int errorCode = 0;
			MongoDb md = new MongoDb();
			try
			{
				md.Initialize("mongodb://localhost:27017");
				md.Connect("BigStuff", out errorCode);
			}
			catch { }
			return md;
		}

		SimpleActivityStreamListDal _mmdal = new SimpleActivityStreamListDal(SetupDb());

		SimpleActivityStreamDal _mdal = new SimpleActivityStreamDal(SetupDb());

		protected static int MaxWorkerThread
		{
			get
			{
				ProcessModelSection swg = (ProcessModelSection)ConfigurationManager.GetSection("system.web/processModel");
				return swg.MaxWorkerThreads;
			}
		}

		private LockFreeQueue<SimpleActivityStreamList>[] _inQueue = SetupLoFre();

		private static LockFreeQueue<SimpleActivityStreamList>[] SetupLoFre()
		{
			int re = MaxWorkerThread/4;
			if(re==0)re=1;
			LockFreeQueue<SimpleActivityStreamList>[] ll = new LockFreeQueue<SimpleActivityStreamList>[re];
			for (int i = 0; i < ll.Length;i++ )
			{
				ll[i] = new LockFreeQueue<SimpleActivityStreamList>();
			}
			return ll;
		}
		
		
		
		public void EnqueueList(SimpleActivityStreamList ten)
		{
			int queue = (int)(DateTime.Now.Ticks % _inQueue.Length);
			_inQueue[queue].Enqueue(ten);
		}

		protected override bool CyclicExecution()
		{
			int errorCode = 0;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			List<Guid> salist = new List<Guid>();
			List<SimpleActivityStream> sal = new List<SimpleActivityStream>();
			for(int i=0;i<_inQueue.Length;i++)
			{
				foreach (SimpleActivityStreamList sasl in _inQueue[i].Dequeue())
				{
					salist.Add(sasl.Id);
					sal.AddRange(sasl.GetExpanded());
					if (sal.Count > 3500 || sw.ElapsedMilliseconds > 1000)
					{
						break;
					}
				}
				if (sal.Count > 3500 || sw.ElapsedMilliseconds > 1000)
				{
					break;
				}
			}
#if !NO_DB
			if (sal.Count > 0)
			{
				_mdal.InsertBatch(sal, out errorCode, 15000, true);
			}
			if (salist.Count > 0)
			{
				_mmdal.Delete(BQuery.Or("Id", salist.ToArray()), out errorCode);
			}
#endif
			Thread.Sleep(0);
			Thread.Sleep(1);
			return true;
		}
	}
}