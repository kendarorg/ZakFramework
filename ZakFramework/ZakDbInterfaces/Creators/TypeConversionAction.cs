using ZakDb.Descriptors;

namespace ZakDb.Creators
{
	public class TypeConversionAction
	{
		public FieldDescriptor Field { get; set; }
		public object Result { get; set; }
		public object FieldValue { get; set; }
	}
}
