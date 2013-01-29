using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using ZakDb.Models;
using ZakDb.Plugins;
using ZakDb.Repositories.Exceptions;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakDb.Repositories
{
	public abstract class BaseRepository : IRepository
	{
		#region Privates

		private readonly string _tableName;
		private readonly String _connectionString;
		private readonly List<IRepositoryPlugin> _repositoryPlugins;

		#endregion

		#region Internals

		protected BaseRepository(string tableName, string connectionString) :
			this(tableName, connectionString, new List<IRepositoryPlugin>())
		{
		}

		public void AddPlugin(IRepositoryPlugin item)
		{
			if (item != null)
			{
				item.Repository = this;
				_repositoryPlugins.Add(item);
				item.RegisterActions();
			}
		}

		protected BaseRepository(string tableName, string connectionString, IEnumerable<IRepositoryPlugin> repositoryPlugins)
		{
			_tableName = tableName;
			_connectionString = connectionString;
			_repositoryPlugins = new List<IRepositoryPlugin>();
			foreach (var item in repositoryPlugins)
			{
				if (item != null)
				{
					AddPlugin(item);
				}
			}
			InitializeUpdatableAndSelectable();
		}

		public Object ExecuteSql(QueryObject query)
		{
			SetupFieldsAndJoins(query);

			var intercalar = AdaptQueryAction(query);
			var queryToExecute =
				string.Format("{0} {1} {2} {3} {4}",
				              query.Action,
				              intercalar,
				              query.QueryBody,
				              string.IsNullOrEmpty(query.WhereCondition)
					              ? string.Empty
					              : string.Format("WHERE {0}", query.WhereCondition),
				              string.IsNullOrEmpty(query.OrderByCondition)
					              ? string.Empty
					              : string.Format("ORDER BY {0}", query.OrderByCondition)
					);


			Object result = CalculateQueryResult(query, queryToExecute);

			return result;
		}

		private object CalculateQueryResult(QueryObject query, string queryToExecute)
		{
			object result;
			using (var conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				using (var command = new SqlCommand(queryToExecute, conn))
				{
					command.CommandType = CommandType.Text;
					try
					{
						result = CalculateBasedOnQueryType(query, command);
					}
					catch (SqlException exception)
					{
						if (exception.Number == 2601) // Cannot insert duplicate key row in object error 
						{
							// handle duplicate key error 
							throw new RepositoryDuplicateKeyException(GetType().Name, queryToExecute, exception);
						}
						throw; // throw exception if this exception is unexpected 
					}
				}
				conn.Close();
			}
			return result;
		}

		private object CalculateBasedOnQueryType(QueryObject query, SqlCommand command)
		{
			object result = null;
			switch (query.TypeOfQuery)
			{
				case (QueryType.Query):
					{
						result = ExecuteSqlQuery(command);
					}
					break;
				case (QueryType.NonQuery):
					{
						// ReSharper disable RedundantAssignment
						result = false;
						// ReSharper restore RedundantAssignment
						result = command.ExecuteNonQuery();
						if (query.Action != QueryAction.Update)
						{
							result = true;
						}
					}
					break;
				case (QueryType.Scalar):
					{
						result = command.ExecuteScalar();
					}
					break;
			}
			return result;
		}

		private string AdaptQueryAction(QueryObject query)
		{
			string intercalar;
			switch (query.Action)
			{
				case (QueryAction.Select):
					{
						intercalar = string.Format("{0} FROM {1}", query.SelectFields, TableName);
					}
					break;
				case (QueryAction.Update):
					{
						intercalar = string.Format(" {0}", TableName);
						query.OrderByCondition = string.Empty;
						query.SelectFields = string.Empty;
					}
					break;
				case (QueryAction.Insert):
					{
						intercalar = string.Format("INTO {0}", TableName);
						query.WhereCondition = string.Empty;
						query.OrderByCondition = string.Empty;
						query.SelectFields = string.Empty;
					}
					break;
				case (QueryAction.Delete):
					{
						intercalar = string.Format("FROM {0}", TableName);
						query.OrderByCondition = string.Empty;
						query.SelectFields = string.Empty;
					}
					break;
				default:
					throw new NotImplementedException(string.Format("Query Action {0} not allowed",
					                                                query.Action));
			}
			return intercalar;
		}

		protected List<object> ExecuteSqlQuery(SqlCommand command)
		{
			var alm = new List<object>();
			var reader = command.ExecuteReader();
			var dr = new ZakDataReader(reader);
			while (dr.Read())
			{
				var item = CreateItem();
				FillFromDb(dr, item);
				if (item != null)
					alm.Add(item);
			}
			return alm;
		}

		#endregion

		#region Implementations

		public virtual List<object> GetAll(QueryObject query)
		{
			if (query == null) query = new QueryObject();
			return ExecuteSql(query) as List<object>;
		}

		public object GetFirst(QueryObject query)
		{
			var list = GetAll(query);
			if (list == null || list.Count == 0) return null;
			return list[0];
		}

		public virtual object GetById(Int64 id, QueryObject query = null)
		{
			if (query == null)
			{
				query = new QueryObject {UseJoins = true};
			}
			if (string.IsNullOrEmpty(query.WhereCondition))
			{
				query.WhereCondition = string.Format(" {0}.Id={1} ", TableName, id);
			}
			else
			{
				query.WhereCondition = string.Format(" ({0}) AND {1} ", query.WhereCondition,
				                                     string.Format(" {0}.Id={1}", TableName, id));
			}

			return GetFirst(query);
		}

		public virtual Int64 Create(object article)
		{
			using (var scope = new TransactionScope())
			{
				if (!OnExecuteBooleanResultEventHandler(new OnPreCreateEventArgs(article), ref _preCreateHandler)) return -1;
				if (!OnPreCreate(article)) return -1;

				if (!OnExecuteBooleanResultEventHandler(new OnVerifyEventArgs(article, "Create"), ref _verifyHandler)) return -1;
				if (!OnVerify(article, "Create")) return -1;

				object result = ExecuteSql(CreateQuery(article));
				if (result == null) return -1;
				((IModel) article).Id = (Int64) result;

				if (!OnExecuteBooleanResultEventHandler(new OnPostCreateEventArgs(article), ref _postCreateHandler)) return -1;

				scope.Complete();
				return (Int64) result;
			}
		}

		public virtual bool OnPreCreate(object item)
		{
			return true;
		}

		public virtual bool OnPreUpdate(object item)
		{
			return true;
		}

		public virtual bool OnPreDelete(Int64 itemId)
		{
			return true;
		}

		public virtual bool OnVerify(object item, string action)
		{
			return true;
		}

		public virtual int Update(object article)
		{
			using (var scope = new TransactionScope())
			{
				if (!OnExecuteBooleanResultEventHandler(new OnPreUpdateEventArgs(article), ref _preUpdateHandler)) return 0;
				if (!OnPreUpdate(article)) return 0;

				if (!OnVerify(article, "Update")) return 0;

				// ReSharper disable RedundantAssignment
				int updated = 0;
				// ReSharper restore RedundantAssignment
				updated = (int) ExecuteSql(UpdateQuery(article));

				if (!OnExecuteBooleanResultEventHandler(new OnPostUpdateEventArgs(article), ref _postUpdateHandler)) return 0;
				scope.Complete();
				return updated;
			}
		}

		public virtual void Delete(long id)
		{
			using (var scope = new TransactionScope())
			{
				if (!OnExecuteBooleanResultEventHandler(new OnPreDeleteEventArgs(id), ref _preDeleteHandler)) return;
				if (!OnPreDelete(id)) return;
				var qo = new QueryObject
					{
						UseJoins = false,
						TypeOfQuery = QueryType.NonQuery,
						Action = QueryAction.Delete,
						WhereCondition = string.Format("{0}.Id={1}",
						                               TableName,
						                               id)
					};
				ExecuteSql(qo);
				if (!OnExecuteBooleanResultEventHandler(new OnPostDeleteEventArgs(id), ref _postDeleteHandler)) return;
				scope.Complete();
			}
		}

		#endregion

		#region Abstracts

		private void SetupFieldsAndJoins(QueryObject queryObject)
		{
			//queryObject.QueryBody = TableName;
			var joinsList = new List<string>();
			var fieldsList = new List<string>();

			OnExecuteBooleanResultEventHandler(new OnPreSelectEventArgs(queryObject), ref _preSelectHandler);
			SetupQueryForPlugins(queryObject, joinsList, fieldsList);
			if (joinsList.Count > 0)
			{
				queryObject.QueryBody += " LEFT JOIN " + string.Join(" LEFT JOIN ", joinsList);
			}
			for (int fld = 0; fld < SelectableFields.Count && !queryObject.ForceSelectField; fld++)
			{
				fieldsList.Add(TableName + "." + SelectableFields[fld]);
			}
			if (!string.IsNullOrEmpty(queryObject.SelectFields)) fieldsList.Add(queryObject.SelectFields);
			queryObject.SelectFields = string.Join(",", fieldsList);
		}

		private void SetupQueryForPlugins(QueryObject queryObject, List<string> joinsList, List<string> fieldsList)
		{
			foreach (var item in _repositoryPlugins)
			{
				var jrep = item as IJoinRepositoryPlugin;

				if (jrep != null)
				{
					if (queryObject.UseJoins)
					{
						SetupJoins(queryObject, joinsList, fieldsList, jrep);
					}
					else if (!queryObject.ForceSelectField)
					{
						SetupStandardFields(fieldsList, jrep);
					}
				}
			}
		}

		private void SetupStandardFields(List<string> fieldsList, IJoinRepositoryPlugin jrep)
		{
			for (int fld = 0; fld < jrep.UpdatableFields.Count; fld++)
			{
				fieldsList.Add(TableName + "." + jrep.UpdatableFields[fld]);
			}
		}

		private void SetupJoins(QueryObject queryObject, List<string> joinsList, List<string> fieldsList,
		                        IJoinRepositoryPlugin jrep)
		{
			var results = new List<string>();
			for (int fld = 0; fld < jrep.JoinableFields.Count; fld++)
			{
				string fieldOnRemote = jrep.JoinableFields[fld];
				string fieldOnLocal = jrep.JoinableFieldsLocal[fld];
				if (!queryObject.ForceSelectField)
					fieldsList.Add(TableName + "." + fieldOnLocal);
				results.Add(TableName + "." + fieldOnLocal + "=" + jrep.JoinedRepository.TableName + "." + fieldOnRemote);
			}
			joinsList.Add(jrep.JoinedRepository.TableName + " ON (" + string.Join(" AND ", results) + ")");

			for (int fld = 0; fld < jrep.JoinedRepository.SelectableFields.Count; fld++)
			{
				fieldsList.Add(jrep.JoinedRepository.TableName + "." +
				               jrep.JoinedRepository.SelectableFields[fld] + " AS " +
				               jrep.JoinedRepository.TableName + "_" +
				               jrep.JoinedRepository.SelectableFields[fld]);
			}
		}

		protected virtual QueryObject UpdateQuery(object article)
		{
			//TODO for sqlserver update u set a=b FROM c inner join d on c.a=d.b 
			//TODO for mysql update u c inner join d on c.a=d.b set a=b
			var qo = new QueryObject
				{
					UseJoins = false,
					Action = QueryAction.Update,
					TypeOfQuery = QueryType.NonQuery
				};
			Dictionary<string, object> converted = ConvertToDb(article);
			string updateQuery = string.Empty;
			int counter = 0;

			var updatableFields = new List<string>();
			updatableFields.AddRange(UpdatableFields);
			for (int index = 0; index < _repositoryPlugins.Count; index++)
			{
				var item = _repositoryPlugins[index];
				updatableFields.AddRange(item.UpdatableFields);
			}

			for (int index = 0; index < updatableFields.Count; index++)
			{
				string fieldName = updatableFields[index];
				if (fieldName != "Id")
				{
					if (counter > 0) updateQuery += ",";
					object ob = converted[fieldName];
					updateQuery += ExtractUpdateQueryParameters(fieldName, ob);
					counter++;
				}
			}
			qo.QueryBody = string.Format(" SET {0}", updateQuery);
			qo.WhereCondition = string.Format(" {0}.Id={1} ",
			                                  TableName,
			                                  ((IModel) article).Id);
			return qo;
		}

		private static string ExtractUpdateQueryParameters(string fieldName, object ob)
		{
			if (ob == null)
			{
				return string.Format("{0}=NULL", fieldName);
			}

			if (ob is AsIsParameter)
			{
				return string.Format("{0}={1}", fieldName, ob.ToString().Trim());
			}
			if (ob is string)
			{
				return string.Format("{0}='{1}'", fieldName, ob.ToString().Trim());
			}
			if (ob is DateTime)
			{
				return string.Format("{0}='{1}'", fieldName, ((DateTime) ob).ToString("yyyy-MM-ddTHH:mm:ss.fff"));
			}
			return string.Format("{0}={1}", fieldName, ob);
		}

		protected virtual QueryObject CreateQuery(object article)
		{
			Dictionary<string, object> converted = ConvertToDb(article);
			var qo = new QueryObject
				{
					Action = QueryAction.Insert,
					UseJoins = false,
					TypeOfQuery = QueryType.Scalar
				};

			string fieldsName = string.Empty;
			string fieldsContent = string.Empty;
			int counter = 0;
			var updatableFields = new List<string>();
			updatableFields.AddRange(UpdatableFields);
			for (int index = 0; index < _repositoryPlugins.Count; index++)
			{
				var item = _repositoryPlugins[index];
				updatableFields.AddRange(item.UpdatableFields);
			}

			for (int index = 0; index < updatableFields.Count; index++)
			{
				var fieldName = updatableFields[index];
				if (fieldName != "Id")
				{
					if (counter > 0)
					{
						fieldsName += ",";
						fieldsContent += ",";
					}
					fieldsName += fieldName;
					object ob = converted[fieldName];
					if (ob == null)
					{
						fieldsContent += string.Format("NULL");
					}
// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
					else if (ob is string)
// ReSharper restore CanBeReplacedWithTryCastAndCheckForNull
					{
						fieldsContent += string.Format("'{0}'", ((string) ob).Trim());
					}
					else if (ob is DateTime)
					{
						fieldsContent += string.Format("'{0}'", ((DateTime) ob).ToString("yyyy-MM-ddTHH:mm:ss.fff"));
					}
					else
					{
						fieldsContent += string.Format("{0}", ob);
					}
					counter++;
				}
			}
			qo.QueryBody = string.Format("({0}) OUTPUT INSERTED.Id VALUES ({1})",
			                             fieldsName,
			                             fieldsContent);
			return qo;
		}


		public PluginOutcome ExecuteAction(string actionName, params object[] pars)
		{
			actionName = actionName.ToLowerInvariant().Trim();
			using (var scope = new TransactionScope())
			{
				var outcome = OnExecuteActionEventHandler(new OnExecuteActionEventArgs(actionName, pars), ref _executeActionHandler);
				if (outcome != null)
				{
					scope.Complete();
					return outcome;
				}

				var outerOutcom = ExecuteActionInternal(actionName, pars);
				if (outerOutcom != null)
				{
					scope.Complete();
					return outerOutcom;
				}
				return new PluginOutcome();
			}
		}

		protected virtual PluginOutcome ExecuteActionInternal(string actionName, object[] pars)
		{
			return null;
		}

		protected virtual Dictionary<string, object> ConvertToDb(object item)
		{
			var toret = new Dictionary<string, object> {{"Id", ((IModel) item).Id}};
			foreach (var itemrep in _repositoryPlugins)
			{
				itemrep.ConvertToDb(item, toret);
			}
			return toret;
		}

		public virtual void FillFromDb(ZakDataReader reader, object item)
		{
			foreach (var itemrep in _repositoryPlugins)
			{
				itemrep.FillFromDb(reader, item);
			}
			((IModel) item).Id = (Int64) reader["Id"];
		}

		public object CreateItem()
		{
			object toret = InstantiateNewItem();
			OnExecuteBooleanResultEventHandler(new OnPostInstantiateEventArgs(toret), ref _postInstantiateHandler);
			return toret;
		}

		protected abstract object InstantiateNewItem();

		public virtual string TableName
		{
			get { return _tableName; }
			set { throw new NotSupportedException(); }
		}

		private void InitializeUpdatableAndSelectable()
		{
			var updatableFields = new List<string>();
			var selectableFields = new List<string>();
			InitializeUpdatableFields(updatableFields);
			InitializeSelectableFields(selectableFields);
			_updatableFields = updatableFields;
			_selectableFields = selectableFields;
		}

		protected virtual void InitializeUpdatableFields(List<string> updatableFields)
		{
		}

		protected virtual void InitializeSelectableFields(List<string> selectableFields)
		{
			selectableFields.Add("Id");
			for (int index = 0; index < _repositoryPlugins.Count; index++)
			{
				var itemrep = _repositoryPlugins[index];
				var item = itemrep as IJoinRepositoryPlugin;
				if (item == null)
				{
					selectableFields.AddRange(itemrep.SelectableFields);
				}
			}
		}

		private List<string> _selectableFields;
		private List<string> _updatableFields;

		public virtual List<string> UpdatableFields
		{
			get { return _updatableFields; }
		}

		public virtual List<string> SelectableFields
		{
			get { return _selectableFields; }
		}

		#endregion

		private bool OnExecuteBooleanResultEventHandler(EventArgs obj, ref BooleanResultEventHandler eventToExecute)
		{
			if (eventToExecute != null && eventToExecute.GetInvocationList().Length > 0)
			{
				foreach (BooleanResultEventHandler elementToInvoke in eventToExecute.GetInvocationList())
				{
					try
					{
						if (!elementToInvoke(this, obj)) return false;
					}
					catch (Exception)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		private PluginOutcome OnExecuteActionEventHandler(EventArgs obj, ref ExecuteActionEventHandler eventToExecute)
		{
			if (eventToExecute != null && eventToExecute.GetInvocationList().Length > 0)
			{
				foreach (ExecuteActionEventHandler elementToInvoke in eventToExecute.GetInvocationList())
				{
					try
					{
						var outcome = elementToInvoke(this, obj);
						if (outcome != null) return outcome;
					}
					catch (Exception)
					{
						return null;
					}
				}
				return null;
			}
			return null;
		}

		// ReSharper disable InconsistentNaming
		private event BooleanResultEventHandler _verifyHandler;
		private event BooleanResultEventHandler _preCreateHandler;
		private event BooleanResultEventHandler _postCreateHandler;
		private event BooleanResultEventHandler _postInstantiateHandler;
		private event ExecuteActionEventHandler _executeActionHandler;
		private event BooleanResultEventHandler _preDeleteHandler;
		private event BooleanResultEventHandler _postDeleteHandler;
		private event BooleanResultEventHandler _preUpdateHandler;
		private event BooleanResultEventHandler _postUpdateHandler;
		private event BooleanResultEventHandler _preSelectHandler;

		// ReSharper restore InconsistentNaming

		event BooleanResultEventHandler IRepository.VerifyHandler
		{
			add { _verifyHandler += value; }
			remove { _verifyHandler -= value; }
		}

		event BooleanResultEventHandler IRepository.PreCreateHandler
		{
			add { _preCreateHandler += value; }
			remove { _preCreateHandler -= value; }
		}

		event BooleanResultEventHandler IRepository.PostCreateHandler
		{
			add { _postCreateHandler += value; }
			remove { _postCreateHandler -= value; }
		}

		event BooleanResultEventHandler IRepository.PostInstantiateHandler
		{
			add { _postInstantiateHandler += value; }
			remove { _postInstantiateHandler -= value; }
		}

		event ExecuteActionEventHandler IRepository.ExecuteActionHandler
		{
			add { _executeActionHandler += value; }
			remove { _executeActionHandler -= value; }
		}

		event BooleanResultEventHandler IRepository.PreDeleteHandler
		{
			add { _preDeleteHandler += value; }
			remove { _preDeleteHandler -= value; }
		}

		event BooleanResultEventHandler IRepository.PostDeleteHandler
		{
			add { _postDeleteHandler += value; }
			remove { _postDeleteHandler -= value; }
		}

		event BooleanResultEventHandler IRepository.PreUpdateHandler
		{
			add { _preUpdateHandler += value; }
			remove { _preUpdateHandler -= value; }
		}

		event BooleanResultEventHandler IRepository.PostUpdateHandler
		{
			add { _postUpdateHandler += value; }
			remove { _postUpdateHandler -= value; }
		}

		event BooleanResultEventHandler IRepository.PreSelectHandler
		{
			add { _preSelectHandler += value; }
			remove { _preSelectHandler -= value; }
		}
	}
}