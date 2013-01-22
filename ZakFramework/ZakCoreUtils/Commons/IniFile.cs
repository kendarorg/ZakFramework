using System;
using System.Collections.Generic;
using System.IO;

namespace ZakCore.Utils.Commons
{
	public class IniFile : IIniFile
	{
		public void SetValues(Dictionary<string, object> vals, String section = "root")
		{
			if (string.IsNullOrEmpty(section))
			{
				section = "root";
			}
			foreach (var el in vals)
			{
				SetValue(el.Key, el.Value, section);
			}
		}

		public Dictionary<string, object> GetValues(String section = "root")
		{
			if (string.IsNullOrEmpty(section))
			{
				section = "ROOT";
			}
			section = section.ToUpper();

			var toret = new Dictionary<string, object>();
			if (_sections.ContainsKey(section))
			{
				foreach (var v in _sections[section.ToUpper()].Values)
				{
					toret.Add(v.Key, v.Value);
				}
			}
			return toret;
		}

		public IEnumerable<string> GetSections()
		{
			foreach (var e in _sections)
			{
				string key = e.Key.ToUpper();
				if (key != "ROOT")
				{
					yield return e.Key.ToUpper();
				}
			}
		}

		public object GetValue(String id, String section = "root")
		{
			if (string.IsNullOrEmpty(section))
			{
				section = "root";
			}
			if (!_sections.ContainsKey(section.ToUpper()))
			{
				return null;
			}
			if (_sections[section.ToUpper()] == null || !_sections[section.ToUpper()].Values.ContainsKey(id.ToUpper()))
			{
				return null;
			}
			return _sections[section.ToUpper()].Values[id.ToUpper()];
		}

		public void SetValue(String key, object value, String section = "root")
		{
			// ReSharper disable PossibleInvalidCastException
			if (string.IsNullOrEmpty(section))
			{
				section = "root";
			}

			if (value is string)
			{
				if (_replaceBuild)
				{
#if DEBUG
					value = ((string)value).Replace("{build}", "Debug");
#elif RELASE
				  value = ((string) value).Replace("{build}", "Release");
#endif
				}
				if (_setupRoot != null)
				{
					value = ((string)value).Replace("{root}", _setupRoot);
				}
			}

			section = section.ToUpper();
			if (!_sections.ContainsKey(section))
			{
				_sections.Add(section, new IniSection());
				_sections[section].Values = new Dictionary<string, object>();
			}
			key = key.ToUpper();
			if (!_sections[section].Values.ContainsKey(key))
			{
				_sections[section].Values.Add(key, value);
			}
			else
			{
				_sections[section].Values[key] = value;
			}
			// ReSharper restore PossibleInvalidCastException
		}

		private class IniSection
		{
			public String Title;
			public Dictionary<string, object> Values;
		}

		private readonly Dictionary<string, IniSection> _sections;

		public virtual bool Save(string fileName = null)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				fileName = _fileName;
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException();
			}
			if (File.Exists(fileName))
			{
				File.Move(fileName, fileName + "." + (DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond) + ".bak");
			}
			_fileName = fileName;
			_setupRoot = Path.GetDirectoryName(_fileName);

			var newIniFile = new List<string>();
			if (_sections != null)
			{
				IniSection se = _sections["ROOT"];
				foreach (var e in se.Values)
				{
					newIniFile.Add(string.Format("{0}={1}", e.Key, e.Value));
				}
				foreach (var s in _sections)
				{
					if (String.Compare(s.Key, "ROOT", StringComparison.Ordinal) != 0)
					{
						newIniFile.Add(string.Format("[{0}]", s.Key.ToUpper()));
						foreach (var e in s.Value.Values)
						{
							newIniFile.Add(string.Format("{0}={1}", e.Key, e.Value));
						}
					}
				}
			}
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			if (newIniFile.Count == 0) newIniFile.Add("#Empty");
			File.WriteAllLines(fileName, newIniFile);
			return true;
		}

		private string _fileName;
		private readonly bool _replaceBuild;
		private string _setupRoot;

		public string FileName
		{
			get { return _fileName; }
		}

		public IniFile(string fileName, string setupRoot = null, bool replaceBuild = false)
		{
			_replaceBuild = false;
			_setupRoot = null;
			_setupRoot = setupRoot;
			if (_setupRoot != null)
			{
				if (_setupRoot.EndsWith("\\") || _setupRoot.EndsWith("/"))
				{
					_setupRoot = _setupRoot.Substring(0, _setupRoot.Length - 1);
				}
			}

			_replaceBuild = replaceBuild;
			_fileName = fileName;
			_sections = new Dictionary<string, IniSection>();
			var allLines = new String[] { };

			_fileName = FileUtils.FindFile(fileName, _setupRoot);

			if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
			{
				allLines = File.ReadAllLines(fileName);
			}
			var ise = new IniSection
				{
					Title = "ROOT",
					Values = new Dictionary<string, object>()
				};
			_sections.Add(ise.Title, ise);

			foreach (string s in allLines)
			{
				String r = s.Trim();
				if (r.Length == 0 || r.StartsWith("#"))
				{
					continue;
				}
				if (r.StartsWith("[") && r.EndsWith("]"))
				{
					ise = new IniSection
						{
							Title = r.Substring(1, r.Length - 2).ToUpper(),
							Values = new Dictionary<string, object>()
						};
					if (!_sections.ContainsKey(ise.Title))
					{
						_sections.Add(ise.Title, ise);
					}
				}
				else if (r.IndexOf('=') > 0)
				{
					String[] bl = r.Split('=');
					if (bl.Length > 2)
					{
						SetValue(bl[0].ToUpper().Trim(), string.Join("=", bl, 1, bl.Length - 1), ise.Title);
					}
					if (bl.Length == 1)
					{
						SetValue(bl[0].ToUpper().Trim(), string.Empty, ise.Title);
					}
					else if (bl.Length == 2)
					{
						SetValue(bl[0].ToUpper().Trim(), bl[1].Trim(), ise.Title);
					}
				}
			}
		}


		public string GetValueString(string id, string section = "root")
		{
			object ob = GetValue(id, section);
			return ob == null ? null : ob.ToString();
		}
	}
}