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
			DirectoryInfo[] dirs = rootDirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
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

		public static string GetSolutionRoot()
		{
			var codeBase = Assembly.GetExecutingAssembly().CodeBase;
			codeBase = Path.GetDirectoryName((new Uri(codeBase)).LocalPath);
			codeBase = SanitizePath(codeBase);

			var result1 = Directory.GetFiles(codeBase, "*.sln", SearchOption.TopDirectoryOnly);
			if (result1.Length >= 1)
			{
				return codeBase;
			}
			var exploded = codeBase.Split(Path.DirectorySeparatorChar);

			for (int i = (exploded.Length - 1); i > 0; i--)
			{
				var explodedNew = exploded.Take(i).Skip(1).ToArray();
				string implodedPath = exploded[0] + Path.DirectorySeparatorChar + string.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), explodedNew);
				var result = Directory.GetFiles(implodedPath, "*.sln", SearchOption.TopDirectoryOnly);
				if (result.Length >= 1)
				{
					return implodedPath;
				}
			}
			return null;
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
