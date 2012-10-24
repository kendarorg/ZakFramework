using System;
using System.Text.RegularExpressions;

namespace ZakWeb.Utils.Commons
{
	public static class SeoUtils
	{
		public static string BuildSeoFriendlyName(string title)
		{
			if (String.IsNullOrEmpty(title)) return "";

			// remove entities
			title = Regex.Replace(title, @"&\w+;", "");
			// remove anything that is not letters, numbers, dash, or space
			title = Regex.Replace(title, @"[^A-Za-z0-9\-\s]", "");
			// remove any leading or trailing spaces left over
			title = title.Trim();
			// replace spaces with single dash
			title = Regex.Replace(title, @"\s+", "-");
			// if we end up with multiple dashes, collapse to single dash            
			title = Regex.Replace(title, @"\-{2,}", "-");
			// make it all lower case
			title = title.ToLower();
			// if it's too long, clip it
			if (title.Length > 80)
				title = title.Substring(0, 79);
			// remove trailing dash, if there is one
			if (title.EndsWith("-"))
				title = title.Substring(0, title.Length - 1);
			return title;
		}
	}
}