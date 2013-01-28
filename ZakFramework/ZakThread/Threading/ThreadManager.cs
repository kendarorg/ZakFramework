using System;
using System.Collections.Generic;
using ZakCore.Utils.Logging;
using ZakThread.Threading.Enums;
using ZakThread.Threading.ThreadManagerInternals;

namespace ZakThread.Threading
{
	public sealed class ThreadManager : BaseMessageThread, IThreadManager
	{
		private class ThreadDescriptor
		{
			public ThreadDescriptor(IBaseMessageThread thread)
			{
				Thread = thread;
				RegisteredTypes = new Dictionary<Type, bool>();
			}
			public IBaseMessageThread Thread { get; private set; }
			public Dictionary<Type, bool> RegisteredTypes { get; private set; }
		}

		private Dictionary<string, ThreadDescriptor> _runningThreads;

		public ThreadManager(ILogger logger) :
			this(logger, "ThreadManager")
		{
			_runningThreads = new Dictionary<string, ThreadDescriptor>();
			_toElaborate = new List<IMessage>();
		}

		public ThreadManager(ILogger logger, string threadName) :
			base(logger, threadName, true)
		{
			RegisterMessages();
		}

		private readonly List<IMessage> _toElaborate;

		protected override bool RunMessagesPump()
		{
			_toElaborate.Clear();
			IMessage msg;
			while ((msg = PeekMessage()) != null)
			{
				if (msg is InternalMessage)
				{
					if (!HandleMessageInternal((IMessage)msg.Clone())) return false;
				}
				_toElaborate.Add(msg);
			}
			return true;
		}

		protected override bool RunSingleCycle()
		{
			ElaborateAllMessages();
			return true;
		}


		public void AddThread(IBaseMessageThread messageThread)
		{
			SendMessageToThread(new InternalMessage(InternalMessageTypes.AddThread, messageThread));
		}

		public void RemoveThread(IBaseMessageThread messageThread, bool forceHalt = false)
		{
			SendMessageToThread(new InternalMessage(InternalMessageTypes.RemoveThread,
																							new RemoveThreadContent(messageThread, forceHalt)));
		}

		public void RemoveThread(string messageThreadName, bool forceHalt = false)
		{
			SendMessageToThread(new InternalMessage(InternalMessageTypes.RemoveThread,
																							new RemoveThreadContent(messageThreadName, forceHalt)));
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
			if (internalMessage == null) return true;
			switch (internalMessage.MessageType)
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
				case (InternalMessageTypes.RegisterMessageType):
					HandleRegisterMessageType(internalMessage);
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
					thread.Thread.RunThread();
				}
			}
		}

		private bool _terminating;

		private void HandleTerminate(InternalMessage internalMessage)
		{
			if (_terminating) return;
			_terminating = true;
			var force = (bool)internalMessage.Content;
			var thrs = new List<ThreadDescriptor>();
			foreach (var thread in _runningThreads.Values)
			{
				thrs.Add(thread);
			}
			_runningThreads = new Dictionary<string, ThreadDescriptor>();
			foreach (var thread in thrs)
			{
				if (thread != null)
				{
					thread.Thread.Terminate(force);
				}
			}
			_terminating = false;
			base.Terminate(force);
		}

		private void HandleRemoveThread(InternalMessage internalMessage)
		{
			if (_terminating) return;

			var th = internalMessage.Content as RemoveThreadContent;
			if (th != null)
			{
				if (_runningThreads.ContainsKey(th.ThreadName))
				{
					var thread = _runningThreads[th.ThreadName];
					_runningThreads[th.ThreadName] = null;
					/*ThreadDescriptor outThread;
					_runningThreads.TryRemove(th.ThreadName, out outThread);*/
					thread.Thread.Terminate(th.ForceHalt);
					_runningThreads.Remove(th.ThreadName);
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
					_runningThreads[th.ThreadName] = new ThreadDescriptor(th);
					th.RunThread();
					th.RegisterMessages();
				}
			}
		}


		public override void Dispose()
		{
			foreach (var thread in _runningThreads.Values)
			{
				if (thread != null)
				{
					thread.Thread.Terminate(true);
				}
			}
			base.Terminate(true);
			_runningThreads.Clear();
			base.Dispose();
		}

		public override void RegisterMessages()
		{
		}


		private void HandleRegisterMessageType(InternalMessage internalMessage)
		{
			internalMessage.SourceThread = internalMessage.SourceThread.ToUpper();
			if (_runningThreads.ContainsKey(internalMessage.SourceThread))
			{
				ThreadDescriptor td;
				if (internalMessage.Content as Type != null && _runningThreads.TryGetValue(internalMessage.SourceThread, out td))
				{
					if (!td.RegisteredTypes.ContainsKey((Type)internalMessage.Content))
					{
						td.RegisteredTypes.Add((Type)internalMessage.Content, true);
					}
				}
			}
		}

		public bool IsTypeRegistered(string threadName, Type messageType)
		{
			var toret = false;
			if (_runningThreads.ContainsKey(threadName.ToUpper()))
			{
				ThreadDescriptor td;
				if (_runningThreads.TryGetValue(threadName, out td))
				{
					toret = td.RegisteredTypes.ContainsKey(messageType);
				}
			}
			return toret;
		}

		private void ElaborateAllMessages()
		{
			if (_toElaborate.Count == 0) return;

			foreach (var message in _toElaborate)
			{
				foreach (var thread in _runningThreads.Values)
				{
					if (thread.Thread.Status == RunningStatus.Running && thread.RegisteredTypes.ContainsKey(message.GetType()))
					{
						thread.Thread.SendMessageToThread((IMessage)message.Clone());
					}
				}
			}
		}
	}
}