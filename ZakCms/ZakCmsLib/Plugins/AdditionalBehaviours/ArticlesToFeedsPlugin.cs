using System.Collections.Generic;
using ZakCms.Models.Entitites;
using ZakCms.Repositories;
using ZakDb.Models;
using ZakDb.Repositories;

namespace ZakCms.Plugins.AdditionalBehaviours
{
	public class ArticlesToFeedsPlugin : ArticlesFeedInteractionPlugin
	{
		public ArticlesToFeedsPlugin(
			IManyToManyRepository articlesToTagsRepository,
			IFeedsRepository feedsRepository,
			IFeedsContentRepository feedsContentRepository,
			IArticlesRepository articlesRepository,
			IFeedsToTagsRepository feedsToTagsRepository) :
				base(articlesToTagsRepository, feedsRepository, feedsContentRepository, articlesRepository, feedsToTagsRepository)
		{
		}

		protected override List<TagModel> GetTagsForItem(object item)
		{
			var am = (ArticleModel) item;
			var attr = _articlesToTagsRepository.GetByOwner(am.Id);
			var tms = new List<TagModel>();
			foreach (var tm in attr)
			{
				var tmm = (TagModel) ((ManyToManyModel) tm).Content;
				tms.Add(tmm);
			}
			return tms;
		}

		protected override FeedContentModel CrateFeedContentModel(object item)
		{
			var am = (ArticleModel) item;
			return new FeedContentModel
				{
					SourceId = am.Id,
					Content = am.Content,
					SeoTitle = am.SeoTitle,
					Title = am.Title
				};
		}

		protected override long GetSourceType()
		{
			return 1;
		}

		protected override IModel RetrieveFeedModel(IModel sourceModelItem)
		{
			return sourceModelItem;
		}
	}
}