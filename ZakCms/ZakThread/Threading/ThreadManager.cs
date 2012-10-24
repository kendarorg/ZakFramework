using System;
using System.Collections.Generic;
using ZakCore.Utils.Logging;
using ZakThread.Threading.Enums;
using ZakThread.Threading.ThreadManagerInternals;

namespace ZakThread.Threading
{
	public sealed class ThreadManager : BaseMessageThread, IThreadManager
	{
		private readonly Dictionary<string, IBaseMessageThread> _runningThreads;

		public ThreadManager(ILogger logger) :
			this(logger, "ThreadManager")
		{
			_runningThreads = new Dictionary<string, IBaseMessageThread>();
		}

		public ThreadManager(ILogger logger,string threadName) :
			base(logger, threadName, true)
		{
			_runningThreads = new Dictionary<string, IBaseMessageThread>();
		}

		protected override bool RunSingleCycle()
		{
			var retrievedMessages = PeekAllMessages();
			ElaborateAllMessages(retrievedMessages);
			return true;
		}

		private List<IMessage> PeekAllMessages()
		{
			var retrievedMessages = new List<IMessage>();
			foreach (var thread in _runningThreads.Values)
			{
				foreach (var msg in thread.PeekMessagesFromThread())
				{
					if (msg is InternalMessage)
					{
						SendMessage(msg);
					}
					else
					{
						retrievedMessages.Add(msg);
					}
				}
			}
			return retrievedMessages;
		}

		public void AddThread(IBaseMessageThread messageThread)
		{
			SendMessageToThread(new InternalMessage(InternalMessageTypes.AddThread,messageThread));
		}

		public void RemoveThread(IBaseMessageThread messageThread,bool forceHalt = false)
		{
			SendMessageToThread(new InternalMessage(InternalMessageTypes.RemoveThread, new RemoveThreadContent(messageThread, forceHalt)));
		}

		public void RemoveThread(string messageThreadName, bool forceHalt = false)
		{
			SendMessageToThread(new InternalMessage(InternalMessageTypes.RemoveThread, new RemoveThreadContent(messageThreadName, forceHalt)));
		}

		public override void Terminate(bool force = false)
		{
			SendMessageToThread(new InternalMessage(InternalMessageTypes.Terminate, force));
		}

		public void RunThread(string messageThreadName)
		{
			SendMessageToThread(new InternalMessage(InternalMessageTypes.RunThread, messageThreadName));
		}

		protected override bool HandleMessage(IMessage msg)
		{
			var internalMessage = msg as InternalMessage;
			if(internalMessage==null) return true;
			switch(internalMessage.MessageType)
			{
				case (InternalMessageTypes.AddThread):
					HandleAddThread(internalMessage);
					break;
				case (InternalMessageTypes.RemoveThread):
					HandleRemoveThread(internalMessage);
					break;
				case (InternalMessageTypes.Terminate):
					HandleTerminate(internalMessage);
					break;
				case (InternalMessageTypes.RunThread):
					HandleRunThread(internalMessage);
					break;
			}
			return true;
		}

		private void HandleRunThread(InternalMessage internalMessage)
		{
			var threadName = internalMessage.Content as string;
			if (threadName != null)
			{
				if (_runningThreads.ContainsKey(threadName))
				{
					var thread = _runningThreads[threadName];
					thread.RunThread();
				}
			}
		}

		private void HandleTerminate(InternalMessage internalMessage)
		{
			var force = (bool)internalMessage.Content;
			foreach (var thread in _runningThreads.Values)
			{
				_runningThreads[thread.ThreadName] = null;
				thread.Terminate(force);
			}
			Terminate(force);
		}

		private void HandleRemoveThread(InternalMessage internalMessage)
		{
			var th = internalMessage.Content as RemoveThreadContent;
			if (th != null)
			{
				if (_runningThreads.ContainsKey(th.ThreadName))
				{
					var thread = _runningThreads[th.ThreadName];
					_runningThreads[th.ThreadName] = null;
					_runningThreads.Remove(th.ThreadName);
					thread.Terminate(th.ForceHalt);
				}
			}
		}

		private void HandleAddThread(InternalMessage internalMessage)
		{
			var th = internalMessage.Content as IBaseMessageThread;
			if (th != null)
			{
				if (!_runningThreads.ContainsKey(th.ThreadName))
				{
					th.Manager = this;
					th.RegisterMessages();
					_runningThreads[th.ThreadName] = th;
					th.RunThread();
				}
			}
		}

		private void ElaborateAllMessages(List<IMessage> retrievedMessages)
		{
			if (retrievedMessages.Count == 0) return;

			foreach (var thread in _runningThreads.Values)
			{
				if (thread.Status == RunningStatus.Running)
				{
					throw new NotImplementedException();
				}
			}
		}

		public override void Dispose()
		{
			foreach (var thread in _runningThreads.Values)
			{
				_runningThreads[thread.ThreadName] = null;
				thread.Terminate(true);
			}
			base.Dispose();
			_runningThreads.Clear();
		}

		public override void RegisterMessages()
		{
			
		}
	}
}
