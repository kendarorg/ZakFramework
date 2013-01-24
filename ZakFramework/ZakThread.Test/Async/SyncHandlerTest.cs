using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ZakThread.Test.Async.SampleObjects;

namespace ZakThread.Test.Async
{
	[TestFixture]
	public class SyncHandlerTest
	{
		[Test]
		public void ItShouldBePossibleToRunTasksInParallelDelegatingThemToTheThread()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			int cores = Environment.ProcessorCount;
			var counter = new CounterContainer();
			var successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var parallelOptions = new ParallelOptions
				{
					MaxDegreeOfParallelism = -1
				};

			Parallel.For((long) 0, iterations, parallelOptions, (i) =>
			{
				try
				{
					var requestObject = new RequestObject(i);
					counter.Add(callsHandler.DoRequestAndWait(requestObject,1000));
					if(i==-requestObject.GetReturnAs<long>()) successful.Increment();
				}
				catch(TimeoutException exception)
				{
					Console.WriteLine(exception);
				}
			});
			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < (waitTimeMs*iterations*2)/cores)
			{
				if (iterations == successful.Counter) break;
				Thread.Sleep(100);
			}
			sw.Stop();
			Assert.AreEqual(iterations,successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
		}

		[Test]
		public void ItShouldBePossibleToRunTasksInParallelDelegatingThemToTheThreadWithFireAndForget()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			int cores = Environment.ProcessorCount;
			var counter = new CounterContainer();
			var successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var parallelOptions = new ParallelOptions
			{
				MaxDegreeOfParallelism = -1
			};

			Parallel.For((long)0, iterations, parallelOptions, (i) =>
			{
				try
				{
					var requestObject = new RequestObject(i);
					callsHandler.FireAndForget(requestObject, 1000);
				}
				catch (TimeoutException exception)
				{
					Console.WriteLine(exception);
				}
			});
			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < (waitTimeMs * iterations * 2) / cores)
			{
				if (iterations == callsHandler.CallsCount) break;
				Thread.Sleep(100);
			}
			sw.Stop();
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
		}

		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingBatchSize()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			const int batchSize = 4;
			const int batchTimeoutMs = 10;

			int cores = Environment.ProcessorCount;
			var counter = new CounterContainer();
			var successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var parallelOptions = new ParallelOptions
			{
				MaxDegreeOfParallelism = -1
			};

			Parallel.For((long)0, iterations, parallelOptions, (i) =>
			{
				try
				{
					var requestObject = new RequestObject(i);
					counter.Add(callsHandler.DoRequestAndWait(requestObject,1000));
					if (i == -requestObject.GetReturnAs<long>()) successful.Increment();
				}
				catch (TimeoutException exception)
				{
					Console.WriteLine(exception);
				}
			});

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < (waitTimeMs * iterations * 2) / cores)
			{
				if (iterations == successful.Counter) break;
				Thread.Sleep(100);
			}
			sw.Stop();
			Assert.AreEqual(iterations, successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
		}

		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingBatchSizeWithFireAndForget()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			const int batchSize = 4;
			const int batchTimeoutMs = 10;

			int cores = Environment.ProcessorCount;
			var counter = new CounterContainer();
			var successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var parallelOptions = new ParallelOptions
			{
				MaxDegreeOfParallelism = -1
			};

			Parallel.For((long)0, iterations, parallelOptions, (i) =>
			{
				try
				{
					var requestObject = new RequestObject(i);
					callsHandler.FireAndForget(requestObject, 1000);
				}
				catch (TimeoutException exception)
				{
					Console.WriteLine(exception);
				}
			});

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < (waitTimeMs * iterations * 2) / cores)
			{
				if (iterations == callsHandler.CallsCount) break;
				Thread.Sleep(100);
			}
			sw.Stop();
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
		}


		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingTimeout()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			const int batchSize = 100;
			const int batchTimeoutMs = 1;

			int cores = Environment.ProcessorCount;
			var counter = new CounterContainer();
			var successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var parallelOptions = new ParallelOptions
			{
				MaxDegreeOfParallelism = -1
			};

			Parallel.For((long)0, iterations, parallelOptions, (i) =>
			{
				try
				{
					var requestObject = new RequestObject(i);
					counter.Add(callsHandler.DoRequestAndWait(requestObject, 1000));
					if (i == -requestObject.GetReturnAs<long>()) successful.Increment();
				}
				catch (TimeoutException exception)
				{
					Console.WriteLine(exception);
				}
			});

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < (waitTimeMs * iterations * 2) / cores)
			{
				if (iterations == successful.Counter) break;
				Thread.Sleep(100);
			}
			sw.Stop();
			Assert.AreEqual(iterations, successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
		}


		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingTimeoutFireAndForget()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			const int batchSize = 100;
			const int batchTimeoutMs = 1;

			int cores = Environment.ProcessorCount;
			var counter = new CounterContainer();
			var successful = new CounterContainer();
			var callsHandler = new SampleSyncTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var parallelOptions = new ParallelOptions
			{
				MaxDegreeOfParallelism = -1
			};

			Parallel.For((long)0, iterations, parallelOptions, (i) =>
			{
				try
				{
					var requestObject = new RequestObject(i);
					callsHandler.FireAndForget(requestObject, 1000);
				}
				catch (TimeoutException exception)
				{
					Console.WriteLine(exception);
				}
			});

			var sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < (waitTimeMs * iterations * 4) / cores)
			{
				if (iterations == callsHandler.CallsCount) break;
				Thread.Sleep(100);
			}
			sw.Stop();
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
		}
	}
}
