
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using _003AConcurrentTreeStructure.Lib;
using _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals;

namespace _003AConcurrentTreeStructure
{
	class Program
	{
		public class ThreadParameter
		{
			public ConcurrentTree<string> Tree;
			public string RootPath;

			public DirectoryInfo Info;

			public int Id;

			public Thread[] Threads;
			public long[] MsElapsed;
		}

		static void Main(string[] args)
		{
			const string templatePath = @"C:\Tmp";
			SingleThreadAccess();
			MultiThreadAccess(templatePath,50);
			
		}

		private static void SingleThreadAccess()
		{
			var tree = new ConcurrentTree<string>();

			var root = tree.Root;
			root.AddChild(tree.NewTreeNode("0", "testContent"));

			var node = tree.FindByPath("");
			Console.WriteLine("Founded '{0}'", node.Name);

			node = tree.FindByPath(string.Format("{0}{1}", TreeNode.PathSeparator, "0"));
			Thread.Sleep(1);
			Console.WriteLine("Founded {0}", node.Name);

			node.AddChild(tree.NewTreeNode("0", "testContent"));
			node.AddChild(tree.NewTreeNode("1", "testContent"));
			node.AddChild(tree.NewTreeNode("2", "testContent"));

			node = tree.FindByPath(string.Format("{0}{1}{0}{2}", TreeNode.PathSeparator, "0", "2"));
			Console.WriteLine("Founded {0}", node.Name);

			node.AddChild(tree.NewTreeNode("0", "testContent"));

			node = tree.FindByPath(string.Format("{0}{1}{0}{2}{0}{3}", TreeNode.PathSeparator, "0", "2", "0"));
			Console.WriteLine("Founded {0}", node.Name);


			tree.Dispose();
		}

		private static long _counter;

		private static void MultiThreadAccess(string templatePath,int threadsCount)
		{
			if (!Directory.Exists(templatePath))
			{
				Console.WriteLine(templatePath + " does not exist, please choose an existing directory!");
			}

			_counter = 0;
			var tree = new ConcurrentTree<string>();
			var rootDir = new DirectoryInfo(templatePath);
			DirectoryInfo[] dirs = rootDir.GetDirectories("*", SearchOption.AllDirectories);
			threadsCount = Math.Min(dirs.Length, threadsCount);

			var threads = new Thread[Math.Min(dirs.Length, threadsCount)];
			var msElapsed = new long[Math.Min(dirs.Length, threadsCount)];

			for (int i = 0; i < threadsCount; i++)
			{
				tree.Root.AddChild(tree.NewTreeNode(dirs[i].Name, string.Empty));
				threads[i] = new Thread(ReplicateDir);
			}
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < threadsCount; i++)
			{
				threads[i].Start(new ThreadParameter
					{
						Info = dirs[i],
						RootPath = TreeNode.PathSeparator + dirs[i].Name,
						Tree = tree,
						Id = i,
						Threads = threads,
						MsElapsed = msElapsed
					});
			}

			long msTotal = 0;
			long elements = 0;
			bool continueAll = true;
			while (continueAll)
			{
				msTotal = 0;
				for (int i = 0; i < threadsCount; i++)
				{
					var elap = Interlocked.Read(ref msElapsed[i]);
					msTotal += elap;
				}
			 elements = Interlocked.Read(ref _counter);
				Console.WriteLine("Elaborated {0} files/directories in {1} ms. Average latency: {2} ms.",
					elements,
					sw.ElapsedMilliseconds,
					elements>0? (int)(msTotal / elements):0);

				Thread.Sleep(1000);
				continueAll = false;
				foreach (var th in threads)
				{
					if (th != null) continueAll = true;
				}
			}
			sw.Stop();

			msTotal = 0;
			elements = Interlocked.Read(ref _counter);
			for (int i = 0; i < threadsCount; i++)
			{
				msTotal += Interlocked.Read(ref msElapsed[i]);
			}
			Console.WriteLine("Elaborated {0} files/directories in {1} ms. Average latency: {2} ms.",
				elements,
				sw.ElapsedMilliseconds,
				elements > 0 ? (int)(msTotal / elements) : 0);
			Console.WriteLine("Done!");
		}

		private static void ReplicateDir(object obj)
		{
			var tp = (ThreadParameter)obj;
			var rootDir = tp.Info;
			var stopWatch = new Stopwatch();

			stopWatch.Start();
			var nodeRoot = tp.Tree.FindByPath(tp.RootPath);
			stopWatch.Stop();
			Interlocked.Add(ref tp.MsElapsed[tp.Id], stopWatch.ElapsedMilliseconds);

			DirectoryInfo[] dirs = rootDir.GetDirectories("*", SearchOption.AllDirectories);

			foreach (DirectoryInfo dir in dirs)
			{
				var newTp = new ThreadParameter
					{
						Info = dir,
						RootPath = tp.RootPath + TreeNode.PathSeparator + dir.Name,
						Tree = tp.Tree,
						MsElapsed = tp.MsElapsed,
						Id = tp.Id
					};
				stopWatch.Restart();
				nodeRoot.AddChild(tp.Tree.NewTreeNode(dir.Name, string.Empty));
				stopWatch.Stop();
				Interlocked.Increment(ref _counter);
				FileInfo[] files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);
				stopWatch.Start();
				foreach (var file in files)
				{

					nodeRoot.AddChild(tp.Tree.NewTreeNode(file.Name, "fileContent"));
					Interlocked.Increment(ref _counter);
				}
				stopWatch.Stop();
				Interlocked.Add(ref tp.MsElapsed[tp.Id], stopWatch.ElapsedMilliseconds);
				ReplicateDir(newTp);
			}
			if (tp.Threads != null)
			{
				tp.Threads[tp.Id] = null;
			}
		}
	}
}
