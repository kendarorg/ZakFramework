using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZakThread.Threading.Enums;

namespace ZakThread.Test.Threading.Simple
{
	[TestClass]
	public class ZQueueBaseThreadingTest
	{
		[TestMethod]
		public void ItShouldBePossibleToInitializeAThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			string expectedTestName = testName.ToUpperInvariant();

			var th = new SimpleThread(sleepTime, testName);

			Assert.AreEqual(expectedTestName, th.ThreadName);
			Assert.AreEqual(null, th.LastError);
			Assert.AreEqual(RunningStatus.None, th.Status);
		}

		[TestMethod]
		public void ItShouldBePossibleToStartAndStopAThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName);

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
		public void ItShouldBePossibleToStartAndStopAThreadWithTheDefaultTerminationTime()
		{
			const int sleepTime = 100;
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


		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
		public void ItShouldBePossibleToInterceptThreadAbortExceptions()
		{
			const int sleepTime = 500;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName) {ThrowThreadAbortException = true};

			th.RunThread();
			Thread.Sleep(100);
			th.Terminate(true);

			Assert.AreEqual(RunningStatus.Aborted, th.Status);

			var exceptionThrown = th.LastError;
			Assert.IsNotNull(exceptionThrown);
			Assert.IsTrue(exceptionThrown is ThreadAbortException);

			Assert.IsTrue(th.IsInitialized);
			Assert.IsFalse(th.IsCleanedUp);
		}



		[TestMethod]
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


		[TestMethod]
		public void ItShouldBePossibleToBlockAThreadWaitingForASpecificTimeout()
		{
			const int sleepTime = 500;
			const string testName = "TestThread";
			var th = new SimpleThread(sleepTime, testName) { ThrowThreadAbortException = true };

			th.RunThread();
			Thread.Sleep(100);
			th.Terminate();
			TimeoutException exceptionThrown = null;
			try
			{
				th.Terminate();
				th.WaitTermination(100);
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

		[TestMethod]
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

			Assert.IsNull(th.LastError);

			Assert.IsTrue(th.IsInitialized);
		}

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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



		[TestMethod]
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