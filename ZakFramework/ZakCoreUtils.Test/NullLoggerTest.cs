using System;
using System.Globalization;
using NUnit.Framework;
using ZakCore.Utils.Logging;

namespace ZakCoreUtils.Test
{
	[TestFixture]
	public class NullLoggerTest
	{
		[Test]
		public void NullLoggerTestConstructor()
		{
			var nullLogger = NullLogger.Create();
			nullLogger.Initialize(null);
			nullLogger.Debug("Message");
		}

		
		#region Log level tests

		[Test]
		public void TheLoggerShouldConsiderLoggerLevelDebug()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogDebug;
			logger.Debug("test");
			logger.Info("test");
			logger.Warn("test");
			logger.Error("test");
			logger.Fatal("test");
		}


		[Test]
		public void TheLoggerShouldConsiderLoggerLevelInfo()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogInfo;
			logger.Debug("test");
			logger.Info("test");
			logger.Warn("test");
			logger.Error("test");
			logger.Fatal("test");
		}

		[Test]
		public void TheLoggerShouldConsiderLoggerLevelWarn()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogWarn;
			logger.Debug("test");
			logger.Info("test");
			logger.Warn("test");
			logger.Error("test");
			logger.Fatal("test");
		}

		[Test]
		public void TheLoggerShouldConsiderLoggerLevelError()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogError;
			logger.Debug("test");
			logger.Info("test");
			logger.Warn("test");
			logger.Error("test");
			logger.Fatal("test");
		}

		[Test]
		public void TheLoggerShouldConsiderLoggerLevelFatal()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogFatal;
			logger.Debug("test");
			logger.Info("test");
			logger.Warn("test");
			logger.Error("test");
			logger.Fatal("test");
		}


		[Test]
		public void TheLoggerShouldConsiderLoggerLevelNone()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogNone;
			logger.Debug("test");
			logger.Info("test");
			logger.Warn("test");
			logger.Error("test");
			logger.Fatal("test");
		}

		#endregion


		[Test]
		public void TheLoggerShouldLogWithDebug()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogDebug;

			logger.Debug("test");
			logger.Debug("test", new Exception("testException"));
			logger.DebugFormat("{0}", "test");
			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.DebugFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.DebugFormat(culture, "{0}-{1}", "test", 0.2533);

			logger.LoggingLevel = LogLevels.LogInfo;

			logger.Debug("test");
			logger.Debug("test", new Exception("testException"));
			logger.DebugFormat("{0}", "test");
			culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.DebugFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.DebugFormat(culture, "{0}-{1}", "test", 0.2533);
		}


		[Test]
		public void TheLoggerShouldLogWithInfo()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogInfo;

			logger.Info("test");
			logger.Info("test", new Exception("testException"));
			logger.InfoFormat("{0}", "test");
			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.InfoFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.InfoFormat(culture, "{0}-{1}", "test", 0.2533);

			logger.LoggingLevel = LogLevels.LogWarn;

			logger.Info("test");
			logger.Info("test", new Exception("testException"));
			logger.InfoFormat("{0}", "test");
			culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.InfoFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.InfoFormat(culture, "{0}-{1}", "test", 0.2533);
		}


		[Test]
		public void TheLoggerShouldLogWithWarn()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogWarn;

			logger.Warn("test");
			logger.Warn("test", new Exception("testException"));
			logger.WarnFormat("{0}", "test");
			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.WarnFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.WarnFormat(culture, "{0}-{1}", "test", 0.2533);

			logger.LoggingLevel = LogLevels.LogError;

			logger.Warn("test");
			logger.Warn("test", new Exception("testException"));
			logger.WarnFormat("{0}", "test");
			culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.WarnFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.WarnFormat(culture, "{0}-{1}", "test", 0.2533);
		}


		[Test]
		public void TheLoggerShouldLogWithError()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogError;

			logger.Error("test");
			logger.Error("test", new Exception("testException"));
			logger.ErrorFormat("{0}", "test");
			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.ErrorFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.ErrorFormat(culture, "{0}-{1}", "test", 0.2533);

			logger.LoggingLevel = LogLevels.LogFatal;

			logger.Error("test");
			logger.Error("test", new Exception("testException"));
			logger.ErrorFormat("{0}", "test");
			culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.ErrorFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.ErrorFormat(culture, "{0}-{1}", "test", 0.2533);
		}

		[Test]
		public void TheLoggerShouldLogWithFatal()
		{
			var logger = NullLogger.Create();
			logger.LoggingLevel = LogLevels.LogFatal;

			logger.Fatal("test");
			logger.Fatal("test", new Exception("testException"));
			logger.FatalFormat("{0}", "test");
			var culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.FatalFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.FatalFormat(culture, "{0}-{1}", "test", 0.2533);

			logger.LoggingLevel = LogLevels.LogNone;

			logger.Fatal("test");
			logger.Fatal("test", new Exception("testException"));
			logger.FatalFormat("{0}", "test");
			culture = CultureInfo.CreateSpecificCulture("en-US");
			logger.FatalFormat(culture, "{0}-{1}", "test", 0.2533);
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			logger.FatalFormat(culture, "{0}-{1}", "test", 0.2533);
		}

		// ReSharper restore StringIndexOfIsCultureSpecific.1
	}
}