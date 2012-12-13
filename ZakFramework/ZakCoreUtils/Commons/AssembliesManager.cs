using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ZakCore.Utils.Commons
{
	public class AssembliesManager
	{
		public static Type LoadType(string fullQualifiedName)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int index = 0; index < assemblies.Length; index++)
			{
				var asm = assemblies[index];
				var toret = LoadType(asm, fullQualifiedName);
				if (toret != null) return toret;

			}
			return null;
		}

		public static Type LoadType(Assembly sourceAssembly, string fullQualifiedName)
		{
			return sourceAssembly.GetType(fullQualifiedName, false);
		}

		public static IEnumerable<Type> LoadTypesWithAttribute(params Type[] types)
		{
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (!asm.IsDynamic && !string.IsNullOrEmpty(asm.CodeBase))
				{
					foreach (var type in LoadTypesWithAttribute(asm, types))
					{
						yield return type;
					}
				}
			}
		}

		public static IEnumerable<Type> LoadTypesWithAttribute(Assembly sourceAssembly, params Type[] types)
		{
			var toret = new List<Type>();

			try
			{

				var classTypes = sourceAssembly.GetTypes();
				for (int classTypeIndex = 0; classTypeIndex < classTypes.Length; classTypeIndex++)
				{
					var classType = classTypes[classTypeIndex];
					var customAttributes = classType.GetCustomAttributes(true);
					bool founded = false;
					for (int attributeIndex = 0; attributeIndex < customAttributes.Length && founded == false; attributeIndex++)
					{
						object attribute = customAttributes[attributeIndex];
						for (int attributesTypeIndex = 0; attributesTypeIndex < types.Length; attributesTypeIndex++)
						{
							var type = types[attributesTypeIndex];
							if (attribute.GetType().FullName == type.FullName)
							{
								toret.Add(classType);
								founded = true;
								break;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return toret;
		}

		public static IEnumerable<Type> LoadTypesInheritingFrom(params Type[] types)
		{
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (!asm.IsDynamic && !string.IsNullOrEmpty(asm.CodeBase))
				{
					foreach (var type in LoadTypesInheritingFrom(asm, types))
					{
						yield return type;
					}
				}
			}
		}

		public static IEnumerable<Type> LoadTypesInheritingFrom(Assembly sourceAssembly, params Type[] types)
		{
			var toret = new List<Type>();
			try
			{
				var classTypes = sourceAssembly.GetTypes();
				for (int classTypeIndex = 0; classTypeIndex < classTypes.Length; classTypeIndex++)
				{
					var classType = classTypes[classTypeIndex];
					for (int attributesTypeIndex = 0; attributesTypeIndex < types.Length; attributesTypeIndex++)
					{
						var type = types[attributesTypeIndex];
						if (classType.IsAssignableFrom(type) || classType.IsSubclassOf(type))
						{
							toret.Add(classType);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return toret;
		}

		private static void DumpAssembly(string path, Assembly assm, Dictionary<string, Assembly> alreadyLoaded)
		{
			AssemblyName fullQualifiedName = assm.GetName();
			if (alreadyLoaded.ContainsKey(fullQualifiedName.FullName))
			{
				return;
			}
			alreadyLoaded[fullQualifiedName.FullName] = assm;

			foreach (AssemblyName name in assm.GetReferencedAssemblies())
			{
				if (!alreadyLoaded.ContainsKey(name.FullName))
				{
					var dllFile = GetAssemblyName(name.FullName);
					var matchingFiles = new List<string>(Directory.GetFiles(path, dllFile, SearchOption.AllDirectories));
					if (matchingFiles.Count != 1)
					{
						try
						{
							Assembly.Load(name);
						}
						catch
						{
							throw new FileNotFoundException(string.Format("Dll not found in {0} or subdirectories", path), dllFile);
						}

					}
					else
					{
						Assembly referenced = Assembly.LoadFrom(matchingFiles[0]);
						DumpAssembly(path, referenced, alreadyLoaded);
					}
				}
			}
		}

		private static string GetAssemblyName(string fullName)
		{
			var comma = fullName.IndexOf(",", StringComparison.InvariantCultureIgnoreCase);
			return fullName.Substring(0, comma) + ".dll";
		}

		public static bool LoadAssemblyFrom(string dllFile, List<string> missingDll, params string[] paths)
		{
			foreach (var path in paths)
			{
				try
				{
					LoadAssemblyFrom(path, dllFile);
					return true;
				}
				catch (FileNotFoundException ex)
				{
					if (missingDll != null) missingDll.Add(ex.FileName);
				}
			}
			return false;
		}

		public static bool LoadAssemblyFrom(string path, string dllFile, bool throwOnError = true)
		{
			var alreadyPresentAssemblies = new Dictionary<string, Assembly>();

			foreach (var alreadyPresent in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (!alreadyPresent.IsDynamic && !string.IsNullOrEmpty(alreadyPresent.CodeBase))
				{
					alreadyPresentAssemblies.Add(alreadyPresent.FullName, alreadyPresent);

				}
			}
			var matchingFiles = new List<string>(Directory.GetFiles(path, dllFile, SearchOption.AllDirectories));
			if (matchingFiles.Count != 1)
			{
				if (!throwOnError) return false;
				throw new FileNotFoundException(string.Format("Dll not found in {0} or subdirectories", path), Path.Combine(path, dllFile));
			}
			try
			{
				var sm = Assembly.LoadFrom(matchingFiles[0]);
				DumpAssembly(path, sm, alreadyPresentAssemblies);
				return true;
			}
			catch (Exception)
			{
				if (!throwOnError) return false;
				throw new FileNotFoundException("Dll not found ", matchingFiles[0]);
			}
		}

		public static IEnumerable<Assembly> LoadAssembliesFrom(string path, bool deep = true)
		{
			var result = new List<Assembly>();
			var asmFileList = new List<string>(Directory.GetFiles(path, "*.dll", deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
			var alreadyPresentAssemblies = new Dictionary<string, Assembly>();

			foreach (var alreadyPresent in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (!alreadyPresent.IsDynamic && !string.IsNullOrEmpty(alreadyPresent.CodeBase))
				{
					alreadyPresentAssemblies.Add(alreadyPresent.FullName, alreadyPresent);
				}
			}

			int assemblyLoaded;
			do
			{
				assemblyLoaded = 0;

				for (int i = asmFileList.Count - 1; i > -1; i--)
				{
					try
					{
						var asmPath = asmFileList[i];
						var reflectionOnlyAssembly = Assembly.ReflectionOnlyLoadFrom(asmPath);
						if (!alreadyPresentAssemblies.ContainsKey(reflectionOnlyAssembly.FullName))
						{
							result.Add(Assembly.LoadFrom(asmPath));
						}

						asmFileList.RemoveAt(i);
						assemblyLoaded++;
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex);
					}
				}
			} while (assemblyLoaded > 0 && asmFileList.Count > 0);

			if (asmFileList.Count > 0)
				throw new Exception(string.Concat("Error loading assemblies: ", Environment.NewLine, string.Join(Environment.NewLine, asmFileList)));

			return result;
		}
	}
}
