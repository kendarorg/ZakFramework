using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.deltatre.common.web.asynch;
using PubSubSimulator.src;
using System.Web.Script.Serialization;
using PubSubLoadSimulator.bo;
using com.deltatre.common.hp;
using System.Text;
using PubSubLoadSimulator.dal;
using com.deltatre.common.drivers.mongodb;

namespace PubSubSimulator
{
	/// <summary>
	/// Summary description for PublishItem
	/// </summary>
	public class PublishItem : HttpQueuedAsynchHandler
	{
		protected override com.deltatre.common.hp.AsyncTask GenerateTask(AsyncCallback callback, object tag, object state, com.deltatre.common.hp.BaseAsyncHandler ash)
		{
			PubSubTask pst = new PubSubTask(callback, tag, state, ash);
			return pst;
		}
		public static AsyncQueuedExecutor[] _singleExecutor = InitializeExecutors();
		public static AsyncQueuedExecutor[] InitializeExecutors()
		{
			int res = MaxWorkerThread / 4;
			if (res == 0) res = 1;
			AsyncQueuedExecutor[] se = new AsyncQueuedExecutor[res];
			for (int i = 0; i < res; i++)
			{
				se[i] = new PubSubExtremeExecutor();
			}
			return se;
		}

		public override com.deltatre.common.hp.AsyncQueuedExecutor[] StaticAsyncExecutors
		{
			get { return _singleExecutor; }
		}

		[ThreadStatic]
		JavaScriptSerializer _jss = new JavaScriptSerializer();

		[ThreadStatic]
		SimpleActivityStreamListDal _mmdal = new SimpleActivityStreamListDal(SetupDb());

		[ThreadStatic]
		SimpleActivityStreamDal _mdal = new SimpleActivityStreamDal(SetupDb());

		[ThreadStatic]
		UserDal _udal = new UserDal(SetupDb());

		[ThreadStatic]
		VerbDal _vdal = new VerbDal(SetupDb());

		[ThreadStatic]
		ApplicationDal _adal = new ApplicationDal(SetupDb());

		private static com.deltatre.common.interfaces.IDatabase SetupDb()
		{
			int errorCode =0;
			MongoDb md = new MongoDb();
			try
			{
				md.Initialize("mongodb://localhost:27017");
				md.Connect("BigStuff", out errorCode);
			}
			catch { }
			return md;
		}

		public override bool ExecuteAsyncProcessing(com.deltatre.common.hp.AsyncTask ass)
		{
			HttpContext ctx = (HttpContext)ass.Tag;
			byte[] bt = new byte[ctx.Request.InputStream.Length];
			ctx.Request.InputStream.Read(bt, 0, bt.Length);
			string s = ctx.Request.ContentEncoding.GetString(bt);
			((PubSubTask)ass).Message = _jss.Deserialize<SimpleActivityStream>(s);
			return true;
		}

		public override void FinalizeBatchElements(List<com.deltatre.common.hp.AsyncTask> atl)
		{
			int errorCode = 0;
			Dictionary<Guid, bool> dverbs = new Dictionary<Guid, bool>();
			Dictionary<Guid, bool> duid = new Dictionary<Guid, bool>();
			Dictionary<Guid, bool> dapp = new Dictionary<Guid, bool>();
			List<SimpleActivityStream> psl = new List<SimpleActivityStream>();
			List<PubSubTask> pst = new List<PubSubTask>();
			SimpleActivityStream sas = null;
			bool dosome = false;
			foreach (AsyncTask e in atl)
			{
				dosome = true;
				sas = ((PubSubTask)e).Message;
				sas.Id = Guid.NewGuid();
				if (!dverbs.ContainsKey(sas.AI))
				{
					if (_vdal.GetById(sas.AI, out errorCode) != null)
					{
						dverbs.Add(sas.AI, true);
					}
					else
					{
						dosome = false;
					}
				}
				if (!duid.ContainsKey(sas.SI))
				{
					if (_udal.GetById(sas.SI, out errorCode) != null)
					{
						duid.Add(sas.SI, true);
					}
					else
					{
						dosome = false;
					}
				}
				if (!dapp.ContainsKey(sas.SA))
				{
					if (_adal.GetById(sas.SA, out errorCode) != null)
					{
						dapp.Add(sas.SA, true);
					}
					else
					{
						dosome = false;
					}
				}
				if (!dosome)
				{
					((PubSubTask)e).Result = null;
					e.CompleteTask();
				}
				else
				{
					psl.Add(((PubSubTask)e).Message);
					pst.Add((PubSubTask)e);
				}
				
			}
			//Added [pst
			if (psl.Count == 1)
			{
				_mdal.Insert(psl[0], out errorCode);
				pst[0].Result = "";
				pst[0].CompleteTask();
			}
			else if (psl.Count > 0)
			{
				SimpleActivityStreamList psmm = new SimpleActivityStreamList();
				psmm.Id = Guid.NewGuid();
				psmm.SetData(psl);
				_mmdal.Insert(psmm, out errorCode);
				foreach (PubSubTask e in pst)
				{
					e.Result = "";
					e.CompleteTask();
				}
				Global._batchinsert.EnqueueList(psmm);
				
			}
		}
	}
}