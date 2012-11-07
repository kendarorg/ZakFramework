using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using _003AConcurrentTreeStructure.Lib;
using _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals;

namespace _003AConcurrentTreeStructure
{
	internal class Program
	{
		const string STARTING_PATH = @"D:\Development";
		/// <summary>
		/// This will set the directory that will be scanned
		/// </summary>
		public class ThreadParameter
		{
			/// <summary>
			/// The concurrent tree instance
			/// </summary>
			public ConcurrentTree<string> Tree;

			/// <summary>
			/// The starting path for this tree
			/// </summary>
			public string RootPath;

			/// <summary>
			/// The current directory info
			/// </summary>
			public DirectoryInfo Info;

			public int Id;

			/// <summary>
			/// Shared list of threads
			/// </summary>
			public Thread[] Threads;

			/// <summary>
			/// The elapsed times
			/// </summary>
			public long[] TicksElapsed;
		}

		// ReSharper disable UnusedParameter.Local
		private static void Main(string[] args)
		// ReSharper restore UnusedParameter.Local
		{
			
			SingleThreadAccess();
			MultiThreadAccess(STARTING_PATH, 50);
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

		private static void MultiThreadAccess(string templatePath, int threadsCount)
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
			var ticksElapsed = new long[Math.Min(dirs.Length, threadsCount)];

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
						TicksElapsed = ticksElapsed
					});
			}

			long ticksTotal;
			long elements;
			bool continueAll = true;
			while (continueAll)
			{
				ticksTotal = 0;
				for (int i = 0; i < threadsCount; i++)
				{
					var elap = Interlocked.Read(ref ticksElapsed[i]);
					ticksTotal += elap;
				}
				elements = Interlocked.Read(ref _counter);
				Console.WriteLine("Elaborated {0} files/directories in {1} ms. Total latency: {2} ms.",
													elements,
													sw.ElapsedMilliseconds,
													elements > 0 ? (int)( (ticksTotal / threadsCount)/TimeSpan.TicksPerMillisecond) : 0);

				Thread.Sleep(1000);
				continueAll = false;
				foreach (var th in threads)
				{
					if (th != null) continueAll = true;
				}
			}
			sw.Stop();

			ticksTotal = 0;
			elements = Interlocked.Read(ref _counter);
			for (int i = 0; i < threadsCount; i++)
			{
				ticksTotal += Interlocked.Read(ref ticksElapsed[i]);
			}
			Console.WriteLine("Elaborated {0} files/directories in {1} ms. Average latency: {2} ms.",
												elements,
												sw.ElapsedMilliseconds,
												elements > 0 ? (int)((ticksTotal / threadsCount)/TimeSpan.TicksPerMillisecond) : 0);
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
			Interlocked.Add(ref tp.TicksElapsed[tp.Id], stopWatch.ElapsedMilliseconds);

			DirectoryInfo[] dirs = rootDir.GetDirectories("*", SearchOption.AllDirectories);

			foreach (DirectoryInfo dir in dirs)
			{
				var newTp = new ThreadParameter
					{
						Info = dir,
						RootPath = tp.RootPath + TreeNode.PathSeparator + dir.Name,
						Tree = tp.Tree,
						TicksElapsed = tp.TicksElapsed,
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
				Interlocked.Add(ref tp.TicksElapsed[tp.Id], stopWatch.ElapsedTicks);
				ReplicateDir(newTp);
			}
			if (tp.Threads != null)
			{
				tp.Threads[tp.Id] = null;
			}
		}
	}
}