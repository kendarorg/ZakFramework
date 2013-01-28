/*
using System;
using System.Collections.Generic;
using System.Threading;

namespace ZakCore.Utils.Collections
{
	public class MultiLockFreeQueue<T> : IQueue<T>
	{
		private const int TEMP_QUEUE = 2;
		private long _enqueueQueue;
		protected int EnqueueQueue { get { return (int)Interlocked.Read(ref _enqueueQueue); } private set { Interlocked.Exchange(ref _enqueueQueue, value); } }
		protected int DequeueQueue { get { return EnqueueQueue == 0 ? 1 : 0; } }

		private readonly LockFreeQueue<T>[] _queues;

		public long Count { get { return _queues[EnqueueQueue].Count + _queues[DequeueQueue].Count; } }

		public bool Dequeue(ref T t)
		{
			return _queues[DequeueQueue].Dequeue(ref t);
		}

		public void Enqueue(T t)
		{
			var markedForSwap = MarkedForSwap;
			int enqueueQueue = markedForSwap ? TEMP_QUEUE : EnqueueQueue;
			_queues[enqueueQueue].Enqueue(t);
			if (markedForSwap)
			{
				AcceptedForSwap = true;
				EnqueueQueue = DequeueQueue;
			}
		}

		public IEnumerable<T> Dequeue(long count = Int64.MaxValue)
		{
			var markedForSwap = MarkedForSwap;
			int enqueueQueue = markedForSwap ? TEMP_QUEUE : EnqueueQueue;
			return _queues[DequeueQueue].Dequeue(count);
		}

		public T DequeueSingle()
		{
			return _queues[DequeueQueue].DequeueSingle();
		}

		public void Enqueue(List<T> toInsert)
		{
			_queues[EnqueueQueue].Enqueue(toInsert);
		}

		public void Clear()
		{
			_queues[EnqueueQueue].Clear();
			_queues[DequeueQueue].Clear();
		}

		public MultiLockFreeQueue()
		{
			_markedForSwap = 0;
			_enqueueQueue = 0;
			_acceptedForSwap = 0;
			_queues = new LockFreeQueue<T>[3];
			_queues[EnqueueQueue] = new LockFreeQueue<T>();
			_queues[DequeueQueue] = new LockFreeQueue<T>();
			_queues[TEMP_QUEUE] = new LockFreeQueue<T>();
		}


		private long _markedForSwap;
		private long _acceptedForSwap;

		public bool MarkedForSwap
		{
			get { return Interlocked.Read(ref _markedForSwap) == 1; }
			set { Interlocked.Exchange(ref _markedForSwap, value ? 1 : 0); }
		}

		public bool AcceptedForSwap
		{
			get { return Interlocked.Read(ref _acceptedForSwap) == 1; }
			set { Interlocked.Exchange(ref _acceptedForSwap, value ? 1 : 0); }
		}
	}
}
*/