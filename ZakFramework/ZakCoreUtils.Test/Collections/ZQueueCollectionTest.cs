using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZakCoreUtils.Test.Collections
{
	[TestClass]
	public class ZQueueCollectionTest
	{
		[TestMethod]
		public void ItShouldBePossibleToReceiveAllStuffPuttedOnTheQueue()
		{
			const int threadsCount = 100;
			const int toSendElements = 10000;
			ZQueueCollectionTestUtils.Initialize(toSendElements);

			var threadList = new List<Thread>();

			for (int i = 0; i < threadsCount/2; i++)
			{
				threadList.Add(new Thread(ZQueueCollectionTestUtils.ProducerThread));
				threadList.Add(new Thread(ZQueueCollectionTestUtils.ConsumerThread));
			}

			var sw = new Stopwatch();
			sw.Start();

			foreach (Thread t in threadList)
			{
				t.Start();
			}

			while (!ZQueueCollectionTestUtils.IsSendCompleted && sw.ElapsedMilliseconds < toSendElements)
			{
				Thread.Sleep(10);
			}

			Assert.IsTrue(ZQueueCollectionTestUtils.IsSendCompleted, "Did not completed the send of data");
		}
	}
}