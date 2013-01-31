using ZakDb.Descriptors;
using ZakDb.Queries;

namespace ZakDb.Services
{
	public interface IDbDriver
	{
		string ConnectionString { get; set; }
		IQueryCreator QueryCreator { get; set; }
		bool Verify(TableDescriptor tableDescriptor, bool throwOnError = false);
		void CreateDb(string dbName);
		void CreateTable(TableDescriptor table);
	}
}
