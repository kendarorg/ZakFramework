using System;
using System.Collections.Generic;
using ZakDb.Models;
using ZakDb.Repositories;
using ZakDb.Repositories.Exceptions;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakDb.Plugins
{
	public class TreeRepositoryPlugin : IRepositoryPlugin
	{
		private readonly object _lockObj = new object();

		protected virtual bool UseNestedSet
		{
			get { return true; }
		}

		#region Plugin Implementation

		public IRepository Repository { get; set; }

		private List<string> _updatableFields;
		private List<string> _selectableFields;

		public List<string> UpdatableFields
		{
			get
			{
				if (UseNestedSet)
				{
					return RepositoryUtils.FillStringArray(ref _updatableFields, _lockObj,
					                                       new[] {"ParentId", "Ordering", "LeftNs", "RightNs"});
				}
				return RepositoryUtils.FillStringArray(ref _updatableFields, _lockObj, new[] {"ParentId", "Ordering"});
			}
		}

		public List<string> SelectableFields
		{
			get
			{
				if (UseNestedSet)
				{
					return RepositoryUtils.FillStringArray(ref _selectableFields, _lockObj,
					                                       new[] {"ParentId", "Ordering", "LeftNs", "RightNs"});
				}
				return RepositoryUtils.FillStringArray(ref _selectableFields, _lockObj, new[] {"ParentId", "Ordering"});
			}
		}


		public void FillFromDb(ZakDataReader reader, object item)
		{
			((ITreeModel) item).ParentId = (Int64) reader["ParentId"];
			((ITreeModel) item).Ordering = (Int32) reader["Ordering"];
			if (UseNestedSet)
			{
				((ITreeModel) item).LeftNs = (Int64) reader["LeftNs"];
				((ITreeModel) item).RightNs = (Int64) reader["RightNs"];
			}
		}

		public void ConvertToDb(object item, Dictionary<string, object> toFill)
		{
			toFill.Add("ParentId", ((ITreeModel) item).ParentId);
			toFill.Add("Ordering", ((ITreeModel) item).Ordering);
			if (UseNestedSet)
			{
				toFill.Add("LeftNs", ((ITreeModel) item).LeftNs);
				toFill.Add("RightNs", ((ITreeModel) item).RightNs);
			}
		}

		private bool OnPreCreate(object sender, EventArgs eventArgs)
		{
			var ea = (OnPreCreateEventArgs) eventArgs;
			object item = ea.Item;
			var getMaxValue = new QueryObject
				{
					TypeOfQuery = QueryType.Scalar,
					UseJoins = false,
					SelectFields = "MAX(Ordering)",
					ForceSelectField = true,
					WhereCondition = string.Format("{0}.ParentId={1}",
					                               Repository.TableName,
					                               ((ITreeModel) item).ParentId)
				};
			object result = Repository.ExecuteSql(getMaxValue);
			((ITreeModel) item).Ordering = 0;
			if (result != DBNull.Value && result != null)
			{
				((ITreeModel) item).Ordering = (Int32) result;
				((ITreeModel) item).Ordering++;
			}

			if (UseNestedSet)
			{
				var parentItem = Repository.GetById(((ITreeModel) item).ParentId, new QueryObject {UseJoins = false});

				if (parentItem != null)
				{
					var updateParentChildren = new QueryObject
						{
							UseJoins = false,
							TypeOfQuery = QueryType.NonQuery,
							Action = QueryAction.Update,
							QueryBody = string.Format("SET " +
							                          "{0}.LeftNs=({0}.LeftNs+2)," +
							                          "{0}.RightNs=({0}.RightNs+2)",
							                          Repository.TableName),
							WhereCondition = string.Format(" {0}.LeftNs>{1} AND {0}.RightNs>{1}",
							                               Repository.TableName,
							                               ((ITreeModel) parentItem).LeftNs)
						};

					((ITreeModel) item).LeftNs = ((ITreeModel) parentItem).LeftNs + 1;
					((ITreeModel) item).RightNs = ((ITreeModel) parentItem).LeftNs + 2;
					((ITreeModel) parentItem).RightNs += 2;
					if (((ITreeModel) item).ParentId != 0) Repository.Update(parentItem);
					Repository.ExecuteSql(updateParentChildren);
				}
				else
				{
					var getMax = new QueryObject
						{
							UseJoins = false,
							SelectFields = "MAX(RightNs)",
							TypeOfQuery = QueryType.Scalar,
							ForceSelectField = true
						};

					result = Repository.ExecuteSql(getMax);
					Int64 max = 0;
					if (result != null && result != DBNull.Value) max = (Int64) result;
					((ITreeModel) item).LeftNs = max + 1;
					((ITreeModel) item).RightNs = max + 2;
				}
			}
			return true;
		}

		private bool OnPreDelete(object sender, EventArgs eventArgs)
		{
			var ea = (OnPreDeleteEventArgs) eventArgs;
			var id = ea.Id;
			var am = (ITreeModel) Repository.GetById(id, new QueryObject {UseJoins = false});


			var parent = (ITreeModel) Repository.GetById(am.ParentId, new QueryObject {UseJoins = false});
			Int64 maxOrdering = 0;

			object maxOrderingObject = Repository.ExecuteSql(new QueryObject
				{
					UseJoins = false,
					ForceSelectField = true,
					SelectFields = "MAX(Ordering)",
					WhereCondition = string.Format("{0}.ParentId={1}",
					                               Repository.TableName,
					                               parent != null ? parent.ParentId : 0),
					TypeOfQuery = QueryType.Scalar
				});

			if (maxOrderingObject != null)
			{
				maxOrdering = (int) maxOrderingObject + 1;
			}

			var updateQuery = new QueryObject
				{
					UseJoins = false,
					TypeOfQuery = QueryType.NonQuery,
					Action = QueryAction.Update,
					QueryBody = string.Format("SET {0}.ParentId={1}, {0}.Ordering=({2}+{0}.Ordering)",
					                          Repository.TableName,
					                          am.ParentId,
					                          maxOrdering),
					WhereCondition = string.Format("{0}.ParentId={1}",
					                               Repository.TableName,
					                               id)
				};

			Repository.ExecuteSql(updateQuery);
			return true;
		}

		private bool OnPreUpdate(object sender, EventArgs eventArgs)
		{
			var ea = (OnPreUpdateEventArgs) eventArgs;
			object itemToUpdate = ea.Item;
			var item = (ITreeModel) Repository.GetById(((ITreeModel) itemToUpdate).Id, new QueryObject {UseJoins = false});
			if (((ITreeModel) itemToUpdate).Ordering == -1) ((ITreeModel) itemToUpdate).Ordering = item.Ordering;
			if (UseNestedSet)
			{
				if (((ITreeModel) itemToUpdate).LeftNs == -1) ((ITreeModel) itemToUpdate).LeftNs = item.LeftNs;
				if (((ITreeModel) itemToUpdate).RightNs == -1) ((ITreeModel) itemToUpdate).RightNs = item.RightNs;
			}
			return true;
		}

		#endregion

		#region Specific Function

		private void UpdateOrderingIfNeeded(object selectedElement, object item)
		{
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (selectedElement != null)
				// ReSharper restore CompareNonConstrainedGenericWithNull
			{
				int tmp = ((ITreeModel) selectedElement).Ordering;
				((ITreeModel) selectedElement).Ordering = ((ITreeModel) item).Ordering;
				((ITreeModel) item).Ordering = tmp;

				Repository.Update(selectedElement);
				Repository.Update(item);
			}
		}

		public void MoveUp(object item)
		{
			var childrens = GetChildren(((ITreeModel) item).ParentId);
			ITreeModel selectedElement = null;
			var imin = Int32.MinValue;
			foreach (var child in childrens)
			{
				if (((ITreeModel) child).Id != ((ITreeModel) item).Id &&
				    ((ITreeModel) child).Ordering < ((ITreeModel) item).Ordering && ((ITreeModel) child).Ordering > imin)
				{
					imin = ((ITreeModel) child).Ordering;
					selectedElement = (ITreeModel) child;
				}
			}
			UpdateOrderingIfNeeded(selectedElement, item);
		}

		public void MoveDown(object item)
		{
			var childrens = GetChildren(((ITreeModel) item).ParentId);
			ITreeModel selectedElement = null;
			var imax = Int32.MaxValue;
			foreach (var child in childrens)
			{
				if (((ITreeModel) child).Id != ((ITreeModel) item).Id &&
				    ((ITreeModel) child).Ordering > ((ITreeModel) item).Ordering && ((ITreeModel) child).Ordering < imax)
				{
					imax = ((ITreeModel) child).Ordering;
					selectedElement = (ITreeModel) child;
				}
			}
			UpdateOrderingIfNeeded(selectedElement, item);
		}

		public List<object> GetTree(Int64 id)
		{
			var am = (ITreeModel) Repository.GetById(id);
			if (am == null) am = (ITreeModel) Repository.CreateItem();

			am.Children.Clear();
			foreach (var item in GetChildren(am.Id))
			{
				am.Children.Add((ITreeModel) item);
			}

			ITreeModel par;

			while ((par = ((ITreeModel) Repository.GetById(am.ParentId))) != null)
			{
				foreach (var item in GetChildren(par.Id))
				{
					par.Children.Add((ITreeModel) item);
				}
				foreach (var child in par.Children)
				{
					if (child.Id == am.Id) child.Children.AddRange(am.Children);
				}
				am = par;
			}
			par = (ITreeModel) Repository.CreateItem();

			foreach (var item in GetChildren(par.Id))
			{
				par.Children.Add((ITreeModel) item);
			}
			foreach (var child in par.Children)
			{
				if (child.Id == am.Id) child.Children.AddRange(am.Children);
			}
			var toret = new List<object>();
			foreach (var child in par.Children)
			{
				toret.Add(child);
			}
			return toret;
		}

		public virtual List<object> GetChildren(Int64 id)
		{
			var qo = new QueryObject
				{
					WhereCondition = string.Format("{0}.ParentId={1}", Repository.TableName, id),
					OrderByCondition = string.Format("{0}.Ordering ASC", Repository.TableName)
				};
			return Repository.ExecuteSql(qo) as List<object>;
		}

		public virtual List<object> GetAllChildren(Int64 id)
		{
			if (!UseNestedSet) throw new NotSupportedException("Should  use Nested Set behaviour");
			var current = Repository.GetById(id, new QueryObject {UseJoins = false});

			var qo = new QueryObject
				{
					WhereCondition = string.Format("{0}.LeftNs>{1} && {0}.RightNs<{2}",
					                               Repository.TableName,
					                               ((ITreeModel) current).LeftNs,
					                               ((ITreeModel) current).RightNs)
				};

			return Repository.ExecuteSql(qo) as List<object>;
		}

		#endregion

		private PluginOutcome ExecuteAction(object source, EventArgs args)
		{
			var eaea = (OnExecuteActionEventArgs) args;
			string actionName = eaea.ActionName;
			object[] pars = eaea.Parameters == null ? new object[0] : eaea.Parameters;
			var toret = new PluginOutcome();
			actionName = actionName.ToLowerInvariant();
			switch (actionName)
			{
				case ("movedown"):
					MoveDown(pars[0]);
					toret.Success = true;
					break;
				case ("moveup"):
					MoveUp(pars[0]);
					toret.Success = true;
					break;
				case ("getchildren"):
					toret.Result = GetChildren((long) pars[0]);
					toret.Success = true;
					break;
				case ("gettree"):
					toret.Result = GetTree((long) pars[0]);
					toret.Success = true;
					break;
				case ("getallchildren"):
					toret.Result = GetAllChildren((long) pars[0]);
					toret.Success = true;
					break;
			}
			return toret;
		}

		private bool OnVerify(object sender, EventArgs eventArgs)
		{
			var ea = (OnVerifyEventArgs) eventArgs;
			object item = ea.Item;
			string operation = ea.Operation;

			if (UseNestedSet)
			{
				if (((ITreeModel) item).LeftNs < 0)
					throw new RepositoryValidationException(
						Repository.GetType().Name + "." + GetType().Name,
						operation,
						"LeftNs",
						"field must be > 0");
				if (((ITreeModel) item).RightNs < 0)
					throw new RepositoryValidationException(
						Repository.GetType().Name + "." + GetType().Name,
						operation,
						"RightNs",
						"field must be > 0");
				if (((ITreeModel) item).RightNs <= ((ITreeModel) item).LeftNs)
					throw new RepositoryValidationException(
						Repository.GetType().Name + "." + GetType().Name,
						operation,
						"RightNs",
						"field must be > LeftNs");
			}
			if (((ITreeModel) item).Ordering < 0)
				throw new RepositoryValidationException(
					Repository.GetType().Name + "." + GetType().Name,
					operation,
					"Ordering",
					"field must be >=0");
			if (((ITreeModel) item).ParentId < 0)
				throw new RepositoryValidationException(
					Repository.GetType().Name + "." + GetType().Name,
					operation,
					"ParentId",
					"field must be >=0");
			return true;
		}

		public void RegisterActions()
		{
			Repository.PreUpdateHandler += OnPreUpdate;
			Repository.PreCreateHandler += OnPreCreate;
			Repository.PreDeleteHandler += OnPreDelete;
			Repository.VerifyHandler += OnVerify;
			Repository.ExecuteActionHandler += ExecuteAction;
		}
	}
}