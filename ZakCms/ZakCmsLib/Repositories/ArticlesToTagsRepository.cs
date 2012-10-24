using System.Collections.Generic;
using ZakDb.Plugins;
using ZakDb.Repositories;

namespace ZakCms.Repositories
{
	public class ArticlesToTagsRepository : ManyToManyRepository, IArticlesToTagsRepository
	{
		public ArticlesToTagsRepository(string tableName, string cmsDb, List<IRepositoryPlugin> pluginsList,
		                                IRepository dataRepository)
			: base(tableName, cmsDb, pluginsList, dataRepository)
		{
		}

		protected override bool UpdateMainOnChange
		{
			get { return false; }
		}
	}
}