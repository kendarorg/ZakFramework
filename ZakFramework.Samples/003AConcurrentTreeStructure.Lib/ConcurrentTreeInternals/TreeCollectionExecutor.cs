using ZakCore.Utils.Logging;
using ZakThread.Async;

namespace _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals
{
	/// <summary>
	/// This class will handle the asynchrounous operation execution, at this moment is more a forced implementation
	/// </summary>
	internal class TreeCollectionExecutor<TContent> : BaseAsyncHandlerThread
	{
		private readonly ConcurrentTree<TContent> _container;

		private const long MAX_MS_SINCE_START = 100;
		private const int MAX_MSGS_ELABORATED = 100;

		public TreeCollectionExecutor(ILogger logger, string threadName, ConcurrentTree<TContent> container)
			: base(logger, threadName)
		{
			MaxMesssagesPerCycle = MAX_MSGS_ELABORATED;
			_container = container;
		}

		/// <summary>
		/// No asynch tasks so we will clear the queue
		/// </summary>
		protected bool RemoveExpiredTasks
		{
			get { return false; }
		}

		/// <summary>
		/// This will forward the messages to the real implementations.
		/// In this specific situation we don't need to take in consideration the messages 
		/// failure or success.
		/// This is a shortcut, since we want that all operations are made inside a single thread,
		/// the queue's one.
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		protected override bool HandleMessage(ZakThread.Threading.IMessage msg)
		{
			return true;
		}

		public override void RegisterMessages()
		{
			RegisterMessage(typeof(ConcurrentTreeMessage));
		}

		public override bool HandleTaskRequest(RequestObjectMessage container, BaseRequestObject requestObject)
		{
			_container.ExecuteOperation((ConcurrentTreeMessage)requestObject);
			return true;
		}

		public override void HandleBatchCompleted(System.Collections.Generic.IEnumerable<RequestObjectMessage> batchExecuted)
		{
			_container.FinalizeBatchElements(batchExecuted);
		}
	}
}