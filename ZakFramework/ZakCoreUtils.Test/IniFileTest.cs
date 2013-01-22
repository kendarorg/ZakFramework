using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ZakCore.Utils.Commons;
using ZakTestUtils;

namespace ZakCoreUtils.Test
{
	/// <summary>
	///This is a test class for IniFileTest and is intended
	///to contain all IniFileTest Unit Tests
	///</summary>
	[TestFixture]
	public class IniFileTest
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes

		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//

		#endregion

		[Test]
		public void ItShouldBePosssibleToSetMultipleValues()
		{
			var iniFile = new IniFile(null);
			var datas = new Dictionary<string, object>
				{
					{"A","AVAL"},
					{"B","BVAL"},
					{"C","CVAL"}
				};
			iniFile.SetValues(datas);
			Assert.AreEqual("AVAL", iniFile.GetValueString("A"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b"));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C"));
		}

		[Test]
		public void ItShouldBePosssibleToSetMultipleValuesOnRootWithoutCaringAboutSectionCase()
		{
			var iniFile = new IniFile(null);
			var datas = new Dictionary<string, object>
				{
					{"A","AVAL"},
					{"B","BVAL"},
					{"C","CVAL"}
				};
			iniFile.SetValues(datas, "ROOT");
			Assert.AreEqual("AVAL", iniFile.GetValueString("A", "Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b", string.Empty));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C", null));
		}


		[Test]
		public void ItShouldBePosssibleToGetAllValuesFromASection()
		{
			var iniFile = new IniFile(null);
			var datas = new Dictionary<string, object>
				{
					{"A","AVAL"},
					{"B","BVAL"},
					{"C","CVAL"}
				};
			iniFile.SetValues(datas, "ROOT");
			var result = iniFile.GetValues();

			Assert.AreEqual(datas["A"], result["A"]);
			Assert.AreEqual(datas["B"], result["B"]);
			Assert.AreEqual(datas["C"], result["C"]);

			result = iniFile.GetValues(null);

			Assert.AreEqual(datas["A"], result["A"]);
			Assert.AreEqual(datas["B"], result["B"]);
			Assert.AreEqual(datas["C"], result["C"]);

			result = iniFile.GetValues(string.Empty);

			Assert.AreEqual(datas["A"], result["A"]);
			Assert.AreEqual(datas["B"], result["B"]);
			Assert.AreEqual(datas["C"], result["C"]);
		}

		[Test]
		public void ItShouldBePosssibleToSetMultipleValuesOnRootWithEmptySection()
		{
			var iniFile = new IniFile(null);
			var datas = new Dictionary<string, object>
				{
					{"A","AVAL"},
					{"B","BVAL"},
					{"C","CVAL"}
				};
			iniFile.SetValues(datas, string.Empty);
			Assert.AreEqual("AVAL", iniFile.GetValueString("A", "Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b", string.Empty));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C", null));
		}


		[Test]
		public void ItShouldBePosssibleToSetMultipleValuesOnRootWithNullSection()
		{
			var iniFile = new IniFile(null);
			var datas = new Dictionary<string, object>
				{
					{"A","AVAL"},
					{"B","BVAL"},
					{"C","CVAL"}
				};
			iniFile.SetValues(datas, null);
			Assert.AreEqual("AVAL", iniFile.GetValueString("A", "Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b", "rooT"));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C"));
		}

		[Test]
		public void ItShouldBePosssibleToSetMultipleValuesOnArbitrarySectionWithoutCaringAboutSectionCase()
		{
			var sectionName = Guid.NewGuid().ToString();
			var iniFile = new IniFile(null);
			var datas = new Dictionary<string, object>
				{
					{"A","AVAL"},
					{"B","BVAL"},
					{"C","CVAL"}
				};
			iniFile.SetValues(datas, sectionName + "ROOT");
			Assert.AreEqual("AVAL", iniFile.GetValueString("A", sectionName + "Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b", sectionName + "rooT"));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C", sectionName + "ROOT"));
		}


		[Test]
		public void ItShouldBePosssibleToGetAllSectionsFromAFile()
		{
			var iniFile = new IniFile(null);
			var datas = new Dictionary<string, object>
				{
					{"A","AVAL"},
					{"B","BVAL"},
					{"C","CVAL"}
				};
			iniFile.SetValues(datas, "ROOT");
			iniFile.SetValues(datas, "fuffa");
			iniFile.SetValues(datas, "test");

			var result = iniFile.GetValues();

			Assert.AreEqual(datas["A"], result["A"]);
			Assert.AreEqual(datas["B"], result["B"]);
			Assert.AreEqual(datas["C"], result["C"]);

			result = iniFile.GetValues("FuFFa");

			Assert.AreEqual(datas["A"], result["A"]);
			Assert.AreEqual(datas["B"], result["B"]);
			Assert.AreEqual(datas["C"], result["C"]);

			result = iniFile.GetValues("test");

			Assert.AreEqual(datas["A"], result["A"]);
			Assert.AreEqual(datas["B"], result["B"]);
			Assert.AreEqual(datas["C"], result["C"]);

			var sections = iniFile.GetSections().ToArray();
			Assert.IsTrue(sections.Any(a => a == "TEST"));
			Assert.IsTrue(sections.Any(a => a == "FUFFA"));
			Assert.IsFalse(sections.Any(a => a == "ROOT"));
		}

		[Test]
		public void ItShouldNotBePosssibleToGetANonExistingValue()
		{
			var iniFile = new IniFile(null);
			var datas = new Dictionary<string, object>
				{
					{"A", "AVAL"},
					{"B", "BVAL"},
					{"C", "CVAL"}
				};
			iniFile.SetValues(datas, "ROOT");
			iniFile.SetValues(datas, "fuffa");

			var result = iniFile.GetValues();

			Assert.AreEqual(datas["A"], result["A"]);
			Assert.AreEqual(datas["B"], result["B"]);
			Assert.AreEqual(datas["C"], result["C"]);

			result = iniFile.GetValues("FuFFa");

			Assert.AreEqual(datas["A"], result["A"]);
			Assert.AreEqual(datas["B"], result["B"]);
			Assert.AreEqual(datas["C"], result["C"]);

			Assert.IsNull(iniFile.GetValue("A","test"));
			Assert.IsNull(iniFile.GetValue("D", "fuffa"));
		}

		[Test]
		public void ItShouldBePosssibleToAddValueToNonExistingSection()
		{
			var iniFile = new IniFile(null);
			var datas = new Dictionary<string, object>
				{
					{"A","AVAL"},
					{"B","BVAL"},
					{"C","CVAL"}
				};
			iniFile.SetValues(datas);
			Assert.AreEqual("AVAL", iniFile.GetValueString("A"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b"));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C"));

			iniFile.SetValue("A","AVAL","test");
			Assert.AreEqual("AVAL", iniFile.GetValueString("A","test"));
		}

		[Test]
		public void ItShouldBePosssibleToAddValueToExistingSection()
		{
			var iniFile = new IniFile(null);
			var datas = new Dictionary<string, object>
				{
					{"A","AVAL"},
					{"B","BVAL"},
					{"C","CVAL"}
				};
			iniFile.SetValues(datas);
			Assert.AreEqual("AVAL", iniFile.GetValueString("A"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b"));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C"));

			iniFile.SetValue("D", "DVAL", "rooT");
			Assert.AreEqual("DVAL", iniFile.GetValueString("D", "roOt"));
		}

		[Test]
		public void ItShouldBePosssibleToAddValueWithBuildSpectificationToExistingSection()
		{
			var iniFile = new IniFile(null,null,true);
			var datas = new Dictionary<string, object>
				{
					{"A","AVAL"},
					{"B","BVAL{build}"},
					{"C","CVAL"}
				};
			iniFile.SetValues(datas);
			iniFile.SetValue("E","EVAL",null);
			Assert.AreEqual("AVAL", iniFile.GetValueString("A"));
			Assert.AreEqual("EVAL", iniFile.GetValueString("E"));
#if DEBUG
			Assert.AreEqual("BVALDebug", iniFile.GetValueString("B", "roOt"));
#else
			Assert.AreEqual("BVALRelease", iniFile.GetValueString("B", "roOt"));
#endif
			Assert.AreEqual("CVAL", iniFile.GetValueString("C"));

			iniFile.SetValue("D", "DVAL{build}", "rooT");
#if DEBUG
			Assert.AreEqual("DVALDebug", iniFile.GetValueString("D", "roOt"));
#else
			Assert.AreEqual("DVALRelease", iniFile.GetValueString("D", "roOt"));
#endif

		}

		[Test]
		public void ItShouldBePosssibleToChangeAVaalue()
		{
			var iniFile = new IniFile(null, null, true);
			var datas = new Dictionary<string, object>
				{
					{"A", "AVAL"},
					{"B", "BVAL{build}"},
					{"C", "CVAL"}
				};
			iniFile.SetValues(datas);
			iniFile.SetValue("B", "EVAL", null);
			Assert.AreEqual("EVAL", iniFile.GetValueString("B", "roOt"));
		}

		[Test]
		public void ItShouldBePosssibleToLoadAFile()
		{
			var projectRoot = Path.Combine(TestFileUtils.GetSolutionRoot(),"ZakCoreUtils.Test");
			var file = Path.Combine(projectRoot, "TestIni.ini");
			
#if DEBUG
			var destFile = Path.Combine(projectRoot, "bin", "Debug", "TestIni.ini");
#else
			var destFile = Path.Combine(projectRoot,"bin","Release", "TestIni.ini");
#endif
			if(File.Exists(destFile)) File.Delete(destFile);
			File.Copy(file,destFile);

			var iniFile = new IniFile(destFile);

			Assert.AreEqual("AVAL", iniFile.GetValueString("A","Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b"));
			Assert.AreEqual(string.Empty, iniFile.GetValueString("z"));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C"));
			Assert.AreEqual("AVALF", iniFile.GetValueString("A","fuffa"));
			Assert.AreEqual("BVALF", iniFile.GetValueString("b", "Fuffa"));
			Assert.AreEqual("CVALF", iniFile.GetValueString("C", "fufFa"));
		}

		[Test]
		public void ItShouldBePosssibleToLoadAFileGivenRootAndFileName()
		{
			var projectRoot = Path.Combine(TestFileUtils.GetSolutionRoot(), "ZakCoreUtils.Test");
			var projectRoot2 = Path.Combine(TestFileUtils.GetSolutionRoot(), "ZakCore.Coverage");
			var file = Path.Combine(projectRoot, "TestIni.ini");

#if DEBUG
			var destRoot = Path.Combine(projectRoot, "bin", "Debug");
			var destFile = Path.Combine(projectRoot, "bin", "Debug", "TestIni.ini");
#else
			var destRoot = Path.Combine(projectRoot, "bin", "Release");
			var destFile = Path.Combine(projectRoot,"bin","Release", "TestIni.ini");
#endif
			var oriDestRoot = destRoot;
			destRoot += Path.DirectorySeparatorChar;
			if (File.Exists(destFile)) File.Delete(destFile);
			File.Copy(file, destFile);

			var iniFile = new IniFile("TestIni.ini",destRoot);
			//Assert.AreEqual(destFile,iniFile.FileName);

			foreach(var item in  iniFile.GetValues("Root"))
				Console.WriteLine(item.Key+"="+item.Value);

			Assert.AreEqual("AVAL", iniFile.GetValueString("A", "Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b"));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C"));
			Assert.AreEqual("AVALF", iniFile.GetValueString("A", "fuffa"));
			Assert.AreEqual("BVALF", iniFile.GetValueString("b", "Fuffa"));
			Assert.AreEqual("CVALF", iniFile.GetValueString("C", "fufFa"));
			
			
			Assert.IsTrue(iniFile.GetValueString("D", "fufFa").StartsWith("DVALF" + oriDestRoot)||
				iniFile.GetValueString("D", "fufFa").StartsWith("DVALF" + projectRoot2));
		}


		[Test]
		public void ItShouldBePosssibleToSaveAFile()
		{
			var projectRoot = Path.Combine(TestFileUtils.GetSolutionRoot(), "ZakCoreUtils.Test");
			var file = Path.Combine(projectRoot, "TestIni.ini");

#if DEBUG
			var destFile = Path.Combine(projectRoot, "bin", "Debug", "TestIni.ini");
#else
			var destFile = Path.Combine(projectRoot,"bin","Release", "TestIni.ini");
#endif
			if (File.Exists(destFile)) File.Delete(destFile);
			File.Copy(file, destFile);

			var iniFile = new IniFile(destFile);

			Assert.AreEqual("AVAL", iniFile.GetValueString("A", "Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b"));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C"));
			Assert.AreEqual("KVAL=3", iniFile.GetValueString("k"));
			Assert.AreEqual("AVALF", iniFile.GetValueString("A", "fuffa"));
			Assert.AreEqual("BVALF", iniFile.GetValueString("b", "Fuffa"));
			Assert.AreEqual("CVALF", iniFile.GetValueString("C", "fufFa"));
			iniFile.SetValue("C","changed");
			iniFile.SetValue("E", "added");

			iniFile.Save(destFile + ".sav");
			Assert.IsTrue(File.Exists(destFile + ".sav"));

			iniFile = new IniFile(destFile + ".sav");
			Assert.AreEqual("AVAL", iniFile.GetValueString("A", "Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b"));
			Assert.AreEqual("changed", iniFile.GetValueString("C"));
			Assert.AreEqual("added", iniFile.GetValueString("e"));
			Assert.AreEqual("AVALF", iniFile.GetValueString("A", "fuffa"));
			Assert.AreEqual("BVALF", iniFile.GetValueString("b", "Fuffa"));
			Assert.AreEqual("CVALF", iniFile.GetValueString("C", "fufFa"));
			Assert.AreEqual("KVAL=3", iniFile.GetValueString("k"));
		}

		[Test]
		public void ItShouldBePosssibleToChangeAFile()
		{
			var projectRoot = Path.Combine(TestFileUtils.GetSolutionRoot(), "ZakCoreUtils.Test");
			var file = Path.Combine(projectRoot, "TestIni.ini");

#if DEBUG
			var destFile = Path.Combine(projectRoot, "bin", "Debug", "TestIni.ini");
#else
			var destFile = Path.Combine(projectRoot,"bin","Release", "TestIni.ini");
#endif
			if (File.Exists(destFile)) File.Delete(destFile);
			File.Copy(file, destFile);

			var iniFile = new IniFile(destFile);

			Assert.AreEqual("AVAL", iniFile.GetValueString("A", "Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b"));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C"));
			Assert.AreEqual("AVALF", iniFile.GetValueString("A", "fuffa"));
			Assert.AreEqual("BVALF", iniFile.GetValueString("b", "Fuffa"));
			Assert.AreEqual("CVALF", iniFile.GetValueString("C", "fufFa"));
			iniFile.SetValue("C", "changed");
			iniFile.SetValue("E", "added");

			iniFile.Save(destFile);
			Assert.IsTrue(File.Exists(destFile));

			iniFile = new IniFile(destFile);
			Assert.AreEqual("AVAL", iniFile.GetValueString("A", "Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b"));
			Assert.AreEqual("changed", iniFile.GetValueString("C"));
			Assert.AreEqual("added", iniFile.GetValueString("e"));
			Assert.AreEqual("AVALF", iniFile.GetValueString("A", "fuffa"));
			Assert.AreEqual("BVALF", iniFile.GetValueString("b", "Fuffa"));
			Assert.AreEqual("CVALF", iniFile.GetValueString("C", "fufFa"));
		}

		[Test]
		public void ItShouldNotBePosssibleToSaveANotInitalizedFile()
		{
			var iniFile = new IniFile(null);
			ArgumentNullException expected = null;
			try
			{
				iniFile.Save();
			}
			catch (ArgumentNullException ex)
			{
				expected = ex;
			}
			Assert.IsNotNull(expected);
		}

		[Test]
		public void ItShouldBePosssibleToSaveAnEmptyFile()
		{
			var projectRoot = Path.Combine(TestFileUtils.GetSolutionRoot(), "ZakCoreUtils.Test");
			var file = Path.Combine(projectRoot, "TestIni.ini");

#if DEBUG
			var destFile = Path.Combine(projectRoot, "bin", "Debug", "TestIni.ini");
#else
			var destFile = Path.Combine(projectRoot,"bin","Release", "TestIni.ini");
#endif
			if (File.Exists(destFile + ".empty")) File.Delete(destFile + ".empty");
			
			var iniFile = new IniFile(null);
			iniFile.Save(destFile + ".empty");
			iniFile.Save(destFile + ".empty");
			var result = File.ReadAllText(destFile + ".empty");
			Assert.IsTrue(result.StartsWith("#Empty"));
		}


		[Test]
		public void ItShouldBePosssibleToSetAFileWithRoot()
		{
			var projectRoot = Path.Combine(TestFileUtils.GetSolutionRoot(), "ZakCoreUtils.Test");
			var projectRoot2 = Path.Combine(TestFileUtils.GetSolutionRoot(), "ZakCore.Coverage");
			var file = Path.Combine(projectRoot, "TestIni.ini");

#if DEBUG
			var destRoot = Path.Combine(projectRoot, "bin", "Debug");
			var destFile = Path.Combine(projectRoot, "bin", "Debug", "TestIni.ini");
#else
			var destRoot = Path.Combine(projectRoot, "bin", "Release");
			var destFile = Path.Combine(projectRoot,"bin","Release", "TestIni.ini");
#endif
			var oriDestRoot = destRoot;
			destRoot += Path.DirectorySeparatorChar;
			if (File.Exists(destFile)) File.Delete(destFile);
			File.Copy(file, destFile);

			var iniFile = new IniFile("TestIni.ini", destRoot,true);
		
			Assert.IsNotNull(iniFile.FileName);

			Assert.AreEqual("AVAL", iniFile.GetValueString("A", "Root"));
			Assert.AreEqual("BVAL", iniFile.GetValueString("b"));
			Assert.AreEqual("CVAL", iniFile.GetValueString("C"));
			Assert.AreEqual("AVALF", iniFile.GetValueString("A", "fuffa"));
			Assert.AreEqual("BVALF", iniFile.GetValueString("b", "Fuffa"));
			Assert.AreEqual("CVALF", iniFile.GetValueString("C", "fufFa"));
			iniFile.SetValue("J","J{root}");
			Assert.IsTrue(iniFile.GetValueString("J").StartsWith("J" + oriDestRoot)||
				iniFile.GetValueString("J").StartsWith("J" + projectRoot2));
		}
	}
}