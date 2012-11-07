using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZakCoreUtils.Test
{
	/// <summary>
	///This is a test class for IniFileTest and is intended
	///to contain all IniFileTest Unit Tests
	///</summary>
	[TestClass]
	public class IniFileTest
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
	///A test for IniFile Constructor
	///</summary>
		[TestMethod]
		public void IniFileConstructorTest()
		{
			string fileName = string.Empty; 
			string setupRoot = string.Empty; 
			bool replaceBuild = false; 
			IniFile target = new IniFile(fileName, setupRoot, replaceBuild);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for GetSections
		///</summary>
		[TestMethod]
		public void GetSectionsTest()
		{
			string fileName = string.Empty; 
			string setupRoot = string.Empty; 
			bool replaceBuild = false; 
			IniFile target = new IniFile(fileName, setupRoot, replaceBuild); 
			IEnumerable<string> expected = null; 
			IEnumerable<string> actual;
			actual = target.GetSections();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetValue
		///</summary>
		[TestMethod]
		public void GetValueTest()
		{
			string fileName = string.Empty; 
			string setupRoot = string.Empty; 
			bool replaceBuild = false; 
			IniFile target = new IniFile(fileName, setupRoot, replaceBuild); 
			string id = string.Empty; 
			string section = string.Empty; 
			string expected = string.Empty; 
			string actual;
			actual = target.GetValue(id, section);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetValues
		///</summary>
		[TestMethod]
		public void GetValuesTest()
		{
			string fileName = string.Empty; 
			string setupRoot = string.Empty; 
			bool replaceBuild = false; 
			IniFile target = new IniFile(fileName, setupRoot, replaceBuild); 
			string section = string.Empty; 
			Dictionary<string, string> expected = null; 
			Dictionary<string, string> actual;
			actual = target.GetValues(section);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Save
		///</summary>
		[TestMethod]
		public void SaveTest()
		{
			string fileName = string.Empty; 
			string setupRoot = string.Empty; 
			bool replaceBuild = false; 
			IniFile target = new IniFile(fileName, setupRoot, replaceBuild); 
			string fileName1 = string.Empty; 
			bool expected = false; 
			bool actual;
			actual = target.Save(fileName1);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for SetValue
		///</summary>
		[TestMethod]
		public void SetValueTest()
		{
			string fileName = string.Empty; 
			string setupRoot = string.Empty; 
			bool replaceBuild = false; 
			IniFile target = new IniFile(fileName, setupRoot, replaceBuild); 
			string key = string.Empty; 
			string value = string.Empty; 
			string section = string.Empty; 
			target.SetValue(key, value, section);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for SetValues
		///</summary>
		[TestMethod]
		public void SetValuesTest()
		{
			string fileName = string.Empty; 
			string setupRoot = string.Empty; 
			bool replaceBuild = false; 
			IniFile target = new IniFile(fileName, setupRoot, replaceBuild); 
			Dictionary<string, string> vals = null; 
			string section = string.Empty; 
			target.SetValues(vals, section);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for FileName
		///</summary>
		[TestMethod]
		public void FileNameTest()
		{
			string fileName = string.Empty; 
			string setupRoot = string.Empty; 
			bool replaceBuild = false; 
			IniFile target = new IniFile(fileName, setupRoot, replaceBuild); 
			string actual;
			actual = target.FileName;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
#endif
	}
}