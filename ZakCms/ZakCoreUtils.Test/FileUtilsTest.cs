using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZakCoreUtils.Test
{


	/// <summary>
	///This is a test class for FileUtilsTest and is intended
	///to contain all FileUtilsTest Unit Tests
	///</summary>
	[TestClass]
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

		#if NOPE
		/// <summary>
		///A test for InitializeRoot
		///</summary>
		[TestMethod]
		public void InitializeRootTest()
		{
			FileUtils.InitializeRoot();
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for FindFile
		///</summary>
		[TestMethod]
		public void FindFileTest()
		{
			string path = string.Empty; 
			string availableRoot = string.Empty; 
			string expected = string.Empty; 
			string actual;
			actual = FileUtils.FindFile(path, availableRoot);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for FileUtils Constructor
		///</summary>
		[TestMethod]
		public void FileUtilsConstructorTest()
		{
			FileUtils target = new FileUtils();
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for LoadAssembly
		///</summary>
		[TestMethod]
		public void LoadAssemblyTest()
		{
			string path = string.Empty; 
			string availableRoot = string.Empty; 
			Assembly expected = null; 
			Assembly actual;
			actual = FileUtils.LoadAssembly(path, availableRoot);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for LoadClassFromAssemblies
		///</summary>
		[TestMethod]
		public void LoadClassFromAssembliesTest()
		{
			string className = string.Empty; 
			Assembly asm = null; 
			Type expected = null; 
			Type actual;
			actual = FileUtils.LoadClassFromAssemblies(className, asm);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
#endif
	}
}
