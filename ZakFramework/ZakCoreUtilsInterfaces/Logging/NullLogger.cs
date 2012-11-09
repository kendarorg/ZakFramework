using ZakCore.Utils.Commons;
using ZakCore.Utils.Logging;

namespace ZakThread.Logging
{
	public class NullLogger : ILogger
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

		public void Log(LogEntity le)
		{
		}

		public ushort LoggingLevel { get; set; }
	}
}