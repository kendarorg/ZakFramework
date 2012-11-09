using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZakCore.Utils.Collections;

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

		[TestMethod]
		public void ItShouldBePossibleToClearAQueueAndDequeueASingleItem()
		{
			var lfq = new LockFreeQueue<string>();
			var lele = new List<string>();
			for (int i = 0; i < ENQUEUED_DATA; i++)
			{
				lele.Add("TEST_" + i);
			}
			lfq.Enqueue(lele);
			Assert.AreEqual("TEST_0", lfq.DequeueSingle());
			lfq.Clear();
			Assert.IsNull(lfq.DequeueSingle());	
		}


		protected const int ENQUEUED_DATA = 10;
	}
}