using System;
using ZakDb.Descriptors;
using ZakDb.Queries;

namespace ZakDb.Services
{
	public interface IDbDriver:IDisposable
	{
		string ConnectionString { get; set; }
		IQueryCreator QueryCreator { get; set; }
		bool Verify(TableDescriptor tableDescriptor, bool throwOnError = false);
		void CreateDb(string dbName);
		void CreateTable(TableDescriptor table);
		bool SupportJoins { get; }
		bool SupportJson { get; }
		bool SupportKeyValueCollections { get; }
	}
}
