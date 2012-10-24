using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakCms.Repositories
{
	public class ArticlesRepository : BaseRepository, IArticlesRepository
	{
		#region Constructors

		public ArticlesRepository(string tableName, String connectionString) :
			this(tableName, connectionString, new List<IRepositoryPlugin>())
		{
		}

		public ArticlesRepository(string tableName, String connectionString, IEnumerable<IRepositoryPlugin> plugins) :
			base(tableName, connectionString, plugins)
		{
		}

		#endregion

		#region Abstract Implementations

		protected override object InstantiateNewItem()
		{
			return new ArticleModel {Id = 0};
		}

		public override void FillFromDb(ZakDataReader reader, object item)
		{
			base.FillFromDb(reader, item);
			((ArticleModel) item).Title = RepositoryUtils.StripSlashes((String) reader["Title"]);
			((ArticleModel) item).Content = RepositoryUtils.StripSlashes((String) reader["Content"]);
			((ArticleModel) item).SeoTitle = RepositoryUtils.StripSlashes((String) reader["SeoTitle"]);
			((ArticleModel) item).IsAuthenticated = (bool) reader["IsAuthenticated"];
		}

		protected override Dictionary<string, object> ConvertToDb(object item)
		{
			var toret = base.ConvertToDb(item);
			toret.Add("Title", RepositoryUtils.AddSlashes(((ArticleModel) item).Title));
			toret.Add("SeoTitle", RepositoryUtils.AddSlashes(((ArticleModel) item).SeoTitle));
			toret.Add("Content", RepositoryUtils.AddSlashes(((ArticleModel) item).Content));
			toret.Add("IsAuthenticated", ((ArticleModel) item).IsAuthenticated ? 1 : 0);
			return toret;
		}

		protected override void InitializeUpdatableFields(List<string> updatableFields)
		{
			base.InitializeUpdatableFields(updatableFields);
			updatableFields.Add("Title");
			updatableFields.Add("SeoTitle");
			updatableFields.Add("Content");
			updatableFields.Add("IsAuthenticated");
		}

		protected override void InitializeSelectableFields(List<string> selectableFields)
		{
			base.InitializeSelectableFields(selectableFields);
			selectableFields.Add("Title");
			selectableFields.Add("SeoTitle");
			selectableFields.Add("Content");
			selectableFields.Add("IsAuthenticated");
		}

		public object GetBySeoTitle(string seoTitle)
		{
			var qo = new QueryObject
				{WhereCondition = string.Format("{0}.SeoTitle='{1}'", TableName, RepositoryUtils.AddSlashes(seoTitle))};
			return GetFirst(qo);
		}

		#endregion
	}
}