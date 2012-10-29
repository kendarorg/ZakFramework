using System;
using System.Collections.Generic;
using ZakDb.Models;
using ZakDb.Plugins;
using ZakDb.Repositories.Utils;

namespace ZakDb.Repositories
{
	public abstract class ListOfValuesRepository : BaseRepository
	{
		protected ListOfValuesRepository(string tableName, string connectionString) :
			this(tableName, connectionString, new List<IRepositoryPlugin>())
		{
		}

		protected ListOfValuesRepository(string tableName, string connectionString,
		                                 IEnumerable<IRepositoryPlugin> repositoryPlugins) :
			                                 base(tableName, connectionString, repositoryPlugins)
		{
		}

		#region Abstract Implementations

		public override void FillFromDb(ZakDataReader reader, object item)
		{
			base.FillFromDb(reader, item);
			((ILovModel) item).Description = RepositoryUtils.StripSlashes((String) reader["Description"]);
			((ILovModel) item).Code = RepositoryUtils.StripSlashes((String) reader["Code"]);
		}

		protected override Dictionary<string, object> ConvertToDb(object item)
		{
			var toret = base.ConvertToDb(item);
			toret.Add("Description", RepositoryUtils.AddSlashes(((ILovModel) item).Description));
			toret.Add("Code", RepositoryUtils.AddSlashes(((ILovModel) item).Code));
			return toret;
		}

		protected override void InitializeUpdatableFields(List<string> updatableFields)
		{
			base.InitializeUpdatableFields(updatableFields);
			updatableFields.Add("Description");
			updatableFields.Add("Code");
		}

		protected override void InitializeSelectableFields(List<string> selectableFields)
		{
			base.InitializeSelectableFields(selectableFields);
			selectableFields.Add("Description");
			selectableFields.Add("Code");
		}

		#endregion
	}
}