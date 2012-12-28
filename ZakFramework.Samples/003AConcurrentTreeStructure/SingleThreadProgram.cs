using System;
using System.Threading;
using _003AConcurrentTreeStructure.Lib;
using _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals;

namespace _003AConcurrentTreeStructure
{
	static internal class SingleThreadProgram
	{
		internal static void SingleThreadAccess()
		{
			var tree = new ConcurrentTree<string>();

			var root = tree.Root;
			root.AddChild(tree.NewTreeNode("0", "testContent"));

			var node = tree.FindByPath("");
			Console.WriteLine("Founded '{0}'", node.Name);

			node = tree.FindByPath(String.Format("{0}{1}", TreeNode.PathSeparator, "0"));
			Thread.Sleep(1);
			Console.WriteLine("Founded {0}", node.Name);

			node.AddChild(tree.NewTreeNode("0", "testContent"));
			node.AddChild(tree.NewTreeNode("1", "testContent"));
			node.AddChild(tree.NewTreeNode("2", "testContent"));

			node = tree.FindByPath(String.Format("{0}{1}{0}{2}", TreeNode.PathSeparator, "0", "2"));
			Console.WriteLine("Founded {0}", node.Name);

			node.AddChild(tree.NewTreeNode("0", "testContent"));

			node = tree.FindByPath(String.Format("{0}{1}{0}{2}{0}{3}", TreeNode.PathSeparator, "0", "2", "0"));
			Console.WriteLine("Founded {0}", node.Name);


			tree.Dispose();
		}
	}
}