using NUnit.Framework;
using ZakCore.Utils.Collections;
using ZakThread.Test.Async.SampleObjects;
using System.Threading;
namespace ZakTestUtils.Test
{
	[TestFixture]
	class TestThreadsTest
	{
		private CounterContainer _sendCounter;
		private CounterContainer _firstTestCounter;
		private CounterContainer _errorTestCounter;

		private void FirstTest(object sender, TestThreads.TestThreadsEventArgs args)
		{
			_firstTestCounter.Increment();
		}

		private void SecondTest(object sender, TestThreads.TestThreadsEventArgs args)
		{
			if (args.Param is int)
			{
				if ((int)args.Param == 2)
				{
					_firstTestCounter.Increment();
				}
			}
			else
			{
				_errorTestCounter.Increment();
			}
		}

		[Test]
		public void DoTest()
		{
			_firstTestCounter = new CounterContainer();
			_errorTestCounter = new CounterContainer();
			var tt = new TestThreads(false, FirstTest, 1);
			tt.RunParallel(100);
			while (_firstTestCounter.Counter < 100)
			{
				Thread.Sleep(10);
			}
			Assert.AreEqual(100, _firstTestCounter.Counter);
			tt.Terminate();
		}

		[Test]
		public void DoTestMulti()
		{
			const int iterations = 10000;
			_firstTestCounter = new CounterContainer();
			_errorTestCounter = new CounterContainer();
			var tt = new TestThreads(false, FirstTest, 100);
			tt.RunParallel(iterations);
			while (_firstTestCounter.Counter < iterations)
			{
				Thread.Sleep(10);
			}
			Assert.AreEqual(iterations, _firstTestCounter.Counter);
			tt.Terminate();
		}


		[Test]
		public void DoTestMultiWithPar()
		{
			_firstTestCounter = new CounterContainer();
			_errorTestCounter = new CounterContainer();
			var tt = new TestThreads(false, SecondTest, 10);
			tt.RunParallel(1000, 2);
			while (_firstTestCounter.Counter < 1000 && _errorTestCounter.Counter==0)
			{
				Thread.Sleep(10);
			}
			Assert.AreEqual(0, _errorTestCounter.Counter);
			Assert.AreEqual(1000, _firstTestCounter.Counter);
			tt.Terminate();
		}

		private void ThirdTestSender(object sender, TestThreads.TestThreadsEventArgs args)
		{
			var lfq = (LockFreeQueue<int>)args.Param;
			_sendCounter.Increment();
			lfq.Enqueue(args.CurrentCycle);
		}


		private void ThirdTestReader(object sender, TestThreads.TestThreadsEventArgs args)
		{
			var lfq = (LockFreeQueue<int>)args.Param;
			int item = -1;
			while ((item = lfq.DequeueSingle())!=0)
			{
				_firstTestCounter.Increment();
			}
		}

		[Test]
		public void DoTestMultiWithParAndSendingThread()
		{
			const int iterations = 10000;
			var lfq = new LockFreeQueue<int>();
			_sendCounter = new CounterContainer();
			_firstTestCounter = new CounterContainer();
			_errorTestCounter = new CounterContainer();
			var ttr = new TestThreads(false, ThirdTestSender, 5);
			ttr.RunParallel(iterations, lfq);


			while (_sendCounter.Counter < iterations && _errorTestCounter.Counter == 0)
			{
				Thread.Sleep(10);
			}
			Assert.AreEqual(0, _errorTestCounter.Counter);
			Assert.AreEqual(iterations, _sendCounter.Counter);
			Assert.AreEqual(iterations, lfq.Count);
			ttr.Terminate();
		}

		[Test]
		public void DoTestMultiWithParAndReadingThread()
		{
			const int iterations = 10000;
			var lfq = new LockFreeQueue<int>();
			_sendCounter = new CounterContainer();
			_firstTestCounter = new CounterContainer();
			_errorTestCounter = new CounterContainer();
			var ttr = new TestThreads(false, ThirdTestReader, 5);
			ttr.RunParallel(iterations, lfq, true);
			
			var tts = new TestThreads(false, ThirdTestSender, 5);
			tts.RunParallel(iterations, lfq);
			while (_firstTestCounter.Counter < (iterations-1) && _errorTestCounter.Counter == 0)
			{
				Thread.Sleep(10);
			}
			Assert.AreEqual(0, _errorTestCounter.Counter);
			Assert.AreEqual(iterations, _sendCounter.Counter);
			Assert.AreEqual(iterations-1, _firstTestCounter.Counter);
			tts.Terminate();
			ttr.Terminate();
		}
	}
}
