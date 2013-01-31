using System;
using System.Collections.Generic;
using ZakDb.Creators;
using ZakDb.Descriptors;

namespace ZakDb.Queries
{
	public interface IQueryCreator
	{
		Dictionary<Type, Action<TypeCreatorAction>> TypeCreatorAction { get;  set; }
		Dictionary<Type, Action<TypeConversionAction>> FromDbConversionAction { get; set; }
		Dictionary<Type, Action<TypeConversionAction>> ToDbConversionAction { get; set; }
		string CreateTable { get; }
		string FieldAs { get; }
		string TableAs { get; }
		string Select { get; }
		string From { get; }
		string Where { get; }
		string SubOpen { get; }
		string SubClose { get; }
		string And { get; }
		string Or { get; }
		string IsNull { get; }
		string IsNotNull { get; }
		string Lt { get; }
		string Lte { get; }
		string Gt { get; }
		string Gte { get; }
		string In { get; }
		bool Validate(TableDescriptor tableDescriptor, bool exceptionOnError = false);
		T CreateTableQuery<T>(TableDescriptor tableDescriptor);
		T CreateDatabaseQuery<T>(DatabaseDescriptor dbDescriptor);
		T CreateQuery<T>(QueryTable table);
		T CreateDataTypeQuery<T>(FieldDescriptor descriptor, string fieldName);
	}
}