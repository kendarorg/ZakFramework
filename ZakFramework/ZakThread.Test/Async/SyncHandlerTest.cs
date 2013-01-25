using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ZakThread.Test.Async.SampleObjects;
using ZakTestUtils;
using ZakThread.Async;

namespace ZakThread.Test.Async
{
	[TestFixture]
	public class SyncHandlerTest
	{
		private const int MAX_DEGREE_OF_PARALLELISM = 4;
		private const int MESSAGES_MULTIPLIER = 100;
		private const int BATCH_SIZE_DIVISOR = 10;
		private const int NUMBER_OF_MESSAGES = MESSAGES_MULTIPLIER * MAX_DEGREE_OF_PARALLELISM;
		private const int BATCH_SIZE = NUMBER_OF_MESSAGES / BATCH_SIZE_DIVISOR;
		private CounterContainer _counter;
		private CounterContainer _successful;

		public void DoFireAndForget(object sender, TestThreads.TestThreadsEventArgs args)
		{
			var callsHandler = (ITasksHandlerThread)args.Param;
			var sw = new Stopwatch();
			sw.Start();
			var requestObject = new RequestObject(args.CurrentCycle);
			callsHandler.FireAndForget(requestObject, 1000);
			sw.Stop();
		}

		public void DoRequestAndWait(object sender, TestThreads.TestThreadsEventArgs args)
		{
			var callsHandler = (ITasksHandlerThread)args.Param;
			var sw = new Stopwatch();
			sw.Start();
			var requestObject = new RequestObject(args.CurrentCycle);
			sw.Stop();
			_counter.Add(callsHandler.DoRequestAndWait(requestObject, 1000));

			if (args.CurrentCycle == -requestObject.GetReturnAs<long>()) _successful.Increment();
		}

		[Test]
		public void AItShouldBePossibleToRunTasksInParallelDelegatingThemToTheThread()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 1;
			_counter = new CounterContainer();
			_successful = new CounterContainer();

			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs);
			callsHandler.RunThread();
			Thread.Sleep(100);

			var testThread = new TestThreads(false,DoRequestAndWait, MAX_DEGREE_OF_PARALLELISM);
			var initTime = testThread.RunParallel(iterations, callsHandler);
			var sw = new Stopwatch();
			sw.Start();
			while ((sw.ElapsedMilliseconds + initTime) < waitTimeMs * iterations * 2 || !testThread.IsFinished)
			{
				if (testThread.IsFinished && initTime==0) initTime = sw.ElapsedMilliseconds;
				if (iterations == callsHandler.CallsCount) break;
				Thread.Sleep(10);
			}
			sw.Stop();
			Debug.WriteLine("");
			Debug.WriteLine("a Init time " + initTime);
			Debug.WriteLine("a Run time " + sw.ElapsedMilliseconds);
			Debug.WriteLine("");
			Assert.AreEqual(iterations, testThread.CyclesCounter.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			Assert.AreEqual(iterations, _successful.Counter);
			callsHandler.Terminate();
			testThread.Terminate();
		}

		[Test]
		public void BItShouldBePossibleToRunTasksInParallelDelegatingThemToTheThreadWithFireAndForget()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 1;
			int cores = Environment.ProcessorCount;
			_counter = new CounterContainer();
			_successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(false, DoFireAndForget, MAX_DEGREE_OF_PARALLELISM);
			var initTime = testThread.RunParallel(iterations, callsHandler);
			var sw = new Stopwatch();
			sw.Start();
			while ((sw.ElapsedMilliseconds + initTime) < waitTimeMs * iterations * 2 || !testThread.IsFinished)
			{
				if (testThread.IsFinished && initTime == 0) initTime = sw.ElapsedMilliseconds;
				if (iterations == callsHandler.CallsCount) break;
				Thread.Sleep(10);
			}
			sw.Stop();
			Debug.WriteLine("");
			Debug.WriteLine("b Init time " + initTime);
			Debug.WriteLine("b Run time " + sw.ElapsedMilliseconds);
			Debug.WriteLine("");
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}

		[Test]
		public void CItShouldBePossibleToRunTasksInBatchCheckingBatchSize()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			const int batchSize = 10;
			const int batchTimeoutMs = 10000;

			int cores = Environment.ProcessorCount;
			_counter = new CounterContainer();
			_successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(false, DoRequestAndWait, MAX_DEGREE_OF_PARALLELISM);
			var initTime = testThread.RunParallel(iterations, callsHandler);

			var sw = new Stopwatch();
			sw.Start();
			while ((sw.ElapsedMilliseconds + initTime) < waitTimeMs * iterations * 2 || !testThread.IsFinished)
			{
				if (testThread.IsFinished && initTime == 0) initTime = sw.ElapsedMilliseconds;
				if (iterations == _successful.Counter && iterations == callsHandler.CallsCount && testThread.IsFinished)
				{
					break;
				}
				Thread.Sleep(100);
			}
			sw.Stop();
			Debug.WriteLine("");
			Debug.WriteLine("c Init time " + initTime);
			Debug.WriteLine("c Run time " + sw.ElapsedMilliseconds);
			Debug.WriteLine("");
			Assert.AreEqual(iterations, _successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}

		[Test]
		public void DItShouldBePossibleToRunTasksInBatchCheckingBatchSizeWithFireAndForget()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 1;
			const int batchSize = 4;
			const int batchTimeoutMs = 1000;

			int cores = Environment.ProcessorCount;
			_counter = new CounterContainer();
			_successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(false, DoFireAndForget, MAX_DEGREE_OF_PARALLELISM);
			testThread.RunParallel(iterations, callsHandler);

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < waitTimeMs * iterations * 2)
			{
				if (iterations == callsHandler.CallsCount) break;
				Thread.Sleep(100);
			}
			sw.Stop();

			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}


		[Test]
		public void EItShouldBePossibleToRunTasksInBatchCheckingTimeout()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 1;
			const int batchSize = BATCH_SIZE;
			const int batchTimeoutMs = 1;

			int cores = Environment.ProcessorCount;
			_counter = new CounterContainer();
			_successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(false, DoRequestAndWait, MAX_DEGREE_OF_PARALLELISM);
			testThread.RunParallel(iterations, callsHandler);

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < waitTimeMs * iterations * 2)
			{
				if (iterations == _successful.Counter) break;
				Thread.Sleep(100);
			}

			Thread.Sleep(1000);
			sw.Stop();
			Assert.AreEqual(iterations, _successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}


		[Test]
		public void FItShouldBePossibleToRunTasksInBatchCheckingTimeoutFireAndForget()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 1;
			const int batchSize = NUMBER_OF_MESSAGES;
			const int batchTimeoutMs = 1;

			int cores = Environment.ProcessorCount;
			_counter = new CounterContainer();
			_successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(false, DoFireAndForget, MAX_DEGREE_OF_PARALLELISM);
			testThread.RunParallel(iterations, callsHandler);

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < waitTimeMs * iterations * 2)
			{
				if (iterations == callsHandler.CallsCount) break;
				Thread.Sleep(100);
			}
			sw.Stop();
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}
	}
}
