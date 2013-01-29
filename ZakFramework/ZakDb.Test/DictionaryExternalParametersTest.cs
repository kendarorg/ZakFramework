
using System;
using System.Collections.Generic;
using NUnit.Framework;
using ZakDb.Utils;

namespace ZakDb.Test
{
	[TestFixture]
	public class DictionaryExternalParametersTest
	{
		[Test]
		public void CreateAnEmptyDictionary()
		{
			var dict = new DictionaryExternalParameters(new Dictionary<string, object>());
			Assert.IsNull(dict[Guid.NewGuid().ToString()]);
			dict.ResetSession();
		}

		[Test]
		public void ItShouldBePossibleToUseDefaultWhenValueIsNotPresent()
		{
			var defaults = new Dictionary<string, object>
				{
					{"Test", "TestValue"},
					{"Test1", 1}
				};
			var dict = new DictionaryExternalParameters(defaults);
			dict.ResetSession();
			Assert.IsNull(dict[Guid.NewGuid().ToString()]);
			Assert.IsNull(dict.GetAs<string>("NotPresent"));
			Assert.AreEqual(0,dict.GetAs<int>("NotPresent"));

			Assert.AreEqual("test",dict.GetAs("NotPresent","test"));
			Assert.AreEqual(-1, dict.GetAs("NotPresent",-1));

			Assert.AreEqual(defaults["Test"], dict.GetAs<string>("Test"));
			Assert.AreEqual(defaults["Test"], dict["Test"]);
			Assert.AreEqual(defaults["Test1"], dict.GetAs<int>("Test1"));
			Assert.AreEqual(defaults["Test1"], dict["Test1"]);
		}

		[Test]
		public void ItShouldBePossibleToSetValuesOnSession()
		{
			var defaults = new Dictionary<string, object>
				{
					{"Test", "TestValue"},
					{"Test1", 1}
				};
			var dict = new DictionaryExternalParameters(defaults);
			dict.ResetSession();
			Assert.IsNull(dict[Guid.NewGuid().ToString()]);
			Assert.IsNull(dict.GetAs<string>("NotPresent"));
			Assert.AreEqual(0, dict.GetAs<int>("NotPresent"));

			Assert.AreEqual("test", dict.GetAs("NotPresent", "test"));
			Assert.AreEqual(-1, dict.GetAs("NotPresent", -1));

			dict["NotPresent"] = "Val";

			Assert.AreEqual(dict["NotPresent"], "Val");
			Assert.AreEqual(dict["NotPresent"],dict.GetAs<string>("NotPresent"));
			Assert.AreEqual("Val", dict.GetAs("NotPresent", "test"));
		}

		[Test]
		public void ItShouldBePossibleToChangeValuesOnSession()
		{
			var defaults = new Dictionary<string, object>
				{
					{"Test", "TestValue"},
					{"Test1", 1}
				};
			var dict = new DictionaryExternalParameters(defaults);
			dict.ResetSession();
			Assert.AreEqual(dict["Test"], "TestValue");
			Assert.AreEqual(dict["Test"], dict.GetAs<string>("Test"));
			Assert.AreEqual("TestValue", dict.GetAs("Test", "test"));

			dict["Test"] = "Val";

			Assert.AreEqual(dict["Test"], "Val");
			Assert.AreEqual(dict["Test"], dict.GetAs<string>("Test"));
			Assert.AreEqual("Val", dict.GetAs("Test", "test"));
		}
	}
}
