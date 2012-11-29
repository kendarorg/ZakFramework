#if THINGS_YET_TO_TEST
using System;
using System.Threading;

namespace ZakThread.Threading
{

	public class CounterInt64
	{
		private Int64 _value;

		public Int64 GetAndReset()
		{
			return Interlocked.Exchange(ref _value, 0);
		}

		public Int64 Value
		{
			get { return Interlocked.Read(ref _value); }
			set { Interlocked.Exchange(ref _value, value); }
		}

		public static explicit operator Int64(CounterInt64 value)
		{
			return value.Value;
		}

		public static implicit operator CounterInt64(Int64 value)
		{
			return new CounterInt64(value);
		}

		public CounterInt64(long value)
		{
			Value = value;
		}

		public static CounterInt64 operator +(CounterInt64 left, CounterInt64 right)
		{
			CounterInt64 add = left;
			add.Value = left.Value + right.Value;
			return add;
		}

		public static CounterInt64 operator +(CounterInt64 value)
		{
			return value;
		}

		public static CounterInt64 operator ++(CounterInt64 value)
		{
			Interlocked.Increment(ref value._value);
			return value;
		}
	}
}
#endif