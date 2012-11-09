using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZakCore.Utils.Logging;
using ZakThread.Logging;

namespace ZakCoreUtils.Test
{
	[TestClass]
	public class NullLoggerTest
	{
		[TestMethod]
		public void NullLoggerTestConstructor()
		{
			var nullLogger = NullLogger.Create();
			nullLogger.Initialize(null);
			nullLogger.Log(new LogEntity());
		}
	}
}
