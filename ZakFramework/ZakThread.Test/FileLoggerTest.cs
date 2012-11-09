using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZakCore.Utils.Commons;
using ZakCore.Utils.Logging;
using ZakTestUtils;
using ZakThread.Logging;

namespace ZakThread.Test
{
	[TestClass]
	public class FileLoggerTest
	{
		private class TestLogger:FileLogger
		{
			public TestLogger(DateTime startTime)
			{
				_startTime = startTime;
			}
		}

		[TestMethod]
		public void ItShouldBePossibleToLogThrougTheFileLogger()
		{
			var loggerFileSource = TestFileUtils.CreateRootDir("Logger");
			var iniFile = new IniFile(null);
			var logFilePath = Path.Combine(loggerFileSource, "test");

			var startime = DateTime.Now;
			var logFilePathReal  =Path.Combine(loggerFileSource, string.Format("{0}.{1:0000}{2:00}{3:00}.log", "test", startime.Year, startime.Month,startime.Day));
			
			iniFile.SetValue("LoggerFile",logFilePath);
			iniFile.SetValue("LoggerLevel", "1");
			var fileLogger = new TestLogger(DateTime.Now);
			fileLogger.Initialize(iniFile,"root");
			fileLogger.RunThread();
			Thread.Sleep(500);
			fileLogger.Log(new LogEntity());
			Thread.Sleep(1000);
			Assert.IsTrue(File.Exists(logFilePathReal));
			fileLogger.Terminate(true);
			Thread.Sleep(1000);
			fileLogger.Dispose();
			TestFileUtils.RemoveDir(loggerFileSource);
		}

		[TestMethod]
		public void ItShouldBePossibleToCreateANewLogFileUponDateChange()
		{
			var loggerFileSource = TestFileUtils.CreateRootDir("Logger");
			var iniFile = new IniFile(null);
			var logFilePath = Path.Combine(loggerFileSource, "test");

			var startime = DateTime.Now;
			var logFilePathReal = Path.Combine(loggerFileSource, string.Format("{0}.{1:0000}{2:00}{3:00}.log", "test", startime.Year, startime.Month, startime.Day));
			var logFilePathRealYesterday = Path.Combine(loggerFileSource, string.Format("{0}.{1:0000}{2:00}{3:00}.log", "test", startime.Year, startime.Month, startime.Day-1));

			iniFile.SetValue("LoggerFile", logFilePath);
			iniFile.SetValue("LoggerLevel", "1");
			var fileLogger = new TestLogger(DateTime.Now-TimeSpan.FromDays(1));
			fileLogger.Initialize(iniFile, "root");
			fileLogger.RunThread();
			Thread.Sleep(500);
			fileLogger.Log(new LogEntity
				{
					CallerId = "1",
					FormatParameter = "{0}",
					Level= 0x01,
					Parameters =new string[] { "test"},
					Timestamp = DateTime.Now
				});
			Thread.Sleep(1000);
			Assert.IsTrue(File.Exists(logFilePathReal));
			Assert.IsTrue(File.Exists(logFilePathRealYesterday));
			fileLogger.Terminate(true);
			Thread.Sleep(1000);
			fileLogger.Dispose();
			TestFileUtils.RemoveDir(loggerFileSource);
		}
	}
}
