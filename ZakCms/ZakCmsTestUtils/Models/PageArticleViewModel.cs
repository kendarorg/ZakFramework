using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;
using ZakDb.Models;

namespace ZakCms.Models
{
	public class PageArticleViewModel : IModel
	{
		public PageArticleViewModel()
		{
			Articles = new List<ArticleModel>();
			Tags = new List<TagModel>();
		}

		public List<ArticleModel> Articles { get; private set; }
		public List<TagModel> Tags { get; private set; }
		public ArticleModel Article { get; set; }

		public Int64 Id { get; set; }
	}
}