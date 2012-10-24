using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakCms.Repositories
{
	public class FeedsContentRepository : BaseRepository, IFeedsContentRepository
	{
		#region Constructors

		public FeedsContentRepository(string tableName, String connectionString) :
			this(tableName, connectionString, new List<IRepositoryPlugin>())
		{
		}

		public FeedsContentRepository(string tableName, String connectionString, IEnumerable<IRepositoryPlugin> plugins) :
			base(tableName, connectionString, plugins)
		{
		}

		#endregion

		#region Abstract Implementations

		protected override object InstantiateNewItem()
		{
			return new FeedContentModel {Id = 0};
		}

		public override void FillFromDb(ZakDataReader reader, object item)
		{
			base.FillFromDb(reader, item);
			((FeedContentModel) item).Title = RepositoryUtils.StripSlashes((String) reader["Title"]);
			((FeedContentModel) item).Content = RepositoryUtils.StripSlashes((String) reader["Content"]);
			((FeedContentModel) item).SeoTitle = RepositoryUtils.StripSlashes((String) reader["SeoTitle"]);
			((FeedContentModel) item).FeedId = (Int64) reader["FeedId"];
			((FeedContentModel) item).SourceId = (Int64) reader["SourceId"];
			((FeedContentModel) item).SourceType = (Int64) reader["SourceType"];
		}

		protected override Dictionary<string, object> ConvertToDb(object item)
		{
			var toret = base.ConvertToDb(item);
			toret.Add("Title", RepositoryUtils.AddSlashes(((FeedContentModel) item).Title));
			toret.Add("SeoTitle", RepositoryUtils.AddSlashes(((FeedContentModel) item).SeoTitle));
			toret.Add("Content", RepositoryUtils.AddSlashes(((FeedContentModel) item).Content));
			toret.Add("FeedId", ((FeedContentModel) item).FeedId);
			toret.Add("SourceId", ((FeedContentModel) item).SourceId);
			toret.Add("SourceType", ((FeedContentModel) item).SourceType);
			return toret;
		}

		protected override void InitializeUpdatableFields(List<string> updatableFields)
		{
			base.InitializeUpdatableFields(updatableFields);
			updatableFields.Add("Title");
			updatableFields.Add("SeoTitle");
			updatableFields.Add("Content");
			updatableFields.Add("FeedId");
			updatableFields.Add("SourceId");
			updatableFields.Add("SourceType");
		}

		protected override void InitializeSelectableFields(List<string> selectableFields)
		{
			base.InitializeSelectableFields(selectableFields);
			selectableFields.Add("Title");
			selectableFields.Add("SeoTitle");
			selectableFields.Add("Content");
			selectableFields.Add("FeedId");
			selectableFields.Add("SourceId");
			selectableFields.Add("SourceType");
		}

		#endregion

		public List<object> GetByFeedId(long feedId)
		{
			return GetAll(new QueryObject
				{
					WhereCondition = string.Format("{0}.FeedId={1}", TableName, feedId),
					OrderByCondition = string.Format("{0}.UpdateTime DESC", TableName)
				});
		}
	}
}