using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using ZakCore.Utils.Logging;
using ZakThread.Test.Threading.Messaging;
using ZakThread.Test.Threading.Simple;
using ZakThread.Threading;
using ZakThread.Threading.Enums;
using ZakThread.Threading.ThreadManagerInternals;
using Assert = NUnit.Framework.Assert;

namespace ZakThread.Test.Threading
{
	[TestFixture]
	public class ThreadManagerWithMessagingTest
	{
		[Test]
		public void ItShouldBePossibleToRegisterAMessageByChildThread()
		{
			var threadManagerMock = new Mock<IThreadManager>();
			threadManagerMock.Setup(m => m.SendMessageToThread(It.IsAny<InternalMessage>()));
			var messageThread = new MessageThread(true,NullLogger.Create(), "test")
				{
					Manager = threadManagerMock.Object
				};
			messageThread.RegisterMessages();
			threadManagerMock.Verify(r => r.SendMessageToThread(It.Is<InternalMessage>(q => q.Content as Type == typeof(TestMessage))));
		}

		[Test]
		public void ItShouldNotBePossibleToRegisterAMessageFromThreadManager()
		{

			var threadManager = new ThreadManager(NullLogger.Create());
			var privateObject = new PrivateObject(threadManager);
			threadManager.RunThread();
			Thread.Sleep(100);			
			Assert.IsTrue(threadManager.Status == RunningStatus.Running);
			threadManager.SendMessageToThread(new InternalMessage(InternalMessageTypes.RegisterMessageType,
																							typeof(TestMessage)));
			Thread.Sleep(100);
			var result = privateObject.Invoke("IsTypeRegistered", threadManager.ThreadName, typeof(TestMessage)) as Boolean?;
			Assert.IsNotNull(result);
			Assert.AreEqual(false, result);
			Thread.Sleep(100);
			threadManager.Terminate();
			Thread.Sleep(500);
			Assert.IsTrue(threadManager.Status == RunningStatus.Halted);
		}


		[Test]
		public void ItShouldBePossibleToRegisterAMessageFromAChildThread()
		{
			var subThread = new SimpleMessageThreadConsumer(1, "SUBTHREAD");
			var threadManager = new ThreadManager(NullLogger.Create());
			var privateObject = new PrivateObject(threadManager);
			threadManager.RunThread();
			threadManager.AddThread(subThread);
			threadManager.RunThread(subThread.ThreadName);
			Thread.Sleep(100);
			Assert.IsTrue(threadManager.Status == RunningStatus.Running);
			threadManager.SendMessageToThread(new InternalMessage(InternalMessageTypes.RegisterMessageType,
																							typeof(TestMessage)){SourceThread = subThread.ThreadName});
			Thread.Sleep(100);
			var result = privateObject.Invoke("IsTypeRegistered", subThread.ThreadName, typeof(TestMessage)) as Boolean?;
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result);
			Thread.Sleep(100);
			threadManager.Terminate();
			Thread.Sleep(500);
			Assert.IsTrue(threadManager.Status == RunningStatus.Halted);
		}


		[Test]
		public void ItShouldBePossibleToRegisterAMessageFromAChildThreadAutomatically()
		{
			var subThread = new MessageThread(true,NullLogger.Create(),"SUBTHREAD");
			var subThread2 = new MessageThread(false,NullLogger.Create(), "SUBTHREAD2");
			var threadManager = new ThreadManager(NullLogger.Create());
			var privateObject = new PrivateObject(threadManager);
			threadManager.RunThread();
			threadManager.AddThread(subThread);
			threadManager.AddThread(subThread2);
			Thread.Sleep(100);

			Assert.IsTrue(threadManager.Status == RunningStatus.Running);
			var result = privateObject.Invoke("IsTypeRegistered", subThread.ThreadName, typeof(TestMessage)) as Boolean?;
			Assert.IsNotNull(result);
			Assert.AreEqual(true, result);

			result = privateObject.Invoke("IsTypeRegistered", subThread2.ThreadName, typeof(TestMessage)) as Boolean?;
			Assert.IsNotNull(result);
			Assert.AreEqual(false, result);

			threadManager.SendMessageToThread(new TestMessage());
			Thread.Sleep(1000);
			Assert.IsTrue(subThread.MessagesCount==1);
			Assert.IsFalse(subThread2.MessagesCount == 1);
			Thread.Sleep(100);
			threadManager.Terminate();
			Thread.Sleep(500);
			Assert.IsTrue(threadManager.Status == RunningStatus.Halted);
		}


		[Test]
		public void ItShouldBePossibleToSendAMessageFromAChildThread()
		{
			var subThread = new MessageThread(true, NullLogger.Create(), "SUBTHREAD");
			var subThread2 = new MessageThread(false, NullLogger.Create(), "SUBTHREAD2");
			var threadManager = new ThreadManager(NullLogger.Create());
			var privateObject = new PrivateObject(threadManager);
			threadManager.RunThread();
			threadManager.AddThread(subThread);
			threadManager.AddThread(subThread2);
			Thread.Sleep(100);

			Assert.IsTrue(threadManager.Status == RunningStatus.Running);
			threadManager.SendMessageToThread(new TestMessage());
			subThread2.SendingMessage = 10;
			Thread.Sleep(1000);
			Assert.IsTrue(subThread.MessagesCount == 11);
			Assert.IsFalse(subThread2.MessagesCount > 0);
			Thread.Sleep(100);
			threadManager.Terminate();
			Thread.Sleep(500);
			Assert.IsTrue(threadManager.Status == RunningStatus.Halted);
		}


		[Test]
		public void ItShouldBePossibleToSendAnInternalMessageFromAChildThread()
		{
			var subThread = new MessageThread(true, NullLogger.Create(), "SUBTHREAD");
			var subThread2 = new MessageThread(false, NullLogger.Create(), "SUBTHREAD2");
			var threadManager = new ThreadManager(NullLogger.Create());
			var privateObject = new PrivateObject(threadManager);
			threadManager.RunThread();
			threadManager.AddThread(subThread);
			threadManager.AddThread(subThread2);
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Running, threadManager.Status);
			threadManager.SendMessageToThread(new TestMessage());
			subThread2.TerminateMessage = 1;
			Thread.Sleep(100);
			threadManager.Terminate();
			Thread.Sleep(500);
			Assert.AreEqual(RunningStatus.Halted, threadManager.Status);
			threadManager.Terminate();
		}
	}
}
