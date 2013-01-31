using System;
using System.Collections.Generic;
using ZakDb.Repositories;
using ZakDb.Services;

namespace ZakDb.Descriptors
{
	public class DatabaseDescriptor
	{
		public string Name { get; private set; }
		public IDbDriver DBDriver { get; private set; }
		private readonly Dictionary<string, TableDescriptor> _tableDescriptors;
		public DatabaseService Parent { get; internal set; }

		public DatabaseDescriptor(string name,IDbDriver dbDriver)
		{
			Name = name;
			DBDriver = dbDriver;
			_tableDescriptors = new Dictionary<string, TableDescriptor>();
		}

		public void RegisterTable(TableDescriptor tableDescriptor)
		{
			var tbname = tableDescriptor.FullName.ToLowerInvariant();
			tableDescriptor.Parent = this;
			if (!_tableDescriptors.ContainsKey(tbname)) _tableDescriptors.Add(tbname, tableDescriptor);
		}

		public TableDescriptor this[string i]
		{
			get
			{
				i = i.ToLowerInvariant();
				if (_tableDescriptors.ContainsKey(i))
				{
					return _tableDescriptors[i];
				}
				return null;
			}
		}

		private bool ExceptionOnError(bool exceptionOnError, bool value = false)
		{
			if (exceptionOnError && !value) throw new Exception();
			return value;
		}

		public bool Verify(bool throwOnError = false)
		{
			foreach (var ownerTable in _tableDescriptors)
			{
				var td = ownerTable.Value;
				if (!DBDriver.Verify(td, throwOnError)) return false;
				foreach (var fk in td.ForeignKeys)
				{
					var ownedTable = this[fk.Descriptor.FullName];
					if (ownedTable == null) return ExceptionOnError(throwOnError);
					for (int index = 0; index < fk.OwnedFields.Length; index++)
					{
						var ownedField = ownedTable[fk.OwnedFields[index]];
						var ownerField = td[fk.OwnerFields[index]];
						if (ownerField.FieldDescriptor != ownedField.FieldDescriptor) return ExceptionOnError(throwOnError);
					}
				}
			}
			return true;
		}

		public void CreateDb()
		{
			DBDriver.CreateDb(Name);
			foreach (var table in _tableDescriptors.Values)
			{
				DBDriver.CreateTable(table);
			}
		}
	}
}
