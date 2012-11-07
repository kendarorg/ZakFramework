using System;
using System.Collections.Generic;
using ZakCore.Utils.Collections;
using ZakCore.Utils.Logging;

namespace ZakThread.Threading
{
	public abstract class BaseMessageThread : BaseThread, IBaseMessageThread
	{
		private readonly LockFreeQueue<IMessage> _incomingMessages;
		private readonly LockFreeQueue<IMessage> _outgoingMessages;

		protected BaseMessageThread(ILogger logger, String threadName, bool restartOnError = true) :
			base(logger, threadName, restartOnError)
		{
			_incomingMessages = new LockFreeQueue<IMessage>();
			_outgoingMessages = new LockFreeQueue<IMessage>();
		}

		protected override bool CyclicExecution()
		{
			foreach (var msg in _incomingMessages.Dequeue())
			{
				if (!HandleMessage(msg)) return false;
			}
			return base.CyclicExecution();
		}

		internal virtual bool HandleMessageInternal(IMessage msg)
		{
			return HandleMessage(msg);
		}

		protected abstract bool HandleMessage(IMessage msg);

		protected void SendMessage(IMessage msg)
		{
			_outgoingMessages.Enqueue(msg);
		}

		public void SendMessageToThread(IMessage msg)
		{
			_incomingMessages.Enqueue(msg);
		}

		protected IMessage PeekMessage()
		{
			return _incomingMessages.DequeueSingle();
		}

		public IMessage PeekMessageFromThread()
		{
			return _outgoingMessages.DequeueSingle();
		}

		public IEnumerable<IMessage> PeekMessagesFromThread()
		{
			return _outgoingMessages.Dequeue();
		}

		private IThreadManager _threadManager;

		public IThreadManager Manager
		{
			get { return _threadManager; }
			set { if (_threadManager == null) _threadManager = value; }
		}

		public abstract void RegisterMessages();

		public override void Dispose()
		{
			base.Dispose();
			_threadManager = null;
			_outgoingMessages.Clear();
			_incomingMessages.Clear();
		}
	}
}