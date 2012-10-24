using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;

namespace ZakCmsFE.Models
{
	public class FeedAndContentViewModel
	{
		public FeedAndContentViewModel()
		{
			FeedContent = new List<FeedContentModel>();
			Feed = new FeedModel();
			LastBuildDate = DateTime.Now;
		}

		public DateTime LastBuildDate { get; set; }
		public FeedModel Feed { get; set; }
		public List<FeedContentModel> FeedContent { get; set; }
	}
}