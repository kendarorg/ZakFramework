using System;
using System.Collections.Generic;
using ZakDb.Creators;
using ZakDb.Descriptors;
using ZakDb.Queries;

namespace MongoDbDb
{
	public class MongoDbQueryCreator : IQueryCreator
	{
		public Dictionary<Type, Action<TypeCreatorAction>> TypeCreatorAction { get; set; }
		public Dictionary<Type, Action<TypeConversionAction>> FromDbConversionAction { get; set; }
		public Dictionary<Type, Action<TypeConversionAction>> ToDbConversionAction { get; set; }
		public string CreateTable { get; private set; }
		public string FieldAs { get; private set; }
		public string TableAs { get; private set; }
		public string Select { get; private set; }
		public string From { get; private set; }
		public string Where { get; private set; }
		public string SubOpen { get; private set; }
		public string SubClose { get; private set; }
		public string And { get; private set; }
		public string Or { get; private set; }
		public string IsNull { get; private set; }
		public string IsNotNull { get; private set; }
		public string Lt { get; private set; }
		public string Lte { get; private set; }
		public string Gt { get; private set; }
		public string Gte { get; private set; }
		public string In { get; private set; }
		public bool Validate(TableDescriptor tableDescriptor, bool exceptionOnError = false)
		{
			throw new NotImplementedException();
		}

		public T CreateTableQuery<T>(TableDescriptor tableDescriptor)
		{
			throw new NotImplementedException();
		}

		public T CreateDatabaseQuery<T>(DatabaseDescriptor dbDescriptor)
		{
			throw new NotImplementedException();
		}

		public T CreateQuery<T>(QueryTable table)
		{
			throw new NotImplementedException();
		}

		public T CreateDataTypeQuery<T>(FieldDescriptor descriptor, string fieldName)
		{
			throw new NotImplementedException();
		}
	}
}
