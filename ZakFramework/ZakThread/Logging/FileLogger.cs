using System;
using System.IO;
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
		public LogLevels LoggingLevel { get; set; }
		private StreamWriter _logFile;
		private string _loggingFile;

		internal DateTime _startTime;

		public FileLogger()
			: base(NullLogger.Create(), "FileLogger", true)
		{
			_startTime = DateTime.UtcNow;
		}

		public void Initialize(IIniFile iniFile, string section = null)
		{
			LoggingLevel = (LogLevels) ushort.Parse(iniFile.GetValueString(LOGGER_LEVEL, section));
			LoggingFile = iniFile.GetValueString(LOGGER_FILE, section);
			_loggingFile = string.Format("{0}.{1:0000}{2:00}{3:00}.log", LoggingFile, _startTime.Year, _startTime.Month,
			                             _startTime.Day);
			_logFile = new StreamWriter(_loggingFile, true);
		}

		private readonly LockFreeQueue<string> _writeLog = new LockFreeQueue<string>();

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
				_logFile.WriteLine(el);
			}
			_logFile.Flush();
			return true;
		}

// ReSharper disable UnusedParameter.Local
		private void WriteStringToLog(string toWriteExpanded, string toWrite, LogLevels level)
// ReSharper restore UnusedParameter.Local
		{
			_writeLog.Enqueue(toWriteExpanded);
		}

		protected void WriteStringToLogInternal(string toWrite, LogLevels level)
		{
			var toWriteExpanded = LoggerFormatter.Format(DateTime.Now, level, toWrite);
			WriteStringToLog(toWriteExpanded, toWrite, level);
		}

		public void Debug(object message)
		{
			if (!IsDebugEnabled) return;
			WriteStringToLogInternal(message.ToString(), LogLevels.LogDebug);
		}

		public void Debug(object message, Exception exception)
		{
			if (!IsDebugEnabled) return;
			DebugFormat("{0}\n{1}", message, exception);
		}

		public void DebugFormat(string format, params object[] args)
		{
			if (!IsDebugEnabled) return;
			WriteStringToLogInternal(string.Format(format, args), LogLevels.LogDebug);
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!IsDebugEnabled) return;
			WriteStringToLogInternal(string.Format(provider, format, args), LogLevels.LogDebug);
		}

		public void Info(object message)
		{
			if (!IsInfoEnabled) return;
			WriteStringToLogInternal(message.ToString(), LogLevels.LogInfo);
		}

		public void Info(object message, Exception exception)
		{
			if (!IsInfoEnabled) return;
			InfoFormat("{0}\n{1}", message, exception);
		}

		public void InfoFormat(string format, params object[] args)
		{
			if (!IsInfoEnabled) return;
			WriteStringToLogInternal(string.Format(format, args), LogLevels.LogInfo);
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!IsInfoEnabled) return;
			WriteStringToLogInternal(string.Format(provider, format, args), LogLevels.LogInfo);
		}

		public void Warn(object message)
		{
			if (!IsWarnEnabled) return;
			WriteStringToLogInternal(message.ToString(), LogLevels.LogWarn);
		}

		public void Warn(object message, Exception exception)
		{
			if (!IsWarnEnabled) return;
			WarnFormat("{0}\n{1}", message, exception);
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (!IsWarnEnabled) return;
			WriteStringToLogInternal(string.Format(format, args), LogLevels.LogWarn);
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!IsWarnEnabled) return;
			WriteStringToLogInternal(string.Format(provider, format, args), LogLevels.LogWarn);
		}

		public void Error(object message)
		{
			if (!IsErrorEnabled) return;
			WriteStringToLogInternal(message.ToString(), LogLevels.LogError);
		}

		public void Error(object message, Exception exception)
		{
			if (!IsErrorEnabled) return;
			ErrorFormat("{0}\n{1}", message, exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			if (!IsErrorEnabled) return;
			WriteStringToLogInternal(string.Format(format, args), LogLevels.LogError);
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!IsErrorEnabled) return;
			WriteStringToLogInternal(string.Format(provider, format, args), LogLevels.LogError);
		}

		public void Fatal(object message)
		{
			if (!IsFatalEnabled) return;
			WriteStringToLogInternal(message.ToString(), LogLevels.LogFatal);
		}

		public void Fatal(object message, Exception exception)
		{
			if (!IsFatalEnabled) return;
			FatalFormat("{0}\n{1}", message, exception);
		}

		public void FatalFormat(string format, params object[] args)
		{
			if (!IsFatalEnabled) return;
			WriteStringToLogInternal(string.Format(format, args), LogLevels.LogFatal);
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (!IsFatalEnabled) return;
			WriteStringToLogInternal(string.Format(provider, format, args), LogLevels.LogFatal);
		}

		public bool IsDebugEnabled
		{
			get { return (int)LoggingLevel <= (int)LogLevels.LogDebug; }
		}

		public bool IsInfoEnabled
		{
			get { return (int)LoggingLevel <= (int)LogLevels.LogInfo; }
		}

		public bool IsWarnEnabled
		{
			get { return (int)LoggingLevel <= (int)LogLevels.LogWarn; }
		}

		public bool IsErrorEnabled
		{
			get { return (int)LoggingLevel <= (int)LogLevels.LogError; }
		}

		public bool IsFatalEnabled
		{
			get { return (int)LoggingLevel <= (int)LogLevels.LogFatal; }
		}
	}
}