using ZakDb.Descriptors;
using ZakDb.Queries;
using ZakDb.Services;

namespace MongoDbDb
{
	public class MongoDbDriver : IDbDriver
	{
		public string ConnectionString { get; set; }
		public IQueryCreator QueryCreator { get; set; }
		public bool Verify(TableDescriptor tableDescriptor, bool throwOnError = false)
		{
			return true;
		}

		public void CreateDb(string dbName)
		{
			throw new System.NotImplementedException();
		}

		public void CreateTable(TableDescriptor table)
		{
			throw new System.NotImplementedException();
		}
	}
}
