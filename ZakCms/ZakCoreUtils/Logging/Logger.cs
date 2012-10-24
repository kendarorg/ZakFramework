using System;

namespace ZakCore.Utils.Logging
{
	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public class Logger
	{
		public const string LOGGER_LEVEL = "LoggerLevel";

		public static ILogger LoggerObject { get; set; }


		public static ushort LoggingLevel
		{
			get
			{
				if (LoggerObject != null) return LoggerObject.LoggingLevel;
				return 0;
			}
			set { if (LoggerObject != null) LoggerObject.LoggingLevel = value; }
		}

		public static void Log(string callerId, Exception ex)
		{
#if !DEBUG
			if (LoggingLevel <= 2)
			{
				Log(LoggingLevel, callerId, "Exception {0}", ex.Message);
			}
			else
#endif
			{
				Log(5, callerId, "Exception {0}.\nStackTrace:{1}", ex.Message, ex.ToString());
			}
		}

		public static void Log(ushort level, string callerId, string formatParameter, params string[] parameters)
		{
			if (LoggerObject != null)
			{
				if (level > LoggingLevel) return;
				var le = new LogEntity
					{
						Level = level,
						CallerId = callerId,
						FormatParameter = formatParameter,
						Parameters = parameters,
						Timestamp = DateTime.UtcNow
					};
				LoggerObject.Log(le);
			}
		}
	}
}