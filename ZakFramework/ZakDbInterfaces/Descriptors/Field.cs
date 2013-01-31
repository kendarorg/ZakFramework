namespace ZakDb.Descriptors
{
	public class Field
	{
		public string FieldName { get; private set; }
		public FieldDescriptor FieldDescriptor { get; private set; }

		public Field(string fieldName, FieldDescriptor fieldDescriptor)
		{
			FieldName = fieldName;
			FieldDescriptor = fieldDescriptor;
		}
	}
}