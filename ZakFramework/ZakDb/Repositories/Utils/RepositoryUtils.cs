using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZakDb.Repositories.Utils
{
	public static class RepositoryUtils
	{
		public static string AddSlashes(string inputTxt)
		{
			// List of characters handled:
			// \000 null
			// \010 backspace
			// \011 horizontal tab
			// \012 new line
			// \015 carriage return
			// \032 substitute
			// \042 double quote
			// \047 single quote
			// \134 backslash
			// \140 grave accent
			if (inputTxt == null) return null;
			return Regex.Replace(inputTxt, @"[\134\140/]", "\\$0").Replace("'", "''");
		}

		public static string StripSlashes(string inputTxt)
		{
			// List of characters handled:
			// \000 null
			// \010 backspace
			// \011 horizontal tab
			// \012 new line
			// \015 carriage return
			// \032 substitute
			// \042 double quote
			// \047 single quote
			// \134 backslash
			// \140 grave accent
			if (inputTxt == null) return null;
			return Regex.Replace(inputTxt, @"(\\)([\134\140/])", "$2").Replace("''", "'");
		}

		public static List<string> FillStringArray(ref List<String> toFill, object toLock, IEnumerable<string> content)
		{
			if (toFill == null)
			{
				lock (toLock)
				{
					toFill = new List<string>();
					if (content != null)
					{
						foreach (var item in content)
						{
							toFill.Add(item);
						}
					}
				}
			}
			return toFill;
		}

		public static string AddWhereParameter(string original, string added)
		{
			if (string.IsNullOrEmpty(original)) return added;
			return string.Format(" ({0}) AND ({1}) ", original, added);
		}

		public static string AddOrderByParameter(string original, string added)
		{
			if (string.IsNullOrEmpty(original)) return added;
			return string.Format(" {0}, {1}", original, added);
		}

		public static string AddSetParameter(string original, string added)
		{
			if (string.IsNullOrEmpty(original)) return added;
			return string.Format(" {0}, {1}", original, added);
		}
	}
}