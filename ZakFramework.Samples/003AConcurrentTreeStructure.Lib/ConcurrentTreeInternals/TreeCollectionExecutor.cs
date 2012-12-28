using ZakCore.Utils.Logging;
using ZakThread.HighPower;

namespace _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals
{
	/// <summary>
	/// This class will handle the asynchrounous operation execution, at this moment is more a forced implementation
	/// </summary>
	internal class TreeCollectionExecutor<TContent> : AsyncQueuedExecutor
	{
		private readonly ConcurrentTree<TContent> _container;

		private const long MAX_MS_SINCE_START = 100;
		private const long MAX_MSGS_ELABORATED = 100;

		public TreeCollectionExecutor(ILogger logger, string threadName, ConcurrentTree<TContent> container)
			: base(logger, threadName)
		{
			_container = container;
		}

		/// <summary>
		/// When a task is executed the result will be made immediatly sent to the caller
		/// </summary>
		protected override bool IsBatch
		{
			get { return false; }
		}

		/// <summary>
		/// No asynch tasks so we will clear the queue
		/// </summary>
		protected override bool RemoveExpiredTasks
		{
			get { return false; }
		}

		protected override void HandleTaskCompleted(AsyncTask asyncTask)
		{
		}

		/// <summary>
		/// All tasks must be handled. We can choose eventually to avoid executing them
		/// </summary>
		/// <param name="at"></param>
		/// <returns></returns>
		protected override bool ShouldHandleTask(AsyncTask at)
		{
			return true;
		}

		/// <summary>
		/// This is to avoid the kidnapping of processor time by the asynchronous operations
		/// </summary>
		/// <param name="at"></param>
		/// <param name="msSinceStart"></param>
		/// <param name="msgsElaborated"></param>
		/// <returns></returns>
		protected override bool CheckIfShouldStop(AsyncTask at, long msSinceStart, int msgsElaborated)
		{
			if (msSinceStart > MAX_MS_SINCE_START)
			{
				return true;
			}
			if (msgsElaborated > MAX_MSGS_ELABORATED)
			{
				return true;
			}
			return false;
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
			_container.ExecuteOperation((ConcurrentTreeMessage) msg);
			return true;
		}

		public override void RegisterMessages()
		{
		}
	}
}