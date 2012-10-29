using System;
using System.Threading;
using ZakCore.Utils.Collections;

namespace ZakCoreUtils.Test.Collections
{
	public class CollectionElement
	{
		public CollectionElement(Int64 i)
		{
			Value = i;
		}

		public Int64 Value { get; set; }
	}

	public class ZQueueCollectionTestUtils
	{
		private static LockFreeQueue<CollectionElement> _internalQueue;
		private static Int64 _collectedElements;
		private static Int64 _sentElements;
		private static Int64 _toSendElements;

		public static void Initialize(int toSendElements)
		{
			_internalQueue = new LockFreeQueue<CollectionElement>();
			_collectedElements = 0;
			_sentElements = 0;
			_toSendElements = toSendElements;
		}

		public static bool IsSendCompleted
		{
			get { return _toSendElements == Interlocked.Read(ref _collectedElements); }
		}

		public static void ProducerThread()
		{
			while (_toSendElements > Interlocked.Read(ref _sentElements))
			{
				if(Interlocked.Increment(ref _sentElements)<=_toSendElements)
				{
					_internalQueue.Enqueue(new CollectionElement(0));	
				}
			}
		}

		public static void ConsumerThread()
		{
			while(_toSendElements != Interlocked.Read(ref _collectedElements))
			{
#pragma warning disable 168
				foreach(CollectionElement ce in _internalQueue.Dequeue())
#pragma warning restore 168
				{
					Interlocked.Increment(ref _collectedElements);
				}
			}
		}
	}
}
