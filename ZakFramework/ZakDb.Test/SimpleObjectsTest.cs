using System;
using NUnit.Framework;
using ZakDb.Exceptions;
using ZakDb.Plugins;
using ZakDb.Repositories.Exceptions;
using ZakDb.Repositories.Queries;

namespace ZakDb.Test
{
	[TestFixture]
	public class SimpleObjectsTest
	{
			[Test]
			public void CreatePluginOutcome()
			{
				var po = new PluginOutcome();
				Assert.IsFalse(po.Success);
				Assert.IsNull(po.Result);
			}

			[Test]
			public void CreateZakException()
			{
				var po = new ZakException("TestMessage");
				Assert.AreEqual(po.Message,"TestMessage");

				var ex = new Exception("test");
				po = new ZakException("TestMessage",ex);
				Assert.AreEqual(po.Message, "TestMessage");
				Assert.AreSame(po.InnerException, ex);
			}

		[Test]
			public void CreateRepositoryDuplicateKeyException()
		{
			var po = new RepositoryDuplicateKeyException("Module","Operation");
			Assert.IsTrue(po.Message.Contains("Module"));
			Assert.IsTrue(po.Message.Contains("Operation"));

			var ex = new Exception("test");
			po = new RepositoryDuplicateKeyException("Module", "Operation", ex);
			Assert.IsTrue(po.Message.Contains("Module"));
			Assert.IsTrue(po.Message.Contains("Operation"));
			Assert.AreSame(po.InnerException, ex);
		}

		[Test]
		public void CreateRepositoryValidationException()
		{
			var po = new RepositoryValidationException("Module", "Operation","Field","Cause");
			Assert.IsTrue(po.Message.Contains("Module"));
			Assert.IsTrue(po.Message.Contains("Operation"));
			Assert.IsTrue(po.Message.Contains("Field"));
			Assert.IsTrue(po.Message.Contains("Cause"));
			Assert.AreEqual(po.Field,"Field");
			Assert.AreEqual(po.Error, "Cause");

			var ex = new Exception("test");
			po = new RepositoryValidationException("Module", "Operation", "Field", "Cause", ex);
			Assert.IsTrue(po.Message.Contains("Module"));
			Assert.IsTrue(po.Message.Contains("Operation"));
			Assert.IsTrue(po.Message.Contains("Field"));
			Assert.IsTrue(po.Message.Contains("Cause"));
			Assert.AreEqual(po.Field, "Field");
			Assert.AreEqual(po.Error, "Cause");
			Assert.AreSame(po.InnerException, ex);
		}

		[Test]
		public void CreateAsIsParameter()
		{
			var ai = new AsIsParameter {Content = "test"};
			Assert.AreEqual(ai.ToString(),ai.Content);
		}

		[Test]
		public void CreateQueryObject()
		{
			var ai = new QueryObject();
			Assert.AreEqual(ai.Limit, -1);
			Assert.AreEqual(ai.Skip, -1);
			Assert.AreEqual(ai.WhereCondition, String.Empty);
			Assert.AreEqual(ai.OrderByCondition, String.Empty);
			Assert.AreEqual(ai.UseJoins, true);
			Assert.AreEqual(ai.Action, QueryAction.Select);
			Assert.AreEqual(ai.SelectFields, string.Empty);
			Assert.AreEqual(ai.QueryBody, string.Empty);
			Assert.AreEqual(ai.TypeOfQuery, QueryType.Query);
			Assert.AreEqual(ai.ForceSelectField, false);
		}
	}
}
