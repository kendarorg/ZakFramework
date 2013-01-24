using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZakCore.Utils.Collections;

namespace ZakTestUtils
{
	public class TestThreads
	{
		public class TestThreadsEventArgs : EventArgs
		{
			public int From { get; internal set; }
			public int To { get; internal set; }
			public int CurrentCycle { get; internal set; }
			public object Param { get; internal set; }
			public object Result { get; set; }
		}

		private int _maxDegreeOfParallelism;
		private EventHandler<TestThreadsEventArgs> _toExecute;
		public TestThreads(EventHandler<TestThreadsEventArgs> toExecute, int maxDegreeOfParallelism = -1)
		{
			_toExecute = toExecute;
			_maxDegreeOfParallelism = maxDegreeOfParallelism == -1 ? Environment.ProcessorCount : maxDegreeOfParallelism;
			_threads = new List<Thread>();
		}

		public List<Thread> _threads;
		public LockFreeQueue<Exception> Exceptions { get; private set; }
		public LockFreeQueue<object> Results { get; private set; }
		private long _runningThreads = 0;
		private AutoResetEvent _eventStart;

		public void RunParallel(int count, object param = null)
		{
			Exceptions = new LockFreeQueue<Exception>();
			Results = new LockFreeQueue<object>();
			if (count % _maxDegreeOfParallelism != 0) throw new Exception();
			var steps = count / _maxDegreeOfParallelism;
			_runningThreads = _maxDegreeOfParallelism;
			_eventStart = new AutoResetEvent(false);

			for (int i = 0; i < _maxDegreeOfParallelism; i++)
			{
				var from = count * i;
				var to = count * (i + 1);
				var thread = new Thread(RunTask);
				thread.Start(new Tuple<int, int, object>(from, to, param));
				_threads.Add(thread);
			}
			_eventStart.Set();
		}

		public bool IsFinished { get { return Interlocked.Read(ref _runningThreads) <= 0; } }

		private void RunTask(object param)
		{
			var tuple = (Tuple<int, int, object>)param;
			var from = tuple.Item1;
			var to = tuple.Item2;
			var par = tuple.Item3;
			_eventStart.WaitOne(5000);
			for (int i = from; i < to; i++)
			{
				try
				{
					var ea = new TestThreadsEventArgs
					{
						CurrentCycle = i,
						From = from,
						To = to,
						Param = par
					};
					_toExecute(this, ea);
					Results.Enqueue(ea.Result);
				}
				catch (Exception ex)
				{
					Exceptions.Enqueue(ex);
				}
				Thread.Sleep(0);
			}
			Interlocked.Decrement(ref _runningThreads);
		}

		public void Terminate()
		{
			foreach (var thread in _threads)
			{
				thread.Abort();
			}
			_threads = new List<Thread>();
			_runningThreads = 0;
		}
	}
}
