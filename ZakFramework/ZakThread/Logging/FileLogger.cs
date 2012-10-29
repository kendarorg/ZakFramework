using System;
using System.IO;
using System.Threading;
using ZakCore.Utils.Collections;
using ZakCore.Utils.Commons;
using ZakCore.Utils.Logging;
using ZakThread.Threading;

namespace ZakThread.Logging
{
	public class FileLogger : BaseThread, ILogger
	{
		public const string LOGGER_FILE = "LoggerFile";
		public const string LOGGER_LEVEL = "LoggerLevel";
		public string LoggingFile { get; set; }
		public ushort LoggingLevel { get; set; }
		private StreamWriter _logFile;
		private string _loggingFile;

		public FileLogger()
			: base(NullLogger.Create(), "FileLogger", true)
		{

		}

		public void Initialize(IIniFile iniFile, string section = null)
		{
			LoggingLevel = ushort.Parse(iniFile.GetValueString(LOGGER_LEVEL, section));
			LoggingFile = iniFile.GetValueString(LOGGER_FILE, section);
			_loggingFile = string.Format("{0}.{1:0000}{2:00}{3:00}.log", LoggingFile, _startTime.Year, _startTime.Month,
			                             _startTime.Day);
			_logFile = new StreamWriter(_loggingFile, true);
		}

		public override void RunThread()
		{
			
		}


		private readonly LockFreeQueue<LogEntity> _writeLog = new LockFreeQueue<LogEntity>();

		public void Log(LogEntity le)
		{
			_writeLog.Enqueue(le);
		}

		private DateTime _startTime = DateTime.UtcNow;

		public override void Dispose()
		{
			_logFile.Close();
			base.Dispose();
		}

		protected override bool RunSingleCycle()
		{
			if (_startTime.Day != DateTime.UtcNow.Day)
			{
				_logFile.Close();
				_startTime = DateTime.UtcNow;
				_loggingFile = string.Format("{0}.{1:0000}{2:00}{3:00}.log", LoggingFile, _startTime.Year, _startTime.Month,
				                             _startTime.Day);
				_logFile = new StreamWriter(_loggingFile, true);
			}
			foreach (var el in _writeLog.Dequeue())
			{
				_logFile.WriteLine(el.ToString());
			}
			_logFile.Flush();
			Thread.Sleep(10);
			return true;
		}
	}
}