using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakCms.Repositories
{
	public class FeedsRepository : BaseRepository, IFeedsRepository
	{
		protected override object InstantiateNewItem()
		{
			return new FeedModel();
		}

		public FeedsRepository(string tableName, string connectionString) :
			this(tableName, connectionString, new List<IRepositoryPlugin>())
		{
		}

		public FeedsRepository(string tableName, string connectionString, IEnumerable<IRepositoryPlugin> repositoryPlugins) :
			base(tableName, connectionString, repositoryPlugins)
		{
		}

		#region Abstract Implementations

		public override void FillFromDb(ZakDataReader reader, object item)
		{
			base.FillFromDb(reader, item);
			((FeedModel) item).Title = RepositoryUtils.StripSlashes((String) reader["Title"]);
			((FeedModel) item).SeoTitle = RepositoryUtils.StripSlashes((String) reader["SeoTitle"]);
		}

		protected override Dictionary<string, object> ConvertToDb(object item)
		{
			var toret = base.ConvertToDb(item);
			toret.Add("Title", RepositoryUtils.AddSlashes(((FeedModel) item).Title));
			toret.Add("SeoTitle", RepositoryUtils.AddSlashes(((FeedModel) item).SeoTitle));
			return toret;
		}

		protected override void InitializeUpdatableFields(List<string> updatableFields)
		{
			base.InitializeUpdatableFields(updatableFields);
			updatableFields.Add("Title");
			updatableFields.Add("SeoTitle");
		}

		protected override void InitializeSelectableFields(List<string> selectableFields)
		{
			base.InitializeSelectableFields(selectableFields);
			selectableFields.Add("Title");
			selectableFields.Add("SeoTitle");
		}

		#endregion

		public object GetBySeoTitle(string seoTitle)
		{
			var qo = new QueryObject
				{WhereCondition = string.Format("{0}.SeoTitle='{1}'", TableName, RepositoryUtils.AddSlashes(seoTitle))};
			return GetFirst(qo);
		}
	}
}