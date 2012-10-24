using System;
using ZakCms.Models.AdditionalBehaviours;
using ZakCms.Models.Joins;

namespace ZakCms.Models.Entitites
{
	public class FeedModel : IJoinedLanguageModel, IJoinedCompanyModel, IElementTimestampModel
	{
		public FeedModel()
		{
			CreateTime = DateTime.Now;
			UpdateTime = DateTime.Now;
			Language = new LanguageModel();
			Company = new CompanyModel();
			SeoTitle = string.Empty;
			Title = string.Empty;
		}

		public Int64 Id { get; set; }
		public String Title { get; set; }
		public String SeoTitle { get; set; }


		public LanguageModel Language { get; set; }
		public CompanyModel Company { get; set; }

		public DateTime CreateTime { get; set; }
		public DateTime UpdateTime { get; set; }
	}
}