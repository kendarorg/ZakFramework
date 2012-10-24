using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;

namespace ZakCms.Models
{
	public class PageLanguageAdminViewModel
	{
		public PageLanguageAdminViewModel()
		{
			View = "Details";
			Languages = new List<LanguageModel>();
		}

		public List<LanguageModel> Languages { get; private set; }
		public LanguageModel Language { get; set; }

		public Int64 Id { get; set; }
		public string View { get; set; }
	}
}