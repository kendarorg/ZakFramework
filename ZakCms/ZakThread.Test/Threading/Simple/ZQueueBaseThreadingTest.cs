using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZQueue.Threading;
using ZQueue.Threading.Enums;

namespace ZQueue.Test.Threading.Simple
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

			var st = new SimpleThread(sleepTime) {ThreadName = testName};
			var th = new StandardThread(st);

			Assert.AreEqual(expectedTestName, th.ThreadName);
			Assert.AreEqual(null,th.LastError);
			Assert.AreEqual(RunningStatus.None,th.Status);
		}

		[TestMethod]
		public void ItShouldBePossibleToStartAndStopAThread()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";
			var st = new SimpleThread(sleepTime) {ThreadName = testName};
			var th = new StandardThread(st);

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
		public void ItShouldBePossibleToStartAndAbortAThreadWithoutCleanUp()
		{
			const int sleepTime = 100;
			const string testName = "TestThread";

			var st = new SimpleThread(sleepTime) {ThreadName = testName};
			var th = new StandardThread( st);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Running, th.Status);

			th.Terminate(true);
			Thread.Sleep(100);
			
			Assert.AreEqual(RunningStatus.Aborted, th.Status);

			Assert.IsTrue(st.IsInitialized);
			Assert.IsFalse(st.IsCleanedUp);
		}

		[TestMethod]
		public void ItShouldBePossibleToStartAndStopAThreadDetectingItSHalting()
		{
			const int sleepTime = 500;
			const string testName = "TestThread";
			var st = new SimpleThread(sleepTime) {ThreadName = testName};
			var th = new StandardThread( st);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Running, th.Status);

			th.Terminate();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.Halting, th.Status);
			Thread.Sleep(1000);

			Assert.AreEqual(RunningStatus.Halted, th.Status);
			Assert.IsNull(th.LastError);

			Assert.IsTrue(st.IsInitialized);
			Assert.IsTrue(st.IsCleanedUp);
		}

		[TestMethod]
		public void ItShouldBePossibleToInterceptAnExceptionOnInitialization()
		{
			const int sleepTime = 10;
			const string testName = "TestThread";
			var st = new SimpleThread(sleepTime) {ThreadName = testName};
			var ex = new Exception("TEST");
			st.ThrowExceptionOnInitialization = ex;
			var th = new StandardThread( st);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.ExceptionThrown, th.Status);
			Exception expectedEx = th.LastError;
			Assert.IsNotNull(expectedEx);
			Assert.AreEqual(expectedEx.Message, st.ThrowExceptionOnInitialization.Message);

			Assert.IsFalse(st.IsInitialized);
			Assert.IsFalse(st.IsCleanedUp);
			Assert.IsTrue(st.IsExceptionHandled);
		}

		[TestMethod]
		public void ItShouldBePossibleToInterceptAnExceptionOnCyclicRunning()
		{
			const int sleepTime = 10;
			const string testName = "TestThread";
			var st = new SimpleThread(sleepTime) {ThreadName = testName};
			var ex = new Exception("TEST");
			st.ThrowExceptionOnCyclicExecution = ex;
			var th = new StandardThread( st);

			th.RunThread();
			Thread.Sleep(100);

			Assert.AreEqual(RunningStatus.ExceptionThrown, th.Status);
			Exception expectedEx = th.LastError;
			Assert.IsNotNull(expectedEx);
			Assert.AreEqual(expectedEx.Message, st.ThrowExceptionOnCyclicExecution.Message);

			Assert.IsTrue(st.IsInitialized);
			Assert.IsFalse(st.IsCleanedUp);
			Assert.IsTrue(st.IsExceptionHandled);
		}

		[TestMethod]
		public void ItShouldBePossibleToInterceptAnExceptionOnCleanUp()
		{
			const int sleepTime = 10;
			const string testName = "TestThread";
			var st = new SimpleThread(sleepTime) {ThreadName = testName};
			var ex = new Exception("TEST");
			st.ThrowExceptionOnCleanUp = ex;
			var th = new StandardThread( st);

			th.RunThread();
			Thread.Sleep(100);
			th.Terminate();
			Thread.Sleep(100);

		  Assert.AreEqual(RunningStatus.ExceptionThrown, th.Status);
			Exception expectedEx = th.LastError;
			Assert.IsNotNull(expectedEx);
			Assert.AreEqual(expectedEx.Message, st.ThrowExceptionOnCleanUp.Message);

			Assert.IsTrue(st.IsInitialized);
			Assert.IsFalse(st.IsCleanedUp);
			Assert.IsTrue(st.IsExceptionHandled);
		}

		[TestMethod]
		public void ItShouldBePossibleToRestartAThreadThatFailed()
		{
			const int sleepTime = 10;
			const string testName = "TestThread";
			const bool restartOnError = true;
			var st = new SimpleThread(sleepTime) {ThreadName = testName};
			var ex = new Exception("TEST");
			st.ThrowExceptionOnCyclicExecution = ex;
			st.RestartOnError = restartOnError;
			st.ResetExceptionAfterThrow = true;
			var th = new StandardThread( st);

			th.RunThread();
			Thread.Sleep(1000);
			th.Terminate();
			Thread.Sleep(1000);
			Assert.IsTrue(st.IsInitialized);
			Assert.IsTrue(st.IsInitialized);
			Assert.IsTrue(st.IsExceptionHandled);
			Assert.IsTrue(st.IsCleanedUp);

			Exception expectedEx = th.LastError;
			Assert.IsNull(expectedEx);
		}
	}
}
