
using System.Threading;

namespace ZakThread.Test.Async
{
	public class CounterContainer
	{
		private long _counter;

		public CounterContainer(long startingValue = 0)
		{
			Counter = startingValue;
		}

		public void Increment()
		{
			Interlocked.Increment(ref _counter);
		}

		public void Decrement()
		{
			Interlocked.Decrement(ref _counter);
		}

		public long Counter
		{
			get { return Interlocked.Read(ref _counter); }
			set { Interlocked.Exchange(ref _counter, value); }
		}

		public void Add(long value)
		{
			for (long i = 0; i < value; i++)
			{
				Increment();
			}
		}

		public void Subtract(long value)
		{
			for (long i = 0; i < value; i++)
			{
				Decrement();
			}
		}
	}
}
