using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;

namespace ZakCms.Models
{
	public class PageFeedAdminViewModel
	{
		public PageFeedAdminViewModel()
		{
			View = "Details";
			Feeds = new List<FeedModel>();
			Tags = new List<TagModel>();
			AvailableTags = new List<TagModel>();
			Languages = new List<LanguageModel>();
			Companies = new List<CompanyModel>();
		}

		public List<LanguageModel> Languages { get; set; }
		public List<CompanyModel> Companies { get; set; }
		public List<TagModel> AvailableTags { get; set; }
		public List<FeedModel> Feeds { get; private set; }
		public List<TagModel> Tags { get; private set; }
		public FeedModel Feed { get; set; }

		public Int64 Id { get; set; }
		public string View { get; set; }
	}
}