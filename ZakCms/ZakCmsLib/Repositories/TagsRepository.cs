using System.Collections.Generic;
using ZakCms.Models.Entitites;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakCms.Repositories
{
	public class TagsRepository : ListOfValuesRepository, ITagsRepository
	{
		public TagsRepository(string tableName, string connectionString) :
			this(tableName, connectionString, new List<IRepositoryPlugin>())
		{
		}

		public TagsRepository(string tableName, string connectionString, IEnumerable<IRepositoryPlugin> repositoryPlugins) :
			base(tableName, connectionString, repositoryPlugins)
		{
		}

		#region Abstract Implementations

		protected override object InstantiateNewItem()
		{
			return new TagModel();
		}

		#endregion

		public object GetByTagCode(string code)
		{
			return GetFirst(new QueryObject
				{
					UseJoins = false,
					WhereCondition = string.Format("Code='{0}'", RepositoryUtils.AddSlashes(code))
				});
		}
	}
}