using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZakCore.Utils.Logging;
using ZakThread.Threading;

namespace _004_Crontab
{
	class CrontabThread : BaseThread
	{
		private readonly List<CrontabTask> _crontabTasks;

		public CrontabThread(List<CrontabTask> crontabTasks) :
			base(NullLogger.Create(), "Crontab", true)
		{
			_crontabTasks = crontabTasks;
		}

		protected override bool RunSingleCycle()
		{
			for (int index = 0; index < _crontabTasks.Count; index++)
			{
				var item = _crontabTasks[index];
				var next = item.CrontabEntry.Next();
				var delta = next - DateTime.Now;
				if (delta.TotalSeconds < 1)
				{
					Task.Factory.StartNew(() => RunCommand(item));
				}
			}
			Thread.Sleep(1000);
			return true;
		}

		private void RunCommand(CrontabTask item)
		{
			try
			{
				Console.WriteLine("Executing command {0} at {1}",item.CommandLine,DateTime.Now);

				// create the ProcessStartInfo using "cmd" as the program to be run,
				// and "/c " as the parameters.
				// Incidentally, /c tells cmd that we want it to execute the command that follows,
				// and then exit.
				var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + item.CommandLine)
					{

						// The following commands are needed to redirect the standard output.
						// This means that it will be redirected to the Process.StandardOutput StreamReader.
						RedirectStandardOutput = true,
						UseShellExecute = false,
						// Do not create the black window.
						CreateNoWindow = true
					};

				// Now we create a process, assign its ProcessStartInfo and start it
				var proc = new System.Diagnostics.Process
					{
						StartInfo = procStartInfo
					};

				proc.Start();

				// Get the output into a string
				string result = proc.StandardOutput.ReadToEnd();
				// Display the command output.
				Console.WriteLine(result);
			}
			catch (Exception objException)
			{
				Console.WriteLine(objException);
			}
		}
	}
}
