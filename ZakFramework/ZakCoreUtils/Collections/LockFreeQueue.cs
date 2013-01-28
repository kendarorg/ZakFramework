using System;
using System.Collections.Generic;
using System.Threading;

namespace ZakCore.Utils.Collections
{
	/// <summary>
	/// Lock free queue implementation
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class LockFreeQueue<T> : IQueue<T>
	{
		/// <summary>
		/// Internal queue node
		/// </summary>
		private class NodeT
		{
			/// <summary>
			/// Contained value
			/// </summary>
			public T _value;

			/// <summary>
			/// Next element
			/// </summary>
			public PointerT _next;
		}

		/// <summary>
		/// Pointer to next node
		/// </summary>
		private struct PointerT
		{
			/// <summary>
			/// Counter
			/// </summary>
			public Int64 _count;

			/// <summary>
			/// Next node
			/// </summary>
			public NodeT _ptr;

			/// <summary>
			/// constructor that allows caller to specify ptr and count
			/// </summary>
			/// <param name="node"></param>
			/// <param name="c"></param>
			public PointerT(NodeT node, long c)
			{
				_ptr = node;
				_count = c;
			}
		}

		/// <summary>
		/// Queue head
		/// </summary>
		private PointerT _head;

		/// <summary>
		/// Queue tail
		/// </summary>
		private PointerT _tail;

		private readonly bool _isNullable;

		private long _count;

		public long Count { get { return Interlocked.Read(ref _count); } }

		/// <summary>
		/// Constructor
		/// </summary>
		public LockFreeQueue()
		{
			_isNullable = default(T) == null;
			var node = new NodeT();
			_head._ptr = _tail._ptr = node;
		}

		/// <summary>
		/// CAS
		/// stands for Compare And Swap
		/// Interlocked Compare and Exchange operation
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="compared"></param>
		/// <param name="exchange"></param>
		/// <returns></returns>
		private static Boolean CAS(ref PointerT destination, PointerT compared, PointerT exchange)
		{
			if (compared._ptr == Interlocked.CompareExchange(ref destination._ptr, exchange._ptr, compared._ptr))
			{
				Interlocked.Exchange(ref destination._count, exchange._count);
				return true;
			}
			//Console.Write("X");
			return false;
		}

		/// <summary>
		/// Dequeue a single element
		/// </summary>
		/// <param name="t">The element to dequeue</param>
		/// <returns></returns>
		public Boolean Dequeue(ref T t)
		{
			// ReSharper disable TooWideLocalVariableScope
			PointerT head;
			// ReSharper restore TooWideLocalVariableScope

			// Keep trying until deque is done
			Boolean bDequeNotDone = true;
			while (bDequeNotDone)
			{
				// read head
				head = _head;

				// read tail
				PointerT tail = _tail;

				// read next
				PointerT next = head._ptr._next;

				// Are head, tail, and next consistent?
				if (head._count == _head._count && head._ptr == _head._ptr)
				{
					// is tail falling behind
					if (head._ptr == tail._ptr)
					{
						// is the queue empty?
						if (null == next._ptr)
						{
							// queue is empty cannnot dequeue
							return false;
						}

						// Tail is falling behind. try to advance it
						CAS(ref _tail, tail, new PointerT(next._ptr, tail._count + 1));
					} // endif

					else // No need to deal with tail
					{
						// read value before CAS otherwise another deque might try to free the next node
						t = next._ptr._value;

						// try to swing the head to the next node
						if (CAS(ref _head, head, new PointerT(next._ptr, head._count + 1)))
						{
							bDequeNotDone = false;
						}
					}
				} // endif
			} // endloop

			// dispose of head.ptr
			Interlocked.Decrement(ref _count); 
			return true;
		}

		/// <summary>
		/// Enqueue a single element
		/// </summary>
		/// <param name="t"></param>
		public void Enqueue(T t)
		{
			// Allocate a new node from the free list
			var node = new NodeT { _value = t };

			// copy enqueued value into node

			// keep trying until Enqueue is done
			Boolean bEnqueueNotDone = true;

			while (bEnqueueNotDone)
			{
				// read Tail.ptr and Tail.count together
				PointerT tail = _tail;

				// read next ptr and next count together
				PointerT next = tail._ptr._next;

				// are tail and next consistent
				if (tail._count == _tail._count && tail._ptr == _tail._ptr)
				{
					// was tail pointing to the last node?
					if (null == next._ptr)
					{
						if (CAS(ref tail._ptr._next, next, new PointerT(node, next._count + 1)))
						{
							bEnqueueNotDone = false;
						} // endif
					} // endif

					else // tail was not pointing to last node
					{
						// try to swing Tail to the next node
						CAS(ref _tail, tail, new PointerT(next._ptr, tail._count + 1));
					}
				} // endif
			} // endloop
			Interlocked.Increment(ref _count); 
		}

		/// <summary>
		/// Dequeu multiple elements
		/// </summary>
		/// <param name="count">The maximum number of elements to dequeue (default Int64.MaxValue) </param>
		/// <returns></returns>
		public IEnumerable<T> Dequeue(Int64 count = Int64.MaxValue)
		{
			// ReSharper disable ExpressionIsAlwaysNull
			object obj = null;
			if (!_isNullable)
			{
				obj = default(T);
			}
			var nd = (T)obj;
			while (Dequeue(ref nd) && count > 0)
			{
				yield return nd;
				count--;
			}
			// ReSharper restore ExpressionIsAlwaysNull
		}

		public T DequeueSingle()
		{
			// ReSharper disable ExpressionIsAlwaysNull
			object obj = null;
			if (!_isNullable)
			{
				obj = default(T);
			}
			
			var nd = (T)obj;
			if (Dequeue(ref nd))
			{
				return nd;
			}
			obj = null;
			if (!_isNullable)
			{
				obj = default(T);
			}
			return (T)obj;
			// ReSharper restore ExpressionIsAlwaysNull
		}

		/// <summary>
		/// Enqueue a list of values
		/// </summary>
		/// <param name="toInsert"></param>
		public void Enqueue(List<T> toInsert)
		{
			for (Int32 i = 0; i < toInsert.Count; i++)
			{
				Enqueue(toInsert[i]);
			}
		}

		public void Clear()
		{
			var cleanUp = new List<T>(Dequeue());
			cleanUp.Clear();
		}
	}
}