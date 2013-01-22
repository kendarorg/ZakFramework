using System;
using System.Collections.Generic;
using System.Threading;

namespace ZakTestUtils
{

	public static class TestConcurrent
	{
		internal class PointerContainer
		{
			public Int64 Marker;
		}

		public class ConcurrentAction
		{
			internal AutoResetEvent EventStarted;
			internal AutoResetEvent EventTerminated;
			internal PointerContainer WaitCount;
			public object Result;
			public object[] Parameters = new object[] { };
			public event EventHandler<EventArgs> ExecuteAction;
			internal void OnExecute()
			{
				try
				{
					ExecuteAction(this, new EventArgs());
				}
				catch (Exception ex)
				{
					Result = ex;
				}
				if (Result == null)
				{
					Result = true;
				}
			}
		}

		public static List<object> StartConcurrent(params ConcurrentAction[] actions)
		{
			var allWait = new PointerContainer { Marker = actions.Length - 1 };
			var startWait = new AutoResetEvent(false);
			var endWait = new AutoResetEvent(false);
			foreach (var action in actions)
			{
				action.EventStarted = startWait;
				action.EventTerminated = endWait;
				action.WaitCount = allWait;
				var th = new Thread(RunSingleAction);
				th.Start(action);
			}
			startWait.Set();
			//Wait for threads to start
			Thread.Sleep(1000);
			endWait.WaitOne(60 * 1000);

			var resultList = new List<object>();
			foreach (var action in actions)
			{
				resultList.Add(action.Result);
			}
			return resultList;
		}

		private static void RunSingleAction(object param)
		{
			var action = (ConcurrentAction)param;
			action.EventStarted.WaitOne(5000);
			action.OnExecute();
			var result = Interlocked.Decrement(ref action.WaitCount.Marker);
			if (result <= -1) action.EventTerminated.Set();
		}
	}
}
