using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZakCore.Utils.Commons;

namespace ZakCoreUtils.Test
{
	/// <summary>
	///This is a test class for ConversionUtilsTest and is intended
	///to contain all ConversionUtilsTest Unit Tests
	///</summary>
	[TestClass]
	public class ConversionUtilsTest
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
		///A test for String2Bytes
		///</summary>
		[TestMethod]
		public void String2BytesTest()
		{
			const string val = "\\asdfl8-8-";
			Encoding enc = Encoding.ASCII;
			int outv;
			byte[] actual = ConversionUtils.String2Bytes(val, enc);
			string expected = ConversionUtils.Bytes2String(out outv, actual, 0, enc);
			Assert.AreEqual(expected, val);
		}

		private int GetOffset(int offset)
		{
			return offset;
		}

		/// <summary>
		///A test for Bytes2Guid
		///</summary>
		[TestMethod]
		public void Bytes2GuidTest()
		{
			Guid vg = Guid.NewGuid();
			byte[] val = ConversionUtils.Guid2Bytes(vg);
			const int offset = 0;
			Guid actual = ConversionUtils.Bytes2Guid(val, GetOffset(offset));
			Assert.AreEqual(vg, actual);
		}

		/// <summary>
		///A test for DateTime2UnixTime
		///</summary>
		[TestMethod]
		public void DateTime2UnixTimeTest()
		{
			var val = new DateTime(2000, 1, 1, 12, 30, 30);
			const int expected = 946729830;
			int actual = ConversionUtils.DateTime2UnixTime(val);
			Assert.AreEqual(expected, actual);
		}
	}
}