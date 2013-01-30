using System;
using System.Collections.Generic;
using ZakDb.Plugins;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakDb.Repositories
{
	public interface IRepository
	{
		void AddPlugin(IRepositoryPlugin item);
		List<object> GetAll(QueryObject query = null);
		object GetById(Int64 id, QueryObject query = null);
		object CreateItem();
		Int64 Create(object article);
		int Update(object article);
		void Delete(Int64 id);
		string TableName { get; set; }
		Object ExecuteSql(QueryObject query);
		PluginOutcome ExecuteAction(string actionName, params object[] pars);
		List<string> UpdatableFields { get; }
		List<string> SelectableFields { get; }
		bool OnVerify(object article, string action);
		object GetFirst(QueryObject query);

		event BooleanResultEventHandler VerifyHandler;
		event BooleanResultEventHandler PreCreateHandler;
		event BooleanResultEventHandler PostCreateHandler;
		event BooleanResultEventHandler PostInstantiateHandler;
		event ExecuteActionEventHandler ExecuteActionHandler;
		event BooleanResultEventHandler PreDeleteHandler;
		event BooleanResultEventHandler PostDeleteHandler;
		event BooleanResultEventHandler PreUpdateHandler;
		event BooleanResultEventHandler PostUpdateHandler;
		event BooleanResultEventHandler PreSelectHandler;
	}
}