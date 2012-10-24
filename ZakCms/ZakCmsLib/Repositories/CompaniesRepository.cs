using System.Collections.Generic;
using ZakCms.Models.Entitites;
using ZakDb.Plugins;
using ZakDb.Repositories;

namespace ZakCms.Repositories
{
	public class CompaniesRepository : ListOfValuesRepository, ICompaniesRepository
	{
		public CompaniesRepository(string tableName, string connectionString) :
			this(tableName, connectionString, new List<IRepositoryPlugin>())
		{
		}

		public CompaniesRepository(string tableName, string connectionString, IEnumerable<IRepositoryPlugin> repositoryPlugins)
			:
				base(tableName, connectionString, repositoryPlugins)
		{
		}

		#region Abstract Implementations

		protected override object InstantiateNewItem()
		{
			return new CompanyModel();
		}

		#endregion
	}
}