using System.Data;
using System.Data.SQLite;
using ZakDb.Descriptors;
using ZakDb.Queries;
using ZakDb.Services;

namespace SqlLiteDb
{
	public class SqLiteDbDriver : IDbDriver
	{
		public string ConnectionString { get; set; }
		public IQueryCreator QueryCreator { get; set; }
		public SqLiteDbDriver(string connectionString)
		{
			ConnectionString = connectionString;
			QueryCreator = new SqLiteQueryCreator();
		}

		public bool Verify(TableDescriptor tableDescriptor, bool throwOnError = false)
		{
			return QueryCreator.Validate(tableDescriptor, throwOnError);
		}

		public void CreateDb(string dbName)
		{
			SQLiteConnection connection = null;
			try
			{
				connection = new SQLiteConnection(ConnectionString);
				connection.Open();
				var version = connection.ServerVersion;
			}
			finally
			{
				if (connection != null) connection.Close();
			}
		}

		public void CreateTable(TableDescriptor table)
		{
			SQLiteConnection connection = null;
			try
			{
				connection = new SQLiteConnection(ConnectionString);
				connection.Open();
				QueryCreator.Validate(table, true);
				var createQuery = QueryCreator.CreateTableQuery<string>(table);
				var cmd = new SQLiteCommand(createQuery, connection);
				cmd.ExecuteNonQuery();
			}
			finally
			{
				if (connection != null) connection.Close();
			}
		}

	}
}
