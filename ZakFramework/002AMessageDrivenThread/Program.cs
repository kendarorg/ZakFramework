using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using ZakThread.Threading.Enums;

namespace _002AMessageDrivenThread
{
	class Program
	{
		static void Main(string[] args)
		{
			var stopwatch = new Stopwatch();
			var stopwatchExternal = new Stopwatch();
			var testThread = new TestMessageThread(stopwatch, "TestThread");
			//How many blocks of 5 ms should run (and how many messages)
			var runBlocks = 10;
			var sendedMessages = 0;

			testThread.RunThread();
			stopwatchExternal.Start();

			//The list of messages received
			List<TestMessage> messagesReceived = new List<TestMessage>();

			TestMessage msg = null;
			while (runBlocks > 0)
			{
				//Send a new message
				msg = new TestMessage(Guid.NewGuid(), runBlocks);
				//Directly to the thread
				testThread.SendMessageToThread(msg);
				sendedMessages++;
				//Peek the messages that the thread sent to us
				while ((msg = (TestMessage)testThread.PeekMessageFromThread()) != null)
				{
					messagesReceived.Add(msg);
				}
				Thread.Sleep(5);
				runBlocks--;
			}
			//Receive the resulting messages
			while ((msg = (TestMessage)testThread.PeekMessageFromThread()) != null)
			{
				messagesReceived.Add(msg);
			}
			//Terminate the thread gracefully
			testThread.Terminate();
			//Wait for 1000 ms that the thread terminate
			testThread.WaitTermination(1000);
			stopwatchExternal.Stop();
			Console.WriteLine(
				string.Format(
					"Completed in {0} ms, lifecycle was of {1} ms. Messages sent were {2} on {3} received",
					stopwatch.ElapsedMilliseconds,
					stopwatchExternal.ElapsedMilliseconds,
					sendedMessages,
					messagesReceived.Count));
			//Just to avoid the app closing...
			Console.ReadKey();
		}
	}
}
