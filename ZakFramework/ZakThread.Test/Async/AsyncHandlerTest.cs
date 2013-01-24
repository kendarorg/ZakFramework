using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ZakThread.Test.Async
{
	[TestFixture]
	public class AsyncHandlerTest
	{
		[Test]
		public void ItShouldBePossibleToRunTasksInParallelDelegatingThemToTheThread()
		{
			const int iterations = 100;
			const int waitTimeMs = 10;
			int cores = Environment.ProcessorCount;
			var counter = new CounterContainer();
			var successful = new CounterContainer();
			var callsHandler = new SampleTasksHandler("TEST", waitTimeMs);
			callsHandler.RunThread();
			Thread.Sleep(100);
			var parallelOptions = new ParallelOptions
				{
					MaxDegreeOfParallelism = cores
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

			Thread.Sleep((waitTimeMs * iterations) / cores);
			Assert.AreEqual(iterations,successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
		}

		[Test]
		public void ItShouldBePossibleToRunTasksInBatchCheckingBatchSize()
		{
			const int iterations = 100;
			const int waitTimeMs = 1;
			const int batchSize = 10;
			const int batchTimeoutMs = 10;

			int cores = Environment.ProcessorCount;
			var counter = new CounterContainer();
			var successful = new CounterContainer();
			var callsHandler = new SampleTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
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

			Thread.Sleep((waitTimeMs * iterations) / cores);
			Assert.AreEqual(iterations, successful.Counter);
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
			var callsHandler = new SampleTasksHandler("TEST", waitTimeMs, batchSize, batchTimeoutMs);
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

			Thread.Sleep((waitTimeMs * iterations) / cores);
			Assert.AreEqual(iterations, successful.Counter);
			Assert.AreEqual(iterations, callsHandler.CallsCount);
			callsHandler.Terminate();
		}
	}
}
