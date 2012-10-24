using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;

namespace ZakCms.Models
{
	public class PageTagAdminViewModel
	{
		public PageTagAdminViewModel()
		{
			View = "Details";
			Tags = new List<TagModel>();
		}

		public List<TagModel> Tags { get; private set; }
		public TagModel Tag { get; set; }

		public Int64 Id { get; set; }
		public string View { get; set; }
	}
}