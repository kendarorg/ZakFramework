using System.Collections.Generic;
using ZakCms.Models.Entitites;

namespace ZakCms.Models
{
	public class PageArticleAdminViewModel : PageArticleViewModel
	{
		public PageArticleAdminViewModel()
		{
			View = "Details";
			AvailableTags = new List<TagModel>();
			Languages = new List<LanguageModel>();
			Companies = new List<CompanyModel>();
		}

		public List<LanguageModel> Languages { get; set; }
		public List<CompanyModel> Companies { get; set; }
		public List<TagModel> AvailableTags { get; set; }
		public string View { get; set; }
	}
}