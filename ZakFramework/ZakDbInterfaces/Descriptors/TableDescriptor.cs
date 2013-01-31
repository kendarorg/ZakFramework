using System;
using System.Collections.Generic;
using ZakDb.Repositories;

namespace ZakDb.Descriptors
{
	public class TableDescriptor
	{
		public bool OptimisticLock { get; private set; }
		public KeyDescriptor PrimaryKey { get; private set; }
		public string Name { get; private set; }
		public string Schema { get; private set; }
		public string FullName { get { return Name + (string.IsNullOrWhiteSpace(Schema) ? string.Empty : "." + Schema); } }

		public DatabaseDescriptor Parent { get; internal set; }

		public List<ForeignKeyDescriptor> ForeignKeys { get { return new List<ForeignKeyDescriptor>(_foreignKeys); } }
		private readonly List<ForeignKeyDescriptor> _foreignKeys;
		private readonly List<KeyDescriptor> _keys;
		private readonly List<Field> _fields;

		public List<Field> Fields { get { return new List<Field>(_fields); } }
		public List<KeyDescriptor> Keys { get { return new List<KeyDescriptor>(_keys); } }


		public TableDescriptor(bool optimisticLock, string name, string schema = null)
		{
			OptimisticLock = optimisticLock;
			Name = name;
			Schema = schema;
			_foreignKeys = new List<ForeignKeyDescriptor>();
			_fields = new List<Field>();
			_keys = new List<KeyDescriptor>();
		}

		public TableDescriptor(string name, string schema = null) :
			this(false, name, schema)
		{

		}

		public void SetPrimaryKey(KeyDescriptor pKeyDescriptor)
		{
			pKeyDescriptor.Unique = true;
			VerifyFieldsPresence(pKeyDescriptor.Fields);
			PrimaryKey = pKeyDescriptor;
			_keys.Add(pKeyDescriptor);
		}

		public void AddForeignKey(TableDescriptor descriptor,
			ForeignKeyMultplicity multplicity = ForeignKeyMultplicity.OneToOne)
		{
			AddForeignKey(new ForeignKeyDescriptor(descriptor, multplicity));
		}

		public void AddForeignKey(ForeignKeyDescriptor foreignKey)
		{
			VerifyFieldsPresence(foreignKey.OwnerFields);
			_foreignKeys.Add(foreignKey);
		}

		public void AddKey(KeyDescriptor key)
		{
			VerifyFieldsPresence(key.Fields);
			_keys.Add(key);
		}

		public void AddField(string fieldName, FieldDescriptor fieldDescriptor)
		{
			_fields.Add(new Field(fieldName, fieldDescriptor));
		}


		public Field this[string i]
		{
			get
			{
				i = i.ToLowerInvariant();
				foreach (var field in _fields)
				{
					if (field.FieldName.ToLowerInvariant() == i) return field;
				}
				return null;
			}
		}

		public void VerifyFieldsPresence(string[] propsedFields)
		{
			int expected = propsedFields.Length;
			foreach (var item in propsedFields)
			{
				var propsed = item;
				foreach (var field in _fields)
				{
					if (field.FieldName == propsed) expected--;
				}
			}
			if (expected != 0) throw new Exception("Field not present!");
		}
	}
}
