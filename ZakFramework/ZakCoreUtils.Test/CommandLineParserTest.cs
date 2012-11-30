using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZakCore.Utils.Commons;

namespace ZakCoreUtils.Test
{
	[TestClass]
	public class CommandLineParserTest
	{
		/// <summary>
		/// Mock class just to show that the call had been done
		/// </summary>
		public class ExitBehaviour : ICommandLineParserExitBehaviour
		{
			public int ShowHelpCalled = 0;
			
			public void HandleApplicationExit()
			{
				ShowHelpCalled++;
			}
		}

		[TestMethod]
		public void ItShouldBePossibleToCreateACommandLneParser()
		{
			const string helpString = "help";
			var args = new [] {"-test","-parameter","parameterValue"};
			var commandLineParser = new CommandLineParser(args, helpString);
			var resultValue = commandLineParser["test"];
			Assert.IsTrue(commandLineParser.IsSet("test"));
			Assert.IsTrue(resultValue.Length==0);
			resultValue = commandLineParser["notPresent"];
			Assert.IsNull(resultValue);
			Assert.IsFalse(commandLineParser.IsSet("notPresent"));
			resultValue = commandLineParser["parameter"];
			Assert.IsTrue(commandLineParser.IsSet("parameter"));
			Assert.AreEqual("parameterValue",resultValue);

			commandLineParser["parameter"] = "newValue";
			Assert.IsTrue(commandLineParser.IsSet("parameter"));
			resultValue = commandLineParser["parameter"];
			Assert.AreEqual("newValue", resultValue);
		}

		[TestMethod]
		public void ItShouldBePossibleToCreateACommandLneParserAndShowHelp()
		{
			const string helpString = "help";
			var args = new[] { "-test","-help" };
			var eb = new ExitBehaviour();
			var commandLineParser = new CommandLineParser(args, helpString,eb);
			Assert.AreEqual(helpString,commandLineParser.Help);
			Assert.IsTrue(commandLineParser.IsSet("help"));
			Assert.AreEqual(1,eb.ShowHelpCalled);
		}

		[TestMethod]
		public void ItShouldBePossibleToCheckForValuesPresence()
		{
			const string helpString = "help";
			var args = new[] { "-test", "-gino","-pino","pinoValue" };
			var eb = new ExitBehaviour();
			var commandLineParser = new CommandLineParser(args, helpString, eb);

			Assert.IsTrue(commandLineParser.Has(new []{"test","gino"}));
			Assert.IsFalse(commandLineParser.Has(new[] { "test", "fake" }));
			Assert.IsFalse(commandLineParser.Has(new[] { "fluke", "fake" }));

			Assert.IsTrue(commandLineParser.HasAllOrNone(new[] { "test", "gino" }));
			Assert.IsFalse(commandLineParser.HasAllOrNone(new[] { "test", "fake" }));
			Assert.IsTrue(commandLineParser.HasAllOrNone(new[] { "fluke", "fake" }));

			Assert.IsFalse(commandLineParser.HasOneAndOnlyOne(new[] { "test", "gino" }));
			Assert.IsTrue(commandLineParser.HasOneAndOnlyOne(new[] { "test", "fake" }));
			Assert.IsFalse(commandLineParser.HasOneAndOnlyOne(new[] { "fluke", "fake" }));

			Assert.AreEqual(0, eb.ShowHelpCalled);
		}


		[TestMethod]
		public void ItShouldBePossibleToGetEnvironmentVariables()
		{
			var args = new[] { "-test"};
			var commandLineParser = new CommandLineParser(args, "help");
			var temp = CommandLineParser.GetEnv("TEMP");
			commandLineParser["TEMP"] = temp;
			Assert.IsTrue(Directory.Exists(temp));
			var os = CommandLineParser.GetEnv("OS");
			commandLineParser["os"] = os;

			var notExistingVariable = CommandLineParser.GetEnv("thisDoesNotExists"+Guid.NewGuid().ToString());
			Assert.IsNull(notExistingVariable);

			Assert.IsTrue(commandLineParser.IsSet("os"));
			Assert.IsTrue(commandLineParser.IsSet("oS"));
			Assert.AreEqual(commandLineParser["os"], commandLineParser["oS"]);

			Assert.IsTrue(commandLineParser.IsSet("Temp"));
			Assert.IsTrue(commandLineParser.IsSet("temP"));
			Assert.AreEqual(commandLineParser["TEMP"], commandLineParser["temp"]);

			Assert.IsFalse(string.IsNullOrWhiteSpace(os));
		}
	}
}
