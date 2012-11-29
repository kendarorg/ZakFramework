using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZakCore.Utils.Logging;
using ZakCoreUtils.Test.Logger;

namespace ZakCoreUtils.Test
{
	// ReSharper disable StringIndexOfIsCultureSpecific.1
	[TestClass]
	public class BaseLoggerTest
	{
		private const int START_OF_DATA_INDEX = 20;

		#region Log level tests
		[TestMethod]
		public void TheLoggerShouldConsiderLoggerLevelDebug()
		{
			var logger = new BaseLoggerMock {LoggingLevel = LogLevels.LogDebug};
			logger.Debug("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(DEBUG) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogDebug, logger.ToWriteLevel);

			logger.Info("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(INFO) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogInfo, logger.ToWriteLevel);

			logger.Warn("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(WARN) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogWarn, logger.ToWriteLevel);

			logger.Error("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(ERROR) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogError, logger.ToWriteLevel);

			logger.Fatal("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(FATAL) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogFatal, logger.ToWriteLevel);
		}


		[TestMethod]
		public void TheLoggerShouldConsiderLoggerLevelInfo()
		{
			var logger = new BaseLoggerMock { LoggingLevel = LogLevels.LogInfo };
			logger.Debug("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Info("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(INFO) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogInfo, logger.ToWriteLevel);

			logger.Warn("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(WARN) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogWarn, logger.ToWriteLevel);

			logger.Error("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(ERROR) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogError, logger.ToWriteLevel);

			logger.Fatal("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(FATAL) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogFatal, logger.ToWriteLevel);
		}

		[TestMethod]
		public void TheLoggerShouldConsiderLoggerLevelWarn()
		{
			var logger = new BaseLoggerMock { LoggingLevel = LogLevels.LogWarn };
			logger.Debug("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Info("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Warn("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(WARN) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogWarn, logger.ToWriteLevel);

			logger.Error("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(ERROR) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogError, logger.ToWriteLevel);

			logger.Fatal("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(FATAL) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogFatal, logger.ToWriteLevel);
		}

		[TestMethod]
		public void TheLoggerShouldConsiderLoggerLevelError()
		{
			var logger = new BaseLoggerMock { LoggingLevel = LogLevels.LogError };
			logger.Debug("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Info("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Warn("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Error("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(ERROR) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogError, logger.ToWriteLevel);

			logger.Fatal("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(FATAL) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogFatal, logger.ToWriteLevel);
		}

		[TestMethod]
		public void TheLoggerShouldConsiderLoggerLevelFatal()
		{
			var logger = new BaseLoggerMock { LoggingLevel = LogLevels.LogFatal };
			logger.Debug("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Info("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Warn("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Error("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Fatal("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(FATAL) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogFatal, logger.ToWriteLevel);
		}


		[TestMethod]
		public void TheLoggerShouldConsiderLoggerLevelNone()
		{
			var logger = new BaseLoggerMock { LoggingLevel = LogLevels.LogNone };
			logger.Debug("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Info("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Warn("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Error("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);

			logger.Fatal("test");
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);
		}
		#endregion


		[TestMethod]
		public void TheLoggerShouldLogWithDebug()
		{
			var logger = new BaseLoggerMock {LoggingLevel = LogLevels.LogDebug};

			logger.Debug("test");
			Assert.AreEqual("test",logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(DEBUG) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogDebug, logger.ToWriteLevel);

			logger.Debug("test", new Exception("testException"));
			Assert.AreEqual("test\nSystem.Exception: testException", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(DEBUG) test\nSystem.Exception: testException") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogDebug, logger.ToWriteLevel);

			logger.DebugFormat("{0}", "test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(DEBUG) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogDebug, logger.ToWriteLevel);

			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.DebugFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.AreEqual("test-0.2533", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(DEBUG) test-0.2533") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogDebug, logger.ToWriteLevel);

			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.DebugFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.AreEqual("test-0,2533", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(DEBUG) test-0,2533") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogDebug, logger.ToWriteLevel);



			logger.LoggingLevel = LogLevels.LogInfo;
			logger.Reset();

			logger.Debug("test");
			logger.Debug("test", new Exception("testException"));
			logger.DebugFormat("{0}", "test");
			culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.DebugFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.DebugFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);
		}


		[TestMethod]
		public void TheLoggerShouldLogWithInfo()
		{
			var logger = new BaseLoggerMock { LoggingLevel = LogLevels.LogInfo };

			logger.Info("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(INFO) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogInfo, logger.ToWriteLevel);

			logger.Info("test", new Exception("testException"));
			Assert.AreEqual("test\nSystem.Exception: testException", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(INFO) test\nSystem.Exception: testException") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogInfo, logger.ToWriteLevel);

			logger.InfoFormat("{0}", "test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(INFO) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogInfo, logger.ToWriteLevel);

			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.InfoFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.AreEqual("test-0.2533", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(INFO) test-0.2533") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogInfo, logger.ToWriteLevel);

			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.InfoFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.AreEqual("test-0,2533", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(INFO) test-0,2533") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogInfo, logger.ToWriteLevel);

			logger.LoggingLevel = LogLevels.LogWarn;
			logger.Reset();

			logger.Info("test");
			logger.Info("test", new Exception("testException"));
			logger.InfoFormat("{0}", "test");
			culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.InfoFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.InfoFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);
		}


		[TestMethod]
		public void TheLoggerShouldLogWithWarn()
		{
			var logger = new BaseLoggerMock { LoggingLevel = LogLevels.LogWarn };

			logger.Warn("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(WARN) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogWarn, logger.ToWriteLevel);

			logger.Warn("test", new Exception("testException"));
			Assert.AreEqual("test\nSystem.Exception: testException", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(WARN) test\nSystem.Exception: testException") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogWarn, logger.ToWriteLevel);

			logger.WarnFormat("{0}", "test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(WARN) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogWarn, logger.ToWriteLevel);

			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.WarnFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.AreEqual("test-0.2533", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(WARN) test-0.2533") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogWarn, logger.ToWriteLevel);

			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.WarnFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.AreEqual("test-0,2533", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(WARN) test-0,2533") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogWarn, logger.ToWriteLevel);

			logger.LoggingLevel = LogLevels.LogError;
			logger.Reset();

			logger.Warn("test");
			logger.Warn("test", new Exception("testException"));
			logger.WarnFormat("{0}", "test");
			culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.WarnFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.WarnFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);
		}


		[TestMethod]
		public void TheLoggerShouldLogWithError()
		{
			var logger = new BaseLoggerMock { LoggingLevel = LogLevels.LogError };

			logger.Error("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(ERROR) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogError, logger.ToWriteLevel);

			logger.Error("test", new Exception("testException"));
			Assert.AreEqual("test\nSystem.Exception: testException", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(ERROR) test\nSystem.Exception: testException") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogError, logger.ToWriteLevel);

			logger.ErrorFormat("{0}", "test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(ERROR) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogError, logger.ToWriteLevel);

			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.ErrorFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.AreEqual("test-0.2533", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(ERROR) test-0.2533") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogError, logger.ToWriteLevel);

			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.ErrorFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.AreEqual("test-0,2533", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(ERROR) test-0,2533") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogError, logger.ToWriteLevel);

			logger.LoggingLevel = LogLevels.LogFatal;
			logger.Reset();

			logger.Error("test");
			logger.Error("test", new Exception("testException"));
			logger.ErrorFormat("{0}", "test");
			culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.ErrorFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.ErrorFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
			Assert.IsNull(logger.ToWriteLevel);
		}

		[TestMethod]
		public void TheLoggerShouldLogWithFatal()
		{
			var logger = new BaseLoggerMock { LoggingLevel = LogLevels.LogFatal };

			logger.Fatal("test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(FATAL) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogFatal, logger.ToWriteLevel);

			logger.Fatal("test", new Exception("testException"));
			Assert.AreEqual("test\nSystem.Exception: testException", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(FATAL) test\nSystem.Exception: testException") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogFatal, logger.ToWriteLevel);

			logger.FatalFormat("{0}", "test");
			Assert.AreEqual("test", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(FATAL) test") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogFatal, logger.ToWriteLevel);

			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.FatalFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.AreEqual("test-0.2533", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(FATAL) test-0.2533") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogFatal, logger.ToWriteLevel);

			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.FatalFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.AreEqual("test-0,2533", logger.ToWrite);
			Assert.IsTrue(logger.ToWriteExpanded.IndexOf("(FATAL) test-0,2533") == START_OF_DATA_INDEX, logger.ToWriteExpanded);
			Assert.AreEqual(LogLevels.LogFatal, logger.ToWriteLevel);

			logger.LoggingLevel = LogLevels.LogNone;
			logger.Reset();

			logger.Fatal("test");
			logger.Fatal("test", new Exception("testException"));
			logger.FatalFormat("{0}", "test");
			culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.FatalFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.FatalFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
		}


		[TestMethod]
		public void TheLoggerShouldNotLogWithNone()
		{
			var logger = new BaseLoggerMock { LoggingLevel = LogLevels.LogNone };

			logger.Fatal("test");
			logger.Fatal("test", new Exception("testException"));
			logger.FatalFormat("{0}", "test");
			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.FatalFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.FatalFormat(culture, "{0}-{1}", "test", 0.2533);
			Assert.IsNull(logger.ToWrite);
			Assert.IsNull(logger.ToWriteExpanded);
		}
	}
	// ReSharper restore StringIndexOfIsCultureSpecific.1
}
