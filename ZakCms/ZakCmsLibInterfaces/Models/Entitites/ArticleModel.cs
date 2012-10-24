using System;
using System.Collections.Generic;
using ZakCms.Models.AdditionalBehaviours;
using ZakCms.Models.Joins;
using ZakDb.Models;

namespace ZakCms.Models.Entitites
{
	public class ArticleModel : ITreeModel, IElementTimestampModel, IJoinedCompanyModel, IJoinedLanguageModel
	{
		private bool _isFolder;

		public ArticleModel()
		{
			Children = new List<ITreeModel>();
			_isFolder = false;
			ParentId = -1;
			Ordering = -1;
			Id = -1;
			LeftNs = -1;
			RightNs = -1;
			Title = string.Empty;
			Content = string.Empty;
			IsAuthenticated = false;
			UpdateTime = DateTime.Now;
			CreateTime = DateTime.Now;
			Language = new LanguageModel();
			Company = new CompanyModel();
		}

		public DateTime UpdateTime { get; set; }
		public DateTime CreateTime { get; set; }
		public Int64 Id { get; set; }
		public bool IsAuthenticated { get; set; }
		public Int32 Ordering { get; set; }
		public Int64 ParentId { get; set; }
		public Int64 LeftNs { get; set; }
		public Int64 RightNs { get; set; }
		public String Title { get; set; }
		public String SeoTitle { get; set; }
		public String Content { get; set; }
		public List<ITreeModel> Children { get; set; }

		public bool IsFolder
		{
			get { return (_isFolder || Children.Count > 0); }
			set { _isFolder = value; }
		}

		public LanguageModel Language { get; set; }
		public CompanyModel Company { get; set; }
	}
}