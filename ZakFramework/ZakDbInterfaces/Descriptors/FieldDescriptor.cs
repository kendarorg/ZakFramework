using System;
using System.Threading;

namespace ZakDb.Descriptors
{
	public class FieldDescriptor
	{
		private static int _comparer= 0;
		private int _hash=0;
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
			_hash = Interlocked.Increment(ref _comparer);
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
			return a.Equals(b);
		}

		public static bool operator !=(FieldDescriptor a, FieldDescriptor b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			var fd = obj as FieldDescriptor;
			if(fd == null) return false;
			return base.Equals(fd);
		}

		private bool Equals(FieldDescriptor b)
		{
			// If both are null, or both are same instance, return true.
			if (ReferenceEquals(this, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if ( ((object)b == null))
			{
				return false;
			}

			// Return true if the fields match:
			return (StartAsNotSet == b.StartAsNotSet) &&
						 (AutoIncrement == b.AutoIncrement) &&
						 (DataType == b.DataType) &&
						 (MinLength == b.MinLength) &&
						 (MaxLength == b.MaxLength) &&
						 (MinValue == b.MinValue) &&
						 (MaxValue == b.MaxValue) &&
						 (Default == b.Default) &&
						 (Precision == b.Precision) &&
						 (IsNullable == b.IsNullable);
		}

		public override int GetHashCode()
		{
			return _hash;
		}
	}
}