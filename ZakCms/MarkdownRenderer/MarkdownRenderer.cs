using System.Collections.Generic;
using System.Text.RegularExpressions;
using MarkdownSharp;
using ZakWeb.Utils.Renderers;

namespace ZakWeb.Renderers
{
	public class MarkdownRenderer : IRenderer
	{
		private readonly Markdown _markdown = new Markdown();

		private static readonly List<Regex> _regexes;
		private static readonly List<string> _replacements;

		private static string BuildRegexLink(string one, string mid, string two)
		{
			return string.Format("(\\[{0}={1})([^ ]{{1,}})( {2}=)([^]]{{1,}})(])", one, mid, two);
		}

		static MarkdownRenderer()
		{
			_regexes = new List<Regex>();
			_replacements = new List<string>();
			_regexes.Add(new Regex(BuildRegexLink("IMG", "http://", "ALT")));
			_replacements.Add("<img src=\"http://$2\" alt=\"$4\"/>");
			_regexes.Add(new Regex(BuildRegexLink("IMG", "https://", "ALT")));
			_replacements.Add("<img src=\"https://$2\" alt=\"$4\"/>");
			_regexes.Add(new Regex(BuildRegexLink("IMG", "", "ALT")));
			_replacements.Add("<img src=\"##IMAGES_ROOT##/$2\" alt=\"$4\"/>");

			_regexes.Add(new Regex(BuildRegexLink("LNK", "http://", "TXT")));
			_replacements.Add("<a href=\"http://$2\">$4</a>");
			_regexes.Add(new Regex(BuildRegexLink("LNK", "https://", "TXT")));
			_replacements.Add("<a href=\"https://$2\">$4</a>");
			_regexes.Add(new Regex(BuildRegexLink("LNK", "", "TXT")));
			_replacements.Add("<a href=\"##SITE_ROOT##/$2\">$4</a>");
		}

		public string Render(string toRender, string siteRoot, string imagesRoot)
		{
			for (int i = 0; i < _regexes.Count; i++)
			{
				Regex regex = _regexes[i];
				string replacement = _replacements[i];
				toRender = regex.Replace(toRender, replacement);
			}
			toRender = toRender.Replace("##SITE_ROOT##", siteRoot);
			toRender = toRender.Replace("##IMAGES_ROOT##", imagesRoot);
			return _markdown.Transform(toRender);
		}
	}
}