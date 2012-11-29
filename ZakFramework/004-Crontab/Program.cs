using System;
using System.Collections.Generic;
using System.IO;
using ZakCore.Utils.Commons;

namespace _004_Crontab
{
	/// <summary>
	/// Crontab program
	/// 
	/// Format string with spaces between the entries
	///  *     *     *    *     *     *    * command to be executed
	///  -     -     -    -     -     -    -
	///  |     |     |    |     |     |    +----- year
	///  |     |     |    |     |     +----- day of week (0 - 6) (Sunday=0)
	///  |     |     |    |     +------- month (1 - 12)
	///  |     |     |    +--------- day of month (1 - 31)
	///  |     |     +----------- hour (0 - 23)
	///  |     +------------- min (0 - 59)
	///  +------------- sec (0 - 59)
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0 || (args.Length==1 && args[0]=="-h"))
			{
				Console.WriteLine("Usage:");
				Console.WriteLine("\tCrontab commandfile.ctb");
				Console.WriteLine("\tCrontab -h");
				Console.WriteLine("File format:");
				Console.WriteLine("* * * * * * *\t[Command]");
				Console.WriteLine("\tThe command is separated by the time with a TAB.");
				Console.WriteLine("\tThe parts of the time are separated between each other by single spaces.");
				Console.WriteLine("\tThe crontab format is considered with seconds included.");
				Console.WriteLine(
@"Crontab time specification:
 Format string with spaces between the entries
   *     *     *    *     *     *    * command to be executed
   -     -     -    -     -     -    -
   |     |     |    |     |     |    +----- year
   |     |     |    |     |     +----- day of week (0 - 6) (Sunday=0)
   |     |     |    |     +------- month (1 - 12)
   |     |     |    +--------- day of month (1 - 31)
   |     |     +----------- hour (0 - 23)
   |     +------------- min (0 - 59)
   +------------- sec (0 - 59)
");
				Environment.Exit(0);
			}
			if (!File.Exists(args[0]))
			{
				Console.WriteLine("File {0} not existing.",args[0]);
			}

			Console.WriteLine("Reading crontab config {0}.", args[0]);
			var readLines = File.ReadAllLines(args[0]);
			var corntabEntries = ParseCrontabEntries(readLines);
			var crontabThread = new CrontabThread(corntabEntries);
			crontabThread.RunThread();
			Console.WriteLine("Crontab started.");
			Console.WriteLine("Press a key to terminate.");
			Console.ReadKey();
			Console.WriteLine("Crontab terminating.");
			crontabThread.Terminate();
			try
			{
				crontabThread.WaitTermination(1000);
			}
			catch (TimeoutException)
			{
				Console.WriteLine("Unable to terminate crontab. Proceeding with abort.");
				crontabThread.Terminate(true);
			}

			crontabThread.Dispose();
		}

		private static List<CrontabTask> ParseCrontabEntries(string[] readLines)
		{
			var crontabTasks = new List<CrontabTask>();
			foreach (var line in readLines)
			{
				var trimmedLine = line.Trim();
				if (string.IsNullOrWhiteSpace(trimmedLine)) continue;
				if (trimmedLine.StartsWith("#")) continue;
				string[] command = trimmedLine.Split('\t');
				if (command.Length!=2) continue;

				crontabTasks.Add(new CrontabTask
					{
						CommandLine = command[1],
						CrontabEntry = new Crontab(command[0],true)
					});
			}
			return crontabTasks;
		}
	}
}
