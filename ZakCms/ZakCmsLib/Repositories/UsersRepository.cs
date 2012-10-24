using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakCms.Repositories
{
	public class UsersRepository : BaseRepository, IFEUsersRepository
	{
		protected override object InstantiateNewItem()
		{
			return new UserModel();
		}

		public UsersRepository(string tableName, string connectionString) :
			this(tableName, connectionString, new List<IRepositoryPlugin>())
		{
		}

		public UsersRepository(string tableName, string connectionString, IEnumerable<IRepositoryPlugin> repositoryPlugins) :
			base(tableName, connectionString, repositoryPlugins)
		{
		}

		#region Abstract Implementations

		public override void FillFromDb(ZakDataReader reader, object item)
		{
			base.FillFromDb(reader, item);
			((UserModel) item).UserId = RepositoryUtils.StripSlashes((String) reader["UserId"]);
			((UserModel) item).UserPassword = RepositoryUtils.StripSlashes((String) reader["UserPassword"]);
		}

		protected override Dictionary<string, object> ConvertToDb(object item)
		{
			var toret = base.ConvertToDb(item);
			toret.Add("UserId", RepositoryUtils.AddSlashes(((UserModel) item).UserId));
			toret.Add("UserPassword", RepositoryUtils.AddSlashes(((UserModel) item).UserPassword));
			return toret;
		}

		protected override void InitializeUpdatableFields(List<string> updatableFields)
		{
			base.InitializeUpdatableFields(updatableFields);
			updatableFields.Add("UserId");
			updatableFields.Add("UserPassword");
		}

		protected override void InitializeSelectableFields(List<string> selectableFields)
		{
			base.InitializeSelectableFields(selectableFields);
			selectableFields.Add("UserId");
			selectableFields.Add("UserPassword");
		}

		public bool ValidateUser(string uid, string pwd)
		{
			return GetAll(new QueryObject
				{
					UseJoins = false,
					WhereCondition = string.Format("UserId='{0}' AND UserPassword='{1}'",
					                               RepositoryUtils.AddSlashes(uid), RepositoryUtils.AddSlashes(pwd))
				}).Count == 1;
		}

		public object GetUserByUserId(string uid)
		{
			return GetFirst(new QueryObject
				{
					UseJoins = false,
					WhereCondition = string.Format("UserId='{0}'",
					                               RepositoryUtils.AddSlashes(uid))
				});
		}

		#endregion
	}
}