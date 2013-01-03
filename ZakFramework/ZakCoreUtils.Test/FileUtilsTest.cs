using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using ZakCore.Utils.Commons;
using ZakTestUtils;

namespace ZakCoreUtils.Test
{
	/// <summary>
	///This is a test class for FileUtilsTest and is intended
	///to contain all FileUtilsTest Unit Tests
	///</summary>
	[TestFixture]
	public class FileUtilsTest
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes

		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//

		#endregion

		[Test]
		public void InitializeFileUtilsWithCommandLinePArser()
		{
			CommandLineParser.SetEnv("ROOT", null);
			var root = TestFileUtils.GetSolutionRoot();
			var commandLineParser = new CommandLineParser(new[] {"-root", root},string.Empty);
			FileUtils.InitializeRoot(commandLineParser);
			Assert.AreEqual(root,FileUtils.BaseRoot);
		}

		[Test]
		public void InitializeFileUtilsWithEnvironmentVariables()
		{
			CommandLineParser.SetEnv("ROOT", null);
			var root = TestFileUtils.GetSolutionRoot();
			CommandLineParser.SetEnv("ROOT", root);
			FileUtils.InitializeRoot();
			Assert.AreEqual(root, FileUtils.BaseRoot);
		}

		[Test]
		public void InitializeFileUtilsAsTheExecutablePath()
		{
			CommandLineParser.SetEnv("ROOT", null);
			var root = Environment.CurrentDirectory;
			FileUtils.InitializeRoot();
			Assert.AreEqual(root, FileUtils.BaseRoot);
		}

		[Test]
		public void ItShouldBePossibleToCreateADirectoryRecursively()
		{
			CommandLineParser.SetEnv("ROOT", null);
			var root = Environment.CurrentDirectory;
			var createdPath = Path.Combine(root, "a", "b", "c");
			FileUtils.CreateFolderRecursive(createdPath);
			FileUtils.CreateFolderRecursive(createdPath);
			Assert.IsTrue(Directory.Exists(createdPath));
			TestFileUtils.RemoveDir(Path.Combine(root,"a"));
		}

		[Test]
		public void ItShouldBePossibleToLoadAnAssmbly()
		{
			var root = TestFileUtils.GetSolutionRoot();
			var dll = Directory.GetFiles(root, "*.dll",SearchOption.AllDirectories).First();
			var path = Path.GetDirectoryName(dll);
			dll = Path.GetFileName(dll);

			var ass = FileUtils.LoadAssembly("Test.dll");
			Assert.IsNull(ass);
			ass = FileUtils.LoadAssembly(dll, path);
			Assert.IsNotNull(ass);
		}

		[Test]
		[Ignore]
		public void ItShouldBePossibleToLoadAnAssmblyClassFromASpecifiedAssembly()
		{
			var root = TestFileUtils.GetSolutionRoot();
			var dll = Directory.GetFiles(root, "Crontab.exe",SearchOption.AllDirectories).First();
			var path = Path.GetDirectoryName(dll);
			dll = Path.GetFileName(dll);

			var ass =  FileUtils.LoadAssembly(dll, path);
			var cla = FileUtils.LoadClassFromAssemblies("_004_Crontab.ExitBehaviour", ass);
			Assert.IsNotNull(ass);
			Assert.IsNotNull(cla);
		}


		[Test]
		[Ignore]
		public void ItShouldNotBePossibleToLoadNonExistingClassFromASpecifiedAssembly()
		{
			var root = TestFileUtils.GetSolutionRoot();
			var dll = Directory.GetFiles(root, "Crontab.exe", SearchOption.AllDirectories).First();
			var path = Path.GetDirectoryName(dll);
			dll = Path.GetFileName(dll);

			var ass = FileUtils.LoadAssembly(dll, path);
			var cla = FileUtils.LoadClassFromAssemblies("_004_Crontab.NonExistingClass", ass);
			Assert.IsNotNull(ass);
			Assert.IsNull(cla);
		}

		[Test]
		public void ItShouldNotBePossibleToLoadNullAssembly()
		{
			var root = TestFileUtils.GetSolutionRoot();
			
			var ass = FileUtils.LoadAssembly(null, root);
			Assert.IsNull(ass);
		}


		[Test]
		public void ItShouldBePossibleToLoadAnAssmblyClassFromANotSpecifiedAssembly()
		{
			var cla = FileUtils.LoadClassFromAssemblies("ZakCore.Utils.Logging.NullLogger");
			Assert.IsNotNull(cla);
		}

		[Test]
		public void ItShouldBePossibleToFindAFileOnADir()
		{
			CommandLineParser.SetEnv("ROOT", null);
			var root = TestFileUtils.GetSolutionRoot();
			CommandLineParser.SetEnv("ROOT", root);
			FileUtils.InitializeRoot();
			var testRoot = Environment.CurrentDirectory;
			File.WriteAllText(Path.Combine(testRoot,"tester.ltx"),"empty");

			var foundedPath = FileUtils.FindFile("ZakFramework.sln");
			Assert.IsTrue(File.Exists(foundedPath));

			foundedPath = FileUtils.FindFile("tester.ltx");
			Assert.IsTrue(File.Exists(foundedPath));

			var newFoundedPath = FileUtils.FindFile(foundedPath);
			Assert.AreEqual(foundedPath, newFoundedPath);

			foundedPath = FileUtils.FindFile(string.Empty);
			Assert.AreEqual(string.Empty,foundedPath);

			var createdPath = Path.Combine(root, "d", "e", "e");
			
			FileUtils.CreateFolderRecursive(createdPath);
			File.WriteAllText(Path.Combine(createdPath,"tester1.ltx"),"aaaa");

			foundedPath = FileUtils.FindFile("tester1.ltx", createdPath);
			Assert.IsTrue(File.Exists(foundedPath));

			foundedPath = FileUtils.FindFile("taskmgr.exe", createdPath);
			Assert.IsTrue(File.Exists(foundedPath));

			var orginalEnvironment = Environment.CurrentDirectory;
			try
			{
				Environment.CurrentDirectory = createdPath;
				var secroot = Environment.CurrentDirectory;
				File.WriteAllText(Path.Combine(secroot, "tester2.ltx"), "aaaa");
				foundedPath = FileUtils.FindFile("tester2.ltx");
				Assert.IsTrue(File.Exists(foundedPath));
			}
			finally
			{
				Environment.CurrentDirectory = orginalEnvironment;
			}

			foundedPath = FileUtils.FindFile("doesNotExists");
			Assert.IsNull(foundedPath);
			TestFileUtils.RemoveDir(Path.Combine(root, "d"));
		}
	}
}