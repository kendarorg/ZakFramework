using ZakCore.Utils.Commons;
using ZakCore.Utils.Logging;

namespace ZakCoreUtils.Test.Logger
{
	public class BaseLoggerMock:BaseLogger
	{
		public BaseLoggerMock()
		{
			Reset();
		}

		internal void Reset()
		{
			ToWriteExpanded = null;
			ToWrite = null;
			ToWriteLevel = null;
		}

		public override void Initialize(IIniFile iniFile, string section = null)
		{
			
		}

		internal string ToWriteExpanded { get; set; }
		internal string ToWrite { get; set; }
		internal LogLevels? ToWriteLevel { get; set; }

		protected override void WriteStringToLog(string toWriteExpanded, string toWrite, LogLevels level)
		{
			ToWriteExpanded = toWriteExpanded;
			ToWrite = toWrite;
			ToWriteLevel = level;
		}
	}
}
