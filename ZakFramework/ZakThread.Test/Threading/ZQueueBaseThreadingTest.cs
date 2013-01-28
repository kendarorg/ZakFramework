using System;
using System.ComponentModel;
using System.Threading;
using NUnit.Framework;
using ZakCore.Utils.Logging;
using ZakThread.Test.Threading.Simple;
using ZakThread.Threading;
using ZakThread.Threading.Enums;

namespace ZakThread.Test.Threading
{
	[TestFixture]
	public class ZQueueBaseThreadingTest
	{
		[Test]
		public void ItShouldBePossibleToInitializeAThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			string expectedTestName = testName.ToUpperInvariant();

			var th = new SimpleThread(sleepTime, testName);
			Assert.IsNotNull(th.Logger);
			Assert.IsTrue(th.Logger is NullLogger);

			Assert.AreEqual(expectedTestName, th.ThreadName);
			Assert.AreEqual(null, th.LastError);
			Assert.AreEqual(RunningStatus.None, th.Status);
		}

		[Test]
		public void ItShouldBePossibleToStartAndStopAThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(1, BaseThread.ThreadCounter);
			Assert.AreEqual(RunningStatus.Running, th.Status);
			Assert.IsTrue(th.CyclesRun > 0);
			th.Terminate();
			Thread.Sleep(200);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			var runs = th.CyclesRun;
			Thread.Sleep(100);
			Assert.AreEqual(runs,th.CyclesRun);
			Assert.IsNull(th.LastError);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
		}

		[Test]
		public void ItShouldBePOssibleToTerminateANonStartedThread()
		{
			const int sleepTime = 50;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);
			th.Terminate();
			th.WaitTermination(1000);
			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);

			Assert.IsFalse(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
		}


		[Test]
		public void ItShouldBePOssibleToTerminateAnHaltingThread()
		{
			const int sleepTime = 1000;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);
			th.RunThread();
			Thread.Sleep(100);
			th.Terminate();
			th.Terminate();
			Assert.AreEqual(RunningStatus.Halting, th.Status);
			Assert.IsNull(th.LastError);
			th.WaitTermination(1000);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
		}


		[Test]
		public void ItShouldBePOssibleToTerminateANotInitializedThread()
		{
			const int sleepTime = 1000;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);
			th.Terminate();
			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);
			th.WaitTermination(1000);

			Assert.IsFalse(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
		}




		[Test]
		public void ItShouldNotBePossibleToWaitForTerminationOfANotHaltingThread()
		{
			const int sleepTime = 1000;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);
			th.RunThread();
			Thread.Sleep(100);
			InvalidAsynchronousStateException resex = null;
			try
			{
				th.WaitTermination(1000);
			}
			catch (InvalidAsynchronousStateException ex)
			{
				resex = ex;
			}


			Assert.IsNotNull(resex);
			Assert.IsTrue(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);

			th.Terminate(true);
		}


		[Test]
		public void ItShouldBePossibleToStartAndStopAThreadWithTheDefaultTerminationTime()
		{
			const int sleepTime = 50;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Running, th.Status);

			th.Terminate();
			th.WaitTermination(90);
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
		}


		[Test]
		public void ItShouldBePossibleToInterruptAThreadDuringTheCyclicExecution()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName) {ExitAfterFirstCycle = true};

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Halted, th.Status);

			th.Terminate();
			Thread.Sleep(200);

			Assert.IsNull(th.LastError);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
		}

		[Test]
		public void ItShouldBePossibleToStartAndAbortAThreadWithoutCleanUp()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";

			var th = new SimpleThread(sleepTime, testName);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Running, th.Status);

			th.Terminate(true);
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Aborted, th.Status);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
		}

		[Test]
		public void ItShouldBePossibleToStartAndStopAThreadDetectingItSHalting()
		{
			const int sleepTime = 500;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Running, th.Status);

			th.Terminate();
			th.WaitTermination(1000);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
		}

		[Test]
		public void ItShouldBePossibleToInterceptThreadAbortExceptions()
		{
			const int sleepTime = 500;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName) { ThrowThreadAbortException = true };

			th.RunThread();
			Thread.Sleep(100);
			th.Terminate(true);
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Aborted, th.Status);

			var exceptionThrown = th.LastError;
			Assert.IsNotNull(exceptionThrown);
			Assert.AreEqual("ThreadAbortException", exceptionThrown.Message);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
		}



		[Test]
		public void ItShouldBePossibleToTerminateAnAlreadyTerminatedThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);

			th.RunThread();
			Thread.Sleep(100);
			th.Terminate();
			th.WaitTermination(1000);

			Assert.AreEqual(RunningStatus.Halted, th.Status);

			Assert.IsNull(th.LastError);
			
			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);

			th.Terminate();
			th.WaitTermination(1000);

			Assert.AreEqual(RunningStatus.Halted, th.Status);

			Assert.IsNull(th.LastError);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsCleanedUp);
		}


		[Test]
		public void ItShouldBePossibleToBlockAThreadWaitingForASpecificTimeout()
		{
			const int sleepTime = 50000;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName) { ThrowThreadAbortException = true };

			th.RunThread();
			Thread.Sleep(100);
			TimeoutException exceptionThrown = null;
			try
			{
				th.Terminate();
				th.WaitTermination(10);
			}
			catch (TimeoutException ex)
			{
				exceptionThrown = ex;
			}
			Assert.AreEqual(RunningStatus.Halting, th.Status);
			
			Assert.IsNotNull(exceptionThrown);
			
			Assert.IsTrue(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
			th.Terminate(true);

			Thread.Sleep(100);
			Assert.AreEqual(RunningStatus.Aborted, th.Status);
			Thread.Sleep(1000);
		}

		[Test]
		public void ItShouldBePossibleToStartAndDisposeIt()
		{
			const int sleepTime = 500;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Running, th.Status);

			th.Dispose();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Aborted, th.Status);
			Thread.Sleep(1000);

			var lastError = th.LastError;
			Assert.IsNull(lastError);

			Assert.IsTrue(th.IsInitialized);
		}

		[Test]
		public void ItShouldBePossibleToInterceptAnExceptionOnInitialization()
		{
			const int sleepTime = 10;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName, false);
			var ex = new Exception("TEST");
			th.ThrowExceptionOnInitialization = ex;

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.ExceptionThrown, th.Status);
			Exception expectedEx = th.LastError;
			Assert.IsNotNull(expectedEx);
			Assert.AreEqual(expectedEx.Message, th.ThrowExceptionOnInitialization.Message);

			Assert.IsFalse(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
			Assert.IsTrue(th.IsExceptionHandled);
		}

		[Test]
		public void ItShouldBePossibleToInterceptAnExceptionOnCyclicRunning()
		{
			const int sleepTime = 10;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName, false);
			var ex = new Exception("TEST");
			th.ThrowExceptionOnCyclicExecution = ex;

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.ExceptionThrown, th.Status);
			Exception expectedEx = th.LastError;
			Assert.IsNotNull(expectedEx);
			Assert.AreEqual(expectedEx.Message, th.ThrowExceptionOnCyclicExecution.Message);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
			Assert.IsTrue(th.IsExceptionHandled);
		}

		[Test]
		public void ItShouldBePossibleToInterceptAnExceptionOnCleanUp()
		{
			const int sleepTime = 10;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName, false);
			var ex = new Exception("TEST");
			th.ThrowExceptionOnCleanUp = ex;

			th.RunThread();
			Thread.Sleep(100);
			th.Terminate();
			Thread.Sleep(100);
			Exception expectedEx = th.LastError;
			Assert.AreEqual(RunningStatus.AbortedOnCleanup, th.Status);

			Assert.IsNotNull(expectedEx);
			Assert.AreEqual(expectedEx.Message, th.ThrowExceptionOnCleanUp.Message);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
			Assert.IsTrue(th.IsExceptionHandled);
		}

		[Test]
		public void ItShouldBePossibleToRestartAThreadThatFailed()
		{
			const int sleepTime = 10;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);
			var ex = new Exception("TEST");
			th.ThrowExceptionOnCyclicExecution = ex;
			th.ResetExceptionAfterThrow = true;

			th.RunThread();
			Thread.Sleep(1000);
			th.Terminate();
			Thread.Sleep(1000);
			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsExceptionHandled);
			Assert.IsTrue(th.IsCleanedUp);

			Exception expectedEx = th.LastError;
			Assert.IsNull(expectedEx);
		}



		[Test]
		public void ItShouldBePossibleToRestartAThreadThatFailedWithStrangeBehaviour()
		{
			const int sleepTime = 10;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName,false);
			var ex = new Exception("TEST");
			th.ThrowExceptionOnCyclicExecution = ex;
			th.ResetExceptionAfterThrow = true;

			th.RunThread();
			Thread.Sleep(1000);
			th.Terminate();
			Thread.Sleep(1000);
			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsInitialized);
			Assert.IsTrue(th.IsExceptionHandled);
			Assert.IsFalse(th.IsCleanedUp);

			Exception expectedEx = th.LastError;
			Assert.IsNotNull(expectedEx);
		}
	}
}