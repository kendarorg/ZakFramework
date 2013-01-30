
using System;
using NUnit.Framework;
using ZakDb.Queries;

namespace ZakDb.Test
{
	[TestFixture]
	public class NewQueryTest
	{
		private string PurgeQuery(string toPurge)
		{
			return toPurge.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
		}

		[Test]
		public void ItShouldBePossibleToCreateASimpleSelectQuery()
		{
			var id = Guid.NewGuid();
			var table = new QueryTable("Users");
			var creator = new Sql99QueryCreator();
			var expected = PurgeQuery(string.Format("SELECT {0}.Id AS {0}_Id,{0}.UserName AS {0}_UserName," +
																	 "{0}.Password AS {0}_Password FROM Users AS {0}" +
																	 " WHERE {0}_Id = '{1}'", table.Alias, id));

			table.AddField("Id");
			table.AddField("UserName");
			table.AddField("Password");

			table.Eq(id).SetFieldName("Id");

			var result = PurgeQuery(creator.CreateQuery(table));
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ItShouldBePossibleToCreateASimpleSelectQueryWithAnd()
		{
			var id = Guid.NewGuid();
			const string userName = "testUser";
			var table = new QueryTable("Users");
			var creator = new Sql99QueryCreator();
			var expected = PurgeQuery(string.Format("SELECT {0}.Id AS {0}_Id,{0}.UserName AS {0}_UserName," +
				"{0}.Password AS {0}_Password FROM Users AS {0} WHERE" +
				"  ( {0}_UserName = '{1}' AND {0}_Id = '{2}' )", table.Alias, userName, id));

			table.AddField("Id");
			table.AddField("UserName");
			table.AddField("Password");

			table.And(
				table.Eq(userName).SetFieldName("UserName"),
				table.Eq(id).SetFieldName("Id")
				);

			var result = PurgeQuery(creator.CreateQuery(table));
			Assert.AreEqual(expected, result);
		}


		[Test]
		public void ItShouldBePossibleToCreateASimpleSelectQueryWithOr()
		{
			var id = Guid.NewGuid();
			const string userName = "testUser";
			var table = new QueryTable("Users");
			var creator = new Sql99QueryCreator();
			var expected = PurgeQuery(string.Format("SELECT {0}.Id AS {0}_Id,{0}.UserName AS {0}_UserName," +
				"{0}.Password AS {0}_Password FROM Users AS {0} WHERE" +
				"  ( {0}_UserName = '{1}' OR {0}_Id = '{2}' )", table.Alias, userName, id));

			table.AddField("Id");
			table.AddField("UserName");
			table.AddField("Password");

			table.Or(
				table.Eq(userName).SetFieldName("UserName"),
				table.Eq(id).SetFieldName("Id")
				);

			var result = PurgeQuery(creator.CreateQuery(table));
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ItShouldBePossibleToCreateASubQuery()
		{
			var id = Guid.NewGuid();
			const string userName = "testUser";
			const string firstName = "Doe";
			var table = new QueryTable("Users");
			var creator = new Sql99QueryCreator();
			var expected = PurgeQuery(string.Format("SELECT {0}.Id AS {0}_Id,{0}.UserName AS {0}_UserName," +
				"{0}.Password AS {0}_Password,{0}.FirstName AS {0}_FirstName FROM Users AS {0} WHERE" +
				"  ( {0}_UserName = '{1}' AND ( {0}_Id = '{2}' OR {0}_FirstName = '{3}' ) )", 
				table.Alias, userName, id, firstName));

			table.AddField("Id");
			table.AddField("UserName");
			table.AddField("Password");
			table.AddField("FirstName");

			table.And(
				table.Eq(userName).SetFieldName("UserName"),
				table.Or(
					table.Eq(id).SetFieldName("Id"),
					table.Eq(firstName).SetFieldName("FirstName")
				)
			);

			var result = PurgeQuery(creator.CreateQuery(table));
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ItShouldBePossibleToSetupNullNotNullQuery()
		{
			var table = new QueryTable("Users");
			var creator = new Sql99QueryCreator();
			var expected = PurgeQuery(string.Format("SELECT {0}.Id AS {0}_Id,{0}.UserName AS {0}_UserName," +
																	 "{0}.Password AS {0}_Password FROM Users AS {0}" +
																	 " WHERE ( {0}_Id IS NOT NULL AND {0}_UserName IS NULL )", table.Alias));

			table.AddField("Id");
			table.AddField("UserName");
			table.AddField("Password");

			table.And(
				table.IsNotNull().SetFieldName("Id"),
				table.IsNull().SetFieldName("UserName")
				);

			var result = PurgeQuery(creator.CreateQuery(table));
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ItShouldBePossibleToSetupGtLtQueries()
		{
			const int id = 10;
			const string userName = "test";
			var table = new QueryTable("Users");
			var creator = new Sql99QueryCreator();
			var expected = PurgeQuery(string.Format("SELECT {0}.Id AS {0}_Id,{0}.UserName AS {0}_UserName," +
				"{0}.Password AS {0}_Password FROM Users AS {0}" +
				" WHERE ( {0}_Id > '{1}' AND {0}_UserName < '{2}' )", table.Alias,id,userName));

			table.AddField("Id");
			table.AddField("UserName");
			table.AddField("Password");

			table.And(
				table.Gt(id).SetFieldName("Id"),
				table.Lt(userName).SetFieldName("UserName")
				);

			var result = PurgeQuery(creator.CreateQuery(table));
			Assert.AreEqual(expected, result);
		}
	}
}
