using System;
using ZakCore.Utils.Commons;

namespace ZakCore.Utils.Logging
{
	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public class Logger:BaseLogger
	{
		public const string LOGGER_LEVEL = "LoggerLevel";

		public static ILogger LoggerObject { get; set; }

		/*public static LogLevels LoggingLevel
		{
			get
			{
				if (LoggerObject != null) return LoggerObject.LoggingLevel;
				return 0;
			}
			set { if (LoggerObject != null) LoggerObject.LoggingLevel = value; }
		}*/

		public override void Initialize(IIniFile iniFile, string section = null)
		{
			throw new NotImplementedException();
		}

		protected override void WriteStringToLog(string toWriteExpanded, string toWrite, LogLevels level)
		{
			throw new NotImplementedException();
		}
	}
}