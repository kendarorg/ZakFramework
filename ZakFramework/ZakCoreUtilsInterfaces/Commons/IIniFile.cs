using System;
using System.Collections.Generic;

namespace ZakCore.Utils.Commons
{
	public interface IIniFile
	{
		void SetValues(Dictionary<string, object> vals, String section = "root");

		Dictionary<string, object> GetValues(String section = "root");

		IEnumerable<string> GetSections();

		object GetValue(String id, String section = "root");

		string GetValueString(String id, String section = "root");

		void SetValue(String key, object value, String section = "root");

		//public IniFile(string fileName, string setupRoot = null, bool replaceBuild = false)
	}
}