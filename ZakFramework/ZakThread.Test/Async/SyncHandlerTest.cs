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
		private const int MAX_DEGREE_OF_PARALLELISM = 10;
		private CounterContainer counter;
		private CounterContainer successful;

		public void DoFireAndForget(object sender, ZakTestUtils.TestThreads.TestThreadsEventArgs args)
		{
			var callsHandler = (ITasksHandlerThread)args.Param;
			var requestObject = new RequestObject(args.CurrentCycle);
			callsHandler.FireAndForget(requestObject, 1000);
		}

		public void DoRequestAndWait(object sender, ZakTestUtils.TestThreads.TestThreadsEventArgs args)
		{
			var callsHandler = (ITasksHandlerThread)args.Param;
			var requestObject = new RequestObject(args.CurrentCycle);
			counter.Add(callsHandler.DoRequestAndWait(requestObject, 1000));
			if (args.CurrentCycle == -requestObject.GetReturnAs<long>()) successful.Increment();
		}

		[Test]
		public void AItShouldBePossibleToRunTasksInParallelDelegatingThemToTheThread()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			counter = new CounterContainer();
			successful = new CounterContainer();

			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs);
			callsHandler.RunThread();
			Thread.Sleep(100);

			var testThread = new TestThreads(DoRequestAndWait, MAX_DEGREE_OF_PARALLELISM);
			testThread.RunParallel(iterations, callsHandler);
			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < waitTimeMs * iterations * 2)
			{
				if (iterations == successful.Counter) break;
				Thread.Sleep(100);
			}
			sw.Stop();
			Assert.AreEqual(iterations, successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}

		[Test]
		public void BItShouldBePossibleToRunTasksInParallelDelegatingThemToTheThreadWithFireAndForget()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			int cores = Environment.ProcessorCount;
			counter = new CounterContainer();
			successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(DoFireAndForget, MAX_DEGREE_OF_PARALLELISM);
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
		public void CItShouldBePossibleToRunTasksInBatchCheckingBatchSize()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			const int batchSize = 10;
			const int batchTimeoutMs = 100;

			int cores = Environment.ProcessorCount;
			counter = new CounterContainer();
			successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(DoRequestAndWait, MAX_DEGREE_OF_PARALLELISM);
			testThread.RunParallel(iterations, callsHandler);

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < waitTimeMs * iterations * 2)
			{
				if (iterations == successful.Counter) break;
				Thread.Sleep(100);
			}
			sw.Stop();
			Thread.Sleep(10000);
			Assert.AreEqual(iterations, successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}

		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingBatchSizeWithFireAndForget()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			const int batchSize = 4;
			const int batchTimeoutMs = 10;

			int cores = Environment.ProcessorCount;
			counter = new CounterContainer();
			successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(DoFireAndForget, MAX_DEGREE_OF_PARALLELISM);
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
		public void ItShouldBePossibleToRunTasksInBatchCheckingTimeout()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			const int batchSize = 100;
			const int batchTimeoutMs = 1;

			int cores = Environment.ProcessorCount;
			counter = new CounterContainer();
			successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(DoRequestAndWait, MAX_DEGREE_OF_PARALLELISM);
			testThread.RunParallel(iterations, callsHandler);

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < waitTimeMs * iterations * 2)
			{
				if (iterations == successful.Counter) break;
				Thread.Sleep(100);
			}

			Thread.Sleep(1000);
			sw.Stop();
			Assert.AreEqual(iterations, successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}


		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingTimeoutFireAndForget()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			const int batchSize = 100;
			const int batchTimeoutMs = 1;

			int cores = Environment.ProcessorCount;
			counter = new CounterContainer();
			successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(DoFireAndForget, MAX_DEGREE_OF_PARALLELISM);
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
