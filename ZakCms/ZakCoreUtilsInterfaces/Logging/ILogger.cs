using ZakCore.Utils.Commons;

namespace ZakCore.Utils.Logging
{
	public interface ILogger
	{
		ushort LoggingLevel { get; set; }
		void Log(LogEntity le);
		void Initialize(IIniFile iniFile, string section = null);
	}
}