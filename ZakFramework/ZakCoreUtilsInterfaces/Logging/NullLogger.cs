using System;
using ZakCore.Utils.Commons;

namespace ZakCore.Utils.Logging
{
	public class NullLogger:ILogger
	{
		private static readonly NullLogger _nullLogger = new NullLogger();

		private NullLogger()
		{
		}

		public static ILogger Create()
		{
			return _nullLogger;
		}

		public void Initialize(IIniFile iniFile, string section = null)
		{
			LoggingLevel = 0;
		}

		protected void WriteStringToLogInternal(string toWrite, LogLevels level)
		{
			var toWriteExpanded = LoggerFormatter.Format(DateTime.Now,level,toWrite);
			WriteStringToLog(toWriteExpanded, toWrite, level);
		}

		public LogLevels LoggingLevel { get; set; }

		protected void WriteStringToLog(string toWriteExpanded,string toWrite, LogLevels level)
		{
			if (level == LogLevels.LogDebug)
			{
				System.Diagnostics.Debug.WriteLine(toWriteExpanded);
			}
			else
			{
				System.Diagnostics.Trace.WriteLine(toWriteExpanded);
			}
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