using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZakCore.Utils.Commons;

namespace ZakCoreUtils.Test
{


	/// <summary>
	///This is a test class for CrontabTest and is intended
	///to contain all CrontabTest Unit Tests
	///</summary>
	[TestClass]
	public class CrontabTest
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
		///A test for Crontab Constructor
		///</summary>
		[TestMethod]
		public void CrontabMayRunAt()
		{
			var target = new Crontab("* * * * * * *");
			Assert.IsTrue(target.MayRunAt(new DateTime(2000, 1, 1, 1, 1, 0)));

			target = new Crontab("* */1 * * * * *");
			Assert.IsTrue(target.MayRunAt(new DateTime(2000, 1, 1, 1, 1, 0)));

			target = new Crontab("* 2-59/3 1,9,22 11-26 1-6 ? 2003");
			Assert.IsTrue(target.MayRunAt(new DateTime(2003, 1, 11, 9, 5, 0)));
			Assert.IsFalse (target.MayRunAt(new DateTime(2000, 1, 11, 9, 5, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 1, 11, 9, 6, 0)));

			target = new Crontab("* 2-59/3 1,9,22 11-26 1-6 ? 2003-2012");
			Assert.IsTrue(target.MayRunAt(new DateTime(2008, 1, 11, 9, 5, 0)));

			target = new Crontab("* 0 0/1 * * * ?");
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 1, 1, 1, 1, 0)));
			Assert.IsTrue(target.MayRunAt(new DateTime(2000, 1, 1, 1, 0, 0)));
		}

		/// <summary>
		///A test for Crontab Constructor
		///</summary>
		[TestMethod]
		public void CrontabNextRun()
		{
			var target = new Crontab("* * * * * * *");
			Assert.AreEqual(target.Next(new DateTime(2000, 1, 1, 1, 1, 0)), new DateTime(2000, 1, 1, 1, 1, 0));

			target = new Crontab("* */1 * * * * *");
			Assert.AreEqual(target.Next(new DateTime(2000, 1, 1, 1, 1, 0)), new DateTime(2000, 1, 1, 1, 1, 0));

			target = new Crontab("* 2-59/3 1,9,22 11-26 1-6 ? 2003");
			Assert.AreEqual(target.Next(new DateTime(2003, 1, 11, 9, 5, 0)), new DateTime(2003, 1, 11, 9, 5, 0));
			Assert.AreEqual(target.Next(new DateTime(2003, 1, 11, 9, 6, 0)), new DateTime(2003, 1, 11, 9, 8, 0));
			Assert.AreEqual(target.Next(new DateTime(2003, 1, 11, 23, 6, 0)), new DateTime(2003, 1, 12, 1, 2, 0));
			Assert.AreEqual(target.Next(new DateTime(2003, 1, 26, 23, 00, 0)), new DateTime(2003, 2, 11, 1, 2, 0));

			target = new Crontab("* 2-59/3 1,9,22 11-26 1-6 ? 2003-2012");
			Assert.AreEqual(target.Next(new DateTime(2008, 7, 11, 9, 5, 0)), new DateTime(2009, 1, 11, 1, 2, 0));
		}

		/// <summary>
		///A test for Crontab Constructor
		///</summary>
		[TestMethod]
		public void CrontabSpecial()
		{
			var target = new Crontab("@yearly");
			Assert.IsTrue(target.MayRunAt(new DateTime(2000, 1, 1, 0, 0, 0)));
			Assert.IsTrue(target.MayRunAt(new DateTime(2001, 1, 1, 0, 0, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 1, 1, 0, 1, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 1, 1, 1, 0, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 1, 2, 0, 0, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 2, 1, 0, 0, 0)));

			target = new Crontab("@annually");
			Assert.IsTrue(target.MayRunAt(new DateTime(2000, 1, 1, 0, 0, 0)));
			Assert.IsTrue(target.MayRunAt(new DateTime(2001, 1, 1, 0, 0, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 1, 1, 0, 1, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 1, 1, 1, 0, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 1, 2, 0, 0, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 2, 1, 0, 0, 0)));

			target = new Crontab("@monthly");
			Assert.IsTrue(target.MayRunAt(new DateTime(2001, 2, 1, 0, 0, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2000, 2, 1, 0, 1, 0)));


			target = new Crontab("@weekly");
			Assert.IsTrue(target.MayRunAt(new DateTime(2012, 4, 15, 0, 0, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2012, 4, 18, 0, 0, 0)));

			target = new Crontab("@daily");
			Assert.IsTrue(target.MayRunAt(new DateTime(2012, 4, 15, 0, 0, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2012, 4, 15, 0, 1, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2012, 4, 18, 1, 0, 0)));

			target = new Crontab("@hourly");
			Assert.IsTrue(target.MayRunAt(new DateTime(2012, 4, 15, 1, 0, 0)));
			Assert.IsFalse(target.MayRunAt(new DateTime(2012, 4, 15, 1, 1, 0)));
		}

		[TestMethod]
		public void CrontabExceed()
		{
			var dt = new DateTime(2000, 1, 1, 1, 0, 0);
			var target = new Crontab("* */80 * * * * *");
			Assert.IsTrue(target.MayRunAt(dt));
			dt += TimeSpan.FromMinutes(40);
			Assert.IsFalse(target.MayRunAt(dt));
			dt += TimeSpan.FromMinutes(40);
			Assert.IsFalse(target.MayRunAt(dt));
		}
	}
}
