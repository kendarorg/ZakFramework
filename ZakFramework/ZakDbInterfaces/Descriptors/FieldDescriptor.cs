using System;

namespace ZakDb.Descriptors
{
	public class FieldDescriptor
	{
		public FieldDescriptor()
		{
			AutoIncrement = false;
			DataType = null;
			MaxLength = -1;
			MinLength = -1;
			MaxValue = -1;
			MinValue = -1;
			StartAsNotSet = true;
			Default = null;
			Precision = -1;
			IsNullable = true;
		}

		public bool IsNullable { get; set; }

		public bool StartAsNotSet { get; set; }
		public bool AutoIncrement { get; set; }
		public Type DataType { get; set; }
		public int MinLength { get; set; }
		public int MaxLength { get; set; }
		public int Precision { get; set; }

		public object MinValue { get; set; }
		public object MaxValue { get; set; }
		public object Default { get; set; }

		public static bool operator ==(FieldDescriptor a, FieldDescriptor b)
		{
			// If both are null, or both are same instance, return true.
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			// Return true if the fields match:
			return (a.StartAsNotSet == b.StartAsNotSet) &&
			       (a.AutoIncrement == b.AutoIncrement) &&
			       (a.DataType == b.DataType) &&
			       (a.MinLength == b.MinLength) &&
			       (a.MaxLength == b.MaxLength) &&
			       (a.MinValue == b.MinValue) &&
			       (a.MaxValue == b.MaxValue) &&
			       (a.Default == b.Default) &&
			       (a.Precision == b.Precision) &&
			       (a.IsNullable == b.IsNullable);
		}

		public static bool operator !=(FieldDescriptor a, FieldDescriptor b)
		{
			return !(a == b);
		}
	}
}