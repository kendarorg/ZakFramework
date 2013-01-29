using System;
using System.Text;
using NUnit.Framework;
using ZakCore.Utils.Commons;
using System.Collections.Generic;

namespace ZakCoreUtils.Test
{
	/// <summary>
	///This is a test class for ConversionUtilsTest and is intended
	///to contain all ConversionUtilsTest Unit Tests
	///</summary>
	[TestFixture]
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

		[Test]
		public void Date2BytesTestWithStandardEncoding()
		{
			var val = DateTime.Now;
			byte[] actual = ConversionUtils.Date2Bytes(val);
			var expected = ConversionUtils.Bytes2Date(actual);
			Assert.AreEqual(expected, val);
		}

		[Test]
		public void String2BytesTest()
		{
			const string val = "\\asdfl8-8-";
			Encoding enc = Encoding.ASCII;
			int outv;
			byte[] actual = ConversionUtils.String2Bytes(val, enc);
			string expected = ConversionUtils.Bytes2String(out outv, actual, 0, enc);
			Assert.AreEqual(expected, val);
		}

		[Test]
		public void String2BytesTestWithNullEntryValue()
		{
			string val = string.Empty;
			Encoding enc = Encoding.ASCII;
			int outv;
			byte[] actual = ConversionUtils.String2Bytes(val, enc);
			string expected = ConversionUtils.Bytes2String(out outv, actual, 0, enc);
			Assert.AreEqual(expected, val);
		}

		[Test]
		public void String2BytesTestWithStandardEncoding()
		{
			const string val = "\\asdfl8-8-";
			int outv;
			byte[] actual = ConversionUtils.String2Bytes(val);
			string expected = ConversionUtils.Bytes2String(out outv, actual);
			Assert.AreEqual(expected, val);
		}

		private int GetOffset(int offset)
		{
			return offset;
		}

		/// <summary>
		///A test for Bytes2Guid
		///</summary>
		[Test]
		public void Bytes2GuidTest()
		{
			Guid vg = Guid.NewGuid();
			byte[] val = ConversionUtils.Guid2Bytes(vg);
			const int offset = 0;
			Guid actual = ConversionUtils.Bytes2Guid(val, GetOffset(offset));
			Assert.AreEqual(vg, actual);
		}

		[Test]
		public void Bytes2GuidTestWithAnotherOffset()
		{
			Guid vg = Guid.NewGuid();
			byte[] val = ConversionUtils.Guid2Bytes(vg);
			var bytesList = new List<byte> {(byte) '0', (byte) '1'};
			bytesList.AddRange(val);
			val = bytesList.ToArray();
			const int offset = 2;
			Guid actual = ConversionUtils.Bytes2Guid(val, GetOffset(offset));
			Assert.AreEqual(vg, actual);
		}

		/// <summary>
		///A test for DateTime2UnixTime
		///</summary>
		[Test]
		public void DateTime2UnixTimeTest()
		{
			var val = new DateTime(2000, 1, 1, 12, 30, 30);
			const int expected = 946729830;
			int actual = ConversionUtils.DateTime2UnixTime(val);
			Assert.AreEqual(expected, actual);
		}
	}
}