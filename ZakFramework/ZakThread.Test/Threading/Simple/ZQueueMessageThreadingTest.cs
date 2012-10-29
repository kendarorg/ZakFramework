using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZakThread.Test.Threading.Simple;
using ZakThread.Threading.Enums;

namespace ZQueue.Test.Threading.Simple
{
	[TestClass]
	public class ZQueueMessageThreadingTest
	{
		[TestMethod]
		public void ItShouldBePossibleToInitializeAMessageingThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			string expectedTestName = testName.ToUpperInvariant();

			var th = new SimpleMessageThreadConsumer(sleepTime,testName);

			Assert.AreEqual(expectedTestName, th.ThreadName);
			Assert.AreEqual(null, th.LastError);
			Assert.AreEqual(RunningStatus.None, th.Status);
		}

		[TestMethod]
		public void ItShouldBePossibleToStartAndStopAMessagingThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			var th = new SimpleMessageThreadConsumer(sleepTime,testName);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Running, th.Status);

			th.Terminate();
			Thread.Sleep(200);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
		}

		[TestMethod]
		public void ItShouldBePossibleToReceiveMessagesForAMessagingThread()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			var th = new SimpleMessageThreadConsumer(sleepTime,testName);

			th.RunThread();
			Thread.Sleep(100);

			for (int i = 0; i < messagesToSend; i++)
			{
				th.SendMessageToThread(new TestMessage());
			}

			Assert.AreEqual(RunningStatus.Running, th.Status);
			Thread.Sleep(100);

			th.Terminate();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
			Assert.AreEqual(messagesToSend,th.HandledMessages);
		}

		[TestMethod]
		public void ItShouldBePossibleToInterceptExceptionsOnAMessagingThreadDuringTheMessageHandling()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			var expectedException = new Exception("TEST");
			var th = new SimpleMessageThreadConsumer(sleepTime, testName,false);

			th.RunThread();
			Thread.Sleep(100);

			for (int i = 0; i < messagesToSend; i++)
			{
				th.SendMessageToThread(new TestMessage());
			}

			th.ThrowExceptionOnMessageHandling = expectedException;

			for (int i = 0; i < messagesToSend; i++)
			{
				th.SendMessageToThread(new TestMessage());
			}
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.ExceptionThrown, th.Status);
			Exception expectedEx = th.LastError;
			Assert.IsNotNull(expectedEx);
			Assert.AreEqual(expectedEx.Message, th.ThrowExceptionOnMessageHandling.Message);


			Assert.IsTrue(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
			Assert.AreNotEqual(messagesToSend*2, th.HandledMessages);
		}


		[TestMethod]
		public void ItShouldBePossibleToRestartAutomaticallyAMessageThreadOnErrorInMessageHandling()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			const bool restartOnError = true;
			var expectedException = new Exception("TEST");
			var th = new SimpleMessageThreadConsumer(sleepTime,testName, restartOnError);

			th.RunThread();
			Thread.Sleep(100);

			for (int i = 0; i < messagesToSend; i++)
			{
				th.SendMessageToThread(new TestMessage());
			}
			Thread.Sleep(100);
			th.ThrowExceptionOnMessageHandling = expectedException;
			Thread.Sleep(100);
			for (int i = 0; i < messagesToSend; i++)
			{
				th.SendMessageToThread(new TestMessage());
			}
			Thread.Sleep(100);
			th.Terminate();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);
			

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
			Assert.AreEqual(messagesToSend*2, th.HandledMessages);
		}
	}
}
