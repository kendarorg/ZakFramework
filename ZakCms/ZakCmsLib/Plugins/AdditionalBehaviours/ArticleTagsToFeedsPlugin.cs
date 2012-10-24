using ZakCms.Repositories;
using ZakDb.Models;
using ZakDb.Repositories;

namespace ZakCms.Plugins.AdditionalBehaviours
{
	public class ArticleTagsToFeedsPlugin : ArticlesToFeedsPlugin
	{
		public ArticleTagsToFeedsPlugin(
			IManyToManyRepository articlesToTagsRepository,
			IFeedsRepository feedsRepository,
			IFeedsContentRepository feedsContentRepository,
			IArticlesRepository articlesRepository,
			IFeedsToTagsRepository feedsToTagsRepository) :
				base(articlesToTagsRepository, feedsRepository, feedsContentRepository, articlesRepository, feedsToTagsRepository)
		{
		}

		protected override IModel RetrieveFeedModel(IModel sourceModelItem)
		{
			var tm = (ManyToManyModel) sourceModelItem;
			return (IModel) _articlesRepository.GetById(tm.LeftId);
		}
	}
}