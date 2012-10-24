using System;
using ZakCms.Models.AdditionalBehaviours;

namespace ZakCms.Models.Entitites
{
	public class FeedContentModel : IElementTimestampModel
	{
		public FeedContentModel()
		{
			UpdateTime = DateTime.Now;
			CreateTime = DateTime.Now;
			SeoTitle = string.Empty;
		}

		public Int64 FeedId { get; set; }
		public Int64 SourceId { get; set; }
		public Int64 SourceType { get; set; }
		public String Title { get; set; }
		public String SeoTitle { get; set; }
		public String Content { get; set; }
		public DateTime UpdateTime { get; set; }
		public DateTime CreateTime { get; set; }
		public Int64 Id { get; set; }
	}
}