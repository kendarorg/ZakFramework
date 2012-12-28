using System;
using ZakCore.Utils.Commons;

namespace _004_Crontab
{
	public class ExitBehaviour : ICommandLineParserExitBehaviour
	{
		public void HandleApplicationExit()
		{
			Console.ReadKey();
			Environment.Exit(0);
		}
	}
}
