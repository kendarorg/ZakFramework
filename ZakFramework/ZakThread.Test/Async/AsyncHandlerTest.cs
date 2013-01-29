using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using ZakTestUtils;
using ZakThread.Async;
using ZakThread.Test.Async.SampleObjects;

namespace ZakThread.Test.Async
{
	[TestFixture]
	public class AsyncHandlerTest
	{
		private const int MAX_DEGREE_OF_PARALLELISM = 8;
		private const int MESSAGES_MULTIPLIER = 1000;
		private const int BATCH_SIZE_DIVISOR = 10;
		private const int NUMBER_OF_MESSAGES = MESSAGES_MULTIPLIER * MAX_DEGREE_OF_PARALLELISM;
		private const int BATCH_SIZE = NUMBER_OF_MESSAGES / BATCH_SIZE_DIVISOR;
		private CounterContainer _counter;
		private CounterContainer _successful;
		private CounterContainer _threadCalls;

		public void DoFireAndForget(object sender, TestThreads.TestThreadsEventArgs args)
		{
			_threadCalls.Increment();
			var callsHandler = (ITasksHandlerThread)args.Param;
			var sw = new Stopwatch();
			sw.Start();
			var requestObject = new RequestObject(args.CurrentCycle);
			callsHandler.FireAndForget(requestObject, 1000);
			
			sw.Stop();
		}

		public void DoRequestAndWaitTimeout(object sender, TestThreads.TestThreadsEventArgs args)
		{
			_threadCalls.Increment();
			var callsHandler = (ITasksHandlerThread)args.Param;
			var sw = new Stopwatch();
			sw.Start();
			var requestObject = new RequestObject(args.CurrentCycle);
			sw.Stop();
			_counter.Add(callsHandler.DoRequestAndWait(requestObject, 2));

			if (args.CurrentCycle == -requestObject.GetReturnAs<long>()) _successful.Increment();
		}

		public void DoRequestAndWait(object sender, TestThreads.TestThreadsEventArgs args)
		{
			_threadCalls.Increment();
			var callsHandler = (ITasksHandlerThread)args.Param;
			var sw = new Stopwatch();
			sw.Start();
			var requestObject = new RequestObject(args.CurrentCycle);
			sw.Stop();
			_counter.Add(callsHandler.DoRequestAndWait(requestObject, 1000));

			if (args.CurrentCycle == -requestObject.GetReturnAs<long>()) _successful.Increment();
		}

		[Test]
		public void ItShouldBePossibleToRunTasksInParallelDelegatingThemToTheThread()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 0;
			_counter = new CounterContainer();
			_successful = new CounterContainer();
			_threadCalls = new CounterContainer();

			var callsHandler = new SampleAsyncTasksHandler("TEST", waitTimeMs);
			callsHandler.RunThread();
			Thread.Sleep(100);

			var testThread = new TestThreads(false,DoRequestAndWait, MAX_DEGREE_OF_PARALLELISM);
			var initTime = testThread.RunParallel(iterations, callsHandler);
			var sw = new Stopwatch();
			sw.Start();
			while ((sw.ElapsedMilliseconds + initTime) < waitTimeMs * iterations * 3 || !testThread.IsFinished)
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
			Assert.AreEqual(iterations, _threadCalls.Counter);
			Assert.AreEqual(iterations, testThread.CyclesCounter.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			Assert.AreEqual(iterations, _successful.Counter);
			callsHandler.Terminate();
			testThread.Terminate();
		}

		[Test]
		public void ItShouldBePossibleToRunTasksInParallelWithTimeouts()
		{
			const int iterations = 10;
			const int waitTimeMs = 100;
			_counter = new CounterContainer();
			_successful = new CounterContainer();
			_threadCalls = new CounterContainer();

			var callsHandler = new SampleAsyncTasksHandler("TEST", waitTimeMs);
			callsHandler.RunThread();
			Thread.Sleep(100);

			var testThread = new TestThreads(false, DoRequestAndWaitTimeout, 2);
			var initTime = testThread.RunParallel(iterations, callsHandler);
			var sw = new Stopwatch();
			sw.Start();
			while ((sw.ElapsedMilliseconds + initTime) < waitTimeMs * iterations * 3 || !testThread.IsFinished)
			{
				if (testThread.IsFinished && initTime == 0) initTime = sw.ElapsedMilliseconds;
				if (iterations == callsHandler.CallsCount) break;
				Thread.Sleep(10);
			}
			sw.Stop();
			Debug.WriteLine("");
			Debug.WriteLine("a Init time " + initTime);
			Debug.WriteLine("a Run time " + sw.ElapsedMilliseconds);
			Debug.WriteLine("");
			Assert.AreEqual(iterations, _threadCalls.Counter);
			Assert.AreEqual(iterations, testThread.CyclesCounter.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			Assert.AreNotEqual(iterations, _successful.Counter);
			var errorsAndSuccesses = testThread.Exceptions.Count + _successful.Counter;
			Assert.AreEqual(iterations, errorsAndSuccesses);
			callsHandler.Terminate();
			testThread.Terminate();
		}

		[Test]
		public void ItShouldBePossibleToRunTasksInParallelDelegatingThemToTheThreadWithFireAndForget()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 0;
			
			_counter = new CounterContainer();
			_successful = new CounterContainer();
			_threadCalls = new CounterContainer();
			var callsHandler = new SampleAsyncTasksHandler("TEST", waitTimeMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(false, DoFireAndForget, MAX_DEGREE_OF_PARALLELISM);
			var initTime = testThread.RunParallel(iterations, callsHandler);
			var sw = new Stopwatch();
			sw.Start();
			while ((sw.ElapsedMilliseconds + initTime) < waitTimeMs * iterations * 3 || !testThread.IsFinished)
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
			Assert.AreEqual(iterations, _threadCalls.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}

		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingBatchSize()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 0;
			const int batchSize = 100;
			const int batchTimeoutMs = 1000;

			_counter = new CounterContainer();
			_successful = new CounterContainer();
			_threadCalls = new CounterContainer();
			var callsHandler = new SampleAsyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(false, DoRequestAndWait, MAX_DEGREE_OF_PARALLELISM);
			var initTime = testThread.RunParallel(iterations, callsHandler);

			var sw = new Stopwatch();
			sw.Start();
			while ((sw.ElapsedMilliseconds + initTime) < waitTimeMs * iterations * 3 || !testThread.IsFinished)
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
			Assert.AreEqual(iterations, _threadCalls.Counter);
			Assert.AreEqual(iterations, _successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}

		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingBatchSizeWithFireAndForget()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 2;
			const int batchSize = 4;
			const int batchTimeoutMs = 1000;

			_counter = new CounterContainer();
			_successful = new CounterContainer();
			_threadCalls = new CounterContainer();
			var callsHandler = new SampleAsyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(false, DoFireAndForget, MAX_DEGREE_OF_PARALLELISM);
			testThread.RunParallel(iterations, callsHandler);

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds  < waitTimeMs * iterations * 3 || !testThread.IsFinished)
			{
				if (iterations == callsHandler.CallsCount) break;
				Thread.Sleep(100);
			}
			sw.Stop();

			Debug.WriteLine("");
			Debug.WriteLine("d Run time " + sw.ElapsedMilliseconds);
			Debug.WriteLine("");
			Assert.AreEqual(iterations, _threadCalls.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}


		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingTimeout()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 2;
			const int batchSize = BATCH_SIZE;
			const int batchTimeoutMs = 1;

			_counter = new CounterContainer();
			_successful = new CounterContainer();
			_threadCalls = new CounterContainer();
			var callsHandler = new SampleAsyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(false, DoRequestAndWait, MAX_DEGREE_OF_PARALLELISM);
			testThread.RunParallel(iterations, callsHandler);

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < waitTimeMs * iterations * 3 || !testThread.IsFinished)
			{
				if (iterations == _successful.Counter) break;
				Thread.Sleep(100);
			}

			Thread.Sleep(1000);
			sw.Stop();
			Assert.AreEqual(iterations, _threadCalls.Counter);
			Assert.AreEqual(iterations, _successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}


		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingTimeoutFireAndForget()
		{
			const int iterations = NUMBER_OF_MESSAGES;
			const int waitTimeMs = 2;
			const int batchSize = NUMBER_OF_MESSAGES;
			const int batchTimeoutMs = 1;

			_counter = new CounterContainer();
			_successful = new CounterContainer();
			_threadCalls = new CounterContainer();
			var callsHandler = new SampleAsyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var testThread = new TestThreads(false, DoFireAndForget, MAX_DEGREE_OF_PARALLELISM);
			testThread.RunParallel(iterations, callsHandler);

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < waitTimeMs * iterations * 3 || !testThread.IsFinished)
			{
				if (iterations == callsHandler.CallsCount) break;
				Thread.Sleep(100);
			}
			sw.Stop();
			Assert.AreEqual(iterations, _threadCalls.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
			testThread.Terminate();
		}
	}
}
