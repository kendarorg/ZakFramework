using System;
using MongoDbDb;
using NUnit.Framework;
using SqlLiteDb;
using ZakDb.Descriptors;
using ZakDb.Services;

namespace ZakDb.Test
{
	[TestFixture]
	public class TableDescriptorsTest
	{
		private TableDescriptor CreateUsers(DatabaseService ds)
		{
			var kd = new KeyDescriptor("Id");
			var td = new TableDescriptor("Users");
			td.AddField("Id", ds.GetFieldDescriptor("guid"));
			td.AddField("Username", ds.GetFieldDescriptor("username"));
			td.AddField("Password", ds.GetFieldDescriptor("password"));
			td.AddField("Nickname", ds.GetFieldDescriptor("nickname"));
			td.SetPrimaryKey(kd);
			kd = new KeyDescriptor("Username");
			kd.SetType(true, false, true);
			td.AddKey(kd);
			return td;
		}

		private TableDescriptor CreateRoles(DatabaseService ds)
		{
			var kd = new KeyDescriptor("Id");
			var td = new TableDescriptor("Roles");
			td.AddField("Id", ds.GetFieldDescriptor("guid"));
			td.AddField("UsersId", ds.GetFieldDescriptor("guid"));
			td.AddField("Role", ds.GetFieldDescriptor("role"));
			td.SetPrimaryKey(kd);
			kd = new KeyDescriptor("UsersId");
			kd.SetType(true, false, true);
			td.AddKey(kd);
			return td;
		}


		private TableDescriptor CreateQueue(DatabaseService ds)
		{
			var kd = new KeyDescriptor("Id");
			var td = new TableDescriptor("Queue");
			td.AddField("Id", ds.GetFieldDescriptor("guid"));
			td.AddField("UsersId", ds.GetFieldDescriptor("guid"));
			td.AddField("Message", ds.GetFieldDescriptor("message"));
			td.AddField("MessageType", ds.GetFieldDescriptor("messagetype"));
			td.AddField("Destination", ds.GetFieldDescriptor("destination"));
			td.SetPrimaryKey(kd);
			kd = new KeyDescriptor("UsersId");
			kd.SetType(true, false, true);
			td.AddKey(kd);
			kd = new KeyDescriptor("Destination");
			kd.SetType(false, false, true);
			td.AddKey(kd);
			kd = new KeyDescriptor("MessageType");
			kd.SetType(false, false, true);
			td.AddKey(kd);
			return td;
		}

		private static void RegisterDataTypes(DatabaseService ds)
		{
			ds.AddFieldDescriptor("username", new FieldDescriptor
				{
					DataType = typeof(string),
					MinLength = 5,
					MaxLength = 30
				});
			ds.AddFieldDescriptor("nickname", new FieldDescriptor
				{
					DataType = typeof(string),
					MinLength = 5,
					MaxLength = 30
				});
			ds.AddFieldDescriptor("password", new FieldDescriptor
				{
					DataType = typeof(string),
					MinLength = 8,
					MaxLength = 30
				});
			ds.AddFieldDescriptor("role", new FieldDescriptor
				{
					DataType = typeof(string),
					MinLength = 8,
					MaxLength = 30
				});

			ds.AddFieldDescriptor("timestamp", new FieldDescriptor
			{
				DataType = typeof(DateTime),
				MinLength = 8,
				MaxLength = 30
			});
			ds.AddFieldDescriptor("message", new FieldDescriptor
			{
				DataType = typeof(string),
				MinLength = 0,
				MaxLength = 16000
			});
			ds.AddFieldDescriptor("destination", new FieldDescriptor
				{
					DataType = typeof(string),
					MinLength = 10,
					MaxLength = 10
				});
			ds.AddFieldDescriptor("messagetype", new FieldDescriptor
			{
				DataType = typeof(string),
				MinLength = 10,
				MaxLength = 10
			});
		}

		private static void SetupFkFromUsersToRoles(TableDescriptor roles, TableDescriptor users)
		{
			var fk = new ForeignKeyDescriptor(roles);
			fk.SetFields("Id", "UsersId");
			users.AddForeignKey(fk);
		}



		[Test]
		public void RegisterTableDescriptor()
		{
			var ds = new DatabaseService();
			var mysql = new DatabaseDescriptor("UserManagement", new SqLiteDbDriver(
				"Data Source=:memory:"));
			var mongo = new DatabaseDescriptor("CommandQueue", new MongoDbDriver(
				"mongodb://db1.example.net,db2.example.net:2500/?replicaSet=test"));

			ds.RegisterDatabase(mysql);
			ds.RegisterDatabase(mongo);
			RegisterDataTypes(ds);

			var users = CreateUsers(ds);
			var roles = CreateRoles(ds);
			SetupFkFromUsersToRoles(roles, users);

			ds["UserManagement"].RegisterTable(users);
			ds["UserManagement"].RegisterTable(roles);
			ds["UserManagement"].Verify();
			ds["UserManagement"].CreateDb();

			var queue = CreateQueue(ds);

			ds["CommandQueue"].RegisterTable(queue);
			ds["CommandQueue"].Verify();

			Assert.IsTrue(ds.Verify());
		}
	}
}
