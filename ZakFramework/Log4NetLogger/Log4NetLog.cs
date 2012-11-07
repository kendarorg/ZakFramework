using ZakCore.Utils.Commons;
using ZakCore.Utils.Logging;
using log4net;

namespace ZakCore.Logger
{
	public class Log4NetLog : ILogger
	{
		private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public ushort LoggingLevel { get; set; }

		public void Log(LogEntity le)
		{
			switch (le.Level)
			{
				case (0):
					_log.Fatal(le.ToString());
					break;
				case (1):
					_log.Error(le.ToString());
					break;
				case (2):
					_log.Warn(le.ToString());
					break;
				case (3):
					_log.Info(le.ToString());
					break;
				default:
					//case (4):
					_log.Debug(le.ToString());
					break;
			}
		}

		public void Initialize(IIniFile iniFile, string section = null)
		{
		}
	}
}