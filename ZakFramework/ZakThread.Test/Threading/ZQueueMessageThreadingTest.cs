using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using ZakCore.Utils.Logging;
using ZakThread.Logging;
using ZakThread.Test.Threading.Simple;
using ZakThread.Threading;
using ZakThread.Threading.Enums;

namespace ZakThread.Test.Threading
{
	[TestFixture]
	public class ZQueueMessageThreadingTest
	{
		[Test]
		public void ItShouldBePossibleToInitializeAMessageingThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			string expectedTestName = testName.ToUpperInvariant();

			var th = new SimpleMessageThreadConsumer(sleepTime, testName);

			Assert.AreEqual(expectedTestName, th.ThreadName);
			Assert.AreEqual(null, th.LastError);
			Assert.AreEqual(RunningStatus.None, th.Status);
		}

		[Test]
		public void ItShouldBePossibleToStartAndStopAMessagingThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			var th = new SimpleMessageThreadConsumer(sleepTime, testName);

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

		[Test]
		public void ItShouldBePossibleToReceiveMessagesForAMessagingThread()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			var th = new SimpleMessageThreadConsumer(sleepTime, testName);

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
			Assert.AreEqual(messagesToSend, th.HandledMessages);
		}

		[Test]
		public void ItShouldBePossibleToInterceptExceptionsOnAMessagingThreadDuringTheMessageHandling()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			var expectedException = new Exception("TEST");
			var th = new SimpleMessageThreadConsumer(sleepTime, testName, false);

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
			Assert.AreEqual(expectedEx.Message, expectedException.Message);


			Assert.IsTrue(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
			Assert.AreNotEqual(messagesToSend*2, th.HandledMessages);
		}


		[Test]
		public void ItShouldBePossibleToRestartAutomaticallyAMessageThreadOnErrorInMessageHandling()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			var expectedException = new Exception("TEST");
			var th = new SimpleMessageThreadConsumer(sleepTime, testName);

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
			Thread.Sleep(500);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);


			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
			Assert.AreEqual(messagesToSend*2, th.HandledMessages);
		}

		[Test]
		public void ItShouldBePossibleToPeekMessagesForAMessagingThread()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			var th = new SimpleMessageThreadConsumer(sleepTime, testName);
			var messages = new List<TestMessage>();
			th.ForwardMessages = true;
			th.RunThread();
			Thread.Sleep(100);

			for (int i = 0; i < messagesToSend; i++)
			{
				var newMsg = new TestMessage();
				th.SendMessageToThread(newMsg);
				messages.Add(newMsg);
			}

			Assert.AreEqual(RunningStatus.Running, th.Status);
			Thread.Sleep(100);

			var msg = th.PeekMessageFromThread() as TestMessage;
			Assert.IsNotNull(msg);
			Assert.AreSame(msg, messages[0]);

			var resultingMessages = new List<TestMessage>();
			foreach (var item in th.PeekMessagesFromThread())
			{
				msg = item as TestMessage;
				if(msg!=null)
				resultingMessages.Add(msg);
			}
			Assert.AreEqual(messages.Count-1,resultingMessages.Count);

			th.Terminate();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
			Assert.AreEqual(messagesToSend, th.HandledMessages);
			th.Dispose();
		}


		[Test]
		public void ItShouldBePossibleToGetAndSetAThreadManager()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			var th = new SimpleMessageThreadConsumer(sleepTime, testName);
			var tm = new ThreadManager(NullLogger.Create());
			var tm1 = new ThreadManager(NullLogger.Create());
			th.Manager = tm;
			th.Manager = tm1;
			Assert.AreNotSame(tm1,th.Manager);
			Assert.AreSame(tm, th.Manager);

		}

		[Test]
		public void ItShouldBePossibleToTerminateTheThreadUponReceivingAMessage()
		{
			const int sleepTime = 1;
			const string testName = "TestThread";
			const int messagesToSend = 100;
			var th = new SimpleMessageThreadConsumer(sleepTime, testName);
			th.ReceiveAStopMessage = true;

			th.RunThread();
			Thread.Sleep(100);
			Assert.AreEqual(RunningStatus.Running, th.Status);

			for (int i = 0; i < messagesToSend; i++)
			{
				th.SendMessageToThread(new TestMessage());
			}

			Thread.Sleep(100);
			
			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
			Assert.AreEqual(1, th.HandledMessages);
		}
	}
}