using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZQueue.Threading;
using ZQueue.Threading.Enums;
using ZQueue.Threading.Model;

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

			var st = new SimpleMessageThreadConsumer(sleepTime) {ThreadName = testName};
			var th = new MessageThread( st);

			Assert.AreEqual(expectedTestName, th.ThreadName);
			Assert.AreEqual(null, th.LastError);
			Assert.AreEqual(RunningStatus.None, th.Status);
		}

		[TestMethod]
		public void ItShouldBePossibleToStartAndStopAMessagingThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			var st = new SimpleMessageThreadConsumer(sleepTime) {ThreadName = testName};
			var th = new MessageThread( st);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Running, th.Status);

			th.Terminate();
			Thread.Sleep(200);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);

			Assert.IsTrue(st.IsInitialized);
			Assert.IsTrue(st.IsCleanedUp);
		}

		[TestMethod]
		public void ItShouldBePossibleToReceiveMessagesForAMessagingThread()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			var st = new SimpleMessageThreadConsumer(sleepTime) {ThreadName = testName};
			var th = new MessageThread( st);

			th.RunThread();
			Thread.Sleep(100);

			for (int i = 0; i < messagesToSend; i++)
			{
				th.AddIncomingMessage(new Message());
			}

			Assert.AreEqual(RunningStatus.Running, th.Status);
			Thread.Sleep(100);

			th.Terminate();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);

			Assert.IsTrue(st.IsInitialized);
			Assert.IsTrue(st.IsCleanedUp);
			Assert.AreEqual(messagesToSend,st.HandledMessages);
		}

		[TestMethod]
		public void ItShouldBePossibleToInterceptExceptionsOnAMessagingThreadDuringTheMessageHandling()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			var expectedException = new Exception("TEST");
			var st = new SimpleMessageThreadConsumer(sleepTime) {ThreadName = testName};
			var th = new MessageThread( st);

			th.RunThread();
			Thread.Sleep(100);

			for (int i = 0; i < messagesToSend; i++)
			{
				th.AddIncomingMessage(new Message());
			}

			st.ThrowExceptionOnMessageHandling = expectedException;

			for (int i = 0; i < messagesToSend; i++)
			{
				th.AddIncomingMessage(new Message());
			}
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.ExceptionThrown, th.Status);
			Exception expectedEx = th.LastError;
			Assert.IsNotNull(expectedEx);
			Assert.AreEqual(expectedEx.Message, st.ThrowExceptionOnMessageHandling.Message);


			Assert.IsTrue(st.IsInitialized);
			Assert.IsFalse(st.IsCleanedUp);
			Assert.AreNotEqual(messagesToSend*2, st.HandledMessages);
		}


		[TestMethod]
		public void ItShouldBePossibleToRestartAutomaticallyAMessageThreadOnErrorInMessageHandling()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			const bool restartOnError = true;
			var expectedException = new Exception("TEST");
			var st = new SimpleMessageThreadConsumer(sleepTime) {ThreadName = testName, RestartOnError = restartOnError};
			var th = new MessageThread( st);

			th.RunThread();
			Thread.Sleep(100);

			for (int i = 0; i < messagesToSend; i++)
			{
				th.AddIncomingMessage(new Message());
			}
			Thread.Sleep(100);
			st.ThrowExceptionOnMessageHandling = expectedException;
			Thread.Sleep(100);
			for (int i = 0; i < messagesToSend; i++)
			{
				th.AddIncomingMessage(new Message());
			}
			Thread.Sleep(100);
			th.Terminate();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);
			

			Assert.IsTrue(st.IsInitialized);
			Assert.IsTrue(st.IsCleanedUp);
			Assert.AreEqual(messagesToSend*2, st.HandledMessages);
		}
	}
}
