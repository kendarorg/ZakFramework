using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ZakCore.Utils.Monitoring
{
	internal class Class1
	{
		private PerformanceCounter _totalOperations;

		public void CreatePerfCounters()
		{
			if (!PerformanceCounterCategory.Exists("MyCategory"))
			{
				var counters = new CounterCreationDataCollection();

				// 1. counter for counting totals: PerformanceCounterType.NumberOfItems32
				var totalOps = new CounterCreationData
					{
						CounterName = "# operations executed",
						CounterHelp = "Total number of operations executed",
						CounterType = PerformanceCounterType.NumberOfItems32
					};
				counters.Add(totalOps);

				// 2. counter for counting operations per second:
				//        PerformanceCounterType.RateOfCountsPerSecond32
				var opsPerSecond = new CounterCreationData
					{
						CounterName = "# operations / sec",
						CounterHelp = "Number of operations executed per second",
						CounterType = PerformanceCounterType.RateOfCountsPerSecond32
					};
				counters.Add(opsPerSecond);

				// 3. counter for counting average time per operation:
				//                 PerformanceCounterType.AverageTimer32
				var avgDuration = new CounterCreationData
					{
						CounterName = "average time per operation",
						CounterHelp = "Average duration per operation execution",
						CounterType = PerformanceCounterType.AverageTimer32
					};
				counters.Add(avgDuration);

				// 4. base counter for counting average time
				//         per operation: PerformanceCounterType.AverageBase
				var avgDurationBase = new CounterCreationData
					{
						CounterName = "average time per operation base",
						CounterHelp = "Average duration per operation execution base",
						CounterType = PerformanceCounterType.AverageBase
					};
				counters.Add(avgDurationBase);


				// create new category with the counters above
				PerformanceCounterCategory.Create("MyCategory",
																					"Sample category for Codeproject", PerformanceCounterCategoryType.SingleInstance,
																					counters);
			}

			// create counters to work with
			_totalOperations = new PerformanceCounter { CategoryName = "MyCategory", CounterName = "# operations executed", MachineName = ".", ReadOnly = false };

			_operationsPerSecond = new PerformanceCounter { CategoryName = "MyCategory", CounterName = "# operations / sec", MachineName = ".", ReadOnly = false };

			_averageDuration = new PerformanceCounter { CategoryName = "MyCategory", CounterName = "average time per operation", MachineName = ".", ReadOnly = false };

			_averageDurationBase = new PerformanceCounter { CategoryName = "MyCategory", CounterName = "average time per operation base", MachineName = ".", ReadOnly = false };
		}

		[DllImport("Kernel32.dll")]
		public static extern void QueryPerformanceCounter(ref long ticks);

		public void EvalDuration()
		{
			/*
			PerformanceCounterSample test = new PerformanceCounterSample();
			Random rand = new Random();
			long startTime = 0;
			long endTime = 0;

			for (int i = 0; i < 1000; i++)
			{
				// measure starting time
				QueryPerformanceCounter(ref startTime);

				System.Threading.Thread.Sleep(rand.Next(500));

				// measure ending time
				QueryPerformanceCounter(ref endTime);

				// do some processing
				test.DoSomeProcessing(endTime - startTime);
			}*/
		}


		public void SetValuesOnPerformanceCounters()
		{
			const long ticks = 10;
			// simply increment the counters
			_totalOperations.Increment();
			_operationsPerSecond.Increment();
			// increment the timer by the time cost of the operation
			_averageDuration.IncrementBy(ticks);
			// increment base counter only by 1
			_averageDurationBase.Increment();
		}

		public PerformanceCounter _operationsPerSecond;

		public PerformanceCounter _averageDuration;

		public PerformanceCounter _averageDurationBase;
	}
}