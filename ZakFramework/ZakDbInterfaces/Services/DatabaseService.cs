using System;
using System.Collections.Generic;
using ZakDb.Descriptors;
using ZakDb.Queries;

namespace ZakDb.Services
{
	public class DatabaseService
	{
		private readonly Dictionary<string, DatabaseDescriptor> _databaseDescriptors;
		private readonly Dictionary<string, FieldDescriptor> _fieldsDescriptors;

		public DatabaseService()
		{
			_databaseDescriptors = new Dictionary<string, DatabaseDescriptor>();
			_fieldsDescriptors = new Dictionary<string, FieldDescriptor>();
			AddFieldDescriptor("guid", new FieldDescriptor
				{
					DataType = typeof(Guid),
				});
			AddFieldDescriptor("autoincrement", new FieldDescriptor
			{
				AutoIncrement = true,
				DataType = typeof(long),
			});
			AddFieldDescriptor("datetime", new FieldDescriptor
			{
				DataType = typeof(DateTime),
			});
		}

		public void AddFieldDescriptor(string descriptorName, FieldDescriptor fieldDescriptor)
		{
			descriptorName = descriptorName.ToLowerInvariant();
			if (!_fieldsDescriptors.ContainsKey(descriptorName)) _fieldsDescriptors.Add(descriptorName, fieldDescriptor);
		}

		public FieldDescriptor GetFieldDescriptor(string descriptorName)
		{
			descriptorName = descriptorName.ToLowerInvariant();
			if (!_fieldsDescriptors.ContainsKey(descriptorName)) return null;
			return _fieldsDescriptors[descriptorName];
		}


		public void RegisterDatabase(DatabaseDescriptor databaseDescriptor)
		{
			var dbname = databaseDescriptor.Name.ToLowerInvariant();
			databaseDescriptor.Parent = this;
			if (!_databaseDescriptors.ContainsKey(dbname)) _databaseDescriptors.Add(dbname, databaseDescriptor);
		}

		public DatabaseDescriptor this[string i]
		{
			get
			{
				i = i.ToLowerInvariant();
				if (_databaseDescriptors.ContainsKey(i))
				{
					return _databaseDescriptors[i];
				}
				return null;
			}
		}

		public TableDescriptor GetTable(string db, string tableWithSchema)
		{
			db = db.ToLowerInvariant();
			tableWithSchema = tableWithSchema.ToLowerInvariant();
			var rdb = this[db];
			if (rdb == null) return null;
			return rdb[tableWithSchema];
		}

		public QueryTable GetQueryTable(string db, string tableWithSchema, string alias)
		{
			var td = GetTable(db, tableWithSchema);
			return new QueryTable(td, alias);
		}

		public bool Verify(bool throwOnError = false)
		{
			foreach (var db in _databaseDescriptors)
			{
				if (!db.Value.Verify(throwOnError)) return false;
			}
			return true;
		}
	}
}
