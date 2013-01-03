using NUnit.Framework;
using ZakCore.Utils.Commons;

namespace ZakCoreUtils.Test
{
	/// <summary>
	///This is a test class for BitUtilsTest and is intended
	///to contain all BitUtilsTest Unit Tests
	///</summary>
	[TestFixture]
	public class BitUtilsTest
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

		/// <summary>
		///A test for Set
		///</summary>
		[Test]
		public void SetTest()
		{
			int value = 1;
			const int valueExpected = 3;
			const int bitId = 1;
			BitUtils.Set(ref value, bitId);
			Assert.AreEqual(valueExpected, value);
		}

		/// <summary>
		///A test for IsSet
		///</summary>
		[Test]
		public void IsSetTest()
		{
			const int value = 2;
			const int bitId = 1;
			const bool expected = true;
			bool actual = BitUtils.IsSet(value, bitId);
			Assert.AreEqual(expected, actual);
		}
	}
}