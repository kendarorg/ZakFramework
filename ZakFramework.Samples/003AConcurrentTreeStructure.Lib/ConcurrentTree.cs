using System;
using System.Collections.Generic;
using System.Threading;
using ZakCore.Utils.Logging;
using ZakThread.HighPower;
using ZakThread.HighPower.Bases;
using ZakThread.Logging;
using _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals;

namespace _003AConcurrentTreeStructure.Lib
{
	/// <summary>
	/// This will act as the main container
	/// </summary>
	/// <typeparam name="TContent"></typeparam>
	public class ConcurrentTree<TContent> : BaseAsyncHandler, IDisposable
	{
		public TreeNode<TContent> Root { get; private set; }

		/// <summary>
		/// The object that will manage the requests queue and will re-forward them to this class
		/// </summary>
		private readonly TreeCollectionExecutor<TContent> _executor;

		public ConcurrentTree()
		{
			Root = new TreeNode<TContent>(string.Empty, this);
			//Setup the executor
			_executor = new TreeCollectionExecutor<TContent>(NullLogger.Create(), "CollectionExecutor", this);
			//And run it!
			_executor.RunThread();
			// Leave the time to the system to do start
			Thread.Sleep(500);
		}

		/// <summary>
		/// Just to have a small factory for the tree nodes and avoiding exposing constructors
		/// </summary>
		/// <param name="name"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		public TreeNode<TContent> NewTreeNode(string name, TContent content = default(TContent))
		{
			var treeNode = new TreeNode<TContent>(name, this) {Content = content};
			return treeNode;
		}

		#region BaseAsyncHandler implementation

		public void Dispose()
		{
			_executor.Terminate(true);
		}

		/// <summary>
		/// This action will be executed asynchronously in respect of the caller thread but synchronously 
		/// inside the executor
		/// </summary>
		/// <param name="asyncTask"></param>
		/// <returns></returns>
		public override bool ExecuteAsyncProcessing(AsyncTask asyncTask)
		{
			var msg = (ConcurrentTreeMessage) asyncTask.Tag;
			if (msg == null) return true;
			asyncTask.Result = ExecuteOperation(msg);
			return false;
		}

		/// <summary>
		/// We will not use this, should see in the future what we could do with this!
		/// </summary>
		/// <param name="atl"></param>
		public override void FinalizeBatchElements(List<AsyncTask> atl)
		{
		}

		#endregion

		#region Called by nodes for asynch operations

		public TreeNode<TContent> FindByPath(string path)
		{
			// This can be handled directly
			if (path == string.Empty)
			{
				return Root;
			}

			var msg = new ConcurrentTreeMessage(ConcurrentTreeMessageTypes.MsgFindByPath,
			                                    path);

			var result = StartTask(msg);
			return (TreeNode<TContent>) result.Result;
		}

		internal List<TreeNode<TContent>> GetChildren(TreeNode<TContent> treeNode)
		{
			var msg = new ConcurrentTreeMessage(ConcurrentTreeMessageTypes.MsgGetChildren,
			                                    treeNode);

			var result = StartTask(msg);
			return (List<TreeNode<TContent>>) result.Result;
		}

		internal bool Rename(TreeNode<TContent> treeNode, string newName)
		{
			var msg = new ConcurrentTreeMessage(ConcurrentTreeMessageTypes.MsgRename,
			                                    newName);

			var result = StartTask(msg);
			return result.Success;
		}

		internal bool AddChild(TreeNode<TContent> treeNode, TreeNode<TContent> item)
		{
			var msg = new ConcurrentTreeMessage(ConcurrentTreeMessageTypes.MsgAddChild,
			                                    treeNode, item);

			var result = StartTask(msg);
			return result.Success;
		}

		internal void RemoveChild(TreeNode<TContent> treeNode, TreeNode<TContent> item)
		{
			var msg = new ConcurrentTreeMessage(ConcurrentTreeMessageTypes.MsgRemoveChild,
			                                    treeNode, item);

			_executor.SendMessageToThread(msg);
		}

		internal void RemoveChild(TreeNode<TContent> treeNode, string itemName)
		{
			var msg = new ConcurrentTreeMessage(ConcurrentTreeMessageTypes.MsgRemoveChildByName,
			                                    treeNode, itemName);

			_executor.SendMessageToThread(msg);
		}

		/// <summary>
		/// Utility function to start the tasks from message
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		private ConcurrentTreeMessageResult StartTask(ConcurrentTreeMessage msg)
		{
			// Create the task to execute, based on the message
			var asop = (AsyncTask) RunAsyncOperation(null, msg, null);
			// Enqueue it on the executor
			_executor.EnqueTask(asop);
			// Wait for completion
			asop.AsyncWaitHandle.WaitOne(5000, true);

			return (ConcurrentTreeMessageResult) asop.Result;
		}

		#endregion

		#region Specific operations in single thread

		/// <summary>
		/// This as the only aim of insulating a bit the -real- execution of all stuffs
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		internal object ExecuteOperation(ConcurrentTreeMessage msg)
		{
			object result = null;
			switch (msg.MessageType)
			{
				case (ConcurrentTreeMessageTypes.MsgAddChild):
					//internal void AddChildren(TreeNode<TContent> treeNode, TreeNode<TContent> item)
					result = ExecuteAddChild(
						(TreeNode<TContent>) msg.Parameters[0],
						(TreeNode<TContent>) msg.Parameters[1]);
					break;
				case (ConcurrentTreeMessageTypes.MsgFindByPath):
					//internal TreeNode<TContent> FindByPath(string path)
					result = ExecuteFindByPath(
						(string) msg.Parameters[0]);
					break;
				case (ConcurrentTreeMessageTypes.MsgGetChildren):
					//internal List<TreeNode<TContent>> GetChildren(TreeNode<TContent> treeNode)
					result = ExecuteGetChildren(
						(TreeNode<TContent>) msg.Parameters[0]);
					break;
				case (ConcurrentTreeMessageTypes.MsgRemoveChild):
					//internal void RemoveChild(TreeNode<TContent> treeNode, string itemName)
					result = ExecuteRemoveChild(
						(TreeNode<TContent>) msg.Parameters[0],
						(string) msg.Parameters[1]);
					break;
				case (ConcurrentTreeMessageTypes.MsgRemoveChildByName):
					//internal void RemoveChild(TreeNode<TContent> treeNode, TreeNode<TContent> item)
					result = ExecuteRemoveChild(
						(TreeNode<TContent>) msg.Parameters[0],
						(TreeNode<TContent>) msg.Parameters[1]);
					break;
				case (ConcurrentTreeMessageTypes.MsgRename):
					//internal void Rename(TreeNode<TContent> treeNode, string newName)
					result = ExecuteRename(
						(TreeNode<TContent>) msg.Parameters[0],
						(string) msg.Parameters[1]);
					break;
			}
			return result;
		}

		private ConcurrentTreeMessageResult ExecuteRename(TreeNode<TContent> treeNode, string newName)
		{
			var result = new ConcurrentTreeMessageResult();
			treeNode.Name = newName;
			result.Success = true;
			return result;
		}

		private ConcurrentTreeMessageResult ExecuteRemoveChild(TreeNode<TContent> parent, TreeNode<TContent> toRemove)
		{
			var result = new ConcurrentTreeMessageResult();
			toRemove.Parent = null;
			parent.Children.Remove(toRemove);
			result.Success = true;
			return result;
		}

		private ConcurrentTreeMessageResult ExecuteRemoveChild(TreeNode<TContent> parent, string nameToRemove)
		{
			var result = new ConcurrentTreeMessageResult();
			for (int i = 0; i < parent.Children.Count; i++)
			{
				var item = parent.Children[i];
				if (item.Name == nameToRemove)
				{
					item.Parent = null;
					parent.Children.RemoveAt(i);
					result.Success = true;
					return result;
				}
			}
			return result;
		}

		private ConcurrentTreeMessageResult ExecuteGetChildren(TreeNode<TContent> parent)
		{
			var result = new ConcurrentTreeMessageResult {Result = new List<TreeNode<TContent>>(parent.Children), Success = true};
			return result;
		}

		private ConcurrentTreeMessageResult ExecuteAddChild(TreeNode<TContent> parent, TreeNode<TContent> newChild)
		{
			var result = new ConcurrentTreeMessageResult();
			for (int i = 0; i < parent.Children.Count; i++)
			{
				var item = parent.Children[i];
				if (item.Name == newChild.Name)
				{
					return result;
				}
			}
			parent.Children.Add(newChild);
			newChild.Parent = parent;
			result.Success = true;
			return result;
		}

		private ConcurrentTreeMessageResult ExecuteFindByPath(string path)
		{
			var result = new ConcurrentTreeMessageResult();

			var pathExploded = path.Split(TreeNode.PathSeparatorChar);
			result.Result = FindByPathInternal(Root, pathExploded, 1);
			result.Success = result.Result != null;
			return result;
		}

		private object FindByPathInternal(TreeNode<TContent> node, string[] pathExploded, int level)
		{
			if (level >= pathExploded.Length) return null;
			for (int i = 0; i < node.Children.Count; i++)
			{
				var subNode = node.Children[i];
				if (subNode.Name == pathExploded[level])
				{
					if (level == (pathExploded.Length - 1)) return subNode;
					level++;
					return FindByPathInternal(subNode, pathExploded, level);
				}
			}
			return null;
		}

		#endregion
	}
}