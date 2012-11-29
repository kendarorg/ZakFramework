using ZakCore.Utils.Logging;
using log4net;

namespace ZakCore.Logger
{
	public class Log4NetLog : BaseLogger
	{
		private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		protected override void WriteStringToLog(string toWriteExpanded,string toWrite, LogLevels level)
		{
			switch (level)
			{
				case (LogLevels.LogFatal):
					_log.Fatal(toWrite);
					break;
				case (LogLevels.LogError):
					_log.Error(toWrite);
					break;
				case (LogLevels.LogWarn):
					_log.Warn(toWrite);
					break;
				case (LogLevels.LogInfo):
					_log.Info(toWrite);
					break;
				case (LogLevels.LogDebug):
					_log.Debug(toWrite);
					break;
			}
		}

		public override void Initialize(Utils.Commons.IIniFile iniFile, string section = null)
		{
			
		}
	}
}