using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ZakTestUtils
{
	public static class TestFileUtils
	{
		public static void RemoveRootDir(string path)
		{
			var codeBase = Assembly.GetExecutingAssembly().CodeBase;
			codeBase = Path.GetDirectoryName((new Uri(codeBase)).LocalPath);
			if (codeBase != null)
			{
				var realPath = SanitizePath(Path.Combine(codeBase, path));
				if (Directory.Exists(realPath))
				{

					var rootDirInfo = new DirectoryInfo(realPath);
					RemoveDirAndContent(rootDirInfo);
				}
			}
		}

		public static void RemoveDir(string path)
		{
			var realPath = SanitizePath(path);
			if (Directory.Exists(realPath))
			{
				var rootDirInfo = new DirectoryInfo(realPath);
				RemoveDirAndContent(rootDirInfo);
			}
		}

		private static void RemoveDirAndContent(DirectoryInfo rootDirInfo)
		{
			DirectoryInfo[] dirs = rootDirInfo.GetDirectories("*", SearchOption.AllDirectories);
			foreach (FileInfo file in rootDirInfo.GetFiles())
			{
				file.Delete();
			}
			foreach (var dir in dirs)
			{
				RemoveDirAndContent(dir);
			}
			Directory.Delete(rootDirInfo.FullName);
		}

		public static string CreateRootDir(string path)
		{
			var codeBase = Assembly.GetExecutingAssembly().CodeBase;
			codeBase = Path.GetDirectoryName((new Uri(codeBase)).LocalPath);
			string realPath = null;
			if (codeBase != null)
			{
				realPath = SanitizePath(Path.Combine(codeBase, path));
				if (Directory.Exists(realPath))
				{
					var rootDirInfo = new DirectoryInfo(realPath);
					RemoveDirAndContent(rootDirInfo);
				}
				if (!Directory.Exists(realPath))
				{
					Directory.CreateDirectory(realPath);
				}
			}
			return realPath;
		}

		public static string SanitizePath(string path)
		{
			return path.Replace("\\", "/").
				Replace("file:///", "").
				Replace("/", Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture));
		}

		public static string GetSolutionRootWhileTesting()
		{
			var codeBase = Assembly.GetExecutingAssembly().CodeBase;
			codeBase = Path.GetDirectoryName((new Uri(codeBase)).LocalPath);
			codeBase = SanitizePath(codeBase);

			var exploded = codeBase.Split(Path.DirectorySeparatorChar);
			var testResultIndex = codeBase.IndexOf(string.Format("{0}TestResults{0}", Path.DirectorySeparatorChar),
																						 StringComparison.InvariantCultureIgnoreCase);
			if (testResultIndex > 0)
			{
				//If it's a test result dir
				codeBase = codeBase.Substring(0, testResultIndex).TrimEnd(Path.DirectorySeparatorChar);
				exploded = codeBase.Split(Path.DirectorySeparatorChar);
			}
			else if (exploded[exploded.Length - 1] == "Debug" || exploded[exploded.Length - 1] == "Release")
			{
				//If it is inside the project
				// Remove the [projectName]/bin/[Debug|Release]
				exploded = exploded.Take(exploded.Length - 3).ToArray();
			}

			return string.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), exploded);
		}

		public static string CreateDir(string testDir)
		{
			var exploded = testDir.Split(Path.DirectorySeparatorChar);
			int pathIndex = 1;
			var path = exploded[0] + Path.DirectorySeparatorChar;
			while (pathIndex < exploded.Length)
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				path = Path.Combine(path, exploded[pathIndex]);
				pathIndex++;
			}
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}

		public static void CopyAll(string source, string dest)
		{
			source = SanitizePath(source);
			dest = SanitizePath(dest);
			CopyAllInternal(source, dest);
		}

		private static void CopyAllInternal(string source, string dest)
		{
			if (!Directory.Exists(dest))
			{
				CreateDir(dest);
			}
			var sourceDirInfo = new DirectoryInfo(source);
			DirectoryInfo[] dirs = sourceDirInfo.GetDirectories("*", SearchOption.AllDirectories);
			foreach (FileInfo file in sourceDirInfo.GetFiles())
			{
				File.Copy(file.FullName, Path.Combine(dest, file.Name));
			}
			foreach (var dir in dirs)
			{
				CopyAllInternal(dir.FullName, Path.Combine(dest, dir.Name));
			}
		}
	}
}
