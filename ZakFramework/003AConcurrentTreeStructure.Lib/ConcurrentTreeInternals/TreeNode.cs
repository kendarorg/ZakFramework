using System.Collections.Generic;

namespace _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals
{
	public class TreeNode
	{
		public static string PathSeparator = ";";
		public static char PathSeparatorChar = ';';
	}

	public class TreeNode<TContent> : TreeNode
	{
		public string Name { get; internal set; }
		public TreeNode<TContent> Parent { get; internal set; }
		public TContent Content { get; set; }

		private readonly ConcurrentTree<TContent> _container;
		internal List<TreeNode<TContent>> Children { get; private set; }

		internal TreeNode(string name,ConcurrentTree<TContent> container)
		{
			_container = container;
			Name = name;
			Children = new List<TreeNode<TContent>>();
		}

		public List<TreeNode<TContent>> GetChildren()
		{
			return _container.GetChildren(this);
		}

		public void AddChild(TreeNode<TContent> item)
		{
			_container.AddChild(this, item);
		}

		public void RemoveChildren(TreeNode<TContent> item)
		{
			_container.RemoveChild(this, item);
		}

		public void RemoveChildren(string itemName)
		{
			_container.RemoveChild(this, itemName);
		}

		public void Rename(string newName)
		{
			_container.Rename(this, newName);
		}
	}
}
