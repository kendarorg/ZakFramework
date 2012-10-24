using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;
using ZakCms.Models.Joins;
using ZakCms.Repositories;
using ZakDb.Models;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakCms.Plugins.AdditionalBehaviours
{
	public abstract class ArticlesFeedInteractionPlugin : IRepositoryPlugin
	{
		protected readonly IManyToManyRepository _articlesToTagsRepository;
		protected readonly IFeedsRepository _feedsRepository;
		protected readonly IFeedsContentRepository _feedsContentRepository;
		protected readonly IArticlesRepository _articlesRepository;
		protected readonly IFeedsToTagsRepository _feedsToTagsRepository;

		protected ArticlesFeedInteractionPlugin(
			IManyToManyRepository articlesToTagsRepository,
			IFeedsRepository feedsRepository,
			IFeedsContentRepository feedsContentRepository,
			IArticlesRepository articlesRepository,
			IFeedsToTagsRepository feedsToTagsRepository)
		{
			_articlesToTagsRepository = articlesToTagsRepository;
			_feedsRepository = feedsRepository;
			_feedsContentRepository = feedsContentRepository;
			_articlesRepository = articlesRepository;
			_feedsToTagsRepository = feedsToTagsRepository;
		}

		public IRepository Repository { get; set; }

		public List<string> UpdatableFields
		{
			get { return new List<string>(); }
		}

		public List<string> SelectableFields
		{
			get { return new List<string>(); }
		}

		/// <summary>
		/// Retrieves the tags that are associated with the leftId (article)
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected abstract List<TagModel> GetTagsForItem(object item);

		/// <summary>
		/// Create a feed content given the item with the content, no db involved
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected abstract FeedContentModel CrateFeedContentModel(object item);

		/// <summary>
		/// Retrieve the type of the element (an id defined boh....1 for article, 2 for news etc)
		/// </summary>
		/// <returns></returns>
		protected abstract Int64 GetSourceType();

		/// <summary>
		/// Given the passed object should return the corresponding feed mode
		/// Retrieves the id of the newly created item, the leftId in case
		/// of the Article. Should understand if it works with the tag or the article
		/// </summary>
		/// <param name="sourceModelItem"></param>
		/// <returns></returns>
		protected abstract IModel RetrieveFeedModel(IModel sourceModelItem);

		private bool OnPostCreate(object source, EventArgs args)
		{
			var modelItem = ((OnPostCreateEventArgs) args).Item;
			return NotifyChangesToFeed(RetrieveFeedModel((IModel) modelItem));
		}

		private bool OnPostUpdate(object source, EventArgs args)
		{
			var modelItem = ((OnPostUpdateEventArgs) args).Item;
			return NotifyChangesToFeed(RetrieveFeedModel((IModel) modelItem));
		}

		protected bool NotifyChangesToFeed(IModel item)
		{
			var tagsForLeftId = GetTagsForItem(item);
			foreach (var el in tagsForLeftId)
			{
				var involvedFeeds = GetFeedsForTag(el, item as IJoinedLanguageModel, item as IJoinedCompanyModel);
				var feedContentModel = CrateFeedContentModel(item);
				foreach (var involvedFeed in involvedFeeds)
				{
					var existingFeedContent = GetExistingFeedContent(involvedFeed.Id, item.Id);
					if (existingFeedContent != null)
					{
						existingFeedContent.SeoTitle = feedContentModel.SeoTitle;
						existingFeedContent.Content = feedContentModel.Content;
						existingFeedContent.Title = feedContentModel.Title;
						_feedsContentRepository.Update(existingFeedContent);
					}
					else
					{
						feedContentModel.Id = 0;
						feedContentModel.FeedId = involvedFeed.Id;
						feedContentModel.SourceId = item.Id;
						feedContentModel.SourceType = GetSourceType();
						_feedsContentRepository.Create(feedContentModel);
					}
				}
			}
			return true;
		}

		private bool OnPostDelete(object source, EventArgs args)
		{
			var itemIdContainingTags = ((OnPostDeleteEventArgs) args).Id;
			var qu = new QueryObject
				{
					Action = QueryAction.Delete,
					TypeOfQuery = QueryType.NonQuery,
					UseJoins = false,
					WhereCondition = string.Format("{0}.SourceId={1}", _feedsContentRepository.TableName, itemIdContainingTags)
				};
			_feedsContentRepository.ExecuteSql(qu);
			return true;
		}

		private FeedContentModel GetExistingFeedContent(long feedId, long sourceId)
		{
			return (FeedContentModel) _feedsContentRepository.GetFirst(new QueryObject
				{
					UseJoins = false,
					WhereCondition = string.Format("FeedId={0} AND SourceId={1}", feedId, sourceId)
				});
		}


		// ReSharper disable ReturnTypeCanBeEnumerable.Local
		private List<FeedModel>
			GetFeedsForTag(TagModel el, IJoinedLanguageModel itemLang, IJoinedCompanyModel itemComp)
		{
			var feeds = _feedsToTagsRepository.GetByOwned(el);
			var ret = new List<FeedModel>();
			foreach (var feed in feeds)
			{
				var qo = new QueryObject();
				if (itemLang != null)
				{
					qo.WhereCondition = RepositoryUtils.AddWhereParameter(qo.WhereCondition,
					                                                      string.Format("LanguageId={0}", itemLang.Language.Id));
				}
				if (itemLang != null)
				{
					qo.WhereCondition = RepositoryUtils.AddWhereParameter(qo.WhereCondition,
					                                                      string.Format("CompanyId={0}", itemComp.Company.Id));
				}
				var realFeed = _feedsRepository.GetById(((ManyToManyModel) feed).LeftId, qo);
				if (realFeed != null)
					ret.Add((FeedModel) realFeed);
			}
			return ret;
		}

		// ReSharper restore ReturnTypeCanBeEnumerable.Local

		public void RegisterActions()
		{
			Repository.PostUpdateHandler += OnPostUpdate;
			Repository.PostCreateHandler += OnPostCreate;
			Repository.PostDeleteHandler += OnPostDelete;
		}


		public void FillFromDb(ZakDataReader reader, object article)
		{
		}

		public void ConvertToDb(object source, Dictionary<string, object> toFill)
		{
		}
	}
}