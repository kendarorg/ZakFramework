using System;
using System.Data;
using System.Data.SQLite;
using ZakDb.Descriptors;
using ZakDb.Queries;
using ZakDb.Services;

namespace SqlLiteDb
{
	public class SqLiteDbDriver : IDbDriver
	{
		[ThreadStatic] 
		private static SQLiteConnection _connection;

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

		private void CloseConnection()
		{
			try
			{
				if (_connection != null) _connection.Close();
			}
			catch (Exception)
			{
				
			}
		}

		private void OpenConnection()
		{
			try
			{
				if (_connection == null)
				{
					_connection = new SQLiteConnection(ConnectionString);
				}
				if (_connection.State == ConnectionState.Closed)
				{
					_connection.Open();
				}
			}
			catch (Exception)
			{
				CloseConnection();
			}
		}

		public void CreateDb(string dbName)
		{
			
			try
			{
				OpenConnection();
				var version = _connection.ServerVersion;
			}
			finally
			{
				CloseConnection();
			}
		}

		public void CreateTable(TableDescriptor table)
		{
			try
			{
				OpenConnection();
				QueryCreator.Validate(table, true);
				var createQuery = QueryCreator.CreateTableQuery<string>(table);
				var cmd = new SQLiteCommand(createQuery, _connection);
				cmd.ExecuteNonQuery();
			}
			catch(Exception)
			{
				CloseConnection();
			}
		}

		public bool SupportJoins { get { return true; } }
		public bool SupportJson { get { return false; } }
		public bool SupportKeyValueCollections { get { return true; } }

		public void Dispose()
		{
			
		}
	}
}
