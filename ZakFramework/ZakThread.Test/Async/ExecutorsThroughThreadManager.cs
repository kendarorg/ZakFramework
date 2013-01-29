using System;
using System.Threading;
using NUnit.Framework;
using ZakCore.Utils.Logging;
using ZakThread.Async;
using ZakThread.Test.Async.SampleObjects;
using ZakThread.Test.Threading.Messaging;
using ZakThread.Test.Threading.Simple;
using ZakThread.Threading;
using ZakThread.Threading.Enums;

namespace ZakThread.Test.Async
{
	[TestFixture]
	class ExecutorsThroughThreadManager
	{
		[Test]
		public void ItShouldBePossibleToAttachASyncHandlerToAThreadManager()
		{
			var subThread = new MessageThread(true, NullLogger.Create(), "SUBTHREAD");
			var subThread2 = new MessageThread(false, NullLogger.Create(), "SUBTHREAD2");
			var subThread3 = new SyncTaskHandlerWithMessageRegistration("SYNCTH", 2);
			var threadManager = new ThreadManager(NullLogger.Create());
			
			threadManager.RunThread();
			threadManager.AddThread(subThread);
			threadManager.AddThread(subThread2);
			threadManager.AddThread(subThread3);
			Thread.Sleep(100);

			Assert.IsTrue(threadManager.Status == RunningStatus.Running);
			threadManager.SendMessageToThread(new TestMessage());
			subThread2.SendingMessage = 10;
			Thread.Sleep(1000);
			Assert.IsTrue(subThread.MessagesCount == 11);
			Assert.IsFalse(subThread2.MessagesCount > 0);
			Assert.IsTrue(subThread3.MessagesCount.Counter == 11);
			Thread.Sleep(100);
			threadManager.Terminate();
			Thread.Sleep(500);
			Assert.IsTrue(subThread.Status == RunningStatus.Halted);
			Assert.IsTrue(subThread2.Status == RunningStatus.Halted);
			Assert.IsTrue(subThread3.Status == RunningStatus.Halted);
		}

		[Test]
		public void CloningARequestObjectMessageShouldGenerateException()
		{
			NotSupportedException expex = null;
			object cloned = null;
			var rom = new RequestObjectMessage(new BaseRequestObject(), 1);
			try
			{
			 cloned = rom.Clone();
			}
			catch (NotSupportedException ex)
			{
				expex = ex;
			}
			Assert.IsNull(cloned);
			Assert.IsTrue(rom.TimeStamp < DateTime.UtcNow);
			Assert.AreNotEqual(rom.Id,Guid.Empty);
			Assert.IsNotNull(expex);
		}
	}
}
