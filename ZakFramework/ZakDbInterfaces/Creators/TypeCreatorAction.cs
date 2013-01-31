using System;
using ZakDb.Descriptors;

namespace ZakDb.Creators
{
	public class TypeCreatorAction
	{
		public FieldDescriptor Field { get; set; }
		public object Result { get; set; }
		public string FieldName { get; set; }
	}
}
