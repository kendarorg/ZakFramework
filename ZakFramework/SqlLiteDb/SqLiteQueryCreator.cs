using System;
using System.Collections.Generic;
using ZakDb.Creators;
using ZakDb.Descriptors;

namespace SqlLiteDb
{
	/// <summary>
	/// Data Source=c:\mydb.db;Version=3;UseUTF16Encoding=True;
	/// </summary>
	public class SqLiteQueryCreator : Sql99QueryCreator
	{
		public SqLiteQueryCreator()
		{
			Initialize();
		}
		public void Initialize()
		{
			FromDbConversionAction = new Dictionary<Type, Action<TypeConversionAction>>();
			ToDbConversionAction = new Dictionary<Type, Action<TypeConversionAction>>();
			TypeCreatorAction = new Dictionary<Type, Action<TypeCreatorAction>>
				{
					{typeof(Int16),CreateInteger},
					{typeof(Int32),CreateInteger},
					{typeof(Int64),CreateInteger},
					{typeof(UInt16),CreateInteger},
					{typeof(UInt32),CreateInteger},
					{typeof(UInt64),CreateInteger},
					{typeof(Byte),CreateInteger},
					{typeof(SByte),CreateText},
					{typeof(Char),CreateText},
					{typeof(Single),CreateReal},
					{typeof(Double),CreateReal},
					{typeof(Boolean),CreateNumeric},
					{typeof(Decimal),CreateNumeric},
					{typeof(String),CreateText},
					{typeof(byte[]),CreateBlob},
					{typeof(char[]),CreateBlob},
					{typeof(DateTime),CreateNumeric},
					{typeof(Guid),CreateNone},
				};
		}

		private static string CreateField(string type, TypeCreatorAction tca)
		{
			var fd = tca.Field;
			var toret = tca.FieldName + " " + type;
			if (fd.AutoIncrement) toret += " PRIMARY KEY ";
			else if (!fd.IsNullable) toret += " NOT NULL ON CONFLICT FAIL";
			return toret;
		}

		private static void CreateInteger(TypeCreatorAction obj)
		{
			obj.Result = CreateField("INTEGER", obj);
		}

		private static void CreateNone(TypeCreatorAction obj)
		{
			obj.Result = CreateField("", obj);
		}

		private static void CreateBlob(TypeCreatorAction obj)
		{
			obj.Result= CreateField("BLOB", obj);
		}

		private static void CreateText(TypeCreatorAction obj)
		{
			obj.Result= CreateField("TEXT", obj);
		}

		private static void CreateReal(TypeCreatorAction obj)
		{
			obj.Result= CreateField("REAL", obj);
		}

		private static void CreateNumeric(TypeCreatorAction obj)
		{
			obj.Result= CreateField("NUMERIC", obj);
		}

		public override T CreateDatabaseQuery<T>(DatabaseDescriptor dbDescriptor)
		{
			return default(T);
		}

		public override T CreateDataTypeQuery<T>(FieldDescriptor descriptor, string fieldName)
		{
			var tca = new TypeCreatorAction {Field = descriptor,FieldName = fieldName};
			TypeCreatorAction[descriptor.DataType](tca);
			return (T)tca.Result;
		}
	}
}
