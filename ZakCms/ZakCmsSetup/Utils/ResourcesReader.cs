using System;
using System.IO;
using System.Reflection;

namespace ZakCmsSetup.Utils
{
	public static class ResourcesReader
	{
		public static String LoadManifestResourceString(string fileName, object instance)
		{
			var stream = LoadManifestResourceStream(fileName, instance);
			if (stream != null)
			{
				stream.Seek(0, SeekOrigin.Begin);
				var reader = new StreamReader(stream);
				var fileContent = reader.ReadToEnd();
				stream.Close();
				return fileContent;
			}
			return null;
		}

		public static Stream LoadManifestResourceStream(string fileName, object instance)
		{
			var ass = instance as Assembly;
			if (ass == null)
			{
				ass = instance.GetType().Assembly;
			}
			string[] names = ass.GetManifestResourceNames();
			foreach (var name in names)
			{
				if (name.EndsWith(fileName, StringComparison.InvariantCultureIgnoreCase))
				{
					return ass.GetManifestResourceStream(name);
				}
			}
			return null;
		}
	}
}