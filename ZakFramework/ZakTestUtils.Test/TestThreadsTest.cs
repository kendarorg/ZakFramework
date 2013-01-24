using NUnit.Framework;
using ZakThread.Test.Async.SampleObjects;
using System.Threading;
namespace ZakTestUtils.Test
{
	[TestFixture]
	class TestThreadsTest
	{
		private CounterContainer _firstTestCounter;
		private CounterContainer _errorTestCounter;

		private void FirstTest(object sender, ZakTestUtils.TestThreads.TestThreadsEventArgs args)
		{
			_firstTestCounter.Increment();
		}

		private void SecondTest(object sender, ZakTestUtils.TestThreads.TestThreadsEventArgs args)
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
			var tt = new TestThreads(FirstTest, 1);
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
			_firstTestCounter = new CounterContainer();
			_errorTestCounter = new CounterContainer();
			var tt = new TestThreads(FirstTest, 10);
			tt.RunParallel(1000);
			while (_firstTestCounter.Counter < 1000)
			{
				Thread.Sleep(10);
			}
			Assert.AreEqual(1000, _firstTestCounter.Counter);
			tt.Terminate();
		}


		[Test]
		public void DoTestMultiWithPar()
		{
			_firstTestCounter = new CounterContainer();
			_errorTestCounter = new CounterContainer();
			var tt = new TestThreads(SecondTest, 10);
			tt.RunParallel(1000, 2);
			while (_firstTestCounter.Counter < 1000 && _errorTestCounter.Counter==0)
			{
				Thread.Sleep(10);
			}
			Assert.AreEqual(0, _errorTestCounter.Counter);
			Assert.AreEqual(1000, _firstTestCounter.Counter);
			tt.Terminate();
		}
	}
}
