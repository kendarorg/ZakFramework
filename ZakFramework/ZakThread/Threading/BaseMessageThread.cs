using System;
using System.Collections.Generic;
using ZakCore.Utils.Collections;
using ZakCore.Utils.Logging;
using ZakThread.Threading.ThreadManagerInternals;

namespace ZakThread.Threading
{
	public abstract class BaseMessageThread : BaseThread, IBaseMessageThread
	{
		private readonly LockFreeQueue<IMessage> _incomingMessages;
		private readonly LockFreeQueue<IMessage> _outgoingMessages;

		public int MaxMesssagesPerCycle { get; protected set; }

		protected BaseMessageThread(ILogger logger, String threadName, bool restartOnError = true) :
			base(logger, threadName, restartOnError)
		{
			_incomingMessages = new LockFreeQueue<IMessage>();
			_outgoingMessages = new LockFreeQueue<IMessage>();
			MaxMesssagesPerCycle = -1;
		}
		
		protected override bool CyclicExecution()
		{
			if (!RunMessagesPump()) return false;
			return base.CyclicExecution();
		}

		protected virtual bool RunMessagesPump()
		{
			IMessage msg;
			int messagesPerCycle = 0;
			while ((msg = PeekMessage()) != null)
			{
				if (!HandleMessageInternal(msg)) return false;
				messagesPerCycle++;
				if (MaxMesssagesPerCycle > 0 && messagesPerCycle >= MaxMesssagesPerCycle)
				{
					break;
				}
			}
			return true;
		}

		internal virtual bool HandleMessageInternal(IMessage msg)
		{
			return HandleMessage(msg);
		}

		protected abstract bool HandleMessage(IMessage msg);

		protected void SendMessage(IMessage msg)
		{
			if (Manager != null)
			{
				Manager.SendMessageToThread(msg);
			}
			else
			{
				_outgoingMessages.Enqueue(msg);
			}
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
			if (Manager != null) throw new NotSupportedException();
			return _outgoingMessages.DequeueSingle();
		}

		public IEnumerable<IMessage> PeekMessagesFromThread()
		{
			if (Manager != null) throw new NotSupportedException();
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

		protected void RegisterMessage(Type messageTypeToRegister)
		{
			if (Manager!=null)
			{
				Manager.SendMessageToThread(new InternalMessage(InternalMessageTypes.RegisterMessageType,
																							messageTypeToRegister){SourceThread = ThreadName});
			}
		}
	}
}